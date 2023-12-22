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
    public class ListEmployeeInfoController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListEmployeeInfoModel Data)
        {
            JObject tmpJoLay01 = new JObject();

            string employee_id = "";
            if (Data.employee_id != null) { employee_id = APCommonFun.CDBNulltrim(Data.employee_id); }            

            //第一步 : 先判斷有沒有必填未填寫，
            string InputIsok = "Y";
            string ReturnErr = "";

            if (employee_id == "") //必填                       
            {
                InputIsok = "N";
                ReturnErr = "執行動作錯誤-employee_id 為必填欄位";
            }
            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[ListEmployeeInfoController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sql = "select a.empid as EmployeeID, a.deptid as DepartmentID, b.deptname as DepartmentName ,a.empchiname as ChineseName,a.empengname as EnglishName , a.telno as TelephoneNo, a.email as Email, a.empdes as Description, a.canbecontactedbycust as  CanBeContactedByCustomer from EMPLOYEES a , DEPARTMENTS b where a.DEPTID = b.DEPTID(+) and Upper(a.empid) = '" + employee_id + "' ";

            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string EmployeeID = APCommonFun.CDBNulltrim(dr["EmployeeID"].ToString());
                        string DepartmentID = APCommonFun.CDBNulltrim(dr["DepartmentID"].ToString());
                        string ChineseName = APCommonFun.CDBNulltrim(dr["ChineseName"].ToString());
                        string EnglishName = APCommonFun.CDBNulltrim(dr["EnglishName"].ToString());
                        string CanBeContactedByCustomer = APCommonFun.CDBNulltrim(dr["CanBeContactedByCustomer"].ToString());
                        string TelephoneNo = APCommonFun.CDBNulltrim(dr["TelephoneNo"].ToString());
                        string Email = APCommonFun.CDBNulltrim(dr["Email"].ToString());
                        string Description = APCommonFun.CDBNulltrim(dr["Description"].ToString());

                        tmpJoLay01.Add(new JProperty("employee_id", EmployeeID));
                        tmpJoLay01.Add(new JProperty("employee_name_c", ChineseName));
                        tmpJoLay01.Add(new JProperty("employee_name_e", EnglishName));
                        tmpJoLay01.Add(new JProperty("employee_department", DepartmentID));
                        tmpJoLay01.Add(new JProperty("employee_tel", TelephoneNo));
                        tmpJoLay01.Add(new JProperty("employee_mail", Email));
                        tmpJoLay01.Add(new JProperty("is_receive_mail", CanBeContactedByCustomer));
                        tmpJoLay01.Add(new JProperty("description", Description));
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
                APCommonFun.Error("[ListEmployeeInfoController]99：" + ex.ToString());
                APCommonFun.Error("[ListEmployeeInfoController]99：" + sql);

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
