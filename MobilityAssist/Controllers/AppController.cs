using MobilityAssist.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MobilityAssist.Controllers
{
    //[Authorize]
    public class AppController : Controller
    {
        // GET: App
        Random rnd = new Random();        
        public ActionResult UserDashBoard()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Home");            

            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                if (Session["Role"].Equals(db.Roles.First(role => role.role_name == "disabled").role_id.ToString())) //check for role
                    return RedirectToAction("DisabledDashBoard");
                return RedirectToAction("HelperDashBoard");
            }            
        }
        public string Greeting()
        {
            string firstName = Session["FirstName"].ToString();
            List<string> hello = new List<string> {
                $"Вітаємо, {firstName}. З чим можемо вам допомогти?",
                $"Вітаю, {firstName}. Які пригоди нас чекатимуть сьогодні?",
                $"Мабуть, маєте великі плани сьогодні, {firstName}? Потрібно все гарно спланувати"
            };

            int choose = rnd.Next(0, hello.Count());
            return hello[choose];
        }
        public ActionResult DisabledDashBoard()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Home");
            ViewBag.Message = Greeting();
            return View();
        }
        public ActionResult HelperDashBoard()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Home");
            ViewBag.Message = Greeting();
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var extrareq = db.GetExtra(Convert.ToInt16(Session["UserID"]));

                if (extrareq != null)
                {
                    ViewData.Add("extrdata", extrareq.ToList());
                }
            }
            return View();
        }
        public List<SelectListItem> GetAddressItems()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var addresses = db.GetAddresses();
                foreach (GetAddresses_Result address in addresses)
                {
                    items.Add(new SelectListItem { Text = $"{address.street_name}, {address.address_numb}", Value = address.address_id.ToString() });
                }
            }
            return items;
        }
        public List<SelectListItem> GetHelpItems()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                foreach (HType help in db.HTypes)
                {
                    items.Add(new SelectListItem { Text = help.help_name, Value = help.help_id.ToString() });
                }
            }
            return items;
        }
        [HttpGet]
        public ActionResult RequestDashBoard()
        {
            Request request = new Request();
            ViewBag.AddressType = GetAddressItems();
            ViewBag.HelpType = GetHelpItems();
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestDashBoard(Request request, string HelpType, string AddressType, string Destinationype)
        {
            if (ModelState.IsValid)
            {
                using (MobilityAssistEntities db = new MobilityAssistEntities())
                {
                    try
                    {
                        request.User = db.Users.Find(Convert.ToInt16(Session["UserID"]));
                        request.Address = db.Addresses.Find(int.Parse(AddressType));
                        request.Address1 = db.Addresses.Find(int.Parse(AddressType));
                        request.HType = db.HTypes.Find(int.Parse(HelpType));
                        request.req_date = DateTime.Now;
                        db.Requests.Add(request);
                        db.SaveChanges();
                        return RedirectToAction("UserDashBoard", "App");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "Ой, щось пішло не так!\n"+ ex;
                    }
                }

            }

            ViewBag.AddressType = GetAddressItems();
            ViewBag.HelpType = GetHelpItems();
            return View(request);
        }
    }
}