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

    public async Task<UsersSearchResponseModel> GetSearchUsersAsync(PaginationRequestModel pagination)
    {
        var query = userManager.Users.ProjectTo<AdminUserItemModel>(mapper.ConfigurationProvider);

        var total = await query.CountAsync();

        var users = await query
            .Skip((pagination.CurrentPage - 1) * pagination.ItemsPerPage)
            .Take(pagination.ItemsPerPage)
            .ToListAsync();

        var userIds = users.Select(u => u.Id).ToList();

        users = await GetRolesLogins(users);

        return new UsersSearchResponseModel
        {
            Users = users,
            Pagination = new PaginationResponseModel
            {
                CurrentPage = pagination.CurrentPage,
                TotalAmount = query.Count()
            }
        };
    }
}
