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
        /// "
        /// </summary>
        [Description("主编")]
        Editor = 1,

        /// <summary>
        /// 主任
        /// </summary>
        [Description("部长")]
        Minister = 2,

        /// <summary>
        /// 主任
        /// </summary>
        [Description("编委会")]
        Editorial = 3,
        
        /// <summary>
        /// 审核员
        /// </summary>
        [Description("审核员")]
        Assessor = 4,

        /// <summary>
        /// 领导
        /// </summary>
        [Description("领导")]
        Leader = 5,
    }
}
