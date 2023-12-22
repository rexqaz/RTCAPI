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
    public class ListDepartmentInfoController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListDepartmentInfoModel Data)
        {
            JObject tmpJoLay01 = new JObject();

            string department_id = "";
            if (Data.department_id != null) { department_id = APCommonFun.CDBNulltrim(Data.department_id); }

            //第一步 : 先判斷有沒有必填未填寫，
            string InputIsok = "Y";
            string ReturnErr = "";

            if (department_id == "") //必填                       
            {
                InputIsok = "N";
                ReturnErr = "執行動作錯誤-employee_id 為必填欄位";
            }
            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[ListDepartmentInfoController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sql = "SELECT deptid as DepartmentID, deptname as DepartmentName, deptdes as Description, canbecontactedbycust as CanBeContactedByCustomer FROM departments where Upper(deptid)= '" + department_id + "' ";


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

                        tmpJoLay01.Add(new JProperty("department_id", DepartmentID));
                        tmpJoLay01.Add(new JProperty("department_name", DepartmentName));
                        tmpJoLay01.Add(new JProperty("is_receive_mail", CanBeContactedByCustomer));
                        tmpJoLay01.Add(new JProperty("descrpiton", Description));
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
                APCommonFun.Error("[ListDepartmentInfoController]99：" + ex.ToString());
                APCommonFun.Error("[ListDepartmentInfoController]99：" + sql);

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
