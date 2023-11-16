namespace QuotesApp.Models
{
    // a view model that represents the info needed for the api "Home page"
    public class QuoteApiViewModel
    {
        // collection of links the api home page offers clients
        public IDictionary<string, Link>? Links { get; set; }
        public string? Version { get; set; }
        public string? Creator { get; set; }

    }
}
