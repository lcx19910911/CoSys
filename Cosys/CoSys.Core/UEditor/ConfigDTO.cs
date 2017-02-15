using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Core.UEditor
{
    public class ConfigDTO
    {
        public string imageActionName { get; set; }
        public string imageFieldName { get; set; }
        public int imageMaxSize { get; set; }
        public string[] imageAllowFiles { get; set; }
        public string imageCompressEnable { get; set; }
        public int imageCompressBorder { get; set; }
        public string imageInsertAlign { get; set; }
        public string imageUrlPrefix { get; set; }
        public string imagePathFormat { get; set; }

        public string scrawlActionName { get; set; }
        public string scrawlFieldName { get; set; }
        public string scrawlPathFormat { get; set; }
        public int scrawlMaxSize { get; set; }
        public string scrawlUrlPrefix { get; set; }
        public string scrawlInsertAlign { get; set; }

        public string snapscreenActionName { get; set; }
        public string snapscreenPathFormat { get; set; }
        public string snapscreenUrlPrefix { get; set; }
        public string snapscreenInsertAlign { get; set; }

        public string[] catcherLocalDomain { get; set; }
        public string catcherActionName { get; set; }
        public string catcherFieldName { get; set; }
        public string catcherPathFormat { get; set; }
        public string catcherUrlPrefix { get; set; }
        public int catcherMaxSize { get; set; }
        public string[] catcherAllowFiles { get; set; }

        public string videoActionName { get; set; }
        public string videoFieldName { get; set; }
        public string videoPathFormat { get; set; }
        public string videoUrlPrefix { get; set; }
        public int videoMaxSize { get; set; }
        public string[] videoAllowFiles { get; set; }

        public string fileActionName { get; set; }
        public string fileFieldName { get; set; }
        public string filePathFormat { get; set; }
        public string fileUrlPrefix { get; set; }
        public int fileMaxSize { get; set; }
        public string[] fileAllowFiles { get; set; }

        public string imageManagerActionName { get; set; }
        public string imageManagerListPath { get; set; }
        public int imageManagerListSize { get; set; }
        public string imageManagerUrlPrefix { get; set; }
        public string imageManagerInsertAlign { get; set; }
        public string[] imageManagerAllowFiles { get; set; }

        public string fileManagerActionName { get; set; }
        public string fileManagerListPath { get; set; }
        public string fileManagerUrlPrefix { get; set; }
        public int fileManagerListSize { get; set; }
        public string[] fileManagerAllowFiles { get; set; }

    }
}
