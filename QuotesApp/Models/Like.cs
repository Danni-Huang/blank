namespace QuotesApp.Models
{
    public class Like
    {
        public int LikeId { get; set; }
        public int UserId { get; set; }
        public int QuoteId { get; set; }

        // navigation property
        public Quote Quote { get; set; }
    }
}
