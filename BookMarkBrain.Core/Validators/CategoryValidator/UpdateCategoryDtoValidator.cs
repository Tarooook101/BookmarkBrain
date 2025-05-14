using BookMarkBrain.Core.DTOs.Category;
using FluentValidation;

namespace BookMarkBrain.Core.Validators.CategoryValidator;

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot be longer than 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot be longer than 500 characters");

        RuleFor(x => x.ColorHex)
            .Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
            .WithMessage("Color must be a valid hex code (e.g., #FF5733)");

        RuleFor(x => x.DisplayOrder)
            .InclusiveBetween(0, 1000)
            .WithMessage("Display order must be between 0 and 1000");
    }
}