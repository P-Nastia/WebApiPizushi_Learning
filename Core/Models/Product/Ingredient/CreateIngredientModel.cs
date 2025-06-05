using Microsoft.AspNetCore.Http;

namespace Core.Models.Product.Ingredient;

public class CreateIngredientModel
{
    public string Name { get; set; } = string.Empty;
    public IFormFile? ImageFile { get; set; } 
}
