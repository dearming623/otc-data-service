using System.Collections.ObjectModel;
using Avalonia.Threading;
using OtcDataService.Models;

namespace OtcDataService.Services;

public sealed class LogService
{
    private const int MaxEntries = 500;
    private readonly object _sync = new();

    public ObservableCollection<LogEntry> Entries { get; } = new();

    public void Info(string message) => Add(LogLevel.Info, message);
    public void Warning(string message) => Add(LogLevel.Warning, message);
    public void Error(string message) => Add(LogLevel.Error, message);

    public void Clear()
    {
        Dispatcher.UIThread.Post(() =>
        {
            lock (_sync)
            {
                Entries.Clear();
            }
        });
    }

    private void Add(LogLevel level, string message)
    {
        var entry = new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = level,
            Message = message
        };

        Dispatcher.UIThread.Post(() =>
        {
            lock (_sync)
            {
                Entries.Add(entry);
                while (Entries.Count > MaxEntries)
                {
                    Entries.RemoveAt(0);
                }
            }
        });
    }
}
