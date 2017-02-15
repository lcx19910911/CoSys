using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    /// <summary>
    /// 性别
    /// </summary>
    public enum UserCode
    {
        None = 0,

        /// <summary>
        /// 协会工作人员
        /// </summary>
        [Description("协会工作人员")]
        Staff = 1,

        /// <summary>
        /// 协会小组长
        /// </summary>
        [Description("协会小组长")]
        GroupLeader = 2,
        /// <summary>
        /// 志愿者
        /// </summary>
        [Description("志愿者")]
        Volunteer = 3,

        /// <summary>
        /// 普通会员
        /// </summary>
        [Description("普通会员")]
        OrdinaryMember = 4
    }
}
