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
    public class EditDepartmentInfoController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_EditDepartmentInfoModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            string department_id = string.Empty;
            string department_name = string.Empty;
            string is_receive_mail = string.Empty;
            string description = string.Empty;

            if (Data.department_id != null)
            {
                department_id = APCommonFun.CDBNulltrim(Data.department_id);
            }

            if (Data.department_name != null)
            {
                department_name = APCommonFun.CDBNulltrim(Data.department_name);
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
            if (department_name == "" || is_receive_mail == "") //必填                       
            {
                if (department_name == "")
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-department_name 為必填欄位";
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
                APCommonFun.Error("[EditDepartmentInfoController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sql = string.Empty;
            string sql2 = "select * FROM  departments where deptid='" + department_id + "' ";



            try
            {
                APCommonFun.ExecSqlCommand(sql2);
                DataTable dt2 = APCommonFun.GetDataTable(sql2);
                if (dt2.Rows.Count > 0)
                {
                    sql = "update departments set deptname='" + department_name 
                         + "' , deptdes='" + description + "',  canbecontactedbycust='" + is_receive_mail + "' where deptid='" + department_id + "' ";
                    APCommonFun.ExecSqlCommand(sql);
                }
                else
                {
                    return new
                    {
                        Result = "T",
                        Message = "部門代碼不存在，請重新輸入員工代碼"
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
                APCommonFun.Error("[EditDepartmentInfoController]99：" + ex.ToString());
                APCommonFun.Error("[EditDepartmentInfoController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
