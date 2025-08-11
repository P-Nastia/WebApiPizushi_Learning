
using Core.Models.Product;
using Core.Models.Product.Ingredient;
using Core.Models.Search;
using Core.Models.Search.Products;
using Domain.Entities;

namespace Core.Interfaces;

public interface IProductService
{
    Task<List<ProductItemModel>> List();
    Task<ProductItemModel> GetById(int id);
    Task<List<ProductItemModel>> GetBySlug(string slug);
    Task<ProductIngredientModel> UploadIngredient(CreateIngredientModel model);
    Task<ProductEntity> Create(ProductCreateModel model);
    Task<ProductItemModel> Edit(ProductEditModel model);
    Task<IEnumerable<ProductIngredientModel>> GetIngredientsAsync();
    Task<IEnumerable<ProductSizeModel>> GetSizesAsync();
    Task Delete(ProductDeleteModel model);
    Task<List<ProductItemModel>> GetByCategory(string category);
    Task<SearchResponseModel<ProductItemModel>> SearchProducts(ProductsSearchParams searchParams);
}
