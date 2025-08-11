
using Core.Models.AdminUser;
using Core.Models.Search;
using Core.Models.Seeder;

namespace Core.Interfaces;

public interface IUserService
{
    Task<List<AdminUserItemModel>> GetAllUsersAsync();
    Task<SearchResponseModel<AdminUserItemModel>> GetSearchUsersAsync(UsersSearchParams searchParams);
    Task<string> SeedAsync(SeedItemsModel model);
    Task<AdminUserItemModel> GetByIdAsync(int id);
    Task<AdminUserItemModel> EditAsync(AdminUserEditItemModel model);
}
