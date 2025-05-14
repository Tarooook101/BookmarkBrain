using BookMarkBrain.MVC.Services.APIServiceImplementations;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BookMarkBrain.MVC.Extensions;

public static class MvcServiceExtensions
{
    public static IServiceCollection AddMvcServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add session services
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // Add custom services
        services.AddScoped<IApiService, ApiService>();
        services.AddScoped<ICategoryApiService, CategoryApiService>();
        services.AddScoped<IHashtagApiService, HashtagApiService>();
        services.AddScoped<ITweetApiService, TweetApiService>();
        services.AddScoped<ITweetHashtagApiService, TweetHashtagApiService>();
        services.AddScoped<ITweetCategoryApiService, TweetCategoryApiService>();


        services.AddScoped<ICollectionApiService, CollectionApiService>();
        services.AddScoped<ICollectionTweetApiService, CollectionTweetApiService>();
        return services;
    }
}