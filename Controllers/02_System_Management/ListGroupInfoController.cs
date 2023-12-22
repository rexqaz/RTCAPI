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
    public class ListGroupInfoController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListGroupInfoModel Data)
        {
            JObject tmpJoLay01 = new JObject();

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
                APCommonFun.Error("[ListGroupInfoController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sql = "SELECT GRPID as GroupID, GRPNAME as GroupName, GRPdes as Description FROM groups where Upper(GRPID)= '" + group_id + "' ";

            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string GroupID = APCommonFun.CDBNulltrim(dr["GroupID"].ToString());
                        string GroupName = APCommonFun.CDBNulltrim(dr["GroupName"].ToString());
                        string Description = APCommonFun.CDBNulltrim(dr["Description"].ToString());

                        
                        tmpJoLay01.Add(new JProperty("group_code", GroupID));
                        tmpJoLay01.Add(new JProperty("group_name", GroupName));
                        tmpJoLay01.Add(new JProperty("note", Description));
                    }
                }
                
                return new
                {
                    Result = "T",
                    Message = "成功",
                    Data = tmpJoLay01
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[ListGroupInfoController]99：" + ex.ToString());
                APCommonFun.Error("[ListGroupInfoController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString(),
                    Data = tmpJoLay01
                };
            }

        }
    }
}
