using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Net.Http;
//
using NPOI;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Client;
using System.Drawing;
using System.Security.Cryptography;
using System.Net;
using rc_interface_API.Controllers;

namespace LTCH_API.Controllers
{

    public class APCommonFun
    {
        ////public class HelloController : Controller  //取得環境資料夾
        //// {
        //public readonly IHostingEnvironment _hostEnvironment;

        //public APCommonFun(IHostingEnvironment hostEnvironment)
        //{
        //    _hostEnvironment = hostEnvironment;
        //}
        //// }
       

        /// <summary>
        /// 紀錄錯誤訊息
        /// </summary>
        /// <param name="ErrorLog"></param>
        /// <returns></returns>
        public static string Error(string ErrorLog)
        {
            /////*以純文字檔案紀錄Bug資訊*/

            //////今日日期
            DateTime Date = DateTime.Now;
            string TodyMillisecond = Date.ToString("yyyy-MM-dd HH:mm:ss");
            string Tody = Date.ToString("yyyy-MM-dd");
            string Folder = Date.ToString("yyyy-MM");

            string strPath = System.Environment.CurrentDirectory;

            //如果此路徑沒有資料夾
            if (!Directory.Exists(strPath + "/XG_Log/" + Folder))
            {
                //新增資料夾
                Directory.CreateDirectory(strPath + "/XG_Log/" + Folder);
            }

            //把內容寫到目的檔案，若檔案存在則附加在原本內容之後(換行)
            File.AppendAllText(strPath + "/XG_Log/" + Folder + "/" + Tody + "_Error.txt", "\r\n" + TodyMillisecond + "： " + ErrorLog);

            return Tody;
        }

        public static string Log_Event(string EventLog)
        {
            /////*以純文字檔案紀錄Bug資訊*/

            //////今日日期
            DateTime Date = DateTime.Now;
            string TodyMillisecond = Date.ToString("yyyy-MM-dd HH:mm:ss");
            string Tody = Date.ToString("yyyy-MM-dd");
            string Folder = Date.ToString("yyyy-MM");

            string strPath = System.Environment.CurrentDirectory;

            //如果此路徑沒有資料夾
            if (!Directory.Exists(strPath + "/XG_Log/" + Folder))
            {
                //新增資料夾
                Directory.CreateDirectory(strPath + "/XG_Log/" + Folder);
            }

            //把內容寫到目的檔案，若檔案存在則附加在原本內容之後(換行)
            File.AppendAllText(strPath + "/XG_Log/" + Folder + "/" + Tody + "_Log_Event.txt", "\r\n" + TodyMillisecond + "： " + EventLog);

            return Tody;
        }

        public static string Log_CallAPI_Message(string CallAPI_MessageLog)
        {
            /////*以純文字檔案紀錄Bug資訊*/

            //////今日日期
            DateTime Date = DateTime.Now;
            string TodyMillisecond = Date.ToString("yyyy-MM-dd HH:mm:ss");
            string Tody = Date.ToString("yyyy-MM-dd");
            string Folder = Date.ToString("yyyy-MM");

            string strPath = System.Environment.CurrentDirectory;

            //如果此路徑沒有資料夾
            if (!Directory.Exists(strPath + "/XG_Log/" + Folder))
            {
                //新增資料夾
                Directory.CreateDirectory(strPath + "/XG_Log/" + Folder);
            }

            //把內容寫到目的檔案，若檔案存在則附加在原本內容之後(換行)
            File.AppendAllText(strPath + "/XG_Log/" + Folder + "/" + Tody + "_Log_CallAPI_Message.txt", "\r\n" + TodyMillisecond + "： " + CallAPI_MessageLog);

            return Tody;
        }


        public static string LogLogin(string LoginID)
        {
            /////*以純文字檔案紀錄Bug資訊*/

            //今日日期
            DateTime Date = DateTime.Now;
            string TodyMillisecond = Date.ToString("yyyy-MM-dd HH:mm:ss");
            string Tody = Date.ToString("yyyy-MM-dd");

            //////如果此路徑沒有資料夾
            ////if (!Directory.Exists(Properties.Settings.Default.AppLog))
            ////{
            ////    //新增資料夾
            ////    Directory.CreateDirectory(Properties.Settings.Default.AppLog);
            ////}

            //////把內容寫到目的檔案，若檔案存在則附加在原本內容之後(換行)
            ////File.AppendAllText(Properties.Settings.Default.AppLog + "Temp" + Tody + ".txt", "\r\n" + TodyMillisecond + "： " + LoginID);

            return Tody;
        }


