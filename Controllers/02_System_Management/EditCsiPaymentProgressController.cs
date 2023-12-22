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
    public class EditCsiPaymentProgressController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_EditCsiPaymentProgressModel Data)
        {
            //第一步 : 先判斷有沒有必填未填寫，
            string InputIsok = "Y";
            string ReturnErr = "";

            string PSPNR = "";
            float target_progress = 0;
            float current_progress = 0;
            if (Data.PSPNR != null) { PSPNR = APCommonFun.CDBNulltrim(Data.PSPNR); }
            if (Data.target_progress != null) { target_progress = (float)Convert.ToInt64(APCommonFun.CDBNulltrim(Data.target_progress)) / 100; }
            if (Data.current_progress != null) { current_progress = (float)Convert.ToInt64(APCommonFun.CDBNulltrim(Data.current_progress)) / 100; }

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
                    APCommonFun.Error("[EditCsiPaymentProgressController]90-" + ReturnErr);
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
                APCommonFun.Error("[EditCsiPaymentProgressController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sqlUpsertExpected = $@"
                MERGE INTO ZCPST15 Z
                USING (SELECT '{PSPNR}' AS PSPNR, '{year}' AS GJAHR, '01' AS WRTTP FROM dual) D
                    ON
                        (Z.PSPNR = D.PSPNR AND Z.GJAHR = D.GJAHR AND Z.WRTTP = D.WRTTP)
                    WHEN MATCHED THEN
                        UPDATE SET Z.WLP{month} = {target_progress}, Z.WTP{month} = {target_progress} WHERE (Z.PSPNR = D.PSPNR AND Z.GJAHR = D.GJAHR AND Z.WRTTP = D.WRTTP)
                    WHEN NOT MATCHED THEN
                        INSERT (MANDT, PSPNR, GJAHR, WRTTP, WLP{month}, WTP{month}) VALUES (160, D.PSPNR, D.GJAHR, D.WRTTP, {target_progress}, {target_progress})
            ";
            string sqlUpsertActual = $@"
                MERGE INTO ZCPST15 Z
                USING (SELECT '{PSPNR}' AS PSPNR, '{year}' AS GJAHR, '04' AS WRTTP FROM dual) D
                    ON
                        (Z.PSPNR = D.PSPNR AND Z.GJAHR = D.GJAHR AND Z.WRTTP = D.WRTTP)
                    WHEN MATCHED THEN
                        UPDATE SET Z.WLP{month} = {current_progress}, Z.WTP{month} = {current_progress} WHERE (Z.PSPNR = D.PSPNR AND Z.GJAHR = D.GJAHR AND Z.WRTTP = D.WRTTP)
                    WHEN NOT MATCHED THEN
                        INSERT (MANDT, PSPNR, GJAHR, WRTTP, WLP{month}, WTP{month}) VALUES (160, D.PSPNR, D.GJAHR, D.WRTTP, {current_progress}, {current_progress})
            ";

            string sqlUpsertExpected2 = $@"
                MERGE INTO ZCPST15T Z
                USING (SELECT '{PSPNR}' AS PSPNR, '{year}' AS GJAHR, '01' AS WRTTP FROM dual) D
                    ON
                        (Z.PSPNR = D.PSPNR AND Z.GJAHR = D.GJAHR AND Z.WRTTP = D.WRTTP)
                    WHEN MATCHED THEN
                        UPDATE SET Z.WLP{month} = {target_progress}, Z.WTP{month} = {target_progress} WHERE (Z.PSPNR = D.PSPNR AND Z.GJAHR = D.GJAHR AND Z.WRTTP = D.WRTTP)
                    WHEN NOT MATCHED THEN
                        INSERT (MANDT, PSPNR, GJAHR, WRTTP, WLP{month}, WTP{month}) VALUES (160, D.PSPNR, D.GJAHR, D.WRTTP, {target_progress}, {target_progress})
            ";
            string sqlUpsertActual2 = $@"
                MERGE INTO ZCPST15T Z
                USING (SELECT '{PSPNR}' AS PSPNR, '{year}' AS GJAHR, '04' AS WRTTP FROM dual) D
                    ON
                        (Z.PSPNR = D.PSPNR AND Z.GJAHR = D.GJAHR AND Z.WRTTP = D.WRTTP)
                    WHEN MATCHED THEN
                        UPDATE SET Z.WLP{month} = {current_progress}, Z.WTP{month} = {current_progress} WHERE (Z.PSPNR = D.PSPNR AND Z.GJAHR = D.GJAHR AND Z.WRTTP = D.WRTTP)
                    WHEN NOT MATCHED THEN
                        INSERT (MANDT, PSPNR, GJAHR, WRTTP, WLP{month}, WTP{month}) VALUES (160, D.PSPNR, D.GJAHR, D.WRTTP, {current_progress}, {current_progress})
            ";

            //string sq11Select = "select * from zcpst15 where WRTTP='01' AND GJAHR='" + year + "' AND PSPNR='" + PSPNR + "' ";
            //string sq12Select = "select * from zcpst15 where WRTTP='04' AND GJAHR='" + year + "' AND PSPNR='" + PSPNR + "' ";
            //string sql1Update = "update zcpst15 set WLP" + month + "='-" + target_progress + "'  where WRTTP='01' AND GJAHR='" + year + "' AND PSPNR='" + PSPNR + "' ";
            //string sql2Update = "update zcpst15 set WLP" + month + "='-" + current_progress + "' where WRTTP='04' AND GJAHR='" + year + "' AND PSPNR='" + PSPNR + "' ";
            //string sql1Insert = "insert into zcpst15 values (MANDT, PSPNR, WRTTP, GJAHR, WLP" + month + ") values ('160', '" + PSPNR + "', '01', '" + year + "', '-" + target_progress + "' ";
            //string sql2Insert = "insert into zcpst15 values (MANDT, PSPNR, WRTTP, GJAHR, WLP" + month + ") values ('160', '" + PSPNR + "', '04', '" + year + "', '-" + current_progress + "' ";

            //string sq13Select = "select * from zcpst15T where WRTTP='01' AND GJAHR='" + year + "' AND PSPNR='" + PSPNR + "' ";
            //string sq14Select = "select * from zcpst15T where WRTTP='04' AND GJAHR='" + year + "' AND PSPNR='" + PSPNR + "' ";
            //string sql3Update = "update zcpst15T set WLP" + month + "='-" + target_progress + "'  where WRTTP='01' AND GJAHR='" + year + "' AND PSPNR='" + PSPNR + "' ";
            //string sql4Update = "update zcpst15T set WLP" + month + "='-" + current_progress + "' where WRTTP='04' AND GJAHR='" + year + "' AND PSPNR='" + PSPNR + "' ";
            //string sql3Insert = "insert into zcpst15T values (MANDT, PSPNR, WRTTP, GJAHR, WLP" + month + ") values ('160', '" + PSPNR + "', '01', '" + year + "', '-" + target_progress + "' ";
            //string sql4Insert = "insert into zcpst15T values (MANDT, PSPNR, WRTTP, GJAHR, WLP" + month + ") values ('160', '" + PSPNR + "', '04', '" + year + "', '-" + current_progress + "' ";

            try
            {
                APCommonFun.ExecSqlCommand(sqlUpsertExpected);
                APCommonFun.ExecSqlCommand(sqlUpsertActual);
                APCommonFun.ExecSqlCommand(sqlUpsertExpected2);
                APCommonFun.ExecSqlCommand(sqlUpsertActual2);
                //DataTable dt1 = APCommonFun.GetDataTable(sq11Select);
                //if (dt1.Rows.Count > 0)
                //{
                //    APCommonFun.ExecSqlCommand(sql1Update);
                //}
                //else
                //{
                //    APCommonFun.ExecSqlCommand(sql1Insert);
                //}

                //DataTable dt2 = APCommonFun.GetDataTable(sq12Select);
                //if (dt2.Rows.Count > 0)
                //{
                //    APCommonFun.ExecSqlCommand(sql2Update);
                //}
                //else
                //{
                //    APCommonFun.ExecSqlCommand(sql2Insert);
                //}

                //DataTable dt3 = APCommonFun.GetDataTable(sq13Select);
                //if (dt3.Rows.Count > 0)
                //{
                //    APCommonFun.ExecSqlCommand(sql3Update);
                //}
                //else
                //{
                //    APCommonFun.ExecSqlCommand(sql3Insert);
                //}

                //DataTable dt4 = APCommonFun.GetDataTable(sq14Select);
                //if (dt4.Rows.Count > 0)
                //{
                //    APCommonFun.ExecSqlCommand(sql4Update);
                //}
                //else
                //{
                //    APCommonFun.ExecSqlCommand(sql4Insert);
                //}

                return new
                {
                    Result = "T",
                    Message = "成功"
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[EditCsiPaymentProgressController]99：" + ex.ToString());
                APCommonFun.Error("[EditCsiPaymentProgressController]99：" + sqlUpsertExpected);
                APCommonFun.Error("[EditCsiPaymentProgressController]99：" + sqlUpsertActual);
                APCommonFun.Error("[EditCsiPaymentProgressController]99：" + sqlUpsertExpected2);
                APCommonFun.Error("[EditCsiPaymentProgressController]99：" + sqlUpsertActual2);
                
                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
