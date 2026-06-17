using System.Reflection;

namespace OtcDataService;

public static class AppInfo
{
    public const string Name = "OTC Data Service";
    public const string SingleInstanceMutexName = "Global\\OtcDataService.SingleInstance";
    public const string SingleInstancePipeName = "OtcDataService.SingleInstance";

    public static string Version { get; } = ResolveVersion();

    public static string WindowTitle => $"{Name} v{Version}";

    public static string VersionLabel => $"v{Version}";

    private static string ResolveVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        if (!string.IsNullOrWhiteSpace(informationalVersion))
        {
            return informationalVersion;
        }

        return assembly.GetName().Version?.ToString(3) ?? "0.0.0";
    }
}
