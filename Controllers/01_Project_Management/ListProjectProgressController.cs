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
    public class ListProjectProgressController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListProjectProgressModel Data)
        {
            JArray newJa = new JArray();            

            string PSPNR = "";
            if (Data.PSPNR != null) { PSPNR = APCommonFun.CDBNulltrim(Data.PSPNR); }

            //第一步 : 先判斷有沒有必填未填寫，
            string InputIsok = "Y";
            string ReturnErr = "";

            if (PSPNR == "") //必填                       
            {
                InputIsok = "N";
                ReturnErr = "執行動作錯誤-project_id 為必填欄位";
            }
            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[ListGroupRightsController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sqlProjName = $"select decode(substr(pspid,3,2),'11','潤營','12','評輝','13','潤輝','14','潤陽','21','潤安','31','潤弘','21','潤德') || ' - ' || POST1 as POST1 from ZCPST11 where PSPNR='{PSPNR}' ";

            string sqlExpected = $"select * from ZCPST16 where PSPNR = '{PSPNR}' and WRTTP = 'P1' ";
            string sqlActual = $"select * from ZCPST16 where PSPNR = '{PSPNR}' and WRTTP = 'P2' ";

            try
            {
                DataTable dtProjName = APCommonFun.GetDataTable(sqlProjName);
                DataTable dtExpected = APCommonFun.GetDataTable(sqlExpected);
                DataTable dtActual = APCommonFun.GetDataTable(sqlActual);

                JArray jaProgress = new JArray();
                JObject output = new JObject();

                output.Add(new JProperty("name", dtProjName.Rows[0]["POST1"].ToString()));

                foreach(DataRow drExpected in dtExpected.Rows)
                {
                    

                    string year = drExpected["GJAHR"].ToString();
                    DataRow[] listDrActual = dtActual.Select($"GJAHR = '{year}'");

                    for(int month = 1; month <= 12; month++)
                    {
                        string mm = month.ToString("D2");
                        string key = $"MEG0{mm}";
                        string expectedFromDb = APCommonFun.CDBNulltrim(drExpected[key].ToString());
                        string expected = expectedFromDb == "" ? "0" : expectedFromDb;
                        string actual = "0";
                        if (listDrActual.Length > 0)
                        {
                            string actualFromDb = APCommonFun.CDBNulltrim(listDrActual[0][key].ToString());
                            actual = actualFromDb == "" ? "0" : actualFromDb;
                        }

                        JObject row = new JObject();
                        row.Add(new JProperty("year", year));
                        row.Add(new JProperty("month", mm));
                        row.Add(new JProperty("expected", expected));
                        row.Add(new JProperty("actual", actual));

                        jaProgress.Add(row);
                    }
                }

                output.Add(new JProperty("progress", jaProgress));

                newJa.Add(output);
              
                return new
                {
                    Result = "T",
                    Message = "成功",
                    Data = newJa
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[ListGroupRightsController]99：" + ex.ToString());
                APCommonFun.Error("[ListGroupRightsController]99：" + sqlExpected);
                APCommonFun.Error("[ListGroupRightsController]99：" + sqlActual);

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
