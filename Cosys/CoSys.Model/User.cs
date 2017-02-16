namespace CoSys.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("User")]
    public partial class User: BaseEntity
    {

        /// <summary>
        /// 用户名
        /// </summary>
        [MaxLength(32)]
        [Required]
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(128)]
        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }

        /// <summary>
        /// 请输入密码
        /// </summary>
        [Display(Name = "请输入密码")]
        [MaxLength(12), MinLength(6)]
        [NotMapped]
        public string NewPassword { get; set; }

        /// <summary>
        /// 再次输入密码
        /// </summary>
        [MaxLength(12), MinLength(6), Compare("NewPassword", ErrorMessage = "两次密码输入不一致")]
        [NotMapped]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(32)]
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [MaxLength(32)]
        [Required]
        public string RealName { get; set; }

        /// <summary>
        /// 见刊笔名
        /// </summary>
        [MaxLength(32)]
        [Required]
        public string PenName { get; set; }

        /// <summary>
        /// 户籍所在地
        /// </summary>
        [MaxLength(256)]
        [Required]
        public string IDCardAddres { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        [MaxLength(32)]
        [Required]
        public string IDCard { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        [MaxLength(64)]
        public string CompanyName { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        [MaxLength(32)]
        public string Position { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [MaxLength(32)]
        public string Zipcode { get; set; }
    

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
        public string StreetName { get; set; }

        /// <summary>
        /// 通信地址
        /// </summary>
        [MaxLength(256)]
        [Required]
        public string Addres { get; set; }


        /// <summary>
        /// 固定电话
        /// </summary>
        [MaxLength(11)]
        public string TelePhone { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(11)]
        [Required(ErrorMessage = "手机号不能为空")]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "手机格式不正确")]
        public string Phone { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [MaxLength(32)]
        public string QQ { get; set; }

        public UserCode Code { get; set; }


        [NotMapped]
        public List<SelectItem> ProvinceItems { get; set; }
    }
}
