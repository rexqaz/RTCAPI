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

namespace rc_interface_API.Controllers
{
    /// <summary>
    /// Polo
    /// EditProjectAttributeController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EditProjectAttributeController : ControllerBase
    {
        private string mStrFuncName = "EditProjectAttributeController";

        [HttpPost]
        public object FormAll(List<Info_ProjectAttributeModel> attributeInput)
        {
            if(attributeInput == null)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No projects attributes", "R", new JArray());
            }
            if(attributeInput.Count <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No projects attributes", "R", new JArray());
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

            OracleConnection conn = APCommonFun.GetOracleConnection();
            if (conn == null)
            {
                return APCommonFun.ReturnError(mStrFuncName, "Oracle connecting fault.", "R", new JArray());
            }
            
            
            try
            {
                OracleTransaction oracleTransaction = conn.BeginTransaction();
                string sqlcommand = "update zcpst11 set IsDemoCase = :IsDemoCase where pspnr = :pspnr";
                OracleCommand cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;
                bool _has_fault = false;
                string _sqlex = "";
                for (int row_index = 0;row_index < attributeInput.Count;row_index++)
                {
                    string _projid = "";
                    try
                    {
                        _projid = attributeInput[row_index].project_id;
                        int _isdemo = int.Parse(attributeInput[row_index].is_demo);
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(":IsDemoCase", _isdemo);
                        cmd.Parameters.Add(":pspnr", _projid);

                        if(cmd.ExecuteNonQuery() <= 0)
                        {
                            _has_fault = true;
                            _sqlex = "";
                            break;
                        }
                    }
                    catch (Exception ex) {
                        _has_fault = true;
                        _sqlex = "Project Id : " + _projid + ", Ex:" + ex.ToString();
                        break;
                    }

                }

                if(!_has_fault)
                {
                    oracleTransaction.Commit();
                }
                else
                {
                    oracleTransaction.Rollback();
                }
                oracleTransaction.Dispose();
                try { conn.Close(); } catch { }

                if(!_has_fault)
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
            //    APCommonFun.Error("[EditProjectAttributeController]99：" + ex.ToString());
            //    //APCommonFun.Error("[Print_Deliver_pdfController]99：" + sql);

            //    return new
            //    {
            //        Result = "F",
            //        Message = ex.ToString()
            //    };
            //}

        }
    }
}
