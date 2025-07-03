
using Core.Models.AdminUser;
using Core.Models.Search;

namespace Core.Interfaces;

public interface IUserService
{
    Task<List<AdminUserItemModel>> GetAllUsersAsync();
    Task<UsersSearchResponseModel> GetSearchUsersAsync(PaginationRequestModel pagination);
}
