using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPizushi.Data;
using WebApiPizushi.Data.Entities;
using WebApiPizushi.Interfaces;
using WebApiPizushi.Models.Category;
using WebApiPizushi.Services;

namespace WebApiPizushi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(AppDbContext context,
    IMapper mapper,IImageService imageService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var model = await mapper.ProjectTo<CategoryItemModel>(context.Categories).ToListAsync();

        return Ok(model);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CategoryCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var exists = context.Categories.Where(x => x.Name == model.Name).FirstOrDefaultAsync();
        if (exists != null)
        {
            return BadRequest($"{model.Name} already exists");
        }
        var entity = mapper.Map<CategoryEntity>(model);
        entity.Image = await imageService.SaveImageAsync(model.ImageFile!);
        await context.Categories.AddAsync(entity);
        await context.SaveChangesAsync();
        return Ok(entity);
    }
}
