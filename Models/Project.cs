using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace demoPro.Models
{
    [Table("tblLeads")]
    public class Project
    {
        [Key]
        public int Lead_ID { get; set; }
        public string Project_Desc { get; set; }
        public string Consultant { get; set; }
        public string Customer { get; set; }
        public string Company { get; set; }
        public string Type{ get; set; }
        public string Status { get; set; }
        public string Application { get; set; }


        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public string Department { get; set; }
        [NotMapped]
        public bool Active { get; set; }
    }
}