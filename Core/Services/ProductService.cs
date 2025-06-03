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

            var model = mapper.Map<ProductItemModel>(entity);

            return model;
        }

        public async Task<List<ProductItemModel>> GetBySlug(string slug)
        {
            var entity = await context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductSize)
            .Include(p => p.ProductIngredients)
               .ThenInclude(pi => pi.Ingredient)
            .Where(x => x.Slug == slug)
            .ToListAsync();

            var model = mapper.Map<List<ProductItemModel>>(entity);

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

            var models = mapper.Map<List<ProductItemModel>>(entities);
            return models;
        }
    }
}
