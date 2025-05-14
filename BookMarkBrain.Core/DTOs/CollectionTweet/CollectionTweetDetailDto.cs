using BookMarkBrain.Core.DTOs.Collection;
using BookMarkBrain.Core.DTOs.Tweet;

namespace BookMarkBrain.Core.DTOs.CollectionTweet;

public class CollectionTweetDetailDto : CollectionTweetDto
{
    public CollectionDto Collection { get; set; }
    public TweetDto Tweet { get; set; }
}