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
    public class EditSystemParameterInfoController : ControllerBase
    {
        private string mStrFuncName = "EditSystemParameterInfoController";
        [HttpPost]
        public object FormAll(Info_EditSystemParameterInfoModel input)
        {
            string project_id = "";
            if (input.project_id != null) { project_id = APCommonFun.CDBNulltrim(input.project_id); }

            string manage_percentage = "";
            if (input.ManagerPercent != null) { manage_percentage = APCommonFun.CDBNulltrim(input.ManagerPercent); }

            string project_name = "";
            if (input.project_name != null) { project_name = APCommonFun.CDBNulltrim(input.project_name); }

            if (project_id.Length <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No project_id is specified.", "R", new JArray());
            }
            if (manage_percentage.Length <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No manage_percentage is specified.", "R", new JArray());
            }
            double dbl_manage_percentage = 0;
            try
            {
                dbl_manage_percentage = double.Parse(manage_percentage) / 100;
            }
            catch
            {
                return APCommonFun.ReturnError(mStrFuncName, "The ManagerPencent has errors.", "F", new JArray());
            }

            //CCTV 應可接受無資料
            if(input.CCTV_setting == null)
            {
                input.CCTV_setting = new List<CCTV_setting>();
            }

            OracleConnection conn = APCommonFun.GetOracleConnection();
            if (conn == null)
            {
                return APCommonFun.ReturnError(mStrFuncName, "Oracle connecting fault.", "R", new JArray());
            }

            OracleCommand cmd = null;
            string _sqlex = "";
            string sqlcommand = "";
            bool _has_fault = false;
            //1.先檢查專案是否存在 -->  ZCPST11 PSPNR=:pspnr  -->  專案不存在則報錯離開
            //2.更新專案名稱
            //3.檢查 ManagerPercent 是否存在該專案設定  -->  存在:則更新, 不存在則新增
            //4.先刪除原有 CCTV 
            //5.加入新傳入的CCTB --> URL 為必填嗎?


            //1.先檢查專案是否存在 -->  ZCPST11 PSPNR=:pspnr  -->  專案不存在則報錯離開
            sqlcommand = "select post1 from ZCPST11 where upper(PSPNR)='" + project_id.ToUpper().Trim() + "'";
            try
            {
                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;
                object _tmpobj = cmd.ExecuteScalar().ToString();
                if(_tmpobj == null)
                {
                    return APCommonFun.ReturnError(mStrFuncName, string.Format("The project:{0} is not exist.", project_id), "F", new JArray());
                }
                
            }
            catch (Exception ex)
            {
                try { conn.Close(); } catch { }
                return APCommonFun.ReturnError(mStrFuncName, "Common exception", ex.ToString(), "F", new JArray());
            }

            try
            {
                //2.更新專案名稱
                OracleTransaction oracleTransaction = conn.BeginTransaction();
                sqlcommand = "update ZCPST11 set POST1=:POST1 where upper(PSPNR)=:PSPNR ";
                cmd = new OracleCommand(sqlcommand, conn);
                cmd.Parameters.Add(":POST1", project_name);
                cmd.Parameters.Add(":PSPNR", project_id.ToUpper().Trim());

                try
                {
                    if (cmd.ExecuteNonQuery() <= 0)
                    {
                        _has_fault = true;
                        _sqlex += "Updating project_name has faults//";
                    }

                }
                catch (Exception ex)
                {
                    _has_fault = true;
                    _sqlex += "Updating project_name has ex:" + ex.ToString() + "//";
                }

                if(!_has_fault)
                {
                    //3.檢查 ManagerPercent 是否存在該專案設定  -->  存在:則更新, 不存在則新增
                    _has_fault = false;
                    try
                    {
                        sqlcommand = "SELECT VALUE FROM  ManagerPercent WHERE PSPNR=:pspnr ";
                        cmd = new OracleCommand(sqlcommand, conn);
                        cmd.Parameters.Add(":pspnr", project_id.ToUpper().Trim());
                        object _tmpobj = cmd.ExecuteScalar();


                        if (_tmpobj == null)
                        {
                            //insert
                            sqlcommand = "INSERT INTO ManagerPercent(PSPNR,VALUE) VALUES(:pspnr,:VALUE) ";
                            cmd = new OracleCommand(sqlcommand, conn);
                            
                            cmd.Parameters.Add(":pspnr", project_id.ToUpper().Trim());
                            cmd.Parameters.Add(":VALUE", dbl_manage_percentage);

                            if (cmd.ExecuteNonQuery() <= 0)
                            {
                                _has_fault = true;
                                _sqlex += "Updating ManagerPercent has faults//";
                            }
                        }
                        else
                        {
                            //update
                            sqlcommand = "UPDATE ManagerPercent SET VALUE=:VALUE WHERE PSPNR=:pspnr ";
                            cmd = new OracleCommand(sqlcommand, conn);
                            cmd.Parameters.Add(":VALUE", dbl_manage_percentage);
                            cmd.Parameters.Add(":pspnr", project_id.ToUpper().Trim());

                            if (cmd.ExecuteNonQuery() <= 0)
                            {
                                _has_fault = true;
                                _sqlex += "Updating ManagerPercent has faults//";
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        _has_fault = true;
                        _sqlex += "Updating ManagerPercent has ex:" + ex.ToString() + "//";
                    }
                    
                }


                //CCTV
                if(!_has_fault)
                {
                    
                    
                    _has_fault = false;

                    //4.先刪除原有 CCTV 
                    sqlcommand = "SELECT count(*) FROM PORJECTCCTVURL where upper(PSPNR)='" + project_id.ToUpper().Trim() + "'";
                    cmd = new OracleCommand(sqlcommand, conn);
                    int _cctv_count = 0;
                    
                    try
                    {
                        _cctv_count = int.Parse(cmd.ExecuteScalar().ToString());

                        sqlcommand = "DELETE FROM PorjectCCTVURL WHERE PSPNR=:pspnr";
                        cmd = new OracleCommand(sqlcommand, conn);
                        cmd.Parameters.Add(":pspnr", project_id.ToUpper().Trim());

                        int _del_cctv_count = cmd.ExecuteNonQuery();
                        if(_del_cctv_count != _cctv_count)
                        {
                            _has_fault = true;
                            _sqlex += "Deleting CCTVs has faults//";
                        }
                    }
                    catch (Exception ex)
                    {
                        _has_fault = true;
                        _sqlex += "Deleting CCTVs has ex:" + ex.ToString() + "//";
                    }

                    //5.加入新傳入的CCTB --> URL 為必填嗎?
                    if(!_has_fault)
                    {
                        _has_fault = false;

                        sqlcommand = "insert into porjectcctvurl (PSPNR,CCTVURL,MSG) values (:pspnr,:CCTVURL,:MSG)";
                        cmd = new OracleCommand(sqlcommand, conn);
                        cmd.Parameters.Add(":pspnr", project_id.ToUpper().Trim());

                        for(int index = 0;index < input.CCTV_setting.Count;index++)
                        {
                            _has_fault = false;
                            cmd.Parameters.Clear();

                            cmd.Parameters.Add(":pspnr", project_id);
                            cmd.Parameters.Add(":CCTVURL", input.CCTV_setting[index].CCTV_URL);
                            cmd.Parameters.Add(":MSG", input.CCTV_setting[index].description);

                            try
                            {
                                if (cmd.ExecuteNonQuery() <= 0)
                                {
                                    _has_fault = true;
                                    _sqlex += "Adding CCTVs has faults at row:" + index.ToString() + "//";
                                }
                            }
                            catch (Exception ex)
                            {
                                _has_fault = true;
                                _sqlex += "Adding CCTVs has faults at row:" + index.ToString() + ", EX:" + ex.ToString() + "//";
                            }

                            if (_has_fault)
                                break;
                        }
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

            JArray newJa = new JArray();
            JArray newJa_Final = new JArray();


            //第一步 : 先判斷有沒有必填未填寫，
            string InputIsok = "Y";
            string ReturnErr = "";

            //if (Data.attributeInput == null) //必填                       
            //{
            //    InputIsok = "N";
            //    ReturnErr = "執行動作錯誤-需傳遞參數";
            //}

            ////第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            //if (InputIsok == "N")
            //{
            //    APCommonFun.Error("[EditProjectAttributeController]90-" + ReturnErr);
            //    return new
            //    {
            //        Result = "R",
            //        Message = ReturnErr,
            //        Data = ""
            //    };
            //}

            try
            {
                return new
                {
                    Result = "T",
                    Message = "成功"
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[EditSystemParameterInfoController]99：" + ex.ToString());
                //APCommonFun.Error("[EditSystemParameterInfoController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
