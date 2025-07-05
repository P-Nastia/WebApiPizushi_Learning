using AutoMapper;
using Core.Interfaces;
using Core.Models.AdminUser;
using Core.Models.Search;
using Core.Models.Seeder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        var model = await userService.GetSearchUsersAsync(searchParams);
        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Console.WriteLine("RunTime " + elapsedTime);
        return model;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedUsers([FromQuery] SeedItemsModel model)
    {
        var result = await userService.SeedAsync(model);

        return Ok(result);
    }
}
