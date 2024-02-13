using CK.Model;
using CK.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.Composition;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq.Expressions;
using Tuple = System.Tuple;
using DocumentFormat.OpenXml.Bibliography;
using System.Security.Cryptography.X509Certificates;

namespace CK.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        string[] storeVal;
        bool exported = false;
        [HttpGet]
        public IActionResult Index()
        {
            DataCenterContext db = new DataCenterContext();
            CkhelperdbContext db3 = new CkhelperdbContext();
            DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
            db.Database.SetCommandTimeout(600);// Set the timeout in seconds
            IQueryable<RptSale> RptSales = db.RptSales;
            IQueryable<RptSalesAxt> RptSalesAxts = db.RptSalesAxts;
            IQueryable<RptSales2> RptSales2s = db4.RptSales2s;
            IQueryable<RptSalesAll> RptSalesAlls = db.RptSalesAlls;
            //Store List Text=StoreName , Value = StoreId
            ViewBag.VBStore = db3.Liststores
                .GroupBy(m => m.StoreName)
                .Select(group => new { Store = group.First().StoreIdD + ":" + group.First().StoreIdR, StoreName = group.Key })
                .OrderBy(m => m.StoreName)
                .ToList();
            ViewBag.VBDepartment = db.Departments
                                                 .GroupBy(m => m.Name)
                                                 .Select(group => new { Code = group.First().Code, Name = group.Key })
                                                 .OrderBy(m => m.Name)
                                                 .ToList();

            //Supplier List Text=SupplierName , Value = Code 
            ViewBag.VBSupplier = db.Suppliers
                                             .GroupBy(m => m.SupplierName)
                                                 .Select(group => new { Code = group.First().Code, SupplierName = group.Key })
                                                 .OrderBy(m => m.SupplierName)
                                                 .ToList();

            ViewBag.VBItemBarcode = db.Items.Select(m => new { m.Id, m.ItemLookupCode }).Distinct();
            ViewBag.VBStoreFranchise = db.Stores
                 .Where(m => m.Franchise != null)
                 .Select(m => m.Franchise)
                 .Distinct()
                 .ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Index(SalesParameters Parobj)
        {
            string startDate = Parobj.startDate;
            string endDate = Parobj.endDate;
            string store = Parobj.Store;
            string department = Parobj.Department;
            string supplier = Parobj.Supplier;
            bool exportAfterClick = Parobj.ExportAfterClick;
            string[] selectedItems = Parobj.SelectedItems;
            bool vPerDay = Parobj.VPerDay;
            bool vPerMonYear = Parobj.VPerMonYear;
            bool vPerMon = Parobj.VPerMon;
            bool vPerYear = Parobj.VPerYear;
            bool vQty = Parobj.VQty;
            bool vPrice = Parobj.VPrice;
            bool vStoreName = Parobj.VStoreName;
            bool vDepartment = Parobj.VDepartment;
            bool VTotalSales = Parobj.VTotalSales;
            bool vTotalCost = Parobj.VTotalCost;
            bool vTotalTax = Parobj.VTotalTax;
            bool VTotalSalesTax = Parobj.VTotalSalesTax;
            bool VTotalSalesWithoutTax = Parobj.VTotalSalesWithoutTax;
            bool vTotalCostQty = Parobj.VTotalCostQty;
            bool vCost = Parobj.VCost;
            bool vItemLookupCode = Parobj.VItemLookupCode;
            bool vItemName = Parobj.VItemName;
            bool vSupplierId = Parobj.VSupplierId;
            bool vSupplierName = Parobj.VSupplierName;
            string franchise = Parobj.Franchise;
            bool vTransactionNumber = Parobj.VTransactionNumber;
            bool vFranchise = Parobj.VFranchise;
            int? monthToFilter = Parobj.MonthToFilter;
            string ItemLookupCodeTxt = Parobj.ItemLookupCodeTxt;
            string itemNameTxt = Parobj.ItemNameTxt;
            bool tmt = Parobj.TMT;
            bool rms = Parobj.RMS;
            bool dbBefore = Parobj.DBbefore;
            bool yesterday = Parobj.Yesterday;
            DataCenterContext db = new DataCenterContext();
            CkhelperdbContext db3 = new CkhelperdbContext();
            DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
            db.Database.SetCommandTimeout(600);// Set the timeout in seconds
            db3.Database.SetCommandTimeout(600);// Set the timeout in seconds
            db4.Database.SetCommandTimeout(600);// Set the timeout in seconds
            IQueryable<RptSale> RptSales = db.RptSales;
            IQueryable<RptSalesAxt> RptSalesAxts = db.RptSalesAxts;
            IQueryable<RptSales2> RptSales2s = db4.RptSales2s;
            IQueryable<RptSalesAll> RptSalesAlls = db.RptSalesAlls;
            //Store List Text=StoreName , Value = StoreId
            if (Parobj.Franchise == "TMT")
            {
                if (Parobj.TMT && (Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0]))))
                {
                    RptSalesAlls = RptSalesAlls.Where(s => s.StoreFranchise == "TMT");
                }
                else if (Parobj.RMS && (string.IsNullOrEmpty(storeVal[0])) || Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0])))//|| storeVal[0] =="0" || RMS && (TMT =false
                {
                    RptSales = RptSales.Where(s => s.StoreFranchise == "TMT");
                }
                else if (Parobj.TMT && !string.IsNullOrEmpty(storeVal[0]))
                {
                    RptSalesAxts = RptSalesAxts.Where(s => s.StoreFranchise == "TMT");
                }
                else
                {
                    RptSales2s = RptSales2s.Where(s => s.StoreFranchise == "TMT");
                }
            }
            else if (Parobj.Franchise == "SUB-FRANCHISE")
            {
                if (Parobj.TMT && (Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0]))))
                {
                    RptSalesAlls = RptSalesAlls.Where(s => s.StoreFranchise == "SUB-FRANCHISE");
                }
                else if (Parobj.RMS && (string.IsNullOrEmpty(storeVal[0])) || Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0])))//|| storeVal[0] =="0" || RMS && (TMT =false
                {
                    RptSales = RptSales.Where(s => s.StoreFranchise == "SUB-FRANCHISE");
                }
                else if (Parobj.TMT && !string.IsNullOrEmpty(storeVal[0]))
                {
                    RptSalesAxts = RptSalesAxts.Where(s => s.StoreFranchise == "FRANCHISE");
                }
                else
                {
                    RptSales2s = RptSales2s.Where(s => s.StoreFranchise == "SUB-FRANCHISE");
                }
            }
            if (Parobj.Store != null)
            {
                storeVal = Parobj.Store.Split(':');
                if (Parobj.Store != "0")
                {
                    if (Parobj.TMT && (Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0]))))
                    {
                        RptSalesAlls = RptSalesAlls
                                                   .Where(s =>
                                                    ((s.StoreIdD.ToString() == storeVal[0].ToString()) ||
                                                    (s.StoreIdR.ToString() == storeVal[1].ToString())));
                        //.Where(s =>
                        // (storeVal[1] != null && s.StoreIdD.ToString() == storeVal[0].ToString()) ||
                        // (storeVal[0] != null && s.StoreIdR.ToString() == storeVal[1].ToString()));
                    }
                    else if (Parobj.RMS && (string.IsNullOrEmpty(storeVal[0])) || Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0])))//|| storeVal[0] =="0" || RMS && (TMT =false
                    {
                        RptSales = RptSales.Where(s => s.StoreId == int.Parse(storeVal[1]));
                    }
                    else if (Parobj.TMT && !string.IsNullOrEmpty(storeVal[0]))
                    {
                        RptSalesAxts = RptSalesAxts.Where(s => s.StoreId == storeVal[0]);
                    }
                    else
                    {
                        RptSales2s = RptSales2s.Where(s => s.StoreId == int.Parse(storeVal[1]));
                    }
                }
            }
            DateTime currentDate = DateTime.Now;
            DateTime lastWeekStartDate = currentDate.AddDays(-7);
            DateTime lastMonthDate = currentDate.AddMonths(-1);
            DateTime firstDayOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime lastDayOfLastMonth = firstDayOfCurrentMonth.AddDays(-1);
            //if (Yesterday)
            //{
            //    RptSales = RptSales.Where(s => s.TransDate >= firstDayOfCurrentMonth && s.TransDate <= lastDayOfLastMonth);
            //    RptSalesAxts = RptSalesAxts.Where(s => s.TransDate >= firstDayOfCurrentMonth && s.TransDate <= lastDayOfLastMonth);
            //}
            //else
            //{
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime startDateTime = Convert.ToDateTime(Parobj.startDate, new CultureInfo("en-GB"));
                RptSales = RptSales.Where(s => s.TransDate.HasValue && s.TransDate.Value.Date >= startDateTime.Date);
                RptSalesAxts = RptSalesAxts.Where(s => s.TransDate.HasValue && s.TransDate.Value.Date >= startDateTime);
                RptSalesAlls = RptSalesAlls.Where(s => s.TransDate.HasValue && s.TransDate.Value.Date >= startDateTime.Date);
                RptSales2s = RptSales2s.Where(s => s.TransDate.HasValue && s.TransDate.Value.Date >= startDateTime.Date);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime endDateTime = Convert.ToDateTime(Parobj.endDate, new CultureInfo("en-GB"));
                RptSales = RptSales.Where(s => s.TransDate.HasValue && s.TransDate.Value.Date <= endDateTime.Date);
                RptSalesAxts = RptSalesAxts.Where(s => s.TransDate.HasValue && s.TransDate.Value.Date <= endDateTime);
                RptSalesAlls = RptSalesAlls.Where(s => s.TransDate.HasValue && s.TransDate.Value.Date <= endDateTime.Date);
                RptSales2s = RptSales2s.Where(s => s.TransDate.HasValue && s.TransDate.Value.Date <= endDateTime.Date);
            }
            if (Parobj.Department != "0")
            {
                if (Parobj.TMT && (Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0]))))
                {
                    RptSalesAlls = RptSalesAlls.Where(s => s.DpId == Parobj.Department);
                }
                else if (Parobj.RMS && (string.IsNullOrEmpty(storeVal[0])) || Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0])))//|| storeVal[0] =="0" || RMS && (TMT =false
                {
                    RptSales = RptSales.Where(s => s.DpId == Parobj.Department);
                }
                else if (Parobj.TMT && !string.IsNullOrEmpty(storeVal[0]))
                {
                    RptSalesAxts = RptSalesAxts.Where(s => s.DpId == Parobj.Department);
                }
                else
                {
                    RptSales2s = RptSales2s.Where(s => s.StoreId == int.Parse(storeVal[1]));
                }
            }
            if (!string.IsNullOrEmpty(Parobj.ItemLookupCodeTxt))
            {
                if (Parobj.TMT && (Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0]))))
                {
                    RptSalesAlls = RptSalesAlls.Where(s => s.ItemLookupCode.Contains(Parobj.ItemLookupCodeTxt));
                }
                else if (Parobj.RMS && (string.IsNullOrEmpty(storeVal[0])) || Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0])))//|| storeVal[0] =="0" || RMS && (TMT =false
                {
                    RptSales = RptSales.Where(s => s.ItemLookupCode.Contains(Parobj.ItemLookupCodeTxt));
                }
                else if (Parobj.TMT && !string.IsNullOrEmpty(storeVal[0]))
                {
                    RptSalesAxts = RptSalesAxts.Where(s => s.ItemLookupCode == Parobj.ItemLookupCodeTxt);
                }
                else
                {
                    RptSales2s = RptSales2s.Where(s => s.ItemLookupCode.Contains(Parobj.ItemLookupCodeTxt));
                }
            }
            if (!string.IsNullOrEmpty(Parobj.ItemNameTxt))
            {
                if (Parobj.TMT && (Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0]))))
                {
                    RptSalesAlls = RptSalesAlls.Where(s => s.ItemName.Contains(Parobj.ItemNameTxt));
                }
                else if (Parobj.RMS && (string.IsNullOrEmpty(storeVal[0])) || Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0])))//|| storeVal[0] =="0" || RMS && (TMT =false
                {
                    RptSales = RptSales.Where(s => s.ItemName.Contains(Parobj.ItemNameTxt));
                }
                else if (Parobj.TMT && !string.IsNullOrEmpty(storeVal[0]))
                {
                    RptSalesAxts = RptSalesAxts.Where(s => s.ItemName.Contains(Parobj.ItemNameTxt));
                }
                else
                {
                    RptSales2s = RptSales2s.Where(s => s.ItemName.Contains(Parobj.ItemNameTxt));
                }
            }
            if (Parobj.Supplier != "0")
            {
                if (Parobj.TMT && (Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0]))))
                {
                    RptSalesAlls = RptSalesAlls.Where(s => s.SupplierCode.ToString() == Parobj.Supplier.ToString());
                }
                else if (Parobj.RMS && (string.IsNullOrEmpty(storeVal[0])) || Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0])))//|| storeVal[0] =="0" || RMS && (TMT =false
                {
                    RptSales = RptSales.Where(s => s.SupplierCode.ToString() == Parobj.Supplier.ToString());
                }
                else if (Parobj.TMT && !string.IsNullOrEmpty(storeVal[0]))
                {
                    RptSalesAxts = RptSalesAxts.Where(s => s.SupplierCode.ToString() == Parobj.Supplier.ToString());
                }
                else
                {
                    RptSales2s = RptSales2s.Where(s => s.SupplierCode.ToString() == Parobj.Supplier.ToString());
                }
            }
            // Dynamic GroupBy based on selected values
            IQueryable<dynamic> reportData1;
            if (Parobj.TMT && (Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0]))))
            {
                // make if Parobj.VTotalSales isnot true
                if (Parobj.VTotalSales && (Parobj.VStoreName || Parobj.VItemLookupCode || Parobj.VDepartment || Parobj.VSupplierName || Parobj.VItemName || Parobj.VPerDay || Parobj.VPerMon || Parobj.VPerMonYear || Parobj.VPerYear || Parobj.VTransactionNumber || Parobj.VFranchise || Parobj.VCost || Parobj.VPrice))
                {
                    reportData1 = RptSalesAlls
                    .GroupBy(d => new
                    {
                        StoreName = Parobj.VStoreName ? d.StoreName : null,
                        ItemLookupCode = Parobj.VItemLookupCode ? d.ItemLookupCode : null,
                        DpName = Parobj.VDepartment ? d.DpName : null,
                        SupplierName = Parobj.VSupplierName ? d.SupplierName : null,
                        ItemName = Parobj.VItemName ? d.ItemName : null,
                        Date = Parobj.VPerDay ? (DateTime?)d.TransDate.Value.Date : null,
                        PerMonth = (Parobj.VPerMon || Parobj.VPerMonYear) ? d.ByMonth : null,
                        PerYear = (Parobj.VPerYear || Parobj.VPerMonYear) ? d.ByYear : null,
                        TransactionNumber = Parobj.VTransactionNumber ? d.TransactionNumber : null,
                        StoreFranchise = Parobj.VFranchise ? d.StoreFranchise : null,
                        Cost = Parobj.VCost ? d.Cost : 0,
                        Price = Parobj.VPrice ? d.Price : 0
                    })
                    .Where(g => !(g.Key.StoreName == null && g.Key.ItemLookupCode == null &&
                    g.Key.DpName == null && g.Key.SupplierName == null && g.Key.ItemName == null &&
                    g.Key.Date == null && g.Key.PerMonth == null && g.Key.PerYear == null && g.Key.TransactionNumber == null && g.Key.StoreFranchise == null
                    && g.Key.Cost == 0 && g.Key.Price == 0
                   )) // Exclude groups where both keys are null
                    .Select(g => new
                    {
                        Total = g.Sum(d => d.TotalSales),
                        TotalQty = g.Sum(d => d.Qty),
                        TotalCost = g.Sum(d => d.Cost),
                        Price = g.Key.Price,
                        Cost = g.Key.Cost,
                        TotalTax = g.Sum(d => d.Tax),
                        TotalSalesTax = g.Sum(d => d.TotalSalesTax),
                        TotalSalesWithoutTax = g.Sum(d => d.TotalSalesWithoutTax),
                        TotalCostQty = g.Sum(d => d.TotalCostQty),
                        StoreName = g.Key.StoreName,
                        ItemLookupCodeTxt = g.Key.ItemLookupCode,
                        DpName = g.Key.DpName,
                        SupplierName = g.Key.SupplierName,
                        ItemName = g.Key.ItemName,
                        TransactionNumber = g.Key.TransactionNumber,
                        StoreFranchise = g.Key.StoreFranchise,
                        PerDay = g.Key.Date != null ? g.Key.Date.Value.ToString("yyyy-MM-dd") : string.Empty,
                        PerMonth = g.Key.PerMonth,
                        PerYear = g.Key.PerYear,
                        //TotalSales = g.Key.TotalSales
                    });
                }
                else
                {
                    reportData1 = RptSalesAlls
                      .GroupBy((RptSalesAll d) => new { })
                      .Select(g => new
                      {
                          Total = g.Sum(d => d.TotalSales),
                          TotalQty = g.Sum(d => d.Qty),
                          TotalCost = g.Sum(d => d.Cost),
                          TotalTax = g.Sum(d => d.Tax),
                          TotalSalesTax = g.Sum(d => d.TotalSalesTax),
                          TotalSalesWithoutTax = g.Sum(d => d.TotalSalesWithoutTax),
                          TotalCostQty = g.Sum(d => d.TotalCostQty),
                      });
                }
            }
            else if (Parobj.RMS && (string.IsNullOrEmpty(storeVal[0])) || Parobj.RMS && (!string.IsNullOrEmpty(storeVal[0])))//|| storeVal[0] =="0" || RMS && (TMT =false
            {
                if (Parobj.VTotalSales && (Parobj.VStoreName || Parobj.VItemLookupCode || Parobj.VDepartment || Parobj.VSupplierName || Parobj.VItemName || Parobj.VPerDay || Parobj.VPerMon || Parobj.VPerMonYear || Parobj.VPerYear || Parobj.VTransactionNumber || Parobj.VFranchise || Parobj.VCost || Parobj.VPrice))
                {
                    reportData1 = RptSales
                    .GroupBy(d => new
                    {
                        StoreName = Parobj.VStoreName ? d.StoreName : null,
                        ItemLookupCode = Parobj.VItemLookupCode ? d.ItemLookupCode : null,
                        DpName = Parobj.VDepartment ? d.DpName : null,
                        SupplierName = Parobj.VSupplierName ? d.SupplierName : null,
                        ItemName = Parobj.VItemName ? d.ItemName : null,
                        Date = Parobj.VPerDay ? (DateTime?)d.TransDate.Value.Date : null,
                        PerMonth = (Parobj.VPerMon || Parobj.VPerMonYear) ? d.ByMonth : null,
                        PerYear = (Parobj.VPerYear || Parobj.VPerMonYear) ? d.ByYear : null,
                        TransactionNumber = Parobj.VTransactionNumber ? d.TransactionNumber : null,
                        StoreFranchise = Parobj.VFranchise ? d.StoreFranchise : null,
                        Cost = Parobj.VCost ? d.Cost : 0,
                        Price = Parobj.VPrice ? d.Price : 0
                    })
                    .Where(g => !(g.Key.StoreName == null && g.Key.ItemLookupCode == null &&
                    g.Key.DpName == null && g.Key.SupplierName == null && g.Key.ItemName == null &&
                    g.Key.Date == null && g.Key.PerMonth == null && g.Key.PerYear == null && g.Key.TransactionNumber == null && g.Key.StoreFranchise == null
                    && g.Key.Cost == 0 && g.Key.Price == 0
                   )) // Exclude groups where both keys are null
                    .Select(g => new
                    {
                        Total = g.Sum(d => d.TotalSales),
                        TotalQty = g.Sum(d => d.Qty),
                        TotalCost = g.Sum(d => d.Cost),
                        Price = g.Key.Price,
                        TotalTax = g.Sum(d => d.Tax),
                        TotalSalesTax = g.Sum(d => d.TotalSalesTax),
                        TotalSalesWithoutTax = g.Sum(d => d.TotalSalesWithoutTax),
                        TotalCostQty = g.Sum(d => d.TotalCostQty),
                        Cost = g.Key.Cost,
                        StoreName = g.Key.StoreName,
                        ItemLookupCodeTxt = g.Key.ItemLookupCode,
                        DpName = g.Key.DpName,
                        SupplierName = g.Key.SupplierName,
                        ItemName = g.Key.ItemName,
                        TransactionNumber = g.Key.TransactionNumber,
                        StoreFranchise = g.Key.StoreFranchise,
                        PerDay = g.Key.Date != null ? g.Key.Date.Value.ToString("yyyy-MM-dd") : string.Empty,
                        PerMonth = g.Key.PerMonth,
                        PerYear = g.Key.PerYear,
                        //TotalSales = g.Key.TotalSales
                    });
                }
                else
                {
                    reportData1 = RptSales
                      .GroupBy((RptSale d) => new { })
                      .Select(g => new
                      {
                          Total = g.Sum(d => d.TotalSales),
                          TotalQty = g.Sum(d => d.Qty),
                          TotalCost = g.Sum(d => d.Cost),
                          TotalTax = g.Sum(d => d.Tax),
                          TotalSalesTax = g.Sum(d => d.TotalSalesTax),
                          TotalSalesWithoutTax = g.Sum(d => d.TotalSalesWithoutTax),
                          TotalCostQty = g.Sum(d => d.TotalCostQty),
                      });
                }
            }
            else if (Parobj.TMT)
            {
                if (Parobj.VTotalSales && (Parobj.VStoreName || Parobj.VItemLookupCode || Parobj.VDepartment || Parobj.VSupplierName || Parobj.VItemName || Parobj.VPerDay || Parobj.VPerMon || Parobj.VPerMonYear || Parobj.VPerYear || Parobj.VTransactionNumber || Parobj.VFranchise || Parobj.VCost || Parobj.VPrice))
                {
                    reportData1 = RptSalesAxts
                    .GroupBy(d => new
                    {
                        StoreName = Parobj.VStoreName ? d.StoreName : null,
                        ItemLookupCode = Parobj.VItemLookupCode ? d.ItemLookupCode : null,
                        DpName = Parobj.VDepartment ? d.DpName : null,
                        SupplierName = Parobj.VSupplierName ? d.SupplierName : null,
                        ItemName = Parobj.VItemName ? d.ItemName : null,
                        Date = Parobj.VPerDay ? (DateTime?)d.TransDate.Value.Date : null,
                        PerMonth = (Parobj.VPerMon || Parobj.VPerMonYear) ? d.ByMonth : null,
                        PerYear = (Parobj.VPerYear || Parobj.VPerMonYear) ? d.ByYear : null,
                        TransactionNumber = Parobj.VTransactionNumber ? d.TransactionNumber : null,
                        StoreFranchise = Parobj.VFranchise ? d.StoreFranchise : null,
                        Cost = Parobj.VCost ? d.Cost : 0,
                        Price = Parobj.VPrice ? d.Price : 0
                    })
                    .Where(g => !(g.Key.StoreName == null && g.Key.ItemLookupCode == null &&
                    g.Key.DpName == null && g.Key.SupplierName == null && g.Key.ItemName == null &&
                    g.Key.Date == null && g.Key.PerMonth == null && g.Key.PerYear == null && g.Key.TransactionNumber == null && g.Key.StoreFranchise == null
                    && g.Key.Cost == 0 && g.Key.Price == 0
                   )) // Exclude groups where both keys are null
                    .Select(g => new
                    {
                        Total = g.Sum(d => d.TotalSales),
                        TotalQty = g.Sum(d => d.Qty),
                        TotalCost = g.Sum(d => d.Cost),
                        Price = g.Key.Price,
                        Cost = g.Key.Cost,
                        TotalTax = g.Sum(d => d.Tax),
                        TotalSalesTax = g.Sum(d => d.TotalSalesTax),
                        TotalSalesWithoutTax = g.Sum(d => d.TotalSalesWithoutTax),
                        TotalCostQty = g.Sum(d => d.TotalCostQty),
                        StoreName = g.Key.StoreName,
                        ItemLookupCodeTxt = g.Key.ItemLookupCode,
                        DpName = g.Key.DpName,
                        SupplierName = g.Key.SupplierName,
                        ItemName = g.Key.ItemName,
                        TransactionNumber = g.Key.TransactionNumber,
                        StoreFranchise = g.Key.StoreFranchise,
                        PerDay = g.Key.Date != null ? g.Key.Date.Value.ToString("yyyy-MM-dd") : string.Empty,
                        PerMonth = g.Key.PerMonth,
                        PerYear = g.Key.PerYear,
                        //TotalSales = g.Key.TotalSales
                    });
                }
                else
                {
                    reportData1 = RptSalesAxts
                      .GroupBy((RptSalesAxt d) => new { })
                      .Select(g => new
                      {
                          Total = g.Sum(d => d.TotalSales),
                          TotalQty = g.Sum(d => d.Qty),
                          TotalCost = g.Sum(d => d.Cost),
                          TotalTax = g.Sum(d => d.Tax),
                          TotalSalesTax = g.Sum(d => d.TotalSalesTax),
                          TotalSalesWithoutTax = g.Sum(d => d.TotalSalesWithoutTax),
                          TotalCostQty = g.Sum(d => d.TotalCostQty),
                      });
                }
            }
            else if (Parobj.DBbefore)
            {
                if (Parobj.VTotalSales && (Parobj.VStoreName || Parobj.VItemLookupCode || Parobj.VDepartment || Parobj.VSupplierName || Parobj.VItemName || Parobj.VPerDay || Parobj.VPerMon || Parobj.VPerMonYear || Parobj.VPerYear || Parobj.VTransactionNumber || Parobj.VFranchise || Parobj.VCost || Parobj.VPrice))
                {
                    reportData1 = RptSales2s
                    .GroupBy(d => new
                    {
                        StoreName = Parobj.VStoreName ? d.StoreName : null,
                        ItemLookupCode = Parobj.VItemLookupCode ? d.ItemLookupCode : null,
                        DpName = Parobj.VDepartment ? d.DpName : null,
                        SupplierName = Parobj.VSupplierName ? d.SupplierName : null,
                        ItemName = Parobj.VItemName ? d.ItemName : null,
                        Date = Parobj.VPerDay ? (DateTime?)d.TransDate.Value.Date : null,
                        PerMonth = (Parobj.VPerMon || Parobj.VPerMonYear) ? d.ByMonth : null,
                        PerYear = (Parobj.VPerYear || Parobj.VPerMonYear) ? d.ByYear : null,
                        TransactionNumber = Parobj.VTransactionNumber ? d.TransactionNumber : null,
                        StoreFranchise = Parobj.VFranchise ? d.StoreFranchise : null,
                        Cost = Parobj.VCost ? d.Cost : 0,
                        Price = Parobj.VPrice ? d.Price : 0
                    })
                    .Where(g => !(g.Key.StoreName == null && g.Key.ItemLookupCode == null &&
                    g.Key.DpName == null && g.Key.SupplierName == null && g.Key.ItemName == null &&
                    g.Key.Date == null && g.Key.PerMonth == null && g.Key.PerYear == null && g.Key.TransactionNumber == null && g.Key.StoreFranchise == null
                    && g.Key.Cost == 0 && g.Key.Price == 0
                   )) // Exclude groups where both keys are null
                    .Select(g => new
                    {
                        Total = g.Sum(d => d.TotalSales),
                        TotalQty = g.Sum(d => d.Qty),
                        TotalCost = g.Sum(d => d.Cost),
                        Price = g.Key.Price,
                        Cost = g.Key.Cost,
                        TotalTax = g.Sum(d => d.Tax),
                        TotalSalesTax = g.Sum(d => d.TotalSalesTax),
                        TotalSalesWithoutTax = g.Sum(d => d.TotalSalesWithoutTax),
                        TotalCostQty = g.Sum(d => d.TotalCostQty),
                        StoreName = g.Key.StoreName,
                        ItemLookupCodeTxt = g.Key.ItemLookupCode,
                        DpName = g.Key.DpName,
                        SupplierName = g.Key.SupplierName,
                        ItemName = g.Key.ItemName,
                        TransactionNumber = g.Key.TransactionNumber,
                        StoreFranchise = g.Key.StoreFranchise,
                        PerDay = g.Key.Date != null ? g.Key.Date.Value.ToString("yyyy-MM-dd") : string.Empty,
                        PerMonth = g.Key.PerMonth,
                        PerYear = g.Key.PerYear,
                        //TotalSales = g.Key.TotalSales
                    });
                }
                else
                {
                    reportData1 = RptSales2s
                      .GroupBy((RptSales2 d) => new { })
                      .Select(g => new
                      {
                          Total = g.Sum(d => d.TotalSales),
                          TotalQty = g.Sum(d => d.Qty),
                          TotalCost = g.Sum(d => d.Cost),
                          TotalTax = g.Sum(d => d.Tax),
                          TotalSalesTax = g.Sum(d => d.TotalSalesTax),
                          TotalSalesWithoutTax = g.Sum(d => d.TotalSalesWithoutTax),
                          TotalCostQty = g.Sum(d => d.TotalCostQty),
                      });
                }
            }
            // if Not RMS or TMT
            else
            {
                return View();
            }
            ViewBag.Data = reportData1;
            //TempData["Al"] = " „ «·Õ›Ÿ »›÷· «··Â";
            //var reportData1 = ViewBag.Data as IEnumerable<dynamic>;
            exportAfterClick = true;
            if (exportAfterClick == false)
            {
                return View();
            }

            else
            {
                // return View();
                return ExportReportData(reportData1, Parobj);
            }
            //TempData["Al"] = " „ «·Õ›Ÿ »›÷· «··Â";

        }

        public IActionResult CheckExportStatus()
        {
            // Read the session variable
            var exportStatus = HttpContext.Session.GetString("ExportStatus");
            if (exportStatus == "complete")
            {
                HttpContext.Session.Remove("ExportStatus");
                return Content("complete");
            }
            return Content(exportStatus ?? "unknown");
        }

        private IActionResult ExportReportData(IEnumerable<dynamic> reportData1, SalesParameters Parobj)
        {
            string startDate = Parobj.startDate;
            string endDate = Parobj.endDate;
            string store = Parobj.Store;
            string department = Parobj.Department;
            string supplier = Parobj.Supplier;
            bool exportAfterClick = Parobj.ExportAfterClick;
            string[] selectedItems = Parobj.SelectedItems;
            bool vPerDay = Parobj.VPerDay;
            bool vPerMonYear = Parobj.VPerMonYear;
            bool vPerMon = Parobj.VPerMon;
            bool vPerYear = Parobj.VPerYear;
            bool vQty = Parobj.VQty;
            bool vPrice = Parobj.VPrice;
            bool vStoreName = Parobj.VStoreName;
            bool vDepartment = Parobj.VDepartment;
            bool VTotalSales = Parobj.VTotalSales;
            bool vTotalCost = Parobj.VTotalCost;
            bool vTotalTax = Parobj.VTotalTax;
            bool VTotalSalesTax = Parobj.VTotalSalesTax;
            bool VTotalSalesWithoutTax = Parobj.VTotalSalesWithoutTax;
            bool vTotalCostQty = Parobj.VTotalCostQty;
            bool vCost = Parobj.VCost;
            bool vItemLookupCode = Parobj.VItemLookupCode;
            bool vItemName = Parobj.VItemName;
            bool vSupplierId = Parobj.VSupplierId;
            bool vSupplierName = Parobj.VSupplierName;
            string franchise = Parobj.Franchise;
            bool vTransactionNumber = Parobj.VTransactionNumber;
            bool vFranchise = Parobj.VFranchise;
            int? monthToFilter = Parobj.MonthToFilter;
            string ItemLookupCodeTxt = Parobj.ItemLookupCodeTxt;
            string itemNameTxt = Parobj.ItemNameTxt;
            bool tmt = Parobj.TMT;
            bool rms = Parobj.RMS;
            bool dbBefore = Parobj.DBbefore;
            bool yesterday = Parobj.Yesterday;
            HttpContext.Session.SetString("ExportStatus", "started");
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SalesReport");
                // Add header row
                int columnCount = 1; // Start with the first column (A)

                if (Parobj.VPerYear || Parobj.VPerMonYear)
                    worksheet.Cells[1, columnCount++].Value = "Date Per Year";
                if (Parobj.VPerMon || Parobj.VPerMonYear)
                    worksheet.Cells[1, columnCount++].Value = "Date Per Month";
                if (Parobj.VPerDay)
                    worksheet.Cells[1, columnCount++].Value = "Date Per Day";
                if (Parobj.VDepartment)
                    worksheet.Cells[1, columnCount++].Value = "Department";
                if (Parobj.VStoreName)
                    worksheet.Cells[1, columnCount++].Value = "Store Name";
                if (Parobj.VItemLookupCode)
                    worksheet.Cells[1, columnCount++].Value = "Item Lookup Code";
                if (Parobj.VItemName)
                    worksheet.Cells[1, columnCount++].Value = "Item Name";
                if (Parobj.VSupplierName)
                    worksheet.Cells[1, columnCount++].Value = "Supplier Name";
                if (Parobj.VFranchise)
                    worksheet.Cells[1, columnCount++].Value = "Franchise";
                if (Parobj.VTransactionNumber)
                    worksheet.Cells[1, columnCount++].Value = "Transaction Number";
                if (Parobj.VQty)
                    worksheet.Cells[1, columnCount++].Value = "Total Qty";
                if (Parobj.VPrice)
                    worksheet.Cells[1, columnCount++].Value = "Max Price";
                if (Parobj.VCost)
                    worksheet.Cells[1, columnCount++].Value = "Cost";
                if (Parobj.VTotalSales)
                    worksheet.Cells[1, columnCount++].Value = "Total Sales";
                if (Parobj.VTotalCost)
                    worksheet.Cells[1, columnCount++].Value = "Total Cost";
                if (Parobj.VTotalTax)
                    worksheet.Cells[1, columnCount++].Value = "Tax";
                if (Parobj.VTotalSalesTax)
                    worksheet.Cells[1, columnCount++].Value = "Total Sales Tax";
                if (Parobj.VTotalSalesWithoutTax)
                    worksheet.Cells[1, columnCount++].Value = "Total Sales Without Tax";
                if (Parobj.VTotalCostQty)
                    worksheet.Cells[1, columnCount++].Value = "Total Quantity Cost";
                // Set header style
                using (var range = worksheet.Cells[1, 1, 1, columnCount])
                {
                    range.Style.Font.Bold = true;

                }

                int row = 2;
                foreach (var item in reportData1)
                {
                    columnCount = 1; // Reset column count for each row

                    if (Parobj.VPerYear || Parobj.VPerMonYear)
                        worksheet.Cells[row, columnCount++].Value = item.PerYear;
                    if (Parobj.VPerMon || Parobj.VPerMonYear)
                        worksheet.Cells[row, columnCount++].Value = item.PerMonth;
                    if (Parobj.VPerDay)
                        worksheet.Cells[row, columnCount++].Value = item.PerDay;
                    if (Parobj.VDepartment)
                        worksheet.Cells[row, columnCount++].Value = item.DpName;
                    if (Parobj.VStoreName)
                        worksheet.Cells[row, columnCount++].Value = item.StoreName;
                    if (Parobj.VItemLookupCode)
                        worksheet.Cells[row, columnCount++].Value = item.ItemLookupCodeTxt;
                    if (Parobj.VItemName)
                        worksheet.Cells[row, columnCount++].Value = item.ItemName;
                    if (Parobj.VSupplierName)
                        worksheet.Cells[row, columnCount++].Value = item.SupplierName;
                    if (Parobj.VFranchise)
                        worksheet.Cells[row, columnCount++].Value = item.StoreFranchise;
                    if (Parobj.VTransactionNumber)
                        worksheet.Cells[row, columnCount++].Value = item.TransactionNumber;
                    if (Parobj.VQty)
                        worksheet.Cells[row, columnCount++].Value = item.TotalQty;
                    if (Parobj.VPrice)
                        worksheet.Cells[row, columnCount++].Value = item.Price;
                    if (Parobj.VCost)
                        worksheet.Cells[row, columnCount++].Value = item.Cost;
                    if (Parobj.VTotalSales)
                        worksheet.Cells[row, columnCount++].Value = item.Total;
                    if (Parobj.VTotalCost)
                        worksheet.Cells[row, columnCount++].Value = item.TotalCost;
                    if (Parobj.VTotalTax)
                        worksheet.Cells[row, columnCount++].Value = item.TotalTax;
                    if (Parobj.VTotalSalesTax)
                        worksheet.Cells[row, columnCount++].Value = item.TotalSalesTax;
                    if (Parobj.VTotalSalesWithoutTax)
                        worksheet.Cells[row, columnCount++].Value = item.TotalSalesWithoutTax;
                    if (Parobj.VTotalCostQty)
                        worksheet.Cells[row, columnCount++].Value = item.TotalCostQty;
                    row++;
                }
                // Auto fit columns
                worksheet.Cells.AutoFitColumns();
                // Save the file
                var stream = new MemoryStream();
                package.SaveAs(stream);
                HttpContext.Session.SetString("ExportStatus", "complete");
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalesReport.xlsx");
            }
        }
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> LogOut()
        {
            // Sign out the user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Set a TempData variable to indicate logout
            TempData["IsLoggedOut"] = true;

            // Clear session on logout
            HttpContext.Session.Clear();

            // Prevent caching by setting appropriate HTTP headers
            //Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            //Response.Headers.Add("Pragma", "no-cache");
            //Response.Headers.Add("Expires", "0");
            try
            {
                if (!Response.Headers.ContainsKey("Cache-Control"))
                {
                    Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                }

                if (!Response.Headers.ContainsKey("Pragma"))
                {
                    Response.Headers.Add("Pragma", "no-cache");
                }

                if (!Response.Headers.ContainsKey("Expires"))
                {
                    Response.Headers.Add("Expires", "0");
                }

                return RedirectToAction("Login", "Login");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Exception in LogOut action: {ex.Message}");
                return RedirectToAction("Login", "Login");
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult index1()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
