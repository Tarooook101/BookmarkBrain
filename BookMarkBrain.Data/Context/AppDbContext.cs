using BookMarkBrain.Core.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookMarkBrain.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Hashtag> Hashtags { get; set; }

    public DbSet<Tweet> Tweets { get; set; }

    public DbSet<TweetHashtag> TweetHashtags { get; set; }
    public DbSet<TweetCategory> TweetCategories { get; set; }

    public DbSet<Collection> Collections { get; set; }
    public DbSet<CollectionTweet> CollectionTweets { get; set; }

    

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Id = entry.Entity.Id == Guid.Empty ? Guid.NewGuid() : entry.Entity.Id;
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.IsDeleted = false;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);



        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.ColorHex)
                .HasMaxLength(7);

            entity.Property(e => e.DisplayOrder)
                .HasDefaultValue(0);

            // Self-referencing relationship for hierarchical categories
            entity.HasOne(e => e.ParentCategory)
                .WithMany(e => e.ChildCategories)
                .HasForeignKey(e => e.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        });

        // Configure Hashtag entity
        modelBuilder.Entity<Hashtag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Tweet entity
        modelBuilder.Entity<Tweet>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.AuthorUsername).IsRequired().HasMaxLength(100);
            entity.Property(e => e.OriginalUrl).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.ImageUrl).HasMaxLength(2000);
            entity.Property(e => e.PlatformName).HasMaxLength(50);
        });


        // Configure TweetHashtag entity
        modelBuilder.Entity<TweetHashtag>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Create a unique constraint for TweetId and HashtagId combination
            entity.HasIndex(e => new { e.TweetId, e.HashtagId }).IsUnique();

            // Configure relationships
            entity.HasOne(th => th.Tweet)
                .WithMany(t => t.TweetHashtags) // This references the navigation property in Tweet
                .HasForeignKey(th => th.TweetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(th => th.Hashtag)
                .WithMany(h => h.TweetHashtags) // This references the navigation property in Hashtag
                .HasForeignKey(th => th.HashtagId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        // Configure TweetCategory entity
        modelBuilder.Entity<TweetCategory>(entity =>
        {
            entity.ToTable("TweetCategories");

            entity.HasKey(e => e.Id);

            // Create a unique constraint for TweetId and CategoryId combination
            entity.HasIndex(e => new { e.TweetId, e.CategoryId }).IsUnique();

            // Configure relationships
            entity.HasOne(tc => tc.Tweet)
                .WithMany(t => t.TweetCategories)  // Updated to use navigation property in Tweet
                .HasForeignKey(tc => tc.TweetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(tc => tc.Category)
                .WithMany(c => c.TweetCategories)  // Updated to use navigation property in Category
                .HasForeignKey(tc => tc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        // Configure Collection entity
        modelBuilder.Entity<Collection>(entity =>
        {
            entity.ToTable("Collections");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.IconUrl)
                .HasMaxLength(2000);

            entity.Property(e => e.DisplayOrder)
                .HasDefaultValue(0);
        });

        // Configure CollectionTweet entity
        modelBuilder.Entity<CollectionTweet>(entity =>
        {
            entity.ToTable("CollectionTweets");

            entity.HasKey(e => e.Id);

            // Create a unique constraint for CollectionId and TweetId combination
            entity.HasIndex(e => new { e.CollectionId, e.TweetId }).IsUnique();

            // Configure relationships
            entity.HasOne(ct => ct.Collection)
                .WithMany(c => c.CollectionTweets)
                .HasForeignKey(ct => ct.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ct => ct.Tweet)
                .WithMany() // No navigation property on Tweet for CollectionTweets
                .HasForeignKey(ct => ct.TweetId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        // Apply global query filter for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var isDeletedProperty = entityType.FindProperty("IsDeleted");
            if (isDeletedProperty != null && isDeletedProperty.ClrType == typeof(bool))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "p");
                var filter = Expression.Lambda(
                    Expression.Equal(
                        Expression.Property(parameter, isDeletedProperty.PropertyInfo),
                        Expression.Constant(false, typeof(bool))),
                    parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }

    }
}
