using Core.Interfaces;
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
        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            var model = await productService.GetById(id);

            return Ok(model);
        }
        [HttpGet("slug")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var model = await productService.GetBySlug(slug);

            return Ok(model);
        }
    }
}
