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
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var model = await categoryService.List();

		return Ok(model);
    }
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromForm] CategoryCreateModel model)
    {
        var category = await categoryService.Create(model);
        return Ok(category);
    }
    [HttpGet("{id}")]
    [Authorize(Roles = $"{Roles.Admin}")] // user має бути авторизований і бути адміном для цієї дії 
    public async Task<IActionResult> GetItemById(int id)
    {
        var model = await categoryService.GetItemById(id);

		if (model == null)
        {
            return NotFound();
        }
        return Ok(model);
    }
    [HttpPut("edit")] // якщо є метод Put -- то це означає, що це edit / зміна даних
    public async Task<IActionResult> Edit([FromForm] CategoryEditModel model)
    {
        var category = await categoryService.Edit(model);

        return Ok(category);
    }
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(CategoryDeleteModel model)
    {
        await categoryService.Delete(model);
        return Ok();
    }
}
