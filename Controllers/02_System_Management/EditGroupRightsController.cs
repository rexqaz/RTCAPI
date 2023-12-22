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
    public class EditGroupRightsController : ControllerBase
    {
        [HttpPost]
        public object FormAll(List<Info_EditGroupRightsModel> inputs)
        {
            string sql = string.Empty;

            try
            {
                foreach (var item in inputs)
                {
                    if (!string.IsNullOrEmpty(item.group_id) && !string.IsNullOrEmpty(item.function_id))
                    {
                        string sqlCheckExists = $@"select * from groupfunctionsmappings where GRPID = '{item.group_id}' and FUNCID = '{item.function_id}'";

                        DataTable dt = APCommonFun.GetDataTable(sqlCheckExists);

                        if (dt.Rows.Count > 0)
                        {
                            sql = "update groupfunctionsmappings set CANREAD='" + item.read + "', CANINSERT='" + item.create
                             + "', CANUPDATE='" + item.update + "', CANDELETE='" + item.delete + "'  where GRPID='" + item.group_id + "' AND FUNCID= '" + item.function_id + "' ";

                            APCommonFun.ExecSqlCommand(sql);
                        }
                        else
                        {
                            sql = "insert into groupfunctionsmappings (GRPID, FUNCID, CANREAD, CANINSERT, CANUPDATE, CANDELETE)"
                                + $"VALUES ('{item.group_id}', '{item.function_id}', '{item.read}', '{item.create}', '{item.update}', '{item.delete}')";

                            APCommonFun.ExecSqlCommand(sql);
                        }

                        
                    }
                }

                return new
                {
                    Result = "T",
                    Message = "成功"
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[EditGroupRightsController]99：" + ex.ToString());
                APCommonFun.Error("[EditGroupRightsController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
