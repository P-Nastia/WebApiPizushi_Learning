
using Core.Models.AdminUser;
using Core.Models.Search;
using Core.Models.Seeder;

namespace Core.Interfaces;

public interface IUserService
{
    Task<List<AdminUserItemModel>> GetAllUsersAsync();
    Task<UsersSearchResponseModel> GetSearchUsersAsync(UsersSearchParams searchParams);
    Task<string> SeedAsync(SeedItemsModel model);
}
