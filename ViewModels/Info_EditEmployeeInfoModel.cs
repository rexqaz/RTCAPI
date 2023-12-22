using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_EditEmployeeInfoModel
    {
        public string employee_id { get; set; }
        public string employee_name_c { get; set; }
        public string employee_name_e { get; set; }
        public string employee_department { get; set; }
        public string employee_tel { get; set; }
        public string employee_mail { get; set; }
        public string is_receive_mail { get; set; }
        public string description { get; set; }
    }
}
