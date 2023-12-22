using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rc_interface_API.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.Controllers._01_Project_Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormUploadFileController : ControllerBase
    {
        [HttpPost]
        public object FormAll([FromForm] Info_FormUploadFileModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            int fileCount = 0;
            var size1 = Data.UploadFile.Length;
            DateTime today = DateTime.Now;

            string ObjId = "111111" + '_' + today.Year.ToString() + today.Month.ToString().PadLeft(2, '0');

            //建立目錄方式統一如下!
            string sFilePath = "XingUpdateFile\\CostFile\\5\\7\\" + ObjId + "\\";

            if (!Directory.Exists(sFilePath))
            {
                System.IO.Directory.CreateDirectory(sFilePath);
            }

            if (size1 > 0)
            {
                var path = sFilePath + "\\" + Data.UploadFile.FileName;
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Data.UploadFile.CopyTo(stream);
                }
                fileCount++;
            }

            //第一步 : 先判斷有沒有必填未填寫，
            if (fileCount == 0 ) //必填                       
            {
                InputIsok = "N";
                ReturnErr = "執行動作錯誤-請上傳檔案";
            }

            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[FormUploadFileController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            try
            {
                return new
                {
                    Result = "T",
                    Message = "成功",
                    ContentType = Data.UploadFile.ContentType.Split('/')[1],
                    FilePath = "\\XingUpdateFile\\CostFile\\5\\7\\" + ObjId + "\\" + Data.UploadFile.FileName
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[FormUploadFileController]99：" + ex.ToString());
                APCommonFun.Error("[FormUploadFileController]99：" + ex.ToString());

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
