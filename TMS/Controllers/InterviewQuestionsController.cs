using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TMS.Models;

namespace TMS.Controllers
{
    public class InterviewQuestionsController : Controller
    {
        private TMSEntities db = new TMSEntities();

        // GET: InterviewQuestions
        public ActionResult Index(string sortOrder, string searchStringKeyword, string Candidate, string Company, string Technology, string From, string To, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SearchKeyword = searchStringKeyword;
            ViewBag.CandidateN = Candidate;
            ViewBag.Company = Company;
            ViewBag.Technology = Technology;
            ViewBag.From = From;
            ViewBag.To = To;

            List<InterviewQuestion> iq = db.InterviewQuestions.ToList();

            if (iq == null)
            {
                return HttpNotFound();
            }

            for (int i = 0; i < iq.Count; i++)
            {
                if(iq[i].DescriptionBlob != null)
                {
                    iq[i].Description = Encoding.ASCII.GetString(iq[i].DescriptionBlob);
                }
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "name" ? "name" : "name_desc";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "email" ? "email" : "email_desc";
            ViewBag.CompanySortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "company" ? "company" : "company_desc";
            ViewBag.InterviewDateSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "interviewdate" ? "interviewdate" : "interviewdate_desc";
            ViewBag.TechnologySortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "technology" ? "technology" : "technology_desc";
            ViewBag.DateCreatedSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "datecreated" ? "datecreated" : "datecreated_desc";
            ViewBag.ActiveSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "description" ? "description" : "description_desc";

            switch (sortOrder)
            {
                case "name":
                    iq = iq.OrderBy(x => x.CreatedBy).ToList();
                    break;

                case "name_desc":
                    iq = iq.OrderByDescending(x => x.CreatedBy).ToList();
                    break;

                case "email":
                    iq = iq.OrderBy(x => x.Email).ToList();
                    break;

                case "email_desc":
                    iq = iq.OrderByDescending(x => x.Email).ToList();
                    break;

                case "company":
                    iq = iq.OrderBy(x => x.CompanyName).ToList();
                    break;

                case "company_desc":
                    iq = iq.OrderByDescending(x => x.CompanyName).ToList();
                    break;

                case "interviewdate":
                    iq = iq.OrderBy(x => x.InterviewDate).ToList();
                    break;

                case "interviewdate_desc":
                    iq = iq.OrderByDescending(x => x.InterviewDate).ToList();
                    break;

                case "technology":
                    iq = iq.OrderBy(x => x.Technology).ToList();
                    break;

                case "technology_desc":
                    iq = iq.OrderByDescending(x => x.Technology).ToList();
                    break;

                case "datecreated":
                    iq = iq.OrderBy(x => x.CreatedOn).ToList();
                    break;

                case "datecreated_desc":
                    iq = iq.OrderByDescending(x => x.CreatedOn).ToList();
                    break;

                case "description":
                    iq = iq.OrderBy(x => x.Description).ToList();
                    break;

                case "description_desc":
                    iq = iq.OrderByDescending(x => x.Description).ToList();
                    break;

                default:
                    iq = iq.OrderByDescending(x => x.InterviewQuestionID).ToList();
                    break;
            }

            var tech = db.TrainingCourses.ToList();

            var techX = tech.Select(x => new
            {
                TrainingCourseID = x.TrainingCourseID,
                TrainingCourseName = x.TrainingCourseName
            }).GroupBy(x => x.TrainingCourseName).Select(g => g.First()).OrderBy(x => x.TrainingCourseName).ToList();

            ViewBag.TECHNOLOGIES = new SelectList(techX, "TrainingCourseID", "TrainingCourseName");

            var comp = db.InterviewQuestions.ToList();

            var compX = comp.Select(x => new
            {
                CompanyName = x.CompanyName
            }).GroupBy(x => x.CompanyName).Select(g => g.First()).OrderBy(x => x.CompanyName).ToList();

            ViewBag.COMPANIES = new SelectList(compX, "CompanyName", "CompanyName");

            var usersX = comp.Select(x => new
            {
                CreatedBy = x.CreatedBy
            }).GroupBy(x => x.CreatedBy).Select(g => g.First()).OrderBy(x => x.CreatedBy).ToList();

            ViewBag.CANDIDATE = new SelectList(usersX, "CreatedBy", "CreatedBy");

            if (!String.IsNullOrEmpty(searchStringKeyword) || !String.IsNullOrEmpty(Candidate) || !String.IsNullOrEmpty(Company) || !String.IsNullOrEmpty(Technology) || !String.IsNullOrEmpty(From) || !String.IsNullOrEmpty(To))
            {
                List<InterviewQuestion> iqX = iq;

                if (!String.IsNullOrEmpty(searchStringKeyword))
                {
                    iqX = iqX.Where(s => (s.Description.ToLower().Contains(searchStringKeyword.ToLower()))).ToList();
                }

                if (!String.IsNullOrEmpty(Candidate))
                {
                    iqX = iqX.Where(s => (s.CreatedBy.ToLower().Contains(Candidate.ToLower()))).ToList();
                }

                if (!String.IsNullOrEmpty(Company))
                {
                    iqX = iqX.Where(s => (s.CompanyName.ToLower().Contains(Company.ToLower()))).ToList();
                }

                if (!String.IsNullOrEmpty(Technology))
                {
                    var TrainingCoursX = db.TrainingCourses.Find(Convert.ToInt32(Technology));

                    iqX = iqX.Where(s => (s.Technology.ToLower().Contains(TrainingCoursX.TrainingCourseName.ToLower()))).ToList();
                }

                if (!String.IsNullOrEmpty(From))
                {
                    iqX = iqX.Where(s => (s.InterviewDate >= Convert.ToDateTime(From))).ToList();
                }

                if (!String.IsNullOrEmpty(To))
                {
                    iqX = iqX.Where(s => (s.InterviewDate <= Convert.ToDateTime(To))).ToList();
                }

                return View(iqX.ToPagedList(pageNumber, pageSize));
            }

            return View(iq.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult InterviewQuestionSearch(FormCollection form)
        {
            string str = form["txtSearchKeyword"].ToString();
            string can = form["Candidate"].ToString();
            string com = form["Company"].ToString();
            string tech = form["Technology"].ToString();
            string from = form["From"].ToString();
            string to = form["To"].ToString();


            return RedirectToAction("Index", "InterviewQuestions", new { searchStringKeyword = str, Candidate = can, Company =com, Technology = tech, From = from, To = to });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult InterviewQuestionSearchExt(FormCollection form)
        {
            string str = form["txtSearchKeyword"].ToString();
            string can = form["Candidate"].ToString();
            string com = form["Company"].ToString();
            string tech = form["Technology"].ToString();
            string from = form["From"].ToString();
            string to = form["To"].ToString();


            return RedirectToAction("InterviewQuestionsAccess", "InterviewQuestions", new { searchStringKeyword = str, Candidate = can, Company = com, Technology = tech, From = from, To = to });
        }

        [HttpGet]
        public void InterviewQuestionsExcel()
        {
            List<InterviewQuestion> data = db.InterviewQuestions.ToList();

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].DescriptionBlob != null)
                {
                    data[i].Description = Encoding.ASCII.GetString(data[i].DescriptionBlob);
                }
            }

            int row = 2;
            var xDate = DateTime.Now.ToShortDateString();

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Programs = Ep.Workbook.Worksheets.Add("Interview Questions");

            Programs.Cells["A1"].Value = "#";
            Programs.Cells["B1"].Value = "Candidate Name";
            Programs.Cells["C1"].Value = "Email";
            Programs.Cells["D1"].Value = "Company Name";
            Programs.Cells["E1"].Value = "Interview Date";
            Programs.Cells["F1"].Value = "Technology";
            Programs.Cells["G1"].Value = "Created on";
            Programs.Cells["H1"].Value = "Description";

            Programs.Cells["A1"].Style.Font.Bold = true;
            Programs.Cells["B1"].Style.Font.Bold = true;
            Programs.Cells["C1"].Style.Font.Bold = true;
            Programs.Cells["D1"].Style.Font.Bold = true;
            Programs.Cells["E1"].Style.Font.Bold = true;
            Programs.Cells["F1"].Style.Font.Bold = true;
            Programs.Cells["G1"].Style.Font.Bold = true;
            Programs.Cells["H1"].Style.Font.Bold = true;

            foreach (var item in data)
            {
                Programs.Cells[string.Format("A{0}", row)].Value = row - 1;
                Programs.Cells[string.Format("B{0}", row)].Value = item.CreatedBy;
                Programs.Cells[string.Format("C{0}", row)].Value = item.Email;
                Programs.Cells[string.Format("D{0}", row)].Value = item.CompanyName;
                Programs.Cells[string.Format("E{0}", row)].Value = Convert.ToDateTime(item.InterviewDate).ToString("MM/dd/yyyy");
                Programs.Cells[string.Format("F{0}", row)].Value = item.Technology;
                Programs.Cells[string.Format("G{0}", row)].Value = Convert.ToDateTime(item.CreatedOn).ToString("MM/dd/yyyy");
                Programs.Cells[string.Format("H{0}", row)].Value = item.Description;
                row++;
            }

            Programs.Cells["A:AZ"].AutoFitColumns();
            Response.ClearContent();

            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=InterviewQuestions_" + DateTime.Parse(xDate).ToShortDateString() + "_Report.xlsx");
            Response.ContentType = "application/ms-excel";
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();

        }

        public ActionResult CreateInterviewQuestion()
        {
            var tech = db.TrainingCourses.ToList();

            var techX = tech.Select(x => new
            {
                TrainingCourseID = x.TrainingCourseID,
                TrainingCourseName = x.TrainingCourseName
            }).GroupBy(x => x.TrainingCourseName).Select(g => g.First()).OrderBy(x => x.TrainingCourseName).ToList();


            ViewBag.TECHNOLOGIES = new SelectList(techX, "TrainingCourseName", "TrainingCourseName");

            return View();
        }


        [AllowAnonymous]
        public ActionResult CreateInterviewQuestionAccess()
        {
            var tech = db.TrainingCourses.ToList();

            var techX = tech.Select(x => new
            {
                TrainingCourseID = x.TrainingCourseID,
                TrainingCourseName = x.TrainingCourseName
            }).GroupBy(x => x.TrainingCourseName).Select(g => g.First()).OrderBy(x => x.TrainingCourseName).ToList();


            ViewBag.TECHNOLOGIES = new SelectList(techX, "TrainingCourseName", "TrainingCourseName");

            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveInterviewQuestion([Bind(Include = "CreatedBy,Email,CompanyName,InterviewDate,Technology,Description")] InterviewQuestion iq)
        {
            if (ModelState.IsValid)
            {
                if (iq.Description != null)
                    iq.DescriptionBlob = Encoding.ASCII.GetBytes(iq.Description);

                iq.Description = "";
                iq.CreatedOn = DateTime.Now;
                db.InterviewQuestions.Add(iq);
                db.SaveChanges();

                return RedirectToAction("InterviewQuestions");
            }

            var tech = db.TrainingCourses.ToList();

            var techX = tech.Select(x => new
            {
                TrainingCourseID = x.TrainingCourseID,
                TrainingCourseName = x.TrainingCourseName
            }).GroupBy(x => x.TrainingCourseName).Select(g => g.First()).OrderBy(x => x.TrainingCourseName).ToList();

            ViewBag.TECHNOLOGIES = new SelectList(techX, "TrainingCourseID", "TrainingCourseName");

            return View(iq);
        }

        [HttpPost, ValidateInput(false)]
        [AllowAnonymous]
        public ActionResult SaveInterviewQuestionExt([Bind(Include = "CreatedBy,Email,CompanyName,InterviewDate,Technology,Description")] InterviewQuestion iq)
        {
            if (ModelState.IsValid)
            {
                if (iq.Description != null)
                    iq.DescriptionBlob = Encoding.ASCII.GetBytes(iq.Description);

                iq.Description = "";
                iq.CreatedOn = DateTime.Now;
                db.InterviewQuestions.Add(iq);
                db.SaveChanges();

                return RedirectToAction("InterviewQuestionsAccess");
            }

            var tech = db.TrainingCourses.ToList();

            var techX = tech.Select(x => new
            {
                TrainingCourseID = x.TrainingCourseID,
                TrainingCourseName = x.TrainingCourseName
            }).GroupBy(x => x.TrainingCourseName).Select(g => g.First()).OrderBy(x => x.TrainingCourseName).ToList();

            ViewBag.TECHNOLOGIES = new SelectList(techX, "TrainingCourseID", "TrainingCourseName");

            return View(iq);
        }

        public ActionResult DeleteInterviewQuestion(int? id)
        {
            ViewBag.ID = id;

            return PartialView("_ConfirmDeleteInterviewQuestion");
        }

        [HttpPost]
        public ActionResult ConfirmDeleteInterviewQuestion(int id)
        {
            InterviewQuestion iq = db.InterviewQuestions.Find(id);
            db.InterviewQuestions.Remove(iq);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult InterviewQuestions(string sortOrder, string searchStringKeyword, string Candidate, string Company, string Technology, string From, string To, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SearchKeyword = searchStringKeyword;
            ViewBag.CandidateN = Candidate;
            ViewBag.Company = Company;
            ViewBag.Technology = Technology;
            ViewBag.From = From;
            ViewBag.To = To;

            List<InterviewQuestion> iq = db.InterviewQuestions.ToList();

            if (iq == null)
            {
                return HttpNotFound();
            }

            for (int i = 0; i < iq.Count; i++)
            {
                if (iq[i].DescriptionBlob != null)
                {
                    iq[i].Description = Encoding.ASCII.GetString(iq[i].DescriptionBlob);
                }
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "name" ? "name" : "name_desc";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "email" ? "email" : "email_desc";
            ViewBag.CompanySortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "company" ? "company" : "company_desc";
            ViewBag.InterviewDateSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "interviewdate" ? "interviewdate" : "interviewdate_desc";
            ViewBag.TechnologySortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "technology" ? "technology" : "technology_desc";
            ViewBag.DateCreatedSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "datecreated" ? "datecreated" : "datecreated_desc";
            ViewBag.ActiveSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "description" ? "description" : "description_desc";

            switch (sortOrder)
            {
                case "name":
                    iq = iq.OrderBy(x => x.CreatedBy).ToList();
                    break;

                case "name_desc":
                    iq = iq.OrderByDescending(x => x.CreatedBy).ToList();
                    break;

                case "email":
                    iq = iq.OrderBy(x => x.Email).ToList();
                    break;

                case "email_desc":
                    iq = iq.OrderByDescending(x => x.Email).ToList();
                    break;

                case "company":
                    iq = iq.OrderBy(x => x.CompanyName).ToList();
                    break;

                case "company_desc":
                    iq = iq.OrderByDescending(x => x.CompanyName).ToList();
                    break;

                case "interviewdate":
                    iq = iq.OrderBy(x => x.InterviewDate).ToList();
                    break;

                case "interviewdate_desc":
                    iq = iq.OrderByDescending(x => x.InterviewDate).ToList();
                    break;

                case "technology":
                    iq = iq.OrderBy(x => x.Technology).ToList();
                    break;

                case "technology_desc":
                    iq = iq.OrderByDescending(x => x.Technology).ToList();
                    break;

                case "datecreated":
                    iq = iq.OrderBy(x => x.CreatedOn).ToList();
                    break;

                case "datecreated_desc":
                    iq = iq.OrderByDescending(x => x.CreatedOn).ToList();
                    break;

                case "description":
                    iq = iq.OrderBy(x => x.Description).ToList();
                    break;

                case "description_desc":
                    iq = iq.OrderByDescending(x => x.Description).ToList();
                    break;

                default:
                    iq = iq.OrderByDescending(x => x.InterviewQuestionID).ToList();
                    break;
            }

            var tech = db.TrainingCourses.ToList();

            var techX = tech.Select(x => new
            {
                TrainingCourseID = x.TrainingCourseID,
                TrainingCourseName = x.TrainingCourseName
            }).GroupBy(x => x.TrainingCourseName).Select(g => g.First()).OrderBy(x => x.TrainingCourseName).ToList();

            ViewBag.TECHNOLOGIES = new SelectList(techX, "TrainingCourseID", "TrainingCourseName");

            var comp = db.InterviewQuestions.ToList();

            var compX = comp.Select(x => new
            {
                CompanyName = x.CompanyName
            }).GroupBy(x => x.CompanyName).Select(g => g.First()).OrderBy(x => x.CompanyName).ToList();

            ViewBag.COMPANIES = new SelectList(compX, "CompanyName", "CompanyName");

            var usersX = comp.Select(x => new
            {
                CreatedBy = x.CreatedBy
            }).GroupBy(x => x.CreatedBy).Select(g => g.First()).OrderBy(x => x.CreatedBy).ToList();

            ViewBag.CANDIDATE = new SelectList(usersX, "CreatedBy", "CreatedBy");

            if (!String.IsNullOrEmpty(searchStringKeyword) || !String.IsNullOrEmpty(Candidate) || !String.IsNullOrEmpty(Company) || !String.IsNullOrEmpty(Technology) || !String.IsNullOrEmpty(From) || !String.IsNullOrEmpty(To))
            {
                List<InterviewQuestion> iqX = iq;

                if (!String.IsNullOrEmpty(searchStringKeyword))
                {
                    iqX = iqX.Where(s => (s.Description.ToLower().Contains(searchStringKeyword.ToLower()))).ToList();
                }

                if (!String.IsNullOrEmpty(Candidate))
                {
                    iqX = iqX.Where(s => (s.CreatedBy.ToLower().Contains(Candidate.ToLower()))).ToList();
                }

                if (!String.IsNullOrEmpty(Company))
                {
                    iqX = iqX.Where(s => (s.CompanyName.ToLower().Contains(Company.ToLower()))).ToList();
                }

                if (!String.IsNullOrEmpty(Technology))
                {
                    var TrainingCoursX = db.TrainingCourses.Find(Convert.ToInt32(Technology));

                    iqX = iqX.Where(s => (s.Technology.ToLower().Contains(TrainingCoursX.TrainingCourseName.ToLower()))).ToList();
                }

                if (!String.IsNullOrEmpty(From))
                {
                    iqX = iqX.Where(s => (s.InterviewDate >= Convert.ToDateTime(From))).ToList();
                }

                if (!String.IsNullOrEmpty(To))
                {
                    iqX = iqX.Where(s => (s.InterviewDate <= Convert.ToDateTime(To))).ToList();
                }

                return View(iqX.ToPagedList(pageNumber, pageSize));
            }

            return View("Index", iq.ToPagedList(pageNumber, pageSize));
        }

        [AllowAnonymous]
        public ActionResult InterviewQuestionsAccess(string sortOrder, string searchStringKeyword, string Candidate, string Company, string Technology, string From, string To, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SearchKeyword = searchStringKeyword;
            ViewBag.CandidateN = Candidate;
            ViewBag.Company = Company;
            ViewBag.Technology = Technology;
            ViewBag.From = From;
            ViewBag.To = To;

            List<InterviewQuestion> iq = db.InterviewQuestions.ToList();

            if (iq == null)
            {
                return HttpNotFound();
            }

            for (int i = 0; i < iq.Count; i++)
            {
                if (iq[i].DescriptionBlob != null)
                {
                    iq[i].Description = Encoding.ASCII.GetString(iq[i].DescriptionBlob);
                }
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "name" ? "name" : "name_desc";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "email" ? "email" : "email_desc";
            ViewBag.CompanySortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "company" ? "company" : "company_desc";
            ViewBag.InterviewDateSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "interviewdate" ? "interviewdate" : "interviewdate_desc";
            ViewBag.TechnologySortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "technology" ? "technology" : "technology_desc";
            ViewBag.DateCreatedSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "datecreated" ? "datecreated" : "datecreated_desc";
            ViewBag.ActiveSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "description" ? "description" : "description_desc";

            switch (sortOrder)
            {
                case "name":
                    iq = iq.OrderBy(x => x.CreatedBy).ToList();
                    break;

                case "name_desc":
                    iq = iq.OrderByDescending(x => x.CreatedBy).ToList();
                    break;

                case "email":
                    iq = iq.OrderBy(x => x.Email).ToList();
                    break;

                case "email_desc":
                    iq = iq.OrderByDescending(x => x.Email).ToList();
                    break;

                case "company":
                    iq = iq.OrderBy(x => x.CompanyName).ToList();
                    break;

                case "company_desc":
                    iq = iq.OrderByDescending(x => x.CompanyName).ToList();
                    break;

                case "interviewdate":
                    iq = iq.OrderBy(x => x.InterviewDate).ToList();
                    break;

                case "interviewdate_desc":
                    iq = iq.OrderByDescending(x => x.InterviewDate).ToList();
                    break;

                case "technology":
                    iq = iq.OrderBy(x => x.Technology).ToList();
                    break;

                case "technology_desc":
                    iq = iq.OrderByDescending(x => x.Technology).ToList();
                    break;

                case "datecreated":
                    iq = iq.OrderBy(x => x.CreatedOn).ToList();
                    break;

                case "datecreated_desc":
                    iq = iq.OrderByDescending(x => x.CreatedOn).ToList();
                    break;

                case "description":
                    iq = iq.OrderBy(x => x.Description).ToList();
                    break;

                case "description_desc":
                    iq = iq.OrderByDescending(x => x.Description).ToList();
                    break;

                default:
                    iq = iq.OrderByDescending(x => x.InterviewQuestionID).ToList();
                    break;
            }

            var tech = db.TrainingCourses.ToList();

            var techX = tech.Select(x => new
            {
                TrainingCourseID = x.TrainingCourseID,
                TrainingCourseName = x.TrainingCourseName
            }).GroupBy(x => x.TrainingCourseName).Select(g => g.First()).OrderBy(x => x.TrainingCourseName).ToList();

            ViewBag.TECHNOLOGIES = new SelectList(techX, "TrainingCourseID", "TrainingCourseName");

            var comp = db.InterviewQuestions.ToList();

            var compX = comp.Select(x => new
            {
                CompanyName = x.CompanyName
            }).GroupBy(x => x.CompanyName).Select(g => g.First()).OrderBy(x => x.CompanyName).ToList();

            ViewBag.COMPANIES = new SelectList(compX, "CompanyName", "CompanyName");

            var usersX = comp.Select(x => new
            {
                CreatedBy = x.CreatedBy
            }).GroupBy(x => x.CreatedBy).Select(g => g.First()).OrderBy(x => x.CreatedBy).ToList();

            ViewBag.CANDIDATE = new SelectList(usersX, "CreatedBy", "CreatedBy");

            if (!String.IsNullOrEmpty(searchStringKeyword) || !String.IsNullOrEmpty(Candidate) || !String.IsNullOrEmpty(Company) || !String.IsNullOrEmpty(Technology) || !String.IsNullOrEmpty(From) || !String.IsNullOrEmpty(To))
            {
                List<InterviewQuestion> iqX = iq;

                if (!String.IsNullOrEmpty(searchStringKeyword))
                {
                    iqX = iqX.Where(s => (s.Description.ToLower().Contains(searchStringKeyword.ToLower()))).ToList();
                }

                if (!String.IsNullOrEmpty(Candidate))
                {
                    iqX = iqX.Where(s => (s.CreatedBy.ToLower().Contains(Candidate.ToLower()))).ToList();
                }

                if (!String.IsNullOrEmpty(Company))
                {
                    iqX = iqX.Where(s => (s.CompanyName.ToLower().Contains(Company.ToLower()))).ToList();
                }

                if (!String.IsNullOrEmpty(Technology))
                {
                    var TrainingCoursX = db.TrainingCourses.Find(Convert.ToInt32(Technology));

                    iqX = iqX.Where(s => (s.Technology.ToLower().Contains(TrainingCoursX.TrainingCourseName.ToLower()))).ToList();
                }

                if (!String.IsNullOrEmpty(From))
                {
                    iqX = iqX.Where(s => (s.InterviewDate >= Convert.ToDateTime(From))).ToList();
                }

                if (!String.IsNullOrEmpty(To))
                {
                    iqX = iqX.Where(s => (s.InterviewDate <= Convert.ToDateTime(To))).ToList();
                }

                return View(iqX.ToPagedList(pageNumber, pageSize));
            }

            return View(iq.ToPagedList(pageNumber, pageSize));
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
