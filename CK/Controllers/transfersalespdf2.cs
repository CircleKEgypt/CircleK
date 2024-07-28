using CK.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Data;
using Document = iTextSharp.text.Document;
using Paragraph = iTextSharp.text.Paragraph;
using Rectangle = iTextSharp.text.Rectangle;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Bibliography;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;


namespace CK.Controllers
{
    public class transfersalespdf2Controller : Controller
    {
        SalesParameters Parobj = new SalesParameters();
        // for image location set in wwwroot
        private readonly IWebHostEnvironment _env;
        public transfersalespdf2Controller(IWebHostEnvironment env)
        {
            _env = env;
        }
        [HttpGet]
        public IActionResult AddTransferDetails(SalesParameters Parobj, string? transferId)
        {
            CkproUsersContext db2 = new CkproUsersContext();
            var username = HttpContext.Session.GetString("Username");
            var Password = HttpContext.Session.GetString("Password");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Isuser = HttpContext.Session.GetString("isUsername");
            var Inventlocation = HttpContext.Session.GetString("Inventlocation");
            ViewBag.Username = username;
            ViewBag.Password = Password;
            ViewBag.Role = Role;
            ViewBag.StoreIdRms = StoreIdRms;
            ViewBag.StoreIddynamic = StoreIddynamic;
            ViewBag.isUsername = Isuser;
            string approve=null;
            if (Parobj.TransferMessage==null)
            {
                Parobj.TransferMessage = "No Comment";
            }
            if (Parobj.Approve)
            {
                approve = "Approved";
            }
            else if (Parobj.UnApprove)
            {
                approve = "UnApproved";
            }
            else
                approve = "Approved";
            DataCenterContext db = new DataCenterContext();
            db.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            ViewBag.TransferId = transferId;
            try
            {
                using (SqlConnection connection = new SqlConnection(Parobj.AxdbConnection))
                {
                    string SQlQuery = @"
delete from [192.168.1.208\New].TopSoft.dbo.TransferDetails  where [Transaction] = '" + transferId + "'";
                    SQlQuery += @"
insert into [192.168.1.208\New].TopSoft.dbo.TransferDetails
select *,'" + approve + @"' Manager, @Message,'Confirmed' from 
(" + Parobj.RptTransfer + ")R where [Transaction] = '" + transferId + "'";
                    //SQlQuery += @" update s set s.[Compare]='Checked' from [192.168.1.208\New].TopSoft.dbo.TransferDetails s where [Transaction] ='" + transferId + "' ";
                    using (SqlCommand command = new SqlCommand(SQlQuery, connection))
                    {

                        command.Parameters.AddWithValue("@transferId", transferId);
                        command.Parameters.AddWithValue("@Message", Parobj.TransferMessage);
                        connection.Open(); // Open the connection
                        command.ExecuteNonQuery(); // Execute the command
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ATransferSol", "transfersalespdf2");
            }
            Parobj.actionValue = 2;
            TempData["Result"] = "done";
            return RedirectToAction("ATransferSol", "transfersalespdf2");
        }

        [HttpGet]
        public IActionResult ATransferSol(SalesParameters Parobj, string? transferId, string? sales,string? ManagerAction)
        {
            CkproUsersContext db2 = new CkproUsersContext();
            var username = HttpContext.Session.GetString("Username");
            var Password = HttpContext.Session.GetString("Password");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Isuser = HttpContext.Session.GetString("isUsername");
            var Inventlocation = HttpContext.Session.GetString("Inventlocation");
            ViewBag.Username = username;
            ViewBag.Password = Password;
            ViewBag.Role = Role;
            ViewBag.StoreIdRms = StoreIdRms;
            ViewBag.StoreIddynamic = StoreIddynamic;
            ViewBag.isUsername = Isuser;
            TempData["Transfer"] = transferId;
            if (Isuser=="True")
            {
                Parobj.ToStore = Inventlocation;
            }
            //Parobj.InvoiceStatus= InvoiceStatus;
            var x = TempData["InvoiceStatus"];
            var Re = TempData["Result"];
            ViewBag.Re = Re;
            //if (Re != null)
            //{
            //    return RedirectToAction("ATransferSol", "transfersalespdf2");
            //}
            DataCenterContext db = new DataCenterContext();
            if (Parobj.actionValue == 2)
            {
                ViewBag.Add = "T";
            }
            db.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            ViewBag.TransferId = transferId;
            List<CK.Models.R> reportDataList = new List<CK.Models.R>();
            IQueryable<Storeuser> queryAll; // Initialize to avoid null reference exception
            if (Parobj.isDmanager || Parobj.isUsername || Parobj.isFmanager)
            {
                // If the username matches either the Dmanager or the Username, filter the stores accordingly
                queryAll = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username || s.Fmanager == username);
            }
            else
            {
                // If neither condition is met, display all stores
                queryAll = db2.Storeusers.Where(x => x.Storenumber != "RMS");
            }
            // Transform and assign to ViewBag
            ViewBag.VBStore = queryAll
            .GroupBy(m => m.Name)
               .Select(group => new { Inventlocation = group.First().Inventlocation, Name = group.Key })
               .OrderBy(m => m.Name)
               .ToList();
            string conn = null;
            string Select = null;
            string UpdateStatusinTop = @"
update s 
set s.[Status]=a.[Status]
from TopSoft.dbo.TransferDetails s
inner join (
    SELECT distinct 
Trline.Transferid AS [Transaction],
CASE 
   WHEN Trline.Remainstatus = 2 THEN 'Created' 
   WHEN Trline.Remainstatus = 1 THEN 'Shipped' 
   ELSE 'Received' 
   END AS Status
FROM 
    [192.168.1.210].AXDB.dbo.Inventtransferline AS Trline
	INNER JOIN 
    [192.168.1.210].AXDB.dbo.Inventtransfertable AS Trtable ON Trline.Transferid = Trtable.Transferid
  where Trline.Transferid = Trtable.Transferid
) a ON s.[Transaction] COLLATE SQL_Latin1_General_CP1_CI_AS = a.[Transaction]
       ";
            string ReviewTransfer = @"
update s 
set s.[Compare]='Not Confirmed'
from TopSoft.dbo.TransferDetails s
where [Transaction] in (
SELECT 
    TopS.[Transaction] 
    --,TopS.Barcode AS TopSoftBarcode, 
    --Ax.Barcode AS AXDBBarcode,
    --TopS.Quantity AS TopSoftQuantity, 
    --Ax.Quantity AS AXDBQuantity
FROM 
    TopSoft.dbo.TransferDetails AS TopS
LEFT JOIN (
    SELECT DISTINCT Trtable.INVENTLOCATIONIDFROM AS FromTable, ca.Itemid AS Barcode, It.Name AS Item,
           Trline.Qtytransfer AS Quantity, Trtable.INVENTLOCATIONIDTO AS ToTable, 
           CONVERT(VARCHAR, Trline.Receivedate, 101) AS Day, Trline.CREATEDDATETIME, 
           Trline.Transferid AS [Transaction]
    FROM [192.168.1.210].AXDB.dbo.Inventtransferline AS Trline
    INNER JOIN [192.168.1.210].AXDB.dbo.Inventtransfertable AS Trtable ON Trline.Transferid = Trtable.Transferid
    INNER JOIN [192.168.1.210].AXDB.dbo.Inventtable AS ca ON Trline.Itemid = ca.Itemid
    INNER JOIN [192.168.1.210].AXDB.dbo.Ecoresproducttranslation AS It ON ca.Product = It.Product
) Ax ON TopS.[Transaction] COLLATE SQL_Latin1_General_CP1_CI_AS = Ax.[Transaction]
       AND TopS.Barcode COLLATE SQL_Latin1_General_CP1_CI_AS = Ax.Barcode
       AND TopS.Quantity = Ax.Quantity
WHERE 
    Ax.[Transaction] IS NULL OR Ax.Barcode IS NULL OR Ax.Quantity IS NULL
	)
	and s.[ManagerAction]in ('Approved' , 'UnApproved') ";
             Select = @" Select ";
            if (Parobj.InvoiceStatus == "Approved" || Parobj.InvoiceStatus == "UnApproved" || ManagerAction != null)
            {
                Select = UpdateStatusinTop+ ReviewTransfer+@" Select ";
            }
            Select += @" distinct 
 fromTable, ToTable,Status,[Transaction] 
";
            if (transferId != null)
            {
                Select += " ,max(Createddatetime)Createddatetime,Day ,Quantity ,Item  ,Barcode ";
            }
            if (Parobj.InvoiceStatus == "Approved" || Parobj.InvoiceStatus == "UnApproved" || ManagerAction != null)
            {
                Select += " ,ManagerAction,TransferMessage,Compare ";
            }
            if (Parobj.InvoiceStatus == "Approved" || Parobj.InvoiceStatus == "UnApproved" || ManagerAction != null)
            {
                conn = Parobj.TopSoftConnection;
                Select += " from TransferDetails where Barcode is not null ";
            }
            else
            {
                conn = Parobj.AxdbConnection;
                Select += " from ("+(Parobj.RptTransfer)+")Rs where Barcode is not null ";
            }
            string Where = null;
            if (Parobj.startDate != null && Parobj.endDate != null)
            {
                Where += " and cast(CREATEDDATETIME as date) between @fromDate and @toDate ";
            }
            if (Parobj.Store != "0" && Parobj.Store != null)
            {
                Where += " and fromtable=@Branch ";
            }
            if (Parobj.ToStore != "0" && Parobj.ToStore != null)
            {
                Where += " and totable=@toBranch ";
            }
            if (Parobj.InvoiceStatus != "0" && Parobj.InvoiceStatus != null)
            {
                if (Parobj.InvoiceStatus == "Approved" || Parobj.InvoiceStatus == "UnApproved")
            {
                Where += " and ManagerAction=@InvoiceStatus ";
            }
            else if (Parobj.InvoiceStatus == "Created")
                Where += " and Status in (@InvoiceStatus , 'Shipped') ";
                else
                    Where += " and Status = @InvoiceStatus  ";
            }
            if (transferId != null)
            {
                Where += " and [Transaction]=@Transferid ";
                if (ManagerAction != null)
                {
                    Where += " and  ManagerAction = '" + ManagerAction + "' ";
                }
            }
            string groupby = " group by fromTable, ToTable,Status,[Transaction] ";
            if (Parobj.InvoiceStatus == "Approved" || Parobj.InvoiceStatus == "UnApproved" || ManagerAction != null)
            {
                groupby += " ,ManagerAction,TransferMessage,Compare ";
            }
            string orderby = null;
            if (transferId != null)
            {
                groupby += " ,Day ,Quantity ,Item  ,Barcode ";
                orderby += " order by max(Createddatetime) ";
            }
            string SqlQuery = Select +Where+ groupby +orderby;
            //if (Parobj.InvoiceStatus== "Approved" || Parobj.InvoiceStatus == "UnApproved")
            //{
            //    SqlQuery = Parobj.RptApprovedTrans;
            //}
            DateTime startDateTime = Convert.ToDateTime(Parobj.startDate, new CultureInfo("en-GB"));
            DateTime endDateTime = Convert.ToDateTime(Parobj.endDate, new CultureInfo("en-GB"));
            bool shouldCheckStore = Parobj.Store != "0";
            //string formattedStartDateTime = startDateTime.ToString("yyyy-MM-dd");
            //string formattedendDateTime = endDateTime.ToString("yyyy-MM-dd");
            try
            {
                if (Parobj.startDate != null || transferId != null)
                {
                    using (SqlConnection connection = new SqlConnection(conn))
                    {
                        string SqlUpdateStatus = @"
update a
set a.[Status]=s.[Status]
from 
 [192.168.1.208\New].TopSoft.dbo.TransferDetails a
inner join 
(SELECT distinct  
Trline.Transferid AS [Transaction],
CASE 
   WHEN Trline.Remainstatus = 2 THEN 'Created' 
   WHEN Trline.Remainstatus = 1 THEN 'Shipped' 
   ELSE 'Received' 
   END AS Status
FROM 
    AXDB.dbo.Inventtransferline AS Trline
INNER JOIN 
    AXDB.dbo.Inventtransfertable AS Trtable ON Trline.Transferid = Trtable.Transferid
  where Trline.Transferid = Trtable.Transferid)
 s on a.[Transaction]=s.[Transaction] collate SQL_Latin1_General_CP1_CI_AS ";
                        using (SqlCommand command = new SqlCommand(SqlQuery, connection))
                        {
                            connection.Open(); // Open the connection
                                               //command.Parameters.Add(new SqlParameter("@Alert", Message));
                            if (transferId != null)
                            {
                                command.Parameters.Add(new SqlParameter("@Transferid", transferId));
                            }
                            if (!string.IsNullOrEmpty(Parobj.startDate))
                            {
                                command.Parameters.AddWithValue("@fromDate", startDateTime.Date.ToString("yyyy-MM-dd"));
                            }
                            if (!string.IsNullOrEmpty(Parobj.endDate))
                            {
                                command.Parameters.AddWithValue("@toDate", endDateTime.Date.ToString("yyyy-MM-dd"));
                            }
                            if (Parobj.Store != "0" && Parobj.Store != null)
                            {
                                command.Parameters.Add(new SqlParameter("@Branch", Parobj.Store));
                            }
                            if (Parobj.ToStore != "0" && Parobj.ToStore != null)
                            {
                                command.Parameters.Add(new SqlParameter("@toBranch", Parobj.ToStore));
                            }
                            if (Parobj.InvoiceStatus != "0" && Parobj.InvoiceStatus != null)
                            {
                                command.Parameters.Add(new SqlParameter("@InvoiceStatus", Parobj.InvoiceStatus));
                            }
                            var vi = new List<RptSale>();
                            double Qty = 0;
                            var test = command.ExecuteReader();
                            while (test.Read())
                            {
                                RptSale si = new RptSale();
                                si.StoreName = test["FromTable"].ToString();
                                si.TransactionNumber = test["Transaction"].ToString();
                                si.StoreFranchise = test["ToTable"].ToString();
                                if (Parobj.InvoiceStatus == "Approved" || Parobj.InvoiceStatus == "UnApproved" || ManagerAction != null)
                                {
                                    ViewBag.DpName = "V";
                                    si.DpName = test["ManagerAction"].ToString();
                                    si.SupplierName = test["TransferMessage"].ToString();
                                    si.DpId = test["Compare"].ToString();
                                }
                                if (transferId != null)
                                {
                                    ViewBag.TransferId = "V";
                                    si.Dmanager = test["Day"].ToString();
                                    si.ItemName = test["Item"].ToString();
                                    si.ItemLookupCode = test["Barcode"].ToString();
                                    si.Qty = Convert.ToDecimal(test["Quantity"]);
                                    si.Username = test["Createddatetime"].ToString();
                                }
                                si.Fmanager = test["Status"].ToString();
                                //ViewBag.Branches = si.StoreName;
                                //DateTime startDate = Convert.ToDateTime(test["StartDate"]);
                                //DateTime endDate = Convert.ToDateTime(test["EndDate"]);
                                //// Assigning the formatted date strings to ViewBag
                                //ViewBag.SupplierName = startDate.ToString("yyyy-MM-dd");
                                //ViewBag.Dmanager = endDate.ToString("yyyy-MM-dd");
                                vi.Add(si);
                            }
                            ViewBag.Data = vi;
                        }
                    }
                }

            }
            catch
            {
                return View();
            }

                    //var orderedVi = vi.OrderBy(sale => sale.StoreName).ToList();
       //             if (Parobj.startDate!=null)
       //     {
       //         var reportDataQuery = db.Rs.Where(d => d.Transaction == transferId)
       //.GroupBy(d => new { d.Quantity, d.FromTable, d.ToTable, d.Item, d.Barcode, d.Day, d.Transaction, d.Status })
       //.Select(g => new
       //{
       //    Quantity = g.Key.Quantity,
       //    FromTable = g.Key.FromTable,
       //    Item = g.Key.Item,
       //    Barcode = g.Key.Barcode,
       //    Day = g.Key.Day,
       //    Transaction = g.Key.Transaction,
       //    ToTable = g.Key.ToTable,
       //    Status = g.Key.Status,
       //    Createddatetime = g.Max(d => d.Createddatetime) // Extract Createddatetime
       //})
       //.OrderBy(g => g.Createddatetime) // Order by Createddatetime
       //.ToList();

       //         // Transform the results into a list of CK.Models.R objects
       //         reportDataList = reportDataQuery
       //            .Select(g => new CK.Models.R
       //            {
       //                FromTable = g.FromTable,
       //                Quantity = g.Quantity,
       //                Item = IsArabic(g.Item) ? g.Item : g.Item, // Apply the logic you need here if necessary
       //                Barcode = g.Barcode,
       //                Day = g.Day,
       //                Transaction = g.Transaction,
       //                ToTable = g.ToTable,
       //                Status = g.Status,
       //                Createddatetime = g.Createddatetime
       //            })
       //            .ToList();

       //         // Apply specific conditions based on whether isUsername is true or false
       //         if (Isuser == "True")
       //         {
       //             reportDataList = reportDataList
       //                .Where(d => d.Transaction == transferId && (d.ToTable == Inventlocation || d.FromTable == Inventlocation))
       //                .ToList();
       //         }

       //         return View(reportDataList);
       //     }
            return View();

        }


        [HttpGet]
        public IActionResult SalesPdf(string? salesid)
        {
            CkproUsersContext db2 = new CkproUsersContext();
            var username = HttpContext.Session.GetString("Username");
            var Password = HttpContext.Session.GetString("Password");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Isuser = HttpContext.Session.GetString("isUsername");
            var Inventlocation = HttpContext.Session.GetString("Inventlocation");
            bool isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));
            ViewBag.Username = username;
            ViewBag.Password = Password;
            ViewBag.Role = Role;
            ViewBag.StoreIdRms = StoreIdRms;
            ViewBag.StoreIddynamic = StoreIddynamic;
            ViewBag.isUsername = Isuser;
            DataCenterContext db = new DataCenterContext();
            ViewBag.SalesId = salesid;
            List<CK.Models.SalesPdf> reportDataList = new List<CK.Models.SalesPdf>();

