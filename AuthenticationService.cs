using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

/// <summary>
/// Сервис аутентификации пользователей.
/// </summary>
public class AuthenticationService
{
    private readonly List<User> users = new List<User>();
    private readonly Dictionary<Guid, Guid?> promotedBy = new Dictionary<Guid, Guid?>();

    /// <summary>
    /// Регистрирует нового пользователя.
    /// </summary>
    /// <param name="username">Имя пользователя.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>Зарегистрированный пользователь.</returns>
    public User RegisterUser(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be empty", nameof(username));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty", nameof(password));
        }

        if (this.users.Any(u => u.Username == username))
        {
            throw new InvalidOperationException("User already exists");
        }

        var user = new User(username, password);
        this.users.Add(user);
        return user;
    }

    /// <summary>
    /// Авторизует пользователя.
    /// </summary>
    /// <param name="username">Имя пользователя.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>Авторизованный пользователь.</returns>
    public User Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be empty", nameof(username));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty", nameof(password));
        }

        var user = this.users.FirstOrDefault(u => u.Username == username);

        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        if (!user.ValidatePassword(password))
        {
            throw new UnauthorizedAccessException("Invalid password");
        }

        return user;
    }

    /// <summary>
    /// Изменяет роль пользователя (только для администратора).
    /// </summary>
    /// <param name="admin">Администратор.</param>
    /// <param name="target">Целевой пользователь.</param>
    /// <param name="newRole">Новая роль.</param>
    public void ChangeUserRole(User admin, User target, UserRole newRole)
    {
        if (admin == null)
        {
            throw new ArgumentNullException(nameof(admin));
        }

        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (admin.Role != UserRole.Admin)
        {
            throw new UnauthorizedAccessException("Only admin can change roles");
        }

        target.ChangeRole(newRole);
    }

    /// <summary>
    /// Создает первого администратора.
    /// </summary>
    /// <param name="username">Имя администратора.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>Созданный администратор.</returns>
    public User CreateFirstAdmin(string username, string password)
    {
        if (this.users.Any())
        {
            throw new InvalidOperationException("First admin can only be created when no users exist");
        }

        var admin = this.RegisterUser(username, password);
        admin.ChangeRole(UserRole.Admin);
        this.promotedBy[admin.Id] = null;
        return admin;
    }
}