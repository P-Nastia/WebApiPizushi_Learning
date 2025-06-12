
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Interfaces;
using Core.Models.Cart;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public class CartService(AppDbContext context,IAuthService authService,
    IMapper mapper) : ICartService
{
    public async Task CreateUpdate(CartCreateUpdateModel model)
    {
        var userId = await authService.GetUserId();
        var entity = await context.Carts
            .SingleOrDefaultAsync(x => x.UserId == userId && x.ProductId == model.ProductId);
        if (entity != null)
        {
            entity.Quantity = model.Quantity;
        }
        else
        {
            entity = new CartEntity
            {
                UserId = userId,
                ProductId = model.ProductId,
                Quantity = model.Quantity
            };
            await context.Carts.AddAsync(entity);
        }
        await context.SaveChangesAsync();
    }

    public async Task<List<CartItemModel>> GetCartItems()
    {
        var userId = await authService.GetUserId();
        var items = await context.Carts.Where(x => x.UserId == userId)
            .ProjectTo<CartItemModel>(mapper.ConfigurationProvider)
            .ToListAsync();
        return items;
    }
}
