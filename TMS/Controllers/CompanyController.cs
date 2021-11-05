using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMS.Models;
using PagedList;
using System.Data.Entity;
using System.IO;
using System.Net;

namespace TMS.Controllers
{
    public class CompanyController : Controller
    {
        private TMSEntities db = new TMSEntities();
        // GET: Company
        public ActionResult CompanyList(string sortOrder, string searchString,int? page)
        {
            //var company = db.Companies.FirstOrDefault();
            //var company = db.Companies.OrderBy(s => s.ID_COMPANY);
            var cOMPANY = db.Companies.OrderBy(s => s.ID_COMPANY);

            if (cOMPANY == null)
            {
                return HttpNotFound();
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (!String.IsNullOrEmpty(searchString))
            {
                var cOMPANYx = cOMPANY.Where(s => s.COMPANY_NAME.ToLower().Contains(searchString));
                return View(cOMPANYx.ToPagedList(pageNumber, pageSize));
            }

            return View(cOMPANY.ToPagedList(pageNumber, pageSize));
            //return View(company);
        }

        [HttpGet]
        public ActionResult CreateCompany(int Id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCompany([Bind(Include = "ID_COMPANY,COMPANY_NAME,PHONE1,EMAIL1,MAPS_URL,ADDRESS,CITY,PROFILE_IMG")] Company cOMPANY, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    // extract only the filename
                    var fileName = DateTime.Now.ToString("yyyyMMdd-THHmmss") + Path.GetFileName(file.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    cOMPANY.CAPTION = fileName;
                    cOMPANY.CONTENT_TYPE = file.ContentType;

                    var path = Path.Combine(Server.MapPath("~/Images/companyLogo"), fileName);
                    file.SaveAs(path);
                }

                db.Companies.Add(cOMPANY);
                db.SaveChanges();

                return RedirectToAction("CompanyList");
            }

            return View("CompanyList");
        }

        [HttpGet]
        public ActionResult DeleteCompany(int id)
        {
            ViewBag.ID = id;

            return PartialView("_ConfirmDeleteCompany");
        }

        [HttpPost]
        public ActionResult ConfirmDeleteCompany(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company cOMPANY = db.Companies.Find(id);
            if (cOMPANY == null)
            {
                return HttpNotFound();
            }

            string path = "~/Images/companyLogo/" + cOMPANY.CAPTION;

            db.Companies.Remove(cOMPANY);
            db.SaveChanges();

            try
            {
                // Determine whether the directory exists.
                //if (Directory.Exists(path))
                {
                    string mappedPath = Server.MapPath("~/Images/companyLogo/" + cOMPANY.CAPTION);
                    Directory.Delete(mappedPath, true);
                    Console.WriteLine("The directory was deleted successfully.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            finally { }

            return RedirectToAction("CompanyList");
        }


        public ActionResult EditCompany(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company cOMPANY = db.Companies.Find(id);
            if (cOMPANY == null)
            {
                return HttpNotFound();
            }

            return View(cOMPANY);
        }

        [HttpPost]
        public ActionResult EditCompany([Bind(Include = "ID_COMPANY,COMPANY_NAME,PHONE1,EMAIL1,MAPS_URL,ADDRESS,CITY,PROFILE_IMG")] Company cOMPANY, HttpPostedFileBase fileX)
        {
            if (ModelState.IsValid)
            {
                if (fileX != null && fileX.ContentLength > 0)
                {
                    // extract only the filename
                    var fileName = DateTime.Now.ToString("yyyyMMdd-THHmmss") + Path.GetFileName(fileX.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    cOMPANY.CAPTION = fileName;
                    cOMPANY.CONTENT_TYPE = fileX.ContentType;
                    var path = Path.Combine(Server.MapPath("~/Images/companyLogo"), fileName);
                    fileX.SaveAs(path);
                }

                db.Entry(cOMPANY).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CompanyList");
            }

            return View(cOMPANY);
        }

        [HttpPost]
        public ActionResult CompanySearch(FormCollection form)
        {
            string str = form["txtSearch"].ToString();
            return RedirectToAction("CompanyList", "Company", new { searchString = str });
        }

    }
}