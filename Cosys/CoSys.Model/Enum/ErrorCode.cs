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
        /// 地区编码已存在
        /// </summary>
        [Description("地区编码已存在.")]
        areacode_had = 64,


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
        /// 验证码过期
        /// </summary>
        [Description("验证码过期.")]
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
