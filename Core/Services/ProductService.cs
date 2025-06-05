using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Interfaces;
using Core.Models.Category;
using Core.Models.Product;
using Core.Models.Product.Ingredient;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class ProductService(IMapper mapper, AppDbContext context,IImageService imageService) : IProductService
    {
        public async Task<ProductItemModel> GetById(int id)
        {
            return await context.Products.Where(x=>x.Id == id).ProjectTo<ProductItemModel>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
        }

        public async Task<List<ProductItemModel>> GetBySlug(string slug)
        {
            return await context.Products.Where(x=>x.Slug == slug).ProjectTo<ProductItemModel>(mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<List<ProductItemModel>> List()
        {
            return await context.Products.ProjectTo<ProductItemModel>(mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<List<ProductIngredientModel>> UploadIngredients(CreateIngredientsModel model)
        {
            List<ProductIngredientModel> ingredients = new();
            foreach(var ingredient in model.Ingredients!)
            {
                var entity = mapper.Map<IngredientEntity>(ingredient);
                entity.Image = await imageService.SaveImageAsync(ingredient.ImageFile!);
                context.Ingredients.Add(entity);
                await context.SaveChangesAsync();
                ingredients.Add(new ProductIngredientModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Image = entity.Image
                });
            }
            return ingredients;
        }
    }
}
