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
    public class ListConnectObjectController : ControllerBase
    {
        [HttpPost]
        public object FormAll()
        {
            JArray newJa = new JArray();

            string sql = "select DeptName,DeptID from Departments a \r\n\t\t\t\t\t\t\twhere a.CanBeContactedByCust =1 and a.DeptID in \r\n\t\t\t\t\t\t\t(select b.DeptID from Employees b where b.CanBeContactedByCust = 1) ";
            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string DeptName = APCommonFun.CDBNulltrim(dr["DeptName"].ToString());
                        string DeptID = APCommonFun.CDBNulltrim(dr["DeptID"].ToString());

                        JObject tmpJoLay01 = new JObject();

                        tmpJoLay01.Add(new JProperty("DeptName", DeptName));
                        tmpJoLay01.Add(new JProperty("DeptID", DeptID));
                        newJa.Add(tmpJoLay01);
                    }
                }

                return new
                {
                    Result = "T",
                    Message = "成功",
                    CommType = newJa
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[ListConnectObject]99：" + ex.ToString());
                APCommonFun.Error("[ListConnectObject]99：" + sql);

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
