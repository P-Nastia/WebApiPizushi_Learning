﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebApiPizushi.Constants;
using WebApiPizushi.Data.Entities;
using WebApiPizushi.Data.Entities.Identity;
using WebApiPizushi.Interfaces;
using WebApiPizushi.Models.Seeder;

namespace WebApiPizushi.Data;

public static class DbSeeder
{
    public static async Task SeedData(this WebApplication webApplication)  // this - розширяє WebApplication, тобто це розширяючий метод
    {
        using var scope = webApplication.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        var imageService = scope.ServiceProvider.GetRequiredService<IImageService>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();

        context.Database.Migrate();

        if (!context.Categories.Any())
        {
            var jsonFile = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "JsonData", "Categories.json");

            if (File.Exists(jsonFile))
            {
                var jsonData = await File.ReadAllTextAsync(jsonFile);
                try
                {
                    var categories = JsonSerializer.Deserialize<List<SeederCategoryModel>>(jsonData);
                    var entityItems = mapper.Map<List<CategoryEntity>>(categories);
                    foreach (var entity in entityItems)
                    {
                        entity.Image = await imageService.SaveImageFromUrlAsync(entity.Image);
                    }
                    await context.AddRangeAsync(entityItems);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Json Parse Data", ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Not found file Categories.json");
            }
        }

        if (!context.Roles.Any())
        {
            foreach (var roleName in Roles.AllRoles)
            {
                var result = await roleManager.CreateAsync(new(roleName));
                if (!result.Succeeded)
                {
                    Console.WriteLine("Error Create Role {0}", roleName);
                }
            }
        }

        if (!context.Users.Any())
        {
            var jsonFile = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "JsonData", "Users.json");
            if (File.Exists(jsonFile))
            {
                var jsonData = await File.ReadAllTextAsync(jsonFile);
                try
                {
                    var users = JsonSerializer.Deserialize<List<SeederUserModel>>(jsonData);
                    foreach (var user in users)
                    {
                        var entity = mapper.Map<UserEntity>(user);
                        entity.UserName = user.Email;
                        entity.Image = await imageService.SaveImageFromUrlAsync(user.Image);
                        var result = await userManager.CreateAsync(entity, user.Password);
                        if (!result.Succeeded)
                        {
                            Console.WriteLine("Error Create User {0}", user.Email);
                            continue;
                        }
                        foreach (var role in user.Roles)
                        {
                            if (await roleManager.RoleExistsAsync(role))
                            {
                                await userManager.AddToRoleAsync(entity, role);
                            }
                            else
                            {
                                Console.WriteLine("Not Found Role {0}", role);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Json Parse Data {0}", ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Not Found File Users.json");
            }
        }
    }
}
