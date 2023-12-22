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
    public class ListDepartmentManagementController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListDepartmentManagementModel Data)
        {
            JArray newJa = new JArray();

            string department_id = string.Empty;
            string department_name = string.Empty;
            string page = string.Empty;
            string fetch_subStr = string.Empty;

            if (Data.department_id != null) { department_id = APCommonFun.CDBNulltrim(Data.department_id); }
            if (Data.department_name != null) { department_name = APCommonFun.CDBNulltrim(Data.department_name); }
            if (Data.page != null) { page = APCommonFun.CDBNulltrim(Data.page); }

            if (Data.page != null && !string.IsNullOrEmpty(Data.page))
            {
                page = APCommonFun.CDBNulltrim(Data.page);
                if (!page.Contains(","))
                {
                    string ReturnErr = "執行動作錯誤-page 欄位格式錯誤";
                    APCommonFun.Error("[ListDepartmentManagementController]90-" + ReturnErr);
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

            string sql = "SELECT deptid as DepartmentID, deptname as DepartmentName, deptdes as Description, canbecontactedbycust as CanBeContactedByCustomer FROM departments where 1=1 ";
            if (!string.IsNullOrEmpty(department_id))
            {
                sql += " and deptid like '%" + department_id + "%' ";
            }

            if (!string.IsNullOrEmpty(department_name))
            {
                sql += " and deptname like '%" + department_name + "%' ";
            }

            sql += " order by DepartmentID  " + fetch_subStr;

            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string DepartmentID = APCommonFun.CDBNulltrim(dr["DepartmentID"].ToString());
                        string DepartmentName = APCommonFun.CDBNulltrim(dr["DepartmentName"].ToString());
                        string Description = APCommonFun.CDBNulltrim(dr["Description"].ToString());
                        string CanBeContactedByCustomer = APCommonFun.CDBNulltrim(dr["CanBeContactedByCustomer"].ToString());


                        JObject tmpJoLay01 = new JObject();
                        tmpJoLay01.Add(new JProperty("department_id", DepartmentID));
                        tmpJoLay01.Add(new JProperty("department_name", DepartmentName));
                        tmpJoLay01.Add(new JProperty("is_receive_mail", CanBeContactedByCustomer));
                        newJa.Add(tmpJoLay01);
                    }
                }

                return new
                {
                    Result = "T",
                    Message = "成功",
                    TotalRec = "2",
                    Data = newJa
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[ListDepartmentManagementController]99：" + ex.ToString());
                APCommonFun.Error("[ListDepartmentManagementController]99：" + sql);

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
