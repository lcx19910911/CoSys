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
        [MaxLength(12), MinLength(6)]
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
        [Required]
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
        [Required]
        public string PenName { get; set; }

        /// <summary>
        /// �������ڵ�
        /// </summary>
        [MaxLength(256)]
        [Required]
        public string IDCardAddres { get; set; }

        /// <summary>
        /// ���֤����
        /// </summary>
        [MaxLength(32)]
        [Required]
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
        public string StreetName { get; set; }

        /// <summary>
        /// ͨ�ŵ�ַ
        /// </summary>
        [MaxLength(256)]
        [Required]
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
    }
}
