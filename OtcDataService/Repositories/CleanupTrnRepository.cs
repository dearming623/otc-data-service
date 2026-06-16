using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using OtcDataService.Models.Entities;
using OtcDataService.Services;

namespace OtcDataService.Repositories;

public sealed class CleanupTrnRepository : ICleanupTrnRepository
{
    private const string SelectColumns =
        "cu_item, cu_date, cu_qty, cu_loc, cu_sett_qty, cu_amt, cu_cost, cu_trn_type";

    private const string TableName = "DBA.t_cleanup_trn";

    private readonly ConfigurationService _configurationService;
    private readonly OdbcConnectionService _odbcConnectionService;

    public CleanupTrnRepository(
        ConfigurationService configurationService,
        OdbcConnectionService odbcConnectionService)
    {
        _configurationService = configurationService;
        _odbcConnectionService = odbcConnectionService;
    }

    public async Task<TCleanupTrn?> GetByKeyAsync(
        TCleanupTrnKey key,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM {TableName}
            WHERE cu_item = ? AND cu_date = ? AND cu_loc = ? AND cu_trn_type = ?
            """;

        await using var connection = _odbcConnectionService.CreateConnection(_configurationService.Current);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        AddKeyParameters(command, key);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken)
            ? MapFromReader(reader)
            : null;
    }

    public async Task<IReadOnlyList<TCleanupTrn>> ListByDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM {TableName}
            WHERE cu_date = ?
            """;

        await using var connection = _odbcConnectionService.CreateConnection(_configurationService.Current);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(CreateDateParameter(date));

        return await ReadAllAsync(command, cancellationToken);
    }

    public async Task<IReadOnlyList<TCleanupTrn>> ListByDateRangeAsync(
        DateOnly start,
        DateOnly end,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM {TableName}
            WHERE cu_date >= ? AND cu_date < ?
            """;

        await using var connection = _odbcConnectionService.CreateConnection(_configurationService.Current);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(CreateDateParameter(start));
        command.Parameters.Add(CreateDateParameter(end));

        return await ReadAllAsync(command, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> ListDistinctCuItemsByDateRangeAsync(
        DateOnly start,
        DateOnly end,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT DISTINCT cu_item
            FROM {TableName}
            WHERE cu_date >= ? AND cu_date < ? AND cu_trn_type = 'P'
            """;

        await using var connection = _odbcConnectionService.CreateConnection(_configurationService.Current);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(CreateDateParameter(start));
        command.Parameters.Add(CreateDateParameter(end));

        var results = new List<string>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(reader.GetString(0));
        }

        return results;
    }

    public async Task<bool> InsertAsync(
        TCleanupTrn entity,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            INSERT INTO {TableName}
              (cu_item, cu_date, cu_qty, cu_loc, cu_sett_qty, cu_amt, cu_cost, cu_trn_type)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?)
            """;

        await using var connection = _odbcConnectionService.CreateConnection(_configurationService.Current);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        AddEntityParameters(command, entity);

        var affected = await command.ExecuteNonQueryAsync(cancellationToken);
        return affected > 0;
    }

    public async Task<bool> UpdateAsync(
        TCleanupTrn entity,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            UPDATE {TableName}
            SET cu_qty = ?, cu_sett_qty = ?, cu_amt = ?, cu_cost = ?
            WHERE cu_item = ? AND cu_date = ? AND cu_loc = ? AND cu_trn_type = ?
            """;

        await using var connection = _odbcConnectionService.CreateConnection(_configurationService.Current);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(CreateDecimalParameter(entity.CuQty));
        command.Parameters.Add(CreateDecimalParameter(entity.CuSettQty));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.CuAmt));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.CuCost));
        AddKeyParameters(command, entity.ToKey());

        var affected = await command.ExecuteNonQueryAsync(cancellationToken);
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(
        TCleanupTrnKey key,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            DELETE FROM {TableName}
            WHERE cu_item = ? AND cu_date = ? AND cu_loc = ? AND cu_trn_type = ?
            """;

        await using var connection = _odbcConnectionService.CreateConnection(_configurationService.Current);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        AddKeyParameters(command, key);

        var affected = await command.ExecuteNonQueryAsync(cancellationToken);
        return affected > 0;
    }

    private static async Task<IReadOnlyList<TCleanupTrn>> ReadAllAsync(
        OdbcCommand command,
        CancellationToken cancellationToken)
    {
        var results = new List<TCleanupTrn>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(MapFromReader(reader));
        }

        return results;
    }

    private static TCleanupTrn MapFromReader(DbDataReader reader) =>
        new()
        {
            CuItem = reader.GetString(reader.GetOrdinal("cu_item")),
            CuDate = ToDateOnly(reader.GetValue(reader.GetOrdinal("cu_date"))),
            CuQty = reader.GetDecimal(reader.GetOrdinal("cu_qty")),
            CuLoc = reader.GetInt32(reader.GetOrdinal("cu_loc")),
            CuSettQty = reader.GetDecimal(reader.GetOrdinal("cu_sett_qty")),
            CuAmt = GetNullableDecimal(reader, "cu_amt"),
            CuCost = GetNullableDecimal(reader, "cu_cost"),
            CuTrnType = reader.GetString(reader.GetOrdinal("cu_trn_type"))
        };

    private static DateOnly ToDateOnly(object value) =>
        value switch
        {
            DateOnly dateOnly => dateOnly,
            DateTime dateTime => DateOnly.FromDateTime(dateTime),
            _ => DateOnly.FromDateTime(Convert.ToDateTime(value))
        };

    private static decimal? GetNullableDecimal(DbDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetDecimal(ordinal);
    }

    private static void AddKeyParameters(OdbcCommand command, TCleanupTrnKey key)
    {
        command.Parameters.Add(new OdbcParameter { Value = key.CuItem });
        command.Parameters.Add(CreateDateParameter(key.CuDate));
        command.Parameters.Add(new OdbcParameter { Value = key.CuLoc });
        command.Parameters.Add(new OdbcParameter { Value = key.CuTrnType });
    }

    private static void AddEntityParameters(OdbcCommand command, TCleanupTrn entity)
    {
        command.Parameters.Add(new OdbcParameter { Value = entity.CuItem });
        command.Parameters.Add(CreateDateParameter(entity.CuDate));
        command.Parameters.Add(CreateDecimalParameter(entity.CuQty));
        command.Parameters.Add(new OdbcParameter { Value = entity.CuLoc });
        command.Parameters.Add(CreateDecimalParameter(entity.CuSettQty));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.CuAmt));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.CuCost));
        command.Parameters.Add(new OdbcParameter { Value = entity.CuTrnType });
    }

    private static OdbcParameter CreateDateParameter(DateOnly date) =>
        new()
        {
            OdbcType = OdbcType.Date,
            Value = date.ToDateTime(TimeOnly.MinValue)
        };

    private static OdbcParameter CreateDecimalParameter(decimal value) =>
        new()
        {
            OdbcType = OdbcType.Decimal,
            Value = value
        };

    private static OdbcParameter CreateNullableDecimalParameter(decimal? value) =>
        new()
        {
            OdbcType = OdbcType.Decimal,
            Value = value.HasValue ? value.Value : DBNull.Value
        };
}
