using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
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
    public class FormCheckProjectSatifyExistsController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_FormCheckProjectSatifyExistsModel Data)
        {
            JArray newJa = new JArray();
            JArray newJa_Final = new JArray();
			string is_season_satisfaction_survey_done = string.Empty;


			string account_id = "";
            if (Data.account_id != null) { account_id = APCommonFun.CDBNulltrim(Data.account_id); }

            string PSPNR = "";
            if (Data.PSPNR != null) { PSPNR = APCommonFun.CDBNulltrim(Data.PSPNR); }


            //第一步 : 先判斷有沒有必填未填寫，
            string InputIsok = "Y";
            string ReturnErr = "";

            if (PSPNR == "") //必填                       
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
                APCommonFun.Error("[FormCheckProjectSatifyExistsController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

			int season = GetProeSeason();
			string season2 = (season != 4) ? (DateTime.Today.Year.ToString() + season) : (DateTime.Today.AddYears(-1).Year.ToString() + season);
			string sql = "select a.probegroupname,a.progeitem ,a.fillpoint  from satisfygrade a  where pgroupcode ='" + PSPNR + "' and season ='" + season2 + "' and ACCID = '" + account_id + "' "  ;

			try
			{
				DataTable dt = new DataTable();
				dt = APCommonFun.GetDataTable(sql);

				if (dt.Rows.Count > 0)
				{
					is_season_satisfaction_survey_done = "Y";
				}
				else
				{
					is_season_satisfaction_survey_done = "N";
				}
				
				return new
				{
					Result = "T",
					Message = "成功",
					is_season_satisfaction_survey_done = is_season_satisfaction_survey_done
				};
			}
			catch (Exception ex)
			{
				APCommonFun.Error("[FormCheckProjectSatifyExistsController]99：" + ex.ToString());
				APCommonFun.Error("[FormCheckProjectSatifyExistsController]99：" + sql);

				return new
				{
					Result = "F",
					Message = ex.ToString(),
					is_season_satisfaction_survey_done = is_season_satisfaction_survey_done
				};
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
