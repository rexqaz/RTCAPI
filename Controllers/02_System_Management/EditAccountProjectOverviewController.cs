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
    public class EditAccountProjectOverviewController : ControllerBase
    {
        [HttpPost]
        public object FormAll(List<Info_EditAccountProjectOverviewModel> settings)
        {
            string sql = string.Empty;
            string sql2 = string.Empty;

            string tmp = String.Concat(settings.Select(o => o.account + ":" + o.project_id + "/"));
            APCommonFun.Log_Event("[EditAccountProjectOverviewController]98：" + tmp);

            try
            {
                foreach (var item in settings)
                {
                    if (!string.IsNullOrEmpty(item.project_id) && !string.IsNullOrEmpty(item.account))
                    {
                        sql2 = "select * from ACCOUNTPROJECTMAPPINGS where ACCID='" + item.account + "' AND PSPNR = (select PSPNR from ZCPST11 where PSPID='" + item.project_id + "' ) ";
                        DataTable dt = APCommonFun.GetDataTable(sql2);

                        string sql3 = "select PSPNR from ZCPST11 where PSPID = '" + item.project_id + "' ";
                        DataTable dt2 = APCommonFun.GetDataTable(sql3);
                        string PSPNR = string.Empty;
                        if (dt2.Rows.Count > 0)
                        {
                            PSPNR  = dt2.Rows[0][0].ToString();
                        }

                        if (item.is_view == "0")
                        {
                            if (dt.Rows.Count > 0)
                            {
                                sql = "delete ACCOUNTPROJECTMAPPINGS  where ACCID ='" + item.account + "' "
                               + " AND PSPNR = '" + PSPNR + "'  ";
                                APCommonFun.ExecSqlCommand(sql);
                                //APCommonFun.Log_Event("[EditAccountProjectOverviewController]98：" + sql);
                            }
                        }
                        else
                        {
                            if (dt.Rows.Count > 0)
                            {
                                sql = "update ACCOUNTPROJECTMAPPINGS SET  CANVIEWPIRESULT='" + item.is_check_project_merge + "' where ACCID ='" + item.account + "' "
                           + " AND PSPNR IN (select PSPNR from ZCPST11 where PSPID='" + item.project_id + "' ) ";
                                APCommonFun.ExecSqlCommand(sql);
                                //APCommonFun.Log_Event("[EditAccountProjectOverviewController]98：" + sql);
                            }
                            else
                            {
                                sql = "insert into ACCOUNTPROJECTMAPPINGS (ACCID, PSPNR, CANVIEWPIRESULT)  VALUES ('" + item.account + "' , '" + PSPNR + "' , '" + item.is_check_project_merge + "') ";
                                APCommonFun.ExecSqlCommand(sql);
                                //APCommonFun.Log_Event("[EditAccountProjectOverviewController]98：" + sql);
                            }
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
                APCommonFun.Error("[EditAccountProjectOverviewController]99：" + ex.ToString());
                APCommonFun.Error("[EditAccountProjectOverviewController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
