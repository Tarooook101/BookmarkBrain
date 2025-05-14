using BookMarkBrain.Core.DTOs.TweetCategory;
using BookMarkBrain.Core.Entities;
using Mapster;

namespace BookMarkBrain.Services.Mappings;

public class TweetCategoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // TweetCategory -> TweetCategoryDto
        config.NewConfig<TweetCategory, TweetCategoryDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TweetId, src => src.TweetId)
            .Map(dest => dest.CategoryId, src => src.CategoryId)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
            .Map(dest => dest.TweetContent, src => src.Tweet != null ? src.Tweet.Content : null)
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null);

        // TweetCategory -> TweetCategoryDetailDto
        config.NewConfig<TweetCategory, TweetCategoryDetailDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TweetId, src => src.TweetId)
            .Map(dest => dest.TweetContent, src => src.Tweet != null ? src.Tweet.Content : null)
            .Map(dest => dest.AuthorUsername, src => src.Tweet != null ? src.Tweet.AuthorUsername : null)
            .Map(dest => dest.CategoryId, src => src.CategoryId)
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null)
            .Map(dest => dest.CategoryColorHex, src => src.Category != null ? src.Category.ColorHex : null)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);

        // CreateTweetCategoryDto -> TweetCategory
        config.NewConfig<CreateTweetCategoryDto, TweetCategory>()
            .Map(dest => dest.TweetId, src => src.TweetId)
            .Map(dest => dest.CategoryId, src => src.CategoryId)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.IsDeleted)
            .Ignore(dest => dest.Tweet)
            .Ignore(dest => dest.Category);

        // UpdateTweetCategoryDto -> TweetCategory
        config.NewConfig<UpdateTweetCategoryDto, TweetCategory>()
            .Map(dest => dest.TweetId, src => src.TweetId)
            .Map(dest => dest.CategoryId, src => src.CategoryId)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.IsDeleted)
            .Ignore(dest => dest.Tweet)
            .Ignore(dest => dest.Category);
    }
}