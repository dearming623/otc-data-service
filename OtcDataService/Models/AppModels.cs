namespace OtcDataService.Models;

public enum AppPage
{
    Home,
    Settings
}

public enum SettingsSection
{
    Database,
    Export
}

public enum LogLevel
{
    Info,
    Warning,
    Error
}

public sealed class LogEntry
{
    public DateTime Timestamp { get; init; }
    public LogLevel Level { get; init; }
    public string Message { get; init; } = string.Empty;

    public string DisplayText => $"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level.ToString().ToUpperInvariant()}] {Message}";
}
