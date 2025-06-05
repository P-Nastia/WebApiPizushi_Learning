
using Microsoft.AspNetCore.Http;

namespace Core.Models.Product;

public class ProductCreateModel
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Weight { get; set; }
    public long CategoryId { get; set; }
    public long ProductSizeId { get; set; }
    public List<long>? ProductIngredientsId { get; set; }
    public List<short>? ImagePriorities { get; set; }
    public List<IFormFile>? ImageFiles { get; set; }
}
