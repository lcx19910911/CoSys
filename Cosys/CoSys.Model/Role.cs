﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    [Table("Role")]
    public class Role : BaseEntity
    {

        public NewsAuditState AuditState { get; set; }
        [NotMapped]
        public string AuditStateStr { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [MaxLength(32)]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }

    }
}
