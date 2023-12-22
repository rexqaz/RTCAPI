/*
 * Class Name   : ListProjectOverviewController
 * Description  : 專案瀏覽列表
 * Author       : Polo
 * Create Date  : 2022/11/10
 * Version      :
 * 
 *
 */
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

using Oracle.ManagedDataAccess.Client;

namespace rc_interface_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListProjectOverviewController : ControllerBase
    {
        private string mStrFuncName = "ListProjectOverviewController";
        [HttpPost]
        public object FormAll(Info_ProjectOverviewModel Data)
        {


            //Char delimiter = ',';

            //page 從 1 開始
            //string page = "";
            //if (Data.page != null) { page = APCommonFun.CDBNulltrim(Data.page); }
            string project_name = "";
            if (Data.project_name != null) { project_name = APCommonFun.CDBNulltrim(Data.project_name); }
            string account_id = "";
            if (Data.account_id != null) { account_id = APCommonFun.CDBNulltrim(Data.account_id); }
            string orderby = "";
            if (Data.orderby != null) { orderby = APCommonFun.CDBNulltrim(Data.orderby); }

            if (account_id.Length <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No account_id is specified.", "R", new JArray());
            }

            if(orderby.Length <= 0)
            {
                orderby = "project_name asc";
            }

            orderby = orderby.ToLower();

            orderby = orderby.Replace("project_name", "post1");
            orderby = orderby.Replace("name_member", "A.PROJ_PROG");
            orderby = " order by " + orderby;


            //string[] tmppage = Data.page == null ? new string[] { "1", "-1" } : Data.page.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

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

            //sqlcommand = "select distinct A.KUNNR,A.PSPNR,decode(substr(A.pspid,3,2),'11','潤營','12','評輝','13','潤輝','14','潤陽','21','潤安','31','潤弘','21','潤德') || ' - ' || A.post1 as post1,";
            string sqlcommand = "select distinct A.KUNNR,A.PSPNR,decode(substr(A.pspid,3,2),'11','潤營','12','評輝','13','潤輝','14','潤陽','21','潤安','31','潤弘','21','潤德') || ' - ' || A.post1 as post1,";
            sqlcommand += "decode(substr(A.pspid,3,2),'11','潤營','12','評輝','13','潤輝','14','潤陽','21','潤安','31','潤弘','21','潤德') as ownername,";
            sqlcommand += " A.PROJ_PROG from ZCPST11 A,PORJECTCCTVURL B where B.PSPNR(+)=A.PSPNR AND ";
            sqlcommand += "A.PSPNR in (select PSPNR from AccountProjectMappings where upper(AccID)='" + account_id.ToUpper() + "') ";
            if (project_name.Length > 0)
            {
                sqlcommand += " AND UPPER(POST1) LIKE '%" + project_name.ToUpper() + "%'";
            }
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
                    _item.Add(new JProperty("name_member", dsdata.Tables[0].Rows[index]["PROJ_PROG"].ToString()));
                    _item.Add(new JProperty("owner_name", dsdata.Tables[0].Rows[index]["ownername"].ToString()));

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


        }
    }
}
