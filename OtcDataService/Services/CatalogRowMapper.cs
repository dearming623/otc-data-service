using System.Globalization;
using OtcDataService.Models;
using OtcDataService.Models.Entities;

namespace OtcDataService.Services;

public static class CatalogRowMapper
{
    public const string DefaultProductCodeType = "UPC";

    public static CatalogExportRow Map(Prodtable product, MktDep? dep, ItemCategory? category) =>
        new()
        {
            ProductCode = FormatProductCode(product),
            ProductCodeType = DefaultProductCodeType,
            ProductName = ResolveProductName(product),
            CategoryCode = dep?.DepNo.ToString() ?? string.Empty,
            CategoryDescription = dep?.DepName ?? string.Empty,//dep?.DepDisplay ?? dep?.DepName ?? string.Empty,
            SubcategoryCode = FormatSubcategoryCode(product.CatNo),
            SubcategoryDescription = category?.CatDisplay ?? category?.CatName ?? string.Empty
        };

    public static string FormatProductCode(Prodtable product)
    {
        // P = PLU fixed,  W = PLU Weight
        var amtType = product.AmtType?.Trim();
        if (amtType is not null &&
            (amtType.Equals("P", StringComparison.OrdinalIgnoreCase) ||
             amtType.Equals("W", StringComparison.OrdinalIgnoreCase)))
        {
            return product.ItemNo.Trim();
        }

        string barcode = product.Barcode ?? string.Empty;
        var trimmed = barcode.Trim();
        return trimmed.Length == 13 ? trimmed[..12] : trimmed;
    }

    public static string FormatSubcategoryCode(int? catNo) =>
        catNo.HasValue ? catNo.Value.ToString("D3", CultureInfo.InvariantCulture) : string.Empty;

    private static string ResolveProductName(Prodtable product) =>
        string.IsNullOrWhiteSpace(product.Proname) ? product.Cpronam ?? string.Empty : product.Proname;
}
