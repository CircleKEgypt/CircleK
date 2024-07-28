using CK.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace CK.Controllers
{
    public class AccountsController : Controller
    {

        AxdbContext Axdb = new AxdbContext();
        CkproUsersContext CC = new CkproUsersContext();
        EasysoftContext ES = new EasysoftContext();
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
            ViewBag.uuu = Isuser;
            var warningMessage = TempData["Warnning"];
            ViewBag.WarningMessage = warningMessage;
            return View();
        }
        [HttpPost]
        public void AESII(double lastsort, double TotAmt, string DocNo, DateTime DocDate, string CustId)
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

        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////Invoices        

        // Insert Easysoft Invoice Products
        [HttpPost]
        public IActionResult AESIIP(string salesid, DateTime start)
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
            var story = (from st in Axdb.Saleslines  // Nagy remove condition Modifieddatetime
                         where st.Salesid == salesid & st.Createddatetime.Date == start.Date & st.Modifieddatetime.Date == DateTime.Now.Date &
                         st.Custgroup == "Third Part" & st.Recversion != 1  // TransDate put DateTime.Now.Date
                         select new MyDy2 { Location = st.Salesid, Depa = st.Custaccount, Descript = st.Itemid, TransDate = st.Modifieddatetime, Cost = st.Qtyordered, Qty = st.Lineamount, CostA = st.Salesprice }).ToList();
            var listtax = (from tax in CC.Itaxes
                           select tax.ItemCode).ToList();
            var listcust = (from tacu in CC.CustInvs
                            select tacu.Code).ToList();
            var invalid = story.Where(x => !listtax.Contains(x.Descript)).ToList();
            var invalidCus = story.Where(x => !listcust.Contains(x.Depa)).ToList();
            if (invalid.Count() != 0)
            {
                string[] testnow = { };
                int i = 0;
                foreach (var item in invalid)
                {
                    TempData["Warnning"] += "Not Found : ";
                    TempData["Warnning"] += item.Descript! + ",";
                    i++;

                }

                // Today's date is blocked

                //return RedirectToAction("AESGet", testnow);
                return RedirectToAction("Index", "Accounts");

            }

            if (invalidCus.Count() != 0)
            {
                TempData["Warnning"] = "Customer Not Found";
                //return RedirectToAction("AESGet");
                return RedirectToAction("Index", "Accounts");
            }
            double lastsort = (from sor in ES.SalesInvoices
                               orderby sor.SortFi descending
                               select sor.SortFi).FirstOrDefault();
            int Sn = 0;
            double TotAmt = 0.0;
            string DocNo = "";
            string CustId = "";
            DateTime DocDate = new DateTime();
            //for
            foreach (var t in story)
            {
                int tax = (from ta in CC.Itaxes
                           where t.Descript == ta.ItemCode
                           select ta.Tax).First();
                SalesInvTran Inv = new SalesInvTran();
                Inv.SortFi = Convert.ToInt32(lastsort) + 1;
                Inv.BranchId = "1";
                Inv.TrType = 1;
                Inv.SerialId = "1";
                ///salesid
                Inv.DocNo = t.Location!.Replace("TMT-00", "");
                Inv.Sn = Sn + 1;
                Inv.RelatedSn = 0;
                ///////product
                Inv.ProdId = t.Descript!.ToString();
                Inv.ChangedId = "";
                Inv.ProdOtherId = Inv.ProdId;
                Inv.ProdOtherName = "";
                Inv.UnitId = "1";
                Inv.Tall = 0;
                Inv.Height = 0;
                Inv.Thikness = 0;
                Inv.Weight = 0;
                ///////qty and rate
                Inv.Qty = (double)t.Cost;
                Inv.Rate = (double)t.CostA;
                Inv.UserPrice = 0;
                Inv.DiscountAmt = 0;
                Inv.DiscountPercent = 0;
                Inv.DiscTax = 0;
                //////taxes
                Inv.EdafaTax = tax;
                Inv.Disc3 = 0;
                Inv.Disc3Val = 0;
                //////
                Inv.Amt = Inv.Qty * Inv.Rate * (1 + tax / 100);
                Inv.StoreId = "1";
                Inv.SelComm = 0;
                Inv.ProdDate = new DateTime(1900, 1, 1);
                Inv.ExpDate = new DateTime(1900, 1, 1);
                Inv.CstId = "";
                Inv.RealQty = 1;
                Inv.TotQty = Inv.Qty;
                Inv.Cost = 0;
                Inv.CostPlus = 0;
                Inv.AllCost = 0;
                Inv.IsF9 = 0;
                Inv.F9qty = 0;
                Inv.ItemNotes = "";
                Inv.NodeId = "";
                Inv.OrderSortfi = 0;
                Inv.OrderDocNo = "";
                Inv.OrderBranchId = "";
                Inv.ProdSerialId = "";
                Inv.ProdAvgCost = 0;
                Inv.CreateBranchId = "1";
                Inv.DeliveryDays = 0;
                Inv.StampingTax = 0;

                TotAmt += Inv.Amt;
                Sn += 1;
                DocNo = Inv.DocNo;
                DocDate = t.TransDate;
                CustId = t.Depa!;
                ES.Add(Inv);
                ES.SaveChanges();

            };


            string? cust = (from cu in CC.CustInvs
                            where cu.Code == CustId
                            select cu.No).First();

            SalesInvoice Inve = new SalesInvoice();

            Inve.SortFi = Convert.ToInt32(lastsort) + 1;
            Inve.BranchId = "1";
            Inve.TrType = 1;
            Inve.SerialId = "1";
            Inve.DocNo = DocNo;
            Inve.Lrno = "";
            Inve.Lrsortfi = 0;
            Inve.LrbranchId = "";
            ////date
            Inve.DocDate = DocDate.Date;
            Inve.DocTime = new DateTime(1900, 1, 1);
            Inve.DocType = 2;
            Inve.Part = 0;
            Inve.SecondPart = 0;
            Inve.DueDateWay = 0;
            Inve.DueDate = Inve.DocDate;
            Inve.IsCarry = 0;
            Inve.SalesWay = 0;
            Inve.AccCode = "40101";
            Inve.Type = 0;
            /////customer
            Inve.CustId = cust!;
            Inve.StaffId = "";
            Inve.CurrId = "1";
            Inve.CurrVal = 1;
            Inve.VisaId = "0";
            ////amount
            Inve.TotAmt = TotAmt;
            Inve.TotInWords = "";
            Inve.Disc = 0;
            Inve.DiscPercent = 0;
            Inve.TotDesc = 0;
            Inve.MTax = 0;
            Inve.Task1 = 0;
            Inve.TotExp = 0;
            Inve.ClassId = "";
            Inve.ClassVal = 0;
            Inve.JournalDoc = 0;
            Inve.StockDoc = "0";
            Inve.VisaCashSortFi = 0;
            //notes
            Inve.Notes = "C";
            Inve.CstId = "";
            Inve.Transfered = 0;
            Inve.BankId = "";
            Inve.Payment = "";
            Inve.Documents = "";
            Inve.TransAcc = 0;
            Inve.OfferStates = 0;
            Inve.CreateBranchId = "1";
            Inve.PartInWords = "";
            Inve.ChangeInWords = "";
            Inve.SumAmt = 0;
            Inve.SumEdafaTax = 0;
            Inve.Uuid = "";
            Inve.Status = "";
            Inve.PosCashRecive = 0;


            CC.Add(Inve);
            CC.SaveChanges();
            TempData["Warnning"] = "Successful";
            // return RedirectToAction("AESGet");
            return RedirectToAction("Index", "Accounts");




            ////////////////////////////////////////////////////////////////////////////////////////////////////////////Invoices
        }
        public ActionResult AESGet()
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
            return View();
        }
        [HttpPost]
        public ActionResult AESGetInv(SalesParameters Parobj, DateTime start, string PO)
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
            string PoID = "TMT-00" + PO;
            string P = PO.Remove(0, 6);
            string? checkpo = (from INVE in ES.SalesInvoices
                               where INVE.DocNo == P
                               select INVE.DocNo).FirstOrDefault();

            /////////Nagyyyyyyyyyyyyyyyy remove Modifieddatetime
            if (checkpo != null)
            {
                TempData["Warnning"] = "PO already Exists in EasySoft";
                // return RedirectToAction("AESGet");
                return RedirectToAction("Index", "Accounts");

            }
            var story = (from st in Axdb.Saleslines
                         where st.Createddatetime.Date == start.Date //& st.Modifieddatetime.Date == DateTime.Now.Date
                                                                     &
                         st.Custgroup == "Third Part" & st.Recversion != 1 &
                         st.Salesid == PO
                         select st.Salesid).Distinct().ToList();
            if (story.Count() != 0)
            {
            try
            {
                using (SqlConnection connection = new SqlConnection(Parobj.AxdbConnection))
                {
                    string SqlQuery = @" 
 delete from AXDBTest.dbo.[InvoiceDetails] where Salesid =  @SalesId  
 insert into  AXDBTest.dbo.[InvoiceDetails]
select Ra.No CustId,t.ItemCode,SUBSTRING(st.Salesid, 7, LEN(st.Salesid)) Location,Ra.Code, st.Custaccount Depa, st.Itemid Descript, 
 Cast(st.Modifieddatetime as date) TransDate,st.Qtyordered Cost
, st.Lineamount Qty, st.Salesprice CostA,t.tax Tax,st.Salesid,cast(st.Createddatetime as date)CreatedDate
from SALESLINE st
inner join (
select tacu.Code,tacu.No from AXDBTest.dbo.CustInv tacu
)Ra on Ra.Code collate SQL_Latin1_General_CP1_CI_AS=st.Custaccount
inner join 
(select distinct ta.Tax,ItemCode from  AXDBTest.dbo.itax ta ) t on  st.Itemid = t.ItemCode collate SQL_Latin1_General_CP1_CI_AS
where  st.Salesid =  @SalesId 
and st.Custgroup = 'Third Part' and st.Recversion != 1
 delete from [192.168.1.76\sql2016].[Easysoft].dbo.[Sales_InvTrans] where DocNo =  SUBSTRING(@SalesId, 7, LEN(@SalesId)) 
 insert into  [192.168.1.76\sql2016].[Easysoft].dbo.[Sales_InvTrans]
  --insert into  [192.168.1.76\sql2016].[Easysoft].[dbo].[Sales_InvTrans]
  select (select top 1 sor.SortFi+1 from  [192.168.1.76\sql2016].Easysoft.dbo.Sales_Invoice sor order by sor.SortFi desc)
  'In','1',1,'1',Location,ROW_NUMBER() OVER (ORDER BY Location) AS Sn,0,Descript,'',Descript,'','1',0,0,0,0,cost,costa,0,0,0,0,Tax,0,0,((cost*costa)*(1+(tax*0.01))),'1',0,'1900-01-01','1900-01-01','',1,cost,0,0,0,0,0,'','',0,'','','',0,'1',0,0
  from AXDBTest.dbo.[InvoiceDetails]
where Salesid =  @SalesId 
and cast(CreatedDate as date) = Cast(@CDate as date) --and cast(st.Modifieddatetime as date) =cast(getdate() as date)
  delete from [192.168.1.76\sql2016].[Easysoft].dbo.[Sales_invoice] where DocNo =  SUBSTRING(@SalesId, 7, LEN(@SalesId)) 
 insert into  [192.168.1.76\sql2016].[Easysoft].dbo.[Sales_invoice]
-- insert into  [192.168.1.76\sql2016].[Easysoft].[dbo].[Sales_invoice]
  select  (select top 1 sor.SortFi+1 from  [192.168.1.76\sql2016].Easysoft.dbo.Sales_Invoice sor order by sor.SortFi desc),
  	'1',1,'1',	Location,	'', 	0,	'',	Cast(getdate() as date),'1900-01-01',2,0,0,0,Cast(getdate() as date),0,0,'48101',0,CustId,'','1',1,'0',sum((cost*costa)*(1+(tax*0.01)))Total,'',0,0,0,0,0,0,'',0,0,'0',0,'C','',0,'','','',0,0,'1','','',0,0,'','',0
  from 
AXDBTest.dbo.[InvoiceDetails]

where Salesid =  @SalesId 
and cast(CreatedDate as date) = Cast(@CDate as date) --and cast(st.Modifieddatetime as date) =cast(getdate() as date)
group by Location,CustId
";
                    using (SqlCommand command = new SqlCommand(SqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@CDate", start);
                        command.Parameters.AddWithValue("@SalesId", PO);

                        connection.Open(); // Open the connection
                        command.ExecuteNonQuery(); // Execute the command
                           TempData["Warnning"] = "Good";
                        }
                    }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Accounts");
            }
        }
        else{
                try
                {
                    string cdate = (from st in Axdb.Saleslines
                                    where //st.Createddatetime.Date == start.Date & st.Modifieddatetime.Date == end.Date &
                                    st.Custgroup == "Third Part" & st.Recversion != 1 &
                                    st.Salesid == PO
                                    select st.Createddatetime).First().ToShortDateString()!;
                    string mdate = (from st in Axdb.Saleslines
                                    where //st.Createddatetime.Date == start.Date & st.Modifieddatetime.Date == end.Date &
                                    st.Custgroup == "Third Part" & st.Recversion != 1 &
                                    st.Salesid == PO
                                    select st.Modifieddatetime).First().ToShortDateString()!;
                    TempData["Warnning"] = "PO Not Found Created Date: " + cdate + " ,Confirmed Date: " + mdate;
                }
                catch
                {
                    return RedirectToAction("Index", "Accounts");
                }

            }
            return RedirectToAction("Index", "Accounts");
        }
        CkproUsersContext Ck =new CkproUsersContext();
        // GET: New Customer / Add
        public IActionResult AInvNewCust(SalesParameters Parobj)
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
            CkproUsersContext Ckpro = new CkproUsersContext();
            using (SqlConnection connection = new SqlConnection(Parobj.AxdbConnection))
            {
                using (SqlCommand command = new SqlCommand("select Code,No,Name from  AXDBTest.dbo.CustInv order by Code asc ", connection))
                {
                    connection.Open(); // Open the connection
                    double TotalSales = 0;
                    var vi = new List<RptSale>();
                    var test = command.ExecuteReader();
                    while (test.Read())
                    {
                        RptSale si = new RptSale();
                        si.StoreName = test["Code"].ToString();
                        ViewBag.Branches = si.StoreName;
                        si.Username = test["Name"].ToString();
                        ViewBag.Users = si.Username;
                       si.TotalSales = Convert.ToDouble(test["No"]);
                        ViewBag.No = si.TotalSales;
                        vi.Add(si);
                    }
                    //var orderedVi = vi.OrderBy(sale => sale.StoreName).ToList();
                    ViewBag.Data = vi;
                    return View("AInvNewCust");

                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewItem(SalesParameters Paroj, string ItemCode, string Tax)
        {
                try
                {
                    using (SqlConnection connection = new SqlConnection(Paroj.AxdbConnection))
                    {
                        using (SqlCommand command = new SqlCommand("INSERT INTO  AXDBTest.dbo.[ITax] (ItemCode, Tax) VALUES (@ItemCode, @Tax)", connection))
                        {

                            command.Parameters.AddWithValue("@ItemCode", ItemCode);
                            command.Parameters.AddWithValue("@Tax", Tax);
                            connection.Open(); // Open the connection
                            command.ExecuteNonQuery(); // Execute the command
                        }
                    }
                }
                catch (Exception ex)
                {
                    return View();
                }
            return RedirectToAction("AInvNewItem", "Accounts");
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewCust(SalesParameters Paroj,int NO, string Name, string Code)
        {
            CkproUsersContext Ckpro = new CkproUsersContext();
            int maxId;
            bool success = int.TryParse(Ckpro.CustInvs.Max(x => x.No).ToString(), out maxId);
            int x=0;
            if (success)
            {
               x= maxId + 1;
            }
            //NO = x.ToString();
           // NO = ViewBag.MaxId;
            if (Name !=null)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(Paroj.AxdbConnection))
                    {
                        using (SqlCommand command = new SqlCommand("INSERT INTO  AXDBTest.dbo.CustInv (No,Name,Code) VALUES (@No,@Name,@Code)", connection))
                        {

                            command.Parameters.AddWithValue("@No", NO);
                            command.Parameters.AddWithValue("@Name", Name);
                            command.Parameters.AddWithValue("@Code", Code);
                            connection.Open(); // Open the connection
                            command.ExecuteNonQuery(); // Execute the command
                        }
                    }
                }
                catch (Exception ex)
                {
                    return View();
                }
            }
            return RedirectToAction("AInvNewCust", "Accounts");
        }
        public IActionResult AInvNewItem(SalesParameters Parobj)
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
            CkproUsersContext Ckpro = new CkproUsersContext();
            using (SqlConnection connection = new SqlConnection(Parobj.AxdbConnection))
            {
                using (SqlCommand command = new SqlCommand("SELECT [ItemCode],[Tax] FROM AXDBTest.dbo.[ITax] ", connection))
                {
                    connection.Open(); // Open the connection
                    double TotalSales = 0;
                    var vi = new List<RptSale>();
                    var test = command.ExecuteReader();
                    while (test.Read())
                    {
                        RptSale si = new RptSale();
                        si.StoreName = test["ItemCode"].ToString();
                        ViewBag.Branches = si.StoreName;
                        si.Username = test["Tax"].ToString();
                        ViewBag.Users = si.Username;
                        vi.Add(si);
                    }
                    //var orderedVi = vi.OrderBy(sale => sale.StoreName).ToList();
                    ViewBag.Data = vi;
                    return View("AInvNewItem");

                }
            }
        }
        public IActionResult DisplayUploadedInvoices(SalesParameters Parobj)
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
            Parobj.DisplayUploadedInvoices = true;
            CkproUsersContext Ckpro = new CkproUsersContext();
            using (SqlConnection connection = new SqlConnection(Parobj.EasySoftConnection))
            {
                using (SqlCommand command = new SqlCommand("select top 1000 SortFi,DocNo,DocDate,DueDate,AccCode,CustId,TotAmt,UUID,Status from Sales_Invoice where cast(DueDate as date) >=cast(getdate()-7 as date) order by SortFi desc ", connection))
                {
                    connection.Open(); // Open the connection
                    double TotalSales = 0;
                    var vi = new List<RptSale>();
                    var test = command.ExecuteReader();
                    while (test.Read())
                    {
                        RptSale si = new RptSale();
                        si.StoreName = test["SortFi"].ToString();
                        si.Username = test["DocNo"].ToString(); 
                        si.Dmanager = test["DocDate"].ToString();
                        si.Fmanager = test["DueDate"].ToString();
                        si.DpName = test["AccCode"].ToString();
                        si.SupplierName = test["CustId"].ToString();
                        si.ItemName = test["TotAmt"].ToString();
                        si.StoreFranchise = test["UUID"].ToString();
                        si.StoreId = test["Status"].ToString();
                        vi.Add(si);
                    }
                    //var orderedVi = vi.OrderBy(sale => sale.StoreName).ToList();
                    ViewBag.Data = vi;
                    return View("DisplayUploadedInvoices");

                }
            }
        }
        // GET: Edit Item 
        [HttpGet]
        public IActionResult AInvEditItem(SalesParameters Parobj, string ItemCode)
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
            CkproUsersContext Ckpro = new CkproUsersContext();
            using (SqlConnection connection = new SqlConnection(Parobj.AxdbConnection))
            {
                using (SqlCommand command = new SqlCommand("SELECT [ItemCode],[Tax] FROM AXDBTest.dbo.[ITax] where Itemcode=@ItemCode", connection))
                {
                    connection.Open(); // Open the connection
                    command.Parameters.Add(new SqlParameter("@ItemCode", ItemCode));
                    double TotalSales = 0;
                    var vi = new List<RptSale>();
                    var test = command.ExecuteReader();
                    while (test.Read())
                    {
                        RptSale si = new RptSale();
                        si.StoreName = test["ItemCode"].ToString();
                        ViewBag.Branches = si.StoreName;
                        si.Username = test["Tax"].ToString();
                        ViewBag.Users = si.Username;
                        vi.Add(si);
                    }
                    //var orderedVi = vi.OrderBy(sale => sale.StoreName).ToList();
                    ViewBag.Data = vi;
                    return View();

                }
            }
        }
        public IActionResult DeleteItem(SalesParameters Parobj, string ItemCode)
        {
            using (SqlConnection connection = new SqlConnection(Parobj.AxdbConnection))
            {
                using (SqlCommand command = new SqlCommand("delete from AXDBTest.dbo.[ITax] where Itemcode=@ItemCode", connection))
                {
                    connection.Open(); // Open the connection
                    command.Parameters.Add(new SqlParameter("@ItemCode", ItemCode));
                    var test = command.ExecuteReader();
                    return RedirectToAction("AInvNewItem", "Accounts");
                }
            }
        }
        public IActionResult DeleteInvoice(SalesParameters Parobj, string Id)
        {
            using (SqlConnection connection = new SqlConnection(Parobj.EasySoftConnection))
            {
                using (SqlCommand command = new SqlCommand("delete  from Sales_Invoice where DocNo=@DocNo delete  from Sales_InvTrans where DocNo=@DocNo", connection))
                {
                    connection.Open(); // Open the connection
                    command.Parameters.Add(new SqlParameter("@DocNo", Id));
                    var test = command.ExecuteReader();
                    return RedirectToAction("DisplayUploadedInvoices", "Accounts");
                }
            }
        }
        [HttpPost]
        public IActionResult AInvEditItem(int itemId, SalesParameters Paroj, string ItemCode, decimal Tax)
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
                using (SqlConnection connection = new SqlConnection(Paroj.AxdbConnection))
                {
                    connection.Open(); // Open the connection

                    using (SqlCommand command = new SqlCommand("UPDATE AXDBTest.dbo.[ITax] SET ItemCode = @ItemCode, Tax = @Tax WHERE ItemCode = @ItemCode", connection))
                    {
                        command.Parameters.Add(new SqlParameter("@ItemCode", ItemCode));
                        command.Parameters.Add(new SqlParameter("@Tax", Tax));
                        command.ExecuteNonQuery(); // Execute the command
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("AInvNewItem", "Accounts");
        }
    }
}

