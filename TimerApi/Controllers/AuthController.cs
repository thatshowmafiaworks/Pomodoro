using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TimerApi.DTOs;

namespace TimerApi.Controllers
{
    [Route("api/auth")]
    public class AuthController(
        UserManager<IdentityUser> userMgr,
        RoleManager<IdentityRole> roleMgr,
        ILogger<AuthController> logger) : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserDto dto)
        {
            return null;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(new { error = "email or password is empty" });
            var user = await userMgr.FindByEmailAsync(dto.Email);
            if (user != null) return BadRequest(new { error = $"this email is alredy used" });
            var newUser = new IdentityUser
            {
                UserName = dto.Email,
                Email = dto.Email,
            };
            var result = await userMgr.CreateAsync(newUser, dto.Password);
            if (!result.Succeeded)
            {
                logger.LogError($"cant create user with email:{dto.Email} and password: {dto.Password}");
                return BadRequest(new { error = $"something went wrong" });
            }
            return Ok();
        }
    }
}
