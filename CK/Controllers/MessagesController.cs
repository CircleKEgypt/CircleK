using CK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CK.Controllers
{
    public class MessagesController : Controller
    {
        SalesParameters Parobj = new SalesParameters();
        [HttpGet]
        public IActionResult EditCloseDay(SalesParameters Parobj, int Id, string Message, string StartDate, string EndDate)
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
                using (SqlCommand command = new SqlCommand("SELECT * from CloseDay where Id=@Id ", connection))
                {
                    connection.Open(); // Open the connection
                    //command.Parameters.Add(new SqlParameter("@Alert", Message));
                    command.Parameters.Add(new SqlParameter("@Id", Id));
                    var vi = new List<RptSale>();
                    var test = command.ExecuteReader();
                    while (test.Read())
                    {
                        RptSale si = new RptSale();
                        //si.StoreName = test["alert"].ToString();
                        //ViewBag.Branches = si.StoreName;
                        DateTime startDate = Convert.ToDateTime(test["StartDate"]);
                        DateTime endDate = Convert.ToDateTime(test["EndDate"]);

                        // Assigning the formatted date strings to ViewBag
                        ViewBag.SupplierName = startDate.ToString("yyyy-MM-dd");
                        ViewBag.Dmanager = endDate.ToString("yyyy-MM-dd");
                        //si.SupplierName = test["StartDate"].ToString();
                        //ViewBag.SupplierName = si.SupplierName;
                        //si.Dmanager = test["EndDate"].ToString();
                        //ViewBag.Dmanager = si.Dmanager;
                        vi.Add(si);
                    }
                    //var orderedVi = vi.OrderBy(sale => sale.StoreName).ToList();
                    ViewBag.Data = vi;
                    return View();
                }
            }
        }
        [HttpPost]
        public IActionResult EditCloseDay(int itemId, SalesParameters Paroj, string Message, int id, string StartDate, string EndDate)
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

                    using (SqlCommand command = new SqlCommand("UPDATE CloseDay SET StartDate = @StartDate ,EndDate=@EndDate where Id=@Id", connection))
                    {
                        //command.Parameters.Add(new SqlParameter("@Message", Message));
                        command.Parameters.Add(new SqlParameter("@Id", id));
                        command.Parameters.Add(new SqlParameter("@StartDate", StartDate));
                        command.Parameters.Add(new SqlParameter("@EndDate", EndDate));
                        command.ExecuteNonQuery(); // Execute the command
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("CreateCloseDay", "Messages");
        }

        public IActionResult DeleteCloseDay(SalesParameters Parobj, int Id)
        {
            using (SqlConnection connection = new SqlConnection(Parobj.TopSoftConnection))
            {
                using (SqlCommand command = new SqlCommand("delete from CloseDay where Id=@Id", connection))
                {
                    connection.Open(); // Open the connection
                    command.Parameters.Add(new SqlParameter("@Id", Id));
                    var test = command.ExecuteReader();
                    return RedirectToAction("CreateCloseDay", "Messages");
                }
            }
        }
        [HttpGet]
        public IActionResult CreateCloseDay()
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
                using (SqlCommand command = new SqlCommand("SELECT * FROM CloseDay ", connection))
                {
                    connection.Open(); // Open the connection
                    double TotalSales = 0;
                    var vi = new List<RptSale>();
                    var test = command.ExecuteReader();
                    while (test.Read())
                    {
                        RptSale si = new RptSale();
                        //si.StoreName = test["alert"].ToString();
                        si.ItemName = test["Id"].ToString();
                        si.SupplierName = test["StartDate"].ToString();
                        si.Dmanager = test["EndDate"].ToString();
                        DateTime parsedDate;
                        if (DateTime.TryParse(si.SupplierName, out parsedDate))
                        {
                            si.SupplierName = parsedDate.ToString("yyyy-MM-dd");
                        }
                        DateTime parsedDate2;
                        if (DateTime.TryParse(si.Dmanager, out parsedDate))
                        {
                            si.Dmanager = parsedDate.ToString("yyyy-MM-dd");
                        }
                        vi.Add(si);
                    }
                    //var orderedVi = vi.OrderBy(sale => sale.StoreName).ToList();
                    ViewBag.Data = vi;
                    return View("CreateCloseDay");
                }
            }
            return View("CreateMessage");
        }
        [HttpPost]
        public async Task<IActionResult> CreateCloseDayIn(SalesParameters Paroj, string Message, string StartDate, string EndDate)
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
                    using (SqlCommand command = new SqlCommand("INSERT INTO  CloseDay (StartDate,EndDate) VALUES (@Start,@End)", connection))
                    {

                        //command.Parameters.AddWithValue("@Message", Message);
                        command.Parameters.AddWithValue("@Start", StartDate);
                        command.Parameters.AddWithValue("@End", EndDate);
                        connection.Open(); // Open the connection
                        command.ExecuteNonQuery(); // Execute the command
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("CreateCloseDay", "Messages");
            }
            return RedirectToAction("CreateCloseDay", "Messages");
        }
    }
}
