using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using rc_interface_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.Controllers._02_System_Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditEmployeeDeleteController : ControllerBase
    {
        [HttpPost]
        public object FormAll(List<Info_ListEmployeeInfoModel> toBeDelete)
        {
            string sql = "delete EMPLOYEES  where  ";

            bool isFirstAccount = true;
            foreach (var item in toBeDelete)
            {
                if (!string.IsNullOrEmpty(item.employee_id))
                {
                    if (isFirstAccount)
                    {
                        sql += " EMPID='" + item.employee_id + "' ";
                    }
                    else
                    {
                        sql += " OR EMPID='" + item.employee_id + "' ";
                    }
                }
                isFirstAccount = false;
            }
            try
            {
                APCommonFun.ExecSqlCommand(sql);
                return new
                {
                    Result = "T",
                    Message = "成功"
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[EditEmployeeDeleteController]99：" + ex.ToString());

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
