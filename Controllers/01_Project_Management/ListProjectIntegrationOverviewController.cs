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
    public class ListProjectIntegrationOverviewController : ControllerBase
    {
        private string mStrFuncName = "ListProjectIntegrationOverviewController";
        [HttpPost]
        public object FormAll(Info_ListProjectIntegrationOverviewModel Data)
        {
            JArray newJa = new JArray();
            JArray newJa_Final = new JArray();

            string project_name = "";
            if (Data.project_name != null) { project_name = APCommonFun.CDBNulltrim(Data.project_name); }
            string project_id = "";
            if (Data.project_id != null) { project_id = APCommonFun.CDBNulltrim(Data.project_id); }
            string orderby = "";
            if (Data.orderby != null) { orderby = APCommonFun.CDBNulltrim(Data.orderby); }
            string merge_description = "";
            if (Data.merge_description != null) { merge_description = APCommonFun.CDBNulltrim(Data.merge_description); }

            if (orderby.Length <= 0)
            {
                orderby = "merge_description asc";
            }

            orderby = orderby.ToLower();

            orderby = orderby.Replace("merge_description", "main.pidescription");
            orderby = orderby.Replace("merge_id", "main.pikey");
            orderby = " order by " + orderby;

            //傳入頁次格式: 1,20  -->  第 1 頁, 每頁 20 筆
            //若未傳入, 或傳入格式不正確, 則以 1,-1 為預設值 (取回全部資料)

            string[] tmppage = new string[] { "1", "-1" };
            try
            {
                tmppage = Data.page.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch { }
            if (tmppage.Length != 2)
            {
                //格式不正確,給定預設值
                tmppage = new string[] { "1", "-1" };
            }

            int _sel_page = int.Parse(tmppage[0]); //取回指定頁次資料, _items_per_page = -1代表全部,則頁次無效
            int _items_per_page = int.Parse(tmppage[1]);    // -1 代表取回全部資料

            OracleConnection conn = APCommonFun.GetOracleConnection();
            if (conn == null)
            {
                return APCommonFun.ReturnError(mStrFuncName, "Oracle connecting fault.", "R", new JArray());
            }

            string sqlcommand = "select main.pikey,main.pidescription,count(detail.subpspnr) as DetailCount ";
            sqlcommand += "from projectintegrationkey main ,projectintegrationDetails detail ";
            sqlcommand += "where main.pikey = detail.pikey(+) ";
            sqlcommand += string.Format("and main.pidescription like '%{0}%' ", merge_description);
            sqlcommand += "and main.pikey in ( select a.pikey from projectintegrationDetails a,ZCPST11 b,kna1 c ";
            sqlcommand += string.Format("where  a.subpspnr=b.PSPNR  and b.KUNNR=c.KUNNR and a.subpspnr like '%{0}%' and b.post1 like '%{1}%') ", project_id, project_name);
            sqlcommand += "group by main.pikey,main.pidescription ";
            
            sqlcommand += " " + orderby;


            //get total count
            OracleCommand cmd = new OracleCommand("select count(*) as total_counter from (" + sqlcommand + ") ", conn);
            cmd.CommandType = CommandType.Text;
            DataSet dscount = new DataSet();
            int _total_rec = 0;

            try
            {
                OracleDataAdapter _da = new OracleDataAdapter();
                _da.SelectCommand = cmd;
                _da.Fill(dscount);
                if (dscount.Tables.Count > 0)
                {
                    _total_rec = int.Parse(dscount.Tables[0].Rows[0]["total_counter"].ToString());
                }
            }
            catch (Exception ex)
            {
                try { conn.Close(); } catch { }
                return APCommonFun.ReturnError(mStrFuncName, "Common exception", ex.ToString(), "F", new JArray());
            }

            if (_total_rec <= 0)
            {
                try { conn.Close(); } catch { }
                return APCommonFun.ReturnSuccess(new JArray(), 0);
            }

            if (_items_per_page > 0 && _sel_page > 0)
            {
                //有指定頁次及筆數
                int _offset_rows = (_sel_page - 1) * _items_per_page;
                if (_offset_rows >= _total_rec)
                {
                    return APCommonFun.ReturnError(mStrFuncName, "Parameters fault.Selected page is out of range.", "F", new JArray());
                }
                sqlcommand += string.Format(" offset {0} rows fetch next {1} rows only ", _offset_rows, _items_per_page);
            }

            cmd = new OracleCommand(sqlcommand, conn);
            cmd.CommandType = CommandType.Text;
            DataSet dsdata = new DataSet();

            JArray newJa02 = new JArray(); //第二層

            try
            {
                OracleDataAdapter _da = new OracleDataAdapter();
                _da.SelectCommand = cmd;
                int _recgot = _da.Fill(dsdata);
                JObject _item;
                for (int index = 0; index < _recgot; index++)
                {
                    _item = new JObject();

                    _item.Add(new JProperty("merge_id", dsdata.Tables[0].Rows[index]["pikey"].ToString()));
                    _item.Add(new JProperty("merge_description", dsdata.Tables[0].Rows[index]["pidescription"].ToString()));
                    _item.Add(new JProperty("merge_count", dsdata.Tables[0].Rows[index]["DetailCount"].ToString()));

                    newJa02.Add(_item);
                }
                try { conn.Close(); } catch { }
                return APCommonFun.ReturnSuccess(newJa02, _total_rec);
                //return new
                //{
                //    Result = "T",
                //    Message = "成功",
                //    TotalRec = _total_rec.ToString(),
                //    Data = newJa02
                //};
            }
            catch (Exception ex)
            {
                try { conn.Close(); } catch { }
                return APCommonFun.ReturnError(mStrFuncName, "Common exception.", ex.ToString(), "F", new JArray());
                //return new
                //{
                //    Result = "F",
                //    Message = "[Common exception]:",
                //    Data = newJa
                //};
            }

            //try
            //{

            //    //第一層
            //    JObject tmpJoLay01 = new JObject();
            //    tmpJoLay01.Add(new JProperty("merge_id", "88"));
            //    tmpJoLay01.Add(new JProperty("merge_description", "央北ALL"));
            //    tmpJoLay01.Add(new JProperty("merge_count", "2"));

            //    newJa.Add(tmpJoLay01);


            //    JObject tmpJoLay02 = new JObject();
            //    tmpJoLay02.Add(new JProperty("merge_id", "99"));
            //    tmpJoLay02.Add(new JProperty("merge_description", "央北ALL2"));
            //    tmpJoLay02.Add(new JProperty("merge_count", "3"));

            //    newJa.Add(tmpJoLay02);

            //    return new
            //    {
            //        Result = "T",
            //        Message = "成功",
            //        TotalRec = "2",
            //        Data = newJa
            //    };
            //}
            //catch (Exception ex)
            //{
            //    APCommonFun.Error("[ListProjectIntegrationOverviewController]99：" + ex.ToString());
            //    //APCommonFun.Error("[ListProjectIntegrationOverviewController]99：" + sql);

            //    return new
            //    {
            //        Result = "F",
            //        Message = ex.ToString(),
            //        Data = newJa
            //    };
            //}

        }
    }
}
