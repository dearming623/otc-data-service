using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using OtcDataService.Models.Entities;
using OtcDataService.Services;

namespace OtcDataService.Repositories;

public sealed class ProdtableRepository : IProdtableRepository
{
    private const string SelectColumns = """
        barcode, p_code, proname, cpronam, p1, p2, p3, p4, dep, food_stmp, sale_tax, amt,
        order_limit, amt_type, qty_in_box, ref_key, unit_dsc, unit_pk_ct, item_dsc, item_pk_ct,
        item_typ_fl, item_dp_fl, vdr_p_code, p5, p6, p7, p8, amt2, amt3, amt4, pkg_dsc, cat_no,
        item_cost, pkg_ct, item_gw, crt_time, pkg_3_dim, item_fob, hot_key, def_loc, item_brand,
        item_origin, smart_link, member_fl, sp_plvl_id, hdby_id, for_sale, item_no, pk_cd, base_ct,
        active_fl, in_whole, asset_acct, income_acct, cogs_acct, item_type, buy_in, mdo_id,
        create_time, create_id
        """;

    private const string TableName = "DBA.prodtable";

    private readonly ConfigurationService _configurationService;
    private readonly OdbcConnectionService _odbcConnectionService;

    public ProdtableRepository(
        ConfigurationService configurationService,
        OdbcConnectionService odbcConnectionService)
    {
        _configurationService = configurationService;
        _odbcConnectionService = odbcConnectionService;
    }

    public async Task<Prodtable?> GetByBarcodeAsync(
        string barcode,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM {TableName}
            WHERE barcode = ?
            """;

        await using var connection = _odbcConnectionService.CreateConnection(_configurationService.Current);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new OdbcParameter { Value = barcode });

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken)
            ? MapFromReader(reader)
            : null;
    }

    public async Task<IReadOnlyList<Prodtable>> ListByPCodeAsync(
        string pCode,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM {TableName}
            WHERE p_code = ?
            """;

        await using var connection = _odbcConnectionService.CreateConnection(_configurationService.Current);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new OdbcParameter { Value = pCode });

