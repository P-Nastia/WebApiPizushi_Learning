using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Identity;

[Table("tbl_carts")]
public class UserEntity : IdentityUser<long>
{
    public string? FirstName { get; set; } = null;
    public string? LastName { get; set; } = null;
    public string? Image { get; set; } = null;
    public DateTime DateCreated { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
    public virtual ICollection<UserRoleEntity>? UserRoles { get; set; }
    public ICollection<CartEntity>? Carts { get; set; }
    public ICollection<OrderEntity>? Orders { get; set; }
}
