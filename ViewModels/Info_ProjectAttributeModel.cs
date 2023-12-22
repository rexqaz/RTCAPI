using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_ProjectAttributeModel
    {
        public string project_id { get; set; }
        public string is_demo { get; set; }
    }

    public class Info_ProjectAttributeOverViewModel
    {
        public string page { get; set; } = "1";
        //public string project_name { get; set; }
        ///// <summary>
        ///// 登入帳號
        ///// 須依權限取得專案資料
        ///// </summary>
        //public string account_id { get; set; }
        ///// <summary>
        ///// 排序 
        ///// project_name asc or desc
        ///// name_member asc or desc
        ///// </summary>
        public string orderby { get; set; } = "";
    }

    public class Info_ProjectAttributeList
    {
        public List<Info_ProjectAttributeModel> attributeInput { get; set; }
    }
}