        //把字串長度都補齊，讓Word排版一致
        public static string TurnStringLength(string OldStr, int lenStr, int Blank_F = 0, string IsMiddle = "N")
        //                                      轉換前字串     轉換後字串    字串前面要空白幾格     字串是否置中
        {

            string NewStr = OldStr;

            if (Blank_F != 0)
            {
                for (int i = 0; i < Blank_F; i++)
                {
                    NewStr = " " + NewStr;
                }
            }

            //先把前面空白補齊
            if (IsMiddle == "Y")  //字串要置中
            {
                byte[] byteStr2 = Encoding.GetEncoding("big5").GetBytes(NewStr); //把string轉為byte 

                int iStart = (lenStr - byteStr2.Length) / 2; ;

                for (int i = 0; i < iStart; i++)
                {
                    NewStr = " " + NewStr;
                }
            }

            byte[] byteStr = Encoding.GetEncoding("big5").GetBytes(NewStr); //把string轉為byte 
            if (byteStr.Length >= lenStr)
            {
            }
            else
            {
                //再把後面空白補齊
                for (int i = 0; i < lenStr - byteStr.Length - 1; i++)
                {
                    NewStr = NewStr + " ";
                }
            }
            NewStr = NewStr + " ";
            return NewStr;

            //string NewStr = " " + OldStr;

            //byte[] byteStr = Encoding.GetEncoding("big5").GetBytes(textBox1.Text); //把string轉為byte 
            //byteStr.Length
            //if (OldStr.Length >= lenStr)
            //{
            //}
            //else
            //{
            //    for (int i = 0; i < lenStr - OldStr.Length - 1; i++)
            //    {
            //        NewStr = NewStr + " ";
            //    }
            //}
            return NewStr;
        }

        /// <summary>
        /// 日 (dd)前面補0
        /// </summary>
        /// <param name="ConverStr"></param>
        /// <returns></returns>
        public static string DayAdd_0(string ConverStr)
        {
            if (ConverStr.Length <= 1)
            {
                ConverStr = "0" + ConverStr ;   // 前面補0
                return ConverStr;
            }
            else
            {
                return ConverStr.Trim();
            }
        }

        public static string CheckData(string ConverStr)
        {
            if (ConverStr == "NoData")
            {
                ConverStr = "";
                return ConverStr;
            }
            else
            {
                return ConverStr.Trim();
            }
        }

        public static string CDBNulltrim(string ConverStr)
        {
            if (ConverStr == null)
            {
                ConverStr = "";
                return ConverStr;
            }
            else
            {
                return ConverStr.Trim();
            }
        }

