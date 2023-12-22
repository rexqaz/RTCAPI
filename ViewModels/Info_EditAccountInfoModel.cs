using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_EditAccountInfoModel
    {
        public string is_reset_password { get; set; }
        public string new_password { get; set; }
        public string confirm_new_password { get; set; }
        public string group { get; set; }
        public string is_employee { get; set; }
        public string employee_id { get; set; }
        public string employee_name { get; set; }
        public string department { get; set; }
        public string note { get; set; }
        public string account { get; set; }
    }
}
