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
using Microsoft.IO;
namespace CK.Controllers
{
   [Authorize]
    public class TenderController : Controller
    {
        SalesParameters Parobj = new SalesParameters();
        AxdbContext Axdb = new AxdbContext();
        DataCenterContext db = new DataCenterContext();
        CkproUsersContext db2 = new CkproUsersContext();
        CkhelperdbContext db3 = new CkhelperdbContext();
        DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
        private readonly ILogger<TenderController> _logger;

        public TenderController(ILogger<TenderController> logger)
        {
            _logger = logger;
        }
        bool exported = false;
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
            DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
            db.Database.SetCommandTimeout(7200);// Set the timeout in seconds
                                                //Store List Text=StoreName , Value = StoreId
            if (username is null)
            {
                return RedirectToAction("Store", "Home");
            }
            bool isDmanager = db2.RptUsers.Any(s => s.Dmanager == username);
            bool isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));

            IQueryable<Storeuser> query;
            if (isDmanager || isUsername)
            {
                // If the username matches either the Dmanager or the Username, filter the stores accordingly
                query = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username);
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
            using (SqlConnection connection = new SqlConnection(Parobj.AxdbConnection))
            {
                using (SqlCommand command = new SqlCommand("select distinct Name from AXDB.dbo.Retailtendertypetable ", connection))
                {

                    connection.Open(); // Open the connection
                    command.ExecuteNonQuery(); // Execute the command
                    var vi = new List<RptSale>();
                    var test = command.ExecuteReader();
                    while (test.Read())
                    {
                        RptSale si = new RptSale();
                        si.DpName = test["Name"].ToString();
                        vi.Add(si);
                    }
                    ViewBag.Paidtype = vi.Select(x => x.DpName).Distinct().ToList();
                }
            }
            return View();
        }
        [HttpPost]
        public IActionResult Index(SalesParameters Parobj)
        {
            Parobj.MainTender = true;
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
            DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
            db.Database.SetCommandTimeout(7200);// Set the timeout in seconds
                                                //Store List Text=StoreName , Value = StoreId
            if (username is null)
            {
                return RedirectToAction("Store", "Home");
            }
            bool isDmanager = db2.RptUsers.Any(s => s.Dmanager == username);
            bool isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));

            IQueryable<Storeuser> query;
            if (isDmanager || isUsername)
            {
                // If the username matches either the Dmanager or the Username, filter the stores accordingly
                query = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username);
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
            using (SqlConnection connection = new SqlConnection(Parobj.AxdbConnection))
            {
                using (SqlCommand command = new SqlCommand("select distinct Name from AXDB.dbo.Retailtendertypetable ", connection))
                {

                    connection.Open(); // Open the connection
                    command.ExecuteNonQuery(); // Execute the command
                    var vi = new List<RptSale>();
                    var test = command.ExecuteReader();
                    while (test.Read())
                    {
                        RptSale si = new RptSale();
                        si.DpName = test["Name"].ToString();
                        vi.Add(si);
                    }
                    ViewBag.Paidtype = vi.Select(x => x.DpName).Distinct().ToList();
                }
            }
            // Dynamic GroupBy based on selected values
            IQueryable<dynamic> reportData1;
            string[] storeVal = Parobj.Store.Split(':');
            string connectionStringAXDB = string.Format("Server=192.168.1.210;User ID=sa;Password=P@ssw0rd;Database=AXDB;Connect Timeout=7200;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            string connectionString = string.Format("Server=192.168.1.156;User ID=sa;Password=P@ssw0rd;Database=DATA_CENTER;Connect Timeout=7200;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            string connectionString2 = string.Format("Server=192.168.1.156;User ID=sa;Password=P@ssw0rd;Database=DATA_CENTER_Prev_Yrs;Connect Timeout=7200;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");

            // Step 2: Format the connection string dynamically
            //string connectionString = FormatConnectionString(serverIp);
            // Call the ExportToExcel method
            if (Parobj.RichCut)
            {
                return ExportToExcel(Parobj.RmsConnection, Parobj);
            }
            else if (Parobj.RMS && Parobj.TMT)
            {
                return ExportToExcel(connectionString, Parobj);
            }
            else if (Parobj.RMS)
            {
                return ExportToExcel(connectionString, Parobj);
            }
            else if (Parobj.TMT)
            {
                return ExportToExcel(connectionStringAXDB, Parobj);
            }
            else if (Parobj.DBbefore)
            {
                return ExportToExcel(connectionString2, Parobj);
            }
            // if Not RMS or TMT
            else
            {
                return View();
            }
            ViewBag.Data = reportData1;
            //  }
            //TempData["Al"] = "تم الحفظ بفضل الله";
            //var reportData1 = ViewBag.Data as IEnumerable<dynamic>;
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
        public IActionResult TenderFromBranch()
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
            using (SqlConnection connection = new SqlConnection(Parobj.TopSoftConnection))
            {
                using (SqlCommand command = new SqlCommand("SELECT StartDate,EndDate,Id FROM Closeday where StartDate is not null ", connection))
                {
                    connection.Open(); // Open the connection
                    var vi = new List<RptSale>();
                    var test = command.ExecuteReader();
                    while (test.Read())
                    {
                        RptSale si = new RptSale();
                        si.Dmanager = test["StartDate"].ToString();
                        si.Fmanager = test["EndDate"].ToString();
                        si.ConvDate = test["Id"].ToString();
                        ViewBag.Message = si.StoreName;
                        vi.Add(si);
                    }
                    if (vi.Count > 0)
                    {
                        var startDate = vi.FirstOrDefault().Dmanager;
                        var EndDate = vi.FirstOrDefault().Fmanager;
                        ViewBag.Start = startDate;
                        ViewBag.End = EndDate;
                    }
                }
            }
            DataCenterContext db = new DataCenterContext();
            CkproUsersContext db2 = new CkproUsersContext();
            CkhelperdbContext db3 = new CkhelperdbContext();
            DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
            db.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            IQueryable<RptSale> RptSales = db.RptSales;
            IQueryable<RptSalesAxt> RptSalesAxts = db.RptSalesAxts;
            IQueryable<RptSales2> RptSales2s = db4.RptSales2s;
            IQueryable<RptSalesAll> RptSalesAlls = db.RptSalesAlls;
            //Store List Text=StoreName , Value = StoreId
            ViewBag.Username = username;
            bool isDmanager = db2.RptUsers.Any(s => s.Dmanager == username);
            bool isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));

            IQueryable<Storeuser> query;
            if (isDmanager || isUsername)
            {
                // If the username matches either the Dmanager or the Username, filter the stores accordingly
                query = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username);
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
            return View();
        }
        [HttpPost]
        public IActionResult TenderFromBranch(SalesParameters Parobj)
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
            using (SqlConnection connection = new SqlConnection(Parobj.TopSoftConnection))
            {
                using (SqlCommand command = new SqlCommand("SELECT StartDate,EndDate,Id FROM Closeday where StartDate is not null ", connection))
                {
                    connection.Open(); // Open the connection
                    var vi = new List<RptSale>();
                    var test = command.ExecuteReader();
                    while (test.Read())
                    {
                        RptSale si = new RptSale();
                        si.Dmanager = test["StartDate"].ToString();
                        si.Fmanager = test["EndDate"].ToString();
                        si.ConvDate = test["Id"].ToString();
                        ViewBag.Message = si.StoreName;
                        vi.Add(si);
                    }
                    if (vi.Count > 0)
                    {
                        var startDate = vi.FirstOrDefault().Dmanager;
                        var EndDate = vi.FirstOrDefault().Fmanager;
                        ViewBag.Start = startDate;
                        ViewBag.End = EndDate;
                    }
                }
            }
            DataCenterContext db = new DataCenterContext();
            CkproUsersContext db2 = new CkproUsersContext();
            CkhelperdbContext db3 = new CkhelperdbContext();
            DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
            db.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            db3.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            db4.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            ViewBag.VBStore = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username)
                .GroupBy(m => m.Name)
                .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })//group.First().StoreIdD + ":" +
                .OrderBy(m => m.Name)
                .ToList();
            string[] storeVal = Parobj.Store.Split(':');
            // Dynamic GroupBy based on selected values
            IQueryable<dynamic> reportData1;
            string connectionStringAXDB = string.Format("Server=192.168.1.210;User ID=sa;Password=P@ssw0rd;Database=AXDB;Connect Timeout=7200;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            string connectionString = string.Format("Server=192.168.1.156;User ID=sa;Password=P@ssw0rd;Database=DATA_CENTER;Connect Timeout=7200;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            string connectionString2 = string.Format("Server=192.168.1.156;User ID=sa;Password=P@ssw0rd;Database=DATA_CENTER_Prev_Yrs;Connect Timeout=7200;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            Parobj.VPaidtype = true;
            Parobj.VTotalSales = true;
            // Step 2: Format the connection string dynamically
            //string connectionString = FormatConnectionString(serverIp);
            // Call the ExportToExcel method
            Parobj.exportAfterClick = true;
            Parobj.Tenderfrombranch = true;
             if (StoreIddynamic == "RMS")
            {
                return ExportToExcel(Parobj.RmsConnection, Parobj);
            }
            else
                return ExportToExcel(Parobj.AxdbConnection, Parobj);
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
            var Inventlocation = HttpContext.Session.GetString("Inventlocation");
            ViewBag.Username = username;
            ViewBag.Password = Password;
            ViewBag.Role = Role;
            ViewBag.StoreIdRms = StoreIdRms;
            ViewBag.StoreIddynamic = StoreIddynamic;
            ViewBag.isUsername = Isuser;
            DataCenterContext db = new DataCenterContext();
            CkproUsersContext db2 = new CkproUsersContext();
            CkhelperdbContext db3 = new CkhelperdbContext();
            DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
            HttpContext.Session.SetString("ExportStatus", "started");
            // Prepare the SQL query with a parameter placeholder
            // Start building the SELECT clause dynamically
            List<string> selectColumns = new List<string>();
            if (Parobj.VPerDay)
                selectColumns.Add("CAST(transdate as date) as Date");
            if (Parobj.VDateInTime )
                selectColumns.Add("DinTime");
            if (Parobj.VStoreName)
                selectColumns.Add("StoreName as 'Store Name'");
            if (Parobj.VStoreId)
                selectColumns.Add("StoreId as 'StoreId'");
            if (Parobj.Vbatch)
            {
                selectColumns.Add("Batchid");
                selectColumns.Add("Terminalid");
                if (Parobj.Vbatch && !Parobj.Tenderfrombranch)
                {
                    selectColumns.Add("Paidtype");
                }
                selectColumns.Add("Startdate");
                selectColumns.Add("Closeddate");            
            }
            if (Parobj.VTransactionNumber)
                selectColumns.Add("TransactionNumber");
            if (Parobj.VItemLookupCode)
                selectColumns.Add("Itemlookupcode");
            if (Parobj.VTotalSales || Parobj.Vbatch)
                selectColumns.Add("sum(TotalSales)TotalSales");
                if (Parobj.VPaidtype &&!Parobj.Vbatch)
            { 
                    selectColumns.Add("Paidtype");
            }
            // Construct the SELECT clause from the list of columns
            string selectClause = string.Join(", ", selectColumns);
            string fromWhereClause = null;
            DateTime startDateTime = Convert.ToDateTime(Parobj.startDate, new CultureInfo("en-GB"));
            DateTime endDateTime = Convert.ToDateTime(Parobj.endDate, new CultureInfo("en-GB"));

            string[] storeVal = Parobj.Store.Split(':');
            if (Parobj.RichCut)
            {
                fromWhereClause = $"FROM ({Parobj.RichCutRptTender()})RptTender WHERE CAST(TransDate AS DATE) BETWEEN @fromDate AND @toDate ";
            }
            else if (Parobj.RMS &&!Parobj.Vbatch&& Parobj.TMT == false || storeVal[0] == "RMS" || Parobj.DBbefore ||(Parobj.Tenderfrombranch && StoreIddynamic=="RMS"))
            {
                fromWhereClause = $"FROM ({Parobj.RmsRptTender()})RptTender WHERE CAST(TransDate AS DATE) BETWEEN @fromDate AND @toDate ";
            }
            else if (Parobj.RMS == false && Parobj.TMT ||Parobj.Vbatch|| (storeVal.Length > 1 && storeVal[1] == "Dy") || (Parobj.Tenderfrombranch && StoreIddynamic != "RMS"))
            {
                if (Parobj.Vbatch)
                {
                    fromWhereClause = $"FROM ({Parobj.DyRptTenderbybatch})RptTender WHERE DATEADD(hour, 3, Startdate) >= @fromDate AND DATEADD(hour, -5, Closeddate) <= DATEADD(day, 1,  @toDate) ";
                }
                else
                {
                    fromWhereClause = $"FROM ({Parobj.DyRptTender()})RptTender WHERE CAST(TransDate AS DATE) BETWEEN @fromDate AND @toDate ";
                }
            }
              else if (Parobj.RMS && Parobj.TMT)
            {

                fromWhereClause = $"FROM ({Parobj.RptTenderAll()})RptTender WHERE CAST(TransDate AS DATE) BETWEEN @fromDate AND @toDate ";
            }
            else
            {
                return View();
            }
            string MessageBox = string.Empty;
            bool isDmanager = db2.RptUsers.Any(s => s.Dmanager == username);
            bool isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));
            IQueryable<Storeuser> query;
            if (isUsername && Parobj.Tenderfrombranch)
            {
                if (StoreIddynamic == "RMS")
                {
                    fromWhereClause += " And StoreId ='" + StoreIdRms + "' ";                }
                else
                    fromWhereClause += " And StoreId ='" + StoreIddynamic + "' ";
        }
            if (Parobj.Store != "0")
            {
                if (Parobj.RichCut)
                {
                    fromWhereClause += "AND StoreId = SUBSTRING(@Store1, 2, 1) ";
                }
                else if (Parobj.RMS && Parobj.TMT == false || storeVal[0] == "RMS" || Parobj.DBbefore)
                {
                    fromWhereClause += "AND StoreId = @Store1 ";
                }
                else if (Parobj.RMS == false && Parobj.TMT || (storeVal.Length > 1 && storeVal[1] == "Dy"))
                {
                    fromWhereClause += "AND StoreId = @Store ";
                }
                else if (Parobj.RMS && Parobj.TMT)
                {
                    fromWhereClause += "AND (StoreIdD = @Store OR StoreIdR = @Store1) ";

                }
                else
                {
                    fromWhereClause += "AND StoreId = @Store1 ";
                }
            }
            if (!isUsername && Parobj.Paidtypelist != "0")
            {
                fromWhereClause += "AND Paidtype = @Paidtype ";
            }
            string sqlQuery = $"SELECT {selectClause} {fromWhereClause}";
            // Start building the GROUP BY clause dynamically
            List<string> groupByColumns = new List<string>();
            if (Parobj.VDateInTime)
                groupByColumns.Add("DinTime");

            if (Parobj.VStoreName)
                groupByColumns.Add("StoreName");
            if (Parobj.VStoreId)
                groupByColumns.Add("StoreId");
            if (Parobj.Vbatch)
                groupByColumns.Add("Batchid");
            if (Parobj.Vbatch)
                groupByColumns.Add("Terminalid");
            if (Parobj.Vbatch &&!Parobj.Tenderfrombranch)
                groupByColumns.Add("Paidtype");
            //if (Parobj.Vbatch)
            //    groupByColumns.Add("TotalSales");
            if (Parobj.Vbatch)
                groupByColumns.Add("Startdate");
            if (Parobj.Vbatch)
                groupByColumns.Add("Closeddate");
            if (Parobj.VTransactionNumber)
                groupByColumns.Add("TransactionNumber");
            if (Parobj.VPaidtype &&!Parobj.Vbatch)
                groupByColumns.Add("Paidtype");
            if (Parobj.VItemLookupCode)
                groupByColumns.Add("Itemlookupcode");
            if (Parobj.VPerDay)
                groupByColumns.Add("CAST(transdate as date)");
            // Do not include sum(totalsales) in the GROUP BY clause

            // Construct the GROUP BY clause from the list of columns
            string groupByClause = groupByColumns.Count > 0 ? "GROUP BY " + string.Join(", ", groupByColumns) : "";

            sqlQuery += groupByClause;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //string storedProcedureName = "R2"; // Replace with your actual stored procedure name
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    //command.CommandType = CommandType.StoredProcedure;
                    //sqlQuery = "SELECT sum(TotalSales) TotalSales FROM RptSales WHERE CAST(TransDate AS DATE) BETWEEN @fromDate AND @toDate";
                    command.CommandTimeout = 7200;
                    // Add the date parameters to the command if they are not null
                    if (!string.IsNullOrEmpty(Parobj.startDate))
                    {
                        command.Parameters.AddWithValue("@fromDate", startDateTime.Date.ToString("yyyy-MM-dd"));
                    }
                    if (!string.IsNullOrEmpty(Parobj.endDate))
                    {
                        command.Parameters.AddWithValue("@toDate", endDateTime.Date.ToString("yyyy-MM-dd"));
                    }
                    
                    if (!isUsername&& Parobj.Paidtypelist !="0")
                    {
                        command.Parameters.AddWithValue("@Paidtype", Parobj.Paidtypelist);
                    }
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
                            if (storeVal.Length > 1 && int.TryParse(storeVal[0], out int storeId))
                            {
                                command.Parameters.AddWithValue("@Store", storeId);
                            }
                        }
                        else if (Parobj.RMS && Parobj.TMT)// && storeVal[0] != "RMS" && storeVal[1] != "Dy" || (Parobj.RMS && Parobj.TMT && storeVal[0] == "0" && storeVal[1] == "0"))
                        {
                            if (storeVal.Length > 1)
                            {
                                int storeIdd, storeIdr;
                                // Attempt to parse storeVal[0] and storeVal[1] as integers
                                bool isStoreIddParsed = int.TryParse(storeVal[0], out storeIdd);
                                bool isStoreIdrParsed = int.TryParse(storeVal[1], out storeIdr);

                                // Check if at least one of the values was successfully parsed
                                if (isStoreIddParsed || isStoreIdrParsed)
                                {
                                    // If storeVal[0] was successfully parsed, add it as a parameter
                                    if (isStoreIddParsed)
                                    {
                                        command.Parameters.AddWithValue("@Store", storeIdd);
                                    }

                                    // If storeVal[1] was successfully parsed, add it as a parameter
                                    if (isStoreIdrParsed)
                                    {
                                        command.Parameters.AddWithValue("@Store1", storeIdr);
                                    }
                                }
                            }
                        }
                        else
                        {
                            return View();
                        }
                    }
                    if (Parobj.MainTender && (Parobj.actionValue == 1 || Parobj.actionValue == 0))
                    {
                        var vi = new List<RptSale>();
                        var ll = 0;
                        var test = command.ExecuteReader();
                        double TotalSales = 0;
                        double TotalQty = 0;
                        double ts = 0;
                        double tq = 0;
                        double Transactions = 0;
                        DateTime Ddate;
                        while (test.Read())
                        {
                            RptSale si = new RptSale();
                            if (Parobj.Vbatch)
                            {
                                ViewBag.Vbatch = "true";
                                si.Dmanager = test["Batchid"].ToString();
                                si.Fmanager = test["Terminalid"].ToString();
                                //si.StoreName = test["PaidType"].ToString();
                                TotalSales = Convert.ToDouble(test["TotalSales"]);
                                si.StoreFranchise = TotalSales.ToString("#,##0.00");
                                si.DpName = test["Startdate"].ToString();
                                si.Username = test["Closeddate"].ToString();
                            }
                            else
                            {
                                if (Parobj.VPaidtype)
                                {
                                    ViewBag.VPaidtype = "V";
                                    si.Dmanager = test["PaidType"].ToString();
                                }
                                if (Parobj.VStoreId)
                                {
                                    ViewBag.VStoreId = "V";
                                    si.StoreId = test["StoreId"].ToString();
                                }
                                if (Parobj.VStoreName)
                                {
                                    ViewBag.VStoreName = "V";
                                    si.StoreName = test["Store Name"].ToString();
                                }
                                if (Parobj.VTransactionNumber)
                                {
                                    ViewBag.VTransactionNumber = "V";
                                    si.TransactionNumber = test["TransactionNumber"].ToString();
                                }
                                if (Parobj.VTotalSales)
                                {
                                    ViewBag.VTotalSales = "V";
                                    TotalSales = Convert.ToDouble(test["TotalSales"]);
                                    si.StoreFranchise = TotalSales.ToString("#,##0.00");
                                }
                                if (Parobj.VItemLookupCode)
                                {
                                    ViewBag.VItemLookupCode = "V";
                                    si.ItemLookupCode = test["ItemLookupCode"].ToString();
                                }
                                if (Parobj.VPerDay)
                                {
                                    ViewBag.VPerDay = "V";
                                    si.ItemName = test["Date"].ToString();
                                    DateTime parsedDate;
                                    if (DateTime.TryParse(si.ItemName, out parsedDate))
                                    {
                                        si.ItemName = parsedDate.ToString("yyyy-MM-dd");
                                    }
                                    ViewBag.Day = si.ItemName;
                                }
                            }
                            ts += TotalSales;
                            vi.Add(si);
                        }
                        ViewBag.Data = vi;
                        ViewBag.Total = ts.ToString("#,##0.00");
                        ViewBag.Transactions = Transactions.ToString("#,##0.00");
                        return View("Index");
                    }

                    if (Parobj.Tenderfrombranch && Parobj.actionValue == 1)
                    {
                        var vi = new List<RptSale>();
                        var ll = 0;
                        var test = command.ExecuteReader();
                        double TotalSales = 0;
                        double TotalQty = 0;
                        double ts = 0;
                        double tq = 0;
                        double Transactions = 0;
                        DateTime Ddate;
                        while (test.Read())
                        {
                            RptSale si = new RptSale();
                            if (Parobj.Vbatch)
                            {
                                ViewBag.batch = "true";
                                si.Dmanager = test["Batchid"].ToString();
                                si.Fmanager = test["Terminalid"].ToString();
                                //si.StoreName = test["PaidType"].ToString();
                                TotalSales = Convert.ToDouble(test["TotalSales"]);
                                si.StoreFranchise = TotalSales.ToString("#,##0.00");
                                si.DpName = test["Startdate"].ToString();
                                si.Username = test["Closeddate"].ToString();
                            }
                            else
                            {
                                si.StoreName = test["PaidType"].ToString();
                                //si.DpName = test["Department"].ToString();
                                //si.Qty = Convert.ToDecimal(test["TotalQty"]);
                                TotalSales = Convert.ToDouble(test["TotalSales"]);
                                si.StoreFranchise = TotalSales.ToString("#,##0.00");
                                if (Parobj.VPerDay)
                                {
                                    si.ItemName = test["Date"].ToString();
                                    DateTime parsedDate;
                                    if (DateTime.TryParse(si.ItemName, out parsedDate))
                                    {
                                        si.ItemName = parsedDate.ToString("yyyy-MM-dd");
                                    }
                                    ViewBag.Day = si.ItemName;
                                }
                            }
                            ts += TotalSales;
                            vi.Add(si);
                        }
                        ViewBag.Data = vi;
                        ViewBag.Total = ts.ToString("#,##0.00");
                        ViewBag.Transactions = Transactions.ToString("#,##0.00");
                        return View("Tenderfrombranch");
                    }
                    // Create a new Excel package
                    using (var package = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("AKTenderReport");
                        int row = 2; // Start from row 2 to leave space for headers
                        int sheetIndex = 1; // Start with the first sheet
                        int columnCount = 1;
                        // Add header row
                        void AddHeaderRow(ExcelWorksheet ws, int columnCount)
                        {
                            int column = 1;
                            if (Parobj.VDateInTime)
                                ws.Cells[1, column++].Value = "Time";
                            if (Parobj.VStoreId)
                                ws.Cells[1, column++].Value = "StoreId";
                            if (Parobj.VStoreName)
                                ws.Cells[1, column++].Value = "Store Name";
                            if (Parobj.VTransactionNumber)
                                ws.Cells[1, column++].Value = "TransactionNumber";
                            if (Parobj.VItemLookupCode)
                                ws.Cells[1, column++].Value = "BarCode";
                            if (Parobj.VItemName)
                                ws.Cells[1, column++].Value = "ItemName";
                            if (Parobj.VPaidtype && !Parobj.Vbatch)
                                ws.Cells[1, column++].Value = "Tender Type"; 
                            if (Parobj.Vbatch)
                                ws.Cells[1, column++].Value = "Batchid";
                            if (Parobj.Vbatch)
                                ws.Cells[1, column++].Value = "Terminalid";
                            if (Parobj.Vbatch && !Parobj.Tenderfrombranch)
                                ws.Cells[1, column++].Value = "Tender Type";
                            if (Parobj.Vbatch)
                                ws.Cells[1, column++].Value = "TotalSales";
                            if (Parobj.Vbatch)
                                ws.Cells[1, column++].Value = "Startdate";
                            if (Parobj.Vbatch)
                                ws.Cells[1, column++].Value = "Closeddate";
                            if (Parobj.VTotalSales)
                                ws.Cells[1, column++].Value = "TotalSales";
                            if (Parobj.VPerDay)
                                ws.Cells[1, column++].Value = "Date";
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
                                if (Parobj.VDateInTime)
                                    worksheet.Cells[row, columnCount++].Value = reader["DinTime"];
                                if (Parobj.VStoreId)
                                    worksheet.Cells[row, columnCount++].Value = reader["StoreId"];
                                if (Parobj.VStoreName)
                                    worksheet.Cells[row, columnCount++].Value = reader["Store Name"];
                                if (Parobj.VTransactionNumber)
                                    worksheet.Cells[row, columnCount++].Value = reader["TransactionNumber"];
                                if (Parobj.VItemLookupCode)
                                    worksheet.Cells[row, columnCount++].Value = reader["Itemlookupcode"];
                                if (Parobj.VItemName)
                                    worksheet.Cells[row, columnCount++].Value = reader["ItemName"];
                                if (Parobj.Vbatch)
                                    worksheet.Cells[row, columnCount++].Value = reader["Batchid"];
                                if (Parobj.Vbatch)
                                    worksheet.Cells[row, columnCount++].Value = reader["Terminalid"];
                                if (Parobj.Vbatch && !Parobj.Tenderfrombranch)
                                    worksheet.Cells[row, columnCount++].Value = reader["Paidtype"];
                                if (Parobj.Vbatch)
                                    worksheet.Cells[row, columnCount++].Value = reader["TotalSales"];
                                if (Parobj.Vbatch)
                                    worksheet.Cells[row, columnCount++].Value = reader["Startdate"];
                                if (Parobj.Vbatch)
                                    worksheet.Cells[row, columnCount++].Value = reader["Closeddate"];
                                if (Parobj.VPaidtype && !Parobj.Vbatch)
                                    worksheet.Cells[row, columnCount++].Value = reader["Paidtype"];
                                worksheet.Cells[row, columnCount].Style.Numberformat.Format = "#,##0.00";
                                if (Parobj.VTotalSales)
                                    worksheet.Cells[row, columnCount++].Value = reader["TotalSales"];
                                worksheet.Cells[row, columnCount].Style.Numberformat.Format = "yyyy-MM-dd";
                                if (Parobj.VPerDay)
                                    worksheet.Cells[row, columnCount++].Value = reader["Date"];
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
                                    worksheet = package.Workbook.Worksheets.Add($"AKTenderReport{sheetIndex++}");
                                    // Re-add the header row to the new worksheet
                                    row = 2; // Reset row count for the new worksheet
                                    columnCount = 1; // Reset column count
                                                     // Re-add the header row to the new worksheet\
                                    AddHeaderRow(worksheet, columnCount);
                                    if (Parobj.VDateInTime)
                                        worksheet.Cells[row, columnCount++].Value = reader["DinTime"];
                                    if (Parobj.VStoreId)
                                        worksheet.Cells[row, columnCount++].Value = reader["StoreID"];
                                    if (Parobj.VStoreName)
                                        worksheet.Cells[row, columnCount++].Value = reader["Store Name"];
                                    if (Parobj.VTransactionNumber)
                                        worksheet.Cells[row, columnCount++].Value = reader["TransactionNumber"];
                                    if (Parobj.VItemLookupCode)
                                        worksheet.Cells[row, columnCount++].Value = reader["Itemlookupcode"];
                                    if (Parobj.VItemName)
                                        worksheet.Cells[row, columnCount++].Value = reader["ItemName"];
                                    if (Parobj.Vbatch)
                                        worksheet.Cells[row, columnCount++].Value = reader["Batchid"];
                                    if (Parobj.Vbatch)
                                        worksheet.Cells[row, columnCount++].Value = reader["Terminalid"];
                                    if (Parobj.Vbatch)
                                        worksheet.Cells[row, columnCount++].Value = reader["Paidtype"];
                                    if (Parobj.Vbatch)
                                        worksheet.Cells[row, columnCount++].Value = reader["TotalSales"];
                                    if (Parobj.Vbatch)
                                        worksheet.Cells[row, columnCount++].Value = reader["Startdate"];
                                    if (Parobj.Vbatch)
                                        worksheet.Cells[row, columnCount++].Value = reader["Closeddate"];
                                    if (Parobj.VPaidtype)
                                        worksheet.Cells[row, columnCount++].Value = reader["Paidtype"];
                                    worksheet.Cells[row, columnCount].Style.Numberformat.Format = "#,##0.00";
                                    if (Parobj.VTotalSales)
                                        worksheet.Cells[row, columnCount++].Value = reader["TotalSales"];
                                    worksheet.Cells[row, columnCount].Style.Numberformat.Format = "yyyy-MM-dd";
                                    if (Parobj.VPerDay)
                                        worksheet.Cells[row, columnCount++].Value = reader["Date"];
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
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AKSoftTender.xlsx");
                    }
                }
            }
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

}
