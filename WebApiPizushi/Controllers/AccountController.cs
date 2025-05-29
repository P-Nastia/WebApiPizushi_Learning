using AutoMapper;
using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApiPizushi.Constants;
using WebApiPizushi.Data.Entities;
using WebApiPizushi.Data.Entities.Identity;
using WebApiPizushi.Interfaces;
using WebApiPizushi.Models.Account;
using WebApiPizushi.Services;

namespace WebApiPizushi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController(IJwtTokenService jwtTokenService,
        UserManager<UserEntity> userManager,
        IImageService imageService, IMapper mapper) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            
                var token = await jwtTokenService.CreateTokenAsync(user);
                return Ok(new { Token = token });
            

        }
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            var user = mapper.Map<UserEntity>(model);

            user.Image = await imageService.SaveImageAsync(model.ImageFile!);

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, Roles.User);
                var token = await jwtTokenService.CreateTokenAsync(user);
                return Ok(new
                {
                    Token = token
                });
            }
            else
            {
                return BadRequest(new
                {
                    status = 400,
                    isValid = false,
                    errors = "Registration failed"
                });
            }

        }
    }
}
