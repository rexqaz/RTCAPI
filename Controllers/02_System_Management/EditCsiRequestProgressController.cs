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
    public class EditCsiRequestProgressController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_EditCsiRequestProgressModel Data)
        {
            //第一步 : 先判斷有沒有必填未填寫，
            string InputIsok = "Y";
            string ReturnErr = "";

            string PSPNR = "";
            string target_progress = "0";
            string current_progress = "0";
            if (Data.PSPNR != null) { PSPNR = APCommonFun.CDBNulltrim(Data.PSPNR); }
            if (Data.target_progress != null && Data.target_progress != "") { target_progress = APCommonFun.CDBNulltrim(Data.target_progress); }
            if (Data.current_progress != null && Data.current_progress != "") { current_progress = APCommonFun.CDBNulltrim(Data.current_progress); }

            if (PSPNR == "") //必填                       
            {
                InputIsok = "N";
                ReturnErr = "執行動作錯誤-PSPNR 為必填欄位";
            }

            string date = string.Empty;
            string year = string.Empty;
            string month = string.Empty;
            if (Data.date != null && !string.IsNullOrEmpty(Data.date))
            {
                date = APCommonFun.CDBNulltrim(Data.date);
                if (!date.Contains("-"))
                {
                    ReturnErr = "執行動作錯誤-date 欄位格式錯誤";
                    APCommonFun.Error("[EditCsiRequestProgressController]90-" + ReturnErr);
                    return new
                    {
                        Result = "R",
                        Message = ReturnErr,
                        Data = ""
                    };
                }
                else
                {
                    string[] dates = date.Split('-');
                    year = dates[0].ToString();
                    month = dates[1].ToString();
                }
            }

            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[EditCsiRequestProgressController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sql1 = "update ZCPST16 set MEG0" + month + "='" + target_progress  + "'  where WRTTP='P1' AND GJAHR='" + year + "' AND PSPNR='" + PSPNR + "' ";
            string sql2 = "update ZCPST16 set MEG0" + month + "='" + current_progress + "'  where WRTTP='P2' AND GJAHR='" + year + "' AND PSPNR='" + PSPNR + "' ";

            try
            {
                APCommonFun.ExecSqlCommand(sql1);
                APCommonFun.ExecSqlCommand(sql2);                
                
                return new
                {
                    Result = "T",
                    Message = "成功"
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[EditCsiRequestProgressController]99：" + ex.ToString());
                APCommonFun.Error("[EditCsiRequestProgressController]99：" + sql1);
                APCommonFun.Error("[EditCsiRequestProgressController]99：" + sql2);

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
