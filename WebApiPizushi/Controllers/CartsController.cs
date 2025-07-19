using Core.Interfaces;
using Core.Models.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities;
using System.Security.Claims;

namespace WebApiPizushi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class CartsController(ICartService cartService, IAuthService authService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUpdate([FromBody] CartCreateUpdateModel model)
    {
        await cartService.CreateUpdate(model);
        return Ok(new { message = "Cart updated succesfully" });
    }
    [HttpGet]
    public async Task<IActionResult> GetItems()
    {
        var items = await cartService.GetCartItems();
        var totalPrice = items.Sum(x => x.Price * x.Quantity);

        var id = await authService.GetUserId();

        return Ok(items);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await cartService.Delete(id);
        return Ok();
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddRange([FromBody] List<CartCreateUpdateModel> modelItems)
    {
        foreach(var item in modelItems)
        {
            await cartService.CreateUpdate(item);
        }
        return Ok();
    }
}
