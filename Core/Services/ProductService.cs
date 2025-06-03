using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Interfaces;
using Core.Models.Category;
using Core.Models.Product;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class ProductService(IMapper mapper, AppDbContext context) : IProductService
    {
        public async Task<ProductItemModel> GetById(int id)
        {
            var entity = await context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductSize)
            .Include(p => p.ProductIngredients)
               .ThenInclude(pi => pi.Ingredient)
            .FirstOrDefaultAsync(x => x.Id == id);

            entity.ProductImages = entity.ProductImages?
                    .OrderBy(img => img.Priority)
                    .ToList();

            var model = mapper.Map<ProductItemModel>(entity);

            return model;
        }

        public async Task<List<ProductItemModel>> GetBySlug(string slug)
        {
            var entities = await context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductSize)
            .Include(p => p.ProductIngredients)
               .ThenInclude(pi => pi.Ingredient)
            .Where(x => x.Slug == slug)
            .ToListAsync();
            foreach (var product in entities)
            {
                product.ProductImages = product.ProductImages?
                    .OrderBy(img => img.Priority)
                    .ToList();
            }

            var model = mapper.Map<List<ProductItemModel>>(entities);

            return model;
        }

        public async Task<List<ProductItemModel>> List()
        {
            var entities = await context.Products
             .Include(p => p.Category)
             .Include(p => p.ProductImages)
             .Include(p => p.ProductSize)
             .Include(p => p.ProductIngredients)
                 .ThenInclude(pi => pi.Ingredient)
             .ToListAsync();
            foreach (var product in entities)
            {
                product.ProductImages = product.ProductImages?
                    .OrderBy(img => img.Priority)
                    .ToList();
            }
            var models = mapper.Map<List<ProductItemModel>>(entities);
            return models;
        }
    }
}
