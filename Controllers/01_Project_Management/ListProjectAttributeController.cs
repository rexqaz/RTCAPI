using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rc_interface_API.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace rc_interface_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListProjectAttributeController : ControllerBase
    {
        private string mStrFuncName = "ListProjectAttributeController";
        [HttpPost]
        public object FormAll(Info_ProjectAttributeOverViewModel Data)
        {
            string orderby = "";
            if (Data.orderby != null) { orderby = APCommonFun.CDBNulltrim(Data.orderby); }


            if (orderby.Length <= 0)
            {
                orderby = "project_name asc";
            }

            orderby = orderby.ToLower();

            orderby = orderby.Replace("project_name", "post1");
            orderby = orderby.Replace("owner_name", "B.Name1");
            orderby = " order by " + orderby;

            //傳入頁次格式: 1,20  -->  第 1 頁, 每頁 20 筆
            //若未傳入, 或傳入格式不正確, 則以 1,-1 為預設值 (取回全部資料)

            //string[] tmppage = Data.page == null ? new string[] { "1", "-1" } : Data.page.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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

            string sqlcommand = "select a.pspnr,decode(substr(a.pspid,3,2),'11','潤營','12','評輝','13','潤輝','14','潤陽','21','潤安','31','潤弘','21','潤德') || ' - ' || a.post1 as post1,";
            sqlcommand += " b.Name1,a.IsDemocase from zcpst11 a,kna1 b where a.kunnr = b.kunnr(+)";
            
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

                    _item.Add(new JProperty("project_id", dsdata.Tables[0].Rows[index]["PSPNR"].ToString()));
                    _item.Add(new JProperty("project_name", dsdata.Tables[0].Rows[index]["POST1"].ToString()));
                    _item.Add(new JProperty("owner_name", dsdata.Tables[0].Rows[index]["Name1"].ToString()));
                    _item.Add(new JProperty("is_demo", dsdata.Tables[0].Rows[index]["IsDemocase"].ToString()));

                    newJa02.Add(_item);
                }

                try { conn.Close(); } catch { }
                return APCommonFun.ReturnSuccess(newJa02, _total_rec);
            }
            catch (Exception ex)
            {
                try { conn.Close(); } catch { }
                return APCommonFun.ReturnError(mStrFuncName, "Common exception.", ex.ToString(), "F", new JArray());
            }
           


            //JArray newJa = new JArray();
            //JArray newJa_Final = new JArray();


            //try
            //{
            //    //第一層
            //    JObject tmpJoLay01 = new JObject();
            //    tmpJoLay01.Add(new JProperty("project_id", "1111"));
            //    tmpJoLay01.Add(new JProperty("project_name", "潤弘 - 台積電F18P7預鑄工程"));
            //    tmpJoLay01.Add(new JProperty("owner_name", "互助營造股份有限公司"));
            //    tmpJoLay01.Add(new JProperty("is_demo", "0"));


            //    newJa.Add(tmpJoLay01);

            //    JObject tmpJoLay02 = new JObject();
            //    tmpJoLay02.Add(new JProperty("project_id", "2222"));
            //    tmpJoLay02.Add(new JProperty("project_name", "潤弘 - 台積電AP6A預鑄工程"));
            //    tmpJoLay02.Add(new JProperty("owner_name", "互助營造股份有限公司"));
            //    tmpJoLay02.Add(new JProperty("is_demo", "0"));


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
            //    APCommonFun.Error("[ListProjectAttributeController]99：" + ex.ToString());
            //    //APCommonFun.Error("[Print_Deliver_pdfController]99：" + sql);

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
