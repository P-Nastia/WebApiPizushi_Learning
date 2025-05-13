using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPizushi.Data;
using WebApiPizushi.Models.Category;

namespace WebApiPizushi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(AppDbContext context,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var model = await mapper.ProjectTo<CategoryItemModel>(context.Categories).ToListAsync();

        return Ok(model);
    }
}
