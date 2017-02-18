using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    /// <summary>
    /// 新闻投递对象部门
    /// </summary>
    public enum NewsForCode
    {
        None = 0,

        /// <summary>
        /// 组织部
        /// </summary>
        [Description("组织部")]
        Organization = 1,

        /// <summary>
        /// 办公室
        /// </summary>
        [Description("办公室")]
        Office = 2,

        /// <summary>
        /// 项目部
        /// </summary>
        [Description("项目部")]
        Project = 3,

        /// <summary>
        /// 协会
        /// </summary>
        [Description("协会")]
        Association = 3,
    }
}
