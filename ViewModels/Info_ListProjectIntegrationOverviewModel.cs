using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_ListProjectIntegrationOverviewModel
    {
        public string page { get; set; }
        public string project_id { get; set; }
        public string project_name { get; set; }
        public string merge_description { get; set; }
        public string orderby { get; set; }
    }
}
