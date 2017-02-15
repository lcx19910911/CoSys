namespace CoSys.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Admin")]
    public partial class Admin: BaseEntity
    {
        /// <summary>
        /// 角色
        /// </summary>
        [Required(ErrorMessage = "角色不能为空")]
        [Column("RoleID", TypeName = "char"), MaxLength(32)]
        public string RoleID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [NotMapped]
        public string RoleName { get; set; }

        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        /// <summary>
        /// 操作权限
        /// </summary>
        public long OperateFlag { get; set; } 

        /// <summary>
        /// 账号
        /// </summary>
        [Display(Name = "账号")]
        [MaxLength(12)]
        [Required(ErrorMessage ="登陆账号不能为空")]
        [Column("Account", TypeName = "varchar")]
        public string Account { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        [MaxLength(32)]
        [Required(ErrorMessage = "名称不能为空")]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码")]
        [MaxLength(128)]
        [Required(ErrorMessage = "密码不能为空")]
        [Column("Password", TypeName = "varchar")]
        public string Password { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "手机号")]
        [MaxLength(11)]
        [Required(ErrorMessage = "手机号不能为空")]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "手机格式不正确")]
        public string Mobile { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }

        /// <summary>
        /// 请输入密码
        /// </summary>
        [Display(Name = "请输入密码")]
        [MaxLength(12),MinLength(6)]
        [NotMapped]
        public string NewPassword { get; set; }

        /// <summary>
        /// 再次输入密码
        /// </summary>
        [Display(Name = "再次输入密码")]
        [MaxLength(12), MinLength(6),Compare("NewPassword",ErrorMessage="两次密码输入不一致")]
        [NotMapped]
        public string ConfirmPassword { get; set; }
       

        /// <summary>
        /// 状态
        /// </summary>
        [NotMapped]
        public string State { get; set; }
    }
}
