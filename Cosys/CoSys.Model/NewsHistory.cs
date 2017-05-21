
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
    /// 历史记录
    /// </summary>
    [Table("NewsHistory")]
    public class NewsHistory : BaseEntity
    {
        [Required]
        [MaxLength(32)]
        [Column("NewsID", TypeName = "char")]
        public string NewsID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(64)]
        public string Title { get; set; }

        
        /// <summary>
        /// 标题
        /// </summary>
        public string Content { get; set; }
    }
}
