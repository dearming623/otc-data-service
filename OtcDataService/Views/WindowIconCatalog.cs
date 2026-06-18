using System.Text.Json;
using Avalonia.Media;
using Avalonia.Platform;

namespace OtcDataService.Views;

internal sealed class WindowIconDefinition
{
    public string Geometry { get; set; } = string.Empty;
    public string Fill { get; set; } = "#1A6FD4";
    public int Size { get; set; } = 32;
    public int Margin { get; set; } = 2;

    public StreamGeometry ParsedGeometry { get; private set; } = null!;
    public Color ParsedFill { get; private set; }

    public void Initialize()
    {
        ParsedGeometry = StreamGeometry.Parse(Geometry);
        ParsedFill = Color.Parse(Fill);
    }
}

internal static class WindowIconCatalog
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static Dictionary<string, WindowIconDefinition>? _icons;

    public static WindowIconDefinition Get(string key)
    {
        _icons ??= Load();

        if (!_icons.TryGetValue(key, out var definition))
            throw new KeyNotFoundException($"Window icon '{key}' was not found in window-icons.json.");

        return definition;
    }

    private static Dictionary<string, WindowIconDefinition> Load()
    {
        using var stream = AssetLoader.Open(new Uri("avares://OtcDataService/Assets/window-icons.json"));
        var icons = JsonSerializer.Deserialize<Dictionary<string, WindowIconDefinition>>(stream, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize window-icons.json.");

        foreach (var definition in icons.Values)
            definition.Initialize();

        return icons;
    }
}
