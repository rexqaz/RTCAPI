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
    public class EditSeasonSatisfactionSurveyController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_EditSeasonSatisfactionSurveyModel input)
        {
            JArray newJa = new JArray();

            string project_id = "";
            if (input.PSPNR != null) { project_id = APCommonFun.CDBNulltrim(input.PSPNR); }
            string account_id = "";
            if (input.account_id != null) { account_id = APCommonFun.CDBNulltrim(input.account_id); }

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
                APCommonFun.Error("[EditSeasonSatisfactionSurveyController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            //string season = DateTime.Now.Year.ToString() + GetCurrentSeason().ToString();
            int season = GetProeSeason();
            string season2 = (season != 4) ? (DateTime.Today.Year.ToString() + season) : (DateTime.Today.AddYears(-1).Year.ToString() + season);
            string sql0 = "SELECT PROBEGROUPNAME, PROBEITEM, GRADE, POINT FROM SATISFYITEM";
            string sql2 = "select probegroupname, listagg(progeitem,'`') within group( order by probegroupname ) AS item ,listagg(GRADE,'`') within group( order by probegroupname ) AS grade  from satisfygrade where pgroupcode = '" + project_id + "' and season = '" + season2 + "' and ACCID = '" + account_id + "' group by probegroupname ";
            try
            {
                DataTable dt0 = APCommonFun.GetDataTable(sql0);
                DataTable dt2 = APCommonFun.GetDataTable(sql2);
                if (dt2.Rows.Count > 0)
                {
                    // update case:

                    string text = DateTime.Now.ToString("yyyyMMddhhmmss");
                    foreach(var SurveyClass in input.Data)
                    {
                        foreach (var SurveyItem in SurveyClass.item)
                        {
                            string sql = "update SatisfyGrade set filldate = '" + text + "',fillpoint = '" + SurveyItem.score + "'  where pgroupcode ='" + project_id + "' and probegroupname ='" + SurveyClass.header + "' and progeitem = '"  + SurveyItem.name + "' and season ='" + season2 + "' and ACCID = '" + account_id + "' ";
                            APCommonFun.ExecSqlCommand(sql);
                        }
                    }
                }
                else
                {
                    //insert case: 

                    string text = DateTime.Now.ToString("yyyyMMddhhmmss");
                    foreach (var SurveyClass in input.Data)
                    {
                        foreach (var SurveyItem in SurveyClass.item)
                        {
                            var dr0 = dt0.AsEnumerable().Where(r => r.Field<string>("PROBEGROUPNAME") == SurveyClass.header && r.Field<string>("PROBEITEM") == SurveyItem.name).ToList();
                            if (dr0.Count == 0) continue;
                            var item = dr0[0];
                            string sql = "insert into SatisfyGrade(pgroupcode, filldate, probegroupname,progeitem, grade, point, fillpoint, season, ACCID) values('" + project_id + "', '" + text + "', '" + SurveyClass.header + "', '" + SurveyItem.name + "', '" + item["GRADE"] + "', '" + item["POINT"] + "', '" + SurveyItem.score + "', '" + season2 + "', '" + account_id + "' )";
                            APCommonFun.ExecSqlCommand(sql);
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
                APCommonFun.Error("[EditSeasonSatisfactionSurveyController]99：" + ex.ToString());
                APCommonFun.Error("[EditSeasonSatisfactionSurveyController]99：" + sql2);

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
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
