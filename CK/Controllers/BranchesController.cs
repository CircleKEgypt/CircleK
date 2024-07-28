using CK.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using CK.Model;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
namespace CK.Controllers
{
    public class BranchesController : Controller
    {
        AxdbContext Axdb = new AxdbContext();
        DataCenterContext db = new DataCenterContext();
        CkproUsersContext db2 = new CkproUsersContext();
        CkhelperdbContext db3 = new CkhelperdbContext();
        DataCenterPrevYrsContext db4 = new DataCenterPrevYrsContext();
        private readonly ILogger<BranchesController> _logger;
        private readonly CkproUsersContext _dbContext;
        private static readonly List<RptUser3> Users = new List<RptUser3>();
        public BranchesController(ILogger<BranchesController> logger, CkproUsersContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
 
        public string encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public string Decrypt(string clearText)
        {
            string DecryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Convert.FromBase64String(clearText);
            using (Aes decryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(DecryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                decryptor.Key = pdb.GetBytes(32);
                decryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return clearText;
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        public IActionResult CreateStore()
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
            ViewBag.VBDManager = _dbContext.Users.Where(a => a.Role == "TerrManager").Select(x => x.User1).Distinct().ToList();
            ViewBag.VBFManager = _dbContext.Users.Where(a => a.Role == "FoodManager").Select(x => x.User1).Distinct().ToList();
            var user = new User();
            ViewBag.MaxId = _dbContext.Storeusers.Max(x => x.Id+1);
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStore([Bind("Inventlocation,Storenumber,Username,Password,Name,Server,RmsstoNumber,Id,Email,Dbase,PriceCategory,Franchise,Company,Zkip,StartDate,ArabicN,District,Dmanager,Fmanager")] Storeuser store)
        {
            // Encrypt the password before saving
            //ViewBag.VBDManager = _dbContext.Users.Where(a => a.Role == "TerrManager").Select(x => x.User1).Distinct().ToList();
            //ViewBag.VBFManager = _dbContext.Users.Where(a => a.Role == "FoodManager").Select(x => x.User1).Distinct().ToList();
            //    DateTime endDate = Convert.ToDateTime(store.StartDate);
            //    store.StartDate = endDate.ToString("yyyy-MM-dd");
            store.Inventlocation ??= "Null";
            store.Storenumber ??= "Null";
            store.RmsstoNumber ??= "Null";
            store.ArabicN ??= "Null";
            store.Company ??= "Null";
            store.StartDate ??= "Null";
            store.Dbase ??= "Null";
            store.DecryptedPassword ??= "Null";
            store.District ??= "Null";
            store.Dmanager ??= "UnKnown";
            store.Fmanager ??= "UnKnown";
            store.Email ??= "Null";
            store.Franchise??= "Null";
            store.Name ??= "Null";
            store.PriceCategory ??= "Null";
            store.Server ??= "Null";
            store.Zkip ??= "Null";
            store.Username ??= "Null";
            store.Password ??= "Null";
            store.Password = encrypt(store.Password);

            _dbContext.Add(store);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("DisplayStores");
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
        public async Task<IActionResult> DisplayStores()
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
            using (var db2 = new CkproUsersContext())
            {
                var storeUsers = await db2.Storeusers.OrderByDescending(x=>x.Id).ToListAsync();
                // Decrypt each user's password
                foreach (var user in storeUsers)
                {
                    user.DecryptedPassword = decrypt(user.Password); // Assuming 'decrypt' is your decryption method
                }

                var stores = _dbContext.Storeusers.OrderByDescending(x => x.Id).ToList();
                if (username == null)
                {
                    return View("Logout");
                }
                return View(storeUsers);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditStore(int? id)
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
            var stores = await _dbContext.Storeusers.FindAsync(id);
            ViewBag.VBDManager = _dbContext.Users.Where(a => a.Role == "TerrManager").Select(x => x.User1).Distinct().ToList();
            ViewBag.VBFManager = _dbContext.Users.Where(a => a.Role == "FoodManager").Select(x => x.User1).Distinct().ToList();
            if (stores.StartDate!= "Null")
            {
                DateTime endDate = Convert.ToDateTime(stores.StartDate);
                stores.StartDate = endDate.ToString("yyyy-MM-dd");
            }
            // Assigning the formatted date strings to ViewBag
           // ViewBag.SupplierName = startDate.ToString("yyyy-MM-dd");
            //if Password Encrypted Algorithm is not 64 i will display this password as it -Ahmed Hosny
            try
            {
                stores.Password = Decrypt(stores.Password);
            }
            catch (Exception ex)
            {
                stores.Password = stores.Password;
            }
            //stores.Password = Decrypt(stores.Password);
            return View(stores);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStore(int id, [Bind("Inventlocation,Storenumber,Username,Password,Name,Server,RmsstoNumber,Id,Email,Dbase,PriceCategory,Franchise,Company,Zkip,StartDate,ArabicN,District,Dmanager,Fmanager")] Storeuser store)
        {

            store.Inventlocation ??= "Null";
            store.Storenumber ??= "Null";
            store.RmsstoNumber ??= "Null";
            store.ArabicN ??= "Null";
            store.Company ??= "Null";
            store.StartDate ??= "Null";
            store.Dbase ??= "Null";
            store.DecryptedPassword ??= "Null";
            store.District ??= "Null";
            store.Dmanager ??= "UnKnown";
            store.Fmanager ??= "UnKnown";
            store.Email ??= "Null";
            store.Franchise ??= "Null";
            store.Name ??= "Null";
            store.PriceCategory ??= "Null";
            store.Server ??= "Null";
            store.Zkip ??= "Null";
            store.Username ??= "Null";
            store.Password = encrypt(store.Password);
            _dbContext.Update(store);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("DisplayStores");
        }

        [HttpPost]
        public IActionResult DeleteStore(int? id)
        {
            // Retrieve the user details from the database based on the username
            var store = _dbContext.Storeusers.FirstOrDefault(u => u.Id == id);
            if (store == null)
            {
                return NotFound(); // Return a 404 Not Found if user is not found
            }

            // Remove the user from the database
            _dbContext.Storeusers.Remove(store);
            _dbContext.SaveChanges();

            // Redirect to the display users page after the deletion is successful
            return RedirectToAction("DisplayStores");
        }
        public IActionResult Privacy()
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

