using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Interfaces;
using Core.Models.Product;
using Core.Models.Product.Ingredient;
using Domain;
using Domain.Entities;
using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class ProductService(IMapper mapper, AppDbContext context,IImageService imageService) : IProductService
    {
        public async Task<ProductEntity> Create(ProductCreateModel model)
        {
            var entity = mapper.Map<ProductEntity>(model);
            context.Products.Add(entity);
            await context.SaveChangesAsync();
            foreach (var ingId in model.IngredientIds!)
            {
                var productIngredient = new ProductIngredientEntity
                {
                    ProductId = entity.Id,
                    IngredientId = ingId
                };
                context.ProductIngredients.Add(productIngredient);
            }
            await context.SaveChangesAsync();

            for (short i = 0; i < model.ImageFiles!.Count; i++)
            {
                try
                {
                    var productImage = new ProductImageEntity
                    {
                        ProductId = entity.Id,
                        Name = await imageService.SaveImageAsync(model.ImageFiles[i]),
                        Priority = i,
                    };
                    context.ProductImages.Add(productImage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Json Parse Data for PRODUCT IMAGE", ex.Message);
                }
            }
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(ProductDeleteModel model)
        {
            var product = await context.Products.Where(x => x.Id == model.Id)
                .Include(x=>x.ProductIngredients)
                .Include(x=>x.ProductImages)
                .FirstOrDefaultAsync();
            if (product!.ProductIngredients != null)
            {
                context.ProductIngredients.RemoveRange(product.ProductIngredients);
            }
            if (product.ProductImages != null)
            {
                foreach(var image in product.ProductImages)
                {
                    await imageService.DeleteImageAsync(image.Name);
                }
                context.ProductImages.RemoveRange(product!.ProductImages);
            }
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }

        public async Task<ProductItemModel> GetById(int id)
        {
            return await context.Products.Where(x=>x.Id == id).ProjectTo<ProductItemModel>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
        }

        public async Task<List<ProductItemModel>> GetBySlug(string slug)
        {
            return await context.Products.Where(x=>x.Slug == slug).ProjectTo<ProductItemModel>(mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<IEnumerable<ProductIngredientModel>> GetIngredientsAsync()
        {
            var ingredients = await context.Ingredients
            .ProjectTo<ProductIngredientModel>(mapper.ConfigurationProvider)
            .ToListAsync();
            return ingredients;
        }

        public async Task<IEnumerable<ProductSizeModel>> GetSizesAsync()
        {
            var sizes = await context.ProductSizes
            .ProjectTo<ProductSizeModel>(mapper.ConfigurationProvider)
            .ToListAsync();
            return sizes;
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
