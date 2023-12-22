using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rc_interface_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rc_interface_API.Controllers._02_System_Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditAccountAddController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_EditAccountInfoModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            string account = string.Empty;
            string group = string.Empty;
            string department = string.Empty;
            string is_employee = string.Empty;
            string note = string.Empty;
            string is_reset_password = string.Empty;
            string new_password = string.Empty;
            string confirm_new_password = string.Empty;
            string employee_id = string.Empty;
            string employee_name = string.Empty;

            if (Data.account != null)
            {
                account = APCommonFun.CDBNulltrim(Data.account);
            }

            if (Data.group != null)
            {
                group = APCommonFun.CDBNulltrim(Data.group);
            }

            if (Data.is_employee != null)
            {
                is_employee = APCommonFun.CDBNulltrim(Data.is_employee);
            }

            if (Data.note != null)
            {
                note = APCommonFun.CDBNulltrim(Data.note);
            }

            if (Data.department != null)
            {
                department = APCommonFun.CDBNulltrim(Data.department);
            }

            if (Data.is_reset_password != null)
            {
                is_reset_password = APCommonFun.CDBNulltrim(Data.is_reset_password);
            }

            if (Data.new_password != null)
            {
                new_password = APCommonFun.CDBNulltrim(Data.new_password);
            }

            if (Data.confirm_new_password != null)
            {
                confirm_new_password = APCommonFun.CDBNulltrim(Data.confirm_new_password);
            }

            if (Data.employee_id != null)
            {
                employee_id = APCommonFun.CDBNulltrim(Data.employee_id);
            }

            if (Data.employee_name != null)
            {
                employee_name = APCommonFun.CDBNulltrim(Data.employee_name);
            }


            //第一步 : 先判斷有沒有必填未填寫，
            if (group == "" || is_employee == "" || account == "") //必填                       
            {
                if (account == "")
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-account 為必填欄位";
                }
                else if (group == "")
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-group 為必填欄位";
                }
                else
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-is_employee 為必填欄位";
                }
            }

            if (is_reset_password == "1")
            {
                if (new_password == "")
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-new_password 為必填欄位";
                }
                else if (confirm_new_password == "")
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-confirm_new_password 為必填欄位";
                }
                else if (new_password != confirm_new_password)
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-密碼新舊不一致";
                }
            }

            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[EditAccountAddController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sql = string.Empty;
            string sql2 = "select * FROM  ACCOUNTS where ACCID='" + account + "' ";

            try
            {
                APCommonFun.ExecSqlCommand(sql2);
                DataTable dt2 = APCommonFun.GetDataTable(sql2);
                if (dt2.Rows.Count > 0)
                {
                    
                    return new
                    {
                        Result = "T",
                        Message = "帳戶已存在，請重新輸入帳戶"
                    };
                }
                else
                {
                    sql = "INSERT INTO ACCOUNTS (ACCID, ACCPWD, GRPID, ISEMPLOYEE, EMPID, ACCDES) VALUES ('" + account + "', '" + Convert.ToBase64String(Encoding.UTF8.GetBytes(new_password)) + "' , '" + group + "', '"
                         +  is_employee + "', '" + employee_id + "', '" + note + "' ) ";
                    APCommonFun.ExecSqlCommand(sql);
                }

                return new
                {
                    Result = "T",
                    Message = "成功"
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[EditAccountAddController]99：" + ex.ToString());
                APCommonFun.Error("[EditAccountAddController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