            if (!string.IsNullOrEmpty(salesid))
            {
                var reportData = db.SalesPdfs
                    .Where(d => d.Transaction == salesid )
                    .GroupBy(d => new { d.FromTable, d.ToTable, d.Item, d.Barcode, d.Day, d.Transaction, d.Status })
                    .Select(g => new
                    {
                        FromTable = g.Key.FromTable,
                        Quantity = g.Sum(d => d.Quantity),
                        Item = g.Key.Item,
                        Barcode = g.Key.Barcode,
                        Day = g.Key.Day,
                        Transaction = g.Key.Transaction,
                        ToTable = g.Key.ToTable,
                        Status = g.Key.Status,
                        Createddatetime = g.Max(d => d.Createddatetime) // Extract Createddatetime
                    })
                    .OrderBy(g => g.Createddatetime) // Order by Createddatetime
                    .ToList();

                reportDataList = reportData
                    .Select(g => new CK.Models.SalesPdf
                    {
                        FromTable = g.FromTable,
                        Quantity = g.Quantity,
                        Item = g.Item,
                        Barcode = g.Barcode,
                        Day = g.Day,
                        Transaction = g.Transaction,
                        ToTable = g.ToTable,
                        Status = g.Status,
                        Createddatetime = g.Createddatetime
                    })
                    .ToList();
            }
            if (Isuser =="True")
            {
                reportDataList = reportDataList
                   .Where(d => d.Transaction == salesid && (d.ToTable == Inventlocation || d.FromTable == Inventlocation))
                   .ToList();
            }

