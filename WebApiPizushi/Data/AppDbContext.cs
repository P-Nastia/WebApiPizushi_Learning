using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using WebApiPizushi.Data.Entities;
using WebApiPizushi.Data.Entities.Identity;

namespace WebApiPizushi.Data;

public class AppDbContext : IdentityDbContext<UserEntity, RoleEntity, long>
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }
    public DbSet<CategoryEntity> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<UserRoleEntity>(ur =>
        {
            ur.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(r => r.RoleId)
                .IsRequired();

            ur.HasOne(ur => ur.User)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(u => u.UserId)
                .IsRequired();
        });
    }

}
