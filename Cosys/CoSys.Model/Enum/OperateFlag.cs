using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    /// <summary>
    /// 权限管理
    /// </summary>
    public enum OperateFlag
    {
        None = 0,
        /// <summary>
        /// 稿件查看
        /// </summary>
        [Description("稿件查看")]
        Detial = 1,
        /// <summary>
        /// 稿件编辑
        /// </summary>
        [Description("稿件编辑")]
        Edit = 2,
        /// <summary>
        /// 稿件审核
        /// </summary>
        [Description("稿件审核")]
        Audit = 4,
        /// <summary>
        /// 发布
        /// </summary>
        [Description("稿件发布")]
        Plush = 8,
    }
}
