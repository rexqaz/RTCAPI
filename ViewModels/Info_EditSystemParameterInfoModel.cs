using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_EditSystemParameterInfoModel
    {
        public string project_id { get; set; }
        public string project_name { get; set; }
        public string ManagerPercent { get; set; }
        public List<CCTV_setting> CCTV_setting { get; set; }
    }

    public class CCTV_setting
    { 
        public string CCTV_URL { get; set; }
        public string description { get; set; }
    }
}
