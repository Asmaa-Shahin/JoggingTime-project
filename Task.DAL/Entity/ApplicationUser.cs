using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.DAL.Entity
{public class ApplicationUser:IdentityUser
    {
       
            public ICollection<JoggingTime> JoggingTimes { get; set; }
        


    }
}
