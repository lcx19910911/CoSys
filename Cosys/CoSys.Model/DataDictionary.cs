using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    [Table("DataDictionary")]
    public class DataDictionary
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Required]
        [MaxLength(32)]
        [Column("ID", TypeName = "char")]
        public string ID { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        [MaxLength(32)]
        [Column("ParentKey", TypeName = "char")]
        public string ParentKey { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        [Required(ErrorMessage = "选择分类")]
        public GroupCode GroupCode { get; set; }

        /// <summary>
        /// 键
        /// </summary>
        [MaxLength(32)]
        [Column("Key", TypeName = "varchar")]
        public string Key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [Required(ErrorMessage = "值不能为空")]
        [MaxLength(32)]
        [Column("Value", TypeName = "varchar")]
        public string Value { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }

        [NotMapped]
        public string ParentName { get; set; }
    }
}
