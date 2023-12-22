using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
    public class ListAccountProjectOverviewController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListAccountProjectOverviewModel Data)
        {
            JArray newJa = new JArray();
            string InputIsok = "Y";
            string ReturnErr = "";
            string account = string.Empty;
            string project_name = string.Empty;
            string where_subStr = "where 1=1 ";

            
            if (Data.project_name != null && !string.IsNullOrEmpty(Data.project_name))
            {
                project_name = APCommonFun.CDBNulltrim(Data.project_name);
                where_subStr += " AND z.POST1 like '%" + project_name + "%' ";
            }

            if (Data.account != null && !string.IsNullOrEmpty(Data.account))
            {
                account = APCommonFun.CDBNulltrim(Data.account);
                where_subStr += " AND a.ACCID ='" + account + "' ";
            }

            //第一步 : 先判斷有沒有必填未填寫，
            if (account == "" ) //必填                       
            {
                InputIsok = "N";
                ReturnErr = "執行動作錯誤-account 為必填欄位";
            }

            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[ListAccountProjectOverviewController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sql = " select* from ACCOUNTPROJECTMAPPINGS a left join ZCPST11 z on a.PSPNR = z.PSPNR "
                         + "left join KNA1 k on k.KUNNR = z.KUNNR  "
                         + where_subStr;

            string sql2 = "select * from ZCPST11 z left join KNA1 k on k.KUNNR = z.KUNNR  ";

            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);
                DataTable dt2 = APCommonFun.GetDataTable(sql2);

                if (dt2.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in dt2.Rows)
                    {
                        bool isMatch = false;
                        if (dt.Rows.Count > 0)
                        {                            
                            foreach (DataRow dr in dt.Rows)
                            {
                                string project_id = APCommonFun.CDBNulltrim(dr["PSPID"].ToString());
                                string POST1 = APCommonFun.CDBNulltrim(dr["POST1"].ToString());
                                string status = APCommonFun.CDBNulltrim(dr["PROJ_PROG"].ToString());
                                string owner_name = APCommonFun.CDBNulltrim(dr["NAME1"].ToString());
                                string is_check_project_merge = APCommonFun.CDBNulltrim(dr["CANVIEWPIRESULT"].ToString());

                                string project_id2 = APCommonFun.CDBNulltrim(dr2["PSPID"].ToString());
                                if (project_id2 == project_id)
                                {
                                    isMatch = true;
                                    JObject tmpJoLay01 = new JObject();
                                    tmpJoLay01.Add(new JProperty("project_id", project_id));
                                    tmpJoLay01.Add(new JProperty("project_name", POST1));
                                    tmpJoLay01.Add(new JProperty("owner_name", owner_name));
                                    tmpJoLay01.Add(new JProperty("status", status));
                                    tmpJoLay01.Add(new JProperty("is_view", "1"));
                                    tmpJoLay01.Add(new JProperty("is_check_project_merge", is_check_project_merge));

                                    newJa.Add(tmpJoLay01);
                                    break;
                                }
                            }
                        }

                        if (!isMatch)
                        {
                            string project_id = APCommonFun.CDBNulltrim(dr2["PSPID"].ToString());
                            string POST1 = APCommonFun.CDBNulltrim(dr2["POST1"].ToString());
                            string status = APCommonFun.CDBNulltrim(dr2["PROJ_PROG"].ToString());
                            string owner_name = APCommonFun.CDBNulltrim(dr2["NAME1"].ToString());

                            JObject tmpJoLay01 = new JObject();
                            tmpJoLay01.Add(new JProperty("project_id", project_id));
                            tmpJoLay01.Add(new JProperty("project_name", POST1));
                            tmpJoLay01.Add(new JProperty("owner_name", owner_name));
                            tmpJoLay01.Add(new JProperty("status", status));
                            tmpJoLay01.Add(new JProperty("is_view", "0"));
                            tmpJoLay01.Add(new JProperty("is_check_project_merge", "0"));

                            newJa.Add(tmpJoLay01);
                        }
                    }
                }
                
                return new
                {
                    Result = "T",
                    Message = "成功",
                    TotalRec = newJa.Count,
                    Data = newJa
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[ListAccountProjectOverviewController]99：" + ex.ToString());
                APCommonFun.Error("[ListAccountProjectOverviewController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString(),
                    Data = newJa
                };
            }

        }
    }
}
