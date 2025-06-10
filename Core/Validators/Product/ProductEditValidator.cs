using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Domain;
using Core.Models.Product;

namespace Core.Validators.Product;

public class ProductEditValidator : AbstractValidator<ProductEditModel>
{
    public ProductEditValidator(AppDbContext db)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Must(name => !string.IsNullOrEmpty(name))
            .WithMessage("Name cannot be empty or null")
            .MaximumLength(250)
            .WithMessage("Name has to be no longer than 250 charachters");

        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithMessage("Slug is required")
            .MaximumLength(250)
            .WithMessage("Slug has to be no longer than 250 charachters");

        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Price is required")
            .Must(x => x > 0)
            .WithMessage("Price has to be bigger than 0");

        RuleFor(x => x.Weight)
            .NotEmpty()
            .WithMessage("Weight is required")
            .Must(x => x > 0)
            .WithMessage("Weight has to be bigger than 0");

        RuleFor(x => x.IngredientIds)
            .NotEmpty()
            .WithMessage("IngredientIds are required");

        RuleFor(x => x.ImageFiles)
            .NotEmpty()
            .WithMessage("ImageFiles are required");

        RuleFor(x => x.ProductSizeId)
            .NotEmpty()
            .WithMessage("Product size is required");
    }
}
