
using Core.Models.Delivery;
using System.Runtime.InteropServices;

namespace Core.Models.Order;

public class OrderModel
{
    public long Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public decimal TotalPrice { get; set; }
    public DeliveryInfoModel? DeliveryInfo { get; set; } = null!;
    public List<OrderItemModel>? OrderItems { get; set; }
}
