using System.ComponentModel.DataAnnotations;

namespace QuotesApp.Controllers
{
    public class NewQuoteRequest
    {
        [Required(ErrorMessage = "Please enter the quote content.")]
        public string Content { get; set; }
        public string? Author { get; set;}
    }
}
