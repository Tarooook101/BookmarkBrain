using BookMarkBrain.Core.DTOs.Tweet;

namespace BookMarkBrain.Core.DTOs.Collection;

public class CollectionDetailDto : CollectionDto
{
    public ICollection<TweetDto> Tweets { get; set; } = new List<TweetDto>();
}