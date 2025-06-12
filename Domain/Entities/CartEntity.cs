

using Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class CartEntity
{
    [ForeignKey("Product")]

    public long ProductId { get; set; }
    public virtual ProductEntity? Product { get; set; }
    [Range(0,50)]
    public int Quantity { get; set; }
    [ForeignKey("User")]
  
    public long UserId { get; set; }
    public virtual UserEntity? User { get; set; }
}
