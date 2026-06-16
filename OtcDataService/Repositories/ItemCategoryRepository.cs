using System.Data.Common;
using System.Data.Odbc;
using OtcDataService.Models.Entities;
using OtcDataService.Services;

namespace OtcDataService.Repositories;

public sealed class ItemCategoryRepository : IItemCategoryRepository
{
    private const string TableName = "DBA.t_item_category";

    private readonly ConfigurationService _configurationService;
    private readonly OdbcConnectionService _odbcConnectionService;

    public ItemCategoryRepository(
        ConfigurationService configurationService,
        OdbcConnectionService odbcConnectionService)
    {
        _configurationService = configurationService;
        _odbcConnectionService = odbcConnectionService;
    }

    public async Task<ItemCategory?> GetByCatNoAsync(int catNo, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT cat_no, cat_name, cat_display
            FROM {TableName}
            WHERE cat_no = ?
            """;

        await using var connection = _odbcConnectionService.CreateConnection(_configurationService.Current);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new OdbcParameter { Value = catNo });

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken)
            ? MapFromReader(reader)
            : null;
    }

    private static ItemCategory MapFromReader(DbDataReader reader) =>
        new()
        {
            CatNo = reader.GetInt32(reader.GetOrdinal("cat_no")),
            CatName = reader.GetString(reader.GetOrdinal("cat_name")),
            CatDisplay = GetNullableString(reader, "cat_display")
        };

    private static string? GetNullableString(DbDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }
}
