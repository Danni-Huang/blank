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

        private IAuthService _authService;
    }
}
