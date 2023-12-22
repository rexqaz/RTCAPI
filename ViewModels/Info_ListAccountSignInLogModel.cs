using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_ListAccountSignInLogModel
    {
        public string page { get; set; }
        public string account { get; set; }
        public string group { get; set; }
        public string login_date_from { get; set; }
        public string login_date_end { get; set; }
        public string logout_date_from { get; set; }
        public string logout_date_end { get; set; }

    }
}
