using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Data.Context;
using BookMarkBrain.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookMarkBrain.Data;
public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IHashtagRepository, HashtagRepository>();
        services.AddScoped<ITweetRepository, TweetRepository>();
        services.AddScoped<ITweetHashtagRepository, TweetHashtagRepository>();
        services.AddScoped<ITweetCategoryRepository, TweetCategoryRepository>();


        services.AddScoped<ICollectionRepository, CollectionRepository>();
        services.AddScoped<ICollectionTweetRepository, CollectionTweetRepository>();

        return services;
    }
}