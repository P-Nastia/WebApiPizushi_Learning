using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApiPizushi.Data;
using WebApiPizushi.Models.Category;

namespace WebApiPizushi.Validators.Category;

public class CategoryCreateValidator : AbstractValidator<CategoryCreateModel>
{
    public CategoryCreateValidator(AppDbContext db)
    {
        // валідація
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(250)
            .WithMessage("Name has to be no longer than 250 charachters")
            .MustAsync(async (name, cancellation) =>
                    !await db.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower().Trim(), cancellation))
                .WithMessage("Category with this name already exists"); ;
        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithMessage("Slug is required")
            .MaximumLength(250)
            .WithMessage("Slug has to be no longer than 250 charachters")
            .MustAsync(async (slug, cancellation) =>
                    !await db.Categories.AnyAsync(c => c.Slug.ToLower() == slug.ToLower().Trim(), cancellation))
                .WithMessage("Category with this slug already exists"); 

        RuleFor(x => x.ImageFile)
            .NotEmpty()
            .WithMessage("Image file is required");
    }
}
