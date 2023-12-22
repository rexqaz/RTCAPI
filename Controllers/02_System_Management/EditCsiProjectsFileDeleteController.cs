using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using rc_interface_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.Controllers._02_System_Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditCsiProjectsFileDeleteController : ControllerBase
    {
        private string mStrFuncName = "EditCsiProjectsFileDeleteController";
        [HttpPost]
        public object FormAll(List<Info_EditCsiProjectsFileDeleteModel> input)
        {
            if (input.Count <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No project_file_ids is specified.", "R", new JArray());
            }

            OracleConnection conn = APCommonFun.GetOracleConnection();
            if (conn == null)
            {
                return APCommonFun.ReturnError(mStrFuncName, "Oracle connecting fault.", "R", new JArray());
            }

            string sqlcommand = "";
            OracleCommand cmd = null;
            bool _has_fault = false;
            string _sqlex = "";
            try
            {
                OracleTransaction oracleTransaction = conn.BeginTransaction();

                sqlcommand = " delete from  FUNCTIONS where FUNCID=:PROJECT_FILE_ID ";
                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;

                for(int index = 0;index < input.Count;index++)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(":PROJECT_FILE_ID", input[index].project_file_id.Trim());
                    try
                    {
                        if (cmd.ExecuteNonQuery() <= 0)
                        {
                            _has_fault = true;
                            _sqlex += "delete FUNCTIONS has faults//";
                        }

                    }
                    catch (Exception ex)
                    {
                        _has_fault = true;
                        _sqlex += "Delete FUNCTIONS has Ex:" + ex.ToString() + "//";
                    }
                }

                

                if (_has_fault)
                {
                    oracleTransaction.Rollback();
                }
                else
                {
                    oracleTransaction.Commit();
                }
                oracleTransaction.Dispose();
                try { conn.Close(); } catch { }

                if (!_has_fault)
                {
                    return new
                    {
                        Result = "T",
                        Message = "成功"
                    };
                }
                else
                {
                    return APCommonFun.ReturnError(mStrFuncName, "失敗", _sqlex, "F", new JArray());
                }
            }
            catch (Exception ex)
            {
                try { conn.Close(); } catch { }
                return APCommonFun.ReturnError(mStrFuncName, "Common exception.", ex.ToString(), "F", new JArray());
            }
        }
    }
}
