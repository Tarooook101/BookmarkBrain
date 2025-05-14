using BookMarkBrain.Core.DTOs.Collection;
using BookMarkBrain.Core.DTOs.CollectionTweet;
using BookMarkBrain.Core.Entities;
using Mapster;

namespace BookMarkBrain.Services.Mappings;


public class CollectionMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Collection mappings
        config.NewConfig<Collection, CollectionDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.IconUrl, src => src.IconUrl)
            .Map(dest => dest.IsPublic, src => src.IsPublic)
            .Map(dest => dest.DisplayOrder, src => src.DisplayOrder)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);

        config.NewConfig<CreateCollectionDto, Collection>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.IconUrl, src => src.IconUrl)
            .Map(dest => dest.IsPublic, src => src.IsPublic)
            .Map(dest => dest.DisplayOrder, src => src.DisplayOrder)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.IsDeleted)
            .Ignore(dest => dest.CollectionTweets);

        config.NewConfig<UpdateCollectionDto, Collection>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.IconUrl, src => src.IconUrl)
            .Map(dest => dest.IsPublic, src => src.IsPublic)
            .Map(dest => dest.DisplayOrder, src => src.DisplayOrder)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.IsDeleted)
            .Ignore(dest => dest.CollectionTweets);

        // CollectionTweet mappings
        config.NewConfig<CollectionTweet, CollectionTweetDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.CollectionId, src => src.CollectionId)
            .Map(dest => dest.TweetId, src => src.TweetId)
            .Map(dest => dest.DisplayOrder, src => src.DisplayOrder)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);

        config.NewConfig<CreateCollectionTweetDto, CollectionTweet>()
            .Map(dest => dest.CollectionId, src => src.CollectionId)
            .Map(dest => dest.TweetId, src => src.TweetId)
            .Map(dest => dest.DisplayOrder, src => src.DisplayOrder)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.IsDeleted)
            .Ignore(dest => dest.Collection)
            .Ignore(dest => dest.Tweet);

        config.NewConfig<UpdateCollectionTweetDto, CollectionTweet>()
            .Map(dest => dest.DisplayOrder, src => src.DisplayOrder)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CollectionId)
            .Ignore(dest => dest.TweetId)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.IsDeleted)
            .Ignore(dest => dest.Collection)
            .Ignore(dest => dest.Tweet);

        // Additional detailed mappings
        config.NewConfig<Collection, CollectionDetailDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.IconUrl, src => src.IconUrl)
            .Map(dest => dest.IsPublic, src => src.IsPublic)
            .Map(dest => dest.DisplayOrder, src => src.DisplayOrder)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
            .Map(dest => dest.Tweets, src => src.CollectionTweets.Select(ct => ct.Tweet));

        config.NewConfig<CollectionTweet, CollectionTweetDetailDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.CollectionId, src => src.CollectionId)
            .Map(dest => dest.TweetId, src => src.TweetId)
            .Map(dest => dest.DisplayOrder, src => src.DisplayOrder)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
            .Map(dest => dest.Collection, src => src.Collection)
            .Map(dest => dest.Tweet, src => src.Tweet);
    }
}