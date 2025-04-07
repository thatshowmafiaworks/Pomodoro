using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Services;
using TimerApi.DTOs;

namespace TimerApi.Controllers
{
    [Route("api/auth")]
    public class AuthController(
        UserManager<IdentityUser> userMgr,
        RoleManager<IdentityRole> roleMgr,
        SignInManager<IdentityUser> signInMgr,
        ILogger<AuthController> logger,
        IConfiguration config,
        TokenGenerator tokenGenerator
        ) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(new { error = "email or password is empty" });
            var user = await userMgr.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized(new { error = "not found user with this email" });
            var result = await signInMgr.PasswordSignInAsync(user, dto.Password, false, false);
            if (!result.Succeeded)
            {
                logger.LogInformation("User entered wrong password");
                return Unauthorized(new { error = "wrong password" });
            }
            var key = config["Jwt:Key"];
            var issuer = config["Jwt:Issuer"];
            var roles = await userMgr.GetRolesAsync(user);
            var token = tokenGenerator.GenerateToken(key, issuer, user, roles);
            return Ok(new { token = token });
        }

        [AllowAnonymous]
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
            await userMgr.AddToRoleAsync(newUser, "User");
            return Ok();
        }
    }
}
