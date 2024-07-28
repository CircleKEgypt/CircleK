using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CK.Models;

public partial class SalesInvTran
{
    public double SortFi { get; set; }

    public string BranchId { get; set; } = null!;

    public byte TrType { get; set; }

    public string SerialId { get; set; } = null!;

    public string DocNo { get; set; } = null!;

    public int Sn { get; set; }

    public int RelatedSn { get; set; }

    public string ProdId { get; set; } = null!;

    public string? ChangedId { get; set; }

    public string ProdOtherId { get; set; } = null!;

    public string? ProdOtherName { get; set; }

    public string UnitId { get; set; } = null!;

    public double Tall { get; set; }

    public double Height { get; set; }

    public double Thikness { get; set; }

    public double Weight { get; set; }

    public double Qty { get; set; }

    public double Rate { get; set; }

    public double UserPrice { get; set; }

    public double DiscountAmt { get; set; }

    public double DiscountPercent { get; set; }

    public double DiscTax { get; set; }

    public double EdafaTax { get; set; }

    public double Disc3 { get; set; }

    public double Disc3Val { get; set; }

    public double Amt { get; set; }

    public string StoreId { get; set; } = null!;

    public double SelComm { get; set; }

    public DateTime? ProdDate { get; set; }

    public DateTime? ExpDate { get; set; }

    public string? CstId { get; set; }

    public double RealQty { get; set; }

    public double TotQty { get; set; }

    public double Cost { get; set; }

    public double CostPlus { get; set; }

    public double AllCost { get; set; }

    public byte IsF9 { get; set; }

    public double F9qty { get; set; }

    public string? ItemNotes { get; set; }

    public string NodeId { get; set; } = null!;

    public double OrderSortfi { get; set; }

    public string OrderDocNo { get; set; } = null!;

    public string OrderBranchId { get; set; } = null!;

    public string ProdSerialId { get; set; } = null!;

    public double ProdAvgCost { get; set; }

    public string CreateBranchId { get; set; } = null!;

    public double DeliveryDays { get; set; }

    public double StampingTax { get; set; }
}
