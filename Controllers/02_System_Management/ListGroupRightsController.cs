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
    public class ListGroupRightsController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListGroupRightsModel Data)
        {
            JArray newJa = new JArray();            

            string group_id = "";
            if (Data.group_id != null) { group_id = APCommonFun.CDBNulltrim(Data.group_id); }

            //第一步 : 先判斷有沒有必填未填寫，
            string InputIsok = "Y";
            string ReturnErr = "";

            if (group_id == "") //必填                       
            {
                InputIsok = "N";
                ReturnErr = "執行動作錯誤-group_id 為必填欄位";
            }
            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[ListGroupRightsController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sql = "select * from functions where nodelevel = 1 ";

            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    //針對每一種功能做遍歷
                    foreach (DataRow dr in dt.Rows)
                    {
                        string FUNCID = APCommonFun.CDBNulltrim(dr["FUNCID"].ToString());
                        string FUNCNAME = APCommonFun.CDBNulltrim(dr["FUNCNAME"].ToString());

                        string sql2 = $@"select a.FUNCID, a.FUNCNAME, NVL(b.CANREAD, 0) AS CANREAD, NVL(b.CANINSERT, 0) AS CANINSERT, NVL(b.CANUPDATE, 0) AS CANUPDATE, NVL(b.CANDELETE, 0) AS CANDELETE
                            from functions a
                            left join groupfunctionsmappings b on a.funcid = b.funcid and b.GRPID = '{group_id}'
                            where a.parentnode = '{FUNCID}' AND a.ISDISPLAYEDONUI = 1
                            order by a.FUNCID";
                        DataTable dt2 = APCommonFun.GetDataTable(sql2);
                        JObject tmpJoLay01 = new JObject();
                        JArray newJa2 = new JArray();

                        if (dt2.Rows.Count > 0)
                        {                            
                            foreach (DataRow dr2 in dt2.Rows)
                            {
                                string function_name = APCommonFun.CDBNulltrim(dr2["FUNCNAME"].ToString());
                                string function_id = APCommonFun.CDBNulltrim(dr2["FUNCID"].ToString());
                                string read = APCommonFun.CDBNulltrim(dr2["CANREAD"].ToString());
                                string create = APCommonFun.CDBNulltrim(dr2["CANINSERT"].ToString());
                                string update = APCommonFun.CDBNulltrim(dr2["CANUPDATE"].ToString());
                                string delete = APCommonFun.CDBNulltrim(dr2["CANDELETE"].ToString());
                                
                                //第一層                                
                                JObject tmpJoLay02 = new JObject();
                                tmpJoLay02.Add(new JProperty("function_name", function_name));
                                tmpJoLay02.Add(new JProperty("function_id", function_id));
                                tmpJoLay02.Add(new JProperty("create", create));
                                tmpJoLay02.Add(new JProperty("read", read));
                                tmpJoLay02.Add(new JProperty("delete", delete));
                                tmpJoLay02.Add(new JProperty("update", update));
                                newJa2.Add(tmpJoLay02);
                            }

                            tmpJoLay01.Add(new JProperty("function_header", FUNCNAME));
                            tmpJoLay01.Add(new JProperty("function_list", newJa2));
                            newJa.Add(tmpJoLay01);
                        }
                        
                    }
                }
              
                return new
                {
                    Result = "T",
                    Message = "成功",
                    Data = newJa
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[ListGroupRightsController]99：" + ex.ToString());
                APCommonFun.Error("[ListGroupRightsController]99：" + sql);

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
