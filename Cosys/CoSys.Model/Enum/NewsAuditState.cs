using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    /// <summary>
    /// 审核流程
    /// </summary>
    public enum NewsAuditState
    {

        None=0,

        /// <summary>
        /// 编辑审核
        /// </summary>
        [Description("编辑审核")]
        EditorAudit = 1,

        /// <summary>
        /// 部长
        /// </summary>
        [Description("部长")]
        MinisterAudit = 2,

        /// <summary>
        /// 编委会审核
        /// </summary>
        [Description("编委会审核")]
        EditorialAudit = 3,

        /// <summary>
        /// 稿件审核员审核
        /// </summary>
        [Description("稿件审核员/协会领导审核")]
        LastAudit = 4,

    }
}
