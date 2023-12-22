using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_EditSeasonSatisfactionSurveyModel
    {
        public string PSPNR { get; set; }
        public string account_id { get; set; }
        public List<SurveyClass> Data { get; set; }
    }

    public class SurveyClass
    {
        public string header { get; set; }
        public List<SurveyItem> item { get; set; }
    }

    public class SurveyItem
    { 
        public string name { get; set; }
        public string score { get; set; }
    }
}
