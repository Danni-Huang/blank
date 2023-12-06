using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuotesApp.Models.Configuration;

namespace QuotesApp.Models
{
    public class QuoteContext : IdentityDbContext<User>
    {
        public QuoteContext(DbContextOptions<QuoteContext> options)
            : base(options) { }

        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Like> Like { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TagAssignment> TagAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // call base class version to init Identity tables:
            base.OnModelCreating(modelBuilder);

            // apply our custom role configuration:
            modelBuilder.ApplyConfiguration(new RoleConfiguration());

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
                },
                new Quote
                {
                    QuoteId = 7,
                    Content = "If you're going through hell keep going.",
                    Author = "Winston Churchill"
                },
                new Quote
                {
                    QuoteId = 8,
                    Content = "Don't raise your voice, improve your argument.",
                    Author = "Anonymous"
                },
                new Quote
                {
                    QuoteId = 9,
                    Content = "Do one thing every day that scares you.",
                    Author = "Anonymous"
                },
                new Quote
                {
                    QuoteId = 10,
                    Content = "Life is not about finding yourself. Life is about creating yourself.",
                    Author = "Lolly Daskal"
                }
                );

            modelBuilder.Entity<Tag>().HasData(
                new Tag { TagId = 1, Name = "Funny" },
                new Tag { TagId = 2, Name = "Philosophical" },
                new Tag { TagId = 3, Name = "Serious" },
                new Tag { TagId = 4, Name = "Educational" },
                new Tag { TagId = 5, Name = "Motivational" }
                );

            modelBuilder.Entity<Like>().HasData(
                new Like { LikeId = 1, UserId = 123, QuoteId = 8 },
                new Like { LikeId = 2, UserId = 123, QuoteId = 8 },
                new Like { LikeId = 3, UserId = 123, QuoteId = 8 },
                new Like { LikeId = 4, UserId = 123, QuoteId = 8 },
                new Like { LikeId = 5, UserId = 123, QuoteId = 2 },
                new Like { LikeId = 6, UserId = 123, QuoteId = 2 },
                new Like { LikeId = 7, UserId = 123, QuoteId = 4 }
                );

            modelBuilder.Entity<TagAssignment>().HasData(
               new TagAssignment { QuoteId = 8, TagId = 1 },
               new TagAssignment { QuoteId = 10, TagId = 2 },
               new TagAssignment { QuoteId = 9, TagId = 3 },
               new TagAssignment { QuoteId = 5, TagId = 4 },
               new TagAssignment { QuoteId = 3, TagId = 5 },
               new TagAssignment { QuoteId = 2, TagId = 2 },
               new TagAssignment { QuoteId = 4, TagId = 3 }
               );

        }

    }
}
