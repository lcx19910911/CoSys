using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoSys.Model;

namespace CoSys.Core.UEditor
{
    public class ListResponseDTO
    {
        public UploadStateCode state { get; set; }

        public int start { get; set; }

        public int total { get; set; }

        public List<ImageUrlDTO> list { get; set; }
    }
}
