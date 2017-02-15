using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    [Table("JD_area")]
    public class JD_area
    {
        [Key]
        [Required]
        [MaxLength(32)]
        [Column("ID", TypeName = "char")]
        public string ID { get; set; }

        public string name { get; set; }

        public string parent_id { get; set; }
    }
}
