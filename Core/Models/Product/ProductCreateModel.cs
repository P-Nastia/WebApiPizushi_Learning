
using Core.Models.Product.Image;
using Core.Models.Product.Ingredient;

namespace Core.Models.Product
{
    public class ProductCreateModel
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Weight { get; set; }
        public long CategoryId { get; set; }
        public long ProductSizeId { get; set; }
        public ICollection<ProductIngredientModel>? ProductIngredients { get; set; }
        public ICollection<ProductImageModel>? ProductImages { get; set; }
    }
}
