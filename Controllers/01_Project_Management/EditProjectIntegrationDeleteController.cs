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
    /// <summary>
    /// Polo
    /// EditProjectIntegrationDeleteController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EditProjectIntegrationDeleteController : ControllerBase
    {
        private string mStrFuncName = "EditProjectIntegrationDeleteController";
        [HttpPost]
        public object FormAll(List<Info_EditProjectIntegrationDeleteModel> input)
        {
            if (input == null)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No project integrations", "R", new JArray());
            }
            if (input.Count <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No project integrations", "R", new JArray());
            }


            OracleConnection conn = APCommonFun.GetOracleConnection();
            if (conn == null)
            {
                return APCommonFun.ReturnError(mStrFuncName, "Oracle connecting fault.", "R", new JArray());
            }
            JArray newJa = new JArray();
            JArray newJa_Final = new JArray();


            //第一步 : 先判斷有沒有必填未填寫，
            string InputIsok = "Y";
            string ReturnErr = "";

            string _sqlex = "";
            try
            {
                OracleTransaction oracleTransaction = conn.BeginTransaction();
                string delcommand1 = "delete  from ProjectIntegrationKey where PIKey= :PIKey ";
                string delcommand2 = "delete  from ProjectIntegrationDetails where PIKey= :PIKey ";
                
                OracleCommand cmd = new OracleCommand(delcommand1, conn);
                cmd.CommandType = CommandType.Text;
                bool _has_fault = false;
                for(int index = 0;index < input.Count;index++)
                {
                    cmd.CommandText = delcommand1;
                    cmd.Parameters.Clear();

                    cmd.Parameters.Add(":PIKey", input[index].merge_id);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        _sqlex += "Deleting ProjectIntegrationKey has an ex:" + ex.ToString();
                        _has_fault = true;
                    }

                    if (_has_fault)
                        break;

                    cmd.CommandText = delcommand2;
                    cmd.Parameters.Clear();

                    cmd.Parameters.Add(":PIKey", input[index].merge_id);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        _sqlex += "Deleting ProjectIntegrationDetails has an ex:" + ex.ToString();
                        _has_fault = true;
                    }
                    if (_has_fault)
                        break;
                }

                if (_has_fault)
                    oracleTransaction.Rollback();
                else
                    oracleTransaction.Commit();

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
                APCommonFun.Error("[EditProjectIntegrationDeleteController]99：" + ex.ToString());
                //APCommonFun.Error("[EditProjectIntegrationDeleteController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
