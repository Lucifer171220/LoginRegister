using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaWeb.Dtos;
using SocialMediaWeb.Services.Interfaces;

namespace SocialMediaWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserRegistrationDto registrationDto)
        {
            try
            {
                await _userService.RegisterAsync(registrationDto);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto authenticationDto)
        {
            try
            {
                var userResponseDto = await _userService.LoginAsync(authenticationDto);
                return Ok(userResponseDto);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
