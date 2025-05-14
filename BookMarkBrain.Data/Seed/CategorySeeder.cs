using BookMarkBrain.Core.Entities;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace BookMarkBrain.Data.Seed;

public static class CategorySeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        // Check if categories already exist
        if (await context.Categories.AnyAsync())
        {
            logger.LogInformation("Categories already exist - skipping seeding");
            return;
        }

        logger.LogInformation("Seeding categories...");

        // Create parent categories
        var programmingCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Programming",
            Description = "Programming topics and tutorials",
            ColorHex = "#3498db",
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow
        };

        var databaseCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Database",
            Description = "Database design and management",
            ColorHex = "#2ecc71",
            DisplayOrder = 2,
            CreatedAt = DateTime.UtcNow
        };

        var uiUxCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "UI/UX Design",
            Description = "User interface and experience design",
            ColorHex = "#e74c3c",
            DisplayOrder = 3,
            CreatedAt = DateTime.UtcNow
        };

        await context.Categories.AddRangeAsync(programmingCategory, databaseCategory, uiUxCategory);
        await context.SaveChangesAsync();

        // Create child categories
        var csharpCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "C#",
            Description = "C# programming language",
            ColorHex = "#9b59b6",
            DisplayOrder = 1,
            ParentCategoryId = programmingCategory.Id,
            CreatedAt = DateTime.UtcNow
        };

        var jsCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "JavaScript",
            Description = "JavaScript programming language",
            ColorHex = "#f39c12",
            DisplayOrder = 2,
            ParentCategoryId = programmingCategory.Id,
            CreatedAt = DateTime.UtcNow
        };

        var sqlServerCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "SQL Server",
            Description = "Microsoft SQL Server",
            ColorHex = "#1abc9c",
            DisplayOrder = 1,
            ParentCategoryId = databaseCategory.Id,
            CreatedAt = DateTime.UtcNow
        };

        await context.Categories.AddRangeAsync(csharpCategory, jsCategory, sqlServerCategory);
        await context.SaveChangesAsync();

        logger.LogInformation("Categories seeded successfully.");
    }
}