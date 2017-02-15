using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoSys.Model;

namespace CoSys.Core.UEditor
{
    public class UploadResponseDTO
    {
        public UploadStateCode state { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string original { get; set; }
    }
}
