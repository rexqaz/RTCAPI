using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_EditCsiPaymentProgressModel
    {
        public string PSPNR { get; set; }
        public string date { get; set; }
        public string target_progress { get; set; }

        public string current_progress { get; set; }
    }
}
