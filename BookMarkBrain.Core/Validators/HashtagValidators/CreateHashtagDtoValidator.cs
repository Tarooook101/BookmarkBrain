using BookMarkBrain.Core.DTOs.Hashtag;
using FluentValidation;

namespace BookMarkBrain.Core.Validators.HashtagValidators;

public class CreateHashtagDtoValidator : AbstractValidator<CreateHashtagDto>
{
    public CreateHashtagDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Hashtag name is required")
            .MaximumLength(50).WithMessage("Hashtag name must not exceed 50 characters")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Hashtag name can only contain letters, numbers, and underscores");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}
