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

namespace rc_interface_API.Controllers._01_Project_Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListCCTVController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListCCTVModel Data)
        {
            JArray newJa = new JArray();      

            string PSPNR = string.Empty;
            string whereStr = "where 1=1";

            if (Data.PSPNR != null)
            {
                PSPNR = APCommonFun.CDBNulltrim(Data.PSPNR);
                if (!string.IsNullOrEmpty(PSPNR))
                {
                    whereStr += " AND PSPNR='" + PSPNR + "' ";
                }
            }

            string sql = "SELECT PSPNR,CCTVURL,MSG FROM PORJECTCCTVURL " + whereStr;
            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string CCTV_URL = APCommonFun.CDBNulltrim(dr["CCTVURL"].ToString());
                        string description = APCommonFun.CDBNulltrim(dr["MSG"].ToString());

                        JObject tmpJoLay01 = new JObject();

                        tmpJoLay01.Add(new JProperty("CCTV_URL", CCTV_URL));
                        tmpJoLay01.Add(new JProperty("description", description));
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
                APCommonFun.Error("[ListCCTVController]99：" + ex.ToString());
                APCommonFun.Error("[ListCCTVController]99：" + sql);

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
