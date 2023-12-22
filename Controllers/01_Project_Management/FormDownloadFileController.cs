using LTCH_API.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rc_interface_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.Controllers._01_Project_Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormDownloadFileController : ControllerBase
    {
        private readonly IWebHostEnvironment hostingEnvironment1;
        public FormDownloadFileController(IWebHostEnvironment hostingEnvironment)
        {
            hostingEnvironment1 = hostingEnvironment;
        }

        [HttpGet]
        public object FormAll(string FilePath, string ContentType)
        {
            string InputIsok = "Y";
            string ReturnErr = "";

            if (!string.IsNullOrEmpty(FilePath))
            {
                FilePath = APCommonFun.CDBNulltrim(FilePath);
                FilePath = System.Net.WebUtility.UrlDecode(FilePath);

                if (!FilePath.StartsWith("\\\\XingUpdateFile\\\\CostFile\\\\5\\\\7"))
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-請確認FilePath 欄位資料正確";
                }
            }

            if (!string.IsNullOrEmpty(ContentType))
            {
                ContentType = APCommonFun.CDBNulltrim(ContentType);
            }

            //第一步 : 先判斷有沒有必填未填寫，
            if (FilePath == "" || ContentType == "") //必填                       
            {
                if (FilePath == "")
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-FilePath 為必填欄位";
                }
                else
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-ContentType 為必填欄位";
                }
            }

            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[FormDownloadFileController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    FilePath = ""
                };
            }
            try
            {
                return PhysicalFile(hostingEnvironment1.ContentRootPath + FilePath, "application/" + ContentType);
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[FormDownloadFileController]99：" + ex.ToString());

                return new
                {
                    Result = "F",
                    Message = ex.ToString(),
                    FilePath = ""
                };
            }

        }
    }
}
