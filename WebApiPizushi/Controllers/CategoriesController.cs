using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPizushi.Constants;
using Domain;
using Domain.Entities;
using Core.Interfaces;
using Core.Models.Category;

namespace WebApiPizushi.Controllers;

// це фільтр
[Route("api/[controller]")]
[ApiController]
public class CategoriesController(AppDbContext context,
    IMapper mapper,IImageService imageService) : ControllerBase
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
        var entity = mapper.Map<CategoryEntity>(model);
        entity.Image = await imageService.SaveImageAsync(model.ImageFile!);
        await context.Categories.AddAsync(entity);
        await context.SaveChangesAsync();
        return Ok(entity);
    }
    [HttpGet("{id}")]
    [Authorize(Roles = $"{Roles.Admin}")] // user має бути авторизований і бути адміном для цієї дії 
    public async Task<IActionResult> GetItemById(int id)
    {
        var model = await mapper
            .ProjectTo<CategoryItemModel>(context.Categories.Where(x => x.Id == id))
            .SingleOrDefaultAsync();
        if (model == null)
        {
            return NotFound();
        }
        return Ok(model);
    }
    [HttpPut("edit")] // якщо є метод Put -- то це означає, що це edit / зміна даних
    public async Task<IActionResult> Edit([FromForm] CategoryEditModel model)
    {
        var existing = await context.Categories.FirstOrDefaultAsync(x => x.Id == model.Id);
        if (existing == null)
        {
            return NotFound();
        }

        existing = mapper.Map(model, existing);

        if (model.ImageFile != null)
        {
            await imageService.DeleteImageAsync(existing.Image);
            existing.Image = await imageService.SaveImageAsync(model.ImageFile);
        }
        await context.SaveChangesAsync();

        return Ok();
    }
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(CategoryDeleteModel model)
    {
        var entity = await context.Categories.SingleOrDefaultAsync(x => x.Id == model.Id);
        
        if (!string.IsNullOrEmpty(entity.Image))
        {
            await imageService.DeleteImageAsync(entity.Image);
        }

        context.Categories.Remove(entity);
        await context.SaveChangesAsync();
        return Ok();
    }
}
