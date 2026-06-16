using System.Data.Common;
using System.Data.Odbc;
using OtcDataService.Models.Entities;
using OtcDataService.Services;

namespace OtcDataService.Repositories;

public sealed class MktDepRepository : IMktDepRepository
{
    private const string TableName = "DBA.t_mkt_dep";

    private readonly ConfigurationService _configurationService;
    private readonly OdbcConnectionService _odbcConnectionService;

    public MktDepRepository(
        ConfigurationService configurationService,
        OdbcConnectionService odbcConnectionService)
    {
        _configurationService = configurationService;
        _odbcConnectionService = odbcConnectionService;
    }

    public async Task<MktDep?> GetByDepNoAsync(int depNo, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT dep_no, dep_name, dep_display
            FROM {TableName}
            WHERE dep_no = ?
            """;

        await using var connection = _odbcConnectionService.CreateConnection(_configurationService.Current);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new OdbcParameter { Value = depNo });

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken)
            ? MapFromReader(reader)
            : null;
    }

    private static MktDep MapFromReader(DbDataReader reader) =>
        new()
        {
            DepNo = reader.GetInt32(reader.GetOrdinal("dep_no")),
            DepName = reader.GetString(reader.GetOrdinal("dep_name")),
            DepDisplay = GetNullableString(reader, "dep_display")
        };

    private static string? GetNullableString(DbDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }
}
