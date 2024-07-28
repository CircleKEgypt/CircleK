using System.Text.Json; // For JsonSerializer.Serialize
using System.Collections.Generic;
using CK.Model;
using CK.Models;
using CK.Models.TopSoft;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IO;
using DocumentFormat.OpenXml.Bibliography;
using System.IO;
using iText.Pdfua.Checkers.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using iText.StyledXmlParser.Jsoup.Safety;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.X509;
namespace CK.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //private readonly DataCenterContext _dataCenterContext;
        //    private readonly CkproUsersContext _ckproUsersContext;
        //    public HomeController(DataCenterContext dataCenterContext, CkproUsersContext ckproUsersContext)
        //    {
        //        _dataCenterContext = dataCenterContext;
        //        _ckproUsersContext = ckproUsersContext;
        //    }
        AxdbContext Axdb = new AxdbContext();
        DataCenterContext db = new DataCenterContext();
        CkproUsersContext db2 = new CkproUsersContext();
        CkhelperdbContext db3 = new CkhelperdbContext();
        SalesParameters Parobj = new SalesParameters();
        DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Shelf()
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
        public IActionResult Shelf(SalesParameters Parobj)
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
            // Dynamic GroupBy based on selected values
            IQueryable<dynamic> reportData1;
            string[] storeVal = Parobj.Store.Split(':');
            ViewBag.VBStore = db2.Storeusers
                   .Where(s => s.Dmanager == username || s.Username == username)
               .GroupBy(m => m.Name)
               .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })//group.First().StoreIdD + ":" +
               .OrderBy(m => m.Name)
               .ToList();
            // Step 2: Format the connection string dynamically
            //string connectionString = FormatConnectionString(serverIp);
            // Call the ExportToExcel method
            Parobj.exportAfterClick = true;
            //Parobj.VQty = true;
            //Parobj.VDepartment = true;
            Parobj.VItemName = true;
            //Parobj.VStoreName = true;
            Parobj.VPrice = true;
            Parobj.Shelf = true;
            //Parobj.VTransactionCount = true;
            Parobj.VItemLookupCode = true;
            Parobj.RMS = true;
            Parobj.TMT = true;
            if (StoreIddynamic!="RMS")
            {
                return ExportToExcel(Parobj.AxdbConnection, Parobj);
            }
            else
            {
                return ExportToExcel(Parobj.RmsConnection, Parobj);
            }
            ViewBag.Data = reportData1;
            //  }
            //TempData["Al"] = " „ «·Õ›Ÿ »›÷· «··Â";
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

        }
    //    [HttpGet]
    //    public IActionResult Home1()
    //    {
    //        var username = HttpContext.Session.GetString("Username");
    //        var Password = HttpContext.Session.GetString("Password");
    //        var Role = HttpContext.Session.GetString("Role");
    //        var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
    //        var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
    //        var PriceCategory = HttpContext.Session.GetString("PriceCategory");
    //        var Isuser = HttpContext.Session.GetString("isUsername");
    //        ViewBag.Username = username;
    //        ViewBag.Password = Password;
    //        ViewBag.Role = Role;
    //        ViewBag.StoreIdRms = StoreIdRms;
    //        ViewBag.StoreIddynamic = StoreIddynamic;
    //        ViewBag.isUsername = Isuser;
    //        Parobj.Home = true;
    //        Parobj.SystemAnaysis = true;
    //        if (Role == "TerrManager" || Role == "FoodManager")
    //        {
    //            Parobj.isDmanager = true;
    //            Parobj.isFmanager = true;
    //        }
    //        Parobj.isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));

    //        IQueryable<Storeuser> query;
    //        if (Parobj.isDmanager || Parobj.isUsername || Parobj.isFmanager)
    //        {
    //            // If the username matches either the Dmanager or the Username, filter the stores accordingly
    //            query = db2.Storeusers
    //                .Where(s => s.Dmanager == username || s.Username == username || s.Fmanager == username);
    //        }
    //        else
    //        {
    //            // If neither condition is met, display all stores
    //            query = db2.Storeusers;
    //        }
    //        ViewBag.VBStore = query
    //.GroupBy(m => m.Name)
    //.Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
    //.OrderBy(m => m.Name)
    //.ToList();
    //        ViewBag.VBDepartment = Axdb.Ecorescategories
    //                                              .Select(g => g.Name)
    //                                              .Distinct()
    //                                              .ToList();

    //        //Supplier List Text=SupplierName , Value = Code 
    //        ViewBag.VBSupplier = db.RptSuppliers
    //                                             .Select(g => g.SupplierName)
    //                                             .ToList();

    //        ViewBag.VBStoreFranchise = db.Stores
    //             .Where(m => m.Franchise != null)
    //             .Select(m => m.Franchise)
    //             .Distinct()
    //             .ToList();
    //        return View();
    //    }
        [HttpGet]
        public IActionResult Home1(SalesParameters Parobj)
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
            Parobj.Home = true;
            if (Parobj.Franchise == "0")
                Parobj.VFranchise = true;
            if (Parobj.VStoreName)
            {
                Parobj.VFranchise = true;
               // Parobj.VTransactionCount = true;
            }
            if (Parobj.Department != null && Parobj.Department != "0")
                Parobj.VDepartment = true;
            Parobj.VTotalSales = true;
            if (Parobj.VPerMon)
                Parobj.VPerMon = true;
            if (Parobj.VPerYear)
                Parobj.VPerYear = true;
            Parobj.SystemAnaysis = true;
            if (!Parobj.VFranchise && !Parobj.VStoreName && !Parobj.VDepartment && !Parobj.VYearVinDash  && !Parobj.VPerMon && !Parobj.Area)
            {
                Parobj.VStoreName = true;
                Parobj.VFranchise = true;
            }
            if (Role == "TerrManager" || Role == "FoodManager")
            {
                Parobj.isDmanager = true;
                Parobj.isFmanager = true;
            }
            Parobj.isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));

            IQueryable<Storeuser> query;
            if (Parobj.isDmanager || Parobj.isUsername || Parobj.isFmanager)
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
            if (Parobj.Store ==null)
           Parobj.Store="0";

            return ExportToExcel(Parobj.TopSoftConnection, Parobj);
        }
        [HttpGet]
        public IActionResult EditMessage(SalesParameters Parobj, int Id,string Message, string Target, string StartDate, string EndDate)
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
            using (SqlConnection connection = new SqlConnection(Parobj.TopSoftConnection))
            {
                using (SqlCommand command = new SqlCommand("SELECT * from Messages where Id=@Id ", connection))
                {
                    connection.Open(); // Open the connection
                    //command.Parameters.Add(new SqlParameter("@Alert", Message));
                    command.Parameters.Add(new SqlParameter("@Id", Id));
                    var vi = new List<RptSale>();
                    var test = command.ExecuteReader();
                    while (test.Read())
                    {
                        RptSale si = new RptSale();
                        si.StoreName = test["alert"].ToString();
                        ViewBag.Branches = si.StoreName;
                        si.SupplierName = test["Target"].ToString();
                        ViewBag.SupplierName = si.SupplierName;
                        vi.Add(si);
                    }
                    //var orderedVi = vi.OrderBy(sale => sale.StoreName).ToList();
                    ViewBag.Data = vi;
                    return View();
                }
            }
        }
        [HttpPost]
        public IActionResult EditMessage(int itemId, SalesParameters Paroj, string Message,int id, string Target)
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
            try
            {
                using (SqlConnection connection = new SqlConnection(Paroj.TopSoftConnection))
                {
                    connection.Open(); // Open the connection

                    using (SqlCommand command = new SqlCommand("UPDATE Messages SET Alert = @Message,Target=@Target where Id=@Id", connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Message", Message));
                        command.Parameters.Add(new SqlParameter("@Id", id));
                        command.Parameters.Add(new SqlParameter("@Target", Target));
                        command.ExecuteNonQuery(); // Execute the command
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("CreateMessage", "Home");
        }

        public IActionResult DeleteMessage(SalesParameters Parobj, int Id)
        {
            using (SqlConnection connection = new SqlConnection(Parobj.TopSoftConnection))
            {
                using (SqlCommand command = new SqlCommand("delete from Messages where Id=@Id", connection))
                {
                    connection.Open(); // Open the connection
                    command.Parameters.Add(new SqlParameter("@Id", Id));
                    var test = command.ExecuteReader();
                    return RedirectToAction("CreateMessage", "Home");
                }
            }
        }
        [HttpGet]
        public IActionResult CreateMessage()
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
                using (SqlCommand command = new SqlCommand("SELECT * FROM Messages ", connection))
                {
                    connection.Open(); // Open the connection
                    double TotalSales = 0;
                    var vi = new List<RptSale>();
                    var test = command.ExecuteReader();
                    while (test.Read())
                    {
                        RptSale si = new RptSale();
                        si.StoreName = test["alert"].ToString();
                        si.ItemName = test["Id"].ToString();
                        si.SupplierName = test["Target"].ToString();
                        //si.Dmanager = test["EndDate"].ToString();
                        vi.Add(si);
                    }
                    //var orderedVi = vi.OrderBy(sale => sale.StoreName).ToList();
                    ViewBag.Data = vi;
                    return View("CreateMessage");
                }
            }
            return View("CreateMessage");
        }
        [HttpPost]
        public async Task<IActionResult> CreateMessageIn(SalesParameters Paroj, string Message,string Target)
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
            try
            {
                using (SqlConnection connection = new SqlConnection(Paroj.TopSoftConnection))
                {
                    using (SqlCommand command = new SqlCommand("INSERT INTO  Messages (Alert,Target) VALUES (@Message,@Target)", connection))
                    {

                        command.Parameters.AddWithValue("@Message", Message);
                        command.Parameters.AddWithValue("@Target", Target);
                        //command.Parameters.AddWithValue("@Start", StartDate);
                        //command.Parameters.AddWithValue("@End", EndDate);
                        connection.Open(); // Open the connection
                        command.ExecuteNonQuery(); // Execute the command
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("CreateMessage", "Home");
            }
            return RedirectToAction("CreateMessage", "Home");
        }
        public IActionResult ItemMovements(SalesParameters Parobj)
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
            Parobj.ItemMovements = true;
            return ExportToExcel(Parobj.AxdbConnection, Parobj);
        }
        public IActionResult Home(SalesParameters Parobj)
        {
            TopSoftContext TopSoftDB = new TopSoftContext();
            var username = HttpContext.Session.GetString("Username");
            var Password = HttpContext.Session.GetString("Password");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Isuser = HttpContext.Session.GetString("isUsername");
            //var alert = TopSoftDB.CloseDays.Select(group => group.Alert).ToList();
            try
            {
                using (SqlConnection connection = new SqlConnection(Parobj.TopSoftConnection))
                {
                    using (SqlCommand command = new SqlCommand("SELECT * FROM Messages where alert is not null ", connection))
                    {
                        connection.Open(); // Open the connection
                        var vi = new List<RptSale>();
                        var test = command.ExecuteReader();
                        while (test.Read())
                        {
                            RptSale si = new RptSale();
                            si.StoreName = test["alert"].ToString();
                            si.ItemLookupCode = test["Id"].ToString();
                            si.ItemName = test["Target"].ToString();
                            ViewBag.Message = si.StoreName;
                            vi.Add(si);
                        }
                        if (Isuser=="False")
                        {
                            var StartMessage = vi.Where(sale => sale.ItemName == "Manager").Select(x => x.StoreName).ToList();
                            if (StartMessage.Count > 0)
                                ViewBag.StartMessage = StartMessage[0];
                        }
                        else
                        {
                            var StartMessage = vi.Where(sale => sale.ItemName == "Afterlogin").Select(x => x.StoreName).ToList();
                            if (StartMessage.Count > 0)
                                ViewBag.StartMessage = StartMessage[0];
                        }
                        ViewBag.Data = vi;
                    }
                }
            }
            catch
            {
                return RedirectToAction("Home", "Home");
            }
            //ViewBag.alert = alert[];
            ViewBag.Username = username;
            ViewBag.Password = Password;
            ViewBag.Role = Role;
            ViewBag.StoreIdRms = StoreIdRms;
            ViewBag.StoreIddynamic = StoreIddynamic;
            ViewBag.isUsername = Isuser;
            Parobj.Home = true;
            return View();
            //return ExportToExcel(Parobj.TopSoftConnection, Parobj);
        }
        public IActionResult Dashboard(SalesParameters Parobj)
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
            Parobj.Dashboard = true;
            return ExportToExcel(Parobj.TopSoftConnection, Parobj);
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
            //Store List Text=StoreName , Value = StoreId
                    if (username is null)
            {
                return RedirectToAction("Store", "Home");
            }
            if (Role=="TerrManager"||Role=="FoodManager")
            {
                Parobj.isDmanager = true;
                Parobj.isFmanager = true;
            }
            Parobj.isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));

            IQueryable<Storeuser> queryAll; // Initialize to avoid null reference exception
            IQueryable<Storeuser> queryRM;
            IQueryable<Storeuser> queryDy;
            IQueryable<Storeuser> queryRich;
            if (Parobj.isDmanager || Parobj.isUsername || Parobj.isFmanager)
            {
                // If the username matches either the Dmanager or the Username, filter the stores accordingly
                queryAll = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username || s.Fmanager == username);
                queryRM = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username || s.Fmanager == username);
                queryDy = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username || s.Fmanager == username);
                queryRich = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username || s.Fmanager == username);
            }
            else
            {
                // If neither condition is met, display all stores
                queryAll = db2.Storeusers;
                queryRM = db2.Storeusers.Where(x => x.Storenumber == "RMS" || x.RmsstoNumber == "Dy"); // RMS or Dy
                queryDy = db2.Storeusers.Where(x => x.Storenumber != "RMS"); // Neither RMS nor Dy
                queryRich = db2.Storeusers.Where(x => x.RmsstoNumber == "R1" || x.RmsstoNumber == "R2" || x.RmsstoNumber == "R3"); // Neither RMS nor Dy
            }
            ViewBag.VBStoreRM = queryRM
                 .GroupBy(m => m.Name)
                 .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
                 .OrderBy(m => m.Name)
                 .ToList();

            ViewBag.VBStoreRich = queryRich
               .GroupBy(m => m.Name)
               .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
               .OrderBy(m => m.Name)
               .ToList();
            ViewBag.VBStoreDy = queryDy
                     .GroupBy(m => m.Name)
                     .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
                     .OrderBy(m => m.Name)
                     .ToList();
            ViewBag.VBStore = queryAll
                                .GroupBy(m => m.Name)
                                .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
                                .OrderBy(m => m.Name)
                                .ToList();
            if (Role == "SalesM")
            {
                ViewBag.VBStore = queryDy
                                .GroupBy(m => m.Name)
                                .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
                                .OrderBy(m => m.Name)
                                .ToList();
            }
            //ViewBag.VBStoreRich = queryRich
            //   .GroupBy(m => m.Name)
            //   .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
            //   .OrderBy(m => m.Name)
            //   .ToList();
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
            return View();
        }
         [HttpGet]
        public IActionResult Store()
        {
            var username = HttpContext.Session.GetString("Username");
            var Password = HttpContext.Session.GetString("Password");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Isuser = HttpContext.Session.GetString("isUsername");
            var checkStart = HttpContext.Session.GetString("StartDate");
            var checkEnd = HttpContext.Session.GetString("EndDate");
            ViewBag.checkStart = checkStart;
            ViewBag.checkEnd = checkEnd;
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
                    if (vi.Count>0)
                    {
                        var startDate = vi.FirstOrDefault().Dmanager;
                        var EndDate = vi.FirstOrDefault().Fmanager;
                        ViewBag.Start = startDate;
                        ViewBag.End = EndDate;
                    }
                }
            }
            if (username is null)
            {
                return RedirectToAction("Logout", "Home");

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
        public IActionResult Store(SalesParameters Parobj)
        {
            var username = HttpContext.Session.GetString("Username");
            var Password = HttpContext.Session.GetString("Password");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic");
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Isuser = HttpContext.Session.GetString("isUsername");
            var checkStart = HttpContext.Session.GetString("StartDate");
            var checkEnd = HttpContext.Session.GetString("EndDate");
            ViewBag.checkStart = checkStart;
            ViewBag.checkEnd = checkEnd;
            ViewBag.Username = username;
            ViewBag.Password = Password;
            ViewBag.Role = Role;
            ViewBag.StoreIdRms = StoreIdRms;
            ViewBag.StoreIddynamic = StoreIddynamic;
            ViewBag.isUsername = Isuser;
            if (username is null)
            {
                return RedirectToAction("Logout", "Home");

            }
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
                    if (vi.Count>0)
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
            TopSoftContext TopSoftDB = new TopSoftContext();
            List<DateTime> CloseDate = TopSoftDB.CloseDays
                                             .Where(cd => cd.StartDate.HasValue)
                                             .Select(cd => cd.StartDate.Value)
                                             .Distinct()
                                             .ToList();
            if (!string.IsNullOrEmpty(Parobj.startDate) && !string.IsNullOrEmpty(Parobj.endDate))
            {
                DateTime startDateTime = Convert.ToDateTime(Parobj.startDate, new CultureInfo("en-GB"));
                if (CloseDate.Contains(startDateTime))
                {
                    ViewBag.Mo = "V";
                }
            }
            else
            ViewBag.Mo = "h";
            db.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            db3.Database.SetCommandTimeout(7200);// Set the timeout in seconds
            db4.Database.SetCommandTimeout(7200);// Set the timeout in seconds  
            ViewBag.VBStore = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username)
                .GroupBy(m => m.Name)
                .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })//group.First().StoreIdD + ":" +
                .OrderBy(m => m.Name)
                .ToList();
            // Dynamic GroupBy based on selected values
            IQueryable<dynamic> reportData1;
                Parobj.Store = "0";
            
            string[] storeVal = Parobj.Store.Split(':');
            // Call the ExportToExcel method
            Parobj.VQty = true;
            //Parobj.VTransactionCount = true;
            Parobj.VTotalSales = true;
            Parobj.VTransactionCount = true;
            Parobj.VDepartment = true;
            Parobj.VStoreName = true;
            //Parobj.VItemLookupCode = true;
            //Parobj.VItemName = true;
            Parobj.RMS = true;
            Parobj.TMT = true;
            Parobj.SalesBranchPage = true;
            if (StoreIddynamic=="RMS" || username== "RichCut" || username == "RichCut_No" || username == "RichCut_Zayed")
            {
                return ExportToExcel(Parobj.RmsConnection, Parobj);
            }
            else 
            {
                return ExportToExcel(Parobj.TopSoftConnection, Parobj);
            }

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
        [HttpGet]
        public IActionResult CompareData()
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
            return View();
        }
        [HttpPost]
        public IActionResult CompareData(SalesParameters Parobj)
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
            Parobj.CompareData = true;
            Parobj.VStoreName = true;
            Parobj.VTotalSales = true;
            return ExportToExcel(Parobj.TopSoftConnection, Parobj);
        }

        public IActionResult ExportToExcel(string connectionString, SalesParameters Parobj)
        {
            DataCenterContext db = new DataCenterContext();
            CkproUsersContext db2 = new CkproUsersContext();
            CkhelperdbContext db3 = new CkhelperdbContext();
            DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
            Storeuser n = new Storeuser();
            var username = HttpContext.Session.GetString("Username");
            var Role = HttpContext.Session.GetString("Role");
            var StoreIddynamic = HttpContext.Session.GetString("StoreIddynamic"); 
            var StoreIdRms = HttpContext.Session.GetString("StoreIdRms");
            var PriceCategory = HttpContext.Session.GetString("PriceCategory");
            var Inventlocation = HttpContext.Session.GetString("Inventlocation");
            ViewBag.Username = username;
            ViewBag.Role = Role;
            ViewBag.StoreIdRms = StoreIdRms;
            HttpContext.Session.SetString("ExportStatus", "started");
            // Prepare the SQL query with a parameter placeholder
            // Start building the SELECT clause dynamically
            List<string> selectColumns = new List<string>();
            if (Parobj.VDateInTime||Parobj.VDateandtime)
            {
                selectColumns.Add("Time");
                selectColumns.Add("DateandTime");
            }
            if (Parobj.SystemAnaysis)
            {
                if (Parobj.VPerMon || Parobj.VPerMonYear)
                    selectColumns.Add("fullMonth"); 
                if (Parobj.Area)
                    selectColumns.Add("Area");
                if (Parobj.VStoreName)
                    selectColumns.Add("Branches");
                selectColumns.Add("Userslogin");
                selectColumns.Add("Users");
                selectColumns.Add("Branch2023");
                selectColumns.Add("Branch2024");
                selectColumns.Add("(select sum(totalsales)from RptBranchesAnalysis where StoreFranchise !='TMT')TotalSub");
                selectColumns.Add("(select sum(totalsales)Total from RptBranchesAnalysis where StoreFranchise='TMT')TotalTmt");
                //if (Parobj.VPerYear || Parobj.VYearVinDash)
                    selectColumns.Add("ByYear");
            }
            if (Parobj.VPerDay)
                selectColumns.Add("CAST(transdate as date) as Date");
            if (Parobj.VPerYear || Parobj.VPerMonYear)
                selectColumns.Add("ByYear");
            if (Parobj.VPerMon || Parobj.VPerMonYear)
                selectColumns.Add("ByMonth");
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
            if (Parobj.VCompany)
                selectColumns.Add("Company");
            if (Parobj.VTransactionNumber)
                selectColumns.Add("TransactionNumber");
            if (Parobj.VQty)
                selectColumns.Add("sum(Qty)TotalQty");
            if (Parobj.VPrice)
                selectColumns.Add("Price");
            if (Parobj.VCost)
                selectColumns.Add("Cost");
            if (Parobj.VTotalSales)
                selectColumns.Add("sum(TotalSales)TotalSales");
            if (Parobj.SalesBranchPage)
            {
                if (Parobj.VTransactionCount)
                {
                    if (StoreIddynamic == "RMS")
                    {
                        selectColumns.Add("(select count(distinct TransactionNumber)T from RptSales where StoreName='" + username + "' and TransDate BETWEEN @fromDate AND @toDate)TransactionsCount ");
                    }
                    else
                    {
                        selectColumns.Add("(select count(distinct TransactionNumber)T from AXDBSales where StoreName='" + username + "' and TransDate BETWEEN @fromDate AND @toDate)TransactionsCount ");
                    }
                }
            }
            if(!Parobj.SalesBranchPage)
            {
                if (Parobj.VTransactionCount)
                    selectColumns.Add("count(distinct TransactionNumber) TransactionsCount");
            }
            if (Parobj.VTotalCost)
                selectColumns.Add("sum(Cost)TotalCost");
            if (Parobj.VTotalTax)
                selectColumns.Add("sum(Tax)TotalTax");
            if (Parobj.VTotalSalesTax)
                selectColumns.Add("sum(TotalSales)TotalSaleswithTax");
            if (Parobj.VTotalSalesWithoutTax)
                selectColumns.Add("sum(TotalSalesWithoutTax)TotalSalesWithoutTax");
            if (Parobj.VTotalCostQty)
                selectColumns.Add("sum(TotalCostQty)TotalCostQty");
            if (Parobj.OfferPage)
            {
                selectColumns.Add(" Discount,ItemName,StoreName,StoreId, ItemLookupCode ,OfferId,OfferName,TransactionNumber,-sum(Qty) Qty,-sum(TotalSales) TotalSales");
            }
            // Construct the SELECT clause from the list of columns
            string selectClause = string.Join(", ", selectColumns);
            string fromWhereClause = null;
            DateTime startDateTime = Convert.ToDateTime(Parobj.startDate, new CultureInfo("en-GB"));
            DateTime endDateTime = Convert.ToDateTime(Parobj.endDate, new CultureInfo("en-GB"));
            if (Parobj.SalesBranchPage || Parobj.CompareData || (Parobj.Home&&!Parobj.SystemAnaysis) || Parobj.Dashboard || Parobj.ItemMovements)
            {
                Parobj.Store = "0";
            }
            string[] storeVal= Parobj.Store.Split(':');
            if (Parobj.SystemAnaysis)
            {
                fromWhereClause = "FROM (" + Parobj.RptSystemAnalysis + ")RptSales2 where DpName is not null ";
            }
            else  if (Parobj.SystemAnaysis)
            {
                fromWhereClause = "FROM (" + Parobj.RptSystemAnalysis + ")RptSales2 where DpName is not null ";
                if (Parobj.YearinDash != "All" && Parobj.YearinDash != null)
                    fromWhereClause += "And byyear = '" + Parobj.YearinDash + "' ";
            }
            else if (Parobj.OfferPage)
            {
                fromWhereClause = "FROM (" + Parobj.RptOffer + ")RptSales2 where CAST(TransDate AS DATE) BETWEEN @fromDate AND @toDate ";
            }
            else if (Parobj.Shelf)
            {
                if (StoreIddynamic!="RMS")
                {
                    if (username == "HSC")
                    {
                        fromWhereClause = $"FROM ({Parobj.DyRptItemPrice})R where Dep='HSC_New '";
                    }
                    else if (PriceCategory == "B")
                    {
                        fromWhereClause = $"FROM ({Parobj.DyRptItemPrice})R where Dep='NorthP' ";
                    }
                    else
                    {
                        fromWhereClause = $"FROM ({Parobj.DyRptItemPrice})R where Dep='Retail' ";
                    }
                }
                else if (username == "RichCut" || username == "RichCut_No" || username == "RichCut_Zayed")
                {
                    fromWhereClause = $"FROM ({Parobj.RichCutItemPrice})R where StoreId=SUBSTRING('{StoreIdRms}', 2, 1) ";
                }
                else
                {
                    fromWhereClause = $"FROM ({Parobj.RmsRptItemPrice})R where StoreId={StoreIdRms} ";
                }

            }
            else if (Parobj.RichCut || (Parobj.SalesBranchPage && (username == "RichCut" || username == "RichCut_No" || username == "RichCut_Zayed")))
            {
                fromWhereClause = "FROM (" + Parobj.RichCutRptSales + ")RptSales2 where CAST(TransDate AS DATE) BETWEEN @fromDate AND @toDate ";

            }
            else if ((Parobj.RMS && Parobj.TMT == false && storeVal[0] == "RMS") || (Parobj.RMS && Parobj.TMT == false && storeVal[0] == "0") || StoreIddynamic == "RMS" || storeVal[0] == "RMS" || (Parobj.RMS && storeVal[0] == "RMS") || Parobj.RichCut)
            {
                fromWhereClause = "from ("+Parobj.RmsRptSalesView()+ ")RptSales WHERE CAST(TransDate AS DATE) BETWEEN @fromDate AND @toDate ";
                }
                else if (Parobj.RMS == false && Parobj.TMT || (storeVal.Length > 1 && storeVal[1] == "Dy") ||(Parobj.SalesBranchPage && StoreIddynamic != "RMS") || (Parobj.RMS == false && Parobj.TMT&&storeVal[0] != "RMS"))
                {
                if (Parobj.startDate== DateTime.Now.ToString("yyyy-MM-dd") || (Parobj.endDate == DateTime.Now.ToString("yyyy-MM-dd")))
                {
                    fromWhereClause = "FROM (" + Parobj.DyRptSaleslive() + ")RptAXDBSales where CAST(TransDate AS DATE) BETWEEN @fromDate AND @toDate ";
                    connectionString = Parobj.AxdbConnection;
                }
                else
                    fromWhereClause = "FROM (" + Parobj.DyRptAXDBSalesView() + ")RptAXDBSales where CAST(TransDate AS DATE) BETWEEN @fromDate AND @toDate ";
            }
            else if (Parobj.DBbefore)
                {
                    fromWhereClause = "FROM ("+Parobj.RmsbeforeRptSalesView()+")RptSales2 where CAST(TransDate AS DATE) BETWEEN @fromDate AND @toDate ";
                }

            else
                {
                        fromWhereClause = "from ("+ Parobj.RptSalesAll()+ ")RptSalesAll WHERE CAST(TransDate AS DATE) BETWEEN @fromDate AND @toDate ";
                }
            if (!string.IsNullOrEmpty(Parobj.Department) && Parobj.Department !="0")
            {
                // Split the string into an array of values
                string[] Departments = Parobj.Department.Split(',');

                // Start building the IN clause
                fromWhereClause += " AND DpName IN (";

                // Add a parameter placeholder for each value
                for (int i = 0; i < Departments.Length; i++)
                {
                    // Trim any whitespace from the value
                    string Department = Departments[i].Trim();

                    // Append the parameter placeholder to the IN clause
                    fromWhereClause += $"@Department{i}";

                    // Add a comma separator if not the last item
                    if (i < Departments.Length - 1)
                    {
                        fromWhereClause += ",";
                    }
                }

                fromWhereClause += ")";
            }

            //if (!string.IsNullOrEmpty(Parobj.Department) && Parobj.Department != "0")
            //{
            //    fromWhereClause += "AND DpName = @Department ";
            //}
            if (!string.IsNullOrEmpty(Parobj.Offer) && Parobj.Offer != "0")
            {
                fromWhereClause += "AND OfferName = @OfferName ";
            }
            if (!string.IsNullOrEmpty(Parobj.Supplier) && Parobj.Supplier != "0")
                {
                    fromWhereClause += "AND SupplierName = @Supplier ";
                }
            if (Role == "TerrManager" || Role == "FoodManager")
            {
                Parobj.isDmanager = true;
                Parobj.isFmanager = true;
            }
             Parobj.isUsername = db2.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));
            if (Parobj.Shelf)
            {
                Parobj.isUsername = db2.RptUsers.Any(s => s.Username == username  && (s.Storenumber != null || s.Storenumber != " "));
            }
            if (Parobj.FoodCategory)
            {
                fromWhereClause += " AND  (dpname in ('Breakfast','Burger','Cookies','Pizza', 'Cold Cut','Corn Dog','Crispy Chicken','Bakery','Donuts','Salad','French Fries') or ItemlookupCode='55888') ";
            }
            IQueryable<Storeuser> query;
                if (Parobj.isDmanager || Parobj.isUsername || Parobj.isFmanager)
                {
                if (Parobj.Shelf)
                {
                    fromWhereClause += $"AND ItemLookupCode !='null' ";
                }
                else
                {
                    fromWhereClause += "AND (Dmanager='" + username + "' or username ='" + username + "' or Fmanager ='" + username + "') ";
                }
            }

                if (!string.IsNullOrEmpty(Parobj.Store) && Parobj.Store != "0")
            {
                string[] storeValArray = Parobj.Store.Split(',');
                if (Parobj.SystemAnaysis)
                {
                    // Split the store values by comma

                    // Wrap each store value in single quotes and join them with a comma
                    List<string> Dylist = new List<string>();
                    foreach (var storeVala in storeValArray)
                    {
                        // Check if the store value contains "RMS:"
                        if (storeVala.Contains(":"))
                        {
                            // Split the store value by ":" to separate "RMS" from the number
                            string[] parts = storeVala.Split(':');
                            if (parts.Length > 1)
                            {
                                // Extract the number part and trim any whitespace
                                string numberPart = parts[0].Trim();
                                // Ensure the extracted part is a valid integer
                                int number;
                                if (int.TryParse(numberPart, out number))
                                {
                                    // Build the formatted number string
                                    Dylist.Add($"'{number}'");
                                }
                            }
                        }
                        else
                        {
                            // If the store value doesn't contain "RMS:", quote it directly
                            Dylist.Add($"'{storeVala}'");
                        }
                    }

                    // Join the quoted store values with a comma
                    string joinedQuotedString = string.Join(", ", Dylist);

                    // Modify the SQL query to include the quoted store values
                    fromWhereClause += $"AND StoreIdd IN ({joinedQuotedString})";


                    // Wrap each store value in single quotes and join them with a comma
                    List<string> quotedStoreValList = new List<string>();
                    foreach (var storeVala in storeValArray)
                    {
                        // Check if the store value contains "RMS:"
                        if (storeVala.Contains(":"))
                        {
                            // Split the store value by ":" to separate "RMS" from the number
                            string[] parts = storeVala.Split(':');
                            if (parts.Length > 1)
                            {
                                // Extract the number part and trim any whitespace
                                string numberPart = parts[1].Trim();
                                // Ensure the extracted part is a valid integer
                                int number;
                                if (int.TryParse(numberPart, out number))
                                {
                                    // Build the formatted number string
                                    quotedStoreValList.Add($"'{number}'");
                                }
                            }
                        }
                        else
                        {
                            // If the store value doesn't contain "RMS:", quote it directly
                            quotedStoreValList.Add($"'{storeVala}'");
                        }
                    }

                    // Join the quoted store values with a comma
                    string joinedQuotedStringRM = string.Join(", ", quotedStoreValList);

                    // Modify the SQL query to include the quoted store values
                    fromWhereClause += $"or StoreIdr IN ({joinedQuotedStringRM})";

                }
                else if (!Parobj.SystemAnaysis)
                {
                    if (username == "RichCut" || username == "RichCut_No" || username == "RichCut_Zayed")
                    {
                        fromWhereClause += "AND StoreId = SUBSTRING(@Store1, 2, 1) ";
                    }
                    else if ((Parobj.RMS && Parobj.TMT == false && storeVal[0] == "RMS") || (Parobj.RMS && Parobj.TMT == false && storeVal[0] == "0") || StoreIddynamic == "RMS" || storeVal[0] == "RMS" || (Parobj.RMS && storeVal[0] == "RMS") || Parobj.RichCut)
                    {
                        List<string> quotedStoreValList = new List<string>();
                        foreach (var storeVala in storeValArray)
                        {
                            // Check if the store value contains "RMS:"
                            if (storeVala.Contains(":"))
                            {
                                // Split the store value by ":" to separate "RMS" from the number
                                    string[] parts = storeVala.Split(':');
                                if (parts.Length > 1)
                                {
                                    // Extract the number part and trim any whitespace
                                    string numberPart = parts[1].Trim();
                                    // Ensure the extracted part is a valid integer
                                    int number;
                                    if (Parobj.RichCut)
                                    {
                                        //string[] RichValue = numberPart.Split('R');
                                        //numberPart = RichValue[1].Trim();
                                        quotedStoreValList.Add($"'{numberPart}'");
                                    }
                                    else if (int.TryParse(numberPart, out number))
                                    {
                                        // Build the formatted number string
                                        quotedStoreValList.Add($"'{number}'");
                                    }
                                }
                            }
                            else
                            {
                                // If the store value doesn't contain "RMS:", quote it directly
                                quotedStoreValList.Add($"'{storeVala}'");
                            }
                        }

                        // Join the quoted store values with a comma
                        string joinedQuotedStringRM = string.Join(", ", quotedStoreValList);

                        // Modify the SQL query to include the quoted store values
                        fromWhereClause += $"AND StoreId IN ({joinedQuotedStringRM})";
                    }
                    else if (Parobj.RMS == false && Parobj.TMT || (storeVal.Length > 1 && storeVal[1] == "Dy") || (Parobj.SalesBranchPage && StoreIddynamic != "RMS") || (Parobj.RMS == false && Parobj.TMT && storeVal[0] != "RMS") || Parobj.OfferPage)
                    {
                        List<string> Dylist = new List<string>();
                        foreach (var storeVala in storeValArray)
                        {
                            // Check if the store value contains "RMS:"
                            if (storeVala.Contains(":"))
                            {
                                // Split the store value by ":" to separate "RMS" from the number
                                string[] parts = storeVala.Split(':');
                                if (parts.Length > 1)
                                {
                                    // Extract the number part and trim any whitespace
                                    string numberPart = parts[0].Trim();
                                    // Ensure the extracted part is a valid integer
                                    int number;
                                    if (int.TryParse(numberPart, out number))
                                    {
                                        // Build the formatted number string
                                        Dylist.Add($"'{number}'");
                                    }
                                }
                            }
                            else
                            {
                                // If the store value doesn't contain "RMS:", quote it directly
                                Dylist.Add($"'{storeVala}'");
                            }
                        }

                        // Join the quoted store values with a comma
                        string joinedQuotedString = string.Join(", ", Dylist);

                        // Modify the SQL query to include the quoted store values
                        fromWhereClause += $"AND StoreId IN ({joinedQuotedString})";
                    }
                    else if (!Parobj.SystemAnaysis || Parobj.RMS && Parobj.TMT || storeVal.Length > 1)
                    {
                        List<string> Dylist = new List<string>();
                        foreach (var storeVala in storeValArray)
                        {
                            // Check if the store value contains "RMS:"
                            if (storeVala.Contains(":"))
                            {
                                // Split the store value by ":" to separate "RMS" from the number
                                string[] parts = storeVala.Split(':');
                                if (parts.Length > 1)
                                {
                                    // Extract the number part and trim any whitespace
                                    string numberPart = parts[0].Trim();
                                    // Ensure the extracted part is a valid integer
                                    int number;
                                    if (int.TryParse(numberPart, out number))
                                    {
                                        // Build the formatted number string
                                        Dylist.Add($"'{number}'");
                                    }
                                }
                            }
                            else
                            {
                                // If the store value doesn't contain "RMS:", quote it directly
                                Dylist.Add($"'{storeVala}'");
                            }
                        }

                        // Join the quoted store values with a comma
                        string joinedQuotedString = string.Join(", ", Dylist);

                        // Modify the SQL query to include the quoted store values
                        fromWhereClause += $"AND (StoreIdd IN ({joinedQuotedString})";


                        // Wrap each store value in single quotes and join them with a comma
                        List<string> quotedStoreValList = new List<string>();
                        foreach (var storeVala in storeValArray)
                        {
                            // Check if the store value contains "RMS:"
                            if (storeVala.Contains(":"))
                            {
                                // Split the store value by ":" to separate "RMS" from the number
                                string[] parts = storeVala.Split(':');
                                if (parts.Length > 1)
                                {
                                    // Extract the number part and trim any whitespace
                                    string numberPart = parts[1].Trim();
                                    // Ensure the extracted part is a valid integer
                                    int number;
                                    if (int.TryParse(numberPart, out number))
                                    {
                                        // Build the formatted number string
                                        quotedStoreValList.Add($"'{number}'");
                                    }
                                }
                            }
                            else
                            {
                                // If the store value doesn't contain "RMS:", quote it directly
                                quotedStoreValList.Add($"'{storeVala}'");
                            }
                        }

                        // Join the quoted store values with a comma
                        string joinedQuotedStringRM = string.Join(", ", quotedStoreValList);

                        // Modify the SQL query to include the quoted store values
                        fromWhereClause += $"or StoreIdr IN ({joinedQuotedStringRM}))";
                    }
                    else
                    {
                        fromWhereClause += "AND StoreId = @Store1 ";
                    }
                }
                else
                { if (storeVal[0] == "RMS")
                        fromWhereClause += "AND (StoreIdR = @Store1) ";
                    else if (storeVal[1] == "Dy")
                        fromWhereClause += "AND (StoreIdD = @Store ) ";
                    else 
                    fromWhereClause += "AND (StoreIdD = @Store OR StoreIdR = @Store1) ";
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
            string sqlQuery = $"SELECT {selectClause} {fromWhereClause}";
                // Start building the GROUP BY clause dynamically
                List<string> groupByColumns = new List<string>();
            if (Parobj.VDateInTime ||Parobj.VDateandtime)
            {
                groupByColumns.Add("Time");
                groupByColumns.Add("DateandTime");
            }
            if (Parobj.SystemAnaysis)
            {
                if (Parobj.VPerMon || Parobj.VPerMonYear)
                    groupByColumns.Add("fullMonth");
                if (Parobj.Area)
                    groupByColumns.Add("Area");
                if (Parobj.VStoreName)
                groupByColumns.Add("Branches");
                groupByColumns.Add("Userslogin"); 
                groupByColumns.Add("Users");
                groupByColumns.Add("Branch2023");
                groupByColumns.Add("Branch2024");
                //if (Parobj.VPerYear ||Parobj.VYearVinDash)
                groupByColumns.Add("ByYear");
            }
            if (Parobj.VPerYear || Parobj.VPerMonYear)
                    groupByColumns.Add("ByYear");
                if (Parobj.VPerMon || Parobj.VPerMonYear)
                    groupByColumns.Add("ByMonth");
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
                if (Parobj.VCompany)
                groupByColumns.Add("Company");
            if (Parobj.VTransactionNumber)
                    groupByColumns.Add("TransactionNumber");
                if (Parobj.VPrice)
                    groupByColumns.Add("Price");
                if (Parobj.VCost)
                    groupByColumns.Add("Cost");
                if (Parobj.VPerDay)
                    groupByColumns.Add("CAST(transdate as date)");
                if (Parobj.OfferPage)
            {
                groupByColumns.Add(" Discount,ItemName,StoreName,StoreId, ItemLookupCode ,OfferId,OfferName,TransactionNumber ");
            }
                // Do not include sum(totalsales) in the GROUP BY clause

                // Construct the GROUP BY clause from the list of columns
                string groupByClause = groupByColumns.Count > 0 ? "GROUP BY " + string.Join(", ", groupByColumns) : "";
                sqlQuery += groupByClause;
            if (Parobj.ItemMovements)
            {
                sqlQuery = Parobj.RptItemMovements; 
                if (Parobj.ItemLookupCodeTxt !=null)
                    sqlQuery +=" And ItemlookupCode = '" +Parobj.ItemLookupCodeTxt+"' ";
            }
            if (Parobj.Dashboard)
            {
                sqlQuery = Parobj.RptSyslog;
            }
            if (Parobj.CompareData)
            {
                sqlQuery = Parobj.CompareData_Ax_Top;
            }
            if (Parobj.Home)
            {
                if (Parobj.isUsername)
                {
                    sqlQuery = Parobj.Analysis + " Where StoreName='" + username+"'";
                }
            }
            if (Parobj.SystemAnaysis)
            {
                if (!Parobj.VDepartment && !Parobj.VYearVinDash && !Parobj.VPerYear && !Parobj.VPerMon && !Parobj.Area)
                {
                    sqlQuery += " order by StoreName,sum(TotalSales)desc";
                }
                else
                {
                    sqlQuery += " order by sum(TotalSales)desc";
                }
            }
           List<SalesParameters> salesDataList = new List<SalesParameters>();
         try
         {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
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
                        if (Parobj.Store != "0")
                        {
                            string[] storeValArray = Parobj.Store.Split(',');
                            List<string> storeValList = new List<string>(storeValArray);
                        //    if (Parobj.SystemAnaysis)
                        //    {
                        //        List<string> selectedStoresRMSList = new List<string>();
                        //        foreach (var selectedStore in storeValArray)
                        //        {
                        //            selectedStoresRMSList.Add(selectedStore.Split(':')[1]);
                        //        }
                        //        string joinedString = string.Join(",", selectedStoresRMSList);
                        //        command.Parameters.AddWithValue("@Store1", joinedString);
                        //}
                         if (!Parobj.SystemAnaysis)
                        {
                            if (Parobj.RichCut)
                            {
                                if (storeVal[1] == "R1" || storeVal[1] == "R2" || storeVal[1] == "R3")
                                {
                                    command.Parameters.AddWithValue("@Store1", storeVal[1]);
                                }
                            }
                        //    else if ((Parobj.RMS && Parobj.TMT == false && storeVal[0] == "RMS") || (Parobj.RMS && Parobj.TMT == false && storeVal[0] == "0") || StoreIddynamic == "RMS" || storeVal[0] == "RMS" || (Parobj.RMS && storeVal[0] == "RMS") || Parobj.RichCut)
                        //    {
                        //        List<string> selectedStoresRMSList = new List<string>();
                        //        foreach (var selectedStore in storeValArray)
                        //        {
                        //            selectedStoresRMSList.Add(selectedStore.Split(':')[1]);
                        //        }
                        //        //string jsonStores = System.Text.Json.JsonSerializer.Serialize(selectedStoresRMSList);
                        //        string joinedString = string.Join(", ", selectedStoresRMSList);
                        //        // Now, add the JSON string as a parameter to your command
                        //        command.Parameters.AddWithValue("@Store1", joinedString);
                        //    }
                        //    else if (Parobj.RMS == false && Parobj.TMT || (storeVal.Length > 1 && storeVal[1] == "Dy") || (Parobj.SalesBranchPage && StoreIddynamic != "RMS") || (Parobj.RMS == false && Parobj.TMT && storeVal[0] != "RMS") || Parobj.OfferPage)
                        //    {

                        //        List<string> selectedStoresTMTList = new List<string>();
                        //        foreach (var selectedStore in storeValArray)
                        //        {
                        //            // if (!string.IsNullOrEmpty(selectedStore[0].ToString().Split(':')[0]))
                        //            //{
                        //            // selectedStoresTMTList.Add(selectedStore[0].ToString().Split(':')[0]);
                        //            selectedStoresTMTList.Add(selectedStore.Split(':')[0]);
                        //            // }
                        //        }
                        //        string joinedString = string.Join(", ", selectedStoresTMTList);
                        //        command.Parameters.AddWithValue("@Store", joinedString);
                        //    }
                        //    else
                        //    {
                        //        List<string> selectedStoresTMTList = new List<string>();
                        //        foreach (var selectedStore in storeValArray)
                        //        {
                        //            // if (!string.IsNullOrEmpty(selectedStore[0].ToString().Split(':')[0]))
                        //            //{
                        //            //selectedStoresTMTList.Add(selectedStore[0].ToString().Split(':')[0]);
                        //            if (!string.IsNullOrEmpty(selectedStore[0].ToString().Split(':')[0]))
                        //            {
                        //                selectedStoresTMTList.Add(selectedStore.Split(':')[0]);
                        //            }
                        //            // }
                        //        }

                        //        List<string> selectedStoresRMSList = new List<string>();
                        //        foreach (var selectedStore in storeValArray)
                        //        {
                        //            //if (!string.IsNullOrEmpty(selectedStore[1].ToString().Split(':')[1]))
                        //            // {
                        //            selectedStoresRMSList.Add(selectedStore.Split(':')[1]);
                        //            // }

                        //        }
                        //        string joinedStringRMS = string.Join(", ", selectedStoresRMSList);
                        //        string joinedStringTMT = string.Join(", ", selectedStoresTMTList);
                        //        if (joinedStringTMT != "RMS")
                        //            command.Parameters.AddWithValue("@Store", joinedStringTMT);
                        //        if (joinedStringRMS != "Dy")
                        //            command.Parameters.AddWithValue("@Store1", joinedStringRMS);
                        //}

                        }
                         else if (Parobj.RMS && Parobj.TMT || storeVal.Length > 1)// && storeVal[0] != "RMS" && storeVal[1] != "Dy" || (Parobj.RMS && Parobj.TMT && storeVal[0] == "0" && storeVal[1] == "0"))
                            {
                            List<string> selectedStoresTMTList = new List<string>();
                            foreach (var selectedStore in storeValArray)
                            {
                                // if (!string.IsNullOrEmpty(selectedStore[0].ToString().Split(':')[0]))
                                //{
                                //selectedStoresTMTList.Add(selectedStore[0].ToString().Split(':')[0]);
                                if (!string.IsNullOrEmpty(selectedStore[0].ToString().Split(':')[0]))
                                {
                                    selectedStoresTMTList.Add(selectedStore.Split(':')[0]);
                                }
                                // }
                            }

                            List<string> selectedStoresRMSList = new List<string>();
                            foreach (var selectedStore in storeValArray)
                            {
                                //if (!string.IsNullOrEmpty(selectedStore[1].ToString().Split(':')[1]))
                                // {
                                selectedStoresRMSList.Add(selectedStore.Split(':')[1]);
                                // }

                            }
                            string joinedStringRMS = string.Join(", ", selectedStoresRMSList);
                            string joinedStringTMT = string.Join(", ", selectedStoresTMTList);
                            if (joinedStringTMT!="RMS")
                             command.Parameters.AddWithValue("@Store", joinedStringTMT);
                            if (joinedStringRMS!="Dy")
                             command.Parameters.AddWithValue("@Store1", joinedStringRMS);
                            }
                            else
                            {
                                return View();
                            }
                        }
                    if (!string.IsNullOrEmpty(Parobj.Department))
                    {
                        string[] Departments = Parobj.Department.Split(',');
                        for (int i = 0; i < Departments.Length; i++)
                        {
                            string Department = Departments[i].Trim();
                            command.Parameters.AddWithValue($"@Department{i}", Department);
                        }
                    }
                    //if (!string.IsNullOrEmpty(Parobj.Department) && Parobj.Department != "0")
                    //    {
                    //        command.Parameters.AddWithValue("@Department", Parobj.Department);
                    //    }
                        if (!string.IsNullOrEmpty(Parobj.Offer) && Parobj.Offer != "0")
                        {
                            command.Parameters.AddWithValue("@OfferName", Parobj.Offer);
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
                        if (Parobj.ItemMovements && Parobj.actionValue == 1)
                        {
                            double TotalSales = 0;
                            var vi = new List<RptSale>();
                            var test = command.ExecuteReader();
                            int id = 0;
                            double TotalQty = 0;
                        double tq = 0;
                        while (test.Read())
                            {
                                RptSale si = new RptSale();
                                si.StoreName = test["Account"].ToString();
                                ViewBag.Account = si.StoreName;
                                si.ConvDate = test["Receivedate"].ToString();
                                DateTime parsedDate;
                                if (DateTime.TryParse(si.ConvDate, out parsedDate))
                                {
                                    si.ConvDate = parsedDate.ToString("yyyy-MM-dd");
                                }
                                si.Dmanager = test["Transactions"].ToString();
                                ViewBag.Userslogin = si.Dmanager;
                                si.StoreFranchise = test["Franchise"].ToString();
                                ViewBag.Branches = si.StoreName;
                                si.Fmanager = test["Storefrom"].ToString();
                                ViewBag.Users = si.Username;
                                si.SupplierName = test["Store_Name"].ToString();
                                ViewBag.Userslogin = si.Dmanager;
                                si.DpName = test["Category"].ToString();
                                ViewBag.Branches = si.StoreName;
                                si.ItemLookupCode = test["ItemlookupCode"].ToString();
                                ViewBag.Users = si.Username;
                                si.ItemName = test["ItemName"].ToString();
                                ViewBag.Userslogin = si.Dmanager;
                                TotalQty = Convert.ToDouble(test["Quantity"]);
                                si.FQty = TotalQty.ToString("#,##0.00");
                            tq += Convert.ToDouble(TotalQty);
                            vi.Add(si);
                            }
                            ViewBag.Data = vi;
                        ViewBag.TotalQty = tq.ToString("#,##0.00");
                        return View("ItemMovements");
                        }
                        if (!Parobj.SystemAnaysis && Parobj.Home)
                        {
                            double TotalSales = 0;
                            var vi = new List<RptSale>();
                            var test = command.ExecuteReader();
                            int id = 0;
                            while (test.Read())
                            {
                                RptSale si = new RptSale();
                                si.StoreName = test["Branches"].ToString();
                                ViewBag.Branches = si.StoreName; 
                                si.Username = test["Users"].ToString();
                                ViewBag.Users = si.Username;
                                si.Dmanager = test["Userslogin"].ToString();
                                ViewBag.Userslogin = si.Dmanager;
                                //si.Fmanager = test["DpName"].ToString();
                                //ViewBag.StoreName = si.Fmanager;
                                //}
                                if (Parobj.Storef)
                                {
                                    si.SupplierName = test["StoreName"].ToString();
                                }
                                if (Parobj.Cat)
                                {
                                    si.SupplierName = test["DpName"].ToString();
                                }
                                ViewBag.StoreName = si.SupplierName;
                                ViewBag.id = id;
                                si.TotalSales = Convert.ToDouble(test["TotalSales"]);
                                TotalSales = Convert.ToDouble(si.TotalSales);
                                si.DpName = TotalSales.ToString("#,##0.00");
                                ViewBag.TotalSales = si.DpName;
                                vi.Add(si);
                            }
                            ViewBag.Data = vi;
                            if (Parobj.SystemAnaysis)
                            {
                                return View("Home1");
                            }
                            else
                            {
                                return View("Home");
                            }
                        }
                        if (Parobj.Dashboard)
                        {
                            double TotalSales = 0;
                            var vi = new List<RptSale>();
                            var test = command.ExecuteReader();
                            int id = 0;
                            while (test.Read())
                            {
                                RptSale si = new RptSale();
                                si.StoreName = test["username"].ToString();
                                ViewBag.Branches = si.StoreName;
                                si.Username = test["LoginDate"].ToString();
                                ViewBag.Users = si.Username;
                                si.Dmanager = test["Role"].ToString();
                                ViewBag.Userslogin = si.Dmanager;
                                vi.Add(si);
                            }
                            ViewBag.Data = vi;
                            return View("Dashboard");

                        }
                        if (Parobj.CompareData && Parobj.actionValue == 1)
                        {
                            var vi = new List<SalesParameters>();
                            var test = command.ExecuteReader();
                            while (test.Read())
                            {
                                SalesParameters si = new SalesParameters();
                                si.DB = test["Store Name"].ToString();
                                si.TotalSales = Convert.ToDouble(test["TotalSales"]);
                                si.TotalSales.ToString("#,##0.00");
                                vi.Add(si);
                            }
                            ViewBag.Data = vi;
                            return View("CompareData");

                        }
                        else if (Parobj.OfferPage && Parobj.actionValue ==1)
                        {
                            var vi = new List<RptSale>();
                            var ll = 0;
                            var test = command.ExecuteReader();
                            double TotalSales = 0;
                            double TotalQty = 0;
                            double Discount = 0;
                            double Transactions = 0;
                            double tsa = 0;
                            double tq = 0;
                            while (test.Read())
                            {
                                RptSale si = new RptSale();
                                    ViewBag.VStoreId = "V";
                                    si.StoreId = test["StoreID"].ToString();

                                si.StoreName = test["StoreName"].ToString();

                                si.Dmanager = test["OfferId"].ToString();
                                si.Fmanager = test["OfferName"].ToString();
                                si.ItemLookupCode = test["ItemLookupCode"].ToString();
                                 si.ItemName = test["ItemName"].ToString();
                                  si.TransactionNumber = test["TransactionNumber"].ToString();
      
                                    ViewBag.VQty = "V";
                                TotalQty = Convert.ToDouble(test["Qty"]);
                                si.FQty = TotalQty.ToString("#,##0.00"); 
                                Discount = Convert.ToDouble(test["Discount"]);
                                si.DpName = Discount.ToString("#,##0.00");

                                TotalSales = Convert.ToDouble(test["TotalSales"]);
                                        si.FTotalSales = TotalSales.ToString("#,##0.00");
                                        // Handle the case where test["TotalSales"] is DBNull
                                        // For example, set FTotalSales to a default value or handle it as needed
                                ViewBag.Q = si.Qty;
                                //TotalSales = Convert.ToDouble(si.TotalSales);
                                //si.ConvTotalSales=TotalSales.ToString("#,##0.00");
                                //TotalQty = Convert.ToDouble(FQty);
                                si.ConvTotalQty = TotalQty.ToString("#,##0.00");
                                tsa += Convert.ToDouble(TotalSales); ;
                                tq += Convert.ToDouble(TotalQty);
                                vi.Add(si);
                            }
                            ViewBag.Data = vi;
                            ViewBag.Total = tsa.ToString("#,##0.00");
                            ViewBag.TotalQty = tq.ToString("#,##0.00");
                            ViewBag.St = "com";
                                return View("Offer");
                        }
                        if (Parobj.SystemAnaysis|| (Parobj.SalesBranchPage || Parobj.MainSales) && (Parobj.actionValue == 1 || Parobj.actionValue == 0))
                        {
                            var vi = new List<RptSale>();
                            // Store the sum in ViewBag
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
                            if (Parobj.SystemAnaysis)
                                {
                                    if (Parobj.VStoreName)
                                    si.ItemName = test["Branches"].ToString();
                                    ViewBag.ItemName = si.ItemName;
                                    si.Username = test["Users"].ToString();
                                    ViewBag.Users = si.Username;
                                    si.Dmanager = test["Userslogin"].ToString();
                                    ViewBag.Userslogin = si.Dmanager;
                                    ViewBag.Branch2024 = test["Branch2024"].ToString();
                                    ViewBag.Branch2023 = test["Branch2023"].ToString();
                                    ViewBag.TotalTmt = test["TotalTmt"].ToString();
                                    ViewBag.TotalSub = test["TotalSub"].ToString();
                                    if (Parobj.Area)
                                    {
                                        ViewBag.VArea = "v";
                                        si.SupplierName = test["Area"].ToString();
                                        si.Fmanager = test["Area"].ToString();
                                    }
                                    else if (Parobj.VPerMon)
                                    {
                                        ViewBag.Mon = "v";
                                        si.SupplierName = test["fullMonth"].ToString();
                                        si.Dmanager = test["fullMonth"].ToString();
                                    }
                                    else if (Parobj.VPerYear || Parobj.VYearVinDash)
                                    {
                                        si.SupplierName = test["ByYear"].ToString();
                                    }
                                    else if (Parobj.VDepartment)
                                    {
                                        si.SupplierName = test["Department"].ToString();
                                    }
                                    else if (Parobj.Store != null || Parobj.VStoreName)
                                    {
                                        si.SupplierName = test["Store Name"].ToString();
                                    }
                                    else if (Parobj.Franchise != null && Parobj.VStoreName)
                                    {
                                        si.SupplierName = test["Store Name"].ToString();
                                    }

                                    //else if (Parobj.VFranchise)
                                    //    si.SupplierName = test["StoreFranchise"].ToString();
                                    si.TotalSales = Convert.ToDouble(test["TotalSales"]);
                            }
                            if (Parobj.VDateInTime)
                            {
                                ViewBag.VDateInTime = "V";
                                si.Dmanager = test["Time"].ToString();
                            }
                                if (Parobj.VDateandtime)
                                {
                                    ViewBag.VDateandtime = "V";
                                    si.Fmanager = test["DateandTime"].ToString();
                                }
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
                                if (Parobj.VCompany)
                                {
                                    ViewBag.VCompany = "V";
                                    si.Company = test["Company"].ToString();
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
                                    if (!Convert.IsDBNull(test["Cost"]))
                                    {
                                        si.Cost = Convert.ToDecimal(test["Cost"]);
                                    }
                                    else
                                    {
                                        si.Cost = 0;
                                    }
                                }
                                if (Parobj.VTotalSales)
                                {
                                    ViewBag.VTotalSales = "V";
                                    if (!Convert.IsDBNull(test["TotalSales"]))
                                    {
                                        TotalSales = Convert.ToDouble(test["TotalSales"]);
                                        si.FTotalSales = TotalSales.ToString("#,##0.00");
                                    }
                                    else
                                    {
                                        // Handle the case where test["TotalSales"] is DBNull
                                        // For example, set FTotalSales to a default value or handle it as needed
                                        si.FTotalSales = "0.00"; // Assuming you want to use 0.00 as a default value
                                    }
                                }
                                if (Parobj.VTransactionCount)
                                {
                                    ViewBag.VTransactionCount = "V";
                                    si.TransactionsCount = Convert.ToDouble(test["TransactionsCount"]);
                                }
                                if (Parobj.VTotalCost)
                                {
                                    ViewBag.VTotalCost = "V";
                                    if (!Convert.IsDBNull(test["TotalCost"]))
                                    {
                                        si.TotalCost = Convert.ToDouble(test["TotalCost"]);
                                    }
                                    else
                                    {
                                        si.TotalCost = 0;
                                    }
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
                            var filteredSalesTmt = vi.Where(sale => sale.StoreFranchise == "TMT");
                            var filteredSalesSub = vi.Where(sale => sale.StoreFranchise == "Sub-Franchise" || sale.StoreFranchise=="SUB-FRANCHISE");

                            // Calculate the sum of totalsales for the filtered list
                            var totalSalesTmt = filteredSalesTmt.Sum(sale => sale.TotalSales);
                            var totalSalesSub = filteredSalesSub.Sum(sale => sale.TotalSales);
                            ViewBag.SelectSumTMT = totalSalesTmt;
                            ViewBag.SelectSumSub = totalSalesSub;
                            if (Parobj.SystemAnaysis)
                        {
                            return View("Home1");
                        }
                        else if (Parobj.MainSales)
                            {
                                ViewBag.Good = "T";
                                return View("Index");
                            }
                            else
                            {
                                return View("Store");
                            }
                        }
                        try
                        {
                            if (Parobj.Shelf)
                            {
                                var vi = new List<RptSale>();
                                var ll = 0;
                                var test = command.ExecuteReader();
                                while (test.Read())
                                {
                                    RptSale si = new RptSale();
                                    //si.StoreName = test["Store Name"].ToString();
                                    si.ItemLookupCode = test["Itemlookupcode"].ToString();
                                    si.ItemName = test["ItemName"].ToString();
                                    decimal price = Math.Round(Convert.ToDecimal(test["Price"]), 2);
                                    si.Price = price;
                                    // si.Cost = 100;
                                    // ViewBag.lname = ll;
                                    vi.Add(si);
                                }
                                ViewBag.Data = vi;
                                return View("Shelf");
                            }
                        }
                        catch (Exception ex)
                        {
                            return View();
                        }
                        // Create a new Excel package
                        try
                        {
                            using (var package = new ExcelPackage())
                            {
                                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("AKSoftSalesReport");
                                int row = 2; // Start from row 2 to leave space for headers
                                int sheetIndex = 1; // Start with the first sheet
                                int columnCount = 1;
                                // Add header row
                                void AddHeaderRow(ExcelWorksheet ws, int columnCount)
                                {
                                    int column = 1;
                                if (Parobj.OfferPage)
                                    {
                                        ws.Cells[1, column++].Value = "Offer Id";
                                        ws.Cells[1, column++].Value = "Offer Name";
                                        ws.Cells[1, column++].Value = "Store Id";
                                        ws.Cells[1, column++].Value = "Store Name";
                                        ws.Cells[1, column++].Value = "TransactionNumber";
                                        ws.Cells[1, column++].Value = "Barcode";
                                        ws.Cells[1, column++].Value = "Item Name";
                                        ws.Cells[1, column++].Value = "Discount";
                                        ws.Cells[1, column++].Value = "Quantity";
                                        ws.Cells[1, column++].Value = "Total Sales";
                                    }
                                if (Parobj.ItemMovements)
                                {
                                    ws.Cells[1, column++].Value = "Reference";
                                    ws.Cells[1, column++].Value = "Date";
                                    ws.Cells[1, column++].Value = "Id";
                                    ws.Cells[1, column++].Value = "Status";
                                    ws.Cells[1, column++].Value = "Store";
                                    ws.Cells[1, column++].Value = "Transfer/Purch/Sale To";
                                    ws.Cells[1, column++].Value = "Category";
                                    ws.Cells[1, column++].Value = "Barcode";
                                    ws.Cells[1, column++].Value = "Description";
                                    ws.Cells[1, column++].Value = "Quantity";
                                }
                                    if (Parobj.VDateInTime)
                                        ws.Cells[1, column++].Value = "Time";
                                    if (Parobj.VPerYear || Parobj.VPerMonYear)
                                    ws.Cells[1, column++].Value = "ByYear";
                                if (Parobj.VPerMon || Parobj.VPerMonYear)
                                        ws.Cells[1, column++].Value = "ByMonth";
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
                                    if (Parobj.VCompany)
                                        ws.Cells[1, column++].Value = "Company";
                                    if (Parobj.VTransactionNumber)
                                        ws.Cells[1, column++].Value = "TransactionNumber";
                                    if (Parobj.VQty)
                                        ws.Cells[1, column++].Value = "TotalQty";
                                    if (Parobj.VPrice)
                                        ws.Cells[1, column++].Value = "Price";
                                    if (Parobj.VCost)
                                        ws.Cells[1, column++].Value = "Cost";
                                    if (Parobj.VTotalSales)
                                        ws.Cells[1, column++].Value = "TotalSales";
                                    if (Parobj.VTransactionCount && !Parobj.SalesBranchPage)
                                        ws.Cells[1, column++].Value = "TransactionsCount";
                                    if (Parobj.VTotalCost)
                                        ws.Cells[1, column++].Value = "TotalCost";
                                    if (Parobj.VTotalTax)
                                        ws.Cells[1, column++].Value = "TotalTax";
                                    if (Parobj.VTotalSalesTax)
                                        ws.Cells[1, column++].Value = "TotalSaleswithTax";
                                    if (Parobj.VTotalSalesWithoutTax)
                                        ws.Cells[1, column++].Value = "TotalSalesWithoutTax";
                                    if (Parobj.VTotalCostQty)
                                        ws.Cells[1, column++].Value = "TotalCostQty";
                                    if (Parobj.VPerDay)
                                        ws.Cells[1, column++].Value = "Date";
                                    if (Parobj.VDateandtime)
                                        ws.Cells[1, column++].Value = "Date&Time";
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
                                     if (Parobj.OfferPage)
                                        {
                                            worksheet.Cells[row, columnCount++].Value = reader["OfferId"];
                                            worksheet.Cells[row, columnCount++].Value = reader["OfferName"];
                                            worksheet.Cells[row, columnCount++].Value = reader["StoreId"];
                                            worksheet.Cells[row, columnCount++].Value = reader["StoreName"];
                                            worksheet.Cells[row, columnCount++].Value = reader["TransactionNumber"];
                                            worksheet.Cells[row, columnCount++].Value = reader["ItemLookupCode"];
                                            worksheet.Cells[row, columnCount++].Value = reader["ItemName"];
                                            worksheet.Cells[row, columnCount++].Value = reader["Discount"];
                                            worksheet.Cells[row, columnCount++].Value = reader["Qty"];
                                            worksheet.Cells[row, columnCount++].Value = reader["TotalSales"];
                                        }
                                    if (Parobj.ItemMovements)
                                    {
                                        worksheet.Cells[row, columnCount++].Value = reader["Account"];
                                            worksheet.Cells[row, columnCount].Style.Numberformat.Format = "yyyy-MM-dd";
                                            worksheet.Cells[row, columnCount++].Value = reader["Receivedate"];
                                        worksheet.Cells[row, columnCount].Style.Numberformat.Format = "yyyy-MM-dd";
                                        worksheet.Cells[row, columnCount++].Value = reader["Transactions"];
                                        worksheet.Cells[row, columnCount++].Value = reader["Franchise"];
                                        worksheet.Cells[row, columnCount++].Value = reader["Storefrom"];
                                        worksheet.Cells[row, columnCount++].Value = reader["Store_Name"];
                                        worksheet.Cells[row, columnCount++].Value = reader["Category"];
                                        worksheet.Cells[row, columnCount++].Value = reader["ItemlookupCode"];
                                        worksheet.Cells[row, columnCount++].Value = reader["ItemName"];
                                        worksheet.Cells[row, columnCount++].Value = reader["Quantity"];
                                    }
                                    if (Parobj.VDateInTime)
                                        worksheet.Cells[row, columnCount++].Value = reader["Time"];
                                        if (Parobj.VPerYear || Parobj.VPerMonYear)
                                            worksheet.Cells[row, columnCount++].Value = reader["ByYear"];
                                        if (Parobj.VPerMon || Parobj.VPerMonYear)
                                            worksheet.Cells[row, columnCount++].Value = reader["ByMonth"];
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
                                        if (Parobj.VCompany)
                                            worksheet.Cells[row, columnCount++].Value = reader["Company"];
                                        if (Parobj.VTransactionNumber)
                                            worksheet.Cells[row, columnCount++].Value = reader["TransactionNumber"];
                                        if (Parobj.VQty)
                                            worksheet.Cells[row, columnCount++].Value = reader["TotalQty"];
                                        if (Parobj.VPrice)
                                            worksheet.Cells[row, columnCount++].Value = reader["Price"];
                                        if (Parobj.VCost)
                                            worksheet.Cells[row, columnCount++].Value = reader["Cost"];
                                        worksheet.Cells[row, columnCount].Style.Numberformat.Format = "#,##0.00";
                                        if (Parobj.VTotalSales)
                                            worksheet.Cells[row, columnCount++].Value = reader["TotalSales"];
                                        if (Parobj.VTransactionCount && !Parobj.SalesBranchPage)
                                            worksheet.Cells[row, columnCount++].Value = reader["TransactionsCount"];
                                        if (Parobj.VTotalCost)
                                            worksheet.Cells[row, columnCount++].Value = reader["TotalCost"];
                                        if (Parobj.VTotalTax)
                                            worksheet.Cells[row, columnCount++].Value = reader["TotalTax"];
                                        if (Parobj.VTotalSalesTax)
                                            worksheet.Cells[row, columnCount++].Value = reader["TotalSaleswithTax"];
                                        if (Parobj.VTotalSalesWithoutTax)
                                            worksheet.Cells[row, columnCount++].Value = reader["TotalSalesWithoutTax"];
                                        if (Parobj.VTotalCostQty)
                                            worksheet.Cells[row, columnCount++].Value = reader["TotalCostQty"];
                                        worksheet.Cells[row, columnCount].Style.Numberformat.Format = "yyyy-MM-dd";
                                        if (Parobj.VPerDay)
                                            worksheet.Cells[row, columnCount++].Value = reader["Date"];
                                        if (Parobj.VDateandtime)
                                            worksheet.Cells[row, columnCount++].Value = reader["DateandTime"];
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
                                            worksheet = package.Workbook.Worksheets.Add($"AKSoftSalesReport{sheetIndex++}");
                                            // Re-add the header row to the new worksheet
                                            row = 2; // Reset row count for the new worksheet
                                            columnCount = 1; // Reset column count
                                                             // Re-add the header row to the new worksheet\
                                            AddHeaderRow(worksheet, columnCount);
                                        if (Parobj.VDateInTime)
                                            worksheet.Cells[row, columnCount++].Value = reader["Time"];
                                        if (Parobj.VPerYear || Parobj.VPerMonYear)
                                            worksheet.Cells[row, columnCount++].Value = reader["ByYear"];
                                        if (Parobj.VPerMon || Parobj.VPerMonYear)
                                                worksheet.Cells[row, columnCount++].Value = reader["ByMonth"];
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
                                            if (Parobj.VCompany)
                                                worksheet.Cells[row, columnCount++].Value = reader["Company"];
                                            if (Parobj.VTransactionNumber)
                                                worksheet.Cells[row, columnCount++].Value = reader["TransactionNumber"];
                                            if (Parobj.VQty)
                                                worksheet.Cells[row, columnCount++].Value = reader["TotalQty"];
                                            if (Parobj.VPrice)
                                                worksheet.Cells[row, columnCount++].Value = reader["Price"];
                                            if (Parobj.VCost)
                                                worksheet.Cells[row, columnCount++].Value = reader["Cost"];
                                            worksheet.Cells[row, columnCount].Style.Numberformat.Format = "#,##0.00";
                                            if (Parobj.VTotalSales)
                                                worksheet.Cells[row, columnCount++].Value = reader["TotalSales"];
                                            if (Parobj.VTransactionCount && !Parobj.SalesBranchPage)
                                                worksheet.Cells[row, columnCount++].Value = reader["TransactionsCount"];
                                            if (Parobj.VTotalCost)
                                                worksheet.Cells[row, columnCount++].Value = reader["TotalCost"];
                                            if (Parobj.VTotalTax)
                                                worksheet.Cells[row, columnCount++].Value = reader["TotalTax"];
                                            if (Parobj.VTotalSalesTax)
                                                worksheet.Cells[row, columnCount++].Value = reader["TotalSaleswithTax"];
                                            if (Parobj.VTotalSalesWithoutTax)
                                                worksheet.Cells[row, columnCount++].Value = reader["TotalSalesWithoutTax"];
                                            if (Parobj.VTotalCostQty)
                                                worksheet.Cells[row, columnCount++].Value = reader["TotalCostQty"];
                                            worksheet.Cells[row, columnCount].Style.Numberformat.Format = "yyyy-MM-dd";
                                            if (Parobj.VPerDay)
                                                worksheet.Cells[row, columnCount++].Value = reader["Date"];
                                            if (Parobj.VDateandtime)
                                                worksheet.Cells[row, columnCount++].Value = reader["DateandTime"];
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
                                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AKSoftSales.xlsx");
                            }
                        }
                        catch
                        {
                            HttpContext.Session.SetString("ExportStatus", "unKnown1");
                            return View();
                        }
                    }
                }

           }
         catch (Exception ex) { return View(); }

            }
            [HttpPost]
            public IActionResult Index(SalesParameters Parobj)
            {
            if (Parobj.Store==null)
            {
                Parobj.Store = "0";
            }
            string[] storeVal = Parobj.Store.Split(':');
            Parobj.MainSales = true;
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
            if (username is null)
            {
                return RedirectToAction("Store", "Home");
            }
            if (Role == "TerrManager" || Role == "FoodManager")
            {
                Parobj.isDmanager = true;
                Parobj.isFmanager = true;
            }
            IQueryable<Storeuser> queryAll; // Initialize to avoid null reference exception
            IQueryable<Storeuser> queryRM;
            IQueryable<Storeuser> queryDy;
            IQueryable<Storeuser> queryRich;
            if (Parobj.isDmanager || Parobj.isUsername || Parobj.isFmanager)
            {
                // If the username matches either the Dmanager or the Username, filter the stores accordingly
                queryAll = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username || s.Fmanager == username);
                queryRM = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username || s.Fmanager == username);
                queryDy = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username || s.Fmanager == username);
                queryRich = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username || s.Fmanager == username);
            }
            else
            {
                // If neither condition is met, display all stores
                queryAll = db2.Storeusers;
                queryRM = db2.Storeusers.Where(x => x.Storenumber == "RMS" || x.RmsstoNumber == "Dy"); // RMS or Dy
                queryDy = db2.Storeusers.Where(x => x.Storenumber != "RMS"); // Neither RMS nor Dy
                queryRich = db2.Storeusers.Where(x => x.RmsstoNumber == "R1" || x.RmsstoNumber == "R2" || x.RmsstoNumber == "R3"); // Neither RMS nor Dy
            }
            // Transform and assign to ViewBag
            ViewBag.VBStoreRM = queryRM
                .GroupBy(m => m.Name)
                .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
                .OrderBy(m => m.Name)
                .ToList();

            ViewBag.VBStoreRich = queryRich
               .GroupBy(m => m.Name)
               .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
               .OrderBy(m => m.Name)
               .ToList();
            ViewBag.VBStoreDy = queryDy
                                .GroupBy(m => m.Name)
                                .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
                                .OrderBy(m => m.Name)
                                .ToList();
            ViewBag.VBStore = queryAll
                                .GroupBy(m => m.Name)
                                .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
                                .OrderBy(m => m.Name)
                                .ToList();
            if (Role == "SalesM")
            {
                ViewBag.VBStore = queryDy
                                .GroupBy(m => m.Name)
                                .Select(group => new { Store = group.First().Storenumber + ":" + group.First().RmsstoNumber, Name = group.Key })
                                .OrderBy(m => m.Name)
                                .ToList();
                Parobj.Franchise = "TMT";
            }
           
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
            // Call the ExportToExcel method

                if ((Parobj.RMS && Parobj.TMT == false && storeVal[0] == "RMS") || (Parobj.RMS && Parobj.TMT == false && storeVal[0] == "0")||(Parobj.RMS && storeVal[0] == "RMS") || StoreIddynamic == "RMS" || Parobj.RichCut)
            {
                return ExportToExcel(Parobj.RmsConnection, Parobj);
             }
                else if (Parobj.RMS == false && Parobj.TMT || (storeVal.Length > 1 && storeVal[1] == "Dy") || (Parobj.SalesBranchPage && StoreIddynamic != "RMS") || (Parobj.RMS == false && Parobj.TMT && storeVal[0] != "RMS"))
                           {
                    return ExportToExcel(Parobj.TopSoftConnection, Parobj);
                }
            else if (Parobj.RMS && Parobj.TMT || storeVal.Length > 1)
            {
                return ExportToExcel(Parobj.TopSoftConnection, Parobj);
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
            }
        [HttpGet]
        public IActionResult Offer()
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
                                                //Store List Text=StoreName , Value = StoreI

            if (username is null)
            {
                return RedirectToAction("Store", "Home");
            }
            if (Role == "TerrManager" || Role == "FoodManager")
            {
                Parobj.isDmanager = true;
                Parobj.isFmanager = true;
            }
            IQueryable<Storeuser> query;
            if (Parobj.isDmanager || Isuser == "True" || Parobj.isFmanager)
            {
                // If the username matches either the Dmanager or the Username, filter the stores accordingly
                query = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username || s.Fmanager == username);
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
            ViewBag.VBOffer = Axdb.Retailperiodicdiscounts
                                                  .Select(g => g.Name)
                                                  .Distinct()
                                                  .ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Offer(SalesParameters Parobj)
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
            AxdbContext Axdb = new AxdbContext();
            CkproUsersContext db2 = new CkproUsersContext();
            db.Database.SetCommandTimeout(7200);// Set the timeout in seconds
                                                //Store List Text=StoreName , Value = StoreI

            if (username is null)
            {
                return RedirectToAction("Store", "Home");
            }
            if (Role == "TerrManager" || Role == "FoodManager")
            {
                Parobj.isDmanager = true;
                Parobj.isFmanager = true;
            }
            IQueryable<Storeuser> query;
            if (Parobj.isDmanager || Isuser == "True" || Parobj.isFmanager)
            {
                // If the username matches either the Dmanager or the Username, filter the stores accordingly
                query = db2.Storeusers
                    .Where(s => s.Dmanager == username || s.Username == username || s.Fmanager == username);
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

            ViewBag.VBOffer = Axdb.Retailperiodicdiscounts
                                                  .Select(g => g.Name)
                                                  .Distinct()
                                                  .ToList();
            Parobj.OfferPage = true;
            string[] storeVal = Parobj.Store.Split(':');
            return ExportToExcel(Parobj.AxdbConnection, Parobj);
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
//[HttpGet]
//public IActionResult SalesReport(string startDate, string endDate, string Store, string Department, int dis, bool exportAfterClick, string[] selectedItems)
//{
//    DataCenterContext db = new DataCenterContext();
//    CkproUsersContext Prodb = new CkproUsersContext();
//    IQueryable<CK.Models.RptStore> RptStoreData = db.RptStores;
//    db.Database.SetCommandTimeout(7800); // Set the timeout in seconds
//    var username = HttpContext.Session.GetString("Username");
//    var Role = HttpContext.Session.GetString("Role");
//    var ServerName = HttpContext.Session.GetString("Server");
//    ViewBag.Username = username;
//    ViewBag.Role = Role;
//    //var serverName = _dataCenterContext.Servers.FirstOrDefault()?.ServerName ?? "DefaultServer";
//    //var dbName = _ckproUsersContext.Databases.FirstOrDefault()?.DatabaseName ?? "DefaultDB";
//    bool isFmanager = Prodb.RptUsers.Any(s => s.Fmanager == username);
//    bool isDmanager = Prodb.RptUsers.Any(s => s.Dmanager == username);
//    bool isUsername = Prodb.RptUsers.Any(s => s.Username == username && (s.Storenumber != null || s.Storenumber != " "));
//    IQueryable<Storeuser> query;
//    if (isDmanager || isUsername || isFmanager)
//    {
//        RptStoreData = RptStoreData.Where(s => s.StoreName == username);
//    }
//    ViewBag.VBDepartment = db.Departments.Select(m => new { m.Code, m.Name }).Distinct();
//    if (Department != null)
//    {
//        RptStoreData = RptStoreData.Where(s => s.DpId.ToString() == Department);
//    }

//    if (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate) && Store is null && Department is null)
//    {
//        return View();
//    }
//    //string ServerStore = (from c in _contextt.Storeuser
//    //                      where c.Name == Retail
//    //                      select c.Server).Single();
//    //string? DbStore = (from c in _contextt.Storeuser
//    //                   where c.Name == Retail
//    //                   select c.Dbase).Single();
//    string constr = string.Format("Server={0};User ID=sa;Password=P@ssw0rd;Database=RetailChannelDatabase;Connect Timeout=50;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;", ServerName);
//    ServerConnection ServerCont = new ServerConnection(constr);
//    ViewBag.StartStopwatch = true;
//    var reportData1 = RptStoreData.GroupBy(d => new { d.StoreName, d.DpName, d.Qty, d.ItemLookupCode })
//        .Select(g => new
//        {
//            Total = g.Sum(d => d.Qty),
//            StoreName = g.Key.StoreName,
//            ItemLookupCode = g.Key.ItemLookupCode,
//            DepName = g.Key.DpName
//        }).OrderByDescending(x => x.Total);
//    ViewBag.Data = reportData1;
//    if (exportAfterClick == false)

//    {
//        return View();
//    }
//    else
//    {
//        return View();
//    }

//}

//[HttpGet]
//public IActionResult ATransferSol1(string transferId)
//{
//    DataCenterContext db = new DataCenterContext();
//    IQueryable<CK.Models.R> reportData1 = Enumerable.Empty<CK.Models.R>().AsQueryable(); // Initialize with an empty queryable
//    var username = HttpContext.Session.GetString("Username");
//    var Role = HttpContext.Session.GetString("Role");
//    ViewBag.Username = username;
//    ViewBag.Role = Role;
//    if (!string.IsNullOrEmpty(transferId))
//    {
//        reportData1 = db.Rs
//            .Where(d => d.Transaction == transferId)
//            .GroupBy(d => new { d.FromTable, d.ToTable, d.Item, d.Barcode, d.Day, d.Transaction, d.Status })
//            .Select(g => new CK.Models.R
//            {
//                FromTable = g.Key.FromTable,
//                Quantity = g.Sum(d => d.Quantity),
//                Item = g.Key.Item,
//                Barcode = g.Select(d => d.Barcode).FirstOrDefault(),
//                Day = g.Key.Day,
//                Transaction = g.Key.Transaction,
//                ToTable = g.Key.ToTable,
//                Status = g.Key.Status
//            });

//    }

//    // Convert IQueryable to List before passing it to the view
//    var reportDataList = reportData1.ToList();

//    return View(reportDataList);
//}
//public IActionResult ATransferSol(string transferId)
//{
//    DataCenterContext db = new DataCenterContext();
//    IQueryable<CK.Models.R> reportData1 = Enumerable.Empty<CK.Models.R>().AsQueryable(); // Initialize with an empty queryable
//    var username = HttpContext.Session.GetString("Username");
//    var Role = HttpContext.Session.GetString("Role");
//    ViewBag.Username = username;
//    ViewBag.Role = Role;
//    if (!string.IsNullOrEmpty(transferId))
//    {
//        reportData1 = db.Rs
//            .Where(d => d.Transaction == transferId)
//            .GroupBy(d => new { d.FromTable, d.ToTable, d.Item, d.Barcode, d.Day, d.Transaction, d.Status })
//            .Select(g => new CK.Models.R
//            {
//                FromTable = g.Key.FromTable,
//                Quantity = g.Sum(d => d.Quantity),
//                Item = g.Key.Item,
//                Barcode = g.Select(d => d.Barcode).FirstOrDefault(),
//                Day = g.Key.Day,
//                Transaction = g.Key.Transaction,
//                ToTable = g.Key.ToTable,
//                Status = g.Key.Status
//            });

//    }

//    // Convert IQueryable to List before passing it to the view
//    var reportDataList = reportData1.ToList();

//    return View(reportDataList);
//}

