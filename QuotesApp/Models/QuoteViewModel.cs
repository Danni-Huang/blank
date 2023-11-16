using QuotesApp.Controllers;

namespace QuotesApp.Models
{
    public class QuoteViewModel
    {
        public List<QuotesResponse>? Quotes { get; set; }

        public DateTime? QuotesLastModified { get; set; }

        public List<string>? Tags { get; set; }
    }
}