            return View(reportDataList);
        }

        private bool IsArabic(string text)
        {
            return Regex.IsMatch(text, @"\p{IsArabic}");
        }
       
        [HttpGet]
        public async Task<IActionResult> PdfExportPage(string transferId, string? salesid)
        {
            CkproUsersContext db2 = new CkproUsersContext();
            var username = HttpContext.Session.GetString("Username");
            var Password = HttpContext.Session.GetString("Password");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Isuser = HttpContext.Session.GetString("isUsername");
            var Inventlocation = HttpContext.Session.GetString("Inventlocation");
            bool isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));
            string FromS = "", ToS = "", Tran = "", Day = "", status = "";
            int pages = 0;
            List<R> CheckC = new List<R>();
            List<SalesPdf> CheckA = new List<SalesPdf>();
            using (var db = new DataCenterContext())
            {
                IQueryable<CK.Models.R> reportData1 = Enumerable.Empty<CK.Models.R>().AsQueryable();
                if (!string.IsNullOrEmpty(transferId))
                {
                    CheckC = db.Rs
                        .Where(d => d.Transaction == transferId )
                        .GroupBy(d => new { d.Quantity, d.FromTable, d.ToTable, d.Item, d.Barcode, d.Day, d.Transaction, d.Status, d.Createddatetime })
                        .Select(g => new R
                        {
                            Quantity = g.Key.Quantity,
                            FromTable = g.Key.FromTable,
                            Item = g.Key.Item,
                            Barcode = g.Key.Barcode,
                            Day = g.Key.Day,
                            Transaction = g.Key.Transaction,
                            ToTable = g.Key.ToTable,
                            Status = g.Key.Status,
                            Createddatetime = g.Key.Createddatetime
                        })
                        .OrderBy(x => x.Createddatetime) // Order by Createddatetime after grouping
                        .ToList();
                    if (Isuser =="True")
                    {
                        CheckC = CheckC
                           .Where(d => d.Transaction == transferId && (d.ToTable == Inventlocation || d.FromTable == Inventlocation))
                           .ToList();
                    }
                }

                else if (!string.IsNullOrEmpty(salesid))
                {
                    CheckA = db.SalesPdfs
                        .Where(d => d.Transaction == salesid )
                        .GroupBy(d => new { d.FromTable, d.ToTable, d.Item, d.Barcode, d.Day, d.Transaction, d.Status, d.Createddatetime })
                        .Select(g => new SalesPdf
                        {
                            FromTable = g.Key.FromTable,
                            Quantity = g.Sum(d => d.Quantity),
                            Item = g.Key.Item,
                            Barcode = g.Key.Barcode,
                            Day = g.Key.Day,
                            Transaction = g.Key.Transaction,
                            ToTable = g.Key.ToTable,
                            Status = g.Key.Status,
                            Createddatetime = g.Key.Createddatetime
                        })
                        .OrderBy(x => x.Createddatetime) // Order by Createddatetime for SalesPdf
                        .ToList();

                    if (Isuser =="True")
                    {
                        CheckA = CheckA
                           .Where(d => d.Transaction == salesid && (d.ToTable == Inventlocation || d.FromTable == Inventlocation))
                           .ToList();
                    }
                }
                if (CheckC.Any())
                {
                    FromS = CheckC.Select(x => x.FromTable).FirstOrDefault()!;
                    ToS = CheckC.Select(x => x.ToTable).FirstOrDefault()!;
                    Tran = CheckC.Select(x => x.Transaction).FirstOrDefault()!;
                    Day = CheckC.Select(x => x.Day).FirstOrDefault()!;
                    status = CheckC.Select(x => x.Status).FirstOrDefault()!;
                    pages = (int)Math.Ceiling((decimal)CheckC.Count() / 25);
                }
                else if (CheckA.Any())
                {
                    FromS = CheckA.Select(x => x.FromTable).FirstOrDefault()!;
                    ToS = CheckA.Select(x => x.ToTable).FirstOrDefault()!;
                    Tran = CheckA.Select(x => x.Transaction).FirstOrDefault()!;
                    Day = CheckA.Select(x => x.Day).FirstOrDefault()!;
                    status = CheckA.Select(x => x.Status).FirstOrDefault()!;
                    pages = (int)Math.Ceiling((decimal)CheckA.Count() / 25);
                }
                var result = CheckC.Any() ?
                    CheckC.Select(x => new { Q = x.Quantity.ToString("0.00"), x.Item, x.Barcode }) :
                    CheckA.Select(x => new { Q = x.Quantity.ToString("0.00"), x.Item, x.Barcode });

                MemoryStream ms = new MemoryStream();
                using (var pdfDoc = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
                    pdfDoc.Open();

                    // Load the Arabic supporting font
                    BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\arial.ttf", BaseFont.IDENTITY_H, true);
                    iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL);
                    iTextSharp.text.Font fontB = new iTextSharp.text.Font(bf, 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                    string webRootPath = _env.WebRootPath;
                    // Load the image
                    string imagePath = Path.Combine(webRootPath, "images/CircleK.png");

                    void AddLogo(PdfWriter writer)
                    {
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imagePath);
                        logo.ScaleToFit(120f, 120f);
                        logo.Alignment = iTextSharp.text.Image.ALIGN_RIGHT;
                        logo.SetAbsolutePosition(350, 750); // Adjust position as needed
                        writer.DirectContent.AddImage(logo);
                    }

                    // Add the logo on the first page
                    AddLogo(writer);

                    // Header table and other setup
                    void AddHeaders(PdfWriter writer, Document pdfDoc, string Tran, string status, string Day)
                    {
                        // Header table and other setup
                        PdfPTable tbHeader = new PdfPTable(1)
                        {
                            TotalWidth = pdfDoc.PageSize.Width - pdfDoc.LeftMargin - pdfDoc.RightMargin,
                            DefaultCell = { Border = 0 }
                        };
                        PdfPCell cellHeader = new PdfPCell(new Phrase("", fontB))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Border = 0
                        };
                        tbHeader.AddCell(cellHeader);
                        tbHeader.WriteSelectedRows(0, -1, pdfDoc.LeftMargin, pdfDoc.TopMargin, writer.DirectContent);

                        ColumnText Textl = new ColumnText(writer.DirectContent);
                        Textl.SetSimpleColumn(new Rectangle(20, 800, 523, 100));
                        Chunk linebreak = new Chunk();
                        if (transferId != null)
                        {
                            linebreak.Font = fontB;
                            linebreak = new Chunk("Transfer");
                            Textl.AddElement(linebreak);
                            Textl.Go();
                        }
                        else
                        {
                            linebreak.Font = fontB;
                            linebreak = new Chunk("Sales");
                            Textl.AddElement(linebreak);
                            Textl.Go();
                        }

                        // Draw Line
                        ColumnText Texttl = new ColumnText(writer.DirectContent);
                        Texttl.SetSimpleColumn(new Rectangle(20, 780, 523, 100));
                        Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1.0F, 60.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                        Texttl.AddElement(p);
                        Texttl.Go();

                        // Number and Date
                        ColumnText ct = new ColumnText(writer.DirectContent);
                        ct.SetSimpleColumn(new Rectangle(350, 780, 523, 100));
                        ct.Go();

                        ColumnText TTrans = new ColumnText(writer.DirectContent);
                        TTrans.SetSimpleColumn(new Rectangle(350, 760, 600, 100));
                        if (salesid != null)
                        {
                            TTrans.AddElement(new Paragraph("sales Number: " + Tran));

                        }
                        else
                        {
                            TTrans.AddElement(new Paragraph("Transaction Number: " + Tran));
                        }
                        TTrans.Go();

                        ColumnText TStatus = new ColumnText(writer.DirectContent);
                        TStatus.SetSimpleColumn(new Rectangle(350, 740, 523, 100));
                        TStatus.AddElement(new Paragraph("Status: " + status));
                        TStatus.Go();

                        ColumnText TDay = new ColumnText(writer.DirectContent);
                        TDay.SetSimpleColumn(new Rectangle(350, 720, 523, 100));
                        TDay.AddElement(new Paragraph("Date: " + Day));
                        TDay.Go();
                    }

                    AddHeaders(writer, pdfDoc, Tran, status, Day);

                    // From / To table
                    PdfPTable tablFT = new PdfPTable(2);
                    PdfPCell cellFT1 = new PdfPCell(new Phrase(20, "From", fontB)) { BackgroundColor = new BaseColor(System.Drawing.Color.Gray) };
                    PdfPCell cellFT2 = new PdfPCell(new Phrase(20, "To", fontB)) { BackgroundColor = new BaseColor(System.Drawing.Color.Gray) };
                    PdfPCell cellFT3 = new PdfPCell(new Phrase(20, FromS, font)) { FixedHeight = 40 };
                    PdfPCell cellFT4 = new PdfPCell(new Phrase(20, ToS, font)) { FixedHeight = 40 };

                    tablFT.AddCell(cellFT1);
                    tablFT.AddCell(cellFT2);
                    tablFT.AddCell(cellFT3);
                    tablFT.AddCell(cellFT4);

                    tablFT.TotalWidth = writer.PageSize.Width - 120;
                    tablFT.WriteSelectedRows(0, -1, 60, 690, writer.DirectContent);
                    tablFT.SpacingAfter = 100;

                    // Items Table
                    PdfPTable tabled = new PdfPTable(4)
                    {
                        SpacingBefore = 30,
                        SpacingAfter = 30,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    tabled.SetWidths(new float[] { 1f, 4f, 5f, 2f });

                    PdfPCell cell1 = new PdfPCell(new Phrase(20, "No.", fontB)) { BackgroundColor = new BaseColor(System.Drawing.Color.Gray), FixedHeight = 20, HorizontalAlignment = Element.ALIGN_CENTER };
                    PdfPCell cell2 = new PdfPCell(new Phrase(20, "Barcode", fontB)) { BackgroundColor = new BaseColor(System.Drawing.Color.Gray), HorizontalAlignment = Element.ALIGN_CENTER };
                    PdfPCell cell3 = new PdfPCell(new Phrase(20, "Item Name", fontB)) { BackgroundColor = new BaseColor(System.Drawing.Color.Gray), HorizontalAlignment = Element.ALIGN_CENTER };
                    PdfPCell cell4 = new PdfPCell(new Phrase(20, "Quantity", fontB)) { BackgroundColor = new BaseColor(System.Drawing.Color.Gray), HorizontalAlignment = Element.ALIGN_CENTER };

                    tabled.AddCell(cell1);
                    tabled.AddCell(cell2);
                    tabled.AddCell(cell3);
                    tabled.AddCell(cell4);

                    int nome = 1;
                    int pa = 1;
                    foreach (var item in result.ToList())
                    {
                        PdfPCell cell5 = new PdfPCell(new Phrase(10, nome.ToString(), font)) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 };
                        PdfPCell cell6 = new PdfPCell(new Phrase(30, item.Barcode, font)) { HorizontalAlignment = Element.ALIGN_CENTER };
                        PdfPCell cell7 = new PdfPCell(new Phrase(30, item.Item, font)) { HorizontalAlignment = Element.ALIGN_CENTER, RunDirection = PdfWriter.RUN_DIRECTION_RTL };
                        PdfPCell cell8 = new PdfPCell(new Phrase(10, item.Q.ToString(), font)) { HorizontalAlignment = Element.ALIGN_CENTER };

                        tabled.AddCell(cell5);
                        tabled.AddCell(cell6);
                        tabled.AddCell(cell7);
                        tabled.AddCell(cell8);

                        nome++;
                        pa++;

                        if (pa > 25)
                        {
                            tabled.TotalWidth = writer.PageSize.Width - 120;
                            tabled.WriteSelectedRows(0, -1, 60, 620, writer.DirectContent);
                            tabled.HorizontalAlignment = Element.ALIGN_CENTER;
                            tabled.SpacingBefore = 30;

                            PdfPTable tbFooter1 = new PdfPTable(2)
                            {
                                TotalWidth = pdfDoc.PageSize.Width - pdfDoc.LeftMargin - pdfDoc.RightMargin,
                                DefaultCell = { Border = 0 }
                            };

                            PdfPCell _cellf3 = new PdfPCell(new Phrase(30, "توقيع المستلم :------------------", font))
                            {
                                RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                                HorizontalAlignment = Element.ALIGN_LEFT,
                                Border = 0
                            };
                            tbFooter1.AddCell(_cellf3);
                            tbFooter1.WriteSelectedRows(0, -1, pdfDoc.LeftMargin, writer.PageSize.GetBottom(pdfDoc.BottomMargin) + 100, writer.DirectContent);

                            PdfPCell _cellf = new PdfPCell(new Paragraph("Page: " + writer.PageNumber + " of " + pages))
                            {
                                HorizontalAlignment = Element.ALIGN_RIGHT,
                                Border = 0
                            };

                            tbFooter1.AddCell(_cellf);
                            tbFooter1.WriteSelectedRows(0, -1, pdfDoc.LeftMargin, writer.PageSize.GetBottom(pdfDoc.BottomMargin) + 50, writer.DirectContent);

                            pdfDoc.NewPage();
                            pa = 1;
                            AddLogo(writer); // Add the logo on the new page
                            AddHeaders(writer, pdfDoc, Tran, status, Day);

                            /////////////////////////////From / To table \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                            tablFT = new iTextSharp.text.pdf.PdfPTable(2);
                            tablFT.AddCell(cellFT1);
                            tablFT.AddCell(cellFT2);
                            tablFT.AddCell(cellFT3);
                            tablFT.AddCell(cellFT4);


                            tablFT.TotalWidth = writer.PageSize.Width - 120;
                            tablFT.WriteSelectedRows(0, -1, 60, 690, writer.DirectContent);
                            tablFT.PaddingTop = 300;
                            tablFT.SpacingBefore = 300;
                            tablFT.SpacingAfter = 100;
                            tabled = new PdfPTable(4)
                            {
                                SpacingBefore = 30,
                                SpacingAfter = 30,
                                HorizontalAlignment = Element.ALIGN_CENTER
                            };
                            tabled.SetWidths(new float[] { 1f, 4f, 5f, 2f });

                            tabled.AddCell(cell1);
                            tabled.AddCell(cell2);
                            tabled.AddCell(cell3);
                            tabled.AddCell(cell4);
                        }
                    }

                    // Add the table to the document
                    tabled.TotalWidth = writer.PageSize.Width - 120;
                    tabled.WriteSelectedRows(0, -1, 60, 620, writer.DirectContent);
                    tabled.HorizontalAlignment = Element.ALIGN_CENTER;
                    tabled.SpacingBefore = 30;

                    // Footer
                    PdfPTable tbFooter = new PdfPTable(2)
                    {
                        TotalWidth = pdfDoc.PageSize.Width - pdfDoc.LeftMargin - pdfDoc.RightMargin,
                        DefaultCell = { Border = 0 }
                    };

                    PdfPCell _cellf2 = new PdfPCell(new Phrase(30, "توقيع المستلم :------------------", font))
                    {
                        RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        Border = 0
                    };
                    tbFooter.AddCell(_cellf2);

                    PdfPCell _cellf1 = new PdfPCell(new Paragraph("Page: " + writer.PageNumber + " of " + pages))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = 0
                    };
                    tbFooter.AddCell(_cellf1);
                    tbFooter.WriteSelectedRows(0, -1, pdfDoc.LeftMargin, writer.PageSize.GetBottom(pdfDoc.BottomMargin) + 50, writer.DirectContent);

                    pdfDoc.Close();
                }
                if (transferId != null)
                {
                    return File(ms.ToArray(), "application/pdf", "ATransfer.pdf");
                }
                else
                {
                    return File(ms.ToArray(), "application/pdf", "sales.pdf");

                }
            }
        }

        public IActionResult SalesPdf()
        {
            return View();
        }
    }
}
