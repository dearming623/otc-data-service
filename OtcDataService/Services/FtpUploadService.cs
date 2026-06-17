using FluentFTP;
using OtcDataService.Models;

namespace OtcDataService.Services;

public sealed class FtpUploadService
{
    public static bool ValidateSettings(AppConfiguration config, out string? errorMessage)
    {
        if (!config.FtpUploadEnabled)
        {
            errorMessage = null;
            return true;
        }

        if (string.IsNullOrWhiteSpace(config.FtpHost))
        {
            errorMessage = "FTP host is required when FTP upload is enabled.";
            return false;
        }

        if (config.FtpPort is <= 0 or > 65535)
        {
            errorMessage = "FTP port must be between 1 and 65535.";
            return false;
        }

        errorMessage = null;
        return true;
    }

    public async Task<bool> TestConnectionAsync(AppConfiguration config, CancellationToken cancellationToken = default)
    {
        if (!ValidateSettings(config, out var validationError))
        {
            throw new InvalidOperationException(validationError);
        }

        if (!config.FtpUploadEnabled)
        {
            return true;
        }

        await using var client = CreateClient(config);
        await client.Connect(cancellationToken);

        var remotePath = NormalizeRemoteDirectory(config.FtpRemotePath);
        if (!string.IsNullOrEmpty(remotePath))
        {
            await client.CreateDirectory(remotePath, true, cancellationToken);
        }

        return true;
    }

    public async Task UploadFileAsync(
        AppConfiguration config,
        string localFilePath,
        CancellationToken cancellationToken = default)
    {
        if (!config.FtpUploadEnabled)
        {
            return;
        }

        if (!ValidateSettings(config, out var validationError))
        {
            throw new InvalidOperationException(validationError);
        }

        var fileName = Path.GetFileName(localFilePath);
        var remoteDirectory = NormalizeRemoteDirectory(config.FtpRemotePath);
        var remotePath = string.IsNullOrEmpty(remoteDirectory)
            ? fileName
            : $"{remoteDirectory}/{fileName}";

        await using var client = CreateClient(config);
        await client.Connect(cancellationToken);

        if (!string.IsNullOrEmpty(remoteDirectory))
        {
            await client.CreateDirectory(remoteDirectory, true, cancellationToken);
        }

        var status = await client.UploadFile(
            localFilePath,
            remotePath,
            FtpRemoteExists.Overwrite,
            true,
            token: cancellationToken);

        if (status != FtpStatus.Success)
        {
            throw new InvalidOperationException($"FTP upload failed with status {status}.");
        }
    }

    private static AsyncFtpClient CreateClient(AppConfiguration config) =>
        new(config.FtpHost.Trim(), config.FtpUserName, config.FtpPassword, config.FtpPort);

    private static string NormalizeRemoteDirectory(string? remotePath)
    {
        if (string.IsNullOrWhiteSpace(remotePath))
        {
            return string.Empty;
        }

        var normalized = remotePath.Trim().Replace('\\', '/').Trim('/');
        return normalized;
    }
}
