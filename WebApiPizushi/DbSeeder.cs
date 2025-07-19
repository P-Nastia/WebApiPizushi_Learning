using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Core.Constants;
using Domain;
using Domain.Entities;
using Domain.Entities.Identity;
using Core.Interfaces;
using Core.Models.Seeder;
using Domain.Entities.Delivery;

namespace WebApiPizushi;

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
        var novaPoshta = scope.ServiceProvider.GetRequiredService<INovaPoshtaService>();

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
                    await context.Categories.AddRangeAsync(entityItems);
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

        if (!context.Ingredients.Any())
        {
            var jsonFile = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "JsonData", "Ingredients.json");

            if (File.Exists(jsonFile))
            {
                var jsonData = await File.ReadAllTextAsync(jsonFile);
                try
                {
                    var items = JsonSerializer.Deserialize<List<SeederIngredientModel>>(jsonData);
                    var entityItems = mapper.Map<List<IngredientEntity>>(items);
                    foreach (var entity in entityItems)
                    {
                        entity.Image = await imageService.SaveImageFromUrlAsync(entity.Image);
                    }
                    await context.Ingredients.AddRangeAsync(entityItems);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Json Parse Data", ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Not found file Ingredients.json");
            }
        }

        if (!context.ProductSizes.Any())
        {
            var jsonFile = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "JsonData", "ProductSizes.json");

            if (File.Exists(jsonFile))
            {
                var jsonData = await File.ReadAllTextAsync(jsonFile);
                try
                {
                    var items = JsonSerializer.Deserialize<List<SeederProductSizeModel>>(jsonData);
                    var entityItems = mapper.Map<List<ProductSizeEntity>>(items);
                    
                    await context.ProductSizes.AddRangeAsync(entityItems);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Json Parse Data", ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Not found file ProductSizes.json");
            }
        }

        if (!context.Products.Any())
        {
            var caesar = new ProductEntity
            {
                Name = "Цезаре",
                Slug = "caesar",
                Price = 195,
                Weight = 540,
                CategoryId = 1,
                ProductSizeId = 1
            };
            await context.Products.AddAsync(caesar);
            await context.SaveChangesAsync();
            var ingredients = await context.Ingredients.ToListAsync();
            foreach (var ingredient in ingredients)
            {
                var productIngredient = new ProductIngredientEntity
                {
                    ProductId = caesar.Id,
                    IngredientId = ingredient.Id
                };
                context.ProductIngredients.Add(productIngredient);
            }
            await context.SaveChangesAsync();

            string[] images = {
            "https://emeals-menubuilder.s3.amazonaws.com/v1/recipes/653321/pictures/large_chicken-caesar-pizza.jpg",
            "https://cdn.lifehacker.ru/wp-content/uploads/2022/03/11187_1522960128.7729_1646727034-1170x585.jpg",
            "https://i.obozrevatel.com/food/recipemain/2020/2/5/zhenygohvrxm865gbgzsoxnru3mxjfhwwjd4bmvp.jpeg?size=636x424"
        };

            foreach (var imageUrl in images)
            {
                try
                {
                    var productImage = new ProductImageEntity
                    {
                        ProductId = caesar.Id,
                        Name = await imageService.SaveImageFromUrlAsync(imageUrl)
                    };
                    context.ProductImages.Add(productImage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Json Parse Data for PRODUCT IMAGE", ex.Message);
                }
            }
            await context.SaveChangesAsync();


            var caesar1 = new ProductEntity
            {
                Name = "Цезаре",
                Slug = "caesar",
                Price = 350,
                Weight = 960,
                CategoryId = 1,
                ProductSizeId = 2
            };
            await context.Products.AddAsync(caesar1);
            await context.SaveChangesAsync();
            var ingredients1 = await context.Ingredients.ToListAsync();
            foreach (var ingredient in ingredients1)
            {
                var productIngredient = new ProductIngredientEntity
                {
                    ProductId = caesar1.Id,
                    IngredientId = ingredient.Id
                };
                context.ProductIngredients.Add(productIngredient);
            }
            await context.SaveChangesAsync();

            string[] images1 = {
            "https://emeals-menubuilder.s3.amazonaws.com/v1/recipes/653321/pictures/large_chicken-caesar-pizza.jpg",
            "https://cdn.lifehacker.ru/wp-content/uploads/2022/03/11187_1522960128.7729_1646727034-1170x585.jpg",
            "https://i.obozrevatel.com/food/recipemain/2020/2/5/zhenygohvrxm865gbgzsoxnru3mxjfhwwjd4bmvp.jpeg?size=636x424"
        };

            foreach (var imageUrl in images1)
            {
                try
                {
                    var productImage = new ProductImageEntity
                    {
                        ProductId = caesar1.Id,
                        Name = await imageService.SaveImageFromUrlAsync(imageUrl)
                    };
                    context.ProductImages.Add(productImage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Json Parse Data for PRODUCT IMAGE", ex.Message);
                }
            }
            await context.SaveChangesAsync();

        }

        if (!context.OrderStatuses.Any())
        {
            List<string> names = new List<string>() {
                "Нове", "Очікує оплати", "Оплачено",
                "В обробці", "Готується до відправки",
                "Відправлено", "У дорозі", "Доставлено",
                "Завершено", "Скасовано (вручну)", "Скасовано (автоматично)",
                "Повернення", "В обробці повернення" };

            var orderStatuses = names.Select(name => new OrderStatusEntity { Name = name }).ToList();

            await context.OrderStatuses.AddRangeAsync(orderStatuses);
            await context.SaveChangesAsync();
        }

        if (!context.Orders.Any())
        {
            List<OrderEntity> orders = new List<OrderEntity>
            {
                new OrderEntity
                {
                    UserId = 1,
                    OrderStatusId = 1,
                },
                new OrderEntity
                {
                    UserId = 1,
                    OrderStatusId = 10,
                },
                new OrderEntity
                {
                    UserId = 1,
                    OrderStatusId = 9,
                },
            };

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();
        }

        if (!context.OrderItems.Any())
        {
            var products = await context.Products.ToListAsync();
            var orders = await context.Orders.ToListAsync();
            var rand = new Random();

            foreach (var order in orders)
            {
                var existing = await context.OrderItems
                    .Where(x => x.OrderId == order.Id)
                    .ToListAsync();

                if (existing.Count > 0) continue;

                var productCount = rand.Next(1, Math.Min(5, products.Count + 1));

                var selectedProducts = products
                    .Where(p => p.Id != 1)
                    .OrderBy(_ => rand.Next())
                    .Take(productCount)
                    .ToList();


                var orderItems = selectedProducts.Select(product => new OrderItemEntity
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    PriceBuy = product.Price,
                    Count = rand.Next(1, 5),
                }).ToList();

                context.OrderItems.AddRange(orderItems);
            }

            await context.SaveChangesAsync();
        }


        if (!context.PaymentTypes.Any())
        {
            var list = new List<PaymentTypeEntity>
            {
                new PaymentTypeEntity { Name = "Готівка" },
                new PaymentTypeEntity { Name = "Картка" }
            };

            await context.PaymentTypes.AddRangeAsync(list);
            await context.SaveChangesAsync();
        }

        if (!context.PostDepartments.Any())
        {
            await novaPoshta.FetchDepartmentsAsync();
        }
        Console.WriteLine("SEED DEPARTMENTS");
    }
}
