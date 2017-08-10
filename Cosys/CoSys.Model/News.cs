
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
        [MaxLength(32)]
        [Column("UserID", TypeName = "char")]
        public string UserID { get; set; }

        [NotMapped]
        public string UserName { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Content { get; set; }
        

        public NewsAuditState AuditState { get; set; } = NewsAuditState.None;
        /// <summary>
        /// 标题
        /// </summary>
        [NotMapped]
        public string AuditStateStr { get; set; }

        public NewsState State { get; set; }

        [NotMapped]
        public string StateStr { get; set; }
        [NotMapped]
        public string RoleName { get; set; }

        [MaxLength(1024)]
        public string Msg { get; set; }

        public long MethodFlag { get; set; }
        public long PlushMethodFlag { get; set; }

        [NotMapped]
        public List<Log> Logs { get; set; }

        /// <summary>
        /// 审核流程
        /// </summary>
        public long AuditFlag { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [MaxLength(32)]
        [Column("NewsTypeID", TypeName = "char")]
        public string NewsTypeID { get; set; }


        /// <summary>
        /// 用户
        /// </summary>
        [Required]
        [MaxLength(512)]
        public string DepartmentID { get; set; }
        [NotMapped]
        public string DepartmentName { get; set; }
        [NotMapped]
        public string NewsTypeName { get; set; }

        public string Paths { get; set; }

        [NotMapped]
        public List<SelectItem> TypeList { get; set; }


        [NotMapped]
        public List<SelectItem> MethodList { get; set; }

        [NotMapped]
        public List<SelectItem> DepartmentList { get; set; }


        /// <summary>
        /// 当前审核ID
        /// </summary>
        public string UpdateAdminID { get; set; }
        /// <summary>
        /// 当前审核员
        /// </summary>
        [NotMapped]
        public string UpdateAdminName { get; set; }

        [NotMapped]
        public List<SelectItem> ChildrenDepartmentList { get; set; }

        [NotMapped]
        public YesOrNoCode CanAudit { get; set; } = YesOrNoCode.No;

        public DateTime? SubmitTime { get; set; }

        
    }
}
