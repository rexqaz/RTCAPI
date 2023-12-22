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
    public class EditAccountDeleteController : ControllerBase
    {
        [HttpPost]
        public object FormAll(List<Info_AccountDeleteModel> toBeDelete)
        {
            string sql = "delete ACCOUNTS  where  ";

            bool isFirstAccount = true;
            foreach (var item in toBeDelete)
            {
                if (!string.IsNullOrEmpty(item.account))
                {
                    if (isFirstAccount)
                    {
                        sql += " ACCID='" + item.account + "' ";
                    }
                    else
                    {
                        sql += " OR ACCID='" + item.account + "' ";
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
                APCommonFun.Error("[EditAccountDeleteController]99：" + ex.ToString());
                APCommonFun.Error("[EditAccountDeleteController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
