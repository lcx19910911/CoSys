
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
    /// 站内通知
    /// </summary>
    [Table("SiteMessage")]
    public class SiteMessage : BaseEntity
    {

        /// <summary>
        /// 消息
        /// </summary>
        [Required(ErrorMessage = "消息不能为空")]
        [Column("Message", TypeName = "text")]
        public string Message { get; set; }
    }
}
