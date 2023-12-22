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
    public class ListGroupManagementController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListGroupManagementModel Data)
        {
            JArray newJa = new JArray();

            string group_id = string.Empty;
            string group_name = string.Empty;
            string page = string.Empty;
            string fetch_subStr = string.Empty;

            if (Data.group_id != null) { group_id = APCommonFun.CDBNulltrim(Data.group_id); }
            if (Data.group_name != null) { group_name = APCommonFun.CDBNulltrim(Data.group_name); }
            if (Data.page != null) { page = APCommonFun.CDBNulltrim(Data.page); }

            if (Data.page != null && !string.IsNullOrEmpty(Data.page))
            {
                page = APCommonFun.CDBNulltrim(Data.page);
                if (!page.Contains(","))
                {
                    string ReturnErr = "執行動作錯誤-page 欄位格式錯誤";
                    APCommonFun.Error("[ListGroupManagementController]90-" + ReturnErr);
                    return new
                    {
                        Result = "R",
                        Message = ReturnErr,
                        Data = ""
                    };
                }
                else
                {
                    string[] page_data = page.Split(',');
                    fetch_subStr = "OFFSET " + ((Convert.ToInt32(page_data[0].ToString()) - 1) * Convert.ToInt32(page_data[1].ToString())).ToString() + " rows fetch first " + page_data[1].ToString() + " rows only ";
                }
            }

            string sql = "SELECT GRPID as GroupID, GRPNAME as GroupName, GRPdes as Description FROM groups where 1=1  ";
            if (!string.IsNullOrEmpty(group_id))
            {
                sql += " and GRPID like '%" + group_id + "%' ";
            }

            if (!string.IsNullOrEmpty(group_name))
            {
                sql += " and GRPNAME like '%" + group_name + "%' ";
            }

            sql += " order by GroupID  " + fetch_subStr;

            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string DepartmentID = APCommonFun.CDBNulltrim(dr["GroupID"].ToString());
                        string DepartmentName = APCommonFun.CDBNulltrim(dr["GroupName"].ToString());
                        string Description = APCommonFun.CDBNulltrim(dr["Description"].ToString());

                        JObject tmpJoLay01 = new JObject();
                        tmpJoLay01.Add(new JProperty("group_code", DepartmentID));
                        tmpJoLay01.Add(new JProperty("group_name", DepartmentName));
                        newJa.Add(tmpJoLay01);
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
                APCommonFun.Error("[ListGroupManagementController]99：" + ex.ToString());
                APCommonFun.Error("[ListGroupManagementController]99：" + sql);

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
