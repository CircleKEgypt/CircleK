using System;
using System.Collections.Generic;

namespace CK.Models.TopSoft;

public partial class CloseDay
{
    public int Id { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Alert { get; set; }
}
