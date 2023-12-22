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

namespace rc_interface_API.Controllers._01_Project_Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditProjectIntegrationInfoController : ControllerBase
    {
        private string mStrFuncName = "EditProjectIntegrationInfoController";

        [HttpPost]
        public object FormAll(Info_EditProjectIntegrationInfoModel input)
        {
            string merge_id = "";
            if (input.merge_id != null) { merge_id = APCommonFun.CDBNulltrim(input.merge_id); }
            string merge_description = "";
            if (input.merge_description != null) { merge_description = APCommonFun.CDBNulltrim(input.merge_description); }

            if (input.project_info == null)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No projects", "R", new JArray());
            }
            if (input.project_info.Count <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No projects", "R", new JArray());
            }

            //if (merge_id.Length <= 0)
            //{
            //    return APCommonFun.ReturnError(mStrFuncName, "No merge_id", "R", new JArray());
            //}
            if (merge_description.Length <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No merge_description", "R", new JArray());
            }

            OracleConnection conn = APCommonFun.GetOracleConnection();
            if (conn == null)
            {
                return APCommonFun.ReturnError(mStrFuncName, "Oracle connecting fault.", "R", new JArray());
            }

            
            try
            {

                //1.判斷是否為新增 merge_id="", 或 merge_id 找不到
                bool is_new = false;
                string _sqlex = "";
                string sqlcommand = "";
                OracleCommand cmd = null;
                int _mergeid = 0;
                if (merge_id.Length > 0)
                {
                    try
                    {
                        _mergeid = int.Parse(merge_id);
                        sqlcommand = "select count(*) from ProjectIntegrationKey where PIKey=" + merge_id;
                        cmd = new OracleCommand(sqlcommand, conn);
                        cmd.CommandType = CommandType.Text;
                        object obj = cmd.ExecuteScalar();
                        if (int.Parse(obj.ToString()) <= 0)
                            is_new = true;
                    }
                    catch (Exception ex)
                    {
                        _sqlex += "Checking PIKey ex:" + ex.ToString() + "//";
                        _mergeid = 0;
                    }
                }
                else
                {
                    is_new = true;
                }

                if(is_new)
                {
                    //取得 next id
                    sqlcommand = "select PROJECTINTEGRATIONID.NEXTVAL from DUAL";

                    cmd = new OracleCommand(sqlcommand, conn);
                    cmd.CommandType = CommandType.Text;
                    try
                    {
                        object obj = cmd.ExecuteScalar();
                        _mergeid = int.Parse(obj.ToString());

                    }
                    catch(Exception ex)
                    {
                        _sqlex += "Getting PIKey ex:" + ex.ToString() + "//";
                        _mergeid = 0;
                    }
                }

                //
                if(_mergeid == 0)
                {
                    try { conn.Close(); } catch { }
                    return APCommonFun.ReturnError(mStrFuncName, "Checking PIKey for Insert/Update ProjectIntegrationKey has errors.", _sqlex,"F", new JArray());
                }
                merge_id = _mergeid.ToString();
                //2.寫入 ProjectIntegrationKey 
                OracleTransaction oracleTransaction = conn.BeginTransaction();
                bool _has_fault = false;
                if (is_new)
                {
                    //insert
                    sqlcommand = "insert into ProjectIntegrationKey(PIKey,PIDescription) values( :PIKey,:PIDescription)";
                    cmd = new OracleCommand(sqlcommand, conn);
                    
                    
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(":PIKey", _mergeid);
                    cmd.Parameters.Add(":PIDescription", merge_description);
                    
                    try
                    {
                        if (cmd.ExecuteNonQuery() <= 0)
                        {
                            _has_fault = true;
                            _sqlex += "Insert merge_description has faults//";
                        }

                    }
                    catch (Exception ex)
                    {
                        _has_fault = true;
                        _sqlex += "Insert merge_description has Ex:" + ex.ToString() + "//";
                    }

                }
                else
                {
                    //UpdateProjectIntegrationKey
                    sqlcommand = "update ProjectIntegrationKey set PIDescription=:PIDescription where PIKey=:PIKey ";
                    cmd = new OracleCommand(sqlcommand, conn);
                    _has_fault = false;
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Clear();

                    cmd.Parameters.Add(":PIDescription", merge_description);
                    cmd.Parameters.Add(":PIKey", _mergeid);
                    try
                    {
                        if (cmd.ExecuteNonQuery() <= 0)
                        {
                            _has_fault = true;
                            _sqlex += "Updating merge_description has faults//";
                        }

                    }
                    catch (Exception ex)
                    {
                        _has_fault = true;
                        _sqlex += "Inserting merge_description has Ex:" + ex.ToString() + "//";
                    }
                }

                //3.寫入 ProjectIntegrationKey 
                if (!_has_fault)
                {
                    if(!is_new)
                    {
                        //for updating
                        //delete ProjectIntegrationDetails first

                        sqlcommand = "delete from  ProjectIntegrationDetails where PIKey=:PIKey";
                        cmd = new OracleCommand(sqlcommand, conn);
                        _has_fault = false;
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(":PIKey", _mergeid);
                        try
                        {
                            if (cmd.ExecuteNonQuery() <= 0)
                            {
                                _has_fault = true;
                                _sqlex += "Deleting ProjectIntegrationDetails has faults//";
                            }

                        }
                        catch (Exception ex)
                        {
                            _has_fault = true;
                            _sqlex += "Deleting ProjectIntegrationDetails has Ex:" + ex.ToString() + "//";
                        }
                    }

                    //and then add ProjectIntegrationDetails 
                    if(!_has_fault)
                    {
                        sqlcommand = "insert into ProjectIntegrationDetails(PIKey,SUBPSPNR) values( :PIKey,:SUPPSPNR) ";
                        cmd = new OracleCommand(sqlcommand, conn);
                        _has_fault = false;
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();

                        for(int dindex = 0;dindex < input.project_info.Count;dindex++)
                        {
                            _has_fault = false;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(":PIKey", _mergeid);
                            cmd.Parameters.Add(":SUPPSPNR", input.project_info[dindex].project_id);

                            try
                            {
                                if(cmd.ExecuteNonQuery() <= 0)
                                {
                                    _has_fault = true;
                                    _sqlex += "Adding ProjectIntegrationDetails has faults at row:" + dindex.ToString() + "//";
                                }
                            }
                            catch(Exception ex)
                            {
                                _has_fault = true;
                                _sqlex += "Adding ProjectIntegrationDetails has faults at row:" + dindex.ToString() + "//";
                            }

                            if (_has_fault)
                                break;
                        }
                    }


                }

                if(_has_fault)
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
                //return APCommonFun.ReturnError(mStrFuncName, "Common exception.", ex.ToString(), "F", new JArray());
                return APCommonFun.ReturnError(mStrFuncName, "Common exception.", ex.ToString(), "F", new JArray());
            }

            //try
            //{
            //    return new
            //    {
            //        Result = "T",
            //        Message = "成功"
            //    };
            //}
            //catch (Exception ex)
            //{
            //    APCommonFun.Error("[EditProjectIntegrationInfoController]99：" + ex.ToString());
            //    //APCommonFun.Error("[EditProjectIntegrationInfoController]99：" + sql);

            //    return new
            //    {
            //        Result = "F",
            //        Message = ex.ToString()
            //    };
            //}

        }
    }
}
