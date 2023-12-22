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
    public class ListCostPlusController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListCostPlusModel Data)
        {
            JArray newJa = new JArray();
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

            string sql1 = "SELECT A.PSPID, A.POST1, A.ESTRT, A.EENDE, A.VERNA, A.ASTNA "
                            + "FROM ZCPST11 A WHERE A.PSPNR = '" + PSPNR + "'";

            string sql2 = "select KSTAR,KTEXT,ACCU_WKG,MCTXT,KSTAR_REAL,KTEXT_REAL,ACCU_WKG_REAL,MCTXT_REAL from";
            sql2 = sql2 + " (SELECT KSTAR,KTEXT,ACCU_WKG,MCTXT FROM zcpst17 where WRTTP='01' and pspnr = '" + PSPNR + "') CostPlan , ";
            sql2 += " (SELECT KSTAR KSTAR_REAL,KTEXT KTEXT_REAL,ACCU_WKG ACCU_WKG_REAL,MCTXT MCTXT_REAL";
            sql2 = sql2 + " FROM zcpst17 where WRTTP='04' and pspnr = '" + PSPNR + "') CostReal";
            sql2 += " WHERE CostPlan.KSTAR(+) = CostReal.KSTAR_REAL";
            sql2 += " and CostPlan.KTEXT(+) = CostReal.KTEXT_REAL";
            sql2 += " and CostPlan.MCTXT(+) = CostReal.MCTXT_REAL ";
            sql2 += " union";
            sql2 += " select KSTAR,KTEXT,ACCU_WKG,MCTXT,KSTAR_REAL,KTEXT_REAL,ACCU_WKG_REAL,MCTXT_REAL from ";
            sql2 = sql2 + " (SELECT KSTAR,KTEXT,ACCU_WKG,MCTXT FROM zcpst17 where WRTTP='01' and pspnr = '" + PSPNR + "') CostPlan , ";
            sql2 += " (SELECT KSTAR KSTAR_REAL,KTEXT KTEXT_REAL,ACCU_WKG ACCU_WKG_REAL,MCTXT MCTXT_REAL";
            sql2 = sql2 + " FROM zcpst17 where WRTTP='04' and pspnr = '" + PSPNR + "') CostReal";
            sql2 += " WHERE CostPlan.KSTAR = CostReal.KSTAR_REAL(+)";
            sql2 += " and CostPlan.KTEXT = CostReal.KTEXT_REAL(+)";
            sql2 += " and CostPlan.MCTXT = CostReal.MCTXT_REAL(+)";

            string sql3 = "SELECT VALUE FROM  ManagerPercent WHERE PSPNR= '" + PSPNR + "' ";

            try
            {
                string percent = "0";
                DataSet ds3 = APCommonFun.GetDataSet(sql3);
                if (ds3.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds3.Tables[0].Rows)
                    {
                        percent = APCommonFun.CDBNulltrim(dr["VALUE"].ToString());
                    }
                }

                DataSet ds = APCommonFun.GetDataSet(sql1);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        string project_name = APCommonFun.CDBNulltrim(dr["POST1"].ToString());
                        string principal = APCommonFun.CDBNulltrim(dr["VERNA"].ToString());
                        string supervise = APCommonFun.CDBNulltrim(dr["ASTNA"].ToString());
                        string date_period_start = APCommonFun.CDBNulltrim(dr["ESTRT"].ToString());
                        string date_period_end = APCommonFun.CDBNulltrim(dr["EENDE"].ToString());
                        string date_period = "從 " + date_period_start.Substring(0, 4) + " 年 " + date_period_start.Substring(4, 2) + " 月 到"
                                            + date_period_end.Substring(0, 4) + " 年 " + date_period_end.Substring(4, 2) + " 月 ";
                        string date = DateTime.Now.Year.ToString() + " 年 " + DateTime.Now.Month.ToString() + " 月 ";

                        tmpJoLay01.Add(new JProperty("project_name", project_name));
                        tmpJoLay01.Add(new JProperty("principal", principal));
                        tmpJoLay01.Add(new JProperty("supervise", supervise));
                        tmpJoLay01.Add(new JProperty("date", date));
                        tmpJoLay01.Add(new JProperty("date_period", date_period));
                    }
                }
                else
                {
                    tmpJoLay01.Add(new JProperty("project_name", string.Empty));
                    tmpJoLay01.Add(new JProperty("principal", string.Empty));
                    tmpJoLay01.Add(new JProperty("supervise", string.Empty));
                    tmpJoLay01.Add(new JProperty("date", string.Empty));
                    tmpJoLay01.Add(new JProperty("date_period", string.Empty));
                }

                DataSet ds2 = APCommonFun.GetDataSet(sql2);

                DataView defaultView = ds2.Tables[0].DefaultView;
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("", typeof(string));
                dataTable.Columns.Add("KSTAR", typeof(string));
                dataTable.Columns.Add("KTEXT", typeof(string));
                dataTable.Columns.Add("Real_ACCU_WKG", typeof(string));
                dataTable.Columns.Add("Plan_ACCU_WKG", typeof(string));
                dataTable.Columns.Add("Dif_ACCU_WKG", typeof(string));
                dataTable.Columns.Add("ACCU_WKG%", typeof(string));
                DataTable dt = APCommonFun.CreateCostPlusSum(dataTable, defaultView, "存貨S");
                int dtCount = dt.Rows.Count;
                DataTable dt2 = APCommonFun.CreateCostPlusSum(dt, defaultView, "成本-材料CM");
                int dt2Count = dt2.Rows.Count - dtCount;
                DataTable dt3 = APCommonFun.CreateCostPlusSum(dt2, defaultView, "成本-工料CA");
                int dt3Count = dt3.Rows.Count - dtCount - dt2Count;
                DataTable dt4 = APCommonFun.CreateCostPlusSum(dt3, defaultView, "成本-人工CL");
                int dt4Count = dt4.Rows.Count - dtCount - dt2Count - dt3Count;
                DataTable dt5 = APCommonFun.CreateCostPlusSum(dt4, defaultView, "成本-費用CF");
                int dt5Count = dt5.Rows.Count - dtCount - dt2Count - dt3Count - dt4Count;
                DataTable costPlusSumAll = APCommonFun.GetCostPlusSumAll(dt5, "工程成本C＝CM+CA+CL+CF 合計", PSPNR);
                DataTable costPlusSumAll2 = APCommonFun.GetCostPlusSumAll(costPlusSumAll, "總工程成本CT=C+S",  PSPNR);
                DataTable costPlusSumAll3 = APCommonFun.GetCostPlusSumAll(costPlusSumAll2, "成本加成管利6％", PSPNR);
                DataTable costPlusSumAll4 = APCommonFun.GetCostPlusSumAll(costPlusSumAll3, "總計", PSPNR);

                
                JArray newJa02 = new JArray();
                string S_A = "0";
                string S_P = "0";
                string S_V = "0";
                string S_VP = "0";
                if (dtCount > 0)
                {
                    JArray newJa03 = new JArray();
                    JObject tmpJoLay02 = new JObject();
                    for (int i = 0; i < dtCount; i++)
                    {
                        string var1 = APCommonFun.CDBNulltrim(dt5.Rows[i][0].ToString());
                        string var2 = APCommonFun.CDBNulltrim(dt5.Rows[i][1].ToString());
                        string var3 = APCommonFun.CDBNulltrim(dt5.Rows[i][2].ToString());
                        string var4 = APCommonFun.CDBNulltrim(dt5.Rows[i][3].ToString());
                        string var5 = APCommonFun.CDBNulltrim(dt5.Rows[i][4].ToString());
                        string var6 = APCommonFun.CDBNulltrim(dt5.Rows[i][5].ToString());
                        string var7 = APCommonFun.CDBNulltrim(dt5.Rows[i][6].ToString());

                        if (i == 0)
                        {
                            
                            tmpJoLay02.Add(new JProperty("account sub_total", var2));
                            tmpJoLay02.Add(new JProperty("real_sub_total", var4));
                            tmpJoLay02.Add(new JProperty("budget_sub_total", var5));
                            tmpJoLay02.Add(new JProperty("gap_sub_total", var6));
                            tmpJoLay02.Add(new JProperty("gap_percent_sub_total", var7));
                            S_A = var4;
                            S_P = var5;
                            S_V = var6;
                            S_VP = var7;
                        }
                        else
                        {
                            JObject tmpJoLay03 = new JObject();
                            tmpJoLay03.Add(new JProperty("account id", var2));
                            tmpJoLay03.Add(new JProperty("account_item", var3));
                            tmpJoLay03.Add(new JProperty("real", var4));
                            tmpJoLay03.Add(new JProperty("budget", var5));
                            tmpJoLay03.Add(new JProperty("gap", var6));
                            tmpJoLay03.Add(new JProperty("gap_percent", var7));
                            newJa03.Add(tmpJoLay03);
                        }
                    }
                    tmpJoLay02.Add("contents", newJa03);
                    newJa02.Add(tmpJoLay02);
                }

                string CM_A = "0";
                string CM_P = "0";
                string CM_V = "0";
                string CM_VP = "0";

                if (dt2Count > 0)
                {
                    JArray newJa03 = new JArray();
                    JObject tmpJoLay02 = new JObject();
                    for (int i = dtCount; i < dtCount + dt2Count; i++)
                    {
                        string var1 = APCommonFun.CDBNulltrim(dt5.Rows[i][0].ToString());
                        string var2 = APCommonFun.CDBNulltrim(dt5.Rows[i][1].ToString());
                        string var3 = APCommonFun.CDBNulltrim(dt5.Rows[i][2].ToString());
                        string var4 = APCommonFun.CDBNulltrim(dt5.Rows[i][3].ToString());
                        string var5 = APCommonFun.CDBNulltrim(dt5.Rows[i][4].ToString());
                        string var6 = APCommonFun.CDBNulltrim(dt5.Rows[i][5].ToString());
                        string var7 = APCommonFun.CDBNulltrim(dt5.Rows[i][6].ToString());

                        if (i == dtCount)
                        {

                            tmpJoLay02.Add(new JProperty("account sub_total", var2));
                            tmpJoLay02.Add(new JProperty("real_sub_total", var4));
                            tmpJoLay02.Add(new JProperty("budget_sub_total", var5));
                            tmpJoLay02.Add(new JProperty("gap_sub_total", var6));
                            tmpJoLay02.Add(new JProperty("gap_percent_sub_total", var7));
                            CM_A = var4;
                            CM_P = var5;
                            CM_V = var6;
                            CM_VP = var7;
                        }
                        else
                        {
                            JObject tmpJoLay03 = new JObject();
                            tmpJoLay03.Add(new JProperty("account id", var2));
                            tmpJoLay03.Add(new JProperty("account_item", var3));
                            tmpJoLay03.Add(new JProperty("real", var4));
                            tmpJoLay03.Add(new JProperty("budget", var5));
                            tmpJoLay03.Add(new JProperty("gap", var6));
                            tmpJoLay03.Add(new JProperty("gap_percent", var7));
                            newJa03.Add(tmpJoLay03);
                        }
                    }
                    tmpJoLay02.Add("contents", newJa03);
                    newJa02.Add(tmpJoLay02);
                }

                string CA_A = "0";
                string CA_P = "0";
                string CA_V = "0";
                string CA_VP = "0";

                if (dt3Count > 0)
                {
                    JArray newJa03 = new JArray();
                    JObject tmpJoLay02 = new JObject();
                    for (int i = dtCount + dt2Count; i < dtCount + dt2Count + dt3Count; i++)
                    {
                        string var1 = APCommonFun.CDBNulltrim(dt5.Rows[i][0].ToString());
                        string var2 = APCommonFun.CDBNulltrim(dt5.Rows[i][1].ToString());
                        string var3 = APCommonFun.CDBNulltrim(dt5.Rows[i][2].ToString());
                        string var4 = APCommonFun.CDBNulltrim(dt5.Rows[i][3].ToString());
                        string var5 = APCommonFun.CDBNulltrim(dt5.Rows[i][4].ToString());
                        string var6 = APCommonFun.CDBNulltrim(dt5.Rows[i][5].ToString());
                        string var7 = APCommonFun.CDBNulltrim(dt5.Rows[i][6].ToString());

                        if (i == dtCount + dt2Count)
                        {

                            tmpJoLay02.Add(new JProperty("account sub_total", var2));
                            tmpJoLay02.Add(new JProperty("real_sub_total", var4));
                            tmpJoLay02.Add(new JProperty("budget_sub_total", var5));
                            tmpJoLay02.Add(new JProperty("gap_sub_total", var6));
                            tmpJoLay02.Add(new JProperty("gap_percent_sub_total", var7));
                            CA_A = var4;
                            CA_P = var5;
                            CA_V = var6;
                            CA_VP = var7;

                        }
                        else
                        {
                            JObject tmpJoLay03 = new JObject();
                            tmpJoLay03.Add(new JProperty("account id", var2));
                            tmpJoLay03.Add(new JProperty("account_item", var3));
                            tmpJoLay03.Add(new JProperty("real", var4));
                            tmpJoLay03.Add(new JProperty("budget", var5));
                            tmpJoLay03.Add(new JProperty("gap", var6));
                            tmpJoLay03.Add(new JProperty("gap_percent", var7));
                            newJa03.Add(tmpJoLay03);
                        }
                    }
                    tmpJoLay02.Add("contents", newJa03);
                    newJa02.Add(tmpJoLay02);
                }

                string CL_A = "0";
                string CL_P = "0";
                string CL_V = "0";
                string CL_VP = "0";
                if (dt4Count > 0)
                {
                    JArray newJa03 = new JArray();
                    JObject tmpJoLay02 = new JObject();
                    for (int i = dtCount + dt2Count + dt3Count; i < dtCount + dt2Count + dt3Count + dt4Count; i++)
                    {
                        string var1 = APCommonFun.CDBNulltrim(dt5.Rows[i][0].ToString());
                        string var2 = APCommonFun.CDBNulltrim(dt5.Rows[i][1].ToString());
                        string var3 = APCommonFun.CDBNulltrim(dt5.Rows[i][2].ToString());
                        string var4 = APCommonFun.CDBNulltrim(dt5.Rows[i][3].ToString());
                        string var5 = APCommonFun.CDBNulltrim(dt5.Rows[i][4].ToString());
                        string var6 = APCommonFun.CDBNulltrim(dt5.Rows[i][5].ToString());
                        string var7 = APCommonFun.CDBNulltrim(dt5.Rows[i][6].ToString());

                        if (i == dtCount + dt2Count + dt3Count)
                        {

                            tmpJoLay02.Add(new JProperty("account sub_total", var2));
                            tmpJoLay02.Add(new JProperty("real_sub_total", var4));
                            tmpJoLay02.Add(new JProperty("budget_sub_total", var5));
                            tmpJoLay02.Add(new JProperty("gap_sub_total", var6));
                            tmpJoLay02.Add(new JProperty("gap_percent_sub_total", var7));
                            CL_A = var4;
                            CL_P = var5;
                            CL_V = var6;
                            CL_VP = var7;
                        }
                        else
                        {
                            JObject tmpJoLay03 = new JObject();
                            tmpJoLay03.Add(new JProperty("account id", var2));
                            tmpJoLay03.Add(new JProperty("account_item", var3));
                            tmpJoLay03.Add(new JProperty("real", var4));
                            tmpJoLay03.Add(new JProperty("budget", var5));
                            tmpJoLay03.Add(new JProperty("gap", var6));
                            tmpJoLay03.Add(new JProperty("gap_percent", var7));
                            newJa03.Add(tmpJoLay03);
                        }
                    }
                    tmpJoLay02.Add("contents", newJa03);
                    newJa02.Add(tmpJoLay02);
                }

                string CF_A = "0";
                string CF_P = "0";
                string CF_V = "0";
                string CF_VP = "0";
                if (dt5Count > 0)
                {
                    JArray newJa03 = new JArray();
                    JObject tmpJoLay02 = new JObject();
                    for (int i = dtCount + dt2Count + dt3Count + dt4Count; i < dtCount + dt2Count + dt3Count + dt4Count + dt5Count; i++)
                    {
                        string var1 = APCommonFun.CDBNulltrim(dt5.Rows[i][0].ToString());
                        string var2 = APCommonFun.CDBNulltrim(dt5.Rows[i][1].ToString());
                        string var3 = APCommonFun.CDBNulltrim(dt5.Rows[i][2].ToString());
                        string var4 = APCommonFun.CDBNulltrim(dt5.Rows[i][3].ToString());
                        string var5 = APCommonFun.CDBNulltrim(dt5.Rows[i][4].ToString());
                        string var6 = APCommonFun.CDBNulltrim(dt5.Rows[i][5].ToString());
                        string var7 = APCommonFun.CDBNulltrim(dt5.Rows[i][6].ToString());

                        if (i == dtCount + dt2Count + dt3Count + dt4Count)
                        {

                            tmpJoLay02.Add(new JProperty("account sub_total", var2));
                            tmpJoLay02.Add(new JProperty("real_sub_total", var4));
                            tmpJoLay02.Add(new JProperty("budget_sub_total", var5));
                            tmpJoLay02.Add(new JProperty("gap_sub_total", var6));
                            tmpJoLay02.Add(new JProperty("gap_percent_sub_total", var7));
                            CF_A = var4;
                            CF_P = var5;
                            CF_V = var6;
                            CF_VP = var7;
                        }
                        else
                        {
                            JObject tmpJoLay03 = new JObject();
                            tmpJoLay03.Add(new JProperty("account id", var2));
                            tmpJoLay03.Add(new JProperty("account_item", var3));
                            tmpJoLay03.Add(new JProperty("real", var4));
                            tmpJoLay03.Add(new JProperty("budget", var5));
                            tmpJoLay03.Add(new JProperty("gap", var6));
                            tmpJoLay03.Add(new JProperty("gap_percent", var7));
                            newJa03.Add(tmpJoLay03);
                        }
                    }
                    tmpJoLay02.Add("contents", newJa03);
                    newJa02.Add(tmpJoLay02);
                }

                Int64 C_A = 0;
                Int64 C_P = 0;
                Int64 C_V = 0;
                Int64 C_VP = 0;
                if (costPlusSumAll.Rows.Count > 0)
                {
                    Int64 var1 = (Int64)decimal.Parse(CM_A) + (Int64)decimal.Parse(CA_A) + (Int64)decimal.Parse(CL_A) + (Int64)decimal.Parse(CF_A);
                    Int64 var2 = (Int64)decimal.Parse(CM_P) + (Int64)decimal.Parse(CA_P) + (Int64)decimal.Parse(CL_P) + (Int64)decimal.Parse(CF_P);
                    Int64 var3 = (Int64)decimal.Parse(CM_V) + (Int64)decimal.Parse(CA_V) + (Int64)decimal.Parse(CL_V) + (Int64)decimal.Parse(CF_V);
                    double var4 = ((double)var3 / (double)var2) * 100;
                    C_A = var1;
                    C_P = var2;
                    C_V = var3;
                    tmpJoLay01.Add(new JProperty("engineer_cost_real", $"{var1:N0}"));
                    tmpJoLay01.Add(new JProperty("engineer_cost_budget", $"{var2:N0}"));
                    tmpJoLay01.Add(new JProperty("engineer_cost_gap", $"{var3:N0}"));
                    if ($"{var2:N0}" == "0" || $"{var3:N0}" == "0")
                    {
                        tmpJoLay01.Add(new JProperty("engineer_cost_gap_percent", "0%"));
                    }
                    else
                    {
                        tmpJoLay01.Add(new JProperty("engineer_cost_gap_percent", string.Format("{0:0.##}%", var4)));
                    }
                }
                else
                {
                    tmpJoLay01.Add(new JProperty("engineer_cost_real", string.Empty));
                    tmpJoLay01.Add(new JProperty("engineer_cost_budget", string.Empty));
                    tmpJoLay01.Add(new JProperty("engineer_cost_gap", string.Empty));
                    tmpJoLay01.Add(new JProperty("engineer_cost_gap_percent", string.Empty));
                }

                Int64 CT_A = 0;
                Int64 CT_P = 0;
                Int64 CT_V = 0;
                Int64 CT_VP = 0;
                if (costPlusSumAll2.Rows.Count > 0)
                {
                    Int64 var1 = C_A + (Int64)decimal.Parse(S_A);
                    Int64 var2 = C_P + (Int64)decimal.Parse(S_P);
                    Int64 var3 = C_V + (Int64)decimal.Parse(S_V);
                    double var4 = ((double)var3 / (double)var2) * 100 ;
                    CT_A = var1;
                    CT_P = var2;
                    CT_V = var3;
                    tmpJoLay01.Add(new JProperty("all_engineer_cost_real", $"{var1:N0}"));
                    tmpJoLay01.Add(new JProperty("all_engineer_cost_budget", $"{var2:N0}"));
                    tmpJoLay01.Add(new JProperty("all_engineer_cost_gap", $"{var3:N0}"));
                    if ($"{var2:N0}" == "0" || $"{var3:N0}" == "0")
                    {
                        tmpJoLay01.Add(new JProperty("all_engineer_cost_gap_percent", "0%"));
                    }
                    else
                    {
                        tmpJoLay01.Add(new JProperty("all_engineer_cost_gap_percent", string.Format("{0:0.##}%", var4)));
                    }
                }
                else
                {
                    tmpJoLay01.Add(new JProperty("all_engineer_cost_real", string.Empty));
                    tmpJoLay01.Add(new JProperty("all_engineer_cost_budget", string.Empty));
                    tmpJoLay01.Add(new JProperty("all_engineer_cost_gap", string.Empty));
                    tmpJoLay01.Add(new JProperty("all_engineer_cost_gap_percent", string.Empty));
                }


                double costPlust6p_A = 0;
                double costPlust6p_P = 0;
                double costPlust6p_V = 0;
                double costPlust6p_VP = 0;
                if (costPlusSumAll3.Rows.Count > 0)
                {
                    double var1 = (CT_A * Convert.ToDouble(percent));
                    double var2 = (CT_P * Convert.ToDouble(percent));
                    double var3 = (CT_V * Convert.ToDouble(percent));
                    double var4 = ((double)var3 / (double)var2) * 100;

                    tmpJoLay01.Add(new JProperty("cost_plus_real", $"{var1:N0}"));
                    tmpJoLay01.Add(new JProperty("cost_plus_budget", $"{var2:N0}"));
                    tmpJoLay01.Add(new JProperty("cost_plus_gap", $"{var3:N0}"));
                    if ($"{var2:N0}" == "0" || $"{var3:N0}" == "0")
                    {
                        tmpJoLay01.Add(new JProperty("cost_plus_gap_percent", "0%"));
                    }
                    else
                    {
                        tmpJoLay01.Add(new JProperty("cost_plus_gap_percent", string.Format("{0:0.##}%", var4)));
                    }

                    costPlust6p_A = var1;
                    costPlust6p_P = var2;
                    costPlust6p_V = var3;
                    costPlust6p_VP = var4;
                }
                else
                {
                    tmpJoLay01.Add(new JProperty("cost_plus_real", string.Empty));
                    tmpJoLay01.Add(new JProperty("cost_plus_budget", string.Empty));
                    tmpJoLay01.Add(new JProperty("cost_plus_gap", string.Empty));
                    tmpJoLay01.Add(new JProperty("cost_plus_gap_percent", string.Empty));
                }

                if (costPlusSumAll4.Rows.Count > 0)
                {
                    Int64 var1 = CT_A + (Int64)costPlust6p_A;
                    Int64 var2 = CT_P + (Int64)costPlust6p_P;
                    Int64 var3 = CT_V + (Int64)costPlust6p_V;
                    double var4 = ((double)var3 / (double)var2) * 100;
                    tmpJoLay01.Add(new JProperty("total_real", $"{var1:N0}"));
                    tmpJoLay01.Add(new JProperty("total_budget", $"{var2:N0}"));
                    tmpJoLay01.Add(new JProperty("total_gap", $"{var3:N0}"));
                    if ($"{var2:N0}" == "0" || $"{var3:N0}" == "0")
                    {
                        tmpJoLay01.Add(new JProperty("total_gap_percent", "0%"));
                    }
                    else
                    {
                        tmpJoLay01.Add(new JProperty("total_gap_percent", string.Format("{0:0.##}%", var4)));
                    }
                }
                else
                {
                    tmpJoLay01.Add(new JProperty("total_real", string.Empty));
                    tmpJoLay01.Add(new JProperty("total_budget", string.Empty));
                    tmpJoLay01.Add(new JProperty("total_gap", string.Empty));
                    tmpJoLay01.Add(new JProperty("total_percent", string.Empty));
                }

                tmpJoLay01.Add(new JProperty("account", newJa02));

                return new
                {
                    Result = "T",
                    Message = "成功",
                    Data = tmpJoLay01
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[ListCostPlusController]99：" + ex.ToString());
                APCommonFun.Error("[ListCostPlusController]99：" + sql1);
                APCommonFun.Error("[ListCostPlusController]99：" + sql2);

                return new
                {
                    Result = "F",
                    Message = ex.ToString(),
                    Data = tmpJoLay01
                };
            }

        }

        
    }
}
