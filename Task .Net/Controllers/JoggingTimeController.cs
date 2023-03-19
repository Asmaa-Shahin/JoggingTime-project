using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

using Task.DAL.Context;
using Task.DAL.Entity;
using Task_.Net.DTO;


namespace Task_.Net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JoggingTimeController : ControllerBase
    {
        private readonly TaskContext context;

        public JoggingTimeController(TaskContext context )
        {
            this.context = context;
        }


      
        [HttpGet("weeklyreport")]
      
        public ActionResult GetWeeklyReport(DateTime startDate)
        {

            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the start and end dates of the week
            DateTime endDate = startDate.AddDays(7);

            // Retrieve all jogging times for the specified week
            var joggingTimes = context.joggingTimes.Where(j => j.UserId == userId && j.Date >= startDate && j.Date < endDate);

            // Calculate the total distance and time for the week
            double totalDistance = joggingTimes.Sum(j => j.Distance);

            var totalTicks = joggingTimes.Select(j => j.Time.Ticks).ToList().Sum();
            TimeSpan totalTime = new TimeSpan(totalTicks);

            // Calculate the average speed for the week
            double averageSpeed = (totalDistance / totalTime.TotalHours);

            // Return the report as an anonymous object
            var report = new
            {
                StartDate = startDate,
                EndDate = endDate,
                AverageDistance = totalDistance / joggingTimes.Count(),
                AverageSpeed = averageSpeed
            };

            return Ok(report);
        }
      
        

        [HttpGet]

        public async Task<ActionResult<IEnumerable<JoggingTimeDto>>> Get(DateTime? from = null, DateTime? to = null)
        {
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            IQueryable<JoggingTime> joggingTimes = context.joggingTimes.Where(j => j.UserId == userId);
            if (from.HasValue)
            {
                joggingTimes = joggingTimes.Where(j => j.Date >= from.Value);
            }
            if (to.HasValue)
            {
                joggingTimes = joggingTimes.Where(j => j.Date <= to.Value);
            }
            var result = await joggingTimes.Select(j => new JoggingTimeDto
            {
                Id = j.Id,
                UserId = j.UserId,
                Date = j.Date,
                Distance = j.Distance,
                Time = j.Time.ToString(@"hh\:mm\:ss")
        }).ToListAsync();

            return Ok(result);

        }



        [HttpGet("{id}")]
        public ActionResult<JoggingTimeDto> Get(int id)
        {
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            JoggingTime joggingTime =context.joggingTimes.Where(j=>j.UserId==userId).FirstOrDefault(j=>j.Id==id);
            if (joggingTime == null)
            {
                return NotFound();
            }
           
            return Ok(new JoggingTimeDto
            {
                Id = joggingTime.Id,
                UserId = joggingTime.UserId,
                Date = joggingTime.Date,
                Distance = joggingTime.Distance,
                Time = joggingTime.Time.ToString(@"hh\:mm\:ss")
        });
        }
        [HttpPost]
        public async Task<ActionResult> Post(JoggingTimeDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ClaimsPrincipal currentUser = HttpContext.User;

            // Get the user's ID
            string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            JoggingTime joggingTime = new JoggingTime
            {
                UserId = userId,
            Date = model.Date,
                Distance = model.Distance,
                Time = TimeSpan.Parse(model.Time),
        };
            
            context.joggingTimes.Add(joggingTime);
         await context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<JoggingTimeDto>> Put(int id, JoggingTimeDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
      JoggingTime joggingTime = context.joggingTimes.Where(j => j.UserId == userId).FirstOrDefault(j => j.Id == id);
            if (joggingTime == null)
            {
                return NotFound();
            }
           

            joggingTime.Date = model.Date;
            joggingTime.Distance = model.Distance;
            joggingTime.Time = TimeSpan.Parse(model.Time);
           await context.SaveChangesAsync();
            return Ok(new JoggingTimeDto
            {
                Id = joggingTime.Id,
                UserId = joggingTime.UserId,
                Date = joggingTime.Date,
                Distance = joggingTime.Distance,
                Time = joggingTime.Time.ToString(@"hh\:mm\:ss")
        });
        }
        [HttpDelete("{id}")]

        public async Task<ActionResult> Delete(int id)
        {
               string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
      JoggingTime joggingTime = context.joggingTimes.Where(j => j.UserId == userId).FirstOrDefault(j => j.Id == id);
           
            if (joggingTime == null)
            {
                return NotFound();
            }
           
            context.joggingTimes.Remove(joggingTime);
          await  context.SaveChangesAsync();
            return Ok();
        }
      
    }
  
}
