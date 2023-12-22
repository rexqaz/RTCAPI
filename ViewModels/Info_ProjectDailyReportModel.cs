using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_ProjectDailyReportModel
    {
        public string year { get; set; }
        public string month { get; set; }
        public string day { get; set; }
        public string project_id { get; set; }
        public string account_id { get; set; }
    }
}
