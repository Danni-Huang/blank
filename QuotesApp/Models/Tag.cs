namespace QuotesApp.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; }

        // navigation property to linking entity
        public ICollection<TagAssignment> TagAssignments { get; set; }
    }
}
