namespace QuotesApp.Controllers
{
    public class QuotesResponse
    {
        public int QuoteId { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public int Likes { get; set; }
        public List<string> Tags { get; set; }
    }
}
