using Microsoft.EntityFrameworkCore;

namespace QuotesApp.Models
{
    public class QuoteContext: DbContext
    {
        public QuoteContext(DbContextOptions<QuoteContext> options) 
            : base(options) { }

        public DbSet<Quote> Quotes { get; set;}
        public DbSet<Like> Like { get; set;}
        public DbSet<Tag> Tags { get; set;}
        public DbSet<User> Users { get; set;}
        public DbSet<TagAssignment> TagAssignments { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quote>().Property(q => q.Content).IsRequired();

            modelBuilder.Entity<Quote>().HasMany(q => q.Likes)
                .WithOne(l => l.Quote)
                .HasForeignKey(l => l.QuoteId);

            modelBuilder.Entity<TagAssignment>().HasKey(ta => new { ta.QuoteId, ta.TagId });

            // configure composite key
            modelBuilder.Entity<TagAssignment>().Property(ta => ta.QuoteId).ValueGeneratedNever();

            modelBuilder.Entity<TagAssignment>()
                .HasOne(ta => ta.Quote)
                .WithMany(q => q.TagAssignments)
                .HasForeignKey(ta => ta.QuoteId);

            modelBuilder.Entity<TagAssignment>()
                .HasOne(ta => ta.Tag)
                .WithMany(t => t.TagAssignments)
                .HasForeignKey(ta => ta.TagId);

            modelBuilder.Entity<Quote>().HasData(
                new Quote
                {
                    QuoteId = 1,
                    Content = "Things work out best for those who make the best of how things work out.",
                    Author = "John Wooden"
                },
                new Quote
                {
                    QuoteId = 2,
                    Content = "If you are not willing to risk the usual you will have to settle for the ordinary.",
                    Author = "Jim Rohn"
                },
                new Quote
                {
                    QuoteId = 3,
                    Content = "All our dreams can come true if we have the courage to pursue them.",
                    Author = "Walt Disney"
                },
                new Quote
                {
                    QuoteId = 4,
                    Content = "Success is walking from failure to failure with no loss of enthusiasm.",
                    Author = "Winston Churchill"
                },
                new Quote
                {
                    QuoteId = 5,
                    Content = "Just when the caterpillar thought the world was ending, he turned into a butterfly.",
                    Author = "Proverb"
                },
                new Quote
                {
                    QuoteId = 6,
                    Content = "Opportunities don't happen, you create them.",
                    Author = "Chris Grosser"
                }
                );
        }

    }
}
