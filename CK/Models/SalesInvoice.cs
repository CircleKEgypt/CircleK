using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CK.Models;

public partial class SalesInvoice
{
    [Key]

    public double SortFi { get; set; }

    public string BranchId { get; set; } = null!;

    public byte TrType { get; set; }

    public string SerialId { get; set; } = null!;

    public string DocNo { get; set; } = null!;

    public string? Lrno { get; set; }

    public double Lrsortfi { get; set; }

    public string LrbranchId { get; set; } = null!;

    public DateTime DocDate { get; set; }

    public DateTime DocTime { get; set; }

    public byte DocType { get; set; }

    public double Part { get; set; }

    public double SecondPart { get; set; }

    public byte DueDateWay { get; set; }

    public DateTime? DueDate { get; set; }

    public byte IsCarry { get; set; }

    public byte SalesWay { get; set; }

    public string? AccCode { get; set; }

    public byte Type { get; set; }

    public string CustId { get; set; } = null!;

    public string StaffId { get; set; } = null!;

    public string CurrId { get; set; } = null!;

    public double CurrVal { get; set; }

    public string VisaId { get; set; } = null!;

    public double TotAmt { get; set; }

    public string? TotInWords { get; set; }

    public double Disc { get; set; }

    public double DiscPercent { get; set; }

    public double TotDesc { get; set; }

    public double MTax { get; set; }

    public double Task1 { get; set; }

    public double TotExp { get; set; }

    public string ClassId { get; set; } = null!;

    public double ClassVal { get; set; }

    public double JournalDoc { get; set; }

    public string StockDoc { get; set; } = null!;

    public double VisaCashSortFi { get; set; }

    public string? Notes { get; set; }

    public string CstId { get; set; } = null!;

    public byte Transfered { get; set; }

    public string BankId { get; set; } = null!;

    public string Payment { get; set; } = null!;

    public string Documents { get; set; } = null!;

    public byte TransAcc { get; set; }

    public byte OfferStates { get; set; }

    public string CreateBranchId { get; set; } = null!;

    public string PartInWords { get; set; } = null!;

    public string ChangeInWords { get; set; } = null!;

    public double? SumAmt { get; set; }

    public double? SumEdafaTax { get; set; }

    public string? Uuid { get; set; }

    public string? Status { get; set; }

    public double PosCashRecive { get; set; }
}
