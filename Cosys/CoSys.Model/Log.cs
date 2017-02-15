namespace CoSys.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// ѧԱ��Ϣ�༭��¼
    /// </summary>
    [Table("Log")]
    public partial class Log
    {
        [Key]
        [Required]
        [MaxLength(32)]
        [Column("ID", TypeName = "char")]
        public string ID { get; set; }

        /// <summary>
        /// ѧԱid
        /// </summary>
        [Required(ErrorMessage = "ѧԱid����Ϊ��")]
        [Column("StudentID", TypeName = "char"), MaxLength(32)]
        public string StudentID { get; set; }

        /// <summary>
        /// �޸�ǰ����
        /// </summary>
        [Column("BeforeJson", TypeName = "text")]
        public string BeforeJson { get; set; }

        /// <summary>
        /// �޸ĺ�����
        /// </summary>
        [Column("AfterJson", TypeName = "text")]
        public string AfterJson { get; set; }


        /// <summary>
        /// �޸�����
        /// </summary>
        [Column("UpdateInfo", TypeName = "varchar"), MaxLength( 512)]
        public string UpdateInfo { get; set; }


        /// <summary>
        /// ��ע
        /// </summary>
        [Column("Remark", TypeName = "varchar"), MaxLength(256)]
        public string Remark { get; set; }


        public LogCode Code { get; set; }
        /// <summary>
        /// ����������
        /// </summary>
        [NotMapped]
        public string CreaterUserName { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [Column("CreaterID", TypeName = "char"), MaxLength(32)]
        public string CreaterID { get; set; }


        /// <summary>
        /// ����ʱ��
        /// </summary>
        [Display(Name = "����ʱ��")]
        [Required]
        public System.DateTime CreatedTime { get; set; }

    }
}
