using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TMS.Models;

namespace TMS.Controllers
{
    public class WebsiteDashboardController : Controller
    {
        private TMSEntities db = new TMSEntities();

        // GET: WebsiteDashboard
        public ActionResult Index(string sortOrderTC, string searchStringTC, string sortOrderU, string searchStringU)
        {
            List <GetTrainingCourses_Result> tc = db.GetTrainingCourses().ToList();
            @ViewBag.TotalUsers = db.WebsiteUsers.Where(x => x.ConfirmedEmail == true).OrderByDescending(x => x.WebsiteUserID).Count();
            @ViewBag.RegisteredToday = db.WebsiteUsers.Where(x => x.IsActive == true && x.RegisterDate >= DateTime.Today && x.RegisterDate <= DateTime.Now).OrderByDescending(x => x.WebsiteUserID).Count();
            @ViewBag.EnrolledToday = db.WebsiteUserTrainings.Where(x => x.PaymentDate >= DateTime.Today && x.PaymentDate <= DateTime.Now).Count();

            //var tc2 = tc.Where(x => x.IsActive == true).Take(10);
            var tc2 = tc.Take(10);

            ViewBag.tcCount = tc.Count;

            if (tc2 == null)
            {
                return HttpNotFound();
            }


            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrderTC) || sortOrderTC != "name" ? "name" : "name_desc";
            ViewBag.StartDate1SortParm = String.IsNullOrEmpty(sortOrderTC) || sortOrderTC != "startdate1" ? "startdate1" : "startdate1_desc";
            //ViewBag.StartDate2SortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "startdate2" ? "startdate2" : "startdate2_desc";
            ViewBag.Payment1SortParm = String.IsNullOrEmpty(sortOrderTC) || sortOrderTC != "payment1" ? "payment1" : "payment1_desc";
            ViewBag.Payment2SortParm = String.IsNullOrEmpty(sortOrderTC) || sortOrderTC != "payment2" ? "payment2" : "payment2_desc";
            ViewBag.TotalSortParm = String.IsNullOrEmpty(sortOrderTC) || sortOrderTC != "total" ? "total" : "total_desc";
            ViewBag.EnrolledSortParm = String.IsNullOrEmpty(sortOrderTC) || sortOrderTC != "enrolled" ? "enrolled" : "enrolled_desc";

            switch (sortOrderTC)
            {
                case "name":
                    tc2 = tc2.OrderBy(x => x.TrainingCourseName).ToList();
                    break;

                case "name_desc":
                    tc2 = tc2.OrderByDescending(x => x.TrainingCourseName).ToList();
                    break;

                case "startdate1":
                    tc2 = tc2.OrderBy(x => x.StartDate1).ToList();
                    break;

                case "startdate1_desc":
                    tc2 = tc2.OrderByDescending(x => x.StartDate1).ToList();
                    break;

                case "payment1":
                    tc2 = tc2.OrderBy(x => x.FirstPayment).ToList();
                    break;

                case "payment1_desc":
                    tc2 = tc2.OrderByDescending(x => x.FirstPayment).ToList();
                    break;

                case "payment2":
                    tc2 = tc2.OrderBy(x => x.SecondPayment).ToList();
                    break;

                case "payment2_desc":
                    tc2 = tc2.OrderByDescending(x => x.SecondPayment).ToList();
                    break;

                case "total":
                    tc2 = tc2.OrderBy(x => x.Price).ToList();
                    break;

                case "total_desc":
                    tc2 = tc2.OrderByDescending(x => x.Price).ToList();
                    break;

                case "enrolled":
                    tc2 = tc2.OrderBy(x => x.TotalEnrolled).ToList();
                    break;

                case "enrolled_desc":
                    tc2 = tc2.OrderByDescending(x => x.TotalEnrolled).ToList();
                    break;

                default:
                    tc2 = tc2.OrderByDescending(x => x.TrainingCourseID).ToList();
                    break;
            }

            ViewBag.CurrentSort = sortOrderU;
            var wu = db.WebsiteUsers.OrderByDescending(x => x.WebsiteUserID).Take(10).ToList();

            if (wu == null)
            {
                return HttpNotFound();
            }

            var w = searchStringU;

