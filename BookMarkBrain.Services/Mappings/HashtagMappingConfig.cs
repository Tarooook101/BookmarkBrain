using BookMarkBrain.Core.DTOs.Hashtag;
using BookMarkBrain.Core.Entities;
using Mapster;

namespace BookMarkBrain.Services.Mappings;

public class HashtagMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Hashtag -> HashtagDto
        config.NewConfig<Hashtag, HashtagDto>();

        // CreateHashtagDto -> Hashtag
        config.NewConfig<CreateHashtagDto, Hashtag>();

        // UpdateHashtagDto -> Hashtag
        config.NewConfig<UpdateHashtagDto, Hashtag>();
    }
}