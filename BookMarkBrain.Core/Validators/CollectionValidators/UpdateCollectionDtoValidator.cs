using BookMarkBrain.Core.DTOs.Collection;
using FluentValidation;

namespace BookMarkBrain.Core.Validators.CollectionValidators;

public class UpdateCollectionDtoValidator : AbstractValidator<UpdateCollectionDto>
{
    public UpdateCollectionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Collection name is required")
            .MaximumLength(100).WithMessage("Collection name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.IconUrl)
            .MaximumLength(2000).WithMessage("Icon URL cannot exceed 2000 characters");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be a non-negative number");
    }
}