using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_EditGroupRightsModel
    {
        public string group_id { get; set; }
        public string function_name { get; set; }
        public string function_id { get; set; }
        public string create { get; set; }
        public string delete { get; set; }
        public string read { get; set; }
        public string update { get; set; }
    }
}
