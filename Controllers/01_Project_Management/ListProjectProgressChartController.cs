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
    public class ListProjectProgressChartController : ControllerBase
    {
        private string mStrFuncName = "ListProjectProgressChartController";

        [HttpPost]
        public object FormAll(ListProjectProgressChartModel Data)
        {
            string project_id = "";
            JObject tmpJoLay01 = new JObject();

            if (Data.project_id != null) { project_id = APCommonFun.CDBNulltrim(Data.project_id); }

            if (project_id.Length <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No project_id is specified.", "R", new JArray());
            }

            OracleConnection conn = APCommonFun.GetOracleConnection();
            if (conn == null)
            {
                return APCommonFun.ReturnError(mStrFuncName, "Oracle connecting fault.", "R", new JArray());
            }

            string sqlcommand = "";
            sqlcommand += " select decode(substr(pspid,3,2),'11','潤營','12','評輝','13','潤輝','14','潤陽','21','潤安','31','潤弘','21','潤德') || ' - ' || POST1 as POST1,REAL_PPP,ESTRT,EENDE from ZCPST11 where PSPNR='" + project_id + "' ";

            OracleCommand cmd = new OracleCommand(sqlcommand, conn);
            cmd.CommandType = CommandType.Text;
            DataSet _ds_proj = new DataSet();
            int _rec_proj = 0;
            double _real_ppp = 0;
            DateTime dtSelected = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            string _sqlex = "";
            JArray newJa = new JArray();
            JArray newJa_Final = new JArray();

            try
            {
                using (OracleDataAdapter _da = new OracleDataAdapter())
                {
                    _da.SelectCommand = cmd;
                    _rec_proj = _da.Fill(_ds_proj);
                }

                if (_rec_proj <= 0)
                {
                    try { conn.Close(); } catch { }
                    return APCommonFun.ReturnSuccess(new JArray(), 0);
                }

                //取得至今為止的預估進度及實際進度
                DataSet _ds_anticipated = new DataSet();
                int _rec_anticipated = 0;
                DataSet _ds_actual = new DataSet();
                int _rec_actual = 0;
                double minusday = 0;

                try
                {
                    //DateTime _dt_minus_1 = DateTime.Parse(_ds_proj.Tables[0].Rows[0]["ESTRT"].ToString());
                    //DateTime _dt_minus_2 = DateTime.Parse(_ds_proj.Tables[0].Rows[0]["EENDE"].ToString());
                    string _tmpdt = _ds_proj.Tables[0].Rows[0]["ESTRT"].ToString();
                    _tmpdt = _tmpdt.Substring(0, 4) + "/" + _tmpdt.Substring(4, 2) + "/" + _tmpdt.Substring(6, 2);
                    DateTime _dt_minus_1 = DateTime.Parse(_tmpdt);

                    _tmpdt = _ds_proj.Tables[0].Rows[0]["EENDE"].ToString();
                    _tmpdt = _tmpdt.Substring(0, 4) + "/" + _tmpdt.Substring(4, 2) + "/" + _tmpdt.Substring(6, 2);
                    DateTime _dt_minus_2 = DateTime.Parse(_tmpdt);

                    TimeSpan _dt_diff = _dt_minus_1 - _dt_minus_2;
                    minusday = Math.Abs(_dt_diff.TotalDays);
                    minusday = minusday / 100;//---> 
                }
                catch (Exception ex) 
                {
                    _sqlex += "ESTRT, EENDE ex:" + ex.ToString();
                }

                try
                {
                    _real_ppp = double.Parse(_ds_proj.Tables[0].Rows[0]["REAL_PPP"].ToString());    //預計
                }
                catch { }

                //預估進度====================================
                //組合查詢字串
                sqlcommand = "(";
                for (int m = 0;m < 12;m++)
                {
                    string _union = "union";
                    if (m == 11)
                        _union = "";
                    // P1-->預估進度
                    //sample                     select MEG001/100 as MEG ,(GJAHR || '/01/01') as YM from ZCPST16 where PSPNR='00002567' and WRTTP='P1' 
                    //sqlcommand += string.Format("select MEG00{0}/100 as MEG ,(GJAHR || '/0{0}/01') as YM from ZCPST16 where PSPNR='{1}' and WRTTP='P1' {2} ", m + 1, project_id, _union);
                    sqlcommand += string.Format("select MEG{0:D3} as MEG ,(GJAHR || '/{0:D2}/01') as YM from ZCPST16 where PSPNR='{1}' and WRTTP='P1' {2} ", m + 1, project_id, _union);
                }
                sqlcommand += ") order by YM ";

                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;
                using (OracleDataAdapter _da = new OracleDataAdapter())
                {
                    _da.SelectCommand = cmd;
                    _rec_anticipated = _da.Fill(_ds_anticipated);
                }

                //實際進度====================================
                //組合查詢字串
                sqlcommand = "(";
                for (int m = 0; m < 12; m++)
                {
                    string _union = "union";
                    if (m == 11)
                        _union = "";

                    //   P2-->實際進度
                    //sample                     select MEG001/100 as MEG1 ,(GJAHR || '/01/01') as YM from ZCPST16 where PSPNR='00002567' and WRTTP='P2' and GJAHR <= '" + Conversions.ToString(DateTime.Now.Year) + "'
                    //sqlcommand += string.Format("select MEG00{0}/100 as MEG1 ,(GJAHR || '/0{0}/01') as YM from ZCPST16 where PSPNR='{1}' and WRTTP='P2' and GJAHR<='{2}' {3} ", m + 1, project_id, dtSelected.Year, _union);
                    sqlcommand += string.Format("select MEG{0:D3} as MEG1 ,(GJAHR || '/{0:D2}/01') as YM from ZCPST16 where PSPNR='{1}' and WRTTP='P2' and GJAHR<='{2}' {3} ", m + 1, project_id, dtSelected.Year, _union);
                }
                sqlcommand += ") order by YM ";

                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;
                using (OracleDataAdapter _da = new OracleDataAdapter())
                {
                    _da.SelectCommand = cmd;
                    _rec_actual = _da.Fill(_ds_actual);
                }
                double valueOfTheMonth = 0.0;
                int lastIndex = 0;
                //

                DataSet ds_actual = _ds_actual.Copy();
                DataSet ds_anticipated = _ds_anticipated.Copy();
                _ds_anticipated = CalSchedules(_ds_actual, _rec_actual, _ds_anticipated, ref _rec_anticipated, dtSelected, _real_ppp, ref valueOfTheMonth, ref lastIndex);

                JArray newJa02 = new JArray(); //第二層
                bool isFindCurrentDate = false;
                string current_prog = "0";
                string predict_prog = string.Empty;
                string gap = string.Empty;
                double previous_curr_prog = 0;

                for (int ant_index = 0; ant_index < _rec_anticipated; ant_index++)
                {
                    try
                    {
                        JObject _item = new JObject();
                        //_item.Add(new JProperty("year", "2021"));           //年份
                        //_item.Add(new JProperty("month", "1"));             //月份
                        //_item.Add(new JProperty("progress", "0.28%"));      //實際值
                        //_item.Add(new JProperty("pred_progress", "0.28%")); //預估值
                        DateTime _tmpdt = DateTime.Parse(_ds_anticipated.Tables[0].Rows[ant_index]["YM"].ToString());

                        //double _curr_prog = double.Parse(_ds_anticipated.Tables[0].Rows[ant_index]["MEG1"].ToString().Trim());
                        double _curr_prog = 0;
                        bool _has_current = false;                        
                        try
                        {

                            _curr_prog = double.Parse(_ds_actual.Tables[0].Rows[ant_index]["MEG1"].ToString().Trim());
                            _has_current = true;
                        }
                        catch { }
                        double _pred_prog = double.Parse(_ds_anticipated.Tables[0].Rows[ant_index]["MEG"].ToString().Trim());
                        _item.Add(new JProperty("year", string.Format("{0:yyyy}", _tmpdt)));           //年份
                        _item.Add(new JProperty("month", string.Format("{0:MM}", _tmpdt)));             //月份

                        if (_has_current)
                        {
                            _item.Add(new JProperty("progress", string.Format("{0:N5}%", _curr_prog)));      //實際值
                            previous_curr_prog = _curr_prog;
                        }
                        else
                            _item.Add(new JProperty("progress", string.Format("{0:N5}%", previous_curr_prog)));      

                        _item.Add(new JProperty("pred_progress", string.Format("{0:N5}%", _pred_prog))); //預估值


                        if (string.Format("{0:yyyy}", _tmpdt) == DateTime.Now.Year.ToString() && string.Format("{0:MM}", _tmpdt) == DateTime.Now.Month.ToString().PadLeft(2, '0'))
                        {
                            tmpJoLay01.Add(new JProperty("expected_progress", string.Format("{0:N5}%", _real_ppp)));
                            tmpJoLay01.Add(new JProperty("current_progress", string.Format("{0:N5}%", previous_curr_prog)));
                            double _prog_diff = previous_curr_prog - _real_ppp;
                            if (_prog_diff > 0)
                            {
                                string prog_diff = string.Format("{0:N2}%", _prog_diff);
                                string prog_diff_minusday = string.Format("({0:N2} Day)", _prog_diff * minusday);
                                if (prog_diff == "-0.00%")
                                {
                                    prog_diff = "0.00%";
                                    prog_diff_minusday = "(0.00 Day)";
                                }
                                if (prog_diff_minusday == "(-0.00 Day)")
                                {
                                    prog_diff = "0.00%";
                                    prog_diff_minusday = "(0.00 Day)";
                                }
                                //落後
                                tmpJoLay01.Add(new JProperty("gap", prog_diff + " " + prog_diff_minusday));
                            }
                            else
                            {
                                //超前
                                string prog_diff = string.Format("{0:N2}%", _prog_diff);
                                string prog_diff_minusday = string.Format("({0:N2} Day)", (_prog_diff * -1) * minusday);
                                if (prog_diff == "-0.00%")
                                {
                                    prog_diff = "0.00%";
                                    prog_diff_minusday = "(0.00 Day)";
                                }
                                if (prog_diff_minusday == "(-0.00 Day)")
                                {
                                    prog_diff = "0.00%";
                                    prog_diff_minusday = "(0.00 Day)";
                                }
                                tmpJoLay01.Add(new JProperty("gap", prog_diff + " " + prog_diff_minusday));
                            }
                            isFindCurrentDate = true;
                        }
                        else
                        {
                            current_prog = string.Format("{0:N5}%", previous_curr_prog);
                            predict_prog = string.Format("{0:N5}%", _pred_prog);
                            double _prog_diff = previous_curr_prog - _real_ppp;
                            if (_prog_diff > 0)
                            {
                                string prog_diff = string.Format("{0:N2}%", _prog_diff);
                                if (prog_diff == "-0.00%") prog_diff = "0.00%";
                                string prog_diff_minusday = string.Format("({0:N2} Day)", _prog_diff * minusday);
                                //落後

                                if (prog_diff == "-0.00%")
                                {
                                    prog_diff = "0.00%";
                                    prog_diff_minusday = "(0.00 Day)";
                                }
                                if (prog_diff_minusday == "(-0.00 Day)")
                                {
                                    prog_diff = "0.00%";
                                    prog_diff_minusday = "(0.00 Day)";
                                }
                                gap = prog_diff + " " + prog_diff_minusday;
                            }
                            else
                            {
                                //超前
                                string prog_diff = string.Format("{0:N2}%", _prog_diff);
                                string prog_diff_minusday = string.Format("({0:N2} Day)", (_prog_diff * -1) * minusday);
                                if (prog_diff == "-0.00%") prog_diff = "0.00%";

                                if (prog_diff == "-0.00%")
                                {
                                    prog_diff = "0.00%";
                                    prog_diff_minusday = "(0.00 Day)";
                                }
                                if (prog_diff_minusday == "(-0.00 Day)")
                                {
                                    prog_diff = "0.00%";
                                    prog_diff_minusday = "(0.00 Day)";
                                }
                                gap = prog_diff + " " + prog_diff_minusday;
                            }
                        }

                        double curr_prog = 0;
                        bool has_current = false;
                        try
                        {
                            curr_prog = double.Parse(ds_actual.Tables[0].Rows[ant_index]["MEG1"].ToString().Trim());
                            has_current = true;
                        }
                        catch { }
                        if (has_current)
                            _item.Add(new JProperty("progress_the_month", string.Format("{0:N5}%", curr_prog)));      //該月實際值
                        else
                            _item.Add(new JProperty("progress_the_month", ""));

                        double ant_prog = 0;
                        bool has_ant = false;
                        try
                        {
                            ant_prog = double.Parse(ds_anticipated.Tables[0].Rows[ant_index]["MEG"].ToString().Trim());
                            has_ant = true;
                        }
                        catch { }
                        if (has_ant)
                            _item.Add(new JProperty("pred_progress_the_month", string.Format("{0:N5}%", ant_prog)));      //該月預估值
                        else
                            _item.Add(new JProperty("pred_progress_the_month", ""));

                        newJa02.Add(_item);

                    }
                    catch { }
                }
                
                tmpJoLay01.Add(new JProperty("project_name", _ds_proj.Tables[0].Rows[0]["POST1"] == null ? "" : _ds_proj.Tables[0].Rows[0]["POST1"].ToString()));
                tmpJoLay01.Add(new JProperty("FundDate", DateTime.Now.ToString("yyyy-MM-dd")));
                if (!isFindCurrentDate)
                {
                    tmpJoLay01.Add(new JProperty("expected_progress", string.Format("{0:N5}%", _real_ppp)));
                    tmpJoLay01.Add(new JProperty("current_progress", current_prog));
                    tmpJoLay01.Add(new JProperty("gap", gap ));
                }
                
                tmpJoLay01.Add(new JProperty("project_progress", newJa02));

                newJa.Add(tmpJoLay01);

                try { conn.Close(); } catch { }
                return APCommonFun.ReturnSuccess(newJa, 1);
            }
            catch (Exception ex)
            {
                try { conn.Close(); } catch { }
                return APCommonFun.ReturnError(mStrFuncName, "Common exception.", ex.ToString(), "F", new JArray());
            }
        }

        /// <summary>
        /// 1.各列累計加總數值 (MEG, MEG1) 
        /// 2.取得最後累計值
        /// </summary>
        /// <param name="ds_actual"></param>
        /// <param name="rec_actual"></param>
        /// <param name="ds_ant"></param>
        /// <param name="rec_ant"></param>
        /// <returns></returns>
        private DataSet CalSchedules(DataSet ds_actual, int rec_actual, DataSet ds_ant, ref int rec_ant, DateTime dtNow, double realppp, ref double valueOfTheMonth, ref int lastIndex)
        {
            ds_ant.Tables[0].Columns.Add("MEG1");
            ds_ant.Tables[0].Columns.Add("YM1", typeof(DateTime));

            //實際值累加
            double num = 0;
            for (int i = 0;i < ds_actual.Tables[0].Rows.Count;i++)
            {
                try
                {
                    DateTime _row_dt = DateTime.Parse(ds_actual.Tables[0].Rows[i]["YM"].ToString());

                    if(_row_dt <= dtNow)
                    {
                        double _tmpnum = 0;
                        try
                        {
                            _tmpnum = double.Parse(ds_actual.Tables[0].Rows[i]["MEG1"].ToString());
                        }
                        catch {
                            _tmpnum = 0;
                            ds_actual.Tables[0].Rows[i]["MEG1"] = _tmpnum;
                        }

                        //if(i > 0)
                        _tmpnum = _tmpnum + num;//加上一筆的值
                        ds_actual.Tables[0].Rows[i]["MEG1"] = _tmpnum;

                        valueOfTheMonth = _tmpnum;//保存最後的累計值

                        num = _tmpnum;
                    }
                }
                catch { }
                
            }
            //實際值無數值移除
            for (int i = ds_actual.Tables[0].Rows.Count - 1;i >= 0;i--)
            {
                string tmpMeg1 = ds_actual.Tables[0].Rows[i]["MEG1"] == null ? "" : ds_actual.Tables[0].Rows[i]["MEG1"].ToString().Trim();
                string tmpYm = ds_actual.Tables[0].Rows[i]["YM"] == null ? "" : ds_actual.Tables[0].Rows[i]["YM"].ToString().Trim();
                if (tmpMeg1.Length <= 0 || tmpYm.Length <= 0)
                    ds_actual.Tables[0].Rows.RemoveAt(i);
            }


            //預估值累加
            //在實際值找到對應日期, 填到預估值.MEG1
            DataRow dr_current_month = null;
            int index_insert = -1;
            num = 0;
            for(int i = 0;i < ds_ant.Tables[0].Rows.Count;i++)
            {
                double _tmpnum = 0;
                try
                {
                    _tmpnum = double.Parse(ds_ant.Tables[0].Rows[i]["MEG"].ToString());
                }
                catch
                {
                    _tmpnum = 0;
                    ds_ant.Tables[0].Rows[i]["MEG"] = _tmpnum;
                }

                //if(i > 0)
                _tmpnum = _tmpnum + num;//加上一筆的值
                ds_ant.Tables[0].Rows[i]["MEG"] = _tmpnum;

                num = _tmpnum;
                string _ant_ym = ds_ant.Tables[0].Rows[i]["YM"] == null ? "" : ds_ant.Tables[0].Rows[i]["YM"].ToString().Trim();
                for (int act_index = 0;act_index < ds_actual.Tables[0].Rows.Count;act_index++)
                {
                    string _act_ym = ds_actual.Tables[0].Rows[act_index]["YM"] == null ? "" : ds_actual.Tables[0].Rows[act_index]["YM"].ToString().Trim();

                    if(_ant_ym == _act_ym)
                    {
                        //對應日期的實際值
                        ds_ant.Tables[0].Rows[i]["MEG1"] = ds_actual.Tables[0].Rows[act_index]["MEG1"].ToString().Trim();
                        break;
                    }
                }

                try
                {
                    ds_ant.Tables[0].Rows[i]["YM1"] = DateTime.Parse(ds_ant.Tables[0].Rows[i]["YM"].ToString().Trim()).AddMonths(1);
                }
                catch { }

                try
                {
                    DateTime _dt_curr_ant = DateTime.Parse(_ant_ym);

                    if(string.Format("{0:yyyy/MM}", _dt_curr_ant) == string.Format("{0:yyyy/MM}", dtNow))
                    {
                        //如果是落在當前月份-->理論上應為最後一筆
                        index_insert = i;
                        dr_current_month = ds_ant.Tables[0].NewRow();

                        dr_current_month["YM1"] = dtNow.ToString("yyyy/MM/dd");
                        dr_current_month["MEG"] = realppp * 0.01;
                        dr_current_month["MEG1"] = ds_ant.Tables[0].Rows[i]["MEG1"] == null ? "0" : ds_ant.Tables[0].Rows[i]["MEG1"].ToString().Trim();
                        ds_ant.Tables[0].Rows[i]["MEG1"] = "";

                    }
                }
                catch { }
            }
            //if(index_insert >= 0 && dr_current_month != null)
            //{
            //    ds_ant.Tables[0].Rows.InsertAt(dr_current_month, index_insert);
            //}
            return ds_ant;
        }
    }
}
