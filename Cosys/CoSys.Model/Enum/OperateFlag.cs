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
        /// 稿件审核
        /// </summary>
        [Description("审核")]
        Audit = 1,
        /// <summary>
        /// 发布
        /// </summary>
        [Description("发布")]
        Plush = 2,


        /// <summary>
        /// 稿件查看
        /// </summary>
        [Description("查看")]
        Detial = 4,


        /// <summary>
        /// 用户管理
        /// </summary>
        [Description("用户管理")]
        Admin = 8,

        /// <summary>
        /// 参数设置
        /// </summary>
        [Description("参数设置")]
        Config = 16,
        

        /// <summary>
        /// 统计
        /// </summary>
        [Description("统计")]
        Statistics = 32,
    }
}
