using BookMarkBrain.Core.DTOs.TweetHashtag;
using FluentValidation;

namespace BookMarkBrain.Core.Validators.TweetHashtagValidators;

/// <summary>
/// Validator for CreateTweetHashtagDto to ensure data integrity
/// </summary>
public class CreateTweetHashtagDtoValidator : AbstractValidator<CreateTweetHashtagDto>
{
    public CreateTweetHashtagDtoValidator()
    {
        RuleFor(x => x.TweetId)
            .NotEmpty().WithMessage("Tweet ID is required");

        RuleFor(x => x.HashtagId)
            .NotEmpty().WithMessage("Hashtag ID is required");
    }
}
