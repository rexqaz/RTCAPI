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
    public class FormCsiProjectsFileController : ControllerBase
    {
        private string mStrFuncName = "FormCsiProjectsFileController";
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
            if (string.IsNullOrEmpty(node_level)) node_level = "1";

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

                sqlcommand = " Insert into  FUNCTIONS (FUNCID,FUNCNAME,WEBURL,NODELEVEL,PARENTNODE,ISDISPLAYEDONUI) values(:PROJECT_FILE_ID,:PRJECT_NAME,:TARGET_LINK,:NODELEVEL,:PARENTNODE,:ISDISPLAYEDONUI) ";
                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Clear();
                cmd.Parameters.Add(":PROJECT_FILE_ID", project_file_id);
                cmd.Parameters.Add(":PRJECT_NAME", project_name);
                cmd.Parameters.Add(":TARGET_LINK", _link);
                cmd.Parameters.Add(":NODELEVEL", node_level);
                cmd.Parameters.Add(":PARENTNODE", "T00");
                cmd.Parameters.Add(":ISDISPLAYEDONUI", "1");
                try
                {
                    if (cmd.ExecuteNonQuery() <= 0)
                    {
                        _has_fault = true;
                        _sqlex += "Insert FUNCTIONS has faults//";
                    }

                }
                catch (Exception ex)
                {
                    _has_fault = true;
                    _sqlex += "Insert FUNCTIONS has Ex:" + ex.ToString() + "//";
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
