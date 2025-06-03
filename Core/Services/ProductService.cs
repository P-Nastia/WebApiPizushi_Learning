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
    }
}
