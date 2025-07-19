
using Core.Models.Delivery;
using Core.Models.Order;

namespace Core.Interfaces;

public interface IOrderService
{
    Task<List<OrderModel>> GetOrdersAsync();
    Task<List<PaymentTypeModel>> GetPymentTypesAsync();
    Task<List<CityModel>> GetCitiesAsync(string city);
    Task<List<PostDepartmentModel>> GetPostDepartmentsAsync(PostDepartmentSearchModel model);
    Task CreateOrderAsync(DeliveryInfoCreateModel model);
}
