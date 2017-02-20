
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
    [Table("News")]
    public class News : BaseEntity
    {

        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(64)]
        public string Title { get; set; }

        /// <summary>
        /// 见刊笔名
        /// </summary>
        [MaxLength(32)]
        [Required]
        public string PenName { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [Required(ErrorMessage = "用户不能为空")]
        [MaxLength(32)]
        [Column("UserID", TypeName = "char")]
        public string UserID { get; set; }

        [NotMapped]
        public string UserName { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Content { get; set; }


        public NewsCode Code { get; set; }

        [NotMapped]
        public string CodeStr { get; set; }

        public NewsState State { get; set; }

        [NotMapped]
        public string StateStr { get; set; }

        [MaxLength(1024)]
        public string Msg { get; set; }

        public long MethodFlag { get; set; }

        [NotMapped]
        public List<Log> Logs { get; set; }

        /// <summary>
        /// 投递审核对象
        /// </summary>
        public NewsForCode ForCode { get; set; }
        
        /// <summary>
        /// 处理角色
        /// </summary>
        public RoleCode AuditRole { get; set; }

        /// <summary>
        /// 审核流程
        /// </summary>
        public long AuditFlag { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [Required(ErrorMessage = "用户不能为空")]
        [MaxLength(32)]
        [Column("NewsTypeID", TypeName = "char")]
        public string NewsTypeID { get; set; }



        /// <summary>
        /// 用户
        /// </summary>
        [Required(ErrorMessage = "用户不能为空")]
        [MaxLength(512)]
        public string NewsDepartmentID { get; set; }
        [NotMapped]
        public string NewsDepartmentName { get; set; }
        [NotMapped]
        public string NewsTypeName { get; set; }

        public string Paths { get; set; }

        [NotMapped]
        public List<SelectItem> TypeList { get; set; }
        [NotMapped]
        public List<SelectItem> DepartmentList { get; set; }


        [NotMapped]
        public List<SelectItem> ChildrenDepartmentList { get; set; }
    }
}
