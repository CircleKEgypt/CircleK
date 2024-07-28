using System;
using System.Collections.Generic;

namespace CK.Models;

public partial class Retailperiodicdiscount
{
    public int Concurrencymode { get; set; }

    public string Currencycode { get; set; } = null!;

    public int Datevalidationtype { get; set; }

    public string? Description { get; set; }

    public string? Disclaimer { get; set; }

    public long Discountcode { get; set; }

    public long Discountledgerdimension { get; set; }

    public decimal Discountpercentvalue { get; set; }

    public long Instancerelationtype { get; set; }

    public int Isdiscountcoderequired { get; set; }

    public string Name { get; set; } = null!;

    public string Offerid { get; set; } = null!;

    public int Periodicdiscounttype { get; set; }

    public int Status { get; set; }

    public string Validationperiodid { get; set; } = null!;

    public DateTime Validfrom { get; set; }

    public DateTime Validto { get; set; }

    public int Pricingprioritynumber { get; set; }

    public int? Countnondiscountitems { get; set; }

    public int? Disconpos { get; set; }

    public int? Quantitylimit { get; set; }

    public int? Multibuydiscounttype { get; set; }

    public decimal? Dealpricevalue { get; set; }

    public decimal? Discountamountvalue { get; set; }

    public int? Mixandmatchcountnondiscountitems { get; set; }

    public int? Mixandmatchdiscounttype { get; set; }

    public int? Noofleastexpensivelines { get; set; }

    public int? Numberoftimesapplicable { get; set; }

    public int? Leastexpensivemode { get; set; }

    public int? Generatesbundleid { get; set; }

    public string Dataareaid { get; set; } = null!;

    public long Partition { get; set; }

    public long Recid { get; set; }

    public int Recversion { get; set; }

    public DateTime Modifieddatetime { get; set; }

    public long Relationtype { get; set; }

    public string Printdescriptiononfiscalreceipt { get; set; } = null!;

    public string? Dlvmodeid { get; set; }

    public int Matchallassociatedpricegroups { get; set; }

    public int Processingstatus { get; set; }

    public DateTime Disabledsince { get; set; }

    public int Pricecomponenttype { get; set; }

    public long Headerpricingrule { get; set; }

    public int Headerpricinggroupcode { get; set; }

    public string Pricecomponentcodename { get; set; } = null!;

    public int Shoulduseinterval { get; set; }

    public int Isgupdiscount { get; set; }

    public string Discountvendorclaimgroupname { get; set; } = null!;

    public int? Istierconfigurationenabled { get; set; }

    public int? Tierpriceadjustmentmethod { get; set; }

    public int? Freeitemcriteriatype { get; set; }

    public int? Freeitemrepeatable { get; set; }

    public int? Freeitemcalculationmethod { get; set; }
}
