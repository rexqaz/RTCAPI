using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.Controllers._01_Project_Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListConnectTypeController : ControllerBase
    {
        [HttpPost]
        public object FormAll()
        {
            JArray newJa = new JArray();

            string sql = "select  PROBEGROUPNAME  from SATISFYITEM group by PROBEGROUPNAME ";
            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string group_name = APCommonFun.CDBNulltrim(dr["PROBEGROUPNAME"].ToString());

                        JObject tmpJoLay01 = new JObject();

                        tmpJoLay01.Add(new JProperty("group_name", group_name));
                        newJa.Add(tmpJoLay01);
                    }
                }

                return new
                {
                    Result = "T",
                    Message = "成功",
                    Data = newJa
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[ListConnectType]99：" + ex.ToString());
                APCommonFun.Error("[ListConnectType]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString(),
                    Data = newJa
                };
            }

        }
    }
}
