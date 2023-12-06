using Microsoft.AspNetCore.Identity;
using QuotesApp.Controllers;

namespace QuotesApp.Services
{
    public interface IAuthService
    {
        public Task<IdentityResult> RegisterUser(UserRegistrationRequest userRegistrationRequest);

        public Task<bool> LoginUser(LoginRequest LoginRequest);

        public Task<string> CreateToken();
    }
}
