using Core.Interfaces;
using Core.Models.Category;
using Core.Models.Product;
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
            if (names.Count != files.Count)
                return BadRequest("Number of images and ingredients` names isn`t the same!");
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
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] ProductCreateModel model)
        {

            if (model.ImageFiles == null)
                return BadRequest("Image files are empty!");
            if (model.IngredientIds == null)
                return BadRequest("Product ingredients are empty!");
            var entity = await productService.Create(model);
            if (entity != null)
                return Ok(model);
            else return BadRequest("Error create product!");
        }
        [HttpGet("sizes")]
        public async Task<IActionResult> Sizes()
        {
            var model = await productService.GetSizesAsync();

            return Ok(model);
        }
        [HttpGet("ingredients")]
        public async Task<IActionResult> Ingredients()
        {
            var model = await productService.GetIngredientsAsync();

            return Ok(model);
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(ProductDeleteModel model)
        {
            await productService.Delete(model);
            return Ok();
        }
        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromForm] ProductEditModel model)
        {

            if (model.ImageFiles == null)
                return BadRequest("Image files are empty!");
            if (model.IngredientIds == null)
                return BadRequest("Product ingredients are empty!");
            var entity = await productService.Edit(model);
            if (entity != null)
                return Ok(model);
            else return BadRequest("Error create product!");
        }
    }
}
