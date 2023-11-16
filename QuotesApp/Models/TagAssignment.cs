using System.ComponentModel.DataAnnotations;

namespace QuotesApp.Models
{
    public class TagAssignment
    {
        [Key]
        public int QuoteId { get; set; }

        [Key]
        public int TagId { get; set; }

        // navigation properties
        public Quote Quote { get; set; }
        public Tag Tag { get; set; }
    }
}
