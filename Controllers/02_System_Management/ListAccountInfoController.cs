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
    public class ListAccountInfoController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_AccountInfoModel Data)
        {
            JObject tmpJoLay01 = new JObject();

            string account = "";
            if (Data.account != null) { account = APCommonFun.CDBNulltrim(Data.account); }

            //第一步 : 先判斷有沒有必填未填寫，
            string InputIsok = "Y";
            string ReturnErr = "";

            if (account == "") //必填                       
            {
                InputIsok = "N";
                ReturnErr = "執行動作錯誤-account 為必填欄位";
            }
            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[ListAccountInfoController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sql = " select * from ACCOUNTS a left join  GROUPS g on g.GRPID = a.GRPID "
                         + "left join "
                         + "( "
                         + "select* from EMPLOYEES e join DEPARTMENTS d on d.DEPTID = e.DEPTID "
                         + " ) ee on a.EMPID = ee.EMPID "
                         + " where a.ACCID='" + account + "' ";

            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string ACCID = APCommonFun.CDBNulltrim(dr["ACCID"].ToString());
                        string GRPID = APCommonFun.CDBNulltrim(dr["GRPID"].ToString());
                        string ISEMPLOYEE = APCommonFun.CDBNulltrim(dr["ISEMPLOYEE"].ToString());
                        string EMPID = APCommonFun.CDBNulltrim(dr["EMPID"].ToString());
                        string EMPCHINAME = APCommonFun.CDBNulltrim(dr["EMPCHINAME"].ToString());
                        string DEPTNAME = APCommonFun.CDBNulltrim(dr["DEPTNAME"].ToString());
                        string ACCDES = APCommonFun.CDBNulltrim(dr["ACCDES"].ToString());

                        tmpJoLay01.Add(new JProperty("is_reset_password", "0"));
                        tmpJoLay01.Add(new JProperty("new_password", ""));
                        tmpJoLay01.Add(new JProperty("confirm_new_password", ""));
                        tmpJoLay01.Add(new JProperty("group", GRPID));
                        tmpJoLay01.Add(new JProperty("is_employee", ISEMPLOYEE));
                        tmpJoLay01.Add(new JProperty("employee_id", EMPID));
                        tmpJoLay01.Add(new JProperty("employee_name", EMPCHINAME));
                        tmpJoLay01.Add(new JProperty("department", DEPTNAME));
                        tmpJoLay01.Add(new JProperty("note", ACCDES));
                    }
                }

                return new
                {
                    Result = "T",
                    Message = "成功",
                    Data = tmpJoLay01
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[ListAccountInfoController]99：" + ex.ToString());
                APCommonFun.Error("[ListAccountInfoController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString(),
                    Data = tmpJoLay01
                };
            }

        }
    }
}
