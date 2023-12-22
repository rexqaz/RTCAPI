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
    public class ListEmployeeManagementController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListEmployeeManagementModel Data)
        {
            JArray newJa = new JArray();

            string employee_id = string.Empty;
            string employee_department = string.Empty;
            string employee_name_c = string.Empty;
            string employee_name_e = string.Empty;
            string page = string.Empty;
            string fetch_subStr = string.Empty;

            if (Data.employee_id != null) { employee_id = APCommonFun.CDBNulltrim(Data.employee_id); }
            if (Data.employee_department != null) { employee_department = APCommonFun.CDBNulltrim(Data.employee_department); }
            if (Data.employee_name_c != null) { employee_name_c = APCommonFun.CDBNulltrim(Data.employee_name_c); }
            if (Data.employee_name_e != null) { employee_id = APCommonFun.CDBNulltrim(Data.employee_name_e); }
            if (Data.page != null) { page = APCommonFun.CDBNulltrim(Data.page); }

            if (Data.page != null && !string.IsNullOrEmpty(Data.page))
            {
                page = APCommonFun.CDBNulltrim(Data.page);
                if (!page.Contains(","))
                {
                    string ReturnErr = "執行動作錯誤-page 欄位格式錯誤";
                    APCommonFun.Error("[ListEmployeeManagementController]90-" + ReturnErr);
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

            string sql = "select a.empid as EmployeeID, a.deptid as DepartmentID, b.deptname as DepartmentName ,a.empchiname as ChineseName,a.empengname as EnglishName , a.telno as TelephoneNo, a.email as Email, a.empdes as Description, a.canbecontactedbycust as  CanBeContactedByCustomer from EMPLOYEES a , DEPARTMENTS b where a.DEPTID = b.DEPTID(+)  ";
            if (!string.IsNullOrEmpty(employee_id))
            {
                sql += " and a.empid like '%" + employee_id + "%' ";
            }

            if (!string.IsNullOrEmpty(employee_department))
            {
                sql += " and a.deptid like '%" + employee_department + "%' ";
            }

            if (!string.IsNullOrEmpty(employee_name_c))
            {
                sql += " and Upper(a.empchiname) like '%" + employee_name_c + "%' ";
            }

            if (!string.IsNullOrEmpty(employee_name_e))
            {
                sql += " and Upper(a.empengname) like '%" + employee_name_e + "%' ";
            }

            sql += " order by EMPLOYEEID  " + fetch_subStr;

            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string EmployeeID = APCommonFun.CDBNulltrim(dr["EmployeeID"].ToString());
                        string DepartmentName = APCommonFun.CDBNulltrim(dr["DepartmentName"].ToString());
                        string ChineseName = APCommonFun.CDBNulltrim(dr["ChineseName"].ToString());
                        string EnglishName = APCommonFun.CDBNulltrim(dr["EnglishName"].ToString());
                        string CanBeContactedByCustomer = APCommonFun.CDBNulltrim(dr["CanBeContactedByCustomer"].ToString());

                        JObject tmpJoLay01 = new JObject();
                        tmpJoLay01.Add(new JProperty("employee_id", EmployeeID));
                        tmpJoLay01.Add(new JProperty("employee_department", DepartmentName));
                        tmpJoLay01.Add(new JProperty("employee_name_c", ChineseName));
                        tmpJoLay01.Add(new JProperty("employee_name_e", EnglishName));
                        tmpJoLay01.Add(new JProperty("is_receive_mail", CanBeContactedByCustomer));
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
                APCommonFun.Error("[ListEmployeeManagementController]99：" + ex.ToString());
                APCommonFun.Error("[ListEmployeeManagementController]99：" + sql);

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
