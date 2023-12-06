using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuotesApp.Models;
using QuotesApp.Services;

namespace QuotesApp.Controllers
{
    [ApiController()]
    public class AccountApiController : Controller
    {
        public AccountApiController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("/api/register")]
        public async Task<IActionResult> RegisterUser(UserRegistrationRequest userRegistrationRequest)
        {
            var result = await _authService.RegisterUser(userRegistrationRequest);
            
            if (result.Succeeded)
            {
                return StatusCode(201);
            }
            else
            {
                foreach(var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return BadRequest(ModelState);
            }
        }

        [HttpPost("/api/login")]
        public async Task<IActionResult> LoginUser(LoginRequest loginRequest)
        {
            bool loginSuccess = await _authService.LoginUser(loginRequest);

            if (loginSuccess)
            {
                return Ok(new { Token = await _authService.CreateToken() });
            }
            else
            {
                return Unauthorized();
            }

        }

        private IAuthService _authService;
    }
}
