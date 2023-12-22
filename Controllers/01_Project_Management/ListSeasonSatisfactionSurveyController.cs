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
    public class ListSeasonSatisfactionSurveyController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListSeasonSatisfactionSurveyModel Data)
        {
            JArray newJa = new JArray();

            string project_id = "";
            if (Data.PSPNR != null) { project_id = APCommonFun.CDBNulltrim(Data.PSPNR); }

            string account_id = "";
            if (Data.account_id != null) { account_id = APCommonFun.CDBNulltrim(Data.account_id); }

            //第一步 : 先判斷有沒有必填未填寫，
            string InputIsok = "Y";
            string ReturnErr = "";

            if (project_id == "") //必填                       
            {
                InputIsok = "N";
                ReturnErr = "執行動作錯誤-PSPNR 為必填欄位";
            }

            if (account_id == "") //必填                       
            {
                InputIsok = "N";
                ReturnErr = "執行動作錯誤-account_id 為必填欄位";
            }

            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[ListSeasonSatisfactionSurveyController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sql = "select post1 from ZCPST11 where PSPNR ='" + project_id + "' ";
            //string season = DateTime.Now.Year.ToString() + GetCurrentSeason().ToString();
            int season = GetProeSeason();
            string season2 = (season != 4) ? (DateTime.Today.Year.ToString() + season) : (DateTime.Today.AddYears(-1).Year.ToString() + season);
            //string sql2 = "select probegroupname, listagg(progeitem,'`') within group( order by probegroupname ) AS item ,listagg(fillpoint,'`') within group( order by probegroupname ) AS grade  from satisfygrade where pgroupcode = '" + project_id + "' and season = '" + season + "' group by probegroupname ";
            string sql2 = "SELECT i.PROBEGROUPNAME, "
                + "LISTAGG(i.PROBEITEM, '`') WITHIN GROUP(ORDER BY i.PROBEGROUPNAME) AS ITEM, "
                + "LISTAGG(NVL(g.FILLPOINT, 5), '`') WITHIN GROUP(ORDER BY i.PROBEGROUPNAME) AS GRADE FROM SATISFYITEM i "
                + "LEFT JOIN SATISFYGRADE g ON i.PROBEGROUPNAME = g.PROBEGROUPNAME AND i.PROBEITEM = g.PROGEITEM "
                + $"AND g.PGROUPCODE = '{project_id}' AND g.SEASON = {season2} AND g.ACCID = '{account_id}' "
                + "GROUP BY i.PROBEGROUPNAME";
            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);
                string project_name = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    project_name = dt.Rows[0]["POST1"].ToString();
                }

                DataTable dt2 = APCommonFun.GetDataTable(sql2);
                if (dt2.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt2.Rows)
                    {
                        string header = APCommonFun.CDBNulltrim(dr["probegroupname"].ToString());
                        string item = APCommonFun.CDBNulltrim(dr["ITEM"].ToString());
                        string score = APCommonFun.CDBNulltrim(dr["GRADE"].ToString());

                        string[] itemArray = item.Split('`');
                        string[] scoreArray = score.Split('`');

                        JArray newJa02 = new JArray(); //第二層
                        for (int i = 0; i < itemArray.Length; i++)
                        {
                            JObject tmpJoLay02 = new JObject();
                            tmpJoLay02.Add(new JProperty("name", itemArray[i]));
                            tmpJoLay02.Add(new JProperty("score", scoreArray[i]));
                            newJa02.Add(tmpJoLay02);
                        }

                        //第一層
                        JObject tmpJoLay01 = new JObject();
                        tmpJoLay01.Add(new JProperty("header", header));
                        tmpJoLay01.Add(new JProperty("item", newJa02));
                        newJa.Add(tmpJoLay01);
                    }
                }

                
                return new
                {
                    Result = "T",
                    Message = "成功",
                    Season = DateTime.Now.Year.ToString() + "年 第" + GetCurrentSeason().ToString() + "季",
                    Project_name = project_name,
                    Data = newJa
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[ListSeasonSatisfactionSurveyController]99：" + ex.ToString());
                APCommonFun.Error("[ListSeasonSatisfactionSurveyController]99：" + sql);
                APCommonFun.Error("[ListSeasonSatisfactionSurveyController]99：" + sql2);

                return new
                {
                    Result = "F",
                    Message = ex.ToString(),
                    Data = newJa
                };
            }

        }

        static public int GetCurrentSeason()
        {
            switch (DateTime.Now.Month)
            {
                case 1:
                case 2:
                case 3:
                    return 1;
                case 4:
                case 5:
                case 6:
                    return 2;
                case 7:
                case 8:
                case 9:
                    return 3;
                case 10:
                case 11:
                case 12:
                    return 4;
                default:
                    return 0;
            }
        }

        static public int GetProeSeason()
        {
            switch (DateTime.Now.Month)
            {
                case 1:
                case 2:
                case 3:
                    return 4;
                case 4:
                case 5:
                case 6:
                    return 1;
                case 7:
                case 8:
                case 9:
                    return 2;
                case 10:
                case 11:
                case 12:
                    return 3;
                default:
                    return 0;
            }
        }
    }
}
