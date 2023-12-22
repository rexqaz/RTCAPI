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
    public class ListAccountSignInLogController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListAccountSignInLogModel Data)
        {
            JArray newJa = new JArray();

            string page = string.Empty;
            string login_date_from = string.Empty;
            string login_date_end = string.Empty;
            string logout_date_from = string.Empty;
            string logout_date_end = string.Empty;
            string account = string.Empty;
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

            if (Data.login_date_from != null && !string.IsNullOrEmpty(Data.login_date_from))
            {
                login_date_from = APCommonFun.CDBNulltrim(Data.login_date_from);
                if (!login_date_from.Contains("-"))
                {
                    string ReturnErr = "執行動作錯誤-login_date_from 欄位格式錯誤";
                    APCommonFun.Error("[ListAccountSignInLogController]90-" + ReturnErr);
                    return new
                    {
                        Result = "R",
                        Message = ReturnErr,
                        Data = ""
                    };
                }
                where_subStr += " AND a.SIGNINDATETIME >= to_date('" + login_date_from + "', 'YYYY-MM-DD')  ";
            }

            if (Data.login_date_end != null && !string.IsNullOrEmpty(Data.login_date_end))
            {
                login_date_end = APCommonFun.CDBNulltrim(Data.login_date_end);
                if (!login_date_end.Contains("-"))
                {
                    string ReturnErr = "執行動作錯誤-login_date_end 欄位格式錯誤";
                    APCommonFun.Error("[ListAccountSignInLogController]90-" + ReturnErr);
                    return new
                    {
                        Result = "R",
                        Message = ReturnErr,
                        Data = ""
                    };
                }

                string tmpLoginEndDate = DateTime.Parse(login_date_end).AddDays(1).ToString("yyyy-MM-dd");
                where_subStr += " AND a.SIGNINDATETIME <= to_date('" + tmpLoginEndDate + "', 'YYYY-MM-DD')  ";
            }

            if (Data.logout_date_from != null && !string.IsNullOrEmpty(Data.logout_date_from))
            {
                logout_date_from = APCommonFun.CDBNulltrim(Data.logout_date_from);
                if (!logout_date_from.Contains("-"))
                {
                    string ReturnErr = "執行動作錯誤-logout_date_from 欄位格式錯誤";
                    APCommonFun.Error("[ListAccountSignInLogController]90-" + ReturnErr);
                    return new
                    {
                        Result = "R",
                        Message = ReturnErr,
                        Data = ""
                    };
                }
                where_subStr += " AND a.SIGNOUTDATETIME >= to_date('" + logout_date_from + "', 'YYYY-MM-DD')  ";
            }

            if (Data.logout_date_end != null && !string.IsNullOrEmpty(Data.logout_date_end))
            {
                logout_date_end = APCommonFun.CDBNulltrim(Data.logout_date_end);
                if (!logout_date_end.Contains("-"))
                {
                    string ReturnErr = "執行動作錯誤-logout_date_end 欄位格式錯誤";
                    APCommonFun.Error("[ListAccountSignInLogController]90-" + ReturnErr);
                    return new
                    {
                        Result = "R",
                        Message = ReturnErr,
                        Data = ""
                    };
                }

                string tmpLogoutEndDate = DateTime.Parse(logout_date_end).AddDays(1).ToString("yyyy-MM-dd");
                where_subStr += " AND a.SIGNOUTDATETIME <= to_date('" + logout_date_end + "', 'YYYY-MM-DD')  ";
            }

            if (Data.account != null && !string.IsNullOrEmpty(Data.account))
            {
                account = APCommonFun.CDBNulltrim(Data.account);
                where_subStr += " AND b.ACCID like '%" + account + "%' ";
            }

            string sql = " select a.SOURCEIP, a.SIGNOUTDATETIME, a.SIGNINDATETIME, b.ACCID, b.EMPID, c.EMPCHINAME, d.GRPNAME , e.DEPTNAME "
                         + "FROM ACCOUNTSIGNINLOG a inner join ACCOUNTS b on a.ACCID = b.ACCID "
                         + "left join EMPLOYEES c on b.EMPID = c.EMPID "
                         + "left join GROUPS d on b.GRPID = d.GRPID "
                         + "left join DEPARTMENTS e on c.DEPTID = e.DEPTID "
                         + where_subStr
                         + "order by a.SIGNINDATETIME desc " + fetch_subStr;

            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string userid = APCommonFun.CDBNulltrim(dr["ACCID"].ToString());
                        string user = APCommonFun.CDBNulltrim(dr["EMPCHINAME"].ToString());
                        string group_name = APCommonFun.CDBNulltrim(dr["GRPNAME"].ToString());
                        string department_name = APCommonFun.CDBNulltrim(dr["DEPTNAME"].ToString());
                        string ip = APCommonFun.CDBNulltrim(dr["SOURCEIP"].ToString());
                        DateTime login_time_dt = DateTime.Parse(APCommonFun.CDBNulltrim(dr["SIGNINDATETIME"].ToString()));
                        string login_time = login_time_dt.ToString("yyyy/MM/dd HH:mm:ss");
                        string logout_time = string.Empty;
                        if (dr["SIGNOUTDATETIME"] == null || dr["SIGNOUTDATETIME"].ToString() == "")
                        {
                            logout_time = "尚未登出";
                        }
                        else
                        {
                            DateTime logout_time_dt = DateTime.Parse(APCommonFun.CDBNulltrim(dr["SIGNOUTDATETIME"].ToString()));
                            logout_time = logout_time_dt.ToString("yyyy/MM/dd HH:mm:ss");
                        }

                        JObject tmpJoLay = new JObject();
                        tmpJoLay.Add(new JProperty("account", userid));
                        tmpJoLay.Add(new JProperty("employee_name", user));
                        tmpJoLay.Add(new JProperty("department", department_name));
                        tmpJoLay.Add(new JProperty("group", group_name));
                        tmpJoLay.Add(new JProperty("login_ip", ip));
                        tmpJoLay.Add(new JProperty("login_time", login_time));
                        tmpJoLay.Add(new JProperty("logout_time", logout_time));
                        newJa.Add(tmpJoLay);
                    }
                }

                return new
                {
                    Result = "T",
                    Message = "成功",
                    TotalRec = dt.Rows.Count,
                    Data = newJa
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[ListAccountSignInLogController]99：" + ex.ToString());
                APCommonFun.Error("[ListAccountSignInLogController]99：" + sql);

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
