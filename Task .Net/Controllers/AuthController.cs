using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task.BLL.interfaces;
using Task.DAL.Entity;
using Task_.Net.DTO;

namespace Task_.Net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService tokenService;

        public AuthController(SignInManager<ApplicationUser> signInManager,UserManager<ApplicationUser> _UserManager ,ITokenService _tokenService)
        {
            _signInManager = signInManager;
            this._userManager = _UserManager;
            tokenService = _tokenService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDTO model)
        {
            if (ModelState.IsValid)
            {
                if (CheckEmailExistsAsync(model.Email).Result.Value)
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


        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password,model.RememberMe ,false);
                    if (result.Succeeded)
                    {
                        var LoginDto = new UserDto()
                        {
                            Email = user.Email,
                            UserName=user.UserName,
                            Token = await tokenService.CreateToken(user, _userManager)
                        };

                        return Ok(LoginDto);
                    }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        }
                    

                }

            }
        
            return BadRequest(ModelState);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}
