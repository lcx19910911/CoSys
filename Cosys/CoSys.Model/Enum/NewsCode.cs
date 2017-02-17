using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    /// <summary>
    /// 新闻类捏
    /// </summary>
    public enum NewsCode
    {
        None = 0,

        /// <summary>
        /// 创业
        /// </summary>
        [Description("创业")]
        Entrepreneurship = 1,

        /// <summary>
        /// 审计
        /// </summary>
        [Description("审计")]
        Audit = 2,

        /// <summary>
        /// 等等
        /// </summary>
        [Description("等等")]
        SoOn = 3,
    }
}
