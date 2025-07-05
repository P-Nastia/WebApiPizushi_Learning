using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bogus;
using Core.Interfaces;
using Core.Models.AdminUser;
using Core.Models.Search;
using Core.Models.Seeder;
using Domain;
using Domain.Entities.Identity;
using FluentValidation.Validators;
using MailKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static Bogus.DataSets.Name;


namespace Core.Services;

public class UserService(UserManager<UserEntity> userManager,
    IMapper mapper,
    AppDbContext context, 
    IImageService imageService,
    RoleManager<RoleEntity> roleManager) : IUserService
{
    public async Task<List<AdminUserItemModel>> GetAllUsersAsync()
    {
        var users = await userManager.Users
            .ProjectTo<AdminUserItemModel>(mapper.ConfigurationProvider)
            .ToListAsync();

        return users;
    }

    public async Task<UsersSearchResponseModel> GetSearchUsersAsync(UsersSearchParams searchParams)
    {
        var query = userManager.Users.ProjectTo<AdminUserItemModel>(mapper.ConfigurationProvider);

        var users = await query.ToListAsync();

        if (!String.IsNullOrEmpty(searchParams.Name) && !String.IsNullOrWhiteSpace(searchParams.Name))
        {
            users = users.Where(x => x.FullName.ToLower().Contains(searchParams.Name.ToLower())).ToList();
        }
        if (!String.IsNullOrEmpty(searchParams.Email) && !String.IsNullOrWhiteSpace(searchParams.Email))
        {
            users = users.Where(x => x.Email.Contains(searchParams.Email)).ToList();
        }

        if (searchParams?.StartDate != null)
        {
            query = query.Where(u => u.DateCreated >= searchParams.StartDate);
        }

        if (searchParams?.EndDate != null)
        {
            query = query.Where(u => u.DateCreated <= searchParams.EndDate);
        }

        if (searchParams?.Roles.Count > 0)
        {
            users = users.Where(user =>
                searchParams.Roles.Any(role => user.Roles.Contains(role))
            ).ToList();
        }


        int total = users.Count;

        users =  users
            .Skip((searchParams.PaginationRequest.CurrentPage - 1) * searchParams.PaginationRequest.ItemsPerPage)
            .Take(searchParams.PaginationRequest.ItemsPerPage)
            .ToList();

        return new UsersSearchResponseModel
        {
            Users = users,
            Pagination = new PaginationResponseModel
            {
                CurrentPage = searchParams.PaginationRequest.CurrentPage,
                TotalAmount = total
            }
        };
    }

    public async Task<string> SeedAsync(SeedItemsModel model)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        var fakeUsers = new Faker<SeederUserModel>("uk")
            .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
            .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName(u.Gender))
            .RuleFor(u => u.LastName, (f, u) => f.Name.LastName(u.Gender))
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .RuleFor(u => u.Password, (f, u) => f.Internet.Password(8))
            .RuleFor(u => u.Roles, f => new List<string>() { f.PickRandom(Constants.Roles.AllRoles) })
            .RuleFor(u => u.Image, f => "https://thispersondoesnotexist.com/");

        var genUsers = fakeUsers.Generate(model.Count);

        try
        {
            
            foreach (var user in genUsers)
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
        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Console.WriteLine("RunTime " + elapsedTime);

        return elapsedTime;
    }
}
