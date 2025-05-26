using FluentValidation;
using Microsoft.AspNetCore.Identity;
using WebApiPizushi.Data.Entities.Identity;
using WebApiPizushi.Models.Account;
using WebApiPizushi.Models.Category;

namespace WebApiPizushi.Validators.Account
{
    public class LoginValidator : AbstractValidator<LoginModel>
    {
        public LoginValidator(UserManager<UserEntity> um)
        {
            RuleFor(x => x.Email)
                .NotEmpty().
                WithMessage("Email is required");
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required");
            RuleFor(x=>x.Email)
                .MustAsync(async (model,email, cancellationToken) =>
                {
                    var user = await um.FindByEmailAsync(model.Email);
                    if (user == null)
                        return false;

                    return await um.CheckPasswordAsync(user, model.Password);
                })
            .WithMessage("Invalid email or password");
        }
    }
}
