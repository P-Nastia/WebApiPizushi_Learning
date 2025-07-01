
using AutoMapper;
using Core.Interfaces;
using Core.Models.Account;
using Core.SMTP;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Core.Services;

public class AccountService(IJwtTokenService tokenService,
    UserManager<UserEntity> userManager,
    IMapper mapper,
    IImageService imageService,
    IConfiguration configuration,
    ISmtpService smtpService) : IAccountService
{

    public async Task<string> LoginByGoogle(string token)
    {
        using var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        //configuration
        string userInfo = configuration["GoogleUserInfo"] ?? "https://www.googleapis.com/oauth2/v2/userinfo";
        var response = await httpClient.GetAsync(userInfo);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        var googleUser = JsonSerializer.Deserialize<GoogleAccountModel>(json);

        var existingUser = await userManager.FindByEmailAsync(googleUser!.Email);
        if (existingUser != null)
        {
            var userLoginGoogle = await userManager.FindByLoginAsync("Google", googleUser.GoogleId);

            if (userLoginGoogle == null)
            {
                await userManager.AddLoginAsync(existingUser, new UserLoginInfo("Google", googleUser.GoogleId, "Google"));
            }
            var jwtToken = await tokenService.CreateTokenAsync(existingUser);
            return jwtToken;
        }
        else
        {
            var user = mapper.Map<UserEntity>(googleUser);

            if (!String.IsNullOrEmpty(googleUser.Picture))
            {
                user.Image = await imageService.SaveImageFromUrlAsync(googleUser.Picture);
            }

            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await userManager.AddLoginAsync(user, new UserLoginInfo(
                    loginProvider: "Google",
                    providerKey: googleUser.GoogleId,
                    displayName: "Google"
                    ));

                await userManager.AddToRoleAsync(user, "User");
                var jwtToken = await tokenService.CreateTokenAsync(user);
                return jwtToken;
            }
        }

        return string.Empty;
    }
    public async Task<bool> ForgotPasswordAsync(ForgotPasswordModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return false;
        }

        string token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"{configuration["ClientUrl"]}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(model.Email)}";

        var emailModel = new EmailMessage
        {
            To = model.Email,
            Subject = "Password Reset",
            Body = $"<p>Click the link below to reset your password:</p><a href='{resetLink}'>Reset Password</a>"
        };

        var result = await smtpService.SendEmailAsync(emailModel);

        return result;
    }

    public async Task ResetPasswordAsync(ResetPasswordModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        if (user != null)
            await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
    }

    public async Task<bool> ValidateResetTokenAsync(ValidateResetTokenModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        return await userManager.VerifyUserTokenAsync(
            user,
            TokenOptions.DefaultProvider,
            "ResetPassword",
            model.Token);
    }
}
