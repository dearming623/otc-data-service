namespace OtcDataService.Models.Entities;

public sealed class MktDep
{
    public int DepNo { get; set; }
    public string DepName { get; set; } = string.Empty;
    public string? DepDisplay { get; set; }
}
