using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Domain;
using Core.Models.Product;

namespace Core.Validators.Product;

public class ProductDeleteValidator : AbstractValidator<ProductDeleteModel>
{
    public ProductDeleteValidator(AppDbContext db)
    {
        RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id has to be bigger than 0")
                .MustAsync(async (id, cancellation) =>
                    await db.Products.AnyAsync(c => c.Id == id, cancellation))
                .WithMessage("Product with this Id is not found");
    }
}
