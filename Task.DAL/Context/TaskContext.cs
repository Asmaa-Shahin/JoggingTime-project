using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.DAL.Entity;

namespace Task.DAL.Context
{
   public class TaskContext:IdentityDbContext<ApplicationUser>
    {
        public DbSet<JoggingTime> joggingTimes { get; set; }
        public TaskContext(DbContextOptions<TaskContext> options) :base(options) { 
        
        }
    }
}
