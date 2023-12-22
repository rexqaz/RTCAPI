using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.ViewModels
{
    public class Info_ListProjectDocPicModel
    {
        public string page { get; set; }
        public string yearS { get; set; }
        public string monthS { get; set; }
        public string weekS { get; set; }
        public string yearE { get; set; }
        public string monthE { get; set; }
        public string weekE { get; set; }
        public string doc_type { get; set; }
        public string PSPNR { get; set; }
    }
}
