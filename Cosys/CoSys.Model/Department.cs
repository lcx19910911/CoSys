
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    [Table("Department")]
    public class Department
    {
        [Key]
        [Required]
        [MaxLength(32)]
        [Column("ID", TypeName = "char")]
        public string ID { get; set; }


        /// <summary>
        /// 值
        /// </summary>
        public long Flag { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [MaxLength(32)]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }

        /// <summary>
        /// 父类Id
        /// </summary>
        [MaxLength(32)]
        [Column("ParentID", TypeName = "char")]
        public string ParentID { get; set; }
        /// <summary>
        /// 父类Id
        /// </summary>
        [NotMapped]
        public string ParentName { get; set; }

        public bool IsDelete { get; set; }
    }
}
