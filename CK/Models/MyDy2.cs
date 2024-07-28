using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace CK.Models
{
    public class MyDy2
    {
        public string? Fran { get; set; }
        public string? Day { get; set; }
        public string? Time { get; set; }
        public decimal Totals { get; set; }
        public decimal TotalsW { get; set; }
        [Key]
        public string? Location { get; set; }
        public string? DescA { get; set; }
        public string? Descript { get; set; }
        public string? DepaA { get; set; }
        public string? Depa { get; set; }

        public string? SuppA { get; set; }
        public string? Supp { get; set; }
        public decimal Qty { get; set; }
        public long Product { get; set; }
        public string? Transaction { get; set; }
        public int? TransactionNum { get; set; }
        public int? Taxid { get; set; }
        public int? TaxidA { get; set; }
        public DateTime TransDate { get; set; }
        public double QtyT { get; set; }
        public int item { get; set; }

        public string? story { get; set; }

        public string? itemN { get; set; }

        public decimal Cost { get; set; }
        //for Airport Store Cost
        public decimal CostA { get; set; }
        public decimal QtyD { get; set; }

        public int Rmst { get; set; }


    }

}
