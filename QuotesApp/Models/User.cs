using Microsoft.AspNetCore.Identity;

namespace QuotesApp.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

    }
}
