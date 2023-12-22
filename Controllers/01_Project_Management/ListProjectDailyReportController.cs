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

namespace rc_interface_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListProjectDailyReportController : ControllerBase
    {
        private string mStrFuncName = "ListProjectDailyReportController";
        [HttpPost]
        public object FormAll(Info_ProjectDailyReportModel Data)
        {
            string _year = "";
            if (Data.year != null) { _year = APCommonFun.CDBNulltrim(Data.year); }

            string _month = "";
            if (Data.month != null) { _month = APCommonFun.CDBNulltrim(Data.month); }

            string _day = "";
            if (Data.day != null) { _day = APCommonFun.CDBNulltrim(Data.day); }

            string account_id = "";
            if (Data.account_id != null) { account_id = APCommonFun.CDBNulltrim(Data.account_id); }

            string project_id = "";
            if (Data.project_id != null) { project_id = APCommonFun.CDBNulltrim(Data.project_id); }

            if (project_id.Length <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No project_id is specified.", "R", new JArray());
            }

            if (_year.Length <= 0 || _month.Length <= 0 || _day.Length <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "The YEAR, MONTH, DAY may be not specified.", "R", new JArray());
            }
            int year = 0, month = 0, day = 0;
            try
            {
                year = int.Parse(_year);
                month = int.Parse(_month);
                day = int.Parse(_day);

                ////可能傳入為民國
                //if (year < 1911)
                //    year += 1911;
                project_id = project_id.Trim();
                account_id = account_id.Trim();
            }
            catch {
                return APCommonFun.ReturnError(mStrFuncName, "The YEAR, MONTH, DAY have errors.", "R", new JArray());
            }
            OracleConnection conn = APCommonFun.GetOracleConnection();
            if (conn == null)
            {
                return APCommonFun.ReturnError(mStrFuncName, "Oracle connecting fault.", "R", new JArray());
            }

            string sqlcommand = "";

            OracleCommand cmd = new OracleCommand(sqlcommand, conn);
            cmd.CommandType = CommandType.Text;
            DataSet _ds = new DataSet();
            bool _canviewsummary = true;
            if(account_id.Length > 0)
            {
                _canviewsummary = false;
                //檢查使用者權限
                sqlcommand = "SELECT CANVIEWPIRESULT FROM accountprojectmappings WHERE upper(ACCID) = '" + account_id.ToUpper() + "' AND upper(PSPNR) = '" + project_id.ToUpper() + "'";
                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;
                object _obj = cmd.ExecuteScalar();
                if(_obj.ToString().Trim() == "1")
                {
                    _canviewsummary = true;
                }
            }

            if(!_canviewsummary)
            {
                try { conn.Close(); } catch { }
                return APCommonFun.ReturnError(mStrFuncName, "Account id:" + account_id + " cannot view the report!", "F", new JArray());
            }
            DateTime selDate = new DateTime(year, month, day);

            sqlcommand =  "SELECT A.PSPID , A.POST1 , B.BLDAT , A.ESTRT , A.EENDE,";
            sqlcommand += "A.EENDE,A.ESTRT,";
            sqlcommand += "B.BLDAT,";
            sqlcommand += " B.ZWEATHER1 , B.ZWEATHER2 , B.ZWEATHER3 FROM  ZCPST11 A,";
            sqlcommand += " ZCPST02 B WHERE A.PSPNR = B.PSPNR(+) AND ";
            //sqlcommand += " (B.BLDAT = '" + BLDAT.BLDAT + "' or B.BLDAT IS NULL)";
            //sqlcommand += " AND A.PSPNR = '" + BLDAT.PSPNR + "' ";
            //date: 20210927
            sqlcommand += " (B.BLDAT = '" +string.Format("{0:yyyyMMdd}", selDate) + "' or B.BLDAT IS NULL)";
            sqlcommand += " AND upper(A.PSPNR) = '" + project_id.ToUpper() + "' ";


            try
            {
                JArray jr = new JArray();

                //主表
                _ds = new DataSet();
                int _recgot = 0;
                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;
                using (OracleDataAdapter _da = new OracleDataAdapter())
                {
                    _da.SelectCommand = cmd;
                    _recgot = _da.Fill(_ds);
                }


                //職別
                DataSet _dsjob = new DataSet();
                
                int _recjob = 0;

                sqlcommand = "SELECT A.WRKST,A.ZCONDITION FROM ZCPST03 A WHERE ";
                //sqlcommand += " A.BLDAT = '" + BLDAT.BLDAT + "'";
                //sqlcommand += " AND A.PSPNR = '" + BLDAT.PSPNR + "' ";
                sqlcommand += " A.BLDAT = '" + string.Format("{0:yyyyMMdd}", selDate) + "'";
                sqlcommand += " AND upper(A.PSPNR) = '" + project_id.ToUpper() + "' ";

                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;
                using (OracleDataAdapter _da = new OracleDataAdapter())
                {
                    _da.SelectCommand = cmd;
                    _recjob = _da.Fill(_dsjob);
                }

                //施工機具
                DataSet _dsmachanic = new DataSet();
                int _recmachanic = 0;

                sqlcommand = "SELECT A.WRKST,A.ZCONDITION FROM ZCPST05 A WHERE ";
                //sqlcommand += " A.BLDAT = '" + BLDAT.BLDAT + "'";
                //sqlcommand += " AND A.PSPNR = '" + BLDAT.PSPNR + "'";
                sqlcommand += " A.BLDAT = '" + string.Format("{0:yyyyMMdd}", selDate) + "'";
                sqlcommand += " AND upper(A.PSPNR) = '" + project_id.ToUpper() + "' ";

                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;
                using (OracleDataAdapter _da = new OracleDataAdapter())
                {
                    _da.SelectCommand = cmd;
                    _recmachanic = _da.Fill(_dsmachanic);
                }

                //進場材料
                DataSet _dsmaterial = new DataSet();
                int _recmat = 0;
                sqlcommand = "SELECT M.MAKTX,A.MEINS,A.LTXA1 FROM ZCPST07 A,MAKT M WHERE";
                //sqlcommand += " WHERE A.BLDAT = '" + BLDAT.BLDAT + "'";
                //sqlcommand += " AND A.PSPNR = '" + BLDAT.PSPNR + "'";
                sqlcommand += " A.BLDAT = '" + string.Format("{0:yyyyMMdd}", selDate) + "'";
                sqlcommand += " AND upper(A.PSPNR) = '" + project_id.ToUpper() + "' ";
                sqlcommand += " AND A.MATNR = M.MATNR ";
                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;
                using (OracleDataAdapter _da = new OracleDataAdapter())
                {
                    _da.SelectCommand = cmd;
                    _recmat = _da.Fill(_dsmaterial);
                }

                //其它
                DataSet _dsother = new DataSet();
                int _recother = 0;

                sqlcommand = "SELECT A.ZTEXT0,A.ZTEXT1,A.ZTEXT2,A.ZTEXT3 FROM ZCPST02 A WHERE ";
                //sqlcommand += " WHERE A.BLDAT = '" + BLDAT.BLDAT + "'";
                //sqlcommand += " AND A.PSPNR = '" + BLDAT.PSPNR + "'";
                sqlcommand += " A.BLDAT = '" + string.Format("{0:yyyyMMdd}", selDate) + "'";
                sqlcommand += " AND upper(A.PSPNR) = '" + project_id.ToUpper() + "' ";

                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;
                using (OracleDataAdapter _da = new OracleDataAdapter())
                {
                    _da.SelectCommand = cmd;
                    _recother = _da.Fill(_dsother);
                }

                if (_recgot > 0)
                {
                    JObject tmpJoLay02 = new JObject();
                    //
                    DateTime dtproj = selDate;

                    try
                    {
                        dtproj = DateTime.Parse(_ds.Tables[0].Rows[0][2].ToString());
                    }
                    catch {
                        //若資料庫的日期異常, 使用查詢的日期
                        dtproj = selDate;
                    }
                    string _tmp = _ds.Tables[0].Rows[0][0] == null ? "" : _ds.Tables[0].Rows[0][0].ToString() +
                                  _ds.Tables[0].Rows[0][1] == null ? "" : _ds.Tables[0].Rows[0][1].ToString().Trim();
                    tmpJoLay02.Add(new JProperty("PrjName", _tmp));
                    tmpJoLay02.Add(new JProperty("NowData", string.Format("{0:yyyy-MM-dd} {1}", dtproj, WhatDay(dtproj))));

                    _tmp = _ds.Tables[0].Rows[0][3] == null ? "" : _ds.Tables[0].Rows[0][3].ToString().Trim();
                    if(_tmp.Length == 8)
                        _tmp = _tmp.Substring(0, 4) + "-" + _tmp.Substring(4, 2) + "-" + _tmp.Substring(6, 2);
                    tmpJoLay02.Add(new JProperty("SDate", _tmp));

                    _tmp = _ds.Tables[0].Rows[0][4] == null ? "" : _ds.Tables[0].Rows[0][4].ToString().Trim();
                    if (_tmp.Length == 8)
                        _tmp = _tmp.Substring(0, 4) + "-" + _tmp.Substring(4, 2) + "-" + _tmp.Substring(6, 2);
                    tmpJoLay02.Add(new JProperty("EDate", _tmp));

                    string _alldate = GetSumDate(_ds.Tables[0].Rows[0][5], _ds.Tables[0].Rows[0][6]);
                    string _sumData = GetSumDate(_ds.Tables[0].Rows[0][7], _ds.Tables[0].Rows[0][6]);
                    tmpJoLay02.Add(new JProperty("AllDate", _alldate + "天"));
                    tmpJoLay02.Add(new JProperty("WeathAM", JudgeWeather(_ds.Tables[0].Rows[0][8])));
                    tmpJoLay02.Add(new JProperty("WeathPM", JudgeWeather(_ds.Tables[0].Rows[0][9])));
                    tmpJoLay02.Add(new JProperty("SumData", _sumData + "天"));
                    tmpJoLay02.Add(new JProperty("DataP", DatePencent(_sumData, _alldate) + "%"));
                    tmpJoLay02.Add(new JProperty("WeathEM", JudgeWeather(_ds.Tables[0].Rows[0][10])));

                    //職別
                    if(_recjob > 0)
                    {
                        JArray ja = new JArray();
                        for (int jindex = 0; jindex < _recjob;jindex++)
                        {
                            JObject _item = new JObject();
                            //職別
                            _item.Add(new JProperty("WRKST", _dsjob.Tables[0].Rows[jindex][0] == null ? "" : _dsjob.Tables[0].Rows[jindex][0].ToString()));
                            //施工狀況
                            _item.Add(new JProperty("ZCONDITION", _dsjob.Tables[0].Rows[jindex][1] == null ? "" : _dsjob.Tables[0].Rows[jindex][1].ToString()));
                            ja.Add(_item);
                        }
                        tmpJoLay02.Add(new JProperty("Jobs", ja));
                    }
                    else
                        tmpJoLay02.Add(new JProperty("Jobs", new JArray()));

                    //施工機具
                    if(_recmachanic > 0)
                    {
                        JArray ja = new JArray();
                        for (int jindex = 0; jindex < _recmachanic; jindex++)
                        {
                            JObject _item = new JObject();
                            //施工機具
                            _item.Add(new JProperty("WRKST", _dsmachanic.Tables[0].Rows[jindex][0] == null ? "" : _dsmachanic.Tables[0].Rows[jindex][0].ToString()));
                            //施工狀況
                            _item.Add(new JProperty("ZCONDITION", _dsmachanic.Tables[0].Rows[jindex][1] == null ? "" : _dsmachanic.Tables[0].Rows[jindex][1].ToString()));

                            ja.Add(_item);
                        }
                        tmpJoLay02.Add(new JProperty("Machanic", ja));
                    }
                    else
                        tmpJoLay02.Add(new JProperty("Machanic", new JArray()));

                    //進場材料
                    if(_recmat > 0)
                    {
                        JArray ja = new JArray();
                        for (int jindex = 0; jindex < _recmat; jindex++)
                        {
                            JObject _item = new JObject();
                            //進場材料
                            _item.Add(new JProperty("MAKTX", _dsmaterial.Tables[0].Rows[jindex][0] == null ? "" : _dsmaterial.Tables[0].Rows[jindex][0].ToString()));
                            //單位
                            _item.Add(new JProperty("MEINS", _dsmaterial.Tables[0].Rows[jindex][1] == null ? "" : _dsmaterial.Tables[0].Rows[jindex][1].ToString()));
                            //施作位置
                            _item.Add(new JProperty("LTXA1", _dsmaterial.Tables[0].Rows[jindex][2] == null ? "" : _dsmaterial.Tables[0].Rows[jindex][2].ToString()));

                            ja.Add(_item);
                        }
                        tmpJoLay02.Add(new JProperty("Matiral", ja));
                    }
                    else 
                        tmpJoLay02.Add(new JProperty("Matiral", new JArray()));

                    //其它
                    if(_recother <= 0)
                    {
                        tmpJoLay02.Add(new JProperty("YZ", ""));    //業主支援
                        tmpJoLay02.Add(new JProperty("GS", ""));    //公司支援
                        tmpJoLay02.Add(new JProperty("PZ", ""));    //品質
                        tmpJoLay02.Add(new JProperty("AW", ""));    //安衛
                    }
                    else
                    {
                        tmpJoLay02.Add(new JProperty("YZ", _dsother.Tables[0].Rows[0][0] == null ? "" : _dsother.Tables[0].Rows[0][0].ToString()));    //業主支援
                        tmpJoLay02.Add(new JProperty("GS", _dsother.Tables[0].Rows[0][1] == null ? "" : _dsother.Tables[0].Rows[0][1].ToString()));    //公司支援
                        tmpJoLay02.Add(new JProperty("PZ", _dsother.Tables[0].Rows[0][2] == null ? "" : _dsother.Tables[0].Rows[0][2].ToString()));    //品質
                        tmpJoLay02.Add(new JProperty("AW", _dsother.Tables[0].Rows[0][3] == null ? "" : _dsother.Tables[0].Rows[0][3].ToString()));    //安衛
                    }

                    jr.Add(tmpJoLay02);
                    return APCommonFun.ReturnSuccess(jr, 1);
                }
                else
                {
                    //no data , return empty data
                    JObject tmpJoLay02 = new JObject();

                    tmpJoLay02.Add(new JProperty("PrjName", "No data."));
                    tmpJoLay02.Add(new JProperty("NowData", ""));
                    tmpJoLay02.Add(new JProperty("SDate", ""));
                    tmpJoLay02.Add(new JProperty("EDate", ""));
                    tmpJoLay02.Add(new JProperty("AllDate", ""));
                    tmpJoLay02.Add(new JProperty("WeathAM", ""));
                    tmpJoLay02.Add(new JProperty("WeathPM", ""));
                    tmpJoLay02.Add(new JProperty("SumData", ""));
                    tmpJoLay02.Add(new JProperty("DataP", ""));
                    tmpJoLay02.Add(new JProperty("WeathEM", ""));

                    tmpJoLay02.Add(new JProperty("Jobs", new JArray()));
                    tmpJoLay02.Add(new JProperty("Machanic", new JArray()));
                    tmpJoLay02.Add(new JProperty("Matiral", new JArray()));

                    tmpJoLay02.Add(new JProperty("YZ", ""));    //業主支援
                    tmpJoLay02.Add(new JProperty("GS", ""));    //公司支援
                    tmpJoLay02.Add(new JProperty("PZ", ""));    //品質
                    tmpJoLay02.Add(new JProperty("AW", ""));    //安衛

                    jr.Add(tmpJoLay02);
                    return APCommonFun.ReturnSuccess(jr, 1);
                    //return new
                    //{
                    //    Result = "T",
                    //    Message = "No data!",
                    //    Data = jr
                    //};
                }
            }
            catch (Exception ex)
            {
                try { conn.Close(); } catch { }
                return APCommonFun.ReturnError(mStrFuncName, "Common exception.", ex.ToString(), "F", new JArray());
            }


            

            //try
            //{
            //    JObject tmpJoLay02 = new JObject();

            //    tmpJoLay02.Add(new JProperty("PrjName", "R-31-2018南港玉成案"));
            //    tmpJoLay02.Add(new JProperty("NowData", "2022-04-23 星期六"));
            //    tmpJoLay02.Add(new JProperty("SDate", "2020-12-28"));
            //    tmpJoLay02.Add(new JProperty("EDate", "2020-12-28"));
            //    tmpJoLay02.Add(new JProperty("AllDate", "1天"));
            //    tmpJoLay02.Add(new JProperty("WeathAM", "晴天"));
            //    tmpJoLay02.Add(new JProperty("WeathPM", "晴天"));
            //    tmpJoLay02.Add(new JProperty("SumData", "85天"));
            //    tmpJoLay02.Add(new JProperty("DataP", "48200%"));
            //    tmpJoLay02.Add(new JProperty("WeathEM", "晴天"));


            //    return new
            //    {
            //        Result = "T",
            //        Message = "成功",
            //        Data = tmpJoLay02
            //    };
            //}
            //catch (Exception ex)
            //{
            //    APCommonFun.Error("[ListProjectDailyReportController]99：" + ex.ToString());
            //    //APCommonFun.Error("[Print_Deliver_pdfController]99：" + sql);

            //    return new
            //    {
            //        Result = "F",
            //        Message = ex.ToString(),
            //        Data = newJa
            //    };
            //}

        }

        /// <summary>
		///  計算日曆天百分比
		/// </summary>
		/// <param name="strSumDate">累計天數</param>
		/// <param name="strAllDate">總工期天數</param>
		/// <returns></returns>
        private string DatePencent(string strSumDate, string strAllDate)
        {
            string strTemp = "";
            if (strSumDate != "" && strAllDate != "")
            {
                if (strSumDate != "0" && strAllDate != "0")
                {
                    try
                    {
                        strTemp = Convert.ToString(Math.Round((Convert.ToDecimal(strSumDate) / Convert.ToDecimal(strAllDate)) * 100, 2)) + "%";
                        return strTemp;
                    }
                    catch
                    {
                        return "0%";
                    }
                }
            }
            return "0%";
        }

        /// <summary>
		/// 計算2個日期間的間隔天數
		/// </summary>
		/// <param name="strDate1">開始日期</param>
		/// <param name="strDate2">結束日期</param>
		/// <returns></returns>
        private string GetSumDate(object strDate1, object strDate2)
        {
            string strTemp = "00000000";
            if (strDate1 == null)
                strDate1 = "";
            if (strDate2 == null)
                strDate2 = "";

            if (strDate1.ToString().Length == 8 && strDate2.ToString().Length == 8)
            {
                if (strDate1.ToString() != "00000000" && strDate2.ToString() != "00000000")
                {
                    try
                    {
                        string date1 = strDate1.ToString().Substring(0, 4) + "-" + strDate1.ToString().Substring(4, 2) + "-" + strDate1.ToString().Substring(6, 2);
                        string date2 = strDate2.ToString().Substring(0, 4) + "-" + strDate2.ToString().Substring(4, 2) + "-" + strDate2.ToString().Substring(6, 2);
                        System.TimeSpan TS = Convert.ToDateTime(date1) - Convert.ToDateTime(date2);
                        strTemp = Convert.ToString(TS.Days + 1);
                        return strTemp;
                    }
                    catch
                    {
                        return "0";
                    }
                }
            }
            return "0";
        }

        private string WhatDay(DateTime dt)
        {
            //string sTemp = DateTime.Parse(strDate).DayOfWeek.ToString();
            string sTemp = dt.DayOfWeek.ToString();
            switch (sTemp.Trim())
            {
                case "Monday":
                    sTemp = " 星期一";
                    break;
                case "Tuesday":
                    sTemp = " 星期二";
                    break;
                case "Wednesday":
                    sTemp = " 星期三";
                    break;
                case "Thursday":
                    sTemp = " 星期四";
                    break;
                case "Friday":
                    sTemp = " 星期五";
                    break;
                case "Saturday":
                    sTemp = " 星期六";
                    break;
                case "Sunday":
                    sTemp = " 星期日";
                    break;
                default:
                    sTemp = "";
                    break;
            }
            return sTemp;
        }

        private string JudgeWeather(object _code)
        {
            if (_code == null)
                return "晴天";
            string _tmpstr = _code.ToString().Trim();
            string strWeather = "";
            switch (_tmpstr.Trim())
            {
                case "1":
                    strWeather = "晴天";
                    break;
                case "2":
                    strWeather = "陰天";
                    break;
                case "3":
                    strWeather = "雨天";
                    break;
                case "4":
                    strWeather = "地震";
                    break;
                case "5":
                    strWeather = "停電";
                    break;
                case "6":
                    strWeather = "颱風";
                    break;
                default:
                    strWeather = "晴天";
                    break;
            }
            return strWeather;
        }
    }
}
