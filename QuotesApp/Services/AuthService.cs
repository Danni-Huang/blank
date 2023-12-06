using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using QuotesApp.Controllers;
using QuotesApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuotesApp.Services
{
    public class AuthService : IAuthService
    {
        public AuthService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterUser(UserRegistrationRequest userRestrationRequest)
        {
            _user = new User()
            {
                FirstName = userRestrationRequest.FirstName,
                LastName = userRestrationRequest.LastName,
                UserName = userRestrationRequest.UserName,
                Email = userRestrationRequest.Email,
                PhoneNumber = userRestrationRequest.Phonenumber
            };

            var result = await _userManager.CreateAsync(_user, userRestrationRequest.Password);

            if (result.Succeeded)
            {
                result = await _userManager.AddToRolesAsync(_user, userRestrationRequest.Roles);
            }

            return result;
        }

        public async Task<bool> LoginUser(LoginRequest loginRequest)
        {
            _user = await _userManager.FindByNameAsync(loginRequest.UserName);

            if (_user == null)
            {
                return false;
            }
            
            return await _userManager.CheckPasswordAsync(_user, loginRequest.Password);
        }

        public async Task<string> CreateToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var secretKeyText = Environment.GetEnvironmentVariable("SECRET");

            var key = Encoding.UTF8.GetBytes(secretKeyText);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
         {
             new Claim(ClaimTypes.Name, _user.UserName)
         };

            var roles = await _userManager.GetRolesAsync(_user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings["validIssuer"],
                audience: jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                signingCredentials: signingCredentials
            );

            return tokenOptions;
        }

        private IConfiguration _configuration;


        private User? _user;
        private UserManager<User> _userManager;
    }
}
