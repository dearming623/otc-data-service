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
            ProductCode = FormatProductCode(product.Barcode),
            ProductCodeType = DefaultProductCodeType,
            ProductName = ResolveProductName(product),
            CategoryCode = dep?.DepName ?? string.Empty,
            CategoryDescription = dep?.DepDisplay ?? dep?.DepName ?? string.Empty,
            SubcategoryCode = FormatSubcategoryCode(product.CatNo),
            SubcategoryDescription = category?.CatDisplay ?? category?.CatName ?? string.Empty
        };

    public static string FormatProductCode(string barcode)
    {
        var trimmed = barcode.Trim();
        return trimmed.Length >= 15 ? trimmed : trimmed.PadLeft(15, '0');
    }

    public static string FormatSubcategoryCode(int? catNo) =>
        catNo.HasValue ? catNo.Value.ToString("D3", CultureInfo.InvariantCulture) : string.Empty;

    private static string ResolveProductName(Prodtable product) =>
        string.IsNullOrWhiteSpace(product.Proname) ? product.Cpronam ?? string.Empty : product.Proname;
}
