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
        /// 新增学员信息
        /// </summary>
        [Description("新增学员信息")]
        AddStudent = 1,

        /// <summary>
        /// 修改学员信息
        /// </summary>
        [Description("编辑学员信息")]
        UpdateStudent = 2,

        /// <summary>
        /// 删除学员信息
        /// </summary>
        [Description("删除学员信息")]
        DeleteStudent = 3,

        [Description("新增缴费记录")]
        AddPayOrder = 4,

        [Description("确认缴费记录")]
        ConfirmPayOrder = 5,


        [Description("修改缴费记录")]
        UpdatePayOrder = 6,

        [Description("删除缴费记录")]
        DeletePayOrder = 7,

        /// <summary>
        /// 新增学员信息
        /// </summary>
        [Description("新增考试记录")]
        AddExam = 8,

        [Description("删除考试记录")]
        DeleteExam = 9,


        [Description("删除退学申请记录")]
        DeleteDropPayOrder = 10,
    }
}
