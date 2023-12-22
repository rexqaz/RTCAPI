using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using rc_interface_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rc_interface_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListProjectDocPicController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListProjectDocPicModel Data)
        {
            JArray newJa = new JArray();

            int yearS = 0;
            if (Data.yearS != null && !string.IsNullOrEmpty(Data.yearS)) { yearS = Convert.ToInt32(APCommonFun.CDBNulltrim(Data.yearS)); }

            int monthS = 0;
            if (Data.monthS != null && !string.IsNullOrEmpty(Data.monthS)) { monthS = Convert.ToInt32(APCommonFun.CDBNulltrim(Data.monthS)); }

            int weekS = 0;
            if (Data.weekS != null && !string.IsNullOrEmpty(Data.weekS)) { weekS = Convert.ToInt32(APCommonFun.CDBNulltrim(Data.weekS)); }

            int yearE = 0;
            if (Data.yearE != null && !string.IsNullOrEmpty(Data.yearE)) { yearE = Convert.ToInt32(APCommonFun.CDBNulltrim(Data.yearE)); }

            int monthE = 0;
            if (Data.monthE != null && !string.IsNullOrEmpty(Data.monthE)) { monthE = Convert.ToInt32(APCommonFun.CDBNulltrim(Data.monthE)); }

            int weekE = 0;
            if (Data.weekE != null && !string.IsNullOrEmpty(Data.weekE)) { weekE = Convert.ToInt32(APCommonFun.CDBNulltrim(Data.weekE)); }

            string doc_type = "";
            if (Data.doc_type != null) { doc_type = APCommonFun.CDBNulltrim(Data.doc_type); }

            string PSPNR = "";
            if (Data.PSPNR != null) { PSPNR = APCommonFun.CDBNulltrim(Data.PSPNR); }

            //第一步 : 先判斷有沒有必填未填寫，
            string InputIsok = "Y";
            string ReturnErr = "";

            if (yearS == 0 || monthS == 0) //必填                       
            {
                if (yearS == 0)
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-yearS 為必填欄位";
                }
                else
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-monthS 為必填欄位";
                }
            }

            if (yearE == 0 || monthE == 0) //必填                       
            {
                if (yearE == 0)
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-yearE 為必填欄位";
                }
                else
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-monthE 為必填欄位";
                }
            }

            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[ListProjectDocPicController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sql = GetSqlString(yearS, monthS, weekS, yearE, monthE, weekE, doc_type, PSPNR);
            try
            {

                DataTable dt = SearchProjectFile(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string category = ConvertFileType(APCommonFun.CDBNulltrim(dr["TYPE"].ToString()));
                        string item = APCommonFun.CDBNulltrim(dr["OPTIONS"].ToString());
                        string file = APCommonFun.CDBNulltrim(dr["FILEP"].ToString());

                        JObject tmpJoLay01 = new JObject();

                        tmpJoLay01.Add(new JProperty("category", category));
                        tmpJoLay01.Add(new JProperty("item", item));
                        tmpJoLay01.Add(new JProperty("file", file));
                        newJa.Add(tmpJoLay01);
                    }
                }

                return new
                {
                    Result = "T",
                    Message = "成功",
                    TotalRec = "2",
                    Data = newJa
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[ListProjectDocPicController]99：" + ex.ToString());
                APCommonFun.Error("[ListProjectDocPicController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString(),
                    Data = newJa
                };
            }

        }

        public string GetSqlString(int startYear, int startMonth, int startWeek, int endYear, int endMonth, int endWeek, string fileType, string projectID)
        {
            if (startWeek == 0) startWeek = 1;
            if (endWeek == 0) endWeek = 5;
            bool flag = false;
            bool flag2 = false;
            StringBuilder stringBuilder = new StringBuilder(2000);
            if (!string.IsNullOrEmpty(projectID))
            {
                stringBuilder.Append("select TYPE, OPTIONS, FILEP,dappl,PERIOD from zcpst14 where pspnr ='");
                stringBuilder.Append(projectID);
                stringBuilder.Append("' ");
            }
            else
            {
                stringBuilder.Append("select TYPE, OPTIONS, FILEP,dappl,PERIOD from zcpst14 where 1=1 ");
            }            
            stringBuilder.Append(" and type = '");
            stringBuilder.Append(fileType);
            stringBuilder.Append("' and substr(options,1,11) in (");
            int num = -1;
            int num2 = -1;
            while (startYear <= endYear)
            {
                flag = (startYear == endYear);
                num = ((num != -1) ? 1 : startMonth);
                while ((flag && startMonth <= endMonth) || (!flag && num <= 12))
                {
                    flag2 = ((flag && startMonth == endMonth) ? true : false);
                    num2 = ((num2 != -1) ? 1 : startWeek);
                    while ((flag2 && startWeek <= endWeek) || (!flag2 && num2 <= 5))
                    {
                        stringBuilder.Append("'");
                        stringBuilder.Append(startYear);
                        stringBuilder.Append("年");
                        int intValue = (!flag) ? num : startMonth;
                        stringBuilder.Append(ConvertIntToString(intValue));
                        stringBuilder.Append("月份第");
                        if (flag2)
                        {
                            stringBuilder.Append(ConvertIntToString(startWeek));
                        }
                        else
                        {
                            stringBuilder.Append(ConvertIntToString(num2));
                        }
                        if (ConvertIntToString(intValue).Length == 1)
                        {
                            stringBuilder.Append("週");
                        }
                        stringBuilder.Append("',");
                        if (flag2)
                        {
                            startWeek++;
                        }
                        else
                        {
                            num2++;
                        }
                    }
                    startWeek = 1;
                    if (flag)
                    {
                        startMonth++;
                    }
                    else
                    {
                        num++;
                    }
                }
                startMonth = 1;
                startYear++;
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append(")");
            stringBuilder.Append(" order by PERIOD desc, OPTIONS desc");
            return stringBuilder.ToString();
        }

        public  string ConvertFileType(object fileID)
        {
            switch (fileID.ToString())
            {
                case "001":
                    return "專案施工照片";
                case "002":
                    return "專案品質照片";
                case "003":
                    return "專案安衛照片";
                case "005":
                    return "專案會議記錄";
                case "006":
                    return "專案進度排程";
                case "007":
                    return "專案預算";
                case "008":
                    return "專案重要資訊";
                case "009":
                default:
                    return "專案其他資訊";
            }
        }

        public static string ConvertIntToString(int intValue)
        {
            switch (intValue)
            {
                case 1:
                    return "一";
                case 2:
                    return "二";
                case 3:
                    return "三";
                case 4:
                    return "四";
                case 5:
                    return "五";
                case 6:
                    return "六";
                case 7:
                    return "七";
                case 8:
                    return "八";
                case 9:
                    return "九";
                case 10:
                    return "十";
                case 11:
                    return "十一";
                case 12:
                    return "十二";
                default:
                    return "";
            }
        }

        public static string ConvertCaseToLower(string strCase)
        {
            switch (strCase)
            {
                case "一":
                    return "01";
                case "二":
                    return "02";
                case "三":
                    return "03";
                case "四":
                    return "04";
                case "五":
                    return "05";
                case "六":
                    return "06";
                case "七":
                    return "07";
                case "八":
                    return "08";
                case "九":
                    return "09";
                case "十":
                    return "10";
                case "十一":
                    return "11";
                case "十二":
                    return "12";
                default:
                    return "";
            }
        }

        public DataTable SearchProjectFile(string sql)
        {
            DataTable dataTable = APCommonFun.GetDataTable(sql);
            foreach (DataRow row in dataTable.Rows)
            {
                string text = row["OPTIONS"].ToString();
                string value = row["PERIOD"].ToString().Substring(4);
                bool flag = Convert.ToInt32(value) > 10;
                StringBuilder stringBuilder = new StringBuilder(text.Substring(0, 5));
                stringBuilder.Append(value);
                stringBuilder.Append("月第");
                if (flag)
                {
                    stringBuilder.Append(ConvertCaseToLower(text.Substring(10, 1)));
                    stringBuilder.Append("週");
                    stringBuilder.Append(text.Substring(12));
                }
                else
                {
                    stringBuilder.Append(ConvertCaseToLower(text.Substring(9, 1)));
                    stringBuilder.Append("週");
                    stringBuilder.Append(text.Substring(11));
                }
                row["OPTIONS"] = stringBuilder.ToString();
            }
            dataTable.AcceptChanges();
            return dataTable;
        }
    }
}
