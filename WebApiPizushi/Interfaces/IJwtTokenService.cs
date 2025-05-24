namespace WebApiPizushi.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> CreateTokenAsync();
    }
}
