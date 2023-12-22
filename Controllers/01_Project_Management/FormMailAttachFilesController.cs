using LTCH_API.Controllers;
using Microsoft.AspNetCore.Hosting;
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
    public class FormMailAttachFilesController : ControllerBase
    {
        [HttpPost]
        public object FormAll([FromForm]Info_FormMailToRuentexModel Data)
        {
            int fileCount = 0;
            var size1 = Data.Attach1.Length;
            Guid guid = Guid.NewGuid();
            string folder = "XingUpdateFile\\" + guid.ToString();
            System.IO.Directory.CreateDirectory(folder);
            if (size1 > 0)
            {                
                var path = folder + "\\" + Data.Attach1.FileName;
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Data.Attach1.CopyToAsync(stream);
                }
                fileCount++;
            }

            var size2 = Data.Attach2.Length;
            if (size2 > 0)
            {
                var path = folder + "\\" + Data.Attach2.FileName;
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Data.Attach2.CopyToAsync(stream);
                }
                fileCount++;
            }

            try
            {
                return new
                {
                    Result = "T",
                    Message = "成功",
                    Count = fileCount,
                    MaileId = guid 
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[FormMailAttachFilesController]99：" + ex.ToString());

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
