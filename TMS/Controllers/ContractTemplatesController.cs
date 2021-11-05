using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Drawing;   
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using TMS.Models;
using Rotativa.MVC;

namespace TMS.Controllers
{
    public class ContractTemplatesController : Controller
    {
        private TMSEntities db = new TMSEntities();

        // GET: TrainingCours
        public ActionResult ContractTemplateList(string sortOrder, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;

            List<ContractTemplate> ct = db.ContractTemplates.ToList();

            if (ct == null)
            {
                return HttpNotFound();
            }


            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "name" ? "name" : "name_desc";
            ViewBag.ActiveSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "active" ? "active" : "active_desc";
            ViewBag.CompanySortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "company" ? "company" : "company_desc";

            switch (sortOrder)
            {
                case "name":
                    ct = ct.OrderBy(x => x.TemplateName).ToList();
                    break;

                case "name_desc":
                    ct = ct.OrderByDescending(x => x.TemplateName).ToList();
                    break;

                case "active":
                    ct = ct.OrderBy(x => x.IsActive).ToList();
                    break;

                case "active_desc":
                    ct = ct.OrderByDescending(x => x.IsActive).ToList();
                    break;

                case "company":
                    ct = ct.OrderBy(x => x.Company.COMPANY_NAME).ToList();
                    break;

                case "company_desc":
                    ct = ct.OrderByDescending(x => x.Company.COMPANY_NAME).ToList();
                    break;

                default:
                    ct = ct.OrderByDescending(x => x.ContractTemplateID).ToList();
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                var ctX = ct.Where(s => s.TemplateName.ToLower().Contains(searchString.ToLower()));
                return View(ctX.ToPagedList(pageNumber, pageSize));
            }

            return View(ct.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult ContractTemplateSearch(FormCollection form)
        {
            string str = form["txtSearch"].ToString();
            return RedirectToAction("ContractTemplateList", "ContractTemplates", new { searchString = str });
        }

        // GET: TrainingCours/Create
        public ActionResult CreateContractTemplate()
        {
            ViewBag.ID_COMPANY = new SelectList(db.Companies, "ID_COMPANY", "COMPANY_NAME");
            return View();
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult SaveContractTemplate([Bind(Include = "ContractTemplateID,TemplateName,TemplateContent,IsActive,CompanyID")] ContractTemplate template)
        {
            if (ModelState.IsValid)
            {
                if (template.TemplateContent != null)
                    template.TemplateContentBlob = Encoding.ASCII.GetBytes(template.TemplateContent);

                template.TemplateContent = "";
                template.DateRecord = DateTime.Now;
                template.DateUpdate = DateTime.Now;
                template.CreatedBy = Convert.ToInt32(Session["LoginId"]);
                template.UpdatedBy = Convert.ToInt32(Session["LoginId"]);
                db.ContractTemplates.Add(template);
                db.SaveChanges();

                return RedirectToAction("ContractTemplateList");
            }

            ViewBag.ID_COMPANY = new SelectList(db.Companies, "ID_COMPANY", "COMPANY_NAME");
            return View(template);
        }

        // GET: TrainingCours/Edit/5
        public ActionResult EditContractTemplate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractTemplate template = db.ContractTemplates.Find(id);
            if (template == null)
            {
                return HttpNotFound();
            }

            ViewBag.ID_COMPANY = new SelectList(db.Companies, "ID_COMPANY", "COMPANY_NAME", template.CompanyID);

            if (template.TemplateContent != null)
                template.TemplateContent = Encoding.ASCII.GetString(template.TemplateContentBlob);

            TempData["updateMessage"] = TempData["updateStatus"];

            return View(template);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult UpdateContractTemplate([Bind(Include = "ContractTemplateID,TemplateName,TemplateContent,IsActive,CompanyID")] ContractTemplate template)
        {
            if (ModelState.IsValid)
            {
                if (template.TemplateContent != null)
                    template.TemplateContentBlob = Encoding.ASCII.GetBytes(template.TemplateContent);
                else
                    template.TemplateContentBlob = Encoding.ASCII.GetBytes("");

                template.TemplateContent = "";
                template.DateUpdate = DateTime.Now;
                template.UpdatedBy = Convert.ToInt32(Session["LoginId"]);
                db.Entry(template).State = EntityState.Modified;
                db.Entry(template).Property(x => x.DateRecord).IsModified = false;
                db.Entry(template).Property(x => x.CreatedBy).IsModified = false;
                
                db.SaveChanges();

                TempData["updateStatus"] = "OK";

                return RedirectToAction("EditContractTemplate", new { id = template.ContractTemplateID});
            }

            ViewBag.ID_COMPANY = new SelectList(db.Companies, "ID_COMPANY", "COMPANY_NAME", template.CompanyID);

            TempData["updateStatus"] = "Error";
            return RedirectToAction("EditContractTemplate", new { id = template.ContractTemplateID});
            //return View(trainingCours);
        }

        public ActionResult PrintViewToPdf(int id, int userID)
        {
            var template = db.ContractTemplates.Find(id);

            return new ActionAsPdf("PDFPreview", new { idx = id, userIDx = userID }) { FileName = template.TemplateName+ ".pdf" };
        }

        public ViewResult PDFPreview(int idx, int userIDx)
        {
            var template = db.ContractTemplates.Find(idx);

            if (template == null)
            {
                return null;
            }


            template.TemplateContent = Encoding.ASCII.GetString(template.TemplateContentBlob);

            if (userIDx != 0)
            {
                var user = db.WebsiteUsers.Find(userIDx);
                template.TemplateContent = template.TemplateContent.Replace("{Date}", DateTime.Now.ToString("MM/dd/yyyy"));
                template.TemplateContent = template.TemplateContent.Replace("{CandidateName}", user.FName+" "+user.LName);
            }

            return View(template);
        }

        public ActionResult CloneContractTemplate(int? id)
        {
            ViewBag.ID = id;

            return PartialView("_ConfirmCloneContract");
        }

        // POST: TrainingCours/Delete/5
        [HttpPost]
        public ActionResult ConfirmCloneContract(int id)
        {
            ContractTemplate template = db.ContractTemplates.Find(id);

            template.TemplateName = template.TemplateName + " (copy " + DateTime.Now.ToString("yyy-MM-dd hh:mm:ss") + ")";
            template.DateRecord = DateTime.Now;
            template.DateUpdate = DateTime.Now;
            template.CreatedBy = Convert.ToInt32(Session["LoginId"]);
            template.UpdatedBy = Convert.ToInt32(Session["LoginId"]);

            db.ContractTemplates.Add(template);
            db.SaveChanges();

            return RedirectToAction("ContractTemplateList");
        }

        // GET: TrainingCours/Create
        public ActionResult SendContract()
        {
            ViewBag.TrainingPrograms = new SelectList(db.GetTrainingCourses(), "TrainingCourseID", "TrainingCourseDate");
            ViewBag.ContractTemplate = new SelectList(db.ContractTemplates, "ContractTemplateID", "TemplateName");

            return View();
        }

        public ActionResult GetUsersList(int? tpID, int ctID)
        {
            
            if (tpID == null)
            {
                return HttpNotFound();
            }

            List<WebsiteUserTraining> wu = db.WebsiteUserTrainings.Where(x => x.TrainingCourseID == tpID && x.WebsiteUser.EvaluationStatus == true).OrderBy(x => x.WebsiteUser.FName).ToList();
            wu = wu.GroupBy(x => x.WebsiteUserID).Select(g => g.First()).OrderBy(x => x.WebsiteUser.FName).ToList();

            ViewBag.wuCount = wu.Count();
            ViewBag.TEMPLATE_ID = ctID;

            return PartialView(wu);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

