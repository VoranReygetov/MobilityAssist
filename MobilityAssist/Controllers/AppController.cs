using MobilityAssist.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
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
                var extrareq = db.GetExtra(Convert.ToInt16(Session["UserID"]));

                if (extrareq != null)
                {
                    ViewData.Add("extrdata", extrareq.ToList());
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
                        return RedirectToAction("UserDashBoard", "App");
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
    }
}
