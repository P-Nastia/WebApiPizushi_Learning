namespace Core.Models.Account;

public class LoginModel
{
    // це буде вказано в свагері
    /// <summary>
    /// Електронна пошта користувача
    /// </summary>
    /// <example>admin@example.com</example>
    public string Email { get; set; }
    /// <summary>
    /// Пароль користувача
    /// </summary>
    /// <example>Admin123!</example>
    public string Password { get; set; }
}
