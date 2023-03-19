using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Task.BLL.interfaces;
using Task.DAL.Entity;
using Task_.Net.DTO;

namespace Task_.Net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, UserManager")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService tokenService;

        public UserController(UserManager<ApplicationUser> _userManager,ITokenService tokenService)
        {
           this._userManager = _userManager;
            this.tokenService = tokenService;
        }
        [HttpPost]
        public async Task<ActionResult<UserDto>> Post(RegisterDTO model)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                     return BadRequest("Email is Already registered");
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {


                    ModelState.AddModelError(string.Empty, "Invalid Register attempt.");
                    return BadRequest(ModelState);
                }
                await _userManager.AddToRoleAsync(user, "regular");
                var userDto = new UserDto()
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    Token = await tokenService.CreateToken(user, _userManager)
                };

                return Ok(userDto);

            }
            return BadRequest(ModelState);

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> Get()
        {
            var users = await _userManager.Users.Select(U=>new UserDto
            {Email=U.Email,UserName=U.UserName}).ToListAsync();
            return Ok(users);
        }
      
        [HttpGet("{Email}")]
        public async Task<ActionResult<UserDto>> GetUser(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
           UserDto registerDTO = new UserDto()
            {
                Email = user.Email,
                UserName = user.UserName,
              
                
            };

            if (user == null)
            {
                return NotFound();
            }

            return Ok(registerDTO);
        }

        [HttpPut("{Email}")]
        public async Task<IActionResult> UpdateUser(string Email, RegisterDTO model)
        {
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                return NotFound();
            }
            
            user.Email = model.Email;
            user.UserName = model.UserName;
           

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return BadRequest(ModelState);
            }

            return Ok();
        }

      
        [HttpDelete("{Email}")]
        public async Task<IActionResult> DeleteUser(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return BadRequest(ModelState);
            }

            return Ok();
        }


    }
}
