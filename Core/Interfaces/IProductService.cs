
using Core.Models.Product;
using Core.Models.Product.Ingredient;
using Domain.Entities;

namespace Core.Interfaces;

public interface IProductService
{
    Task<List<ProductItemModel>> List();
    Task<ProductItemModel> GetById(int id);
    Task<List<ProductItemModel>> GetBySlug(string slug);
    Task<List<ProductIngredientModel>> UploadIngredients(CreateIngredientsModel model);
    Task<ProductEntity> Create(ProductCreateModel model);
    Task<IEnumerable<ProductIngredientModel>> GetIngredientsAsync();
    Task<IEnumerable<ProductSizeModel>> GetSizesAsync();
    Task Delete(ProductDeleteModel model);

}
