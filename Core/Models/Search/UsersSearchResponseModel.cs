

using Core.Models.AdminUser;

namespace Core.Models.Search;

public class UsersSearchResponseModel
{
    public List<AdminUserItemModel> Users { get; set; }
    public PaginationResponseModel Pagination { get; set; }
}
