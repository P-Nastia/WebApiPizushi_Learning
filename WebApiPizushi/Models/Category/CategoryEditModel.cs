using System.ComponentModel.DataAnnotations;

namespace WebApiPizushi.Models.Category;

public class CategoryEditModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public IFormFile? ImageFile { get; set; } = null;
}
