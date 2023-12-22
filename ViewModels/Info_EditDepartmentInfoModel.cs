using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_EditDepartmentInfoModel
    {
        public string department_id { get; set; }
        public string department_name { get; set; }
        public string is_receive_mail { get; set; }
        public string description { get; set; }
    }
}
