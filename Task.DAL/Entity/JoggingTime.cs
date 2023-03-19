using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.DAL.Entity
{
    public class JoggingTime
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Distance { get; set; }
        public TimeSpan Time { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }

}
