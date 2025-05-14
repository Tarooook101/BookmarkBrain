using BookMarkBrain.Core.DTOs.Tweet;
using FluentValidation;


namespace BookMarkBrain.Core.Validators.TweetValidators;

public class CreateTweetDtoValidator : AbstractValidator<CreateTweetDto>
{
    public CreateTweetDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Tweet content is required")
            .MaximumLength(1000).WithMessage("Content cannot exceed 1000 characters");

        RuleFor(x => x.AuthorUsername)
            .NotEmpty().WithMessage("Author username is required")
            .MaximumLength(100).WithMessage("Author username cannot exceed 100 characters");

        RuleFor(x => x.OriginalUrl)
            .NotEmpty().WithMessage("Original URL is required")
            .MaximumLength(2000).WithMessage("URL cannot exceed 2000 characters");
    }
}
