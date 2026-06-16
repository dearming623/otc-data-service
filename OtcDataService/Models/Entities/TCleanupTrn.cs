namespace OtcDataService.Models.Entities;

public sealed class TCleanupTrn
{
    public string CuItem { get; set; } = string.Empty;
    public DateOnly CuDate { get; set; }
    public decimal CuQty { get; set; }
    public int CuLoc { get; set; }
    public decimal CuSettQty { get; set; }
    public decimal? CuAmt { get; set; }
    public decimal? CuCost { get; set; }
    public string CuTrnType { get; set; } = "P";

    public TCleanupTrnKey ToKey() =>
        new(CuItem, CuDate, CuLoc, CuTrnType);
}

public sealed record TCleanupTrnKey(string CuItem, DateOnly CuDate, int CuLoc, string CuTrnType);
