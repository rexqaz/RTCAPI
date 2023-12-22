using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_FormLoginModel
    {
        public string account { get; set; }
        public string password { get; set; }
    }

    public class Info_FormLogoutModel
    {
        public string account { get; set; }
        public string logon { get; set; }
    }
}
