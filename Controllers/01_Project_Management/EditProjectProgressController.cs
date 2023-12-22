using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using rc_interface_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.Controllers._01_Project_Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditProjectProgressController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_EditProjectProgressModel Data)
        {
            //第一步 : 先判斷有沒有必填未填寫，
            string InputIsok = "Y";
            string ReturnErr = "";

            string PSPNR = "";
            string expected = "0";
            string actual = "0";
            if (Data.PSPNR != null) { PSPNR = APCommonFun.CDBNulltrim(Data.PSPNR); }
            if (Data.expected != null && Data.expected != "") { expected = APCommonFun.CDBNulltrim(Data.expected); }
            if (Data.actual != null && Data.actual != "") { actual = APCommonFun.CDBNulltrim(Data.actual); }

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

            //string sqlFindExpected = $"select * from ZCPST16 where WRTTP = 'P1' AND GJAHR = '{year}' AND PSPNR = '{PSPNR}' ";
            //string sqlFindActual = $"select * from ZCPST16 where WRTTP = 'P2' AND GJAHR = '{year}' AND PSPNR = '{PSPNR}' ";

            //string sqlUpdateExpected = "update ZCPST16 set MEG0" + month + "='" + expected  + "'  where WRTTP='P1' AND GJAHR='" + year + "' AND PSPNR='" + PSPNR + "' ";
            //string sqlUPdateActual = "update ZCPST16 set MEG0" + month + "='" + actual + "'  where WRTTP='P2' AND GJAHR='" + year + "' AND PSPNR='" + PSPNR + "' ";

            //string sqlInsertExpected = $"insert into ZCPST16 (PSPNR, GJAHR, WRTTP, MEG0{month}) VALUES ('{PSPNR}', '{year}', 'P1', )";

            string sqlUpsertExpected = $@"
                MERGE INTO ZCPST16 Z
                USING (SELECT '{PSPNR}' AS PSPNR, '{year}' AS GJAHR, 'P1' AS WRTTP FROM dual) D
                    ON
                        (Z.PSPNR = D.PSPNR AND Z.GJAHR = D.GJAHR AND Z.WRTTP = D.WRTTP)
                    WHEN MATCHED THEN
                        UPDATE SET Z.MEG0{month} = {expected} WHERE (Z.PSPNR = D.PSPNR AND Z.GJAHR = D.GJAHR AND Z.WRTTP = D.WRTTP)
                    WHEN NOT MATCHED THEN
                        INSERT (MANDT, PSPNR, GJAHR, WRTTP, MEG0{month}) VALUES (160, D.PSPNR, D.GJAHR, D.WRTTP, {expected})
            ";
            string sqlUpsertActual = $@"
                MERGE INTO ZCPST16 Z
                USING (SELECT '{PSPNR}' AS PSPNR, '{year}' AS GJAHR, 'P2' AS WRTTP FROM dual) D
                    ON
                        (Z.PSPNR = D.PSPNR AND Z.GJAHR = D.GJAHR AND Z.WRTTP = D.WRTTP)
                    WHEN MATCHED THEN
                        UPDATE SET Z.MEG0{month} = {actual} WHERE (Z.PSPNR = D.PSPNR AND Z.GJAHR = D.GJAHR AND Z.WRTTP = D.WRTTP)
                    WHEN NOT MATCHED THEN
                        INSERT (MANDT, PSPNR, GJAHR, WRTTP, MEG0{month}) VALUES (160, D.PSPNR, D.GJAHR, D.WRTTP, {actual})
            ";

            try
            {
                APCommonFun.ExecSqlCommand(sqlUpsertExpected);
                APCommonFun.ExecSqlCommand(sqlUpsertActual);                
                
                return new
                {
                    Result = "T",
                    Message = "成功"
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[EditCsiRequestProgressController]99：" + ex.ToString());
                APCommonFun.Error("[EditCsiRequestProgressController]99：" + sqlUpsertExpected);
                APCommonFun.Error("[EditCsiRequestProgressController]99：" + sqlUpsertActual);

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
