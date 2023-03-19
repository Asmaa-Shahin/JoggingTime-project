using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task.DAL.Entity;

namespace Task_.Net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class RoleController : ControllerBase
    {
        public  readonly UserManager<ApplicationUser> userManager ;
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

      

        [HttpPost]
        public async Task<ActionResult> AddRole(string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
        await  roleManager.CreateAsync(new IdentityRole(role));
            }
            return Ok(role);
            

        }
        [HttpPost("UserToRole")]
     public async Task<ActionResult> AddUserToRoleByEmail(string roleName, string email)
            {
   

              var user = await userManager.FindByEmailAsync(email);

    if (user == null)
    {
        return NotFound();
    }

    var result = await userManager.AddToRoleAsync(user, roleName);

    if (result.Succeeded)
    {
        return Ok(user.Email);
    }
    else
    {
        return BadRequest(result.Errors.FirstOrDefault());
    }
}
    }
}
