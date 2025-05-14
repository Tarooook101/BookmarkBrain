using BookMarkBrain.Core.DTOs.CollectionTweet;
using FluentValidation;


namespace BookMarkBrain.Core.Validators.CollectionValidators;

public class UpdateCollectionTweetDtoValidator : AbstractValidator<UpdateCollectionTweetDto>
{
    public UpdateCollectionTweetDtoValidator()
    {
        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be a non-negative number");
    }
}