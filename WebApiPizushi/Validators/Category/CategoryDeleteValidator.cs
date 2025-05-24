using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApiPizushi.Data;
using WebApiPizushi.Models.Category;

namespace WebApiPizushi.Validators.Category;

public class CategoryDeleteValidator : AbstractValidator<CategoryDeleteModel>
{
    public CategoryDeleteValidator(AppDbContext db)
    {
        // валідація
        RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id has to be bigger than 0")
                .MustAsync(async (id, cancellation) =>
                    await db.Categories.AnyAsync(c => c.Id == id, cancellation))
                .WithMessage("Category with this Id is not found");
    }
}
