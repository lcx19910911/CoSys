namespace CoSys.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Admin")]
    public partial class Admin: BaseEntity
    {
        /// <summary>
        /// ��ɫ
        /// </summary>
        [Required(ErrorMessage = "��ɫ����Ϊ��")]
        [Column("RoleID", TypeName = "char"), MaxLength(32)]
        public string RoleID { get; set; }

        /// <summary>
        /// ״̬
        /// </summary>
        [NotMapped]
        public string RoleName { get; set; }

        /// <summary>
        /// �Ƿ����Ա
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        /// <summary>
        /// ����Ȩ��
        /// </summary>
        public long OperateFlag { get; set; } 

        /// <summary>
        /// �˺�
        /// </summary>
        [Display(Name = "�˺�")]
        [MaxLength(12)]
        [Required(ErrorMessage ="��½�˺Ų���Ϊ��")]
        [Column("Account", TypeName = "varchar")]
        public string Account { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Display(Name = "����")]
        [MaxLength(32)]
        [Required(ErrorMessage = "���Ʋ���Ϊ��")]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Display(Name = "����")]
        [MaxLength(128)]
        [Required(ErrorMessage = "���벻��Ϊ��")]
        [Column("Password", TypeName = "varchar")]
        public string Password { get; set; }
        /// <summary>
        /// �ֻ���
        /// </summary>
        [Display(Name = "�ֻ���")]
        [MaxLength(11)]
        [Required(ErrorMessage = "�ֻ��Ų���Ϊ��")]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "�ֻ���ʽ����ȷ")]
        public string Mobile { get; set; }
        /// <summary>
        /// ��ע
        /// </summary>
        [Display(Name = "��ע")]
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }

        /// <summary>
        /// ����������
        /// </summary>
        [Display(Name = "����������")]
        [MaxLength(12),MinLength(6)]
        [NotMapped]
        public string NewPassword { get; set; }

        /// <summary>
        /// �ٴ���������
        /// </summary>
        [Display(Name = "�ٴ���������")]
        [MaxLength(12), MinLength(6),Compare("NewPassword",ErrorMessage="�����������벻һ��")]
        [NotMapped]
        public string ConfirmPassword { get; set; }
       

        /// <summary>
        /// ״̬
        /// </summary>
        [NotMapped]
        public string State { get; set; }
    }
}
