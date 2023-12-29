using MobilityAssist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace MobilityAssist.Controllers
{
    public class HomeController : Controller
    {
        MobilityAssistEntities db = new MobilityAssistEntities();
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
            var obj = db.Users.Where(a => a.email.Equals(objUser.email) && a.password.Equals(objUser.password));
            if (!obj.Any())
            {
                ViewBag.Message = "Credentials is not valid";
                return View();
            }
            Session["UserID"] = obj.First().user_id.ToString();
            Session["FirstName"] = obj.First().first_name.ToString();
            Session["Role"] = obj.First().user_role.ToString();
            return RedirectToAction("UserDashBoard", "App");

        }
        [HttpGet]
        public ActionResult Register()
        {
            var rolelist = db.Roles.Where(role => role.role_name != "admin").OrderBy(item => item.role_id).ToList();
            ViewData["rolelist"] = rolelist;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User objUser)
        {
            if (ModelState.IsValid)
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
                catch
                {
                    ViewBag.Message = "Credentials is not valid";
                }
            }
            var rolelist = db.Roles.OrderBy(item => item.role_id).ToList();
            ViewData["rolelist"] = rolelist;

            return View();
        }
        public ActionResult LogOut()
        {
            Session.Clear();
            return View("Login");
        }      
        
    }
    /// <summary>
    /// Attribute for Helper Role verification on view
    /// </summary>
    public class HelperCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var role = filterContext.HttpContext.Session["Role"];

            if (role == null || role.ToString() != "2")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        { "controller", "App" },
                        { "action", "UserDashBoard" }
                    }
                );
            }

            base.OnActionExecuting(filterContext);
        }
    }

    /// <summary>
    /// Attribute for Admin Role verification on view
    /// </summary>
    public class AdminCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var role = filterContext.HttpContext.Session["Role"];

            if (role == null || role.ToString() != "3")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        { "controller", "App" },
                        { "action", "UserDashBoard" }
                    }
                );
            }

            base.OnActionExecuting(filterContext);
        }
    }
    /// <summary>
    /// Attribute for Disabled Role verification on view
    /// </summary>
    public class DisabledCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var role = filterContext.HttpContext.Session["Role"];

            if (role == null || role.ToString() != "1")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        { "controller", "App" },
                        { "action", "UserDashBoard" }
                    }
                );
            }

            base.OnActionExecuting(filterContext);
        }
    }
}