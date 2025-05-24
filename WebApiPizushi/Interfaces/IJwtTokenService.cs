using WebApiPizushi.Data.Entities.Identity;

namespace WebApiPizushi.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> CreateTokenAsync(UserEntity user);
    }
}
