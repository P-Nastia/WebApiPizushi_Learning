
using Core.Models.Product;
using Core.Models.Product.Ingredient;

namespace Core.Interfaces;

public interface IProductService
{
    Task<List<ProductItemModel>> List();
    Task<ProductItemModel> GetById(int id);
    Task<List<ProductItemModel>> GetBySlug(string slug);
    Task<List<ProductIngredientModel>> UploadIngredients(CreateIngredientsModel model);
}
