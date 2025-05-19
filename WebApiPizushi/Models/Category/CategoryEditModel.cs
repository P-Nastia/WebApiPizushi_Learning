using System.ComponentModel.DataAnnotations;

namespace WebApiPizushi.Models.Category;

public class CategoryEditModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required")]
    [StringLength(250, ErrorMessage = "Name has to be no longer than 250 charachters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug is required")]
    [StringLength(250, ErrorMessage = "Slug has to be no longer than 250 charachters")]
    public string Slug { get; set; } = string.Empty;
    [Required(ErrorMessage = "ImageFile is required")]
    public IFormFile? ImageFile { get; set; } = null;
}
