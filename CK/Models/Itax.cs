using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CK.Models;

public partial class Itax
{
    [Key]
    [Required]
    public string? ItemCode { get; set; }
    [Required]
    public int Tax { get; set; }
}
