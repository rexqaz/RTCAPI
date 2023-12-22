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
    public class ListProjectFundController : ControllerBase
    {
		private string mStrFuncName = "ListProjectFundController";
		[HttpPost]
        public object FormAll(Info_ListProjectFundModel Data)
        {
			//第一層
			JObject tmpJoLay01 = new JObject();

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

			//第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
			if (InputIsok == "N")
			{
				APCommonFun.Error("[ListProjectFundController]90-" + ReturnErr);
				return new
				{
					Result = "R",
					Message = ReturnErr,
					Data = ""
				};
			}

			string str = " SELECT a.gjahr, ABS (wlp01), ABS (wlp02), ABS (wlp03), ABS (wlp04),";
			str += " ABS (wlp05), ABS (wlp06), ABS (wlp07), ABS (wlp08), ABS (wlp09),";
			str += " ABS (wlp10), ABS (wlp11), ABS (wlp12), ABS (wlp01_real),";
			str += " ABS (wlp02_real), ABS (wlp03_real), ABS (wlp04_real),";
			str += " ABS (wlp05_real), ABS (wlp06_real), ABS (wlp07_real),";
			str += " ABS (wlp08_real), ABS (wlp09_real), ABS (wlp10_real),";
			str += " ABS (wlp11_real), ABS (wlp12_real), B.meg001, B.meg002, B.meg003,";
			str += " B.meg004, B.meg005, B.meg006, B.meg007, B.meg008, B.meg009, B.meg010,";
			str += " B.meg011, B.meg012";
			str += " FROM (SELECT a.gjahr, a.wlp01, a.wlp02, a.wlp03, a.wlp04, a.wlp05, a.wlp06, a.wlp07,";
			str += " a.wlp08, a.wlp09, a.wlp10, a.wlp11, a.wlp12, b.meg001, b.meg002,";
			str += " b.meg003, b.meg004, b.meg005, b.meg006, b.meg007, b.meg008, b.meg009,";
			str += " b.meg010, b.meg011, b.meg012";
			str += " FROM (SELECT a.gjahr, a.pspnr, a.wlp01, a.wlp02, a.wlp03, a.wlp04, a.wlp05,";
			str += " a.wlp06, a.wlp07, a.wlp08, a.wlp09, a.wlp10, a.wlp11, a.wlp12";
			str += " FROM zcpst15 a";
			str = str + " WHERE a.wrttp = '01' AND a.pspnr = '" + PSPNR + "') a,";
			str += " (SELECT b.pspnr, b.gjahr, b.meg001, b.meg002, b.meg003, b.meg004,";
			str += " b.meg005, b.meg006, b.meg007, b.meg008, b.meg009, b.meg010,";
			str += " b.meg011, b.meg012";
			str += " FROM zcpst16 b";
			str = str + " WHERE b.wrttp = 'P1' AND b.pspnr = '" + PSPNR + "') b";
			str += " WHERE a.gjahr = b.gjahr(+) AND a.pspnr = b.pspnr(+)) a,";
			str += " (SELECT a.gjahr, a.wlp01 wlp01_real, a.wlp02 wlp02_real, a.wlp03 wlp03_real,";
			str += " a.wlp04 wlp04_real, a.wlp05 wlp05_real, a.wlp06 wlp06_real, a.wlp07 wlp07_real,";
			str += " a.wlp08 wlp08_real, a.wlp09 wlp09_real, a.wlp10 wlp10_real, a.wlp11 wlp11_real,";
			str += " a.wlp12 wlp12_real, b.meg001, b.meg002,";
			str += " b.meg003, b.meg004, b.meg005, b.meg006, b.meg007, b.meg008, b.meg009,";
			str += " b.meg010, b.meg011, b.meg012";
			str += " FROM (SELECT a.gjahr, a.pspnr, a.wlp01, a.wlp02, a.wlp03, a.wlp04, a.wlp05,";
			str += " a.wlp06, a.wlp07, a.wlp08, a.wlp09, a.wlp10, a.wlp11, a.wlp12";
			str = str + " FROM zcpst15 a WHERE a.wrttp = '04' AND a.pspnr = '" + PSPNR + "') a,";
			str += " (SELECT b.pspnr, b.gjahr, b.meg001, b.meg002, b.meg003, b.meg004,";
			str += " b.meg005, b.meg006, b.meg007, b.meg008, b.meg009, b.meg010,";
			str += " b.meg011, b.meg012 FROM zcpst16 b";
			str = str + " WHERE b.wrttp = 'P2' AND b.pspnr = '" + PSPNR + "') b";
			str += " WHERE a.gjahr = b.gjahr(+) AND a.pspnr = b.pspnr(+)) b";
			str += " WHERE a.gjahr(+) = b.gjahr";
			str += " UNION";
			str += " SELECT a.gjahr, ABS (wlp01), ABS (wlp02), ABS (wlp03), ABS (wlp04),";
			str += " ABS (wlp05), ABS (wlp06), ABS (wlp07), ABS (wlp08), ABS (wlp09),";
			str += " ABS (wlp10), ABS (wlp11), ABS (wlp12), ABS (wlp01_real),";
			str += " ABS (wlp02_real), ABS (wlp03_real), ABS (wlp04_real),";
			str += " ABS (wlp05_real), ABS (wlp06_real), ABS (wlp07_real),";
			str += " ABS (wlp08_real), ABS (wlp09_real), ABS (wlp10_real),";
			str += " ABS (wlp11_real), ABS (wlp12_real), B.meg001, B.meg002, B.meg003,";
			str += " B.meg004, B.meg005, B.meg006, B.meg007, B.meg008, B.meg009, B.meg010,";
			str += " B.meg011, B.meg012";
			str += " FROM (SELECT a.gjahr, a.wlp01, a.wlp02, a.wlp03, a.wlp04, a.wlp05, a.wlp06, a.wlp07,";
			str += " a.wlp08, a.wlp09, a.wlp10, a.wlp11, a.wlp12, b.meg001, b.meg002,";
			str += " b.meg003, b.meg004, b.meg005, b.meg006, b.meg007, b.meg008, b.meg009,";
			str += " b.meg010, b.meg011, b.meg012";
			str += " FROM (SELECT a.gjahr, a.pspnr, a.wlp01, a.wlp02, a.wlp03, a.wlp04, a.wlp05,";
			str += " a.wlp06, a.wlp07, a.wlp08, a.wlp09, a.wlp10, a.wlp11, a.wlp12";
			str = str + " FROM zcpst15 a WHERE a.wrttp = '01' AND a.pspnr = '" + PSPNR + "') a,";
			str += " (SELECT b.pspnr, b.gjahr, b.meg001, b.meg002, b.meg003, b.meg004,";
			str += " b.meg005, b.meg006, b.meg007, b.meg008, b.meg009, b.meg010,";
			str += " b.meg011, b.meg012 FROM zcpst16 b";
			str = str + " WHERE b.wrttp = 'P1' AND b.pspnr = '" + PSPNR + "') b";
			str += " WHERE a.gjahr = b.gjahr(+) AND a.pspnr = b.pspnr(+)) a,";
			str += " (SELECT a.gjahr, a.wlp01 wlp01_real, a.wlp02 wlp02_real, a.wlp03 wlp03_real,";
			str += " a.wlp04 wlp04_real, a.wlp05 wlp05_real, a.wlp06 wlp06_real, a.wlp07 wlp07_real,";
			str += " a.wlp08 wlp08_real, a.wlp09 wlp09_real, a.wlp10 wlp10_real, a.wlp11 wlp11_real,";
			str += " a.wlp12 wlp12_real, b.meg001, b.meg002,";
			str += " b.meg003, b.meg004, b.meg005, b.meg006, b.meg007, b.meg008, b.meg009,";
			str += " b.meg010, b.meg011, b.meg012";
			str += " FROM (SELECT a.gjahr, a.pspnr, a.wlp01, a.wlp02, a.wlp03, a.wlp04, a.wlp05,";
			str += " a.wlp06, a.wlp07, a.wlp08, a.wlp09, a.wlp10, a.wlp11, a.wlp12";
			str = str + " FROM zcpst15 a WHERE a.wrttp = '04' AND a.pspnr = '" + PSPNR + "') a,";
			str += " (SELECT b.pspnr, b.gjahr, b.meg001, b.meg002, b.meg003, b.meg004,";
			str += " b.meg005, b.meg006, b.meg007, b.meg008, b.meg009, b.meg010,b.meg011, b.meg012";
			str = str + " FROM zcpst16 b WHERE b.wrttp = 'P2' AND b.pspnr = '" + PSPNR + "') b";
			str += " WHERE a.gjahr = b.gjahr(+) AND a.pspnr = b.pspnr(+)) b";
			str += " WHERE a.gjahr = b.gjahr(+) ORDER BY gjahr";

			string str2 = "SELECT DISTINCT A.PSPID||A.POST1,A.ACCU_P,TOTAL_P,A.AFDAT,";
			str2 += " A.ACCU_BILL,A.bill02,A.BILL01 ";
			str2 += " from ZCPST11 A,ZCPST15 B ";
			str2 += " WHERE A.pspnr = B.pspnr";
			str2 = str2 + " AND B.pspnr = '" + PSPNR + "'";

			try
			{
				OracleConnection conn = APCommonFun.GetOracleConnection();
				if (conn == null)
				{
					return APCommonFun.ReturnError(mStrFuncName, "Oracle connecting fault.", "R", new JArray());
				}

				string sqlcommand = "";
				sqlcommand += " select decode(substr(pspid,3,2),'11','潤營','12','評輝','13','潤輝','14','潤陽','21','潤安','31','潤弘','21','潤德') || ' - ' || POST1 as POST1,REAL_PPP,ESTRT,EENDE,AFDAT from ZCPST11 where PSPNR='" + PSPNR + "' ";

				OracleCommand cmd = new OracleCommand(sqlcommand, conn);
				cmd.CommandType = CommandType.Text;
				DataSet _ds_proj = new DataSet();
				int _rec_proj = 0;
				double _real_ppp = 0;
				DateTime dtSelected = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
				string _sqlex = "";

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
				string AFDAT = string.Empty;

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

					AFDAT = _ds_proj.Tables[0].Rows[0]["AFDAT"].ToString();
					AFDAT = AFDAT.Substring(0, 4) + "-" + AFDAT.Substring(4, 2) + "-" + AFDAT.Substring(6, 2);

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
				for (int m = 0; m < 12; m++)
				{
					string _union = "union";
					if (m == 11)
						_union = "";
					// P1-->預估進度
					//sample                     select MEG001/100 as MEG ,(GJAHR || '/01/01') as YM from ZCPST16 where PSPNR='00002567' and WRTTP='P1' 
					//sqlcommand += string.Format("select MEG00{0}/100 as MEG ,(GJAHR || '/0{0}/01') as YM from ZCPST16 where PSPNR='{1}' and WRTTP='P1' {2} ", m + 1, project_id, _union);
					sqlcommand += string.Format("select MEG{0:D3}/100 as MEG ,(GJAHR || '/{0:D2}/01') as YM from ZCPST16 where PSPNR='{1}' and WRTTP='P1' {2} ", m + 1, PSPNR, _union);
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
					sqlcommand += string.Format("select MEG{0:D3}/100 as MEG1 ,(GJAHR || '/{0:D2}/01') as YM from ZCPST16 where PSPNR='{1}' and WRTTP='P2' and GJAHR<='{2}' {3} ", m + 1, PSPNR, dtSelected.Year, _union);
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



				DataSet dataSet = new DataSet();
				dataSet = APCommonFun.GetDataSet(str);
				string heightLightDate = GetHeightLightDate(PSPNR);
				DataTable dataTable = new DataTable();
				dataTable = CreatePrjFund(dataSet, heightLightDate);
				Int64 gap = 0;
				string total_real_fund = string.Empty;
				string total_expected_fund = string.Empty;
				DateTime today = DateTime.Now;
				string contractMoney = string.Empty;
				List<string> addedItemArry = new List<string>();

				if (dataTable.Rows.Count > 0)
				{
					JArray newJa02 = new JArray(); //第二層
					bool isFindCurrentPeriod = false;
					double pred_prog = 0.0;
					double real_get_fund = 0.0;
					foreach (DataRow dr in dataTable.Rows)
					{						
						string year = APCommonFun.CDBNulltrim(dr["GJAHR"].ToString());
						string month = APCommonFun.CDBNulltrim(dr["Month"].ToString());
						string addedItem = year + month;
						if (!addedItemArry.Contains(addedItem))
						{
							DataRow[] results;
							results = _ds_actual.Tables[0].Select("YM = '" + year + "/" + month.ToString().PadLeft(2, '0') + "/01'");
							double _pred_prog = 0.0;
							if (results.Length > 0)
							{
								if (results[0]["MEG1"] == null)
									_pred_prog = 0.0;
								else
									double.TryParse(results[0]["MEG1"].ToString().Trim(), out _pred_prog);
							}
							else
							{
								_pred_prog = pred_prog;
							}

							//string progress = APCommonFun.CDBNulltrim(dr["SumFund"].ToString());
							string current_expected_fund = APCommonFun.CDBNulltrim(dr["YFund"].ToString());
							total_expected_fund = APCommonFun.CDBNulltrim(dr["YSumFund"].ToString());
							string current_real_fund = APCommonFun.CDBNulltrim(dr["RealFund"].ToString());
							total_real_fund = APCommonFun.CDBNulltrim(dr["RealSumFund"].ToString());
							gap = (Int64)decimal.Parse(total_real_fund) - (Int64)decimal.Parse(total_expected_fund);
							contractMoney = total_expected_fund;
							real_get_fund = real_get_fund + (Int64)decimal.Parse(current_real_fund) - (Int64)decimal.Parse(current_expected_fund);

							JObject tmpJoLay02 = new JObject();

							tmpJoLay02.Add(new JProperty("year", year));
							tmpJoLay02.Add(new JProperty("month", month));
							if (Convert.ToInt32(year) > today.Year || (Convert.ToInt32(year) == today.Year &&  Convert.ToInt32(month) > today.Month))
							{
								tmpJoLay02.Add(new JProperty("progress", string.Format("{0:N2}%", pred_prog * 100)));
							}
							else
							{
								if (pred_prog == 1)
								{
									tmpJoLay02.Add(new JProperty("progress", string.Format("{0:N2}%", pred_prog * 100)));
								}
								else
								{
									tmpJoLay02.Add(new JProperty("progress", string.Format("{0:N2}%", _pred_prog * 100)));
									pred_prog = _pred_prog;
								}								
							}
							tmpJoLay02.Add(new JProperty("current_expected_fund", current_expected_fund));
							tmpJoLay02.Add(new JProperty("total_expected_fund", total_expected_fund));
							tmpJoLay02.Add(new JProperty("current_real_fund", current_real_fund));
							tmpJoLay02.Add(new JProperty("total_real_fund", total_real_fund));
							newJa02.Add(tmpJoLay02);

							if (isFindCurrentPeriod)
							{
								tmpJoLay01.Add(new JProperty("FundMNext", string.Format("{0:N0}", current_expected_fund)));
								isFindCurrentPeriod = false;
							}

							if (year == today.Year.ToString() && month == today.Month.ToString())
							{
								if (current_expected_fund == "0" && real_get_fund < 0)
								{
									tmpJoLay01.Add(new JProperty("FundM", string.Format("{0:N0}", Math.Abs(real_get_fund))));
								}
								else
								{
									tmpJoLay01.Add(new JProperty("FundM", string.Format("{0:N0}", current_expected_fund)));
								}
								tmpJoLay01.Add(new JProperty("FundMSum", string.Format("{0:N0}", total_expected_fund)));
								tmpJoLay01.Add(new JProperty("CurrentSum", string.Format("{0:N0}", total_real_fund)));
								tmpJoLay01.Add(new JProperty("gap", string.Format("{0:N0}", gap)));
								isFindCurrentPeriod = true;
							}

							addedItemArry.Add(addedItem);
						}
					}
					tmpJoLay01.Add(new JProperty("ContractMoney", string.Format("{0:N0}", contractMoney)));
					tmpJoLay01.Add(new JProperty("fund_progress", newJa02)); //將第二層加入第一層中

					if (!isFindCurrentPeriod)
					{
						if (!tmpJoLay01.ContainsKey("FundM"))
						{
							if (real_get_fund < 0)
							{
								tmpJoLay01.Add(new JProperty("FundM", string.Format("{0:N0}", Math.Abs(real_get_fund))));
							}
							else
							{
								tmpJoLay01.Add(new JProperty("FundM", string.Format("{0:N0}", "0")));
							}
						}
						if (!tmpJoLay01.ContainsKey("FundMSum"))
						{
							tmpJoLay01.Add(new JProperty("FundMSum", string.Format("{0:N0}", total_expected_fund)));
						}
						if (!tmpJoLay01.ContainsKey("FundMNext"))
						{
							tmpJoLay01.Add(new JProperty("FundMNext", string.Format("{0:N0}", "0")));
						}
						if (!tmpJoLay01.ContainsKey("CurrentSum"))
						{
							tmpJoLay01.Add(new JProperty("CurrentSum", string.Format("{0:N0}", total_real_fund)));
						}
						if (!tmpJoLay01.ContainsKey("gap"))
						{
							tmpJoLay01.Add(new JProperty("gap", string.Format("{0:N0}", (Int64)decimal.Parse(total_real_fund) - (Int64)decimal.Parse(total_expected_fund))));
						}
					}
				}
				else
				{
					JArray newJa02 = new JArray(); //第二層
					tmpJoLay01.Add(new JProperty("fund_progress", newJa02)); //將第二層加入第一層中
					tmpJoLay01.Add(new JProperty("project_name", string.Empty));
					tmpJoLay01.Add(new JProperty("FundDate", string.Empty));
					tmpJoLay01.Add(new JProperty("FundM", string.Empty));
					tmpJoLay01.Add(new JProperty("FundMSum", string.Empty));
					tmpJoLay01.Add(new JProperty("FundMNext", string.Empty));
					tmpJoLay01.Add(new JProperty("ContractMoney", string.Empty));
				}				

				DataSet ds = APCommonFun.GetDataSet(str2);
				if (ds.Tables[0].Rows.Count > 0)
				{
					if (!tmpJoLay01.ContainsKey("project_name"))
					{
						tmpJoLay01.Add(new JProperty("project_name", ds.Tables[0].Rows[0][0].ToString().Trim()));
					}

					if (!tmpJoLay01.ContainsKey("FundDate"))
					{
						tmpJoLay01.Add(new JProperty("FundDate", AFDAT));
					}									
				}
				else
				{
					if (!tmpJoLay01.ContainsKey("project_name"))
					{
						tmpJoLay01.Add(new JProperty("project_name", string.Empty));
					}

					if (!tmpJoLay01.ContainsKey("FundDate"))
					{
						tmpJoLay01.Add(new JProperty("FundDate", AFDAT));
					}
				}

				return new
                {
                    Result = "T",
                    Message = "成功",
                    TotalRec = "2",
                    Data = tmpJoLay01
				};
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[ListProjectFundController]99：" + ex.ToString());
				APCommonFun.Error("[ListProjectFundController]99：" + str);
				APCommonFun.Error("[ListProjectFundController]99：" + str2);

				return new
                {
                    Result = "F",
                    Message = ex.ToString(),
                    Data = tmpJoLay01
				};
            }

        }

		private DataSet CalSchedules(DataSet ds_actual, int rec_actual, DataSet ds_ant, ref int rec_ant, DateTime dtNow, double realppp, ref double valueOfTheMonth, ref int lastIndex)
		{
			ds_ant.Tables[0].Columns.Add("MEG1");
			ds_ant.Tables[0].Columns.Add("YM1", typeof(DateTime));

			//實際值累加
			double num = 0;
			for (int i = 0; i < ds_actual.Tables[0].Rows.Count; i++)
			{
				try
				{
					DateTime _row_dt = DateTime.Parse(ds_actual.Tables[0].Rows[i]["YM"].ToString());

					if (_row_dt <= dtNow)
					{
						double _tmpnum = 0;
						try
						{
							_tmpnum = double.Parse(ds_actual.Tables[0].Rows[i]["MEG1"].ToString());
						}
						catch
						{
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
			//for (int i = ds_actual.Tables[0].Rows.Count - 1; i >= 0; i--)
			//{
			//	string tmpMeg1 = ds_actual.Tables[0].Rows[i]["MEG1"] == null ? "" : ds_actual.Tables[0].Rows[i]["MEG1"].ToString().Trim();
			//	string tmpYm = ds_actual.Tables[0].Rows[i]["YM"] == null ? "" : ds_actual.Tables[0].Rows[i]["YM"].ToString().Trim();
			//	if (tmpMeg1.Length <= 0 || tmpYm.Length <= 0)
			//		ds_actual.Tables[0].Rows.RemoveAt(i);
			//}


			//預估值累加
			//在實際值找到對應日期, 填到預估值.MEG1
			DataRow dr_current_month = null;
			int index_insert = -1;
			num = 0;
			for (int i = 0; i < ds_ant.Tables[0].Rows.Count; i++)
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
				for (int act_index = 0; act_index < ds_actual.Tables[0].Rows.Count; act_index++)
				{
					string _act_ym = ds_actual.Tables[0].Rows[act_index]["YM"] == null ? "" : ds_actual.Tables[0].Rows[act_index]["YM"].ToString().Trim();

					if (_ant_ym == _act_ym)
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

					if (string.Format("{0:yyyy/MM}", _dt_curr_ant) == string.Format("{0:yyyy/MM}", dtNow))
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
			if (index_insert >= 0 && dr_current_month != null)
			{
				ds_ant.Tables[0].Rows.InsertAt(dr_current_month, index_insert);
			}
			return ds_ant;
		}

		public string FormatDate(string strDate)
		{
			string strTemp = strDate.Trim();
			if (strTemp != "" && strTemp.Length == 8)
			{
				try
				{
					strTemp = strTemp.Substring(0, 4) + "-" + strTemp.Substring(4, 2) + "-" + strTemp.Substring(6, 2);
				}
				catch
				{
					return strTemp;
				}
			}
			return strTemp;
		}
		public string GetHeightLightDate(string PSPNR)
		{
			string text = "SELECT AFDAT FROM ZCPST11 WHERE PSPNR = '" + PSPNR + "'";
			DataSet dataSet = new DataSet();
			dataSet = APCommonFun.GetDataSet(text);
			return dataSet.Tables[0].Rows[0][0].ToString();
		}


		public DataTable CreatePrjFund(DataSet ds, string hDate)
		{
			_ = DateTime.Today.Month;
			decimal value = 0m;
			decimal value2 = 0m;
			decimal d = 0m;
			bool flag = true;
			int num;
			int num2;
			if (hDate.Trim().Length == 8 && hDate.Trim() != "00000000")
			{
				num = Convert.ToInt32(hDate.Substring(0, 4));
				num2 = Convert.ToInt32(hDate.Substring(4, 2));
			}
			else
			{
				num = DateTime.Now.Year;
				num2 = DateTime.Now.Month;
			}
			DataView defaultView = ds.Tables[0].DefaultView;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("GJAHR", typeof(string));
			dataTable.Columns.Add("Month", typeof(string));
			dataTable.Columns.Add("SumFund", typeof(string));
			dataTable.Columns.Add("YFund", typeof(string));
			dataTable.Columns.Add("YSumFund", typeof(string));
			dataTable.Columns.Add("RealFund", typeof(string));
			dataTable.Columns.Add("RealSumFund", typeof(string));
			for (int i = 0; i < defaultView.Count; i++)
			{
				for (int j = 1; j < 13; j++)
				{					
					if (!(defaultView[i][0].ToString() != ""))
					{
						continue;
					}
					//if (Convert.ToInt32(defaultView[i][0]) == num && j == num2)
					//{
					//	if (j + 3 < 12)
					//	{
					//		for (int k = 0; k < 4; k++)
					//		{
					//			DataRow dataRow = dataTable.NewRow();
					//			dataRow[0] = defaultView[i][0].ToString();
					//			dataRow[1] = j.ToString();
					//			if (flag)
					//			{
					//				if (defaultView[i][j + 24].ToString() != "")
					//				{
					//					d += Convert.ToDecimal(defaultView[i][j + 24]);
					//				}
					//				else
					//				{
					//					d += 0m;
					//				}
					//				if (defaultView[i][j].ToString() != "")
					//				{
					//					value += Convert.ToDecimal(defaultView[i][j]);
					//				}
					//				else
					//				{
					//					value += 0m;
					//				}
					//				if (defaultView[i][j + 12].ToString() != "")
					//				{
					//					value2 += Convert.ToDecimal(defaultView[i][j + 12]);
					//				}
					//				else
					//				{
					//					value2 += 0m;
					//				}
					//				if (defaultView[i][j + 12].ToString() != "")
					//				{
					//					dataRow[5] = $"{Math.Round(Convert.ToDecimal(defaultView[i][j + 12]) * 100m, 0):N0}";
					//				}
					//				else
					//				{
					//					dataRow[5] = "0";
					//				}
					//				flag = false;
					//			}
					//			else
					//			{
					//				if (defaultView[i][j].ToString() != "")
					//				{
					//					value += Convert.ToDecimal(defaultView[i][j]);
					//				}
					//				else
					//				{
					//					value += 0m;
					//				}
					//				//dataRow[5] = "0";
					//				if (defaultView[i][j + 12].ToString() != "")
					//				{
					//					dataRow[5] = $"{Math.Round(Convert.ToDecimal(defaultView[i][j + 12]) * 100m, 0):N0}";
					//				}
					//				else
					//				{
					//					dataRow[5] = "0";
					//				}
					//			}
					//			dataRow[2] = Convert.ToString(Math.Round(d, 2)) + "%";
					//			dataRow[4] = $"{Math.Round(Convert.ToDecimal(value) * 100m, 0):N0}";
					//			dataRow[6] = $"{Math.Round(Convert.ToDecimal(value2) * 100m, 0):N0}";
					//			if (defaultView[i][j].ToString() != "")
					//			{
					//				dataRow[3] = $"{Math.Round(Convert.ToDecimal(defaultView[i][j]) * 100m, 0):N0}";
					//			}
					//			else
					//			{
					//				dataRow[3] = "0";
					//			}
					//			j++;
					//			dataTable.Rows.Add(dataRow);
					//		}
					//		j--;
					//		continue;
					//	}
					//	int num3 = 0;
					//	for (int l = num2; l < 13; l++)
					//	{
					//		DataRow dataRow2 = dataTable.NewRow();
					//		dataRow2[0] = defaultView[i][0].ToString();
					//		dataRow2[1] = Convert.ToString(l);
					//		if (flag)
					//		{
					//			if (defaultView[i][j + 24].ToString() != "")
					//			{
					//				d += Convert.ToDecimal(defaultView[i][j + 24]);
					//			}
					//			else
					//			{
					//				d += 0m;
					//			}
					//			if (defaultView[i][j].ToString() != "")
					//			{
					//				value += Convert.ToDecimal(defaultView[i][j]);
					//			}
					//			else
					//			{
					//				value += 0m;
					//			}
					//			if (defaultView[i][j + 12].ToString() != "")
					//			{
					//				value2 += Convert.ToDecimal(defaultView[i][j + 12]);
					//			}
					//			else
					//			{
					//				value2 += 0m;
					//			}
					//			if (defaultView[i][j + 12].ToString() != "")
					//			{
					//				dataRow2[5] = $"{Math.Round(Convert.ToDecimal(defaultView[i][j + 12]) * 100m, 0):N0}";
					//			}
					//			else
					//			{
					//				dataRow2[5] = "0";
					//			}
					//			flag = false;
					//		}
					//		else
					//		{
					//			if (defaultView[i][j].ToString() != "")
					//			{
					//				value += Convert.ToDecimal(defaultView[i][j]);
					//			}
					//			else
					//			{
					//				value += 0m;
					//			}
					//			dataRow2[5] = "0";
					//		}
					//		dataRow2[2] = Convert.ToString(Math.Round(d, 2)) + "%";
					//		dataRow2[4] = $"{Math.Round(Convert.ToDecimal(value) * 100m, 0):N0}";
					//		dataRow2[6] = $"{Math.Round(Convert.ToDecimal(value2) * 100m, 0):N0}";
					//		if (defaultView[i][j].ToString() != "")
					//		{
					//			dataRow2[3] = $"{Math.Round(Convert.ToDecimal(defaultView[i][j]) * 100m, 0):N0}";
					//		}
					//		else
					//		{
					//			dataRow2[3] = "0";
					//		}
					//		dataTable.Rows.Add(dataRow2);
					//		num3++;
					//		j++;
					//	}
					//	if (num3 == 3 || i >= defaultView.Count - 1 || !(defaultView[i + 1][0].ToString() == Convert.ToString(num + 1)))
					//	{
					//		continue;
					//	}
					//	for (int m = 0; m < 4 - num3; m++)
					//	{
					//		DataRow dataRow3 = dataTable.NewRow();
					//		dataRow3[0] = defaultView[i + 1][0].ToString();
					//		dataRow3[1] = Convert.ToString(m + 1);
					//		if (defaultView[i + 1][m + 1].ToString() != "")
					//		{
					//			value += Convert.ToDecimal(defaultView[i + 1][m + 1]);
					//		}
					//		else
					//		{
					//			value += 0m;
					//		}
					//		dataRow3[2] = Convert.ToString(Math.Round(d, 2)) + "%";
					//		dataRow3[4] = $"{Math.Round(Convert.ToDecimal(value) * 100m, 0):N0}";
					//		dataRow3[5] = "0";
					//		dataRow3[6] = $"{Math.Round(Convert.ToDecimal(value2) * 100m, 0):N0}";
					//		if (defaultView[i + 1][m + 1].ToString() != "")
					//		{
					//			dataRow3[3] = $"{Math.Round(Convert.ToDecimal(defaultView[i + 1][m + 1]) * 100m, 0):N0}";
					//		}
					//		else
					//		{
					//			dataRow3[3] = "0";
					//		}
					//		dataTable.Rows.Add(dataRow3);
					//	}
					//}
					//else
					{
						// 註解這一段，讓API可回傳未來的資料
						//if (Convert.ToInt32(defaultView[i][0]) > num || (Convert.ToInt32(defaultView[i][0]) == num && j >= num2))
						//{
						//	break;
						//}
						DataRow dataRow4 = dataTable.NewRow();
						dataRow4[0] = defaultView[i][0].ToString();
						dataRow4[1] = j.ToString();
						if (defaultView[i][j + 24].ToString() != "")
						{
							d += Convert.ToDecimal(defaultView[i][j + 24]);
						}
						else
						{
							d += 0m;
						}
						if (defaultView[i][j].ToString() != "")
						{
							value += Convert.ToDecimal(defaultView[i][j]);
						}
						else
						{
							value += 0m;
						}
						if (defaultView[i][j + 12].ToString() != "")
						{
							value2 += Convert.ToDecimal(defaultView[i][j + 12]);
						}
						else
						{
							value2 += 0m;
						}
						dataRow4[2] = Convert.ToString(Math.Round(d, 2)) + "%";
						dataRow4[4] = $"{Math.Round(Convert.ToDecimal(value) * 100m, 0):N0}";
						dataRow4[6] = $"{Math.Round(Convert.ToDecimal(value2) * 100m, 0):N0}";
						if (defaultView[i][j].ToString() != "")
						{
							dataRow4[3] = $"{Math.Round(Convert.ToDecimal(defaultView[i][j]) * 100m, 0):N0}";
						}
						else
						{
							dataRow4[3] = "0";
						}
						if (defaultView[i][j + 12].ToString() != "")
						{
							dataRow4[5] = $"{Math.Round(Convert.ToDecimal(defaultView[i][j + 12]) * 100m, 0):N0}";
						}
						else
						{
							dataRow4[5] = "0";
						}
						dataTable.Rows.Add(dataRow4);
					}
				}
			}
			return dataTable;
		}
	}
}
