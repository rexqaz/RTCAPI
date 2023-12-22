using LTCH_API;
using LTCH_API.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rc_interface_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace rc_interface_API.Controllers._01_Project_Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormDownloadProjectDocPicController : ControllerBase
    {
        private readonly IHostingEnvironment hostingEnvironment1;
        

        public FormDownloadProjectDocPicController(IHostingEnvironment hostingEnvironment)
        {
            hostingEnvironment1 = hostingEnvironment;
        }

        [HttpPost]
        public async Task<object> FormAll(Info_FormDownloadProjectDocPicModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            string file = string.Empty;


            if (Data.file != null)
            {
                file = APCommonFun.CDBNulltrim(Data.file);
            }

            //第一步 : 先判斷有沒有必填未填寫，
            if (file == "") //必填                       
            {
                InputIsok = "N";
                ReturnErr = "執行動作錯誤-file 為必填欄位";
            }

            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[FormDownloadProjectDocPicController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    FilePath = ""
                };
            }

            string sql = "select TYPE, OPTIONS, FILEP,dappl,PERIOD from zcpst14 where FILEP='" + file + "' ";
            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);
                string fileType = string.Empty;
                string apiPath = Startup.ReadFromAppSettings("ApiPath");  //ip
                if (dt.Rows.Count > 0)
                {
                    fileType = GetContentType(dt.Rows[0]["dappl"].ToString());
                }

                string sFilePath = Startup.ReadFromAppSettings("FilePath");
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    using (HttpClient client = new HttpClient(handler))
                    {
                        #region 呼叫遠端 Web API
                        string FooUrl = apiPath;
                        HttpResponseMessage response = null;

                        #region  設定相關網址內容
                        var fooFullUrl = $"{FooUrl}{file}";

                        response = await client.GetAsync(fooFullUrl);
                        #endregion
                        #endregion

                        #region 處理呼叫完成 Web API 之後的回報結果
                        if (response != null)
                        {
                            if (response.IsSuccessStatusCode == true)
                            {
                                Guid guid = Guid.NewGuid();
                                string folder = "XingUpdateFile\\" + guid.ToString();
                                using (var filestream = System.IO.File.Open(folder, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                {
                                    using (var stream = await response.Content.ReadAsStreamAsync())
                                    {
                                        stream.CopyTo(filestream);
                                    }
                                }

                                bool isFileExist = System.IO.File.Exists(folder);

                                if (isFileExist)
                                {
                                    return PhysicalFile(sFilePath + "\\" + guid.ToString(), fileType);
                                }
                                else
                                {
                                    return new
                                    {
                                        Result = "F",
                                        Message = "檔案不存在",
                                        FilePath = folder//sFilePath + file
                                    };
                                }
                            }
                            else
                            {
                                return new
                                {
                                    Result = "F",
                                    Message = string.Format("Error Code:{0}, Error Message:{1}", response.StatusCode, response.RequestMessage),
                                    FilePath = ""
                                };
                            }
                        }
                        else
                        {
                            return new
                            {
                                Result = "F",
                                Message = "應用程式呼叫 API 發生異常",
                                FilePath = ""
                            };
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[FormDownloadProjectDocPicController]99：" + ex.ToString());
                APCommonFun.Error("[FormDownloadProjectDocPicController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString(),
                    FilePath = ""
                };
            }

        }

        public string GetContentType(string fileType)
        {
            switch (fileType.ToUpper())
            {
                default:
                case "HTM":
                case "HTML":
                    return "text/html";
                case "XLS":
                    return "application/vnd.ms-excel";
                case "WRD":
                case "DOC":
                    return "application/msword";
                case "TXT":
                    return "text/plain";
                case "PPT":
                    return "application/x-mspowerpoint";
                case "PDF":
                    return "application/pdf";
                case "MPP":
                    return "application/vnd.ms-project";
                case "ACD":
                    return "image/vnd.dwg";
                case "GIF":
                    return "image/jpeg";

            }
        }
    }
}
