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
        /// 部门权限
        /// </summary>
        public long DepartmentFlag { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public long OperateFlag { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        [Required(ErrorMessage = "角色不能为空")]
        [Column("RoleID", TypeName = "char"), MaxLength(32)]
        public string RoleID { get; set; }
        [NotMapped]
        public Role Role { get; set; }
        [NotMapped]
        public string CodeStr { get; set; }
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
        /// 位置
        /// </summary>
        [NotMapped]
        public string ProvoniceName { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        [NotMapped]
        public string CityName { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        [NotMapped]
        public string CountyName { get; set; }

        /// <summary>
        /// 街道
        /// </summary>
        [NotMapped]
        public string StreetName { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        [MaxLength(32)]
        public string ProvoniceCode { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        [MaxLength(32)]
        public string CityCode { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        [MaxLength(32)]
        public string CountyCode { get; set; }

        /// <summary>
        /// 街道
        /// </summary>
        [MaxLength(32)]
        public string StreetCode { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        [NotMapped]
        public string State { get; set; }

        /// <summary>
        /// 审核数
        /// </summary>
        public int AuditCount { get; set; }
        /// <summary>
        /// 修稿数
        /// </summary>
        public int EditCount { get; set; }
        /// <summary>
        /// 审核通过数
        /// </summary>
        public int AuditPassCount { get; set; }
        /// <summary>
        /// 发布数
        /// </summary>
        public int PlushCount { get; set; }
    }
}
