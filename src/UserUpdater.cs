/// <summary>
/// Сервис для обновления данных пользователя.
/// Обеспечивает регистрацию нового пользователя, смену пароля и изменение роли.
/// </summary>
public class UserUpdater
{
    private readonly AuthenticationService _authService;
    private readonly FileLogService _logService;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="UserUpdater"/>.
    /// </summary>
    /// <param name="authService">Сервис аутентификации для поиска, регистрации и управления пользователями.</param>
    /// <param name="logService">Сервис логирования для записи событий изменения данных пользователя.</param>
    public UserUpdater(AuthenticationService authService,
                       FileLogService logService)
    {
        _authService = authService;
        _logService = logService;
    }

    /// <summary>
    /// Обновляет данные пользователя: регистрирует нового, меняет пароль или обновляет роль.
    /// </summary>
    /// <param name="username">Имя пользователя (уникальный идентификатор).</param>
    /// <param name="newPassword">Новый пароль пользователя.</param>
    /// <param name="newRole">Новая роль пользователя (Admin/User/Guest).</param>
  
    public User Update(string username, string newPassword, UserRole newRole)
    {
        var user = _authService.FindUser(username);

        if (user == null)
        {
            user = _authService.RegisterUser(username, newPassword);
            _logService.WriteLog("User registered");
        }
        else
        {
            user.ChangePassword(newPassword);
            _logService.WriteLog("Password updated");
        }

        user.ChangeRole(newRole);
        _logService.WriteLog("Role updated");

        return user;
    }
}