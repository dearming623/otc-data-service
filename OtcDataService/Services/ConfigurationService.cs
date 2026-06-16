using System.Text.Json;
using OtcDataService.Models;

namespace OtcDataService.Services;

public sealed class ConfigurationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly string _configDirectory;
    private readonly string _configPath;

    public AppConfiguration Current { get; private set; } = new();

    public ConfigurationService()
    {
        _configDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "OtcDataService");
        _configPath = Path.Combine(_configDirectory, "config.json");
        Load();
    }

    public void Load()
    {
        try
        {
            if (!File.Exists(_configPath))
            {
                Current = new AppConfiguration();
                Save();
                return;
            }

            var json = File.ReadAllText(_configPath);
            Current = JsonSerializer.Deserialize<AppConfiguration>(json, JsonOptions) ?? new AppConfiguration();
        }
        catch
        {
            Current = new AppConfiguration();
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(_configDirectory);
        var json = JsonSerializer.Serialize(Current, JsonOptions);
        File.WriteAllText(_configPath, json);
    }

    public void Update(Action<AppConfiguration> update)
    {
        update(Current);
        Save();
    }
}
