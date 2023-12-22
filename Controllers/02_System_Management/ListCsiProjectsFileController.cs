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

namespace rc_interface_API.Controllers._02_System_Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListCsiProjectsFileController : ControllerBase
    {
        private string mStrFuncName = "ListCsiProjectsFileController";
        [HttpPost]
        public object FormAll(Info_CsiProjectsFileQueryModel Data)
        {
            string project_name = "";
            if (Data.project_name != null) { project_name = APCommonFun.CDBNulltrim(Data.project_name); }

            string project_file_id = "";
            if (Data.project_file_id != null) { project_file_id = APCommonFun.CDBNulltrim(Data.project_file_id); }

            string _link = "";
            if (Data.link != null) { _link = APCommonFun.CDBNulltrim(Data.link); }

            string orderby = "";
            if (Data.orderby != null) { orderby = APCommonFun.CDBNulltrim(Data.orderby); }

            string user_id = "";
            if (Data.user_id != null) { user_id = APCommonFun.CDBNulltrim(Data.user_id); }

            if (orderby.Length <= 0)
            {
                orderby = "project_name asc";
            }

            orderby = orderby.ToLower();

            orderby = orderby.Replace("project_name", "FUNCNAME");
            orderby = orderby.Replace("link", "WEBURL");
            orderby = " order by " + orderby;


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

            string _condition = "";
            string _condconnector = "";
            if(project_name.Length > 0)
            {
                _condition += " FUNCNAME Like '%" + project_name + "%' ";
                if (_condconnector.Length <= 0)
                    _condconnector = " And ";
            }
            if (_link.Length > 0)
            {
                _condition = _condition + _condconnector + " WEBURL Like '%" + _link + "%' ";
                if (_condconnector.Length <= 0)
                    _condconnector = " And ";
            }

            if (project_file_id.Length > 0)
            {
                _condition = _condition + _condconnector + " FUNCID = '" + project_file_id + "' ";
                if (_condconnector.Length <= 0)
                    _condconnector = " And ";
            }

            //第一步 : 先判斷有沒有必填未填寫，
            string InputIsok = "Y";
            string ReturnErr = "";

            if (user_id == "") //必填                       
            {
                InputIsok = "N";
                ReturnErr = "執行動作錯誤-user_id 為必填欄位";
            }
            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[ListCsiProjectsFileController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sql = " select * from ACCOUNTS  "
                         + " where ACCID='" + user_id + "' ";

            DataTable dt = APCommonFun.GetDataTable(sql);
            string GPRID = string.Empty;
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    GPRID = APCommonFun.CDBNulltrim(dr["GRPID"].ToString());                    
                }
            }


            string sqlcommand = "";
            OracleCommand cmd = null;

            sqlcommand = " select * from Functions a inner join GROUPFUNCTIONSMAPPINGS b on a.FUNCID=b.FUNCID  ";
            if (_condition.Length > 0)
            {
                sqlcommand += " Where " + _condition + " And PARENTNODE='T00' AND b.GRPID='" + GPRID + "' and canread=1 ";
            }
            else
            {
                sqlcommand += " Where  PARENTNODE='T00' AND b.GRPID='" + GPRID + "' and canread=1 ";
            }
            sqlcommand += orderby;


            
            int _total_rec = 0;

            try
            {
                cmd = new OracleCommand("select count(*) as total_counter from (" + sqlcommand + ") ", conn);
                cmd.CommandType = CommandType.Text;
                _total_rec = int.Parse(cmd.ExecuteScalar().ToString().Trim());
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

            JArray newJa02 = new JArray(); //第二層

            try
            {
                DataSet dsdata = new DataSet();
                OracleDataAdapter _da = new OracleDataAdapter();
                cmd = new OracleCommand(sqlcommand, conn);
                _da.SelectCommand = cmd;
                int _recgot = _da.Fill(dsdata);

                JObject _item;
                for (int index = 0; index < _recgot; index++)
                {
                    _item = new JObject();

                    _item.Add(new JProperty("project_file_id", dsdata.Tables[0].Rows[index]["FUNCID"].ToString()));
                    _item.Add(new JProperty("project_name", dsdata.Tables[0].Rows[index]["FUNCNAME"].ToString()));
                    _item.Add(new JProperty("link", dsdata.Tables[0].Rows[index]["WEBURL"].ToString()));

                    newJa02.Add(_item);
                }
                try { conn.Close(); } catch { }
                return APCommonFun.ReturnSuccess(newJa02, _total_rec);

            }
            catch(Exception ex)
            {
                try { conn.Close(); } catch { }
                return APCommonFun.ReturnError(mStrFuncName, "Common exception.", ex.ToString(), "F", new JArray());
            }
        }
    }
}
