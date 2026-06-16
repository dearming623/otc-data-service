using System.Data;
using System.Data.Odbc;
using OtcDataService.Models;

namespace OtcDataService.Services;

public sealed class OdbcConnectionService
{
    private readonly LogService _logService;

    public OdbcConnectionService(LogService logService)
    {
        _logService = logService;
    }

    public bool TestConnection(AppConfiguration config, out string? errorMessage)
    {
        errorMessage = null;
        var connectionString = config.BuildConnectionString();

        try
        {
            using var connection = new OdbcConnection(connectionString);
            connection.Open();
            if (connection.State == ConnectionState.Open)
            {
                _logService.Info("ODBC connection test succeeded.");
                return true;
            }

            errorMessage = "Connection did not open.";
            _logService.Error(errorMessage);
            return false;
        }
        catch (Exception ex)
        {
            errorMessage = FormatOdbcError(ex);
            _logService.Error($"ODBC connection test failed: {errorMessage}");
            return false;
        }
    }

    public OdbcConnection CreateConnection(AppConfiguration config) =>
        new(config.BuildConnectionString());

    public static string FormatOdbcError(Exception ex)
    {
        var message = ex.Message;

        if (message.Contains("IM014", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("architecture mismatch", StringComparison.OrdinalIgnoreCase))
        {
            var processBits = Environment.Is64BitProcess ? "64-bit" : "32-bit";
            return $"ODBC architecture mismatch: this app runs as {processBits}, but DSN/driver use a different bitness. " +
                   "This app is built for 32-bit ODBC (use DSN from C:\\Windows\\SysWOW64\\odbcad32.exe). " +
                   "Alternatively, register a 64-bit DSN in C:\\Windows\\System32\\odbcad32.exe if a 64-bit driver exists.";
        }

        return message;
    }
}
