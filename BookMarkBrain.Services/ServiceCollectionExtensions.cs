using BookMarkBrain.Services.Mappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;
using BookMarkBrain.Core.ServiceInterfaces;
using BookMarkBrain.Services.Implementations;


namespace BookMarkBrain.Services;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register AutoMapper
        services.AddMappingConfiguration();

        // Register Fluent Validation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register helpers
        services.AddScoped<HtmlParserHelper>();

        // Register application services
        services.AddScoped<ICategoryService, CategoryService>();

        services.AddScoped<IHashtagService, HashtagService>();
        services.AddScoped<ITweetService, TweetService>();
        services.AddScoped<ITweetHashtagService, TweetHashtagService>();
        services.AddScoped<ITweetCategoryService, TweetCategoryService>();


        services.AddScoped<ICollectionService, CollectionService>();
        services.AddScoped<ICollectionTweetService, CollectionTweetService>();

        return services;
    }
}