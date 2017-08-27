
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{

    /// <summary>
    /// 稿件文件
    /// </summary>
    [Table("NewsFile")]
    public class NewsFile : BaseEntity
    {
        [Required]
        [MaxLength(32)]
        [Column("NewsID", TypeName = "char")]
        public string NewsID { get; set; }

       


        [Required]
        [MaxLength(32)]
        [Column("UserID", TypeName = "char")]
        public string UserID { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [MaxLength(64)]
        public string Name { get; set; }

        
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        [NotMapped]
        public string NewsName { get; set; }
        [NotMapped]
        public string UserName { get; set; }

    }
}
