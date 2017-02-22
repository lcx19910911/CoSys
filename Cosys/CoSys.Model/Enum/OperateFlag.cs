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
        /// 审核
        /// </summary>
        [Description("审核")]
        Audit = 1,

        /// <summary>
        /// 新增管理员
        /// </summary>
        [Description("新增管理员")]
        NewAdmin = 2,

        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        Detial = 4,

        /// <summary>
        /// 发布
        /// </summary>
        [Description("发布")]
        Plush = 8,

        /// <summary>
        /// 修改投稿渠道
        /// </summary>
        [Description("修改投稿渠道")]
        UpdateChannel = 16,

        /// <summary>
        /// 领导
        /// </summary>
        [Description("修改投稿类型")]
        UpdateType = 32,

        /// <summary>
        /// 角色管理
        /// </summary>
        [Description("角色管理")]
        Role = 64,

        /// <summary>
        /// 会员管理
        /// </summary>
        [Description("会员管理")]
        User = 128,

        /// <summary>
        /// 地区管理
        /// </summary>
        [Description("地区管理")]
        Area = 256,

        /// <summary>
        /// 地区统计
        /// </summary>
        [Description("地区统计")]
        AreaStatistics = 512,

        /// <summary>
        /// 渠道统计
        /// </summary>
        [Description("渠道统计")]
        ChannelStatistics = 1024,

        /// <summary>
        /// 渠道统计
        /// </summary>
        [Description("渠道统计")]
        RegisterStatistics = 2048,
    }
}
