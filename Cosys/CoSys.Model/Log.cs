namespace CoSys.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 学员信息编辑记录
    /// </summary>
    [Table("Log")]
    public partial class Log
    {
        [Key]
        [Required]
        [MaxLength(32)]
        [Column("ID", TypeName = "char")]
        public string ID { get; set; }

        /// <summary>
        /// 新闻id
        /// </summary>
        [Required]
        [Column("NewsID", TypeName = "char"), MaxLength(32)]
        public string NewsID { get; set; }

        /// <summary>
        /// 修改前数据
        /// </summary>
        [Column("BeforeJson", TypeName = "text")]
        public string BeforeJson { get; set; }

        /// <summary>
        /// 修改后数据
        /// </summary>
        [Column("AfterJson", TypeName = "text")]
        public string AfterJson { get; set; }


        /// <summary>
        /// 修改详情
        /// </summary>
        [Column("UpdateInfo", TypeName = "varchar"), MaxLength( 512)]
        public string UpdateInfo { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        [Column("Remark", TypeName = "varchar"), MaxLength(256)]
        public string Remark { get; set; }


        public LogCode Code { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        [NotMapped]
        public string AdminName { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Column("CreaterID", TypeName = "char"), MaxLength(32)]
        public string AdminID { get; set; }


        public RoleCode RoleCode { get; set; }

        [NotMapped]
        public RoleCode RoleCodeStr { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [Required]
        public System.DateTime CreatedTime { get; set; }

    }
}
