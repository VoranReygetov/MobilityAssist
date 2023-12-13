using MobilityAssist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

            int choose = rnd.Next(0, hello.Count() - 1);
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
            return View();
        }
    }
}