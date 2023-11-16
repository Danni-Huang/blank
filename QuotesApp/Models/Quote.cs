using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace QuotesApp.Models
{
    public class Quote
    {
        public int QuoteId { get; set; }

        [Required(ErrorMessage = "Please enter the quote content.")]
        public string Content { get; set; }
        public string? Author { get; set; }

        public DateTime? LastModified { get; set; } = DateTime.Now;

        [JsonIgnore]
        public ICollection<Like>? Likes { get; set; }

        [JsonIgnore]
        // navigation property to linking entity
        public ICollection<TagAssignment>? TagAssignments { get; set; }
    }
}
