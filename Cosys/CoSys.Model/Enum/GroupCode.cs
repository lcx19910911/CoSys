using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    /// <summary>
    /// 字典分组
    /// </summary>
    public enum GroupCode
    {
        None=0,
        /// <summary>
        /// 地区
        /// </summary>
        [Description("地区")]
        Area = 1,

        [Description("投稿渠道")]
        Channel = 2,

        [Description("投稿类型")]
        Type = 3,      
    }
}
