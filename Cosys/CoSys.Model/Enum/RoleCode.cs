using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    /// <summary>
    /// 角色类型
    /// </summary>
    public enum RoleCode
    {
        None = 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        Editor = 1,

        /// <summary>
        /// 部长
        /// </summary>
        [Description("部长")]
        Minister = 2,

        /// <summary>
        /// 等等
        /// </summary>
        [Description("等等")]
        SoOn = 3,
    }
}