        return await ReadAllAsync(command, cancellationToken);
    }

    public async Task<bool> InsertAsync(
        Prodtable entity,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            INSERT INTO {TableName}
              ({SelectColumns})
            VALUES ({CreatePlaceholders(60)})
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
        Prodtable entity,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            UPDATE {TableName}
            SET p_code = ?, proname = ?, cpronam = ?, p1 = ?, p2 = ?, p3 = ?, p4 = ?, dep = ?,
                food_stmp = ?, sale_tax = ?, amt = ?, order_limit = ?, amt_type = ?, qty_in_box = ?,
                ref_key = ?, unit_dsc = ?, unit_pk_ct = ?, item_dsc = ?, item_pk_ct = ?, item_typ_fl = ?,
                item_dp_fl = ?, vdr_p_code = ?, p5 = ?, p6 = ?, p7 = ?, p8 = ?, amt2 = ?, amt3 = ?,
                amt4 = ?, pkg_dsc = ?, cat_no = ?, item_cost = ?, pkg_ct = ?, item_gw = ?, crt_time = ?,
                pkg_3_dim = ?, item_fob = ?, hot_key = ?, def_loc = ?, item_brand = ?, item_origin = ?,
                smart_link = ?, member_fl = ?, sp_plvl_id = ?, hdby_id = ?, for_sale = ?, item_no = ?,
                pk_cd = ?, base_ct = ?, active_fl = ?, in_whole = ?, asset_acct = ?, income_acct = ?,
                cogs_acct = ?, item_type = ?, buy_in = ?, mdo_id = ?, create_time = ?, create_id = ?
            WHERE barcode = ?
            """;

        await using var connection = _odbcConnectionService.CreateConnection(_configurationService.Current);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        AddUpdateParameters(command, entity);
        command.Parameters.Add(new OdbcParameter { Value = entity.Barcode });

        var affected = await command.ExecuteNonQueryAsync(cancellationToken);
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(
        string barcode,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            DELETE FROM {TableName}
            WHERE barcode = ?
            """;

        await using var connection = _odbcConnectionService.CreateConnection(_configurationService.Current);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new OdbcParameter { Value = barcode });

        var affected = await command.ExecuteNonQueryAsync(cancellationToken);
        return affected > 0;
    }

    private static async Task<IReadOnlyList<Prodtable>> ReadAllAsync(
        OdbcCommand command,
        CancellationToken cancellationToken)
    {
        var results = new List<Prodtable>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(MapFromReader(reader));
        }

        return results;
    }

    private static Prodtable MapFromReader(DbDataReader reader) =>
        new()
        {
            Barcode = reader.GetString(reader.GetOrdinal("barcode")),
            PCode = reader.GetString(reader.GetOrdinal("p_code")),
            Proname = GetNullableString(reader, "proname"),
            Cpronam = GetNullableString(reader, "cpronam"),
            P1 = GetNullableDecimal(reader, "p1"),
            P2 = GetNullableDecimal(reader, "p2"),
            P3 = GetNullableDecimal(reader, "p3"),
            P4 = GetNullableDecimal(reader, "p4"),
            Dep = GetNullableDecimal(reader, "dep"),
            FoodStmp = GetNullableDecimal(reader, "food_stmp"),
            SaleTax = GetNullableString(reader, "sale_tax"),
            Amt = GetNullableDecimal(reader, "amt"),
            OrderLimit = GetNullableDecimal(reader, "order_limit"),
            AmtType = GetNullableString(reader, "amt_type"),
            QtyInBox = GetNullableDecimal(reader, "qty_in_box"),
            RefKey = GetNullableString(reader, "ref_key"),
            UnitDsc = GetNullableString(reader, "unit_dsc"),
            UnitPkCt = GetNullableDecimal(reader, "unit_pk_ct"),
            ItemDsc = GetNullableString(reader, "item_dsc"),
            ItemPkCt = GetNullableDecimal(reader, "item_pk_ct"),
            ItemTypFl = GetNullableString(reader, "item_typ_fl"),
            ItemDpFl = GetNullableString(reader, "item_dp_fl"),
            VdrPCode = GetNullableString(reader, "vdr_p_code"),
            P5 = GetNullableDecimal(reader, "p5"),
            P6 = GetNullableDecimal(reader, "p6"),
            P7 = GetNullableDecimal(reader, "p7"),
            P8 = GetNullableDecimal(reader, "p8"),
            Amt2 = GetNullableDecimal(reader, "amt2"),
            Amt3 = GetNullableDecimal(reader, "amt3"),
            Amt4 = GetNullableDecimal(reader, "amt4"),
            PkgDsc = GetNullableString(reader, "pkg_dsc"),
            CatNo = GetNullableInt32(reader, "cat_no"),
            ItemCost = GetNullableDecimal(reader, "item_cost"),
            PkgCt = GetNullableDecimal(reader, "pkg_ct"),
            ItemGw = GetNullableDecimal(reader, "item_gw"),
            CrtTime = GetNullableDateTime(reader, "crt_time"),
            Pkg3Dim = GetNullableInt32(reader, "pkg_3_dim"),
            ItemFob = GetNullableDecimal(reader, "item_fob"),
            HotKey = GetNullableString(reader, "hot_key"),
            DefLoc = reader.GetInt32(reader.GetOrdinal("def_loc")),
            ItemBrand = GetNullableInt32(reader, "item_brand"),
            ItemOrigin = GetNullableInt32(reader, "item_origin"),
            SmartLink = GetNullableString(reader, "smart_link"),
            MemberFl = GetNullableString(reader, "member_fl"),
            SpPlvlId = GetNullableInt32(reader, "sp_plvl_id"),
            HdbyId = GetNullableInt32(reader, "hdby_id"),
            ForSale = reader.GetString(reader.GetOrdinal("for_sale")),
            ItemNo = reader.GetString(reader.GetOrdinal("item_no")),
            PkCd = GetNullableString(reader, "pk_cd"),
            BaseCt = reader.GetDecimal(reader.GetOrdinal("base_ct")),
            ActiveFl = reader.GetString(reader.GetOrdinal("active_fl")),
            InWhole = reader.GetString(reader.GetOrdinal("in_whole")),
            AssetAcct = GetNullableInt32(reader, "asset_acct"),
            IncomeAcct = GetNullableInt32(reader, "income_acct"),
            CogsAcct = GetNullableInt32(reader, "cogs_acct"),
            ItemType = reader.GetString(reader.GetOrdinal("item_type")),
            BuyIn = reader.GetString(reader.GetOrdinal("buy_in")),
            MdoId = GetNullableInt32(reader, "mdo_id"),
            CreateTime = GetNullableDateTime(reader, "create_time"),
            CreateId = GetNullableString(reader, "create_id")
        };

    private static void AddEntityParameters(OdbcCommand command, Prodtable entity)
    {
        command.Parameters.Add(new OdbcParameter { Value = entity.Barcode });
        AddUpdateParameters(command, entity);
    }

    private static void AddUpdateParameters(OdbcCommand command, Prodtable entity)
    {
        command.Parameters.Add(new OdbcParameter { Value = entity.PCode });
        command.Parameters.Add(CreateNullableStringParameter(entity.Proname));
        command.Parameters.Add(CreateNullableStringParameter(entity.Cpronam));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.P1));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.P2));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.P3));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.P4));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.Dep));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.FoodStmp));
        command.Parameters.Add(CreateNullableStringParameter(entity.SaleTax));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.Amt));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.OrderLimit));
        command.Parameters.Add(CreateNullableStringParameter(entity.AmtType));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.QtyInBox));
        command.Parameters.Add(CreateNullableStringParameter(entity.RefKey));
        command.Parameters.Add(CreateNullableStringParameter(entity.UnitDsc));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.UnitPkCt));
        command.Parameters.Add(CreateNullableStringParameter(entity.ItemDsc));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.ItemPkCt));
        command.Parameters.Add(CreateNullableStringParameter(entity.ItemTypFl));
        command.Parameters.Add(CreateNullableStringParameter(entity.ItemDpFl));
        command.Parameters.Add(CreateNullableStringParameter(entity.VdrPCode));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.P5));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.P6));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.P7));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.P8));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.Amt2));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.Amt3));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.Amt4));
        command.Parameters.Add(CreateNullableStringParameter(entity.PkgDsc));
        command.Parameters.Add(CreateNullableInt32Parameter(entity.CatNo));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.ItemCost));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.PkgCt));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.ItemGw));
        command.Parameters.Add(CreateNullableDateTimeParameter(entity.CrtTime));
        command.Parameters.Add(CreateNullableInt32Parameter(entity.Pkg3Dim));
        command.Parameters.Add(CreateNullableDecimalParameter(entity.ItemFob));
        command.Parameters.Add(CreateNullableStringParameter(entity.HotKey));
        command.Parameters.Add(new OdbcParameter { Value = entity.DefLoc });
        command.Parameters.Add(CreateNullableInt32Parameter(entity.ItemBrand));
        command.Parameters.Add(CreateNullableInt32Parameter(entity.ItemOrigin));
        command.Parameters.Add(CreateNullableStringParameter(entity.SmartLink));
        command.Parameters.Add(CreateNullableStringParameter(entity.MemberFl));
        command.Parameters.Add(CreateNullableInt32Parameter(entity.SpPlvlId));
        command.Parameters.Add(CreateNullableInt32Parameter(entity.HdbyId));
        command.Parameters.Add(new OdbcParameter { Value = entity.ForSale });
        command.Parameters.Add(new OdbcParameter { Value = entity.ItemNo });
        command.Parameters.Add(CreateNullableStringParameter(entity.PkCd));
        command.Parameters.Add(CreateDecimalParameter(entity.BaseCt));
        command.Parameters.Add(new OdbcParameter { Value = entity.ActiveFl });
        command.Parameters.Add(new OdbcParameter { Value = entity.InWhole });
        command.Parameters.Add(CreateNullableInt32Parameter(entity.AssetAcct));
        command.Parameters.Add(CreateNullableInt32Parameter(entity.IncomeAcct));
        command.Parameters.Add(CreateNullableInt32Parameter(entity.CogsAcct));
        command.Parameters.Add(new OdbcParameter { Value = entity.ItemType });
        command.Parameters.Add(new OdbcParameter { Value = entity.BuyIn });
        command.Parameters.Add(CreateNullableInt32Parameter(entity.MdoId));
        command.Parameters.Add(CreateNullableDateTimeParameter(entity.CreateTime));
        command.Parameters.Add(CreateNullableStringParameter(entity.CreateId));
    }

    private static string CreatePlaceholders(int count) =>
        string.Join(", ", Enumerable.Repeat("?", count));

    private static string? GetNullableString(DbDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    private static decimal? GetNullableDecimal(DbDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetDecimal(ordinal);
    }

    private static int? GetNullableInt32(DbDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
    }

    private static DateTime? GetNullableDateTime(DbDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
    }

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

    private static OdbcParameter CreateNullableStringParameter(string? value) =>
        new()
        {
            Value = string.IsNullOrEmpty(value) ? DBNull.Value : value
        };

    private static OdbcParameter CreateNullableInt32Parameter(int? value) =>
        new()
        {
            Value = value.HasValue ? value.Value : DBNull.Value
        };

    private static OdbcParameter CreateNullableDateTimeParameter(DateTime? value) =>
        new()
        {
            OdbcType = OdbcType.DateTime,
            Value = value.HasValue ? value.Value : DBNull.Value
        };
}
