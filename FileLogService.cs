using System.IO;

/// <summary>
/// Сервис для работы с файлом журнала.
/// </summary>
public class FileLogService
{
    private readonly string path;

    /// <summary>
    /// Инициализирует новый экземпляр FileLogService.
    /// </summary>
    /// <param name="path">Путь к файлу журнала.</param>
    public FileLogService(string path)
    {
        this.path = path ?? throw new ArgumentNullException(nameof(path));
    }

    /// <summary>
    /// Записывает сообщение в журнал.
    /// </summary>
    /// <param name="message">Сообщение для записи.</param>
    public void WriteLog(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var directory = Path.GetDirectoryName(this.path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.AppendAllText(this.path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
    }

    /// <summary>
    /// Читает все записи из журнала.
    /// </summary>
    /// <returns>Содержимое журнала.</returns>
    public string ReadLog()
    {
        if (!File.Exists(this.path))
        {
            return string.Empty;
        }

        return File.ReadAllText(this.path);
    }

    /// <summary>
    /// Очищает журнал.
    /// </summary>
    public void ClearLog()
    {
        if (File.Exists(this.path))
        {
            File.Delete(this.path);
        }
    }
}