using System;
using System.Collections.Generic;

namespace CK.Models;

public partial class SalesPdf
{
    public string FromTable { get; set; } = null!;

    public decimal Quantity { get; set; }

    public string Item { get; set; } = null!;

    public string Barcode { get; set; } = null!;

    public string? Day { get; set; }

    public string Transaction { get; set; } = null!;

    public string ToTable { get; set; } = null!;

    public DateTime Createddatetime { get; set; }

    public string Status { get; set; } = null!;
}
