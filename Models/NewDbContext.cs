using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using demoPro.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace demoPro
{
   
    public class NewDbContext : DbContext
    {
        public NewDbContext(): base("DefaultConnection")
        {}

        public DbSet<Project> Projects { get; set; }
        public DbSet<MyProject> MyProjects { get; set; }
        public DbSet<User> Users { get; set; }

    }
}