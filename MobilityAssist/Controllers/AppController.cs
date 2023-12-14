using MobilityAssist.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace MobilityAssist.Controllers
{
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
                var extrareq = db.GetExtra(Convert.ToInt16(Session["UserID"])).ToList();

                if (extrareq.Any())
                {
                    ViewData.Add("extrdata", extrareq);
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult RequestDashBoard()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Home");
            Request request = new Request();
            return View(request);
        }

        [HttpPost]
        public ActionResult RequestDashBoard(Request request)
        {
            if (ModelState.IsValid)
            {
                using (MobilityAssistEntities db = new MobilityAssistEntities())
                {
                    try
                    {
                        request.User = db.Users.Find(Convert.ToInt16(Session["UserID"]));
                        request.req_date = DateTime.Now;
                        db.Requests.Add(request);
                        db.SaveChanges();
                        return RedirectToAction("RequestListDashBoard", "App");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "Ой, щось пішло не так!\t" + ex;
                    }
                }

            }
            return View(request);
        }
        public ActionResult RequestListDashBoard()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Home");

            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var requestquery = db.GetRequests(Convert.ToInt16(Session["UserID"]));
                if (requestquery != null)
                {
                    ViewData.Add("requestquery", requestquery.ToList());
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult DeleteRequest(int? request_id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Home");
            if (request_id == null)
                return RedirectToAction("RequestListDashBoard", "App");

            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var request = db.GetRequests(Convert.ToInt16(Session["UserID"])).Where(req => req.request_id == request_id).First();
                return View(request);
            }
        }
        [HttpPost]
        public ActionResult DeleteRequest(int? request_id, FormCollection collection)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Home");
            if (request_id == null)
                return RedirectToAction("RequestListDashBoard", "App");

            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var request = (from req in db.Requests where req.request_id == request_id select req).First();
                try
                {
                    db.Requests.Remove(request);
                    db.SaveChanges();
                    return RedirectToAction("RequestListDashBoard");
                }
                catch (Exception)
                {
                    return View(request);
                }
            }
        }
        public ActionResult ExtraDashBoard()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Home");

            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                int id = Convert.ToInt16(Session["UserID"]);
                var extrausers = db.Users.Where(user => user.user_id == id).First().Users1;

                if (extrausers.Any())
                {
                    ViewData.Add("extrusers", extrausers.ToList());
                }
            }
            return View();
        }
        public ActionResult AddExtraUser(string email)
        {
            if (Request.HttpMethod != "POST")
                return RedirectToAction("UserDashBoard", "App");
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {

                var helpuser = db.Users.Where(user => user.email == email & user.Role.role_name == "support").FirstOrDefault();
                if (helpuser == null)
                {
                    TempData["Alert"] = "Немає помічника з такою поштою";
                    return RedirectToAction("ExtraDashBoard", "App");
                }

                int id = Convert.ToInt16(Session["UserID"]);
                var currentuser = db.Users.Where(user => user.user_id == id).First();
                if (currentuser.Users1.Contains(helpuser))
                {
                    TempData["Alert"] = "Вже є помічник з такою поштою";
                    return RedirectToAction("ExtraDashBoard", "App");

                }
                try
                {
                    currentuser.Users1.Add(helpuser);
                    db.SaveChanges();
                }
                catch
                {
                    TempData["Alert"] = "Ой, щось пішло не так";
                    return RedirectToAction("ExtraDashBoard", "App");
                }
                return RedirectToAction("ExtraDashBoard", "App");
            }
        }
        public ActionResult DeleteExtraUser(int extra_id)
        {
            if (Request.HttpMethod != "POST")
                return RedirectToAction("UserDashBoard", "App");
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                int id = Convert.ToInt16(Session["UserID"]);
                var currentuser = db.Users.Where(user => user.user_id == id).First();
                var helpuser = db.Users.Where(user => user.user_id == extra_id).First();
                try
                {
                    currentuser.Users1.Remove(helpuser);
                    db.SaveChanges();
                    return RedirectToAction("ExtraDashBoard", "App");
                }
                catch
                {
                    TempData["Alert"] = "Ой, щось пішло не так";
                    return RedirectToAction("ExtraDashBoard", "App");
                }
            }

        }
        public ActionResult MakeExtraRequests()
        {
            return View();
        }
        [HttpPost]
        public ActionResult MakeExtraRequests(Request request)
        {
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {                
                try
                {
                    request.User = db.Users.Find(Convert.ToInt16(Session["UserID"]));
                    request.req_date = DateTime.Now;
                    request.HType = db.HTypes.Where(help => help.help_name == "Екстрена").First();
                    db.Requests.Add(request);
                    db.SaveChanges();
                    return RedirectToAction("RequestListDashBoard", "App");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Ой, щось пішло не так!\t" + ex;
                }
            }
            return RedirectToAction("RequestListDashBoard");
        }
    }
}
