using BookMarkBrain.Core.DTOs.Category;
using BookMarkBrain.Core.DTOs.Collection;
using BookMarkBrain.Core.DTOs.CollectionTweet;
using BookMarkBrain.Core.DTOs.Hashtag;
using BookMarkBrain.Core.DTOs.Tweet;
using BookMarkBrain.Core.DTOs.TweetCategory;
using BookMarkBrain.Core.DTOs.TweetHashtag;
using BookMarkBrain.Core.Validators.CategoryValidator;
using BookMarkBrain.Core.Validators.CollectionValidators;
using BookMarkBrain.Core.Validators.HashtagValidators;
using BookMarkBrain.Core.Validators.TweetCategoryValidators;
using BookMarkBrain.Core.Validators.TweetHashtagValidators;
using BookMarkBrain.Core.Validators.TweetValidators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BookMarkBrain.Core;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Explicitly register specific validators
        services.AddScoped<IValidator<CreateCategoryDto>, CreateCategoryDtoValidator>();
        services.AddScoped<IValidator<UpdateCategoryDto>, UpdateCategoryDtoValidator>();


        services.AddScoped<IValidator<CreateHashtagDto>, CreateHashtagDtoValidator>();
        services.AddScoped<IValidator<UpdateHashtagDto>, UpdateHashtagDtoValidator>();

        services.AddScoped<IValidator<CreateTweetDto>, CreateTweetDtoValidator>();
        services.AddScoped<IValidator<UpdateTweetDto>, UpdateTweetDtoValidator>();

        services.AddScoped<IValidator<CreateTweetHashtagDto>, CreateTweetHashtagDtoValidator>();


        services.AddScoped<IValidator<CreateTweetCategoryDto>, CreateTweetCategoryDtoValidator>();
        services.AddScoped<IValidator<UpdateTweetCategoryDto>, UpdateTweetCategoryDtoValidator>();



        services.AddScoped<IValidator<CreateCollectionDto>, CreateCollectionDtoValidator>();
        services.AddScoped<IValidator<CreateCollectionTweetDto>, CreateCollectionTweetDtoValidator>();


        services.AddScoped<IValidator<UpdateCollectionDto>, UpdateCollectionDtoValidator>();
        services.AddScoped<IValidator<UpdateCollectionTweetDto>, UpdateCollectionTweetDtoValidator>();

        return services;
    }
}