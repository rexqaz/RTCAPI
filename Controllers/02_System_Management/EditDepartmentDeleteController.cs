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
    public class EditDepartmentDeleteController : ControllerBase
    {
        [HttpPost]
        public object FormAll(List<Info_DepartmentDeleteModel> toBeDelete)
        {
            string sql = "delete departments  where  ";

            bool isFirstAccount = true;
            foreach (var item in toBeDelete)
            {
                if (!string.IsNullOrEmpty(item.department_id))
                {
                    if (isFirstAccount)
                    {
                        sql += " Upper(deptid)='" + item.department_id + "' ";
                    }
                    else
                    {
                        sql += " OR Upper(deptid)='" + item.department_id + "' ";
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
                APCommonFun.Error("[EditDepartmentDeleteController]99：" + ex.ToString());
                APCommonFun.Error("[EditDepartmentDeleteController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
