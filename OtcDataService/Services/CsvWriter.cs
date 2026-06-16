using System.Text;

namespace OtcDataService.Services;

public static class CsvWriter
{
    public static string FormatLine(IReadOnlyList<string> values) =>
        string.Join(",", values.Select(EscapeField));

    public static async Task WriteAsync(
        string filePath,
        IReadOnlyList<string> headers,
        IEnumerable<IReadOnlyList<string>> rows,
        CancellationToken cancellationToken = default)
    {
        await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await using var writer = new StreamWriter(stream, Encoding.UTF8);

        await writer.WriteLineAsync(FormatLine(headers).AsMemory(), cancellationToken);

        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await writer.WriteLineAsync(FormatLine(row).AsMemory(), cancellationToken);
        }
    }

    private static string EscapeField(string value)
    {
        if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}
