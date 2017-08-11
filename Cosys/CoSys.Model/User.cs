namespace CoSys.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("User")]
    public partial class User: BaseEntity
    {

        /// <summary>
        /// �û���
        /// </summary>
        [MaxLength(32)]
        [Required]
        public string Account { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [MaxLength(128)]
        [Required(ErrorMessage = "���벻��Ϊ��")]
        public string Password { get; set; }

        /// <summary>
        /// ����������
        /// </summary>
        [Display(Name = "����������")]
        [MaxLength(12, ErrorMessage = "�������Ϊ12"), MinLength(6,ErrorMessage ="��������Ϊ6")]
        [NotMapped]
        public string NewPassword { get; set; }

        /// <summary>
        /// �ٴ���������
        /// </summary>
        [MaxLength(12), MinLength(6), Compare("NewPassword", ErrorMessage = "�����������벻һ��")]
        [NotMapped]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [MaxLength(32)]
        public string Email { get; set; }

        /// <summary>
        /// ��ʵ����
        /// </summary>
        [MaxLength(32)]
        [Required]
        public string RealName { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [MaxLength(32)]
        public string PenName { get; set; }

        /// <summary>
        /// �������ڵ�
        /// </summary>
        [MaxLength(256)]
        public string IDCardAddres { get; set; }

        /// <summary>
        /// ���֤����
        /// </summary>
        [MaxLength(32)]
        public string IDCard { get; set; }

        /// <summary>
        /// ��˾����
        /// </summary>
        [MaxLength(64)]
        public string CompanyName { get; set; }

        /// <summary>
        /// ְλ
        /// </summary>
        [MaxLength(32)]
        public string Position { get; set; }

        /// <summary>
        /// �ʱ�
        /// </summary>
        [MaxLength(32)]
        public string Zipcode { get; set; }
    

        /// <summary>
        /// λ��
        /// </summary>
        [NotMapped]
        public string ProvoniceName { get; set; }

        /// <summary>
        /// λ��
        /// </summary>
        [NotMapped]
        public string CityName { get; set; }

        /// <summary>
        /// λ��
        /// </summary>
        [NotMapped]
        public string CountyName { get; set; }

        /// <summary>
        /// �ֵ�
        /// </summary>
        [NotMapped]
        public string StreetName { get; set; }

        /// <summary>
        /// λ��
        /// </summary>
        [MaxLength(32)]
        public string ProvoniceCode { get; set; }

        /// <summary>
        /// λ��
        /// </summary>
        [MaxLength(32)]
        public string CityCode { get; set; }

        /// <summary>
        /// λ��
        /// </summary>
        [MaxLength(32)]
        public string CountyCode { get; set; }

        /// <summary>
        /// �ֵ�
        /// </summary>
        [MaxLength(32)]
        public string StreetCode { get; set; }


        /// <summary>
        /// ͨ�ŵ�ַ
        /// </summary>
        [MaxLength(256)]
        public string Addres { get; set; }


        /// <summary>
        /// �̶��绰
        /// </summary>
        [MaxLength(11)]
        public string TelePhone { get; set; }

        /// <summary>
        /// �ֻ���
        /// </summary>
        [MaxLength(11)]
        [Required(ErrorMessage = "�ֻ��Ų���Ϊ��")]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "�ֻ���ʽ����ȷ")]
        public string Phone { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [MaxLength(32)]
        public string QQ { get; set; }

        public UserCode Code { get; set; }


        [NotMapped]
        public List<SelectItem> ProvinceItems { get; set; }
        [NotMapped]
        public List<SelectItem> CityItems { get; set; }
        [NotMapped]
        public List<SelectItem> CountyItems { get; set; }
        [NotMapped]
        public List<SelectItem> StreetItems { get; set; }


        [NotMapped]
        public string ValiteCode { get; set; }



        /// <summary>
        /// ����Ȩ��
        /// </summary>
        public long DepartmentFlag { get; set; }

        /// <summary>
        /// Ȩ��
        /// </summary>
        public long OperateFlag { get; set; }
        /// <summary>
        /// ��ɫ
        /// </summary>
        [Column("RoleID", TypeName = "char"), MaxLength(32)]
        public string RoleID { get; set; }
        /// <summary>
        /// ״̬
        /// </summary>
        [NotMapped]
        public string RoleName { get; set; }
        /// <summary>
        /// �Ƿ񳬼�����Ա
        /// </summary>
        public bool IsSuperAdmin { get; set; }
        /// <summary>
        /// �Ƿ����Ա
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// �����
        /// </summary>
        public int AuditCount { get; set; }
        /// <summary>
        /// �޸���
        /// </summary>
        public int EditCount { get; set; }
        /// <summary>
        /// ���ͨ����
        /// </summary>
        public int AuditPassCount { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public int PlushCount { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [NotMapped]
        public int AllCount { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [NotMapped]
        public int PassCount { get; set; }
    }
}
