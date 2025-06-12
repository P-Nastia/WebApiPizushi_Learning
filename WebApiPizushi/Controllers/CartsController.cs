using Core.Interfaces;
using Core.Models.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApiPizushi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class CartsController(ICartService cartService) : ControllerBase
{
    [HttpPost]
    public IActionResult CreateUpdate([FromBody] CartCreateUpdateModel model)
    {
        var email = User.Claims.First().Value; // з JWt-Token
        return Ok(new { message = "Cart updated succesfully" });
    }
}
