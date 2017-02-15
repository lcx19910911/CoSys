using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Core
{
    public enum UploadStateCode
    {
        [Description("未知错误")]
        Unknown,

        [Description("SUCCESS")]
        Success,

        [Description("文件访问出错，请检查写入权限")]
        SizeLimitExceed,

        [Description("文件大小超出服务器限制")]
        TypeNotAllow,

        [Description("不允许的文件格式")]
        FileAccessError,

        [Description("网络错误")]
        NetworkError,


    }
}
