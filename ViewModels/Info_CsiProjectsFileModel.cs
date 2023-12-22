using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_CsiProjectsFileModel
    {
        public string project_file_id { get; set; }
        public string project_name { get; set; }
        public string link { get; set; }
        public string node_level { get; set; }

    }

    public class Info_CsiProjectsFileQueryModel
    {
        public string project_file_id { get; set; }
        public string project_name { get; set; }
        public string link { get; set; }
        public string user_id { get; set; }

        public string page { get; set; } = "1";

        public string orderby { get; set; } = "";
    }
}