            ViewBag.NameUSortParm = String.IsNullOrEmpty(sortOrderU) || sortOrderU != "nameU" ? "nameU" : "nameU_desc";
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrderU) || sortOrderU != "lastname" ? "lastname" : "lastname_desc";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrderU) || sortOrderU != "email" ? "email" : "email_desc";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrderU) || sortOrderU != "date" ? "date" : "date_desc";
            ViewBag.ActiveSortParm = String.IsNullOrEmpty(sortOrderU) || sortOrderU != "active" ? "active" : "active_desc";

            switch (sortOrderU)
            {
                case "nameU":
                    wu = wu.OrderBy(x => x.FName).ToList();
                    break;

                case "nameU_desc":
                    wu = wu.OrderByDescending(x => x.FName).ToList();
                    break;

                case "lastname":
                    wu = wu.OrderBy(x => x.LName).ToList();
                    break;

                case "lastname_desc":
                    wu = wu.OrderByDescending(x => x.LName).ToList();
                    break;

                case "email":
                    wu = wu.OrderBy(x => x.Email).ToList();
                    break;

                case "email_desc":
                    wu = wu.OrderByDescending(x => x.Email).ToList();
                    break;

                case "date":
                    wu = wu.OrderBy(x => x.RegisterDate).ToList();
                    break;

                case "date_desc":
                    wu = wu.OrderByDescending(x => x.RegisterDate).ToList();
                    break;

                case "active":
                    wu = wu.OrderBy(x => x.IsActive).ToList();
                    break;

                case "active_desc":
                    wu = wu.OrderByDescending(x => x.IsActive).ToList();
                    break;

                default:
                    wu = wu.OrderByDescending(x => x.WebsiteUserID).ToList();
                    break;
            }

            

            ViewBag.Count = wu.Count;


            dynamic MultiModel = new ExpandoObject();

            if (!String.IsNullOrEmpty(searchStringTC))
            {
                //var tcX = tc2.Where(s => s.TrainingCourseName.ToLower().Contains(searchStringTC)).ToList();
                var tcX = db.GetTrainingCoursesByName(searchStringTC).ToList();
                MultiModel.TrainingCourses = tcX;
                //MultiModel.WebsiteUsers = webUsers;

                ViewBag.TotalTC = tc2.Count();
                //ViewBag.TotalUsers = webUsers.Count();

                TempData["tt"] = tcX.Count();
                //return View(MultiModel);
            }

            else
            {
                TempData["tt"] = tc2.Count();
                //MultiModel.WebsiteUsers = webUsers;
                MultiModel.TrainingCourses = tc2;

                ViewBag.TotalTC = tc2.Count();
                //ViewBag.TotalUsers = webUsers.Count();

            }

            if (!String.IsNullOrEmpty(searchStringU))
            {
                //var wuX = wu.Where(s => s.FName.ToLower().Contains(searchString)).ToList();
                var wuX = db.WebsiteUsers.Where(x => x.FName.ToLower().Contains(searchStringU) || x.LName.ToLower().Contains(searchStringU) || x.Email.Contains(searchStringU) || x.Phone.Contains(searchStringU)).OrderByDescending(x => x.WebsiteUserID).ToList();
                ViewBag.Count = wuX.Count;

                MultiModel.WebsiteUsers = wuX;

                //return View(wuX.ToPagedList(pageNumberU, pageSizeU));
            }

            else
            {
                MultiModel.WebsiteUsers = wu;
            }



            return View(MultiModel);
        }

        [HttpPost]
        public ActionResult TrainingCoursesSearch(FormCollection form)
        {
            string str = form["txtSearch"].ToString();
            return RedirectToAction("Index", "WebsiteDashboard", new { searchStringTC = str });
        }

        [HttpPost]
        public ActionResult WebsiteUsersSearch(FormCollection form)
        {
            string str = form["txtSearch"].ToString();
            return RedirectToAction("Index", "WebsiteDashboard", new { searchStringU = str });
        }

        public ActionResult EnrolledUsersChart()
        {
            var data = db.GetTrainingCourses().OrderByDescending(x => x.TotalEnrolled).Take(5).ToList();

            //Create bar chart
            var chart = new Chart(width: 950, height: 500)
                .AddSeries("Default",
                                xValue: data, xField: "TrainingCourseDate",
                                yValues: data, yFields: "TotalEnrolled")
                            .GetBytes("png");

            return File(chart, "image/bytes");
        }
    }
}