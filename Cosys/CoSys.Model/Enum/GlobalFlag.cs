using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    /// <summary>
    /// 全局状态
    /// </summary>
    [Flags]
    public enum GlobalFlag
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 0,

        /// <summary>
        /// 已删除
        /// </summary>
        [Description("已删除")]
        Removed = 1,

        /// <summary>
        /// 已禁用
        /// </summary>
        [Description("已禁用")]
        Unabled =2,

    }
}
