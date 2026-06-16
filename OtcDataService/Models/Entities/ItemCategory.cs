namespace OtcDataService.Models.Entities;

public sealed class ItemCategory
{
    public int CatNo { get; set; }
    public string CatName { get; set; } = string.Empty;
    public string? CatDisplay { get; set; }
}
