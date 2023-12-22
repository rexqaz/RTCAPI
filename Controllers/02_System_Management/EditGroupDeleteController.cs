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
    public class EditGroupDeleteController : ControllerBase
    {
        [HttpPost]
        public object FormAll(List<Info_EditGroupDeleteModel> toBeDelete)
        {
            string sql = "delete groups  where  ";

            bool isFirstAccount = true;
            foreach (var item in toBeDelete)
            {
                if (!string.IsNullOrEmpty(item.group_id))
                {
                    if (isFirstAccount)
                    {
                        sql += " Upper(GRPID)='" + item.group_id + "' ";
                    }
                    else
                    {
                        sql += " OR Upper(GRPID)='" + item.group_id + "' ";
                    }
                }
                isFirstAccount = false;
            }

            try
            {
                if (toBeDelete.Count != 0)
                {
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
                APCommonFun.Error("[EditGroupDeleteController]99：" + ex.ToString());
                APCommonFun.Error("[EditGroupDeleteController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