        [Obsolete]
        public static DataTable GetMyDataTable_Npgsql(string ComStr)
        {


            DataTable myDataTable = new DataTable();

            //APCommonFun ap = new APCommonFun(new IHostingEnvironment());

            //string FullFilePath = $"{ap._hostEnvironment.ContentRootPath }" ;

            ////// var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            //var builder = new ConfigurationBuilder().SetBasePath($"{ _hostEnvironment.ContentRootPath }").AddJsonFile("settings.json");
            //建立配置根物件
            //var configurationRoot = builder.Build();
            ////取配置根下的 name 部分
            //var nameSection = configurationRoot.GetSection("db_ip");


            //    var domain = _config["Domain"];

            //   NpgsqlConnection conn = new NpgsqlConnection(_config.);

            // SqlConnection icn = OpenConn(Properties.Settings.Default.info1, Properties.Settings.Default.info4, Properties.Settings.Default.info2, Properties.Settings.Default.info3);
            //  NpgsqlConnection conn = new NpgsqlConnection("Server=61.31.168.222; Port=5432;User Id=postgres;Password=Msi83^c;Database=test;");
            string Info_1 = Startup.ReadFromAppSettings("Info_1");
            string Info_2 = Startup.ReadFromAppSettings("Info_2");
            string Info_3 = Startup.ReadFromAppSettings("Info_3");
            string Info_4 = Startup.ReadFromAppSettings("Info_4");
            string Info_5 = Startup.ReadFromAppSettings("Info_5");

            //NpgsqlConnection conn = new NpgsqlConnection("Host=61.31.168.222; Port=5432;Username=postgres;Password=Msi83^c;Database=LTCH;");
            NpgsqlConnection conn = new NpgsqlConnection("Host=" + Info_1 + "; Port=" + Info_2 + ";Username=" + Info_3 + ";Password=" + Info_4 + ";Database=" + Info_5 + ";");
            //   NpgsqlConnection conn = new NpgsqlConnection("Host=localhost; Port=5432;Username=postgres;Password=!qazASD;Database=LTCH;");
            conn.Open();

            try
            {
                NpgsqlCommand command = new NpgsqlCommand(ComStr, conn);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

                DataSet ds = new DataSet();
                ds.Clear();
                da.Fill(ds);
                myDataTable = ds.Tables[0];
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            finally
            {
                conn.Close();
            }



            //SqlCommand isc = new SqlCommand();
            //SqlDataAdapter da = new SqlDataAdapter(isc);
            //isc.Connection = icn;
            //isc.CommandText = ComStr;
            //isc.CommandTimeout = 600;

            //DataSet ds = new DataSet();
            //ds.Clear();
            //da.Fill(ds);
            //myDataTable = ds.Tables[0];
            //if (icn.State == ConnectionState.Open) icn.Close();
            return myDataTable;
        }

        


        public static string RunSQL_Npgsql(string ComStr)
        {
            string result = "true";


            string Info_1 = Startup.ReadFromAppSettings("Info_1");
            string Info_2 = Startup.ReadFromAppSettings("Info_2");
            string Info_3 = Startup.ReadFromAppSettings("Info_3");
            string Info_4 = Startup.ReadFromAppSettings("Info_4");
            string Info_5 = Startup.ReadFromAppSettings("Info_5");

            //NpgsqlConnection conn = new NpgsqlConnection("Host=61.31.168.222; Port=5432;Username=postgres;Password=Msi83^c;Database=LTCH;");
            NpgsqlConnection conn = new NpgsqlConnection("Host="+ Info_1 + "; Port="+ Info_2 + ";Username="+ Info_3+ ";Password="+ Info_4 + ";Database="+ Info_5 + ";");

            //     NpgsqlConnection conn = new NpgsqlConnection("Host=localhost; Port=5432;Username=postgres;Password=!qazASD;Database=LTCH;");
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand(ComStr, conn);
            NpgsqlTransaction sqlTransaction = conn.BeginTransaction();
            try
            {
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

                command.Transaction = sqlTransaction;
                command.ExecuteNonQuery();
                sqlTransaction.Commit();


                if (conn.State == ConnectionState.Open) conn.Close();
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                sqlTransaction.Rollback();
                throw (ex);
            }
            finally
            {
                conn.Close();
            }

            ////SqlConnection icn = OpenConn(Properties.Settings.Default.info1, Properties.Settings.Default.info4, Properties.Settings.Default.info2, Properties.Settings.Default.info3);

            ////SqlCommand cmd = new SqlCommand(ComStr, icn);
            ////SqlTransaction sqlTransaction = icn.BeginTransaction();
            ////try
            ////{
            ////    cmd.Transaction = sqlTransaction;
            ////    cmd.ExecuteNonQuery();
            ////    sqlTransaction.Commit();
            ////}
            ////catch (Exception ex)
            ////{
            ////    result = ex.ToString();
            ////    sqlTransaction.Rollback();
            ////    throw (ex);
            ////}
            ////if (icn.State == ConnectionState.Open) icn.Close();
            return result;
        }




        public static DataTable GetMyDataTable_MySQL(string ComStr)
        {
            DataTable myDataTable = new DataTable();

            string Info_1 = Startup.ReadFromAppSettings("Info_1");
            string Info_2 = Startup.ReadFromAppSettings("Info_2");
            string Info_3 = Startup.ReadFromAppSettings("Info_3");
            string Info_4 = Startup.ReadFromAppSettings("Info_4");
            string Info_5 = Startup.ReadFromAppSettings("Info_5");



                MySqlConnection icn = MyOpenConn(Info_1, Info_5, Info_3, Info_4);
            //正式環境
            //  MySqlConnection icn = MyOpenConn("106.12.92.187", "lhemr","lh", "LH@1234");
          //  MySqlConnection icn = MyOpenConn("34.81.25.84", "antique", "usr_anti", "LN8wae;si*3");

            //   NpgsqlConnection conn = new NpgsqlConnection("Host=61.31.168.222; Port=5432;Username=postgres;Password=Msi83^c;Database=LTCH;");
            //   NpgsqlConnection conn = new NpgsqlConnection("Host=" + Info_1 + "; Port=" + Info_2 + ";Username=" + Info_3 + ";Password=" + Info_4 + ";Database=" + Info_5 + ";");

            MySqlCommand isc = new MySqlCommand();
            MySqlDataAdapter da = new MySqlDataAdapter(isc);
            isc.Connection = icn;
            isc.CommandText = ComStr;
            isc.CommandTimeout = 600;

            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            myDataTable = ds.Tables[0];
            if (icn.State == ConnectionState.Open) icn.Close();
            return myDataTable;
        }


        public static string RunSQL_MySQL(string ComStr)
        {
            string result = "true";

            string Info_1 = Startup.ReadFromAppSettings("Info_1");
            string Info_2 = Startup.ReadFromAppSettings("Info_2");
            string Info_3 = Startup.ReadFromAppSettings("Info_3");
            string Info_4 = Startup.ReadFromAppSettings("Info_4");
            string Info_5 = Startup.ReadFromAppSettings("Info_5");

            MySqlConnection icn = MyOpenConn(Info_1, Info_5, Info_3, Info_4);

            //正式環境
            //   MySqlConnection icn = MyOpenConn("106.12.92.187", "lhemr","lh", "LH@1234");


            MySqlCommand cmd = new MySqlCommand(ComStr, icn);
            MySqlTransaction mySqlTransaction = icn.BeginTransaction();
            try
            {
                cmd.Transaction = mySqlTransaction;
                cmd.ExecuteNonQuery();
                mySqlTransaction.Commit();
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                mySqlTransaction.Rollback();
                throw (ex);
            }
            if (icn.State == ConnectionState.Open) icn.Close();
            return result;
        }

        public static DataTable GetDataTable(string ComStr)
        {
            DataTable myDataTable = new DataTable();

            string conStr = Startup.ReadFromAppSettings("OracleConnectionString");
            using (OracleConnection conn = new OracleConnection(conStr))
            {
                conn.Open();
                OracleCommand command = new OracleCommand(ComStr, conn);
                DataSet ds = new DataSet();
                command.CommandText = ComStr;
                command.CommandTimeout = 600;
                OracleDataAdapter da = new OracleDataAdapter(command);
                da.Fill(ds);

                myDataTable = ds.Tables[0];
                return myDataTable;
            }

        }

        public static DataSet GetDataSet(string ComStr)
        {
            string conStr = Startup.ReadFromAppSettings("OracleConnectionString");
            using (OracleConnection conn = new OracleConnection(conStr))
            {
                conn.Open();
                OracleCommand command = new OracleCommand(ComStr, conn);
                DataSet ds = new DataSet();
                command.CommandText = ComStr;
                command.CommandTimeout = 600;
                OracleDataAdapter da = new OracleDataAdapter(command);
                da.Fill(ds);
                return ds;
            }

        }
        public static void ExecSqlCommand(string ComStr)
        {

            string conStr = Startup.ReadFromAppSettings("OracleConnectionString"); ;

            using (OracleConnection connection = new OracleConnection(conStr))
            {
                OracleCommand command = new OracleCommand(ComStr, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static MySqlConnection MyOpenConn(string Server, string Database, string dbuid, string dbpwd)
        {
            string cnstr = string.Format("server={0};database={1};uid={2};pwd={3};Connect Timeout = 180; CharSet=utf8", Server, Database, dbuid, dbpwd);
            MySqlConnection icn = new MySqlConnection();
            icn.ConnectionString = cnstr;
            if (icn.State == ConnectionState.Open) icn.Close();
            icn.Open();
            return icn;
        }


        ////public static SqlConnection OpenConn(string Server, string Database, string dbuid, string dbpwd)
        ////{
        ////    //////Console.OutputEncoding = System.Text.Encoding.UTF8;
        ////    ////string cnstr = string.Format("Persist Security Info=False;User ID={0};Password={1};Initial Catalog={2};Server={3}", dbuid, dbpwd, Database, Server);
        ////    ////SqlConnection icn = new SqlConnection();
        ////    ////icn.ConnectionString = cnstr;
        ////    ////if (icn.State == ConnectionState.Open) icn.Close();
        ////    ////icn.Open();
        ////    return icn;
        ////}



        public static DataTable GetMyDataTable_MySQL_His(string ComStr)
        {
            DataTable myDataTable = new DataTable();

            string Info_1 = Startup.ReadFromAppSettings("Info_1_his");
            string Info_2 = Startup.ReadFromAppSettings("Info_2_his");
            string Info_3 = Startup.ReadFromAppSettings("Info_3_his");
            string Info_4 = Startup.ReadFromAppSettings("Info_4_his");
            string Info_5 = Startup.ReadFromAppSettings("Info_5_his");


            MySqlConnection icn = MyOpenConn(Info_1, Info_5, Info_3, Info_4);
            //正式環境
            //  MySqlConnection icn = MyOpenConn("106.12.92.187", "lhemr","lh", "LH@1234");


            //NpgsqlConnection conn = new NpgsqlConnection("Host=61.31.168.222; Port=5432;Username=postgres;Password=Msi83^c;Database=LTCH;");
            //   NpgsqlConnection conn = new NpgsqlConnection("Host=" + Info_1 + "; Port=" + Info_2 + ";Username=" + Info_3 + ";Password=" + Info_4 + ";Database=" + Info_5 + ";");

            MySqlCommand isc = new MySqlCommand();
            MySqlDataAdapter da = new MySqlDataAdapter(isc);
            isc.Connection = icn;
            isc.CommandText = ComStr;
            isc.CommandTimeout = 600;

            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            myDataTable = ds.Tables[0];
            if (icn.State == ConnectionState.Open) icn.Close();
            return myDataTable;
        }


        public static string RunSQL_MySQL_His(string ComStr)
        {
            string result = "true";

            string Info_1 = Startup.ReadFromAppSettings("Info_1_his");
            string Info_2 = Startup.ReadFromAppSettings("Info_2_his");
            string Info_3 = Startup.ReadFromAppSettings("Info_3_his");
            string Info_4 = Startup.ReadFromAppSettings("Info_4_his");
            string Info_5 = Startup.ReadFromAppSettings("Info_5_his");

            MySqlConnection icn = MyOpenConn(Info_1, Info_5, Info_3, Info_4);

            //正式環境
            //   MySqlConnection icn = MyOpenConn("106.12.92.187", "lhemr","lh", "LH@1234");


            MySqlCommand cmd = new MySqlCommand(ComStr, icn);
            MySqlTransaction mySqlTransaction = icn.BeginTransaction();
            try
            {
                cmd.Transaction = mySqlTransaction;
                cmd.ExecuteNonQuery();
                mySqlTransaction.Commit();
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                mySqlTransaction.Rollback();
                throw (ex);
            }
            if (icn.State == ConnectionState.Open) icn.Close();
            return result;
        }

        /// <summary>
        /// 呼叫-慧誠API 使用 (2022-05-03 Ben)
        /// </summary>
        /// <param name="jArray"></param>
        /// <param name="imedtac_API_Base"></param>
        /// <param name="imedtac_API_Url"></param>
        /// <returns></returns>
        public static string Call_imedtac_Api(JArray jArray, string imedtac_API_Base, string imedtac_API_Url)
        {
            // (1) 呼叫API 
            var client = new HttpClient();

            // (2) 設定基底URL
            client.BaseAddress = new Uri(imedtac_API_Base);

            // (3) Initializes a new instance of the HttpRequestMessage class with an HTTP method and a request Uri
            HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Post, imedtac_API_Url);

            // (4) 設定body內容和格式
            request.Content = new StringContent(jArray.ToString(), Encoding.UTF8, "application/json");

            var response = client.SendAsync(request).Result;

            string return_tmp = "false";
            // (5) 判斷是否連線成功
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return_tmp = "true";
                    APCommonFun.Log_Event("[呼叫慧誠API，寫入慧誠資料庫]97： Post慧誠 API 呼叫成功 -" + imedtac_API_Url); // (Log)_97
                }
                catch (Exception ex)
                {
                    APCommonFun.Error("[呼叫慧誠API，寫入慧誠資料庫]99： Post慧誠 API 呼叫失敗 -" + ex.ToString()); // (Log)_99
                }
            }

            return return_tmp;
        }


        /// <summary>
        /// 讀取Excel 匯成DataTable
        /// </summary>
        /// <param name="filepath">匯入的檔案路徑（包括檔名）</   param>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列    名</param>
        /// <returns>DataTable</returns>
        public static DataTable ExcelToDataTable(string filePath, string sheetName, bool isFirstRowColumn, int firstRowNum_para, string LastColumn)
        {
            DataTable data = new DataTable();
            FileStream fs;
            int startRow = 0;
            using (fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    IWorkbook workbook = filePath.Contains(".xlsx") ? (IWorkbook)new XSSFWorkbook(fs) : new HSSFWorkbook(fs);//xlsx使用XSSFWorkbook， xls使用HSSFWorkbokk
                    ISheet sheet = workbook.GetSheet(sheetName) ?? workbook.GetSheetAt(0);//如果沒有找到指sheetName 對應的sheet，則嘗試獲取第一個sheet
                    if (sheet != null)
                    {
                        IRow firstrow = sheet.GetRow(firstRowNum_para);//第一行
                        int firstCellNum = firstrow.FirstCellNum;// 行第一個cell的編號,從0開始
                        int lastCellNum = firstrow.LastCellNum; //  行最後一個cell的編號 即總的列數,（不忽略中間某    列空格）
                        if (isFirstRowColumn)//如果第一行是表格列頭
                        {
                            for (int i = firstCellNum; i < lastCellNum; i++)
                            {
                                ICell cell = firstrow.GetCell(i);
                                if (cell != null)
                                {
                                    string cellValue = cell.StringCellValue;
                                    if (cellValue != null)
                                    {
                                        DataColumn column = new DataColumn(cellValue);
                                        data.Columns.Add(column);
                                    }
                                }
                            }
                            startRow = sheet.FirstRowNum + 1;
                        }
                        else
                        {
                            startRow = sheet.FirstRowNum;
                        }
                        //讀資料行
                        int rowCont = sheet.LastRowNum;                       
                        for (int i = startRow; i <= rowCont; i++)
                        {                           
                            IRow row = sheet.GetRow(i); 
                            DataRow dataRow = data.NewRow();
                            //判斷需要讀取的最後一行
                            if (row != null && (row.GetCell(row.FirstCellNum) != null && row.GetCell(row.FirstCellNum).ToString() != LastColumn))
                            {
                                for (int j = row.FirstCellNum; j < lastCellNum; j++)
                                {
                                    dataRow[j] = row.GetCell(j).ToString();
                                }
                                data.Rows.Add(dataRow);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    return data;
                }
                catch (Exception ex)
                {
                    //Debug.WriteLine("Exception: " + ex.Message);
                    return null;
                }
                finally
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        /// <summary>
        /// 日期轉換：MM/dd/yyyy to yyyy-MM-dd
        /// </summary>
        /// <param name="DateChang"></param>
        /// <returns>usbs</returns>
        public static string DateChangToEn(string DateChang)
        {
            string usbs = "";
            if (DateChang.Length == 10) {
                usbs = DateChang.Substring(6,4) + "-" + DateChang.Substring(0, 2) + "-" + DateChang.Substring(3, 2);
            }
            
            return usbs;
        }

        /// <summary>
        /// 時間轉換：HHmm to HH:mm
        /// </summary>
        /// <param name="TimeChangPara"></param>
        /// <returns>usbs</returns>
        public static string TimeChang(string TimeChangPara)
        {
            string usbs = "";
            if (TimeChangPara != "") {
                usbs = TimeChangPara.Substring(0, 2) + ":" + TimeChangPara.Substring(2, 2);
            }
            return usbs;
        }

        /// <summary>
        /// 確認是否有 ' (SQL寫入需要處理)
        /// </summary>
        /// <param name="addPara"></param>
        /// <returns></returns>
        public static string AddChange(string addPara)
        {
            string usbs = "";
            string[] usbsA = { };
            usbsA = addPara.Split("'");
            if (usbsA.Length == 2)
            {
                usbs = usbsA[0] + "''" + usbsA[1];
            }
            else if (usbsA.Length == 3) { usbs = usbsA[0] + "''" + usbsA[1] + "''" + usbsA[2]; }
            else { usbs = usbsA[0]; }
            return usbs;
        }

        /// <summary>
        /// 確認欄位是否為nil
        /// </summary>
        /// <param name="Nil"></param>
        /// <returns></returns>
        public static string CheckNil(string Nil)
        {
            string usbs = "";
            if (Nil == "nil")
            {
                usbs = "";
            }
            else { usbs = Nil; }
            

            return usbs;
        }
        /// <summary>
        /// 時間字串轉 hh:mm 格式 5/4 Jamie
        /// </summary>
        /// <param name="timeStr">傳入值</param>
        /// <returns>newNumber</returns>
        public static string timeformat(string timeStr)
        {

            string timeformat = "";

            try
            {
                string tfront = timeStr.Substring(0, 2);
                string tback = timeStr.Substring(2, 2);
                timeformat = tfront + ":" + tback;
            }
            catch (Exception ex)
            {
                Error("timeformat Error-" + ex.ToString());
            }
            return timeformat;
        }

        public static DataTable GetCostPlusSumAll(DataTable dt, string strCType, string pspnr)
        {
            DataRow dataRow = dt.NewRow();
            DataView defaultView = dt.DefaultView;
            Int64 num = 0;
            Int64 num2 = 0;
            dataRow[0] = "";
            if (strCType != "")
            {
                switch (strCType.Trim())
                {
                    case "工程成本C＝CM+CA+CL+CF 合計":
                        {
                            for (int k = 0; k < defaultView.Count; k++)
                            {
                                if (defaultView[k][0].ToString().Trim() != "" && defaultView[k][1].ToString().Trim() != "存貨S")
                                {
                                    defaultView[k][3].ToString();
                                    num += Convert.ToInt64(defaultView[k][3].ToString().Replace(",", ""));
                                    num2 += Convert.ToInt64(defaultView[k][4].ToString().Replace(",", ""));
                                }
                            }
                            dataRow[0] = "**";
                            break;
                        }
                    case "總工程成本CT=C+S":
                        {
                            for (int j = 0; j < defaultView.Count; j++)
                            {
                                if (defaultView[j][0].ToString().Trim() != "" && defaultView[j][1].ToString().Trim() != "工程成本C＝CM+CA+CL+CF 合計")
                                {
                                    num += Convert.ToInt64(defaultView[j][3].ToString().Replace(",", ""));
                                    num2 += Convert.ToInt64(defaultView[j][4].ToString().Replace(",", ""));
                                }
                            }
                            dataRow[0] = "***";
                            break;
                        }
                    case "成本加成管利6％":
                        {
                            for (int l = 0; l < defaultView.Count; l++)
                            {
                                if (defaultView[l][0].ToString().Trim() == "***")
                                {
                                    string sql = "SELECT VALUE FROM ManagerPercent WHERE PSPNR ='" + pspnr + "' ";
                                    string managePercent = string.Empty;
                                    DataTable dtt = APCommonFun.GetDataTable(sql);
                                    if (dtt.Rows.Count > 0)
                                    {
                                        foreach (DataRow dr in dtt.Rows)
                                        {
                                            managePercent = APCommonFun.CDBNulltrim(dr["VALUE"].ToString());
                                        }
                                    }
                                    double managePercentDouble = 0.0;
                                    try
                                    {
                                        managePercentDouble = Convert.ToDouble(managePercent);
                                    }
                                    catch (Exception)
                                    {

                                    }

                                    strCType = "成本加成管利" + Convert.ToString(managePercentDouble * 100.0) + "％";
                                    num = Convert.ToInt64(Math.Round(Convert.ToDouble(defaultView[l][3].ToString().Replace(",", "")) * managePercentDouble));
                                    num2 = Convert.ToInt64(Math.Round(Convert.ToDouble(defaultView[l][4].ToString().Replace(",", "")) * managePercentDouble));
                                }
                            }
                            dataRow[0] = "****";
                            break;
                        }
                    case "總計":
                        {
                            Int64 num3 = 0;
                            Int64 num4 = 0;
                            Int64 num5 = 0;
                            Int64 num6 = 0;
                            for (int i = 0; i < defaultView.Count; i++)
                            {
                                if (defaultView[i][0].ToString().Trim() == "***")
                                {
                                    num5 = Convert.ToInt64(defaultView[i][3].ToString().Replace(",", ""));
                                    num6 = Convert.ToInt64(defaultView[i][4].ToString().Replace(",", ""));
                                }
                                if (defaultView[i][0].ToString().Trim() == "****")
                                {
                                    num3 = Convert.ToInt64(defaultView[i][3].ToString().Replace(",", ""));
                                    num4 = Convert.ToInt64(defaultView[i][4].ToString().Replace(",", ""));
                                }
                                num = num5 + num3;
                                num2 = num6 + num4;
                            }
                            dataRow[0] = "*****";
                            break;
                        }
                }
            }
            dataRow[1] = strCType.Trim();
            dataRow[2] = "";
            dataRow[3] = $"{num:N0}";
            dataRow[4] = $"{num2:N0}";
            dataRow[5] = $"{num2 - num:N0}";
            if (num2 != 0)
            {                
                dataRow[6] = string.Format("{0:N2}%", Convert.ToString(Math.Round(Convert.ToDouble(num2 - num) / Convert.ToDouble(num2), 4) * 100.0));
                if (dataRow[6].ToString().Contains("."))
                {
                    string[] deci = dataRow[6].ToString().Split('.');
                    if (deci[1].ToString().Length > 2)
                    {
                        dataRow[6] = deci[0].ToString() + "." + deci[1].ToString().Substring(0, 2) + "%";
                    }
                }
            }
            else
            {
                dataRow[6] = "0%";
            }
            dt.Rows.Add(dataRow);
            return dt;
        }
        public static DataTable CreateCostPlusSum(DataTable dt, DataView dv, string strMCTXT)
        {
            Int64 num = 0;
            Int64 num2 = 0;
            dv.RowFilter = "MCTXT = '" + strMCTXT + "' or MCTXT_REAL = '" + strMCTXT + "'";
            for (int i = 0; i < dv.Count; i++)
            {
                num = ((!(dv[i][6].ToString().Trim() != "")) ? num : (num + Convert.ToInt64(dv[i][6].ToString().Trim())));
                num2 = ((!(dv[i][2].ToString().Trim() != "")) ? num2 : (num2 + Convert.ToInt64(dv[i][2].ToString().Trim())));
            }
            DataRow dataRow = dt.NewRow();
            dataRow[0] = "**";
            dataRow[1] = strMCTXT + " 合計";
            dataRow[2] = "";
            dataRow[3] = $"{num:N0}";
            dataRow[4] = $"{num2:N0}";
            dataRow[5] = $"{num2 - num:N0}";
            if (num2 != 0)
            {                
                dataRow[6] = string.Format("{0:0.##}%", Convert.ToString(Math.Round(Convert.ToDouble(num2 - num) / Convert.ToDouble(num2), 4) * 100.0));
                if (dataRow[6].ToString().Contains("."))
                {
                    string[] deci = dataRow[6].ToString().Split('.');
                    if (deci[1].ToString().Length > 2)
                    {
                        dataRow[6] = deci[0].ToString() + "." + deci[1].ToString().Substring(0, 2) + "%";
                    }
                }
            }
            else
            {
                dataRow[6] = "0%";
            }
            dt.Rows.Add(dataRow);
            for (int j = 0; j < dv.Count; j++)
            {
                DataRow dataRow2 = dt.NewRow();
                dataRow2[0] = "";
                Int64 num3 = 0;
                Int64 num4 = 0;
                Int64 num5 = 0;
                if (dv[j][0].ToString().Trim() != "")
                {
                    dataRow2[1] = dv[j][0].ToString();
                }
                else
                {
                    dataRow2[1] = dv[j][4].ToString();
                }
                if (dv[j][1].ToString().Trim() != "")
                {
                    dataRow2[2] = dv[j][1].ToString();
                }
                else
                {
                    dataRow2[2] = dv[j][5].ToString();
                }
                if (dv[j][6].ToString().Trim() != "" && dv[j][6].ToString().Trim() != "0")
                {
                    dataRow2[3] = $"{dv[j][6]:N0}";
                    num4 = Convert.ToInt64(dv[j][6]);
                }
                else
                {
                    dataRow2[3] = 0;
                }
                if (dv[j][2].ToString().Trim() != "" && dv[j][2].ToString().Trim() != "0")
                {
                    dataRow2[4] = $"{dv[j][2]:N0}";
                    num3 = Convert.ToInt64(dv[j][2]);
                }
                else
                {
                    dataRow2[4] = 0;
                }
                dataRow2[5] = $"{num3 - num4:N0}";
                num5 = num3 - num4;
                if (num3 != 0)
                {
                    dataRow2[6] = string.Format("{0:0.##}%", Convert.ToString(Math.Round(Convert.ToDouble(num5) / Convert.ToDouble(num3), 4) * 100.0));
                    if (dataRow2[6].ToString().Contains("."))
                    {
                        string[] deci = dataRow2[6].ToString().Split('.');
                        if (deci[1].ToString().Length > 2)
                        {
                            dataRow2[6] = deci[0].ToString() + "." + deci[1].ToString().Substring(0, 2) + "%";
                        }
                    }
                }
                else
                {
                    dataRow2[6] = "0%";
                }
                dt.Rows.Add(dataRow2);
            }
            return dt;
        }

        //===========Added by Polo==================================================
        /// <summary>
        /// 系統參數設定-取得 ManagePercent 預設值 
        /// 專案對應成本報表計算設定-->計算百分比:
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultManagerPencent()
        {
            try
            {
                return Startup.ReadFromAppSettings("ManagerPencent");
            }
            catch { }
            return "0.06";
        }

        /// <summary>
        /// 
        /// Get the Oracle connection
        /// </summary>
        /// <returns></returns>
        public static OracleConnection GetOracleConnection()
        {
            try
            {
                string conn = Startup.ReadFromAppSettings("OracleConnectionString");

                OracleConnection oralceConnection = new OracleConnection(conn);
                oralceConnection.Open();

                return oralceConnection;
            }
            catch
            {

            }

            return null;
        }

        /// <summary>
        /// 讀取專案摘要
        /// </summary>
        /// <param name="filepathname"></param>
        /// <returns></returns>
        public static string ReadFileContent(string filename)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (StreamReader streamReader = new StreamReader(filename, Encoding.GetEncoding(950)))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 讀取專案圖檔轉成base64
        /// </summary>
        /// <param name="filepathname"></param>
        /// <returns></returns>
        public static string ReadImageFileToBase64(string filename)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (!File.Exists(filename))
            {
                return "";
            }
            try
            {
                byte[] imgbytes = null;
                using (Image img = Image.FromFile(filename))
                {
                    ImageConverter imgconvert = new ImageConverter();
                    imgbytes = (byte[])imgconvert.ConvertTo(img, typeof(byte[]));
                }
                if (imgbytes == null)
                    return "";

                return Convert.ToBase64String(imgbytes);
            }
            catch { }
            return "";
        }

        public static object ReturnError(string functionName, string errmsg, string result = "F", object data = null)
        {
            errmsg = string.Format("[{0}]:{1}", functionName, errmsg);
            Error(errmsg);
            if (data == null)
                data = "";
            return new
            {
                Result = result,
                Message = errmsg,
                TotalRec = "0",
                Data = data
            };
        }

        public static object ReturnError(string functionName, string errmsg, string exceptionmsg, string result = "F", object data = null)
        {
            errmsg = string.Format("[{0}]:{1}", functionName, errmsg);
            Error(string.Format("[{0}]:{1}, Exception:{2}", functionName, errmsg, exceptionmsg));
            if (data == null)
                data = "";

            if (result == "F")
                errmsg = "失敗" + exceptionmsg;
            else if (result == "R")
                errmsg = "必填或重覆";
            return new
            {
                Result = result,
                Message = errmsg,
                TotalRec = "0",
                Data = data
            };
        }

        public static object ReturnSuccess(object data, int total_count)
        {
            return new
            {
                Result = "T",
                Message = "成功",
                TotalRec = total_count.ToString(),
                Data = data
            };
        }

        public static object ReturnSuccess(object data)
        {
            return new
            {
                Result = "T",
                Message = "成功",
                Data = data
            };
        }




        private static RijndaelManaged m_oCrypt = new RijndaelManaged();
        private static byte[] m_bytKey = new byte[32];
        private static byte[] m_bytIV = new byte[16];
        private static bool GetKeyFile(string keyfilepath)
        {
            FileStream fileStream = default(FileStream);
            bool gotkey = false;
            try
            {
                fileStream = new FileStream(keyfilepath, FileMode.Open, FileAccess.Read);
                FileStream fileStream2 = fileStream;
                fileStream2.Read(m_bytKey, 0, 32);
                fileStream2.Read(m_bytIV, 0, 16);
                fileStream2 = null;
                m_oCrypt.Key = m_bytKey;
                m_oCrypt.IV = m_bytIV;
                gotkey = true;
            }
            catch (FileNotFoundException ex)
            {
                FileNotFoundException ex2 = ex;
                throw new FileNotFoundException("無法取得指定的Key File", keyfilepath);
            }
            catch (Exception ex3)
            {
                Exception ex4 = ex3;
                throw new Exception("取得指定的Key File時出現未知例外，原因：" + ex4.Message.Replace("\r\n", ""));
            }
            finally
            {
                fileStream?.Close();

            }
            return gotkey;
        }


        public enum CryptographyActionType
        {
            Encrypt,
            Decrypt
        }

        private static string ExecCryptoServiceForString(CryptographyActionType ActionType, string SrcString)
        {
            string text = "";
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            MemoryStream memoryStream = new MemoryStream();
            byte[] bytes = unicodeEncoding.GetBytes(SrcString);
            CryptoStream cryptoStream = (ActionType != 0) ? new CryptoStream(memoryStream, m_oCrypt.CreateDecryptor(m_bytKey, m_bytIV), CryptoStreamMode.Write) : new CryptoStream(memoryStream, m_oCrypt.CreateEncryptor(m_bytKey, m_bytIV), CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.Close();
            byte[] bytes2 = memoryStream.ToArray();
            text = unicodeEncoding.GetString(bytes2);
            memoryStream.Close();
            return text;
        }

        public static string DecryptString(string _src)
        {
            bool keyfileok = GetKeyFile(Startup.ReadFromAppSettings("key_file_path"));
            if (!keyfileok)
            {
                return "";
            }
            try
            {
                return ExecCryptoServiceForString(CryptographyActionType.Decrypt, Encoding.UTF8.GetString(Convert.FromBase64String(_src)));
            }
            catch
            {

            }
            return "";
        }

        public static string EncryptString(string _src)
        {
            bool keyfileok = GetKeyFile(Startup.ReadFromAppSettings("key_file_path"));
            if (!keyfileok)
            {
                return "";
            }
            try
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(ExecCryptoServiceForString(CryptographyActionType.Encrypt, _src)));
            }
            catch
            {

            }
            return "";
        }
        //===========Added by Polo==================================================
    }
}
