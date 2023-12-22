using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_FormMailToRuentexModel
    {
        public string GroupName { get; set; }
        public string CommType { get; set; }
        public string Subject { get; set; }
        public string MailBody { get; set; }
        public IFormFile Attach1 { get; set; }
        public IFormFile Attach2 { get; set; }
    }
}
