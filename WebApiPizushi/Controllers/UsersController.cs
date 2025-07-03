using AutoMapper;
using Core.Interfaces;
using Core.Models.AdminUser;
using Core.Models.Search;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApiPizushi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : Controller
{
    [HttpGet("list")]
    public async Task<IActionResult> GetAllUsers()
    {
        var model = await userService.GetAllUsersAsync();

        return Ok(model);
    }
    [HttpPost("search")]
    public async Task<UsersSearchResponseModel> GetUsersSearchAsync(UsersSearchParams searchParams)
    {
        var model = await userService.GetSearchUsersAsync(searchParams);

        return model;
    }
}
