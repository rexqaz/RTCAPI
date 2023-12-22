using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_EditProjectIntegrationInfoModel
    {
        public string merge_id { get; set; }
        public string merge_description { get; set; }
        public List<Project_Info> project_info { get; set; }
    }

    public class Project_Info
    { 
        public string project_id { get; set; }
        public string project_name { get; set; }
        public string owner_name { get; set; }
    }
}
