
using Core.Models.AdminUser;

namespace Core.Models.Search;

public class SearchResponseModel<T> where T : class
{
    public List<T> List { get; set; }
    public PaginationResponseModel Pagination { get; set; }
}
