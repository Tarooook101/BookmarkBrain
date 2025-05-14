using BookMarkBrain.Core.DTOs.CollectionTweet;
using FluentValidation;

namespace BookMarkBrain.Core.Validators.CollectionValidators;

public class CreateCollectionTweetDtoValidator : AbstractValidator<CreateCollectionTweetDto>
{
    public CreateCollectionTweetDtoValidator()
    {
        RuleFor(x => x.CollectionId)
            .NotEmpty().WithMessage("Collection ID is required");

        RuleFor(x => x.TweetId)
            .NotEmpty().WithMessage("Tweet ID is required");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be a non-negative number");
    }
}