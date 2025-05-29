using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Entities.Identity;
using WebApiPizushi.Interfaces;

namespace WebApiPizushi.Services;

public class JwtTokenService(IConfiguration configuration, UserManager<UserEntity> userManager) : IJwtTokenService
{
    public async Task<string> CreateTokenAsync(UserEntity user)
    {
        var key = configuration["Jwt:Key"]; // витягає з appsettings.json
        var claims = new List<Claim>
        {
            new Claim("email",user.Email),
            new Claim("name",$"{user.LastName} {user.FirstName}"),
            new Claim("image",$"{user.Image}")
        };
        foreach(var role in await userManager.GetRolesAsync(user))
        {
            claims.Add(new Claim("roles", role));
        }

        // ключ для підпису токена -- перетворення в байти
        var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

        //створюємо об'єкт для підпису токена
        var symmetricSecurityKey = new SymmetricSecurityKey(keyBytes);

        //вказуємо ключ і алгоритм підпису токена
        var signingCredentials = new SigningCredentials(
            symmetricSecurityKey,
            SecurityAlgorithms.HmacSha256);

        //створення токена
        var jwtSecretToken = new JwtSecurityToken(
            claims: claims,//список параметрів у токені, які є доступні
            expires: DateTime.UtcNow.AddDays(7), //після цього часу токен буде недійсним
            signingCredentials: signingCredentials);

        string token = new JwtSecurityTokenHandler().WriteToken(jwtSecretToken);
        return token;
    }
}
