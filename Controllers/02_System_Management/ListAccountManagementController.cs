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
    public class ListAccountManagementController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_AccountManagementModel Data)
        {
            JArray newJa = new JArray();

            string page = string.Empty;
            string account = string.Empty;
            string group = string.Empty;
            string where_subStr = "where 1=1 ";
            string fetch_subStr = string.Empty;

            if (Data.page != null && !string.IsNullOrEmpty(Data.page))
            {
                page = APCommonFun.CDBNulltrim(Data.page);
                if (!page.Contains(","))
                {
                    string ReturnErr = "執行動作錯誤-page 欄位格式錯誤";
                    APCommonFun.Error("[ListAccountSignInLogController]90-" + ReturnErr);
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

            if (Data.group != null && !string.IsNullOrEmpty(Data.group))
            {
                group = APCommonFun.CDBNulltrim(Data.group);
                where_subStr += " AND a.GRPID = '" + group + "' ";
            }

            if (Data.account != null && !string.IsNullOrEmpty(Data.account))
            {
                account = APCommonFun.CDBNulltrim(Data.account);
                where_subStr += " AND ACCID like '%" + account + "%' ";
            }

            string sql = " select * from ACCOUNTS a left join  GROUPS g on g.GRPID = a.GRPID "
                         + "left join "
                         + "( "
                         + "select* from EMPLOYEES e join DEPARTMENTS d on d.DEPTID = e.DEPTID "
                         + " ) ee on a.EMPID = ee.EMPID "
                         + where_subStr
                         + "order by ACCID desc " + fetch_subStr;

            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string ACCID = APCommonFun.CDBNulltrim(dr["ACCID"].ToString());
                        string GRPNAME = APCommonFun.CDBNulltrim(dr["GRPNAME"].ToString());
                        string ISEMPLOYEE = APCommonFun.CDBNulltrim(dr["ISEMPLOYEE"].ToString());
                        string EMPID = APCommonFun.CDBNulltrim(dr["EMPID"].ToString());
                        string EMPCHINAME = APCommonFun.CDBNulltrim(dr["EMPCHINAME"].ToString());
                        string DEPTNAME = APCommonFun.CDBNulltrim(dr["DEPTNAME"].ToString());

                        JObject tmpJoLay = new JObject();
                        tmpJoLay.Add(new JProperty("account", ACCID));
                        tmpJoLay.Add(new JProperty("group", GRPNAME));
                        tmpJoLay.Add(new JProperty("is_employee", ISEMPLOYEE));
                        tmpJoLay.Add(new JProperty("employee_id", EMPID));
                        tmpJoLay.Add(new JProperty("employee_name", EMPCHINAME));
                        tmpJoLay.Add(new JProperty("department", DEPTNAME));
                        newJa.Add(tmpJoLay);
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
                APCommonFun.Error("[ListAccountManagementController]99：" + ex.ToString());
                APCommonFun.Error("[ListAccountManagementController]99：" + sql);

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
