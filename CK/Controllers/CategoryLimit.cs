using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using CK.Models;
using iTextSharp.text.pdf;
namespace CK.Controllers
{
    public class CategoryLimit : Controller
    {
        SalesParameters Parobj = new SalesParameters();
        [HttpPost]
        public IActionResult AttachItemwithCategory(SalesParameters Paroj, string CategoryName, string ItemBarCode)
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
            using (SqlConnection connection = new SqlConnection(Paroj.RmsConnection))
            {
                using (SqlCommand command = new SqlCommand("select distinct BarCode,Category from Ckprousers.dbo.Catlimit", connection))
                {

                    connection.Open(); // Open the connection
                    command.ExecuteNonQuery(); // Execute the command
                    var vi = new List<RptSale>();
                    var ll = 0;
                    var test = command.ExecuteReader();
                    while (test.Read())
                    {
                        RptSale si = new RptSale();
                        si.DpName = test["Category"].ToString();
                        si.ItemLookupCode = test["BarCode"].ToString();
                        vi.Add(si);
                    }
                    ViewBag.Data = vi.Select(x => x.DpName).Distinct().ToList();
                    var Item = vi.Where(x => x.ItemLookupCode == ItemBarCode).Select(x => x.ItemLookupCode).Distinct().ToList();
                    if (Item.Count > 0)
                    {
                        ViewBag.Item = "V";
                        return View("AttachItemwithCategory");
                    }
                }
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(Paroj.RmsConnection))
                {
                    using (SqlCommand command = new SqlCommand("INSERT INTO  Ckprousers.dbo.Catlimit (Category, BarCode) VALUES (@Category, @Barcode)", connection))
                    {

                        command.Parameters.AddWithValue("@Category", CategoryName);
                        command.Parameters.AddWithValue("@Barcode", ItemBarCode);
                        connection.Open(); // Open the connection
                        command.ExecuteNonQuery(); // Execute the command
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
            ViewBag.ItemSave = "V";
            return View("AttachItemwithCategory");
        }
        [HttpGet]
        public IActionResult AttachItemwithCategory(string ItemBarCode)
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
            using (SqlConnection connection = new SqlConnection(Parobj.RmsConnection))
            {
                using (SqlCommand command = new SqlCommand("select distinct BarCode,Category from Ckprousers.dbo.Catlimit", connection))
                {

                    connection.Open(); // Open the connection
                    command.ExecuteNonQuery(); // Execute the command
                    var vi = new List<RptSale>();
                    var ll = 0;
                    var test = command.ExecuteReader();
                    while (test.Read())
                    {
                        RptSale si = new RptSale();
                        si.DpName = test["Category"].ToString();
                        si.ItemLookupCode = test["BarCode"].ToString();
                        vi.Add(si);
                    }
                    ViewBag.Data = vi.Select(x => x.DpName).Distinct().ToList();
                    var Item = vi.Where(x => x.ItemLookupCode == ItemBarCode).Select(x => x.ItemLookupCode).Distinct().ToList();
                    if (Item.Count > 0)
                    {
                        ViewBag.Item = "V";
                        return View("AttachItemwithCategory");
                    }
                }
            }
            return View("AttachItemwithCategory");
        }
        public IActionResult DeleteItem(SalesParameters Paroj)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Paroj.RmsConnection))
                {
                    using (SqlCommand command = new SqlCommand("Delete from  Ckprousers.dbo.Catlimit where Id =@Id", connection))
                    {

                        command.Parameters.AddWithValue("@Id", Paroj.Supplier);
                        connection.Open(); // Open the connection
                        command.ExecuteNonQuery(); // Execute the command
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("DisplayItems", "CategoryLimit");
        }
        [HttpGet]
        public IActionResult DisplayItems(SalesParameters Paroj, string CategoryName, string ItemBarCode)
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
                using (SqlConnection connection = new SqlConnection(Paroj.RmsConnection))
                {
                    using (SqlCommand command = new SqlCommand("select Id,Category, Barcode from Ckprousers.dbo.Catlimit ", connection))
                    {
                        connection.Open(); // Open the connection
                        command.ExecuteNonQuery(); // Execute the command
                        var vi = new List<RptSale>();
                        var ll = 0;
                        var test = command.ExecuteReader();
                        while (test.Read())
                        {
                            RptSale si = new RptSale();
                            si.StoreName = test["Category"].ToString();
                            si.ItemLookupCode = test["Barcode"].ToString();
                            si.ItemName = test["Id"].ToString();
                            ViewBag.Id = si.ItemName;
                            vi.Add(si);
                        }
                        ViewBag.Data = vi;
                        return View("DisplayItems");
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }
    }
}
