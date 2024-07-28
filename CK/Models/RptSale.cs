using System;
using System.Collections.Generic;

namespace CK.Models;

public partial class RptSale
{
    public string? DpId { get; set; }

    public int? GroupId { get; set; }

    public string? DpName { get; set; }

    public int? StoreCode { get; set; }

    public string? StoreId { get; set; }

    public string? StoreName { get; set; }

    public string? StoreFranchise { get; set; }
    public string? Company { get; set; }

    public int ItemId { get; set; }

    public string? ItemName { get; set; }

    public string? ItemLookupCode { get; set; }

    public DateTime? TransTime { get; set; }

    public int? ByDay { get; set; }

    public int? ByMonth { get; set; }

    public int? ByYear { get; set; }

    public DateTime? TransDate { get; set; }

    public decimal Qty { get; set; }
    public decimal TotalinDash { get; set; }
    public string FormattedTotalinDash
    {
        get
        {
            // Format the decimal to have two decimal places
            return TotalinDash.ToString("N2");
        }
        set
        {
            // Convert the value back to decimal if necessary
            TotalinDash = Decimal.Parse(value);
        }
    }
    public decimal Price { get; set; }

    public double? TotalSales { get; set; }
    public double? TotalCost { get; set; }
    public double? TotalTax { get; set; }
    public double? TotalSaleswithTax { get; set; }
    public double? TransactionsCount { get; set; }

    public string ConvDate { get; set; }
    public string OpenDate { get; set; }
    public string ClosedDate { get; set; }
    public string ConvTotalSales { get; set; }
    public string? FTotalSales { get; set; }
    public string FQty { get; set; }
    public string ConvTotalQty { get; set; }
    public string? TransactionNumber { get; set; }

    public decimal Cost { get; set; }

    public double? TotalCostQty { get; set; }

    public double? Profit { get; set; }

    public decimal Tax { get; set; }

    public double? TotalSalesTax { get; set; }

    public double? TotalSalesWithoutTax { get; set; }

    public double? TotalCostWithoutTax { get; set; }

    public string? SupplierId { get; set; }
    public string? SupplierName { get; set; }

    public string? Dmanager { get; set; }

    public string? Username { get; set; }

    public string? Fmanager { get; set; }
}
