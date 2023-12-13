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
            if (ModelState.IsValid)
            {
                using (MobilityAssistEntities db = new MobilityAssistEntities())
                {
                    var obj = db.Users.Where(a => a.email.Equals(objUser.email) && a.password.Equals(objUser.password)).FirstOrDefault();
                    if (obj != null)
                    {
                        //FormsAuthentication.SetAuthCookie(obj.email, false);
                        Session["UserID"] = obj.user_id.ToString();
                        Session["FirstName"] = obj.first_name.ToString();
                        Session["Role"] = obj.user_role.ToString();
                        return RedirectToAction("UserDashBoard", "App");
                    }
                }
                ViewBag.Message = "Credentials is not valid";
            }
            return View(objUser);
        }

        public List<SelectListItem> GetRoleItems()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                foreach (Role role in db.Roles)
                {
                    items.Add(new SelectListItem { Text = role.role_name, Value = role.role_id.ToString() });
                }
            }
            return items;
        }
        [HttpGet]
        public ActionResult Register()
        {
            User objUser = new User();
            ViewBag.RoleType = GetRoleItems();
            return View(objUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User objUser, string RoleType)
        {
            if (ModelState.IsValid)
            {
                using (MobilityAssistEntities db = new MobilityAssistEntities())
                {
                    try
                    {
                        objUser.Role = db.Roles.Find(int.Parse(RoleType));
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

            ViewBag.RoleType = GetRoleItems();
            return View(objUser);
        }

    }
}