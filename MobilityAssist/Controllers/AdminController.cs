using MobilityAssist.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MobilityAssist.Controllers
{
    [AdminCheck]
    public class AdminController : Controller
    {

        public ActionResult AdminDashBoard()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Home");

            return View();
        }

        public ActionResult AdminViewRequests()
        {
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var user_id = Convert.ToInt32(Session["UserID"]);

                ViewData["role"] = db.Users.Find(user_id).Role.role_name;
                ViewData["addresslist"] = db.GetAddresses().ToList();

                var query = from req in db.Requests select req;
                var viewreq = query
                    .Include(item => item.User)
                    .Include(item => item.Address)
                    .Include(item => item.Address.Street)
                    .Include(item => item.HType).ToList(); //all requests list with includes

                return View("~/Views/App/ViewPublicRequests.cshtml", viewreq);
            }

        }

        public ActionResult AdminSelectRequest(int? request_id)
        {
            if (Request.HttpMethod != "POST" || request_id == null)
                return RedirectToAction("UserDashBoard", "App");

            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                ViewData["addresslist"] = db.GetAddresses().ToList();
                var request = db.Requests
                    .Include(item => item.User)
                    .Include(item => item.Address)
                    .Include(item => item.Address.Street)
                    .Include(item => item.HType).First(req => req.request_id == request_id); //all requests list with includes
                return View(request);
            }
        }

        [HttpPost]
        public ActionResult AdminDeleteRequest(int? request_id)
        {
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                try
                {
                    var request = db.Requests.Find(request_id);
                    db.Requests.Remove(request);
                    db.SaveChanges();
                }
                catch
                {
                    return RedirectToAction("AdminViewRequests");
                }

                return RedirectToAction("AdminViewRequests");
            }
        }

        public ActionResult AdminViewAddresses()
        {
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var addresses = db.GetAddresses().ToList();
                return View(addresses);
            }
        }


        public ActionResult UpdateAddress(int? address_id)
        {
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var address = db.Addresses.Find(address_id);
                if (address != null)
                {
                    SelectList selectstreets = new SelectList(db.Streets.ToList(), "street_id", "street_name");
                    ViewData["streets"] = selectstreets;
                    return View(address);
                }
                return RedirectToAction("AdminViewAddresses");
            }
        }

        [HttpPost]
        public ActionResult UpdateAddress(Address new_address)
        {
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var address = db.Addresses.Find(new_address.address_id);
                if (address != null)
                {
                    // Update properties with modified values
                    address.address_numb = new_address.address_numb;
                    address.street_id = new_address.street_id;
                    address.is_adaptated = new_address.is_adaptated;
                    address.address_coordx = new_address.address_coordx;
                    address.address_coordy = new_address.address_coordy;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch
                    {
                        ViewBag.Message = "Вже є такі дані";
                        return View(address.street_id);
                    }
                }
                return RedirectToAction("AdminViewAddresses");
            }
        }


        public ActionResult DeletePageAddress(int? address_id)
        {
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var address = db.Addresses.Include(item => item.Street).Where(item => item.address_id == address_id).First();
                if (address != null)
                {
                    return View(address);
                }
                return RedirectToAction("AdminViewAddresses");
            }
        }

        [HttpPost]
        public ActionResult DeleteAddress(int? address_id)
        {
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var address = db.Addresses.Find(address_id);
                db.Addresses.Remove(address);
                db.SaveChanges();
                return RedirectToAction("AdminViewAddresses");
            }
        }

        public ActionResult CreateAddress()
        {
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                SelectList selectstreets = new SelectList(db.Streets.ToList(), "street_id", "street_name");
                ViewData["streets"] = selectstreets;
                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateAddress(Address address)
        {
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                db.Addresses.Add(address);
                db.SaveChanges();
                return RedirectToAction("AdminViewAddresses");
            }
        }
    }
}