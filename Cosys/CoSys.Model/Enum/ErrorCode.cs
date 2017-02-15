using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    public enum ErrorCode
    {
        #region 系统操作0-99之间
        /// <summary>
        /// 操作成功
        /// </summary>
        [Description("操作成功.")]
        sys_success = 0,

        /// <summary>
        /// 操作失败,请联系管理员
        /// </summary>
        [Description("服务器异常.")]
        sys_fail = 1,

        /// <summary>
        /// 参数值格式有误
        /// </summary>
        [Description("参数值格式有误.")]
        sys_param_format_error = 2,

        /// <summary>
        /// 数据为空
        /// </summary>
        [Description("数据为空.")]
        param_null = 3,

        /// <summary>
        /// 授权码无效
        /// </summary>
        [Description("授权码无效.")]
        sys_token_invalid = 11,

        /// <summary>
        /// 你没有该权限
        /// </summary>
        [Description("你没有该权限.")]
        sys_user_role_error = -100,
        

        #endregion

        #region 数据库操作 100-199
        /// <summary>
        /// 无法找到对应主键Id
        /// </summary>
        [Description("无法找到对应主键Id.")]
        datadatabase_primarykey_not_found = 60,

        /// <summary>
        /// 名称已存在
        /// </summary>
        [Description("名称已存在.")]
        datadatabase_name_had = 61,

        /// <summary>
        /// 身份证号码已存在
        /// </summary>
        [Description("身份证号码已存在.")]
        datadatabase_idcards__had = 62,


        /// <summary>
        /// 手机号码已存在
        /// </summary>
        [Description("手机号码已存在.")]
        datadatabase_mobile__had = 63,


        /// <summary>
        /// 已存在未确认的缴费记录
        /// </summary>
        [Description("已存在未确认的缴费记录.")]
        unconfirm_payorder__had = 64,

        /// <summary>
        /// 不能删除，存在未确认的缴费记录
        /// </summary>
        [Description("不能删除，存在未确认的缴费记录.")]
        cant_delete_unconfirm_payorder__had = 65,


        /// <summary>
        /// 不能申请退学，存在未确认的缴费记录
        /// </summary>
        [Description("不能申请退学，存在未确认的缴费记录.")]
        cant_drop_unconfirm_payorder__had = 66,

        /// <summary>
        /// 制卡时间必须大于报名时间
        /// </summary>
        [Description("制卡时间必须大于报名时间.")]
        make_card_time_error = 67,

        
        #endregion

        #region 业务逻辑
        /// <summary>
        /// 该学员该科目已通过
        /// </summary>
        [Description("该学员该科目已通过.")]
        theme_had_pass =80,

        /// <summary>
        /// 次数已存在
        /// </summary>
        [Description("次数已存在.")]
        count_had_exit = 81,


        /// <summary>
        /// 考试记录缺失
        /// </summary>
        [Description("考试记录缺失.")]
        exam_had_lose = 82,

        /// <summary>
        /// 考试时间必须是本科目上次考试时间之后
        /// </summary>
        [Description("考试时间必须是本科目上次考试时间之后.")]
        exam_timer_error = 83,

        /// <summary>
        /// 该学员未分配该考试科目教练员
        /// </summary>
        [Description("该学员未分配该考试科目教练员.")]
        exam_coache_not_exit = 84,

        /// <summary>
        /// 该学员正在退学中，不能新增考试记录
        /// </summary>
        [Description("该学员正在退学中，不能新增考试记录")]
        student_want_drop = 85,

        /// <summary>
        /// 科目二学时未完成
        /// </summary>
        [Description("科目二学时未完成.")]
        themetwo_timecode_not_complete = 86,

        /// <summary>
        /// 科目三学时未完成
        /// </summary>
        [Description("科目三学时未完成.")]
        themethree_timecode_not_complete = 87,

        /// <summary>
        /// 学员当前是科目二，必须分配教练
        /// </summary>
        [Description("学员当前是科目二，必须分配教练.")]
        themetwo_no_had_coach = 88,

        /// <summary>
        /// 该学员当前不是科目三，不能分配教练
        /// </summary>
        [Description("学员当前是科目三，必须分配教练.")]
        themethree_no_had_coach = 89,

        /// <summary>
        /// 该学员当前不是科目三或科目三，不能分配教练
        /// </summary>
        [Description("该学员当前不是科目三或科目三，必须分配教练.")]
        themecode_no_allow = 90,

        /// <summary>
        /// 学员当前是科目一，不能记录学时
        /// </summary>
        [Description("学员当前是科目一，不能记录学时.")]
        themeont_no_pass = 91,

        /// <summary>
        /// 科目一考试时间必须在制卡时间之后
        /// </summary>
        [Description("科目一考试时间必须在制卡时间之后")]
        theme_one_time_error = 92,
        #endregion




        #region 用户

        /// <summary>
        /// 账号或者密码错误
        /// </summary>
        [Description("账号或者密码错误.")]
        user_login_error = 100,

        /// <summary>
        /// 账号已存在
        /// </summary>
        [Description("账号已存在.")]
        user_name_already_exist = 101,

        /// <summary>
        /// 已过验证时间
        /// </summary>
        [Description("已过验证时间.")]
        verification_time_out = 104,

        /// <summary>
        /// 用户不存在
        /// </summary>
        [Description("用户不存在.")]
        user_not_exit= 105,

        /// <summary>
        /// 账号已过期
        /// </summary>
        [Description("账号已过期")]
        user_expire = 106,

        /// <summary>
        /// 账号已被禁用
        /// </summary>
        [Description("账号已被禁用")]
        user_disabled = 111,

        /// <summary>
        /// 转账凭证不能为空
        /// </summary>
        [Description("转账凭证不能为空")]
        user_payimage_notnull = 107,

        /// <summary>
        /// 公司名称不能为空
        /// </summary>
        [Description("公司名称不能为空")]
        company_name_notnull = 108,

        /// <summary>
        /// 旧密码输入错误
        /// </summary>
        [Description("旧密码输入错误")]
        user_password_nottrue = 109,

        /// <summary>
        /// 两次密码输入不一样
        /// </summary>
        [Description("两次密码输入不一样")]
        user_password_notequal = 110,
        #endregion

    }
}
