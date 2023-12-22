using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_FormUploadFileModel
    {
        public IFormFile UploadFile { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
    }
}
