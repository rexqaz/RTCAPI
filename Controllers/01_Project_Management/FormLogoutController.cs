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

namespace rc_interface_API.Controllers._01_Project_Management
{
    /// <summary>
    /// Polo
    /// FormLogout
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FormLogoutController : ControllerBase
    {
        private string mStrFuncName = "FormLogoutController";
        [HttpPost]
        public object FormAll(Info_FormLogoutModel logout)
        {
            JArray newJa = new JArray();
            JArray newJa_Final = new JArray();

            string account = "";
            if (logout.account != null) { account = APCommonFun.CDBNulltrim(logout.account); }

            string _logon = "";
            if (logout.logon != null) { _logon = APCommonFun.CDBNulltrim(logout.logon); }


            if (_logon.Length <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "The logon must be specified.", "R", new JArray());
            }
            account = account.ToUpper();

            OracleConnection conn = APCommonFun.GetOracleConnection();
            if (conn == null)
            {
                return APCommonFun.ReturnError(mStrFuncName, "Oracle connecting fault.", "F", new JArray());
            }

            string sqlcommand = "";

            OracleCommand cmd = new OracleCommand(sqlcommand, conn);
            cmd.CommandType = CommandType.Text;
            DataSet _ds = new DataSet();
            int _rec_count = 0;
            
            try
            {
                

                string _client_ip1 = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                if (Request.Headers.ContainsKey("HTTP_X_FORWARDED_FOR"))
                    _client_ip1 = Request.Headers["HTTP_X_FORWARDED_FOR"];

                OracleDataAdapter _da = new OracleDataAdapter();
                sqlcommand = "update accountsigninlog set signoutdatetime=:signoutdatetime where logno=:logno";
                try
                {
                    cmd = new OracleCommand(sqlcommand, conn);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add(":logno", _logon);
                    
                    //cmd.Parameters.Add(":signindatetime", string.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now));
                    cmd.Parameters.Add(":signoutdatetime", DateTime.Now);

                    if (cmd.ExecuteNonQuery() <= 0)
                    {
                        APCommonFun.Error(string.Format("[{0}]:Logging fault, Known", mStrFuncName));
                        try { conn.Close(); } catch { }
                        return new
                        {
                            Result = "F",
                            Message = "失敗"
                        };
                    }
                    else
                    {
                        try { conn.Close(); } catch { }
                        return new
                        {
                            Result = "T",
                            Message = "成功"
                        };
                    }
                }
                catch (Exception ex)
                {
                    APCommonFun.Error(string.Format("[{0}]:Logging fault, ex:{1}", mStrFuncName, ex.ToString()));
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
            try { conn.Close(); } catch { }
            return new
            {
                Result = "F",
                Message = "失敗"
            };
        }
    }
}
