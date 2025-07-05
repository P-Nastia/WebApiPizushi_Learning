

namespace Core.Models.Search;

public class UsersSearchParams
{
    public PaginationRequestModel PaginationRequest { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
