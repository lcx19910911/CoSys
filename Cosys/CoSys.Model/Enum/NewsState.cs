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
    public enum NewsState
    {
        /// <summary>
        /// 草稿
        /// </summary>
        [Description("草稿")]
        None = 0,
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("审核中")]
        WaitAudit = 1,

        /// <summary>
        /// 终审
        /// </summary>
        [Description("终审")]
        LastAudit =2,

        /// <summary>
        /// 已录用
        /// </summary>
        [Description("已录用")]
        Pass = 3,

        [Description("被退")]
        Reject = 4,

    }
}
