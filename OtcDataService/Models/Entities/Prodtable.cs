namespace OtcDataService.Models.Entities;

public sealed class Prodtable
{
    public string Barcode { get; set; } = string.Empty;
    public string PCode { get; set; } = string.Empty;
    public string? Proname { get; set; }
    public string? Cpronam { get; set; }
    public decimal? P1 { get; set; }
    public decimal? P2 { get; set; }
    public decimal? P3 { get; set; }
    public decimal? P4 { get; set; }
    public decimal? Dep { get; set; }
    public decimal? FoodStmp { get; set; }
    public string? SaleTax { get; set; }
    public decimal? Amt { get; set; }
    public decimal? OrderLimit { get; set; }
    public string? AmtType { get; set; }
    public decimal? QtyInBox { get; set; }
    public string? RefKey { get; set; }
    public string? UnitDsc { get; set; }
    public decimal? UnitPkCt { get; set; }
    public string? ItemDsc { get; set; }
    public decimal? ItemPkCt { get; set; }
    public string? ItemTypFl { get; set; }
    public string? ItemDpFl { get; set; }
    public string? VdrPCode { get; set; }
    public decimal? P5 { get; set; }
    public decimal? P6 { get; set; }
    public decimal? P7 { get; set; }
    public decimal? P8 { get; set; }
    public decimal? Amt2 { get; set; }
    public decimal? Amt3 { get; set; }
    public decimal? Amt4 { get; set; }
    public string? PkgDsc { get; set; }
    public int? CatNo { get; set; }
    public decimal? ItemCost { get; set; }
    public decimal? PkgCt { get; set; }
    public decimal? ItemGw { get; set; }
    public DateTime? CrtTime { get; set; }
    public int? Pkg3Dim { get; set; }
    public decimal? ItemFob { get; set; }
    public string? HotKey { get; set; }
    public int DefLoc { get; set; }
    public int? ItemBrand { get; set; }
    public int? ItemOrigin { get; set; }
    public string? SmartLink { get; set; }
    public string? MemberFl { get; set; }
    public int? SpPlvlId { get; set; }
    public int? HdbyId { get; set; }
    public string ForSale { get; set; } = string.Empty;
    public string ItemNo { get; set; } = string.Empty;
    public string? PkCd { get; set; }
    public decimal BaseCt { get; set; }
    public string ActiveFl { get; set; } = string.Empty;
    public string InWhole { get; set; } = string.Empty;
    public int? AssetAcct { get; set; }
    public int? IncomeAcct { get; set; }
    public int? CogsAcct { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public string BuyIn { get; set; } = string.Empty;
    public int? MdoId { get; set; }
    public DateTime? CreateTime { get; set; }
    public string? CreateId { get; set; }
}
