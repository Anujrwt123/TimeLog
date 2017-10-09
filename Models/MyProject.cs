﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace demoPro.Models
{
    [Table("DeveloperProject")]
    public class MyProject
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public int Dev_Id{ get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public TimeSpan? TimeTaken { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public TimeSpan? StartTime{ get; set; }
        public TimeSpan? EndTime { get; set; }
        public string Active { get; set; }
        public string AdjustTime { get; set; }
    }
}