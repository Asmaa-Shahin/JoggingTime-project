using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Task.DAL.Context;
using Task.DAL.Entity;
using Task_.Net.DTO;

namespace Task_.Net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class JoggingTimeAllUsers : ControllerBase
    {
        private readonly TaskContext context;

        public JoggingTimeAllUsers(TaskContext context)
        {
            this.context = context;
        }
        [HttpGet]

        public async Task<ActionResult<IEnumerable<JoggingTimeDto>>> Get()
        {

            IQueryable<JoggingTime> joggingTimes = context.joggingTimes;
          
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
            JoggingTime joggingTime = context.joggingTimes.Find(id);
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
        

            // Get the user's ID
          

            JoggingTime joggingTime = new JoggingTime
            {
                UserId = model.UserId,
                Date = model.Date,
                Distance = model.Distance,
                Time = TimeSpan.Parse(model.Time),
            };

            context.joggingTimes.Add(joggingTime);
            await context.SaveChangesAsync();
            return Ok(joggingTime);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<JoggingTimeDto>> Put(int id, JoggingTimeDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            JoggingTime joggingTime = context.joggingTimes.Find(id);
            if (joggingTime == null)
            {
                return NotFound();
            }


            joggingTime.Date = model.Date;
            joggingTime.Distance = model.Distance;
            joggingTime.Time = TimeSpan.Parse(model.Time);
            joggingTime.UserId= model.UserId;
            await context.SaveChangesAsync();
            return Ok(new JoggingTimeDto
            {
                Id = joggingTime.Id,
                UserId = joggingTime.UserId,
                Date = joggingTime.Date,
                Distance = joggingTime.Distance,
                Time = joggingTime.Time.ToString(@"hh\:mm\:ss"),
               
                
            });
        }
        [HttpDelete("{id}")]

        public async Task<ActionResult> Delete(int id)
        {
            JoggingTime joggingTime = context.joggingTimes.Find(id);
            if (joggingTime == null)
            {
                return NotFound();
            }

            context.joggingTimes.Remove(joggingTime);
            await context.SaveChangesAsync();
            return Ok();
        }

    }

}

