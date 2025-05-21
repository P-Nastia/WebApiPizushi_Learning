using FluentValidation;
using WebApiPizushi.Models.Category;

namespace WebApiPizushi.Validators.Category;

public class CategoryCreateValidator : AbstractValidator<CategoryCreateModel>
{
    public CategoryCreateValidator()
    {
        // валідація
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(250)
            .WithMessage("Name has to be no longer than 250 charachters");
        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithMessage("Slug is required")
            .MaximumLength(250)
            .WithMessage("Slug has to be no longer than 250 charachters");
        RuleFor(x => x.ImageFile)
            .NotEmpty()
            .WithMessage("Image file is required");
    }
}
