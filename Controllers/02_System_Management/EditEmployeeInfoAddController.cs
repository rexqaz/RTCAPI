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
    public class EditEmployeeInfoAddController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_EditEmployeeInfoModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            string employee_id = string.Empty;
            string employee_name_c = string.Empty; 
            string employee_name_e = string.Empty;
            string employee_department = string.Empty;
            string employee_tel = string.Empty;
            string employee_mail = string.Empty;
            string is_receive_mail = string.Empty;
            string description = string.Empty;

            if (Data.employee_id != null)
            {
                employee_id = APCommonFun.CDBNulltrim(Data.employee_id);
            }

            if (Data.employee_name_c != null)
            {
                employee_name_c = APCommonFun.CDBNulltrim(Data.employee_name_c);
            }

            if (Data.employee_name_e != null)
            {
                employee_name_e = APCommonFun.CDBNulltrim(Data.employee_name_e);
            }

            if (Data.employee_department != null)
            {
                employee_department = APCommonFun.CDBNulltrim(Data.employee_department);
            }

            if (Data.employee_tel != null)
            {
                employee_tel = APCommonFun.CDBNulltrim(Data.employee_tel);
            }

            if (Data.employee_mail != null)
            {
                employee_mail = APCommonFun.CDBNulltrim(Data.employee_mail);
            }

            if (Data.is_receive_mail != null)
            {
                is_receive_mail = APCommonFun.CDBNulltrim(Data.is_receive_mail);
            }

            if (Data.description != null)
            {
                description = APCommonFun.CDBNulltrim(Data.description);
            }

            //第一步 : 先判斷有沒有必填未填寫，
            if (employee_id == "" || employee_name_c == "" || employee_department == "" || is_receive_mail == "") //必填                       
            {
                if (employee_id == "")
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-employee_id 為必填欄位";
                }
                else if(employee_name_c == "")
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-employee_name_c 為必填欄位";
                }
                else if (employee_department == "")
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-employee_department 為必填欄位";
                }
                else 
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-is_receive_mail 為必填欄位";
                }
            }

            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[EditEmployeeInfoAddController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sql = string.Empty;
            string sql2 = "select * FROM  employees where EMPID='" + employee_id + "' ";


            try
            {
                APCommonFun.ExecSqlCommand(sql2);
                DataTable dt2 = APCommonFun.GetDataTable(sql2);
                if (dt2.Rows.Count == 0)
                {
                    sql = "Insert Into employees(empid, deptid, empchiname, empengname, telno,email, empdes,canbecontactedbycust) Values ('"
                            + employee_id + "', '" + employee_department + "', '" + employee_name_c + "', '" + employee_name_e + "', '"
                            + employee_tel + "', '" + employee_mail + "', '" + description + "', '" + is_receive_mail + "' )";
                    APCommonFun.ExecSqlCommand(sql);
                }
                else
                {
                    return new
                    {
                        Result = "T",
                        Message = "員工代碼重複，請重新輸入員工代碼"
                    };
                }

                return new
                {
                    Result = "T",
                    Message = "成功"
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[EditEmployeeAddController]99：" + ex.ToString());
                APCommonFun.Error("[EditEmployeeAddController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
