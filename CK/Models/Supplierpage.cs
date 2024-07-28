using System;
using System.Collections.Generic;

namespace CK.Models;

public partial class Supplierpage
{
    public string? SupplierName { get; set; }

    public string? ItemName { get; set; }
    public string? StoreId { get; set; }

    public string ItemLookupCode { get; set; } = null!;
}
