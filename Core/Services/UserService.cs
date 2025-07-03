using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Interfaces;
using Core.Models.AdminUser;
using Core.Models.Search;
using Domain;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Core.Services;

public class UserService(UserManager<UserEntity> userManager,
    IMapper mapper,
    AppDbContext context) : IUserService
{
    public async Task<List<AdminUserItemModel>> GetAllUsersAsync()
    {
        var users = await userManager.Users
            .ProjectTo<AdminUserItemModel>(mapper.ConfigurationProvider)
            .ToListAsync();

        users = await GetRolesLogins(users);
        return users;
    }

    private async Task<List<AdminUserItemModel>> GetRolesLogins(List<AdminUserItemModel> users)
    {
        await context.UserLogins.ForEachAsync(login =>
        {
            var user = users.FirstOrDefault(u => u.Id == login.UserId);
            if (user != null)
            {
                user.LoginTypes.Add(login.LoginProvider);
            }
        });

        var roleUsers = await userManager.Users.AsNoTracking().ToListAsync();

        foreach (var roleUser in roleUsers)
        {
            var adminUser = users.FirstOrDefault(u => u.Id == roleUser.Id);
            if (adminUser != null)
            {
                var roles = await userManager.GetRolesAsync(roleUser);
                adminUser.Roles = roles.ToList();

                if (!string.IsNullOrEmpty(roleUser.PasswordHash))
                {
                    adminUser.LoginTypes.Add("Password");
                }
            }
        }

        return users;
    }

    public async Task<UsersSearchResponseModel> GetSearchUsersAsync(UsersSearchParams searchParams)
    {
        var query = userManager.Users.ProjectTo<AdminUserItemModel>(mapper.ConfigurationProvider);

        var total = await query.CountAsync();

        var users = await query
            .Skip((searchParams.PaginationRequest.CurrentPage - 1) * searchParams.PaginationRequest.ItemsPerPage)
            .Take(searchParams.PaginationRequest.ItemsPerPage)
            .ToListAsync();

        if (!String.IsNullOrEmpty(searchParams.Name) && !String.IsNullOrWhiteSpace(searchParams.Name))
        {
            users = users.Where(x => x.FullName.ToLower().Contains(searchParams.Name.ToLower())).ToList();
        }
        if (!String.IsNullOrEmpty(searchParams.Email) && !String.IsNullOrWhiteSpace(searchParams.Email))
        {
            users = users.Where(x => x.Email.Contains(searchParams.Email)).ToList();
        }

        users = await GetRolesLogins(users);

        if (searchParams?.Roles.Count > 0)
        {
            users = users.Where(user =>
                searchParams.Roles.Any(role => user.Roles.Contains(role))
            ).ToList();
        }

        return new UsersSearchResponseModel
        {
            Users = users,
            Pagination = new PaginationResponseModel
            {
                CurrentPage = searchParams.PaginationRequest.CurrentPage,
                TotalAmount = query.Count()
            }
        };
    }
}
