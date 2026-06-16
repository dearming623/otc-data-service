namespace OtcDataService.Models;

public sealed class CatalogExportRow
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductCodeType { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string CategoryCode { get; set; } = string.Empty;
    public string CategoryDescription { get; set; } = string.Empty;
    public string SubcategoryCode { get; set; } = string.Empty;
    public string SubcategoryDescription { get; set; } = string.Empty;

    public static IReadOnlyList<string> Headers { get; } =
    [
        "ProductCode",
        "ProductCodeType",
        "ProductName",
        "CategoryCode",
        "CategoryDescription",
        "SubcategoryCode",
        "SubcategoryDescription"
    ];

    public IReadOnlyList<string> ToValues() =>
    [
        ProductCode,
        ProductCodeType,
        ProductName,
        CategoryCode,
        CategoryDescription,
        SubcategoryCode,
        SubcategoryDescription
    ];
}
