
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bogus.DataSets;
using Core.Interfaces;
using Core.Models.Delivery;
using Core.Models.Order;
using Core.SMTP;
using Domain;
using Domain.Entities;
using Domain.Entities.Delivery;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public class OrderService(IAuthService authService, ISmtpService smtpService, AppDbContext context,
    IMapper mapper) : IOrderService
{
    public async Task CreateOrderAsync(DeliveryInfoCreateModel model)
    {
        var userId = (await authService.GetUserId()).ToString();
        var user = await context.Users.Where(x => x.Id.ToString() == userId).Include(x=>x.Carts).ThenInclude(x=>x.Product).FirstOrDefaultAsync();

        if(user!=null && user.Carts != null)
        {
            var order = new OrderEntity
            {
                UserId = user.Id,
                OrderStatusId = 1
            };
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            var orderItems = user.Carts.Select(pr =>
            {
                var orderItem = mapper.Map<OrderItemEntity>(pr);
                orderItem.OrderId = order.Id;
                return orderItem;
            }).ToList();

            await context.OrderItems.AddRangeAsync(orderItems);
            await context.SaveChangesAsync();

            var delInfo = mapper.Map<DeliveryInfoEntity>(model);
            delInfo.OrderId = order.Id;

            await context.DeliveryInfos.AddAsync(delInfo);
            user.Carts.Clear();
            await context.SaveChangesAsync();

            var price = orderItems.Sum(i => i.Count * i.PriceBuy);

            var emailModel = new EmailMessage
            {
                To = user.Email,
                Subject = "Успішне оформлення замовлення (PIZUSHI)",
                Body = $@"
        <p>{model.RecipientName}, ваше замовлення на суму {price:N2} грн було успішно оформлено.</p>
        <p>Незабаром його буде відправлено.</p>"
            };

            var result = await smtpService.SendEmailAsync(emailModel);

        }
    }

    public async Task<List<CityModel>> GetCitiesAsync(string city)
    {
        var query = context.Cities.AsQueryable();
        query=query.Where(x => x.Name.ToLower().Contains(city.ToLower()) == true || x.Name.ToLower() == city.ToLower());
        query = query.OrderBy(c =>
                c.Name.ToLower() == city.ToLower() ? 0 : 1
            );
        var cities = await query.ProjectTo<CityModel>(mapper.ConfigurationProvider).Take(15).ToListAsync();
        return cities;
    }

    public async Task<List<OrderModel>> GetOrdersAsync()
    {
        var userId = await authService.GetUserId();
        var orderModelList = await context.Orders
            .Where(x => x.UserId == userId)
            .ProjectTo<OrderModel>(mapper.ConfigurationProvider)
            .ToListAsync();
        orderModelList = orderModelList
        .Select(item =>
        {
            item.TotalPrice = item.OrderItems.Sum(oi => oi.PriceBuy * oi.Count);
            return item;
        })
        .ToList();

        return orderModelList;
    }

    public async Task<List<PostDepartmentModel>> GetPostDepartmentsAsync(PostDepartmentSearchModel model)
    {
        var query = context.PostDepartments.AsQueryable();
        query = query.Where(x => x.CityId == model.CityId);
        if (!string.IsNullOrEmpty(model.Name))
            query = query.Where(x => x.Name.ToLower().Contains(model.Name.ToLower()) == true);
        var departments = await query.ProjectTo<PostDepartmentModel>(mapper.ConfigurationProvider).Take(15).ToListAsync();
        return departments;
    }

    public async Task<List<PaymentTypeModel>> GetPymentTypesAsync()
    {
        var types = await context.PaymentTypes.ProjectTo<PaymentTypeModel>(mapper.ConfigurationProvider).ToListAsync();
        return types;
    }
}
