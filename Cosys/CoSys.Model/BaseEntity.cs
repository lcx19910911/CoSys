using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    //基础实体
    public class BaseEntity
    {
        [Key]
        [Required]
        [MaxLength(32)]
        [Column("ID", TypeName = "char")]
        public string ID { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [Required]
        public System.DateTime CreatedTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
