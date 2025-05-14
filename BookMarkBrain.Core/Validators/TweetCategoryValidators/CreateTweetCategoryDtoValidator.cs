using BookMarkBrain.Core.DTOs.TweetCategory;
using FluentValidation;

namespace BookMarkBrain.Core.Validators.TweetCategoryValidators;

public class CreateTweetCategoryDtoValidator : AbstractValidator<CreateTweetCategoryDto>
{
    public CreateTweetCategoryDtoValidator()
    {
        RuleFor(x => x.TweetId)
            .NotEmpty().WithMessage("Tweet ID is required");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required");
    }
}