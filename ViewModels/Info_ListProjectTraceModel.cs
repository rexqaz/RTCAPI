using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_ListProjectTraceModel
    {
        public string PSPNR { get; set; }
        public string type { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string page { get; set; }
    }
}
