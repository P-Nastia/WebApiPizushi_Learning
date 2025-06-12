
using Core.Interfaces;
using Core.Models.Cart;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public class CartService(AppDbContext context) : ICartService
{
    public async Task<long> CreateUpdate(CartCreateUpdateModel model, long userId)
    {
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
        }
        context.Carts.Update(entity);
        await context.SaveChangesAsync();
        return entity.ProductId;
    }
}
