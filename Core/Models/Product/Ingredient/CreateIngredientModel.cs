using Microsoft.AspNetCore.Http;

namespace Core.Models.Product.Ingredient;

public class CreateIngredientModel
{
    public string Name { get; set; } 
    public IFormFile? ImageFile { get; set; } 
}
