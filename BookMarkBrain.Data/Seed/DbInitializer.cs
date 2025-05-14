using BookMarkBrain.Core.Entities;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BookMarkBrain.Data.Seed;
public static class DbInitializer
{
    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            var logger = services.GetRequiredService<ILogger<AppDbContext>>();

            // Apply pending migrations
            await context.Database.MigrateAsync();

            // Seed data
            await SeedDataAsync(context, logger);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<AppDbContext>>();
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    private static async Task SeedDataAsync(AppDbContext context, ILogger logger)
    {
        // Seed data in the correct order based on dependencies
        await CategorySeeder.SeedAsync(context, logger);
        await HashtagSeeder.SeedAsync(context, logger);
        await TweetSeeder.SeedAsync(context, logger);
        await TweetHashtagSeeder.SeedAsync(context, logger);
        await TweetCategorySeeder.SeedAsync(context, logger);
        await CollectionSeeder.SeedAsync(context, logger);
    }
}