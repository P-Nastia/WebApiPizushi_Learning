using Microsoft.EntityFrameworkCore;
using System;
using WebApiPizushi.Data.Entities;

namespace WebApiPizushi.Data;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }
    public DbSet<CategoryEntity> Categories { get; set; }

}
