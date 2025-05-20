using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPizushi.Data;
using WebApiPizushi.Data.Entities;
using WebApiPizushi.Interfaces;
using WebApiPizushi.Models.Category;

namespace WebApiPizushi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(AppDbContext context,
    IMapper mapper,IImageService imageService,
    IValidator<CategoryCreateModel> createValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var model = await mapper.ProjectTo<CategoryItemModel>(context.Categories.OrderBy(x=>x.Id)).ToListAsync();

        return Ok(model);
    }
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromForm] CategoryCreateModel model)
    {
        var res = await createValidator.ValidateAsync(model);
        if (!res.IsValid)
        {
            return BadRequest(res.Errors);
        }
        var repeated = await context.Categories.Where(x => x.Name == model.Name).SingleOrDefaultAsync();
        if (repeated != null)
        {
            return BadRequest($"{model.Name} already exists");
        }
        var entity = mapper.Map<CategoryEntity>(model);
        entity.Image = await imageService.SaveImageAsync(model.ImageFile!);
        await context.Categories.AddAsync(entity);
        await context.SaveChangesAsync();
        return Ok(entity);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetItemById(int id)
    {
        var item = await context.Categories.Where(x => x.Id == id).SingleOrDefaultAsync();
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }
    [HttpPost("edit")]
    public async Task<IActionResult> Edit([FromForm] CategoryEditModel model)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existing = await context.Categories.FirstOrDefaultAsync(x => x.Id == model.Id);
        if (existing == null)
        {
            return NotFound();
        }

        var duplicate = await context.Categories.FirstOrDefaultAsync(x => x.Id != model.Id && x.Name == model.Name);
        if (duplicate != null)
        {
            ModelState.AddModelError("Name", "Така категорія уже існує");
            return BadRequest(ModelState);
        }

        existing = mapper.Map(model, existing);

        if (model.ImageFile != null)
        {
            await imageService.DeleteImageAsync(existing.Image);
            existing.Image = await imageService.SaveImageAsync(model.ImageFile);
        }
        await context.SaveChangesAsync();

        return Ok(existing);
    }
}
