﻿using MobilityAssist.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace MobilityAssist.Controllers
{
    public class AppController : Controller
    {
        Random rnd = new Random();
        public ActionResult UserDashBoard()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Home");

            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                if (Session["Role"].Equals(db.Roles.First(role => role.role_name == "disabled").role_id.ToString())) //check for role
                    return RedirectToAction("DisabledDashBoard");
                if (Session["Role"].Equals(db.Roles.First(role => role.role_name == "admin").role_id.ToString()))
                    return RedirectToAction("AdminDashBoard", "Admin");
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
        [DisabledCheck]
        public ActionResult DisabledDashBoard()
        {
            int user_id = Convert.ToInt16(Session["UserID"]);
            ViewBag.Message = Greeting();
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var extrareq = db.Responces
                    .Include(resp => resp.User)
                    .Include(resp => resp.Address)
                    .Where(resp => resp.Request.user_id == user_id & resp.Request.req_status != true);

                Dictionary<int, double?> distance = new Dictionary<int, double?>();
                foreach (var resp in extrareq)
                {
                    distance[resp.res_id] = db.GetDistance(resp.address_id, resp.Request.address_id).First().distance;
                }

                if (extrareq.Any())
                {
                    ViewData.Add("distance", distance);
                    ViewData.Add("extrdata", extrareq.ToList());
                }
            }
            return View();
        }

        [HelperCheck]
        public ActionResult HelperDashBoard()
        {
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
        [DisabledCheck]
        public ActionResult RequestDashBoard()
        {
            Request request = new Request();
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                SelectList selectaddress = new SelectList(db.GetAddresses().ToList(), "address_id", "street");
                SelectList selecthelp = new SelectList(db.HTypes.ToList(), "help_id", "help_name");

                ViewData["helplist"] = selecthelp;
                ViewData["addresslist"] = selectaddress;
            }
            return View(request);
        }

        [HttpPost]
        [DisabledCheck]
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

        [DisabledCheck]
        public ActionResult RequestListDashBoard()
        {

            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var requestquery = db.GetRequests(Convert.ToInt16(Session["UserID"]));
                if (requestquery != null)
                {
                    ViewData["requestquery"] =  requestquery.ToList();
                }
            }
            return View();
        }

        [HttpGet]
        [DisabledCheck]
        public ActionResult DeleteRequest(int? request_id)
        {
            if (request_id == null)
                return RedirectToAction("RequestListDashBoard", "App");

            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var request = db.GetRequests(Convert.ToInt16(Session["UserID"])).Where(req => req.request_id == request_id).First();
                return View(request);
            }
        }

        [HttpPost]
        [DisabledCheck]
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

        [DisabledCheck]
        public ActionResult ExtraDashBoard()
        {
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

        [DisabledCheck]
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

        [DisabledCheck]
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

        [DisabledCheck]
        public ActionResult MakeExtraRequests()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Home");

            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                SelectList selectaddress = new SelectList(db.GetAddresses().ToList(), "address_id", "street");
                ViewData["addresslist"] = selectaddress;
            }
            return View();
        }


        [HttpPost]
        [DisabledCheck]
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


        public ActionResult MakeRouteDashBoard()
        {
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                SelectList selectaddress = new SelectList(db.GetAddresses().ToList(), "address_id", "street");
                ViewData["addresslist"] = selectaddress;
            }
            return View();
        }

        [HttpPost]
        public ActionResult MakeRouteResult(FormCollection collection)
        {
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var route = db.GetDistance(int.Parse(collection["address_id"]), int.Parse(collection["destination_id"])).First();
                return View(route);
            }
        }

        [HelperCheck]
        public ActionResult ViewPublicRequests()
        {

            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                var user_id = Convert.ToInt32(Session["UserID"]);
                if (db.Responces.Where(responce => responce.user_id == user_id)
                    .Where(responce => responce.Request.req_status == false).Any())
                {
                    return RedirectToAction("GetRouteToRequest");
                }
                var query = from req in db.Requests
                            where !db.Responces.Any(res => res.req_id == req.request_id) &&
                                  req.req_status == false &&
                                  req.help_id != (from h in db.HTypes
                                                  where h.help_name == "Екстрена"
                                                  select h.help_id).FirstOrDefault()
                            select req;     //Not answered, incompleted and Extra requests query;

                ViewData["addresslist"] = db.GetAddresses().ToList();
                var viewreq = query.Include(item => item.User)
                    .Include(item => item.Address)
                    .Include(item => item.Address.Street)
                    .Include(item => item.HType).ToList();      //requests list with includes
                return View("ViewPublicRequests", viewreq);
            }
        }

        [HelperCheck]
        public ActionResult AnswerPublicRequest(int? request_id)
        {
            if (Request.HttpMethod != "POST")
                return RedirectToAction("UserDashBoard", "App");

            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                ViewData["addresslist"] = db.GetAddresses().ToList();
                var request = db.Requests.Include(item => item.User)
                    .Include(item => item.Address)
                    .Include(item => item.Address.Street)
                    .Include(item => item.HType).First(req => req.request_id == request_id);
                return View(request);
            }
        }

        [HttpPost]
        [HelperCheck]
        public ActionResult GetRouteToRequest(FormCollection collection)
        {
            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                try
                {
                    int req_id = int.Parse(collection["request_id"]);
                    int address_id = int.Parse(collection["address"]);
                    var user_id = Convert.ToInt32(Session["UserID"]);
                    var request = db.Requests.Include(item => item.User).First(item => item.request_id == req_id);
                    Responce response = new Responce
                    {
                        res_date = DateTime.Now,
                        req_id = req_id,
                        Address = db.Addresses.Find(address_id),
                        User = db.Users.Find(user_id),
                        res_comm = collection["resp_desc"]
                    };
                    db.Responces.Add(response);
                    db.SaveChanges();

                    ViewData["distance"] = db.GetDistance(int.Parse(collection["address"]), request.address_id).First();
                    ViewData["help"] = request.HType.help_name;

                    return View(request);
                }
                catch
                {
                    ViewBag.Message = "Ой, щось пішло не так!";
                    return RedirectToAction("AnswerPublicRequest", collection["collection"].First());
                }
            }
        }

        [HelperCheck]
        public ActionResult GetRouteToRequest()
        {

            using (MobilityAssistEntities db = new MobilityAssistEntities())
            {
                try
                {
                    var user_id = Convert.ToInt32(Session["UserID"]);
                    var responce = db.Responces
                        .Include(item => item.Request.User)
                        .Where(resp => resp.User.user_id == user_id)
                        .Where(resp => resp.Request.req_status == false).First();       //Responce element from User

                    ViewData["distance"] = db.GetDistance(responce.address_id, responce.Request.address_id).First();
                    ViewData["help"] = responce.Request.HType.help_name.ToString();

                    return View(responce.Request);
                }
                catch
                {
                    var user_id = Convert.ToInt32(Session["UserID"]);
                    ViewBag.Message = "Ой, щось пішло не так!";
                    return RedirectToAction("AnswerPublicRequest", db.Responces.Where(resp => resp.User.user_id == db.Users.Find(user_id).user_id).First().req_id);
                }
            }
        }

        public ActionResult MarkRequestCompleted(int request_id)
        {
            if (Request.HttpMethod != "POST")
                return RedirectToAction("GetRouteToRequest", "App");

            using (var db = new MobilityAssistEntities())
            {
                var requestToUpdate = db.Requests.First(r => r.request_id == request_id);
                if (requestToUpdate != null)
                {
                    requestToUpdate.req_status = true;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("ViewPublicRequests");
        }       

    }
    
}