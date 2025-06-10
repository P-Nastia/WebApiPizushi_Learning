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
    public class ProductService(IMapper mapper, AppDbContext context,
        IImageService imageService) : IProductService
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
                .FirstOrDefaultAsync();
            product.IsDeleted = true;
            await context.SaveChangesAsync();
        }

        public async Task<ProductItemModel> Edit(ProductEditModel model)
        {
            var entity = await context.Products.Where(x => x.Id == model.Id)
                .FirstOrDefaultAsync();

            var item = await context.Products
                .Where(x => x.Id == model.Id)
                .ProjectTo<ProductItemModel>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            mapper.Map(model, entity);

            var imgDelete = item.ProductImages
                .Where(x => !model.ImageFiles!.Any(y => y.FileName == x.Name))
                .ToList();

            foreach(var img in imgDelete)
            {
                var productImage = await context.ProductImages
                    .Where(x => x.Id == img.Id)
                    .SingleOrDefaultAsync();

                if (productImage != null)
                {
                    await imageService.DeleteImageAsync(img.Name);
                    context.ProductImages.Remove(productImage);
                }
            }
            await context.SaveChangesAsync();

            short p = 0;
            foreach(var imgFile in model.ImageFiles!)
            {
                if(imgFile.ContentType == "old-image")
                {
                    var img = await context.ProductImages
                        .Where(x => x.Name == imgFile.FileName)
                        .SingleOrDefaultAsync();
                    img.Priority = p;

                }
                else
                {
                    var productImage = new ProductImageEntity
                    {
                        ProductId = entity.Id,
                        Name = await imageService.SaveImageAsync(imgFile),
                        Priority = p,
                    };
                    context.ProductImages.Add(productImage);
                }
                p++;
            }
            await context.SaveChangesAsync();

            var ingr = await context.ProductIngredients.Where(x => x.ProductId == model.Id).ToListAsync();
            context.ProductIngredients.RemoveRange(ingr);

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

            return item;
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
            return await context.Products.Where(x=>!x.IsDeleted).ProjectTo<ProductItemModel>(mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<ProductIngredientModel> UploadIngredient(CreateIngredientModel model)
        {
            var entity = mapper.Map<IngredientEntity>(model);
            entity.Image = await imageService.SaveImageAsync(model.ImageFile!);
            context.Ingredients.Add(entity);
            await context.SaveChangesAsync();
            
            return mapper.Map<ProductIngredientModel>(entity);
        }
    }
}
