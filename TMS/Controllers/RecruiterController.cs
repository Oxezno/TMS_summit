using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TMS.Models;

namespace TMS.Controllers
{
    public class RecruiterController : Controller
    {
        private TMSEntities db = new TMSEntities();
        // GET: Recruiter
        public ActionResult RecruiterList(string sortOrder, string searchString, int? page)
        {
            var rECRUITERS = db.Recruiters.OrderBy(s => s.Name);

            if (rECRUITERS == null)
            {
                return HttpNotFound();
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (!String.IsNullOrEmpty(searchString))
            {
                var rECRUITERSx = rECRUITERS.Where(s => s.Name.ToLower().Contains(searchString) || s.LastName.ToLower().Contains(searchString));
                return View(rECRUITERSx.ToPagedList(pageNumber, pageSize));
            }

            return View(rECRUITERS.ToPagedList(pageNumber, pageSize));
            //return View(company);
        }

        [HttpGet]
        public ActionResult CreateRecruiter(int Id)
        {
            ViewBag.ID_COMPANY = new SelectList(db.Companies.OrderBy(s => s.COMPANY_NAME), "ID_COMPANY", "COMPANY_NAME");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRecruiter([Bind(Include = "Name,LastName,Email,Phone,ID_COMPANY")] Recruiter rec)
        {
            if (ModelState.IsValid)
            {
                db.Recruiters.Add(rec);
                db.SaveChanges();

                return RedirectToAction("RecruiterList");
            }

            return View("RecruiterList");
        }

        [HttpGet]
        public ActionResult DeleteRecruiter(int id)
        {
            ViewBag.ID = id;

            return PartialView("_ConfirmDeleteRecruiter");
        }

        [HttpPost]
        public ActionResult ConfirmDeleteRecruiter(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Recruiter rec = db.Recruiters.Find(id);
            if (rec == null)
            {
                return HttpNotFound();
            }


            db.Recruiters.Remove(rec);
            db.SaveChanges();

            return RedirectToAction("RecruiterList");
        }

        public ActionResult EditRecruiter(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Recruiter rec = db.Recruiters.Find(id);
            if (rec == null)
            {
                return HttpNotFound();
            }

            ViewBag.ID_COMPANY = new SelectList(db.Companies.OrderBy(s => s.COMPANY_NAME), "ID_COMPANY", "COMPANY_NAME", rec.ID_COMPANY);


            return View(rec);
        }

        [HttpPost]
        public ActionResult EditRecruiter([Bind(Include = "ID,Name,LastName,Email,Phone,ID_COMPANY")] Recruiter recruiter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recruiter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("RecruiterList");
            }

            return View(recruiter);
        }

        [HttpPost]
        public ActionResult RecruiterSearch(FormCollection form)
        {
            string str = form["txtSearch"].ToString();
            return RedirectToAction("RecruiterList", "Recruiter", new { searchString = str });
        }
    }
}