using Core.Interfaces;
using Core.Models.Product.Ingredient;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiPizushi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var model = await productService.List();

            return Ok(model);
        }
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var model = await productService.GetById(id);

            return Ok(model);
        }
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var model = await productService.GetBySlug(slug);

            return Ok(model);
        }
        [HttpPost("ingredients")]
        public async Task<IActionResult> CreateIngredients([FromForm] List<string> names, [FromForm] List<IFormFile> files)
        {
            CreateIngredientsModel model = new();
            
            for(int i = 0; i < names.Count; i++)
            {
                model.Ingredients.Add(new CreateIngredientModel
                {
                    Name = names[i],
                    ImageFile = files[i]
                });
            }
            var ingredients = await productService.UploadIngredients(model);
            if (ingredients != null)
                return Ok(ingredients);
            else
                return BadRequest();
        }
    }
}
