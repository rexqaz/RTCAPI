using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using rc_interface_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using LTCH_API;
using System.Text;

namespace rc_interface_API.Controllers._01_Project_Management
{
    /// <summary>
    /// Polo
    /// FormLogin
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FormLoginController : ControllerBase
    {
        private string mStrFuncName = "FormLoginController";
        [HttpPost]
        public object FormAll(Info_FormLoginModel login)
        {
            JArray newJa = new JArray();
            JArray newJa_Final = new JArray();

            string account = "";
            if (login.account != null) { account = APCommonFun.CDBNulltrim(login.account); }

            string pwd = "";
            if (login.password != null) { pwd = APCommonFun.CDBNulltrim(login.password); }


            if (pwd.Length <= 0 || account.Length <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "Account and password must be specified.", "R", new JArray());
            }

            OracleConnection conn = APCommonFun.GetOracleConnection();
            if (conn == null)
            {
                return APCommonFun.ReturnError(mStrFuncName, "Oracle connecting fault.", "F", new JArray());
            }

            string sqlcommand = "select a.ACCPWD, a.ACCID, b.ACCOUNTNAME as ACCNAME from ACCOUNTS a left join ACCOUNTINFO b on a.ACCID=b.ACCOUNTID where UPPER(a.accid)= '" + account.ToUpper() + "' or LOWER(a.accid)='" + account.ToLower() + "' ";

            OracleCommand cmd = new OracleCommand(sqlcommand, conn);
            cmd.CommandType = CommandType.Text;
            DataSet _ds = new DataSet();
            int _rec_count = 0;
            
            try
            {
                //var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;

                //var remoteIpAddress = (HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                //   HttpContext.Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();
                //string _client_ip1 = HttpContext.Request.HttpContext.GetServerVariable("HTTP_X_FORWARDED_FOR") ?? HttpContext.Request.HttpContext.GetServerVariable("REMOTE_ADDR");

                string _client_ip1 = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                if (Request.Headers.ContainsKey("HTTP_X_FORWARDED_FOR"))
                    _client_ip1 = Request.Headers["HTTP_X_FORWARDED_FOR"];

                OracleDataAdapter _da = new OracleDataAdapter();
                _da.SelectCommand = cmd;
                _rec_count = _da.Fill(_ds);

                switch(_rec_count)
                {
                    case 0:
                        try { conn.Close(); } catch { }
                        return new
                        {
                            Result = "F",
                            Message = string.Format("AccountNotExist", account),
                            user_id = string.Empty,
                            user_name = string.Empty,
                            logon = "-1"
                        };

                    case 1:
                        string _dbpwd = _ds.Tables[0].Rows[0]["ACCPWD"].ToString().Trim();
                        string __id = _ds.Tables[0].Rows[0]["ACCID"].ToString().Trim();
                        string __name = _ds.Tables[0].Rows[0]["ACCNAME"].ToString().Trim();
                        string _decrypted_pwd = Encoding.UTF8.GetString(Convert.FromBase64String(_dbpwd));  //APCommonFun.DecryptString(_dbpwd);
                        if (_decrypted_pwd.Length <= 0)
                        {
                            //Decoding error
                            try { conn.Close(); } catch { }
                            return new
                            {
                                Result = "F",
                                Message = string.Format("PasswordInCorrect"),
                                user_id = string.Empty,
                                user_name = string.Empty,
                                logon = "-1"
                            };
                        }
                        else
                        {
                            if(_decrypted_pwd == pwd)
                            {
                                //sqlcommand = string.Format("SELECT ACCPWD,ACCID,(Select ACCOUNTNAME From ACCOUNTINFO Where upper(ACCOUNTS.ACCID)=upper(ACCOUNTINFO.ACCOUNTID)) as _ACCNAME FROM ACCOUNTS where Upper(accid)= '{0}'", account);
                                sqlcommand = "SELECT ACCOUNTLOGID.NEXTVAL FROM DUAL";
                                int _nextid = -1;
                                try
                                {
                                    cmd = new OracleCommand(sqlcommand, conn);
                                    cmd.CommandType = CommandType.Text;
                                    object tmpret = cmd.ExecuteScalar();
                                    _nextid = int.Parse(tmpret.ToString());
                                }
                                catch (Exception ex) {
                                    //return APCommonFun.ReturnError(mStrFuncName, "Getting the next id has faults.", "F", new JArray());
                                    APCommonFun.Error(string.Format("[{0}]:Logging fault, ex:{1}", mStrFuncName, ex.ToString()));
                                }

                                if(_nextid > 0)
                                {
                                    sqlcommand = "insert into accountsigninlog(logno, accid, signindatetime, sourceip) VALUES(:logno, :accid, :signindatetime, :sourceip)";
                                    try
                                    {
                                        cmd = new OracleCommand(sqlcommand, conn);
                                        cmd.CommandType = CommandType.Text;

                                        cmd.Parameters.Add(":logno", _nextid);
                                        cmd.Parameters.Add(":accid", __id);
                                        //cmd.Parameters.Add(":signindatetime", string.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now));
                                        cmd.Parameters.Add(":signindatetime", DateTime.Now);
                                        cmd.Parameters.Add(":sourceip", _client_ip1);

                                        if (cmd.ExecuteNonQuery() == 0)
                                        {
                                            APCommonFun.Error(string.Format("[{0}]:Logging fault, Known", mStrFuncName));
                                            _nextid = -1;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        APCommonFun.Error(string.Format("[{0}]:Logging fault, ex:{1}", mStrFuncName, ex.ToString()));
                                        _nextid = -1;
                                    }
                                    
                                }

                                try { conn.Close(); } catch { }
                                return new
                                {
                                    Result = "T",
                                    Message = "成功",
                                    user_id = __id,
                                    user_name = __name,
                                    logon = _nextid.ToString()  //For logging
                                };
                            }
                            else
                            {
                                try { conn.Close(); } catch { }
                                return new
                                {
                                    Result = "F",
                                    Message = string.Format("PasswordInCorrect"),
                                    user_id = string.Empty,
                                    user_name = string.Empty,
                                    logon = "-1"
                                };
                            }
                        }
                        

                    default:
                        try { conn.Close(); } catch { }
                        return new
                        {
                            Result = "R",
                            Message = string.Format("There are more than 2 Accounts:{0}.", account),
                            user_id = string.Empty,
                            user_name = string.Empty,
                            logon = "-1"
                        };
                }

                //return new
                //{
                //    Result = "T",
                //    Message = "成功",
                //    user_id = "111",
                //    user_name = "ssss"
                //};
            }
            catch (Exception ex)
            {
                try { conn.Close(); } catch { }
                
                return APCommonFun.ReturnError(mStrFuncName, "Common exception", ex.ToString(), "F", new JArray());
                //APCommonFun.Error("[FormLoginController]99：" + ex.ToString());
                ////APCommonFun.Error("[FormLoginController]99：" + sql);

                //return new
                //{
                //    Result = "F",
                //    Message = ex.ToString(),
                //    user_id = string.Empty,
                //    user_name = string.Empty
                //};
            }
            
        }
    }
}
