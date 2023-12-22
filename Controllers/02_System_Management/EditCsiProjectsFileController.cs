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
    public class EditCsiProjectsFileController : ControllerBase
    {
        private string mStrFuncName = "EditCsiProjectsFileController";

        [HttpPost]
        public object FormAll(Info_CsiProjectsFileModel input)
        {
            string project_file_id = "";
            if (input.project_file_id != null) { project_file_id = APCommonFun.CDBNulltrim(input.project_file_id); }
            string project_name = "";
            if (input.project_name != null) { project_name = APCommonFun.CDBNulltrim(input.project_name); }
            string _link = "";
            if (input.link != null) { _link = APCommonFun.CDBNulltrim(input.link); }
            string node_level = "";
            if (input.node_level != null) { node_level = APCommonFun.CDBNulltrim(input.node_level); }

            if (project_file_id.Length <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No project_file_id is specified.", "R", new JArray());
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

                sqlcommand = " update FUNCTIONS set FUNCNAME='" + project_name + "',WEBURL='" + _link + "',NODELEVEL='" + node_level + "'  where FUNCID='" + project_file_id + "' ";
                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;

                try
                {
                    if (cmd.ExecuteNonQuery() <= 0)
                    {
                        _has_fault = true;
                        _sqlex += "Update FUNCTIONS has faults//";
                    }

                }
                catch (Exception ex)
                {
                    _has_fault = true;
                    _sqlex += "Update FUNCTIONS has Ex:" + ex.ToString() + "//";
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
