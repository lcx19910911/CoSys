using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    /// <summary>
    /// 日志状态
    /// </summary>
    public enum LogCode
    {
        None=0,


        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        Create = 1,

        /// <summary>
        /// 修改学员信息
        /// </summary>
        [Description("查看")]
        Look = 2,

        [Description("编辑")]
        Update = 4,

        /// <summary>
        /// 删除学员信息
        /// </summary>
        [Description("退回")]
        AuditFail = 3,


        [Description("审核通过")]
        AuditPass = 5,

        [Description("发布")]
        Plush = 6,
    }
}
