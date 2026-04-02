using System.Text;
using System.IO;
public enum UserRole
{
    User,
    Admin
}
/// <summary>
/// Представляет пользователя системы.
/// </summary>
public class User
{
    private const int MinPasswordLength = 4;

    /// <summary>
    /// Инициализирует новый экземпляр пользователя.
    /// </summary>
    /// <param name="username">Имя пользователя.</param>
    /// <param name="password">Пароль.</param>
    public User(string username, string password)
    {
        this.Id = Guid.NewGuid();
        this.Username = username ?? throw new ArgumentNullException(nameof(username));
        this.PasswordHash = this.HashPassword(password);
        this.Role = UserRole.User;
        this.IsActive = true;
        this.ExpirationDate = DateTime.MaxValue;
    }

    public Guid Id { get; }

    public string Username { get; }

    public string PasswordHash { get; private set; }

    public UserRole Role { get; private set; }

    public bool IsActive { get; set; }

    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Хеширует пароль.
    /// </summary>
    /// <param name="password">Пароль для хеширования.</param>
    /// <returns>Хеш пароля.</returns>
    private string HashPassword(string password)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
    }

    /// <summary>
    /// Проверяет, соответствует ли пароль хешу.
    /// </summary>
    /// <param name="password">Пароль для проверки.</param>
    /// <returns>True, если пароль верный.</returns>
    public bool ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        return this.PasswordHash == this.HashPassword(password);
    }

    /// <summary>
    /// Изменяет пароль пользователя.
    /// </summary>
    /// <param name="newPassword">Новый пароль.</param>
    public void ChangePassword(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
        {
            throw new ArgumentException("New password cannot be empty", nameof(newPassword));
        }

        if (newPassword.Length < MinPasswordLength)
        {
            throw new ArgumentException($"Password must be at least {MinPasswordLength} characters", nameof(newPassword));
        }

        this.PasswordHash = this.HashPassword(newPassword);
    }

    /// <summary>
    /// Изменяет роль пользователя.
    /// </summary>
    /// <param name="newRole">Новая роль.</param>
    public void ChangeRole(UserRole newRole)
    {
        this.Role = newRole;
    }

    /// <summary>
    /// Проверяет, является ли пароль надежным.
    /// </summary>
    /// <param name="password">Пароль для проверки.</param>
    /// <returns>True, если пароль надежный.</returns>
    public static bool IsPasswordStrong(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        if (password.Length < 6)
        {
            return false;
        }

        bool hasDigit = false;
        bool hasLetter = false;

        foreach (char c in password)
        {
            if (char.IsDigit(c))
            {
                hasDigit = true;
            }

            if (char.IsLetter(c))
            {
                hasLetter = true;
            }
        }

        return hasDigit && hasLetter;
    }
}