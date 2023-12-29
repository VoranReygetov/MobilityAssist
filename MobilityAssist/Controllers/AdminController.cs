using MobilityAssist.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PagedList;

namespace MobilityAssist.Controllers
{
    [AdminCheck]
    public class AdminController : Controller
    {
        MobilityAssistEntities db = new MobilityAssistEntities();

        public ActionResult AdminDashBoard()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Home");

            return View();
        }

        public ActionResult AdminViewRequests(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.AddressSortParm = sortOrder == "Address" ? "address_desc" : "Address";
            ViewBag.HelpSortParm = sortOrder == "Help" ? "help_desc" : "Help";
            ViewBag.TimeSortParm = sortOrder == "Time" ? "time_desc" : "Time";
            ViewBag.StateSortParm = sortOrder == "State" ? "state_desc" : "State";

            if (searchString != null)       //If searchbar is not null -> first page
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            var query = from req in db.Requests select req;
            var viewreq = query
                .Include(item => item.User)
                .Include(item => item.Address)
                .Include(item => item.Address.Street)
                .Include(item => item.HType) as IEnumerable<Request>;       //all requests list with includes

            if (!String.IsNullOrEmpty(searchString))        //searching bar 
            {
                int.TryParse(searchString, out int id);
                viewreq = viewreq.Where(s => s.User.first_name.Contains(searchString)
                                       || s.request_id == id);
            }

            //page sorting switch
            switch (sortOrder)      
                {
                    case "id_desc":
                        viewreq = viewreq.OrderByDescending(s => s.request_id);
                        break;
                    case "Address":
                        viewreq = viewreq.OrderBy(s => s.Address.address_numb).OrderBy(s => s.Address.Street.street_name);
                        break;
                    case "address_desc":
                        viewreq = viewreq.OrderBy(s => s.Address.address_numb).OrderByDescending(s => s.Address.Street.street_name);
                        break;
                    case "Name":
                        viewreq = viewreq.OrderBy(s => s.User.first_name);
                        break;
                    case "name_desc":
                        viewreq = viewreq.OrderByDescending(s => s.User.first_name);
                        break;
                    case "Help":
                        viewreq = viewreq.OrderBy(s => s.HType.help_name);
                        break;
                    case "help_desc":
                        viewreq = viewreq.OrderByDescending(s => s.HType.help_name);
                        break;
                    case "Time":
                        viewreq = viewreq.OrderBy(s => s.req_date);
                        break;
                    case "time_desc":
                        viewreq = viewreq.OrderByDescending(s => s.req_date);
                        break;
                    case "State":
                        viewreq = viewreq.OrderByDescending(s => s.req_status);
                        break;
                    case "state_desc":
                        viewreq = viewreq.OrderBy(s => s.req_status);
                        break;
                    default:
                        viewreq = viewreq.OrderBy(s => s.request_id);
                        break;
                }

            int pageSize = 10;      //instances per page
            int pageNumber = (page ?? 1);
            return View(viewreq.ToPagedList(pageNumber, pageSize));


        }

        public ActionResult AdminSelectRequest(int? request_id)
        {
            if (Request.HttpMethod != "POST" || request_id == null)
                return RedirectToAction("UserDashBoard", "App");

            ViewData["addresslist"] = db.GetAddresses().ToList();
            var request = db.Requests
                .Include(item => item.User)
                .Include(item => item.Address)
                .Include(item => item.Address.Street)
                .Include(item => item.HType).First(req => req.request_id == request_id); //request with includes

            return View(request);
        }

        [HttpPost]
        public ActionResult AdminDeleteRequest(int? request_id)
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

        public ActionResult AdminViewAddresses(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.StreetSortParm = sortOrder == "Street" ? "street_desc" : "Street";
            ViewBag.AdaptedSortParm = sortOrder == "Adapted" ? "is_adapted_desc" : "Adapted";            

            if (searchString != null) //If searchbar is not null -> first page
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;
            var addresses = from address in db.GetAddresses() select address;

            if (!string.IsNullOrEmpty(searchString))
            {
                int.TryParse(searchString, out int id);
                addresses = addresses.Where(s => s.street.Contains(searchString)
                                       || s.address_id == id);
            }

            //page sorting switch
            switch (sortOrder)
            {
                case "id_desc":
                    addresses = addresses.OrderByDescending(s => s.address_id);
                    break;
                case "Street":
                    addresses = addresses.OrderBy(s => s.street);
                    break;
                case "street_desc":
                    addresses = addresses.OrderByDescending(s => s.street);
                    break;
                case "Adapted":
                    addresses = addresses.OrderByDescending(s => s.is_adaptated);
                    break;
                case "is_adapted_desc":
                    addresses = addresses.OrderBy(s => s.is_adaptated);
                    break;
                default:
                    addresses = addresses.OrderBy(s => s.address_id);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(addresses.ToList().ToPagedList(pageNumber, pageSize));


        }

        public ActionResult UpdateAddress(int? address_id)
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

        [HttpPost]
        public ActionResult UpdateAddress(Address new_address)
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


        public ActionResult DeletePageAddress(int? address_id)
        {
            var address = db.Addresses.Include(item => item.Street).Where(item => item.address_id == address_id).First();
            if (address != null)
            {
                return View(address);
            }
            return RedirectToAction("AdminViewAddresses");
        }

        [HttpPost]
        public ActionResult DeleteAddress(int? address_id)
        {
            var address = db.Addresses.Find(address_id);
            db.Addresses.Remove(address);
            db.SaveChanges();
            return RedirectToAction("AdminViewAddresses");
        }

        public ActionResult CreateAddress()
        {
            SelectList selectstreets = new SelectList(db.Streets.ToList(), "street_id", "street_name");
            ViewData["streets"] = selectstreets;
            return View();
        }

        [HttpPost]
        public ActionResult CreateAddress(Address address)
        {
            db.Addresses.Add(address);
            db.SaveChanges();
            return RedirectToAction("AdminViewAddresses");
        }
    }
}