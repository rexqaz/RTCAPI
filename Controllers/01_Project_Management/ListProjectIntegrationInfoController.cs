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
    public class ListProjectIntegrationInfoController : ControllerBase
    {
        private string mStrFuncName = "ListProjectIntegrationInfoController";
        [HttpPost]
        public object FormAll(Info_ProjectIntegrationInfoModel Data)
        {
            JArray newJa = new JArray();
            JArray newJa_Final = new JArray();

            //Char delimiter = ',';

            string merge_id = "";
            if (Data.merge_id != null) { merge_id = APCommonFun.CDBNulltrim(Data.merge_id); }

            string orderby = "";
            if (Data.orderby != null) { orderby = APCommonFun.CDBNulltrim(Data.orderby); }

            if (merge_id.Length <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No merge_id is specified.", "R", new JArray());
            }

            if (orderby.Length <= 0)
            {
                orderby = "project_name asc";
            }

            orderby = orderby.ToLower();

            orderby = orderby.Replace("project_name", "ProName");
            orderby = orderby.Replace("owner_name", "Name");
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

            //string sqlcommand = "select ProjectIntegrationKey.PIDescription from ProjectIntegrationKey where ProjectIntegrationKey.PIKey=:PIKey";
            string sqlcommand = "select ProjectIntegrationKey.PIDescription from ProjectIntegrationKey where ProjectIntegrationKey.PIKey=" + merge_id;

            //Get main project
            OracleCommand cmd = new OracleCommand(sqlcommand, conn);

            string _description = "";
            try
            {
                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;
                object tmpret = cmd.ExecuteScalar();
                _description = tmpret.ToString();
            }
            catch (Exception ex)
            {
                //return APCommonFun.ReturnError(mStrFuncName, "Getting the next id has faults.", "F", new JArray());
                APCommonFun.Error(string.Format("[{0}]:Common fault!, ex:{1}", mStrFuncName, ex.ToString()));
            }

            sqlcommand = "select  a.SUBPSPNR PSPNR ,decode(substr(b.pspid,3,2),'11','潤營','12','評輝','13','潤輝','14','潤陽','21','潤安','31','潤弘','21','潤德') || ' - ' || b.post1 as ProName,"; 
            sqlcommand += "c.NAME1 Name from ProjectIntegrationDetails a,ZCPST11 b,KNA1 c where a.SUBPSPNR=b.PSPNR and b.KUNNR=c.KUNNR and a.PIKey=" + merge_id;

            sqlcommand += " " + orderby;

            //get total count
            cmd = new OracleCommand("select count(*) as total_counter from (" + sqlcommand + ") ", conn);
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
                    try { conn.Close(); } catch { }
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
                    _item.Add(new JProperty("project_name", dsdata.Tables[0].Rows[index]["ProName"].ToString()));
                    _item.Add(new JProperty("owner_name", dsdata.Tables[0].Rows[index]["Name"].ToString()));

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


            ////第一步 : 先判斷有沒有必填未填寫，
            //string InputIsok = "Y";
            //string ReturnErr = "";

            //if (merge_id == "") //必填                       
            //{
            //    InputIsok = "N";
            //    ReturnErr = "執行動作錯誤-merge_id 為必填欄位";
            //}
            ////第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            //if (InputIsok == "N")
            //{
            //    APCommonFun.Error("[ListProjectIntegrationInfoController]90-" + ReturnErr);
            //    return new
            //    {
            //        Result = "R",
            //        Message = ReturnErr,
            //        Data = ""
            //    };
            //}


            //try
            //{
            //    //第一層
            //    JObject tmpJoLay01 = new JObject();
            //    tmpJoLay01.Add(new JProperty("project_id", "00002478"));
            //    tmpJoLay01.Add(new JProperty("project_name", "潤陽 - 潤泰央北案 假設工程"));
            //    tmpJoLay01.Add(new JProperty("owner_name", "潤泰創新國際股份有限公司"));

            //    newJa.Add(tmpJoLay01);


            //    JObject tmpJoLay02 = new JObject();

            //    tmpJoLay02.Add(new JProperty("project_id", "00002479"));
            //    tmpJoLay02.Add(new JProperty("project_name", "潤陽 - 潤泰央北案 假設工程B"));
            //    tmpJoLay02.Add(new JProperty("owner_name", "潤泰創新國際股份有限公司B"));

            //    newJa.Add(tmpJoLay02);

            //    return new
            //    {
            //        Result = "T",
            //        Message = "成功",
            //        Data = newJa
            //    };
            //}
            //catch (Exception ex)
            //{
            //    APCommonFun.Error("[ListProjectIntegrationInfoController]99：" + ex.ToString());
            //    //APCommonFun.Error("[ListProjectIntegrationInfoController]99：" + sql);

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
