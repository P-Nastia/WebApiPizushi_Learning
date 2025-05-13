using System.ComponentModel.DataAnnotations;

namespace WebApiPizushi.Data.Entities;

public class CategoryEntity : BaseEntity<long>
{
    [StringLength(250)]
    public string Name { get; set; } = string.Empty;
    [StringLength(200)]
    public string Image { get; set; } = string.Empty;
    [StringLength(250)]
    public string Slug { get; set; } = string.Empty;
}
