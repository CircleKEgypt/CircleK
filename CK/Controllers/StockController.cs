using CK.Model;
using CK.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using Polly;
using System;
using ClosedXML.Excel;
using System.Drawing;
using Microsoft.CodeAnalysis.Elfie.Model.Structures;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using Microsoft.Data.SqlClient;
using DocumentFormat.OpenXml.Features;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using DocumentFormat.OpenXml.InkML;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using iText.Pdfua.Checkers.Utils;
namespace CK.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        AxdbContext Axdb = new AxdbContext();
        DataCenterContext db = new DataCenterContext();
        CkproUsersContext db2 = new CkproUsersContext();
        CkhelperdbContext db3 = new CkhelperdbContext();
        SalesParameters Parobj = new SalesParameters();
        DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
        private readonly ILogger<StockController> _logger;
        public StockController(ILogger<StockController> logger)
        {
            _logger = logger;
        }
        bool exported = false;
        [HttpGet]
        public IActionResult SupplierItemBarcode()
        {
            var username = HttpContext.Session.GetString("Username");
            var Password = HttpContext.Session.GetString("Password");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Isuser = HttpContext.Session.GetString("isUsername");
            ViewBag.Username = username;
            ViewBag.Password = Password;
            ViewBag.Role = Role;
            ViewBag.StoreIdRms = StoreIdRms;
            ViewBag.StoreIddynamic = StoreIddynamic;
            ViewBag.isUsername = Isuser;
            DataCenterContext db = new DataCenterContext();
            ViewBag.ItemLookupCodeTxt = Parobj.ItemLookupCodeTxt;

                return View();

        }
        [HttpPost]
        public IActionResult SupplierItemBarcode(SalesParameters Parobj)
        {
            var username = HttpContext.Session.GetString("Username");
            var Password = HttpContext.Session.GetString("Password");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Isuser = HttpContext.Session.GetString("isUsername");
            ViewBag.Username = username;
            ViewBag.Password = Password;
            ViewBag.Role = Role;
            ViewBag.StoreIdRms = StoreIdRms;
            ViewBag.StoreIddynamic = StoreIddynamic;
            ViewBag.isUsername = Isuser;
            DataCenterContext db = new DataCenterContext();
            ViewBag.ItemLookupCodeTxt = Parobj.ItemLookupCodeTxt;
            // IQueryable<RptSalesAxt> RptSalesAxts = db.RptSalesAxts;
            //IQueryable<CK.Models.Supplierpage> reportData1 = Enumerable.Empty<CK.Models.Supplierpage>().AsQueryable(); // Initialize with an empty queryable
            //ViewBag.ItemLookupCodeTxt = ItemLookupCodeTxt;
            //try
            //{
            //    if (!string.IsNullOrEmpty(ItemLookupCodeTxt))
            //    {
            //        reportData1 = db.Supplierpages
            //            .Where(d => d.ItemLookupCode == ItemLookupCodeTxt &&(d.StoreId==StoreIddynamic ||d.StoreId==StoreIdRms))
            //            .GroupBy(d => new { d.SupplierName, d.ItemLookupCode, d.ItemName })
            //            .Select(g => new CK.Models.Supplierpage // Corrected the projection to maintain the same type
            //            {
            //                SupplierName = g.Key.SupplierName,
            //                ItemLookupCode = g.Key.ItemLookupCode,
            //                ItemName = g.Key.ItemName,
            //            });
            //    }

            //    var reportDataList = reportData1.ToList();
            //return View(reportDataList);
            //}
            //catch (Exception ex){
            //    return View();
            //}
            Parobj.SupplierofItem = true;
            Parobj.actionValue = 1;
            Parobj.TMT = true;
            Parobj.MainStock = true;
            Parobj.VSupplierName = true;
            Parobj.VItemName = true;
            Parobj.VItemLookupCode = true;
            Parobj.Store = "0";
            if (ViewBag.ItemLookupCodeTxt!=null)
            {
                return ExportToExcel(Parobj.AxdbConnection, Parobj);
            }
            else
            {
                return View();
            }

        }
        [HttpGet]
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            var Password = HttpContext.Session.GetString("Password");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Isuser = HttpContext.Session.GetString("isUsername");
            ViewBag.Username = username;
            ViewBag.Password = Password;
            ViewBag.Role = Role;
            ViewBag.StoreIdRms = StoreIdRms;
            ViewBag.StoreIddynamic = StoreIddynamic;
            ViewBag.isUsername = Isuser;
            DataCenterContext db = new DataCenterContext();
            CkproUsersContext db2 = new CkproUsersContext();
            CkhelperdbContext db3 = new CkhelperdbContext();
            AxdbContext Axdb = new AxdbContext();
            DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
            db.Database.SetCommandTimeout(7200);// Set the timeout in seconds

            if (username is null)
            {
                return RedirectToAction("Store", "Home");
            }
            bool isDmanager = db2.RptUsers.Any(s => s.Dmanager == username);
            bool isFmanager = db2.RptUsers.Any(s => s.Fmanager == username);
            bool isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));

            IQueryable<Storeuser> query;
            if (isDmanager || isUsername || isFmanager)
            {
                // If the username matches either the Dmanager or the Username, filter the stores accordingly
                query = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username || s.Fmanager == username);
            }
            else
            {
                // If neither condition is met, display all stores
                query = db2.Storeusers;
            }
            ViewBag.VBStore = query
    .GroupBy(m => m.Name)
    .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
    .OrderBy(m => m.Name)
    .ToList();
            ViewBag.VBDepartment = Axdb.Ecorescategories
                                                 .Select(g=> g.Name )
                                                 .Distinct()
                                                 .ToList();

            //Supplier List Text=SupplierName , Value = Code 
            ViewBag.VBSupplier = db.RptSuppliers
                                                 .Select(g => g.SupplierName )
                                                 .ToList();

            
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
            DataCenterContext db = new DataCenterContext();
            CkproUsersContext db2 = new CkproUsersContext();
            CkhelperdbContext db3 = new CkhelperdbContext();
            AxdbContext Axdb = new AxdbContext();
            DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
            db.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            db3.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            db4.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            var username = HttpContext.Session.GetString("Username");
            var Password = HttpContext.Session.GetString("Password");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Isuser = HttpContext.Session.GetString("isUsername");
            ViewBag.Username = username;
            ViewBag.Password = Password;
            ViewBag.Role = Role;
            ViewBag.StoreIdRms = StoreIdRms;
            ViewBag.StoreIddynamic = StoreIddynamic;
            ViewBag.isUsername = Isuser;
            Parobj.MainStock = true;
            if (username is null)
            {
                return RedirectToAction("Store", "Home");
            }
            bool isDmanager = db2.RptUsers.Any(s => s.Dmanager == username);
            bool isFmanager = db2.RptUsers.Any(s => s.Fmanager == username);
            bool isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));

            IQueryable<Storeuser> query;
            if (isDmanager || isUsername || isFmanager)
            {
                // If the username matches either the Dmanager or the Username, filter the stores accordingly
                query = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username || s.Fmanager == username);
            }
            else
            {
                // If neither condition is met, display all stores
                query = db2.Storeusers;
            }
            ViewBag.VBStore = query
    .GroupBy(m => m.Name)
    .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
    .OrderBy(m => m.Name)
    .ToList();
            ViewBag.VBDepartment = Axdb.Ecorescategories
                                                .Select(g => g.Name)
                                                .Distinct()
                                                .ToList();

            //Supplier List Text=SupplierName , Value = Code 
            ViewBag.VBSupplier = db.RptSuppliers
                                                .Select(g => g.SupplierName)
                                                .ToList();

            
            ViewBag.VBStoreFranchise = db.Stores
                 .Where(m => m.Franchise != null)
                 .Select(m => m.Franchise)
                 .Distinct()
                 .ToList();
            // Dynamic GroupBy based on selected values
            IQueryable<dynamic> reportData1;
            string[] storeVal = Parobj.Store.Split(':');
            // Call the ExportToExcel method
            if (username == "RichCut" || username == "RichCut_No" || username == "RichCut_Zayed")
            {
                return ExportToExcel(Parobj.RmsConnection, Parobj);
            }
            else if ((Parobj.RMS && Parobj.TMT == false) || (Parobj.RMS && storeVal[0] == "RMS") || Parobj.RichCut)
            {
                return ExportToExcel(Parobj.RmsConnection, Parobj);
            }
            else if (Parobj.RMS == false && Parobj.TMT || (storeVal.Length > 1 && storeVal[1] == "Dy"))
            {
                return ExportToExcel(Parobj.AxdbConnection, Parobj);
            }
            else if (Parobj.RMS && Parobj.TMT)
            {
                return ExportToExcel(Parobj.RmsConnection, Parobj);
            }
            else if (Parobj.DBbefore)
            {
                return ExportToExcel(Parobj.RmsBeforeConnection, Parobj);
            }
            // if Not RMS or TMT
            else
            {
                return View();
            }
            ViewBag.Data = reportData1;
            Parobj.exportAfterClick = true;
            if (Parobj.exportAfterClick == false)
            {
                return View();
            }

            else
            {
                return View();
            }

        }
        [HttpGet]
        public IActionResult StockFromBranch()
        {
            var username = HttpContext.Session.GetString("Username");
            var Password = HttpContext.Session.GetString("Password");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Isuser = HttpContext.Session.GetString("isUsername");
            ViewBag.Username = username;
            ViewBag.Password = Password;
            ViewBag.Role = Role;
            ViewBag.StoreIdRms = StoreIdRms;
            ViewBag.StoreIddynamic = StoreIddynamic;
            ViewBag.uuu = Isuser;
            db.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            IQueryable<RptSale> RptSales = db.RptSales;
            IQueryable<RptSalesAxt> RptSalesAxts = db.RptSalesAxts;
            IQueryable<RptSales2> RptSales2s = db4.RptSales2s;
            IQueryable<RptSalesAll> RptSalesAlls = db.RptSalesAlls;
            //Store List Text=StoreName , Value = StoreId
            bool isDmanager = db2.RptUsers.Any(s => s.Dmanager == username);
            bool isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));
            IQueryable<Storeuser> query;
            if (isDmanager || isUsername)
            {
                ViewBag.IsUsername = "true";
                // If the username matches either the Dmanager or the Username, filter the stores accordingly
                query = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username);
            }
            else
            {
                // If neither condition is met, display all stores
                query = db2.Storeusers.Where(x => x.Storenumber != "RMS");
            }
            ViewBag.VBStore = query
    .GroupBy(m => m.Name)
    .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
    .OrderBy(m => m.Name)
    .ToList();
            ViewBag.VBDepartment = Axdb.Ecorescategories
                                                  .Select(g => g.Name)
                                                  .Distinct()
                                                  .ToList();
            return View();
        }
        [HttpPost]
        public IActionResult StockFromBranch(SalesParameters Parobj)
        {
            DataCenterContext db = new DataCenterContext();
            CkproUsersContext db2 = new CkproUsersContext();
            CkhelperdbContext db3 = new CkhelperdbContext();
            DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
            Parobj.Server = HttpContext.Session.GetString("Server");
            db.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            db3.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            db4.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            var username = HttpContext.Session.GetString("Username");
            var Password = HttpContext.Session.GetString("Password");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Isuser = HttpContext.Session.GetString("isUsername");
            ViewBag.Username = username;
            ViewBag.Password = Password;
            ViewBag.Role = Role;
            ViewBag.StoreIdRms = StoreIdRms;
            ViewBag.StoreIddynamic = StoreIddynamic;
            ViewBag.uuu = Isuser;
            var Inventlocation = db2.Storeusers
                    .Where(s => s.Username == username)
                .Select(group => group.Inventlocation).ToList();
            bool isDmanager = db2.RptUsers.Any(s => s.Dmanager == username);
            bool isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));
            IQueryable<Storeuser> query;
            if (isDmanager || isUsername)
            {
                ViewBag.IsUsername = "true";
                // If the username matches either the Dmanager or the Username, filter the stores accordingly
                query = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username);
            }
            else
            {
                // If neither condition is met, display all stores
                query = db2.Storeusers.Where(x => x.Storenumber != "RMS");
            }
            ViewBag.VBStore = query
    .GroupBy(m => m.Name)
    .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
    .OrderBy(m => m.Name)
    .ToList();
            ViewBag.VBDepartment = Axdb.Ecorescategories
                                                .Select(g => g.Name)
                                                .Distinct()
                                                .ToList();
            // Dynamic GroupBy based on selected values
            IQueryable<dynamic> reportData1;
            string[] storeVal = Parobj.Store.Split(':');
            string serverIp = GetServerIpFromDatabase();

            // Step 2: Format the connection string dynamically
            //string connectionString = FormatConnectionString(serverIp);
            // Call the ExportToExcel method
            Parobj.exportAfterClick = true;
            Parobj.VQty = true;
            Parobj.VDepartment = true;
            Parobj.VItemName = true;
            Parobj.VStoreName = true;
            Parobj.StockFromBranch = true;
            //Parobj.VTransactionCount = true;
            Parobj.VItemLookupCode = true;
            Parobj.RMS = true;
            Parobj.TMT = true;
            if (Parobj.RMS && Parobj.TMT)
            {
                return ExportToExcel(Parobj.AxdbConnection, Parobj);
            }
            else
            {
                return View();
            }
            ViewBag.Data = reportData1;
            //  }
            //TempData["Al"] = "تم الحفظ بفضل الله";
            //var reportData1 = ViewBag.Data as IEnumerable<dynamic>;
            if (Parobj.exportAfterClick == false)
            {
                return View();
            }

            else
            {
                // return View();
                return View();
            }
            //TempData["Al"] = "تم الحفظ بفضل الله";


            //Parobj.exportAfterClick = true;
            //if (Parobj.exportAfterClick == false)
            //{
            //    return View();
            //}
            //else if (Parobj.TMT)
            //{
            //    return ExportToExcelAx(connectionString, Parobj);
            //}

        }
        public IActionResult ExportToExcel(string connectionString, SalesParameters Parobj)
        {
            var username = HttpContext.Session.GetString("Username");
            var Password = HttpContext.Session.GetString("Password");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Isuser = HttpContext.Session.GetString("isUsername");
            ViewBag.Username = username;
            ViewBag.Password = Password;
            ViewBag.Role = Role;
            ViewBag.StoreIdRms = StoreIdRms;
            ViewBag.StoreIddynamic = StoreIddynamic;
            ViewBag.uuu = Isuser;
            var Inventlocation = HttpContext.Session.GetString("Inventlocation");
            Parobj.Server = HttpContext.Session.GetString("Server");
            HttpContext.Session.SetString("ExportStatus", "started");
            // Prepare the SQL query with a parameter placeholder
            // Start building the SELECT clause dynamically
            List<string> selectColumns = new List<string>();
            if (Parobj.VStoreId)
                selectColumns.Add("StoreID");
            if (Parobj.VStoreName)
                selectColumns.Add("StoreName as 'Store Name'");
            if (Parobj.VDpId)
                selectColumns.Add("DpID as 'Department Id'");
            if (Parobj.VDepartment)
                selectColumns.Add("dpname Department");
            if (Parobj.VItemLookupCode)
                selectColumns.Add("Itemlookupcode");
            if (Parobj.VItemName)
                selectColumns.Add("ItemName");
            if (Parobj.VSupplierId)
                selectColumns.Add("SupplierCode");
            if (Parobj.VSupplierName)
                selectColumns.Add("SupplierName");
            if (Parobj.VFranchise)
                selectColumns.Add("StoreFranchise");
            if (Parobj.VQty)
                selectColumns.Add("sum(Qty)TotalQty");
            //if (Parobj.VPrice)
            //	selectColumns.Add("Price");
            if (Parobj.VCost)
                selectColumns.Add("Cost");
            // Construct the SELECT clause from the list of columns
            bool isFmanager = db2.RptUsers.Any(s => s.Fmanager == username);
            bool isDmanager = db2.RptUsers.Any(s => s.Dmanager == username);
            bool isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));
            string[] storeVal = Parobj.Store.Split(':');
            if (Parobj.StockFromBranch)
            {
                if (Isuser =="False" && storeVal.Length > 1)
                {
                    string? BranchServer = (from INVE in db2.RptUsers
                                            where INVE.Storenumber == storeVal[0]
                                            select INVE.Server).FirstOrDefault(); 
                    string? BranchName = (from INVE in db2.RptUsers
                               where INVE.Storenumber == storeVal[0]
                                select INVE.Inventlocation).FirstOrDefault();
                    Parobj.Server = BranchServer;
                    Inventlocation = BranchName;
                }
                    isUsername = db2.RptUsers.Any(s => s.Username == username || s.Inventlocation == Inventlocation && (s.Storenumber != null || s.Storenumber != " "));
            }
            if (Parobj.SupplierofItem)
            {
                Parobj.Store = "0";
            }
            //db2.Storeusers.Any(s => s.Inventlocation == Inventlocation);
            IQueryable<Storeuser> query;
            string selectClause = string.Join(", ", selectColumns);
            string fromWhereClause = null;
            DateTime startDateTime = Convert.ToDateTime(Parobj.startDate, new CultureInfo("en-GB"));
            DateTime endDateTime = Convert.ToDateTime(Parobj.endDate, new CultureInfo("en-GB"));
            if ((isDmanager || isUsername || isFmanager)&&!Parobj.SupplierofItem)
            {
                if (Parobj.StockFromBranch)
                {
                    //fromWhereClause = "from (" + Parobj.RptStockofBranchView + ")RptStore Where StoreName ='"+Inventlocation+"' ";
                    fromWhereClause = @"from
(
select (-sum(Q1)) Qty1,(-sum(Q2)) Qty2,(-sum(Q3)) Qty3,(sum(Q4)) Qty4,
sum(Q4)-((-sum(Q1))-(-sum(Q2)))-(-sum(Q3)) Qty,R.ItemlookupCode,R.StoreName
,It.Name ItemName,Cat.dpName,Cat.dpId,''Fmanager,''username,''Dmanager from (
select
ZSalesd.Item  collate SQL_Latin1_General_CP1_CI_AS ItemlookupCode ,sum(ZSalesd.Qty) Q1,0 Q2,0 Q3,0 Q4,
ZSalesH.INVENTLOCATIONID collate SQL_Latin1_General_CP1_CI_AS StoreName
from [" + Parobj.Server + @"].RetailChannelDatabase.ax.Retailtransactiontable ZSalesH 
inner join (select distinct Transactionid,sum(QTY)Qty,ITEMID Item from [" + Parobj.Server + @"].RetailChannelDatabase.ax.Retailtransactionsalestrans group by Transactionid,ITEMID) zSalesD
on ZSalesH.Transactionid = ZSalesD.Transactionid
where   zSalesH.Type = 2  and zSalesH.Entrystatus != 1 and cast(zSalesH.TRANSDATE as date)>=cast(getdate() as date)
group by ZSalesd.Item,ZSalesH.INVENTLOCATIONID
union all
select Item ,0,sum(Q1)Q2,sum(Q2)Q3,0,Store from AXDBTest.dbo.Store1
group by Item,Store
union all 
select  Inv.ItemId Item,0,0,0, sum(Inv.Physicalinvent)Q4,Inv.INVENTLOCATIONID Store
from AXDB.dbo.INVENTSUM Inv
inner join AXDB.dbo.Inventtable Ca on Ca.Itemid=Inv.itemid
 where ca.Dataareaid = 'tmt' and  Inv.Wmslocationid is not null 
 group by Inv.INVENTLOCATIONID,Inv.ItemId
)R
inner join AXDB.dbo.Inventtable Ca on Ca.Itemid=R.ItemlookupCode
inner join AXDB.dbo.Ecoresproducttranslation It on ca.Product = It.Product
inner join AXDB.dbo.Ecoresproductcategory Re on It.product =Re.Product
left join AXDB.dbo.Ecorescategory CateN on Re.Category = CateN.Recid
INNER JOIN  (Select distinct Cat.ReCId,Cat.CODE dpId,Cat.NAME dpName from AXDB.dbo.Ecorescategory Cat)Cat on Re.Category = Cat.Recid ";

                    if ((Parobj.Department == "102") || (Parobj.Department == "103") || (Parobj.Department == "104") || (Parobj.Department == "105") ||
    (Parobj.Department == "108") || (Parobj.Department == "112") || (Parobj.Department == "113") || (Parobj.Department == "114") ||
    (Parobj.Department == "140") || (Parobj.Department == "171"))
                    {
                        fromWhereClause += " inner join (select Category,Barcode from [192.168.1.156].CkproUsers.dbo.Catlimit)C on C.Category=Cat.dpName and C.barcode=R.ItemlookupCode ";
                    }
                    fromWhereClause += @"
 where ca.Dataareaid = 'tmt'
 GROUP BY StoreName, Cat.dpId, Itemlookupcode, It.Name,Cat.dpName
)Rs where itemlookupcode is not null ";
                }
            }
            else if (Parobj.RichCut || (Parobj.SalesBranchPage && (username == "RichCut" || username == "RichCut_No" || username == "RichCut_Zayed")))
            {
                fromWhereClause = @"FROM (" + Parobj.RichCutStock + ")AkRptStore Where ItemLookupCode is not null  ";
            }
            else if ((Parobj.RMS && Parobj.TMT == false) || (Parobj.RMS && storeVal[0] == "RMS"))
            {
                fromWhereClause = @"FROM (" + Parobj.RmsRptStoreView + ")RptStore Where StoreId != '' ";
            }
            else if (Parobj.RMS == false && Parobj.TMT || (storeVal.Length > 1 && storeVal[1] == "Dy") ||Parobj.SupplierofItem)
            {
                fromWhereClause = @"FROM (" + Parobj.DyRptStoreView() + ")AkRptStore Where ItemLookupCode is not null  ";
            }
            else
            {
                if (Parobj.StockFromBranch)
                {

                    fromWhereClause = "from (" + Parobj.RptStockofBranchView + ")RptStore Where StoreId != '' ";
                }
                else
                {
                    fromWhereClause = $"FROM ({Parobj.RptStoreAll()})RptStore Where StoreId != ''  ";

                }
            }
            // string MessageBox = string.Empty;
            // Add department filter if a department is specified
            if (!string.IsNullOrEmpty(Parobj.Department) && Parobj.Department != "0")
            {
                fromWhereClause += "AND DpName = @Department ";
            }
            if (!string.IsNullOrEmpty(Parobj.Supplier) && Parobj.Supplier != "0")
            {
                fromWhereClause += "AND SupplierName = @Supplier ";
            }
            if (isDmanager || isUsername || isFmanager)
            {
                fromWhereClause += "AND (Dmanager='" + username + "' or username ='" + username + "' or StoreName ='" + Inventlocation + "' or Fmanager ='" + username + "') ";

            }
            if (Parobj.SupplierofItem)
            {
                fromWhereClause += "AND StoreName = '"+username+"'";
            }
            if (Parobj.Store != "0"&&!Parobj.StockFromBranch)
            {
                if (Parobj.RMS && Parobj.TMT == false || storeVal[0] == "RMS" || Parobj.DBbefore)
                {
                    fromWhereClause += "AND StoreId = @Store1 ";
                }
                else if (Parobj.RMS == false && Parobj.TMT || (storeVal.Length > 1 && storeVal[1] == "Dy"))
                {
                    fromWhereClause += "AND StoreId = @Store ";
                }
                else if (Parobj.RMS && Parobj.TMT && !Parobj.StockFromBranch)
                {
                    fromWhereClause += "AND (StoreIdD = @Store OR StoreIdR = @Store1) ";

                }
                else
                {
                    fromWhereClause += "AND StoreId = @Store1 ";
                }
            }
            if (Parobj.Franchise == "TMT")
            {
                fromWhereClause += "AND StoreFranchise = 'TMT'";
            }
            if (Parobj.Franchise == "SUB-FRANCHISE")
            {
                fromWhereClause += "AND StoreFranchise = 'SUB-FRANCHISE'";
            }
            // Add ItemLookupCode filter if ItemLookupCodeTxt is not null or empty

            if (!string.IsNullOrEmpty(Parobj.ItemLookupCodeTxt))
            {
                // Split the string into an array of values
                string[] itemLookupCodes = Parobj.ItemLookupCodeTxt.Split(',');

                // Start building the IN clause
                fromWhereClause += " AND ItemLookupCode IN (";

                // Add a parameter placeholder for each value
                for (int i = 0; i < itemLookupCodes.Length; i++)
                {
                    // Trim any whitespace from the value
                    string itemLookupCode = itemLookupCodes[i].Trim();

                    // Append the parameter placeholder to the IN clause
                    fromWhereClause += $"@ItemLookupCode{i}";

                    // Add a comma separator if not the last item
                    if (i < itemLookupCodes.Length - 1)
                    {
                        fromWhereClause += ",";
                    }
                }

                fromWhereClause += ")";
            }
            if (!string.IsNullOrEmpty(Parobj.ItemNameTxt))
            {
                // Split the string into an array of values
                string[] ItemNames = Parobj.ItemNameTxt.Split(',');

                // Start building the IN clause
                fromWhereClause += " AND ItemName IN (";

                // Add a parameter placeholder for each value
                for (int i = 0; i < ItemNames.Length; i++)
                {
                    // Trim any whitespace from the value
                    string ItemName = ItemNames[i].Trim();

                    // Append the parameter placeholder to the IN clause
                    fromWhereClause += $"@ItemName{i}";

                    // Add a comma separator if not the last item
                    if (i < ItemNames.Length - 1)
                    {
                        fromWhereClause += ",";
                    }
                }

                fromWhereClause += ")";
            }
            string sqlQuery;
            if (Parobj.StockFromBranch)
            {
                sqlQuery = @"
delete from AXDBTest.dbo.Store1 where Store='" + Inventlocation + @"'

--delete from AXDBTest.dbo.Store where Store='" + Inventlocation + @"'
--insert into AXDBTest.dbo.Store 
--select distinct Inv.INVENTLOCATIONID Store,cast(max(Inv.Modifieddatetime) as date)Ddate from AXDB.dbo.INVENTSUM Inv group by INVENTLOCATIONID
insert into AXDBTest.dbo.Store1
select
Salesd.Item Item,sum(Salesd.Q1) Q2,0 Q3,
SalesH.INVENTLOCATIONID Store
from
AXDB.dbo.Retailtransactiontable SalesH 
inner join (select distinct Transactionid,sum(QTY)Q1,ITEMID Item from AXDB.dbo.Retailtransactionsalestrans group by Transactionid,ITEMID) SalesD
on SalesH.Transactionid = SalesD.Transactionid
where   SalesH.Type = 2  and SalesH.Entrystatus != 1 and cast(SalesH.TRANSDATE as date)>=cast(getdate() as date)
and Salesh.INVENTLOCATIONID='" + Inventlocation + @"'
group by Salesd.Item,SalesH.INVENTLOCATIONID
union all
select
Salesd2.Item Item,0 Q2,sum(SalesD2.Q1)Q3,
SalesH.INVENTLOCATIONID Store
from
AXDB.dbo.Retailtransactiontable SalesH 
inner join (select distinct Transactionid,sum(QTY)Q1,ITEMID Item from AXDB.dbo.Retailtransactionsalestrans
where  Inventstatussales=0  group by Transactionid,ITEMID )SalesD2
on SalesH.Transactionid = SalesD2.Transactionid
where   SalesH.Type = 2  and SalesH.Entrystatus != 1 and cast(SalesH.TRANSDATE as date)>=cast(getdate() as date)
and Salesh.INVENTLOCATIONID='" + Inventlocation + @"'
group by Salesd2.Item,SalesH.INVENTLOCATIONID

 " + $"SELECT {selectClause} {fromWhereClause}";
            }
            else
            {
                sqlQuery = $"SELECT {selectClause} {fromWhereClause}";
            }
                
            // Start building the GROUP BY clause dynamically
            List<string> groupByColumns = new List<string>();
            if (Parobj.VStoreId)
                groupByColumns.Add("StoreID");
            if (Parobj.VStoreName)
                groupByColumns.Add("StoreName");
            if (Parobj.VDpId)
                groupByColumns.Add("DpID");
            if (Parobj.VDepartment)
                groupByColumns.Add("dpname");
            if (Parobj.VItemLookupCode)
                groupByColumns.Add("Itemlookupcode");
            if (Parobj.VItemName)
                groupByColumns.Add("ItemName");
            if (Parobj.VSupplierId)
                groupByColumns.Add("SupplierCode");
            if (Parobj.VSupplierName)
                groupByColumns.Add("SupplierName");
            if (Parobj.VFranchise)
                groupByColumns.Add("StoreFranchise");
            //if (Parobj.VPrice)
            //	groupByColumns.Add("Price");
            if (Parobj.VCost)
                groupByColumns.Add("Cost");
            // Do not include sum(totalsales) in the GROUP BY clause

            // Construct the GROUP BY clause from the list of columns
            string groupByClause = groupByColumns.Count > 0 ? "GROUP BY " + string.Join(", ", groupByColumns) : "";

            sqlQuery += groupByClause;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
               
                    //string storedProcedureName = "R2"; // Replace with your actual stored procedure name
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                     {
                        //command.CommandType = CommandType.StoredProcedure;
                        //sqlQuery = "SELECT sum(TotalSales) TotalSales FROM RptSales WHERE CAST(TransDate AS DATE) BETWEEN @fromDate AND @toDate";
                        command.CommandTimeout = 7200;
                        // Add the date parameters to the command if they are not null
                        if (Parobj.Store != "0")
                        {
                            if (Parobj.RichCut)
                            {
                                if (storeVal[1] == "R1" || storeVal[1] == "R2" || storeVal[1] == "R3")
                                {
                                    command.Parameters.AddWithValue("@Store1", storeVal[1]);
                                }
                            }
                            else if (Parobj.RMS && Parobj.TMT == false || storeVal[0] == "RMS")
                            {
                                if (storeVal.Length > 1 && int.TryParse(storeVal[1], out int storeId))
                                {
                                    command.Parameters.AddWithValue("@Store1", storeId);
                                }
                            }
                            else if (Parobj.RMS == false && Parobj.TMT || (storeVal.Length > 1 && storeVal[1] == "Dy"))
                            {
                                //if (storeVal.Length > 1 && int.TryParse(storeVal[0], out int storeId))
                                //{
                                command.Parameters.AddWithValue("@Store", storeVal[0]);
                                //}
                            }
                            else if (Parobj.RMS && Parobj.TMT)// && storeVal[0] != "RMS" && storeVal[1] != "Dy" || (Parobj.RMS && Parobj.TMT && storeVal[0] == "0" && storeVal[1] == "0"))
                            {
                                command.Parameters.AddWithValue("@Store1", storeVal[1]);
                                command.Parameters.AddWithValue("@Store", storeVal[0]);
                            }
                            else
                            {
                                return View();
                            }
                        }
                        if (!string.IsNullOrEmpty(Parobj.Department) && Parobj.Department != "0")
                        {
                            command.Parameters.AddWithValue("@Department", Parobj.Department);
                        }
                        if (!string.IsNullOrEmpty(Parobj.Supplier) && Parobj.Supplier != "0")
                        {
                            command.Parameters.AddWithValue("@Supplier", Parobj.Supplier);
                        }
                        if (!string.IsNullOrEmpty(Parobj.ItemLookupCodeTxt))
                        {
                            string[] itemLookupCodes = Parobj.ItemLookupCodeTxt.Split(',');
                            for (int i = 0; i < itemLookupCodes.Length; i++)
                            {
                                string itemLookupCode = itemLookupCodes[i].Trim();
                                command.Parameters.AddWithValue($"@ItemLookupCode{i}", itemLookupCode);
                            }
                        }
                        if (!string.IsNullOrEmpty(Parobj.ItemNameTxt))
                        {
                            string[] ItemNames = Parobj.ItemNameTxt.Split(',');
                            for (int i = 0; i < ItemNames.Length; i++)
                            {
                                string ItemName = ItemNames[i].Trim();
                                command.Parameters.AddWithValue($"@ItemName{i}", ItemName);
                            }
                        }
                        if (Parobj.MainStock && (Parobj.actionValue == 1 || Parobj.actionValue == 0))
                        {
                            var vi = new List<RptSale>();
                            var ll = 0;
                            var test = command.ExecuteReader();
                            double TotalSales = 0;
                            double TotalQty = 0;
                            double Transactions = 0;
                            double tsa = 0;
                            double tq = 0;
                            while (test.Read())
                            {
                                RptSale si = new RptSale();
                                if (Parobj.VPerYear || Parobj.VPerMonYear)
                                {
                                    ViewBag.VPerYear = "V";
                                    ViewBag.VPerMonYear = "V";
                                    si.ByYear = Convert.ToInt16(test["ByYear"]);
                                }
                                if (Parobj.VPerMon || Parobj.VPerMonYear)
                                {
                                    ViewBag.VPerMon = "V";
                                    ViewBag.VPerMonYear = "V";
                                    si.ByMonth = Convert.ToInt16(test["ByMonth"]);
                                }
                                if (Parobj.VStoreId)
                                {
                                    ViewBag.VStoreId = "V";
                                    si.StoreId = test["StoreID"].ToString();
                                }
                                if (Parobj.VStoreName)
                                {
                                    si.StoreName = test["Store Name"].ToString();
                                    ViewBag.VStoreName = "V";
                                }
                                if (Parobj.VDpId)
                                {
                                    ViewBag.VDpId = "V";
                                    si.DpId = test["Department Id"].ToString();
                                }
                                if (Parobj.VDepartment)
                                {
                                    ViewBag.VDepartment = "V";
                                    si.DpName = test["Department"].ToString();
                                }
                                if (Parobj.VItemLookupCode)
                                {
                                    ViewBag.VItemLookupCode = "V";
                                    si.ItemLookupCode = test["ItemLookupCode"].ToString();
                                }
                                if (Parobj.VItemName)
                                {
                                    ViewBag.VItemName = "V";
                                    si.ItemName = test["ItemName"].ToString();
                                }
                                if (Parobj.VSupplierId)
                                {
                                    ViewBag.VSupplierId = "V";
                                    si.SupplierId = test["SupplierCode"].ToString();
                                }
                                if (Parobj.VSupplierName)
                                {
                                    ViewBag.VSupplierName = "V";
                                    si.SupplierName = test["SupplierName"].ToString();
                                }
                                if (Parobj.VFranchise)
                                {
                                    ViewBag.VFranchise = "V";
                                    si.StoreFranchise = test["StoreFranchise"].ToString();
                                }
                                if (Parobj.VTransactionNumber)
                                {
                                    ViewBag.VTransactionNumber = "V";
                                    si.TransactionNumber = test["TransactionNumber"].ToString();
                                }
                                if (Parobj.VQty)
                                {
                                    ViewBag.VQty = "V";
                                    TotalQty = Convert.ToDouble(test["TotalQty"]);
                                    si.FQty = TotalQty.ToString("#,##0.00");
                                }
                                if (Parobj.VPrice)
                                {
                                    ViewBag.VPrice = "V";
                                    si.Price = Convert.ToDecimal(test["Price"]);
                                }
                                if (Parobj.VCost)
                                {
                                    ViewBag.VCost = "V";
                                    si.Cost = Convert.ToDecimal(test["Cost"]);
                                }
                                if (Parobj.VTotalSales)
                                {
                                    ViewBag.VTotalSales = "V";
                                    TotalSales = Convert.ToDouble(test["TotalSales"]);
                                    si.FTotalSales = TotalSales.ToString("#,##0.00");
                                }
                                if (Parobj.VTransactionCount)
                                {
                                    ViewBag.VTransactionCount = "V";
                                    si.TransactionsCount = Convert.ToDouble(test["TransactionsCount"]);
                                }
                                if (Parobj.VTotalCost)
                                {
                                    ViewBag.VTotalCost = "V";
                                    si.TotalCost = Convert.ToDouble(test["TotalCost"]);
                                }
                                if (Parobj.VTotalTax)
                                {
                                    ViewBag.VTotalTax = "V";
                                    si.TotalTax = Convert.ToDouble(test["TotalTax"]);
                                }
                                if (Parobj.VTotalSalesTax)
                                {
                                    ViewBag.VTotalSaleswithTax = "V";
                                    si.TotalSaleswithTax = Convert.ToDouble(test["TotalSaleswithTax"]);
                                }
                                if (Parobj.VTotalSalesWithoutTax)
                                {
                                    ViewBag.VTotalSalesWithoutTax = "V";
                                    si.TotalSalesWithoutTax = Convert.ToDouble(test["TotalSalesWithoutTax"]);
                                }
                                if (Parobj.VTotalCostQty)
                                {
                                    ViewBag.VTotalCostQty = "V";
                                    si.TotalCostQty = Convert.ToDouble(test["TotalCostQty"]);
                                }
                                if (Parobj.VPerDay)
                                {
                                    ViewBag.VPerDay = "V";
                                    si.ConvDate = test["Date"].ToString();
                                    DateTime parsedDate;
                                    if (DateTime.TryParse(si.ConvDate, out parsedDate))
                                    {
                                        si.ConvDate = parsedDate.ToString("yyyy-MM-dd");
                                    }
                                }
                                ViewBag.Q = si.Qty;
                                //TotalSales = Convert.ToDouble(si.TotalSales);
                                //si.ConvTotalSales=TotalSales.ToString("#,##0.00");
                                //TotalQty = Convert.ToDouble(FQty);
                                si.ConvTotalQty = TotalQty.ToString("#,##0.00");
                                Transactions = Convert.ToDouble(si.TransactionsCount);
                                tsa += Convert.ToDouble(TotalSales); ;
                                tq += Convert.ToDouble(TotalQty);
                                vi.Add(si);
                            }
                            ViewBag.Data = vi;
                            ViewBag.Total = tsa.ToString("#,##0.00");
                            ViewBag.TotalQty = tq.ToString("#,##0.00");
                            ViewBag.Transactions = Transactions.ToString("#,##0.00");
                            ViewBag.St = "com";
                            if (Parobj.SupplierofItem)
                            {
                                return View("SupplierItemBarcode");
                            }
                            else if (Parobj.MainStock)
                            {
                                return View("Index");
                            }
                          }

                        if (Parobj.StockFromBranch)
                        {
                            var vi = new List<RptSale>();
                            var ll = 0;
                            var test = command.ExecuteReader();
                            while (test.Read())
                            {
                                RptSale si = new RptSale();
                                si.StoreName = test["Store Name"].ToString();
                                si.ItemLookupCode = test["Itemlookupcode"].ToString();
                                si.ItemName = test["ItemName"].ToString();
                                si.Qty = Convert.ToDecimal(test["TotalQty"]);
                                si.Cost = 100;
                                ViewBag.lname = ll;
                                ViewBag.Q = si.Qty;
                                vi.Add(si);
                            }
                            ViewBag.Data = vi;
                            return View("StockFromBranch");
                        }
                        //var data = ExecuteQuery(); // This method should execute your SQL query and return the results
                        //return View(data);
                        // Create a new Excel package
                        //try
                        //{
                        using (var package = new ExcelPackage())
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("AKStockReport");
                            int row = 2; // Start from row 2 to leave space for headers
                            int sheetIndex = 1; // Start with the first sheet
                            int columnCount = 1;
                            // Add header row
                            void AddHeaderRow(ExcelWorksheet ws, int columnCount)
                            {
                                int column = 1;
                                if (Parobj.VStoreId)
                                    ws.Cells[1, column++].Value = "StoreID";
                                if (Parobj.VStoreName)
                                    ws.Cells[1, column++].Value = "Store Name";
                                if (Parobj.VDpId)
                                    ws.Cells[1, column++].Value = "Department Id";
                                if (Parobj.VDepartment)
                                    ws.Cells[1, column++].Value = "Department";
                                if (Parobj.VItemLookupCode)
                                    ws.Cells[1, column++].Value = "BarCode";
                                if (Parobj.VItemName)
                                    ws.Cells[1, column++].Value = "ItemName";
                                if (Parobj.VSupplierId)
                                    ws.Cells[1, column++].Value = "SupplierCode";
                                if (Parobj.VSupplierName)
                                    ws.Cells[1, column++].Value = "SupplierName";
                                if (Parobj.VFranchise)
                                    ws.Cells[1, column++].Value = "StoreFranchise";
                                if (Parobj.VQty)
                                    ws.Cells[1, column++].Value = "TotalQty";
                                //if (Parobj.VPrice)
                                //	ws.Cells[1, column++].Value = "Price";
                                if (Parobj.VCost)
                                    ws.Cells[1, column++].Value = "Cost";
                                using (var headerRange = ws.Cells[1, 1, 1, column - 1])
                                {
                                    headerRange.Style.Font.Bold = true;
                                    headerRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    headerRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    headerRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    headerRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SkyBlue);
                                    ws.Cells[1, 1, 1, column - 1].AutoFilter = true;
                                }
                            }
                            AddHeaderRow(worksheet, columnCount);
                            //row = 2;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    columnCount = 1; // Reset column count for each row
                                    if (Parobj.VStoreId)
                                        worksheet.Cells[row, columnCount++].Value = reader["StoreID"];
                                    if (Parobj.VStoreName)
                                        worksheet.Cells[row, columnCount++].Value = reader["Store Name"];
                                    if (Parobj.VDpId)
                                        worksheet.Cells[row, columnCount++].Value = reader["Department Id"];
                                    if (Parobj.VDepartment)
                                        worksheet.Cells[row, columnCount++].Value = reader["Department"];
                                    if (Parobj.VItemLookupCode)
                                        worksheet.Cells[row, columnCount++].Value = reader["Itemlookupcode"];
                                    if (Parobj.VItemName)
                                        worksheet.Cells[row, columnCount++].Value = reader["ItemName"];
                                    if (Parobj.VSupplierId)
                                        worksheet.Cells[row, columnCount++].Value = reader["SupplierCode"];
                                    if (Parobj.VSupplierName)
                                        worksheet.Cells[row, columnCount++].Value = reader["SupplierName"];
                                    if (Parobj.VFranchise)
                                        worksheet.Cells[row, columnCount++].Value = reader["StoreFranchise"];
                                    if (Parobj.VQty)
                                        worksheet.Cells[row, columnCount++].Value = reader["TotalQty"];
                                    //if (Parobj.VPrice)
                                    //	worksheet.Cells[row, columnCount++].Value = reader["Price"];
                                    if (Parobj.VCost)
                                        worksheet.Cells[row, columnCount++].Value = reader["Cost"];
                                    worksheet.Cells[row, columnCount].Style.Numberformat.Format = "#,##0.00";
                                    if (columnCount <= 1)
                                    {
                                        Console.WriteLine("Error: columnCount is 0. No data to process.");
                                        // Optionally, throw an exception to halt execution
                                        // throw new InvalidOperationException("columnCount is 0. No data to process.");
                                    }
                                    else
                                    {
                                        using (var rowRange = worksheet.Cells[row, 1, row, columnCount - 1])
                                        {
                                            rowRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                            if (row % 2 == 0) // Even row
                                            {
                                                rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                rowRange.Style.Fill.BackgroundColor.SetColor(Color.LightBlue); // Light gray for even rows
                                            }
                                            rowRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                            rowRange.Style.Border.Top.Color.SetColor(Color.LightBlue); // Set border color to black
                                            rowRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                            rowRange.Style.Border.Bottom.Color.SetColor(Color.LightBlue); // Set border color to black
                                            rowRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                            rowRange.Style.Border.Left.Color.SetColor(Color.LightBlue); // Set border color to black
                                            rowRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                            rowRange.Style.Border.Right.Color.SetColor(Color.LightBlue); // Set border color to black
                                        }
                                        row++;
                                    }

                                    if (row == 1000001)
                                    {
                                        // Create a new worksheet and reset the row count
                                        worksheet = package.Workbook.Worksheets.Add($"AKSalesReport{sheetIndex++}");
                                        // Re-add the header row to the new worksheet
                                        row = 2; // Reset row count for the new worksheet
                                        columnCount = 1; // Reset column count
                                                         // Re-add the header row to the new worksheet\
                                        AddHeaderRow(worksheet, columnCount);
                                        if (Parobj.VStoreId)
                                            worksheet.Cells[row, columnCount++].Value = reader["StoreID"];
                                        if (Parobj.VStoreName)
                                            worksheet.Cells[row, columnCount++].Value = reader["Store Name"];
                                        if (Parobj.VDpId)
                                            worksheet.Cells[row, columnCount++].Value = reader["Department Id"];
                                        if (Parobj.VDepartment)
                                            worksheet.Cells[row, columnCount++].Value = reader["Department"];
                                        if (Parobj.VItemLookupCode)
                                            worksheet.Cells[row, columnCount++].Value = reader["Itemlookupcode"];
                                        if (Parobj.VItemName)
                                            worksheet.Cells[row, columnCount++].Value = reader["ItemName"];
                                        if (Parobj.VSupplierId)
                                            worksheet.Cells[row, columnCount++].Value = reader["SupplierCode"];
                                        if (Parobj.VSupplierName)
                                            worksheet.Cells[row, columnCount++].Value = reader["SupplierName"];
                                        if (Parobj.VFranchise)
                                            worksheet.Cells[row, columnCount++].Value = reader["StoreFranchise"];
                                        if (Parobj.VQty)
                                            worksheet.Cells[row, columnCount++].Value = reader["TotalQty"];
                                        //if (Parobj.VPrice)
                                        //	worksheet.Cells[row, columnCount++].Value = reader["Price"];
                                        if (Parobj.VCost)
                                            worksheet.Cells[row, columnCount++].Value = reader["Cost"];
                                        worksheet.Cells[row, columnCount].Style.Numberformat.Format = "#,##0.00";
                                        if (columnCount <= 1)
                                        {
                                            Console.WriteLine("Error: columnCount is 0. No data to process.");
                                            // Optionally, throw an exception to halt execution
                                            // throw new InvalidOperationException("columnCount is 0. No data to process.");
                                        }
                                        else
                                        {
                                            // Apply styling to the row
                                            using (var rowRange = worksheet.Cells[row, 1, row, columnCount - 1])
                                            {
                                                rowRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                                if (row % 2 == 0) // Even row
                                                {
                                                    rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                    rowRange.Style.Fill.BackgroundColor.SetColor(Color.LightBlue); // Light gray for even rows
                                                }
                                                rowRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                                rowRange.Style.Border.Top.Color.SetColor(Color.LightBlue); // Set border color to black
                                                rowRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                rowRange.Style.Border.Bottom.Color.SetColor(Color.LightBlue); // Set border color to black
                                                rowRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                                rowRange.Style.Border.Left.Color.SetColor(Color.LightBlue); // Set border color to black
                                                rowRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                                rowRange.Style.Border.Right.Color.SetColor(Color.LightBlue); // Set border color to black
                                            }
                                        }
                                    }
                                }
                            }
                            worksheet.Cells.AutoFitColumns();
                            // Save the package to a MemoryStream
                            var stream = new MemoryStream();
                            package.SaveAs(stream);

                            // Reset the stream position to the beginning
                            stream.Position = 0;
                            Console.WriteLine(sqlQuery); // Print the final query string

                            // Before executing the command
                            foreach (SqlParameter param in command.Parameters)
                            {
                                Console.WriteLine($"{param.ParameterName}: {param.Value}"); // Print each parameter name and value
                            }
                            // Return the file as a FileResult
                            Console.WriteLine(sqlQuery); // Print the final query string
                            foreach (SqlParameter param in command.Parameters)
                            {
                                Console.WriteLine($"{param.ParameterName}: {param.Value}"); // Print each parameter name and value
                            }
                            HttpContext.Session.SetString("ExportStatus", "complete");
                            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AKSoftStock.xlsx");
                        }
                        /* }
                         catch
                         {
                             HttpContext.Session.SetString("ExportStatus", "unKnown1");
                             return View();
                         }
                        */
                    }

                }
                catch (Exception ex)
                {
                    return View();
                }
            }
        }
            
        static void Main(string[] args)
		{
			// Step 1: Retrieve the server IP from the database
			string serverIp = GetServerIpFromDatabase();

			// Step 2: Format the connection string dynamically
			string connectionString = FormatConnectionString(serverIp);

			// Use the connection string to connect to the database
			// For demonstration, let's just print the connection string
			Console.WriteLine(connectionString);
		}
		static string GetServerIpFromDatabase()
		{
			string serverIp = string.Empty;
			string connectionString = "Server=192.168.1.156;User ID=sa;Password=P@ssw0rd;Database=CkproUsers;Connect Timeout=7200;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "SELECT TOP 1 server FROM Storeuser where Server='192.168.104.222/New'"; // Assuming you want the first server IP found
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					object result = command.ExecuteScalar();
					if (result != null)
					{
						serverIp = result.ToString();
					}
				}
			}

			return serverIp;
		}

		static string FormatConnectionString(string serverIp)
		{
			// Assuming the rest of the connection string remains the same except for the server IP
			string connectionStringFormat = "Server={0};User ID=sa;Password=P@ssw0rd;Database=RetailChannelDatabase;Connect Timeout=7200;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";
			string connectionString = string.Format(connectionStringFormat, serverIp);
			return connectionString;
		}
		public IActionResult CheckExportStatus()
		{
			// Read the session variable
			var exportStatus = HttpContext.Session.GetString("ExportStatus");
			if (exportStatus == "complete")
			{
				// If the status is "complete", reset it to an empty string or any other default value
				HttpContext.Session.SetString("ExportStatus", "");
				return Content("complete");
			}
			return Content(exportStatus ?? "unknown");
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
        public bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }
        public string decrypt(string cipherText)
        {
            if (!IsBase64String(cipherText))
            {
                // Handle the error, e.g., log it, return a default value, or throw an exception
                return "Invalid encrypted password format";
            }

            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public async Task<IActionResult> indexa()
        {
            using (var db2 = new CkproUsersContext())
            {
                var storeUsers = await db2.RptUsers2s.ToListAsync();

                // Decrypt each user's password
                foreach (var user in storeUsers)
                {
                    user.DecryptedPassword = decrypt(user.Password); // Assuming 'decrypt' is your decryption method
                }

                return View(storeUsers);
            }
        }
        public async Task<IActionResult> index2()
        {
            using (var db2 = new DataCenterContext())
            {
                var storeUsers = await db2.RptSales.ToListAsync();

                return View(storeUsers);
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}

}
