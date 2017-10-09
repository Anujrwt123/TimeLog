using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace demoPro.Models
{
    [Table("Developer")]
    public class User
    {
        [Key]
        public int Dev_ID { get; set; }
        public string Developer{ get; set; }
        public string Email { get; set; }
        public string Phone{ get; set; }
        public string Department { get; set; }
        public string Password { get; set; }
    }
}