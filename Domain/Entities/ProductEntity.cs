

using Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("tblProducts")]
public class ProductEntity : BaseEntity<long>
{
    [StringLength(250)]
    public string Name { get; set; } = string.Empty;
    [StringLength(250)]
    public string Slug { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Weight { get; set; }

    [ForeignKey("Category")]
    public long CategoryId { get; set; }
    public CategoryEntity? Category { get; set; }

    [ForeignKey("ProductSize")]
    public long? ProductSizeId { get; set; } // не обов'язкова наявність розміру
    public ProductSizeEntity? ProductSize { get; set; }
    public ICollection<ProductIngredientEntity>? ProductIngredients { get; set; }
    public ICollection<ProductImageEntity>? ProductImages { get; set; }
}
