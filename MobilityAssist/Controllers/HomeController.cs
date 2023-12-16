using MobilityAssist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MobilityAssist.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Про що цей застосунок";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "You can contact me here.";

            return View();
        }
        public ActionResult Login()
        {            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User objUser)
        {
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var obj = db.Users.Where(a => a.email.Equals(objUser.email) && a.password.Equals(objUser.password));
                if(!obj.Any())
                {
                    ViewBag.Message = "Credentials is not valid";
                    return View();
                }
                Session["UserID"] = obj.First().user_id.ToString();
                Session["FirstName"] = obj.First().first_name.ToString();
                Session["Role"] = obj.First().user_role.ToString();
                return RedirectToAction("UserDashBoard", "App");
            }
            
        }
        [HttpGet]
        public ActionResult Register()
        {
            User objUser = new User();
            return View(objUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User objUser)
        {
            if (ModelState.IsValid)
            {
                using (MobilityAssistEntities db = new MobilityAssistEntities())
                {
                    try
                    {
                        db.Users.Add(objUser);
                        db.SaveChanges();
                        Session["UserID"] = objUser.user_id.ToString();
                        Session["FirstName"] = objUser.first_name.ToString();
                        Session["Role"] = objUser.user_role.ToString();
                        return RedirectToAction("UserDashBoard", "App");
                    }
                    catch (Exception)
                    {
                        ViewBag.Message = "Credentials is not valid";
                    }
                }
                
            }
            return View(objUser);
        }
        public ActionResult LogOut()
        {
            Session.Clear();
            return View("Login");
        }
    }
}