using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TMS.Models;

namespace TMS.Controllers
{
    public class WebsiteUsersController : Controller
    {
        private TMSEntities db = new TMSEntities();

        public ActionResult WebsiteUsersList(string sortOrder, string searchString, int? page)
        {
            int pageSize = 25;
            int pageNumber = (page ?? 1);

            if (!String.IsNullOrEmpty(searchString))
            {
                //var wuX = wu.Where(s => s.FName.ToLower().Contains(searchString)).ToList();
                var wuX = db.WebsiteUsers.Where(x => x.FName.ToLower().Contains(searchString) || x.LName.ToLower().Contains(searchString) || x.Email.Contains(searchString) || x.Phone.Contains(searchString)).OrderByDescending(x => x.WebsiteUserID).ToList();
                ViewBag.Count = wuX.Count;
                return View(wuX.ToPagedList(pageNumber, pageSize));
            }

            ViewBag.CurrentSort = sortOrder;
            var wu = db.WebsiteUsers.Where(x => x.ConfirmedEmail == true).OrderByDescending(x => x.WebsiteUserID).ToList();

            if (wu == null)
            {
                return HttpNotFound();
            }

            var w = searchString;

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "name" ? "name" : "name_desc";
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "lastname" ? "lastname" : "lastname_desc";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "email" ? "email" : "email_desc";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "date" ? "date" : "date_desc";
            ViewBag.ActiveSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "active" ? "active" : "active_desc";
            ViewBag.StatusSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "status" ? "status" : "status_desc";
            ViewBag.RecruiterSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "recruiter" ? "recruiter" : "recruiter_desc";

            switch (sortOrder)
            {
                case "name":
                    wu = wu.OrderBy(x => x.FName).ToList();
                    break;

                case "name_desc":
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

                case "status":
                    wu = wu.OrderBy(x => x.Status).ToList();
                    break;

                case "status_desc":
                    wu = wu.OrderByDescending(x => x.Status).ToList();
                    break;

                case "recruiter":
                    wu = wu.OrderBy(x => x.RecruiterName).ToList();
                    break;

                case "recruiter_desc":
                    wu = wu.OrderByDescending(x => x.RecruiterName).ToList();
                    break;

                default:
                    wu = wu.OrderByDescending(x => x.WebsiteUserID).ToList();
                    break;
            }

            ViewBag.Count = wu.Count;
            return View(wu.ToPagedList(pageNumber, pageSize));
        }

        // GET: TrainingCours/Edit/5
        public ActionResult ViewWebsiteUser(int? id, string fromController, string fromView, int? tID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteUser websiteUser = db.WebsiteUsers.Find(id);
            if (websiteUser == null)
            {
                return HttpNotFound();
            }

            ViewBag.fromController = fromController;
            ViewBag.fromView = fromView;
            ViewBag.tID = tID;

            List<WebsiteUserDetail> wud = db.WebsiteUserDetails.Where(x => x.WebsiteUserID == id).ToList();

            

            var websiteUserTraining = db.WebsiteUserTrainings.Include(t => t.TrainingCours).Where(x => x.WebsiteUserID == id).OrderByDescending(x => x.PaymentDate).ToList();
            var websiteUserInvoice = db.WebsiteInvoices.Include(t => t.TrainingCours).Where(x => x.WebsiteUserID == id).ToList();
            List<MockInterview> mi = db.MockInterviews.Where(x => x.WebsiteUserID == id).OrderByDescending(x => x.MockInterviewID).ToList();
            List<User> users = db.Users.Where(x => (x.Roles_id == 1 || x.Roles_id == 2 || x.Roles_id == 4) && x.ActiveInactive == true).OrderBy(x => x.FullName).ToList();

            websiteUserTraining = websiteUserTraining.GroupBy(x => x.TrainingCourseID).Select(g => g.First()).ToList();

            ViewBag.TrainingC = websiteUserTraining.Count;
            ViewBag.InvoiceC = websiteUserInvoice.Count;

            ViewBag.UserID = websiteUser.WebsiteUserID;

            WebsiteUserResume wur = db.WebsiteUserResumes.Where(x => x.WebsiteUserID == id).OrderByDescending(x => x.ResumeID).FirstOrDefault();
            List<ResumeSendApproval> rsa = db.ResumeSendApprovals.Where(x => x.WebsiteUserID == id).ToList();

            dynamic MultiModel = new ExpandoObject();
            MultiModel.User = websiteUser;

            if (wud.Count > 0)
                MultiModel.UserDet = wud[0];
            else
                MultiModel.UserDet = null;

            MultiModel.UserTraining = websiteUserTraining;
            MultiModel.UserInvoice = websiteUserInvoice;
            MultiModel.Mocks = mi;
            MultiModel.Users = users;
            MultiModel.Resume = wur;
            MultiModel.Approvals = rsa;

            TempData["sendResume"] = TempData["sendResume"];

            return View(MultiModel);
        }

        public ActionResult EditWebsiteUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteUser websiteUser = db.WebsiteUsers.Find(id);
            if (websiteUser == null)
            {
                return HttpNotFound();
            }

            WebsiteUserFull wuInfo = new WebsiteUserFull();
            wuInfo.WebsiteUserID = websiteUser.WebsiteUserID;
            wuInfo.FName = websiteUser.FName;
            wuInfo.LName = websiteUser.LName;
            wuInfo.Email = websiteUser.Email;
            wuInfo.Phone = websiteUser.Phone;
            wuInfo.SkypeID = websiteUser.SkypeID;
            wuInfo.RegisterDate = websiteUser.RegisterDate;
            wuInfo.AboutUs = websiteUser.AboutUs;
            wuInfo.WhyInterested = websiteUser.WhyInterested;
            wuInfo.AcceptPP = websiteUser.AcceptPP;
            wuInfo.ReceiveUpdates = websiteUser.ReceiveUpdates;
            wuInfo.IsActive = websiteUser.IsActive;
            wuInfo.Status = websiteUser.Status;
            wuInfo.TakeEvaluation = websiteUser.TakeEvaluation;
            wuInfo.EvaluationStatus = websiteUser.EvaluationStatus;
            wuInfo.RecruiterName = websiteUser.RecruiterName;

            List <WebsiteUserDetail> wud = db.WebsiteUserDetails.Where(x => x.WebsiteUserID == id).ToList();

            if (wud.Count > 0)
            {
                List<YesNo> yn = new List<YesNo>();
                yn.Add(new YesNo() { ID = 0, Value = "No" });
                yn.Add(new YesNo() { ID = 1, Value = "Yes" });

                wuInfo.HighestEducation = wud[0].HighestEducation;
                wuInfo.CompletionYear = wud[0].CompletionYear;
                wuInfo.WorkExperienceIT = wud[0].WorkExperienceIT;
                wuInfo.WorkExperienceNonIT = wud[0].WorkExperienceNonIT;
                wuInfo.CurrentlyEmployed = wud[0].CurrentlyEmployed;
                wuInfo.AuthorizedWorkUSA = wud[0].AuthorizedWorkUSA;
                wuInfo.VisaStatus = wud[0].VisaStatus;
                wuInfo.Address = wud[0].Address;
                wuInfo.City = wud[0].City;
                wuInfo.CountryState = wud[0].CountryState;
                wuInfo.ZIPCode = wud[0].ZIPCode;
                wuInfo.OpenToRelocate = wud[0].OpenToRelocate;

                ViewBag.Relocate = new SelectList(yn, "ID", "Value", wuInfo.OpenToRelocate);
                ViewBag.WorkUSA = new SelectList(yn, "ID", "Value", wuInfo.AuthorizedWorkUSA);
            }
                
            else
            {
                wuInfo.HighestEducation = null;
                wuInfo.CompletionYear = null;
                wuInfo.WorkExperienceIT = null;
                wuInfo.WorkExperienceNonIT = null;
                wuInfo.CurrentlyEmployed = null;
                wuInfo.AuthorizedWorkUSA = null;
                wuInfo.VisaStatus = null;
                wuInfo.Address = null;
                wuInfo.City = null;
                wuInfo.CountryState = null;
                wuInfo.ZIPCode = null;
                wuInfo.OpenToRelocate = null;
            }

            List<User> users = db.Users.Where(x => x.Roles_id == 3 && x.ActiveInactive == true).OrderBy(x => x.FullName).ToList();
            ViewBag.ID_RECRUITER = new SelectList(users, "FullName", "FullName", wuInfo.RecruiterName);

            return View(wuInfo);
        }

        [HttpPost]
        public ActionResult EditWebsiteUser([Bind(Include = "WebsiteUserID,Email,Phone,SkypeID,RecruiterName,VisaStatus,AuthorizedWorkUSA,OpenToRelocate")] WebsiteUserFull wu)
        {

            WebsiteUser user = db.WebsiteUsers.Find(wu.WebsiteUserID);

            user.Email = wu.Email;
            user.Phone = wu.Phone;
            user.SkypeID = wu.SkypeID;
            user.RecruiterName = wu.RecruiterName;

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            WebsiteUserDetail userDetails = db.WebsiteUserDetails.Where(x => x.WebsiteUserID == wu.WebsiteUserID).First();

            userDetails.VisaStatus = wu.VisaStatus;
            userDetails.AuthorizedWorkUSA = wu.AuthorizedWorkUSA;
            userDetails.OpenToRelocate = wu.OpenToRelocate;

            db.Entry(userDetails).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Message"] = "Updated successfully";

            return RedirectToAction("EditWebsiteUser", new { id = wu.WebsiteUserID});
        }

        [HttpPost]
        public ActionResult WebsiteUsersSearch(FormCollection form)
        {
            string str = form["txtSearch"].ToString();
            return RedirectToAction("WebsiteUsersList", "WebsiteUsers", new { searchString = str });
        }

        [HttpPost]
        public ActionResult WebsiteEnrolledUsersSearch(FormCollection form)
        {
            string str = form["txtSearch"].ToString();
            return RedirectToAction("WebsiteEnrolledUsersList", "WebsiteUsers", new { searchString = str });
        }

        [HttpGet]
        public void WebsiteUsersExcel()
        {
            List<WebsiteUser> data = db.WebsiteUsers.OrderByDescending(x => x.WebsiteUserID).ToList();

            int row = 2;
            int rowActive = 2;
            int rowInactive = 2;
            //if(data.Count() > 0  )
            {
                var xDate = DateTime.Now.ToShortDateString();

                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet WebsiteUsers = Ep.Workbook.Worksheets.Add("Website Users");
                ExcelWorksheet WebsiteUsersA = Ep.Workbook.Worksheets.Add("Website Active Users");
                ExcelWorksheet WebsiteUsersI = Ep.Workbook.Worksheets.Add("Website Inactive Users");

                WebsiteUsers.Cells["A1"].Value = "#";
                WebsiteUsers.Cells["B1"].Value = "First Name";
                WebsiteUsers.Cells["C1"].Value = "Last Name";
                WebsiteUsers.Cells["D1"].Value = "Phone Number";
                WebsiteUsers.Cells["E1"].Value = "Email";
                WebsiteUsers.Cells["F1"].Value = "SkypeID";
                WebsiteUsers.Cells["G1"].Value = "How did you hear about us?";
                WebsiteUsers.Cells["H1"].Value = "Why are you interested in Brightrace?";
                WebsiteUsers.Cells["I1"].Value = "Accept Privacy Policy";
                WebsiteUsers.Cells["J1"].Value = "Receive Updates";
                WebsiteUsers.Cells["K1"].Value = "Registration Date";
                WebsiteUsers.Cells["L1"].Value = "Active";
                WebsiteUsers.Cells["M1"].Value = "Status";

                WebsiteUsers.Cells["A1"].Style.Font.Bold = true;
                WebsiteUsers.Cells["B1"].Style.Font.Bold = true;
                WebsiteUsers.Cells["C1"].Style.Font.Bold = true;
                WebsiteUsers.Cells["D1"].Style.Font.Bold = true;
                WebsiteUsers.Cells["E1"].Style.Font.Bold = true;
                WebsiteUsers.Cells["F1"].Style.Font.Bold = true;
                WebsiteUsers.Cells["G1"].Style.Font.Bold = true;
                WebsiteUsers.Cells["H1"].Style.Font.Bold = true;
                WebsiteUsers.Cells["I1"].Style.Font.Bold = true;
                WebsiteUsers.Cells["J1"].Style.Font.Bold = true;
                WebsiteUsers.Cells["K1"].Style.Font.Bold = true;
                WebsiteUsers.Cells["L1"].Style.Font.Bold = true;
                WebsiteUsers.Cells["M1"].Style.Font.Bold = true;

                WebsiteUsersA.Cells["A1"].Value = "#";
                WebsiteUsersA.Cells["B1"].Value = "First Name";
                WebsiteUsersA.Cells["C1"].Value = "Last Name";
                WebsiteUsersA.Cells["D1"].Value = "Phone Number";
                WebsiteUsersA.Cells["E1"].Value = "Email";
                WebsiteUsersA.Cells["F1"].Value = "SkypeID";
                WebsiteUsersA.Cells["G1"].Value = "How did you hear about us?";
                WebsiteUsersA.Cells["H1"].Value = "Why are you interested in Brightrace?";
                WebsiteUsersA.Cells["I1"].Value = "Accept Privacy Policy";
                WebsiteUsersA.Cells["J1"].Value = "Receive Updates";
                WebsiteUsersA.Cells["K1"].Value = "Registration Date";
                WebsiteUsersA.Cells["L1"].Value = "Active";
                WebsiteUsersA.Cells["M1"].Value = "Status";

                WebsiteUsersA.Cells["A1"].Style.Font.Bold = true;
                WebsiteUsersA.Cells["B1"].Style.Font.Bold = true;
                WebsiteUsersA.Cells["C1"].Style.Font.Bold = true;
                WebsiteUsersA.Cells["D1"].Style.Font.Bold = true;
                WebsiteUsersA.Cells["E1"].Style.Font.Bold = true;
                WebsiteUsersA.Cells["F1"].Style.Font.Bold = true;
                WebsiteUsersA.Cells["G1"].Style.Font.Bold = true;
                WebsiteUsersA.Cells["H1"].Style.Font.Bold = true;
                WebsiteUsersA.Cells["I1"].Style.Font.Bold = true;
                WebsiteUsersA.Cells["J1"].Style.Font.Bold = true;
                WebsiteUsersA.Cells["K1"].Style.Font.Bold = true;
                WebsiteUsersA.Cells["L1"].Style.Font.Bold = true;
                WebsiteUsersA.Cells["M1"].Style.Font.Bold = true;

                WebsiteUsersI.Cells["A1"].Value = "#";
                WebsiteUsersI.Cells["B1"].Value = "First Name";
                WebsiteUsersI.Cells["C1"].Value = "Last Name";
                WebsiteUsersI.Cells["D1"].Value = "Phone Number";
                WebsiteUsersI.Cells["E1"].Value = "Email";
                WebsiteUsersI.Cells["F1"].Value = "SkypeID";
                WebsiteUsersI.Cells["G1"].Value = "How did you hear about us?";
                WebsiteUsersI.Cells["H1"].Value = "Why are you interested in Brightrace?";
                WebsiteUsersI.Cells["I1"].Value = "Accept Privacy Policy";
                WebsiteUsersI.Cells["J1"].Value = "Receive Updates";
                WebsiteUsersI.Cells["K1"].Value = "Registration Date";
                WebsiteUsersI.Cells["L1"].Value = "Active";
                WebsiteUsersI.Cells["M1"].Value = "Status";

                WebsiteUsersI.Cells["A1"].Style.Font.Bold = true;
                WebsiteUsersI.Cells["B1"].Style.Font.Bold = true;
                WebsiteUsersI.Cells["C1"].Style.Font.Bold = true;
                WebsiteUsersI.Cells["D1"].Style.Font.Bold = true;
                WebsiteUsersI.Cells["E1"].Style.Font.Bold = true;
                WebsiteUsersI.Cells["F1"].Style.Font.Bold = true;
                WebsiteUsersI.Cells["G1"].Style.Font.Bold = true;
                WebsiteUsersI.Cells["H1"].Style.Font.Bold = true;
                WebsiteUsersI.Cells["I1"].Style.Font.Bold = true;
                WebsiteUsersI.Cells["J1"].Style.Font.Bold = true;
                WebsiteUsersI.Cells["K1"].Style.Font.Bold = true;
                WebsiteUsersI.Cells["L1"].Style.Font.Bold = true;
                WebsiteUsersI.Cells["M1"].Style.Font.Bold = true;

                foreach (var item in data)
                {
                    var privacy = "N/A";
                    var updates = "N/A";
                    var active = "N/A";
                    var date = "N/A";

                    if (item.AcceptPP == true)
                        privacy = "Yes";
                    else if (item.AcceptPP == false)
                        privacy = "No";

                    if (item.ReceiveUpdates == true)
                        updates = "Yes";
                    else if (item.ReceiveUpdates == false)
                        updates = "No";

                    if (item.IsActive == true)
                        active = "Yes";
                    else if (item.IsActive == false)
                        active = "No";

                    if (item.RegisterDate != null)
                        date = Convert.ToDateTime(item.RegisterDate).ToString("MM/dd/yyyy");
                    else if (item.IsActive == false)
                        date = "";


                    WebsiteUsers.Cells[string.Format("A{0}", row)].Value = row - 1;
                    WebsiteUsers.Cells[string.Format("B{0}", row)].Value = item.FName;
                    WebsiteUsers.Cells[string.Format("C{0}", row)].Value = item.LName;
                    WebsiteUsers.Cells[string.Format("D{0}", row)].Value = item.Phone;
                    WebsiteUsers.Cells[string.Format("E{0}", row)].Value = item.Email;
                    WebsiteUsers.Cells[string.Format("F{0}", row)].Value = item.SkypeID;
                    WebsiteUsers.Cells[string.Format("G{0}", row)].Value = item.AboutUs;
                    WebsiteUsers.Cells[string.Format("H{0}", row)].Value = item.WhyInterested;
                    WebsiteUsers.Cells[string.Format("I{0}", row)].Value = privacy;
                    WebsiteUsers.Cells[string.Format("J{0}", row)].Value = updates;
                    WebsiteUsers.Cells[string.Format("K{0}", row)].Value = date;
                    WebsiteUsers.Cells[string.Format("L{0}", row)].Value = active;
                    WebsiteUsers.Cells[string.Format("M{0}", row)].Value = item.Status;

                    if (item.IsActive == true)
                    {
                        WebsiteUsersA.Cells[string.Format("A{0}", rowActive)].Value = rowActive - 1;
                        WebsiteUsersA.Cells[string.Format("B{0}", rowActive)].Value = item.FName;
                        WebsiteUsersA.Cells[string.Format("C{0}", rowActive)].Value = item.LName;
                        WebsiteUsersA.Cells[string.Format("D{0}", rowActive)].Value = item.Phone;
                        WebsiteUsersA.Cells[string.Format("E{0}", rowActive)].Value = item.Email;
                        WebsiteUsersA.Cells[string.Format("F{0}", rowActive)].Value = item.SkypeID;
                        WebsiteUsersA.Cells[string.Format("G{0}", rowActive)].Value = item.AboutUs;
                        WebsiteUsersA.Cells[string.Format("H{0}", rowActive)].Value = item.WhyInterested;
                        WebsiteUsersA.Cells[string.Format("I{0}", rowActive)].Value = privacy;
                        WebsiteUsersA.Cells[string.Format("J{0}", rowActive)].Value = updates;
                        WebsiteUsersA.Cells[string.Format("K{0}", rowActive)].Value = date;
                        WebsiteUsersA.Cells[string.Format("L{0}", rowActive)].Value = active;
                        WebsiteUsersA.Cells[string.Format("M{0}", rowActive)].Value = item.Status;

                        rowActive++;

                    }

                    if (item.IsActive == false)
                    {
                        WebsiteUsersI.Cells[string.Format("A{0}", rowInactive)].Value = rowInactive - 1;
                        WebsiteUsersI.Cells[string.Format("B{0}", rowInactive)].Value = item.FName;
                        WebsiteUsersI.Cells[string.Format("C{0}", rowInactive)].Value = item.LName;
                        WebsiteUsersI.Cells[string.Format("D{0}", rowInactive)].Value = item.Phone;
                        WebsiteUsersI.Cells[string.Format("E{0}", rowInactive)].Value = item.Email;
                        WebsiteUsersI.Cells[string.Format("F{0}", rowInactive)].Value = item.SkypeID;
                        WebsiteUsersI.Cells[string.Format("G{0}", rowInactive)].Value = item.AboutUs;
                        WebsiteUsersI.Cells[string.Format("H{0}", rowInactive)].Value = item.WhyInterested;
                        WebsiteUsersI.Cells[string.Format("I{0}", rowInactive)].Value = privacy;
                        WebsiteUsersI.Cells[string.Format("J{0}", rowInactive)].Value = updates;
                        WebsiteUsersI.Cells[string.Format("K{0}", rowInactive)].Value = date;
                        WebsiteUsersI.Cells[string.Format("L{0}", rowInactive)].Value = active;
                        WebsiteUsersI.Cells[string.Format("M{0}", rowInactive)].Value = item.Status;

                        rowInactive++;
                    }
                    row++;
                }

                WebsiteUsers.Cells["A:AZ"].AutoFitColumns();
                WebsiteUsersA.Cells["A:AZ"].AutoFitColumns();
                WebsiteUsersI.Cells["A:AZ"].AutoFitColumns();
                Response.ClearContent();

                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=WebsiteUsers_" + DateTime.Parse(xDate).ToShortDateString() + "_Report.xlsx");
                Response.ContentType = "application/ms-excel";
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
            }

        }

        [HttpGet]
        public void WebsiteEnrolledUsersExcel()
        {
            List<WebsiteUserTraining> data = db.WebsiteUserTrainings.Where(x => (x.PaymentType != 5 || x.PaymentType != 6)).OrderBy(x => x.WebsiteUser.FName).GroupBy(x => x.WebsiteUserID).Select(y => y.FirstOrDefault()).ToList();

            data = data.OrderBy(x => x.WebsiteUser.FName).ToList();

            int row = 2;

            var xDate = DateTime.Now.ToShortDateString();

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet WebsiteEnrolledUsers = Ep.Workbook.Worksheets.Add("Enrolled Users");

            WebsiteEnrolledUsers.Cells["A1"].Value = "#";
            WebsiteEnrolledUsers.Cells["B1"].Value = "First Name";
            WebsiteEnrolledUsers.Cells["C1"].Value = "Last Name";
            WebsiteEnrolledUsers.Cells["D1"].Value = "Phone Number";
            WebsiteEnrolledUsers.Cells["E1"].Value = "Email";
            WebsiteEnrolledUsers.Cells["F1"].Value = "SkypeID";
            WebsiteEnrolledUsers.Cells["G1"].Value = "How did you hear about us?";
            WebsiteEnrolledUsers.Cells["H1"].Value = "Why are you interested in Brightrace?";
            WebsiteEnrolledUsers.Cells["I1"].Value = "Accept Privacy Policy";
            WebsiteEnrolledUsers.Cells["J1"].Value = "Receive Updates";
            WebsiteEnrolledUsers.Cells["K1"].Value = "Registration Date";
            WebsiteEnrolledUsers.Cells["L1"].Value = "Active";
            WebsiteEnrolledUsers.Cells["M1"].Value = "Status";

            WebsiteEnrolledUsers.Cells["A1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["B1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["C1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["D1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["E1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["F1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["G1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["H1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["I1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["J1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["K1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["L1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["M1"].Style.Font.Bold = true;

            foreach (var item in data)
            {
                var privacy = "N/A";
                var updates = "N/A";
                var active = "N/A";
                var date = "N/A";

                if (item.WebsiteUser.AcceptPP == true)
                    privacy = "Yes";
                else if (item.WebsiteUser.AcceptPP == false)
                    privacy = "No";

                if (item.WebsiteUser.ReceiveUpdates == true)
                    updates = "Yes";
                else if (item.WebsiteUser.ReceiveUpdates == false)
                    updates = "No";

                if (item.WebsiteUser.IsActive == true)
                    active = "Yes";
                else if (item.WebsiteUser.IsActive == false)
                    active = "No";

                if (item.WebsiteUser.RegisterDate != null)
                    date = Convert.ToDateTime(item.WebsiteUser.RegisterDate).ToString("MM/dd/yyyy");
                else if (item.WebsiteUser.IsActive == false)
                    date = "";


                WebsiteEnrolledUsers.Cells[string.Format("A{0}", row)].Value = row - 1;
                WebsiteEnrolledUsers.Cells[string.Format("B{0}", row)].Value = item.WebsiteUser.FName;
                WebsiteEnrolledUsers.Cells[string.Format("C{0}", row)].Value = item.WebsiteUser.LName;
                WebsiteEnrolledUsers.Cells[string.Format("D{0}", row)].Value = item.WebsiteUser.Phone;
                WebsiteEnrolledUsers.Cells[string.Format("E{0}", row)].Value = item.WebsiteUser.Email;
                WebsiteEnrolledUsers.Cells[string.Format("F{0}", row)].Value = item.WebsiteUser.SkypeID;
                WebsiteEnrolledUsers.Cells[string.Format("G{0}", row)].Value = item.WebsiteUser.AboutUs;
                WebsiteEnrolledUsers.Cells[string.Format("H{0}", row)].Value = item.WebsiteUser.WhyInterested;
                WebsiteEnrolledUsers.Cells[string.Format("I{0}", row)].Value = privacy;
                WebsiteEnrolledUsers.Cells[string.Format("J{0}", row)].Value = updates;
                WebsiteEnrolledUsers.Cells[string.Format("K{0}", row)].Value = date;
                WebsiteEnrolledUsers.Cells[string.Format("L{0}", row)].Value = active;
                WebsiteEnrolledUsers.Cells[string.Format("M{0}", row)].Value = item.WebsiteUser.Status;

                row++;
            }

            WebsiteEnrolledUsers.Cells["A:AZ"].AutoFitColumns();
            Response.ClearContent();

            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=BootcampEnrolledUsers_" + DateTime.Parse(xDate).ToShortDateString() + "_Report.xlsx");
            Response.ContentType = "application/ms-excel";
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();

        }

        [HttpGet]
        public void WebsiteEnrolledRegularTrainingExcel()
        {
            List<WebsiteUserTraining> data = db.WebsiteUserTrainings.Where(x => (x.PaymentType == 5 || x.PaymentType == 6)).OrderBy(x => x.WebsiteUser.FName).GroupBy(x => x.WebsiteUserID).Select(y => y.FirstOrDefault()).ToList();

            data = data.OrderBy(x => x.WebsiteUser.FName).ToList();

            int row = 2;

            var xDate = DateTime.Now.ToShortDateString();

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet WebsiteEnrolledUsers = Ep.Workbook.Worksheets.Add("Enrolled Users");

            WebsiteEnrolledUsers.Cells["A1"].Value = "#";
            WebsiteEnrolledUsers.Cells["B1"].Value = "First Name";
            WebsiteEnrolledUsers.Cells["C1"].Value = "Last Name";
            WebsiteEnrolledUsers.Cells["D1"].Value = "Phone Number";
            WebsiteEnrolledUsers.Cells["E1"].Value = "Email";
            WebsiteEnrolledUsers.Cells["F1"].Value = "SkypeID";
            WebsiteEnrolledUsers.Cells["G1"].Value = "How did you hear about us?";
            WebsiteEnrolledUsers.Cells["H1"].Value = "Why are you interested in Brightrace?";
            WebsiteEnrolledUsers.Cells["I1"].Value = "Accept Privacy Policy";
            WebsiteEnrolledUsers.Cells["J1"].Value = "Receive Updates";
            WebsiteEnrolledUsers.Cells["K1"].Value = "Registration Date";
            WebsiteEnrolledUsers.Cells["L1"].Value = "Active";
            WebsiteEnrolledUsers.Cells["M1"].Value = "Status";

            WebsiteEnrolledUsers.Cells["A1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["B1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["C1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["D1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["E1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["F1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["G1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["H1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["I1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["J1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["K1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["L1"].Style.Font.Bold = true;
            WebsiteEnrolledUsers.Cells["M1"].Style.Font.Bold = true;

            foreach (var item in data)
            {
                var privacy = "N/A";
                var updates = "N/A";
                var active = "N/A";
                var date = "N/A";

                if (item.WebsiteUser.AcceptPP == true)
                    privacy = "Yes";
                else if (item.WebsiteUser.AcceptPP == false)
                    privacy = "No";

                if (item.WebsiteUser.ReceiveUpdates == true)
                    updates = "Yes";
                else if (item.WebsiteUser.ReceiveUpdates == false)
                    updates = "No";

                if (item.WebsiteUser.IsActive == true)
                    active = "Yes";
                else if (item.WebsiteUser.IsActive == false)
                    active = "No";

                if (item.WebsiteUser.RegisterDate != null)
                    date = Convert.ToDateTime(item.WebsiteUser.RegisterDate).ToString("MM/dd/yyyy");
                else if (item.WebsiteUser.IsActive == false)
                    date = "";


                WebsiteEnrolledUsers.Cells[string.Format("A{0}", row)].Value = row - 1;
                WebsiteEnrolledUsers.Cells[string.Format("B{0}", row)].Value = item.WebsiteUser.FName;
                WebsiteEnrolledUsers.Cells[string.Format("C{0}", row)].Value = item.WebsiteUser.LName;
                WebsiteEnrolledUsers.Cells[string.Format("D{0}", row)].Value = item.WebsiteUser.Phone;
                WebsiteEnrolledUsers.Cells[string.Format("E{0}", row)].Value = item.WebsiteUser.Email;
                WebsiteEnrolledUsers.Cells[string.Format("F{0}", row)].Value = item.WebsiteUser.SkypeID;
                WebsiteEnrolledUsers.Cells[string.Format("G{0}", row)].Value = item.WebsiteUser.AboutUs;
                WebsiteEnrolledUsers.Cells[string.Format("H{0}", row)].Value = item.WebsiteUser.WhyInterested;
                WebsiteEnrolledUsers.Cells[string.Format("I{0}", row)].Value = privacy;
                WebsiteEnrolledUsers.Cells[string.Format("J{0}", row)].Value = updates;
                WebsiteEnrolledUsers.Cells[string.Format("K{0}", row)].Value = date;
                WebsiteEnrolledUsers.Cells[string.Format("L{0}", row)].Value = active;
                WebsiteEnrolledUsers.Cells[string.Format("M{0}", row)].Value = item.WebsiteUser.Status;

                row++;
            }

            WebsiteEnrolledUsers.Cells["A:AZ"].AutoFitColumns();
            Response.ClearContent();

            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=RegularTrainingEnrolledUsers_" + DateTime.Parse(xDate).ToShortDateString() + "_Report.xlsx");
            Response.ContentType = "application/ms-excel";
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();

        }

        public ActionResult WebsiteEnrolledUsersList(string sortOrder, string searchString, int? page)
        {
            int pageSize = 25;
            int pageNumber = (page ?? 1);

            if (!String.IsNullOrEmpty(searchString))
            {
                //var wuX = wu.Where(s => s.FName.ToLower().Contains(searchString)).ToList();
                var wuX = db.WebsiteUserTrainings.Where(x => (x.WebsiteUser.FName.ToLower().Contains(searchString) || x.WebsiteUser.LName.ToLower().Contains(searchString) || x.WebsiteUser.Email.Contains(searchString) || x.WebsiteUser.Phone.Contains(searchString)) && x.PaymentType != 4).OrderBy(x => x.WebsiteUser.FName).GroupBy(x => x.WebsiteUserID).Select(y => y.FirstOrDefault()).ToList();
                ViewBag.Count = wuX.Count;
                return View(wuX.ToPagedList(pageNumber, pageSize));
            }

            ViewBag.CurrentSort = sortOrder;
            //var wu = db.WebsiteUserTrainings.Where(x => x.PaymentType != 4).OrderBy(x => x.WebsiteUser.FName).GroupBy(x => x.WebsiteUserID).Select(y => y.FirstOrDefault()).ToList();
            var wu = db.WebsiteUserTrainings.Where(x => x.PaymentType != 5 && x.PaymentType != 6).OrderBy(x => x.WebsiteUser.FName).GroupBy(x => x.WebsiteUserID).Select(y => y.FirstOrDefault()).ToList();

            if (wu == null)
            {
                return HttpNotFound();
            }

            var w = searchString;

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "name" ? "name" : "name_desc";
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "lastname" ? "lastname" : "lastname_desc";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "email" ? "email" : "email_desc";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "date" ? "date" : "date_desc";
            ViewBag.ActiveSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "active" ? "active" : "active_desc";
            ViewBag.StatusSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "status" ? "status" : "status_desc";
            ViewBag.RecruiterSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "recruiter" ? "recruiter" : "recruiter_desc";

            switch (sortOrder)
            {
                case "name":
                    wu = wu.OrderBy(x => x.WebsiteUser.FName).ToList();
                    break;

                case "name_desc":
                    wu = wu.OrderByDescending(x => x.WebsiteUser.FName).ToList();
                    break;

                case "lastname":
                    wu = wu.OrderBy(x => x.WebsiteUser.LName).ToList();
                    break;

                case "lastname_desc":
                    wu = wu.OrderByDescending(x => x.WebsiteUser.LName).ToList();
                    break;

                case "email":
                    wu = wu.OrderBy(x => x.WebsiteUser.Email).ToList();
                    break;

                case "email_desc":
                    wu = wu.OrderByDescending(x => x.WebsiteUser.Email).ToList();
                    break;

                case "date":
                    wu = wu.OrderBy(x => x.WebsiteUser.RegisterDate).ToList();
                    break;

                case "date_desc":
                    wu = wu.OrderByDescending(x => x.WebsiteUser.RegisterDate).ToList();
                    break;

                case "active":
                    wu = wu.OrderBy(x => x.WebsiteUser.IsActive).ToList();
                    break;

                case "active_desc":
                    wu = wu.OrderByDescending(x => x.WebsiteUser.IsActive).ToList();
                    break;

                case "status":
                    wu = wu.OrderBy(x => x.WebsiteUser.Status).ToList();
                    break;

                case "status_desc":
                    wu = wu.OrderByDescending(x => x.WebsiteUser.Status).ToList();
                    break;

                case "recruiter":
                    wu = wu.OrderBy(x => x.WebsiteUser.RecruiterName).ToList();
                    break;

                case "recruiter_desc":
                    wu = wu.OrderByDescending(x => x.WebsiteUser.RecruiterName).ToList();
                    break;

                default:
                    wu = wu.OrderBy(x => x.WebsiteUser.FName).ToList();
                    break;
            }

            ViewBag.Count = wu.Count;
            return View(wu.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult RegularTrainingEnrolledUsersList(string sortOrder, string searchString, int? page)
        {
            int pageSize = 25;
            int pageNumber = (page ?? 1);

            if (!String.IsNullOrEmpty(searchString))
            {
                //var wuX = wu.Where(s => s.FName.ToLower().Contains(searchString)).ToList();
                var wuX = db.WebsiteUserTrainings.Where(x => (x.WebsiteUser.FName.ToLower().Contains(searchString) || x.WebsiteUser.LName.ToLower().Contains(searchString) || x.WebsiteUser.Email.Contains(searchString) || x.WebsiteUser.Phone.Contains(searchString)) && x.PaymentType != 4).OrderBy(x => x.WebsiteUser.FName).GroupBy(x => x.WebsiteUserID).Select(y => y.FirstOrDefault()).ToList();
                ViewBag.Count = wuX.Count;
                return View(wuX.ToPagedList(pageNumber, pageSize));
            }

            ViewBag.CurrentSort = sortOrder;
            //var wu = db.WebsiteUserTrainings.Where(x => x.PaymentType != 4).OrderBy(x => x.WebsiteUser.FName).GroupBy(x => x.WebsiteUserID).Select(y => y.FirstOrDefault()).ToList();
            var wu = db.WebsiteUserTrainings.Where(x => (x.PaymentType == 5 || x.PaymentType == 6)).OrderBy(x => x.WebsiteUser.FName).GroupBy(x => x.WebsiteUserID).Select(y => y.FirstOrDefault()).ToList();

            if (wu == null)
            {
                return HttpNotFound();
            }

            var w = searchString;

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "name" ? "name" : "name_desc";
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "lastname" ? "lastname" : "lastname_desc";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "email" ? "email" : "email_desc";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "date" ? "date" : "date_desc";
            ViewBag.ActiveSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "active" ? "active" : "active_desc";
            ViewBag.StatusSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "status" ? "status" : "status_desc";

            switch (sortOrder)
            {
                case "name":
                    wu = wu.OrderBy(x => x.WebsiteUser.FName).ToList();
                    break;

                case "name_desc":
                    wu = wu.OrderByDescending(x => x.WebsiteUser.FName).ToList();
                    break;

                case "lastname":
                    wu = wu.OrderBy(x => x.WebsiteUser.LName).ToList();
                    break;

                case "lastname_desc":
                    wu = wu.OrderByDescending(x => x.WebsiteUser.LName).ToList();
                    break;

                case "email":
                    wu = wu.OrderBy(x => x.WebsiteUser.Email).ToList();
                    break;

                case "email_desc":
                    wu = wu.OrderByDescending(x => x.WebsiteUser.Email).ToList();
                    break;

                case "date":
                    wu = wu.OrderBy(x => x.WebsiteUser.RegisterDate).ToList();
                    break;

                case "date_desc":
                    wu = wu.OrderByDescending(x => x.WebsiteUser.RegisterDate).ToList();
                    break;

                case "active":
                    wu = wu.OrderBy(x => x.WebsiteUser.IsActive).ToList();
                    break;

                case "active_desc":
                    wu = wu.OrderByDescending(x => x.WebsiteUser.IsActive).ToList();
                    break;

                case "status":
                    wu = wu.OrderBy(x => x.WebsiteUser.Status).ToList();
                    break;

                case "status_desc":
                    wu = wu.OrderByDescending(x => x.WebsiteUser.Status).ToList();
                    break;

                default:
                    wu = wu.OrderBy(x => x.WebsiteUser.FName).ToList();
                    break;
            }

            ViewBag.Count = wu.Count;
            return View(wu.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult GetMockInterviews(int? id)
        {
            List<MockInterview> mi = db.MockInterviews.Where(x => x.WebsiteUserID == id).OrderByDescending(x => x.MockInterviewID).ToList();

            dynamic MultiModel = new ExpandoObject();
            MultiModel.Mocks = mi;

            return PartialView(MultiModel);
        }

        [HttpPost, ValidateInput(false)]
        public async System.Threading.Tasks.Task<ActionResult> SendResumeApproval([Bind(Include = "WebsiteUserID,Send, SendToID")] WebsiteUserResumeCustom resume, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var user = db.WebsiteUsers.Find(resume.WebsiteUserID);
                var fileName = "";

                if (file != null && file.ContentLength > 0)
                {
                    // extract only the filename
                    fileName = user.FName + user.LName + DateTime.Now.ToString("yyyyMMdd-THHmmss") + Path.GetFileName(file.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    resume.FileName = fileName;
                    var path = Path.Combine(Server.MapPath("~/BrightraceResources/Resumes"), fileName);
                    resume.FilePath = path;
                    file.SaveAs(path);
                }



                WebsiteUserResume data = new WebsiteUserResume();
                data.FileName = resume.FileName;
                data.WebsiteUserID = resume.WebsiteUserID;
                data.DateCreate = DateTime.Now;
                data.UserCreate = Convert.ToInt32(Session["LoginId"]);
                data.Status = "Waiting for approval";
                data.Send = true;


                string filename1 = file.FileName;
                string relPath = "/BrightraceResources/Resumes/" + data.FileName;
                string imagesDirectory = Path.Combine(Environment.CurrentDirectory, relPath);
                var imagesDirectoryX = string.Format("{0}\\{1}", imagesDirectory, filename1);
                //string path1 = "https://brightrace.com/tms" + imagesDirectory;
                string path1 = Path.Combine(Server.MapPath("~/BrightraceResources/Resumes"));

                string path2 = Path.Combine(Server.MapPath("~/BrightraceResources/Resumes"), fileName);

                data.FilePath = path1;
                db.WebsiteUserResumes.Add(data);
                db.SaveChanges();

                var userSend = resume.SendToID.Count;

                for (int k = 0; k < userSend; k++)
                {
                    if (resume.Send[k] == true)
                    {
                        ResumeSendApproval rs = new ResumeSendApproval();
                        rs.ResumeID = data.ResumeID;
                        rs.WebsiteUserID = resume.WebsiteUserID;
                        rs.DateSubmited = DateTime.Now;
                        rs.SubmitedTo = resume.SendToID[k];
                        rs.SubmitedFrom = Convert.ToInt32(Session["LoginId"]);
                        db.ResumeSendApprovals.Add(rs);
                        db.SaveChanges();

                        var fName = user.FName + " " + user.LName;
                        var subject = "Resume approval";
                        var fromTrainer = db.Users.Find(Convert.ToInt32(Session["LoginId"]));
                        var toTrainer = db.Users.Find(resume.SendToID[k]);

                        var hrefApprove = "<a href = " + Url.Action("ApproveRejectResume", "WebsiteUsers", new { approvedBy = toTrainer.ID, userID = user.WebsiteUserID, rID = rs.ResumeSendApprovalID, approve = true }, Request.Url.Scheme) + " title =\"Approve\"> Approve </a>";
                        var hrefReject = "<a href = " + Url.Action("ApproveRejectResume", "WebsiteUsers", new { approvedBy = toTrainer.ID, userID = user.WebsiteUserID, rID = rs.ResumeSendApprovalID, approve = false }, Request.Url.Scheme) + " title =\"Reject\"> Reject </a>";
                        //var hrefReject = this.Url.RouteUrl("Default", new { Controller = "WebsiteUsers", Action = "ApproveRejectResume", approvedBy = fromTrainer.ID, userID = user.WebsiteUserID, approve = false });

                        var body = $@"<body style=""text-align:center; font-family: Arial, Helvetica, sans-serif; "">
                                <div style=""position:relative; 
    `                                           padding:50px 0px 50px; 
                                                background-position:center center; 
                                                background-repeat:no-repeat; 
                                                background-size:cover; 
                                                background-image:url('https://brightrace.com/ui/assets/images/inner-bg.jpg');"">
                                                <img src=""https://brightrace.com/ui/assets/images/logo-2.png"" alt=""Brightrace"" style=""width:200px"">
                                                <h1>{subject}</h1><br/>
                                                <table style=""margin-left:auto; margin-right:auto; width:450px; border:none; text-align:justify; border-radius:0px;"" cellpadding=""0"" cellspacing=""0"">
                                                <tbody style=""background-color:f7f8fd;""> 
                                                    <tr>
                                                        <td style=""padding: 10px 30px 10px 30px;"" > Hi <b> {toTrainer.FullName} </b>,</td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td style=""padding: 10px 30px 10px 30px;"" > Please approve/reject {fName} Resume.</td>
                                                    </tr>
                                                    
                                                    <tr>
                                                        <td style=""padding: 10px 30px 10px 30px; text-align:center;"" > 
                                                            <a href=""{path1}"" target = ""_blank"" > Click here to open resume</a>
                                                        </td>
                                                    </tr>

                                                    <tr>
                                                        <td style=""padding: 10px 30px 10px 30px; text-align:center;"" > 
                                                            { hrefApprove } | {hrefReject}
                                                        </td>
                                                    </tr>

                                                </tbody>
                                                <tfoot style=""text-align:center; font-size:small;"">
                                                    <tr>
                                                        <td>
                                                            <hr>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <a href = ""https://brightrace.com/ui/course/all"" target = ""_blank"" > Programs </a>
                                                            |
                                                            <a href = ""https://brightrace.com/ui/register"" target = ""_blank"" > Register </a>
                                                            |
                                                            <a href = ""https://brightrace.com/ui/login"" target = ""_blank"" > Sign in</a>
                                                            |
                                                            <a href =""https://brightrace.com/ui/policy"" target = ""_blank"" > Privacy Policy </a>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style = ""padding: 10px 30px 10px 30px;"" > 50 Cragwood Road, South Plainfield, <br/>
                                                                                                    New Jersey, United States<br/>
                                                                                                    Email: info@brightrace.com <br/>
                                                                                                    Phone: (908) 458 - 4161 <br/>
                                                                                                    <a href = ""https://brightrace.com"" target = ""_blank"" role = ""button""> www.brightrace.com </a>
                                                        </td>
                                                    </tr>
                                                </tfoot>
                                            </table>
                                        </div>
                                    </body >";

                        var subjectEmail = subject;

                        SendEmail se = new SendEmail();
                        await se.sendEmailAsync(subjectEmail, body, toTrainer.Email, path2);
                    }
                }

                WebsiteUser wu = db.WebsiteUsers.Find(resume.WebsiteUserID);
                db.Entry(wu).State = EntityState.Modified;
                db.Entry(wu).Property(x => x.FName).IsModified = false;
                db.Entry(wu).Property(x => x.LName).IsModified = false;
                db.Entry(wu).Property(x => x.Email).IsModified = false;
                db.Entry(wu).Property(x => x.Phone).IsModified = false;
                db.Entry(wu).Property(x => x.SkypeID).IsModified = false;
                db.Entry(wu).Property(x => x.AboutUs).IsModified = false;
                db.Entry(wu).Property(x => x.WhyInterested).IsModified = false;
                db.Entry(wu).Property(x => x.AcceptPP).IsModified = false;
                db.Entry(wu).Property(x => x.ReceiveUpdates).IsModified = false;
                db.Entry(wu).Property(x => x.Password).IsModified = false;
                db.Entry(wu).Property(x => x.RegisterDate).IsModified = false;
                db.Entry(wu).Property(x => x.LastUpdate).IsModified = false;
                db.Entry(wu).Property(x => x.ProfilePic).IsModified = false;
                db.Entry(wu).Property(x => x.IsActive).IsModified = false;
                db.Entry(wu).Property(x => x.ConfirmedEmail).IsModified = false;
                db.Entry(wu).Property(x => x.Token).IsModified = false;
                db.Entry(wu).Property(x => x.RecoveryCode).IsModified = false;
                wu.Status = "Waiting for Resume approval";
                db.SaveChanges();

                TempData["sendResume"] = "OK";
                return RedirectToAction("ViewWebsiteUser", new { id = resume.WebsiteUserID });
            }
            TempData["sendResume"] = "Error";
            return RedirectToAction("ViewWebsiteUser", new { id = resume.WebsiteUserID });
        }

        [AllowAnonymous]
        public ActionResult ApproveRejectResume(int? approvedBy, int? userID, int? rID, string approve)
        {
            if (approvedBy == null || userID == null || rID == null || (approve == null || approve == ""))
            {
                ViewBag.Message = "Error: Resume can't be approved";
                ViewBag.Response = "error.png";
                return View();
            }

            List<ResumeSendApproval> rsa = db.ResumeSendApprovals.Where(x => (x.ResumeSendApprovalID == rID && x.SubmitedTo == approvedBy && x.WebsiteUserID == userID)).ToList();

            if(rsa[0] == null)
            {
                ViewBag.Message = "Error: Resume can't be approved";
                ViewBag.Response = "error.png";
                return View();
            }
            else
            {
                if (rsa[0].Approved == true)
                {
                    ViewBag.Message = "The resume was already approved on "+rsa[0].DateApproved;
                    ViewBag.Response = "warning.png";
                    return View();
                }
                else if (rsa[0].Approved == false)
                {
                    ViewBag.Message = "The resume was already rejected";
                    ViewBag.Response = "warning.png";
                    return View();
                }
                else
                {
                    ResumeSendApproval updateStatus = db.ResumeSendApprovals.Find(rID);
                    db.Entry(updateStatus).State = EntityState.Modified;
                    updateStatus.Approved = Convert.ToBoolean(approve);
                    updateStatus.DateApproved = DateTime.Now;
                    db.Entry(updateStatus).Property(x => x.ResumeID).IsModified = false;
                    db.Entry(updateStatus).Property(x => x.WebsiteUserID).IsModified = false;
                    db.Entry(updateStatus).Property(x => x.DateSubmited).IsModified = false;
                    db.Entry(updateStatus).Property(x => x.SubmitedTo).IsModified = false;
                    db.Entry(updateStatus).Property(x => x.SubmitedFrom).IsModified = false;
                    db.SaveChanges();

                    var status = "rejected";
                    if (Convert.ToBoolean(approve) == true)
                        status = "approved";

                    ViewBag.Message = "The resume was "+ status + " successfully";
                    ViewBag.Response = "success.png";
                    return View();

                }
            }

            //return View();
        }

        [HttpPost]
        public ActionResult UploadUserResume(FormCollection form)
        {
            int userID = Convert.ToInt32(form["WebsiteUserID"].ToString());

            bool isSavedSuccessfully = true;
            string fname = "";

            try
            {
                foreach (string filename in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[filename];
                    fname = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        WebsiteUserResume wur = new WebsiteUserResume();
                        wur.FileName = file.FileName;
                        wur.WebsiteUserID = userID;
                        wur.DateCreate = DateTime.Now;
                        wur.UserCreate = Convert.ToInt32(Session["LoginId"]);
                        wur.Status = "Uploaded";
                        wur.FileSize = file.ContentLength;

                        db.WebsiteUserResumes.Add(wur);
                        db.SaveChanges();

                        string path1 = Path.Combine(Server.MapPath("~/BrightraceResources/Resumes/"));
                        string relPath = "/BrightraceResources/Resumes/";
                        string imagesDirectory = Path.Combine(Environment.CurrentDirectory, relPath);

                        string pathstring1 = Path.Combine(path1.ToString());

                        //string filename1 = DateTime.Now.ToFileTime()+"_"+file.FileName; 
                        string filename1 = file.FileName; 
                        bool isexist1 = Directory.Exists(pathstring1);
                        //bool isexist2 = Directory.Exists(pathstring2);
                        if (!isexist1)
                        {
                            Directory.CreateDirectory(pathstring1);
                        }

                        string uploadpath1 = string.Format("{0}\\{1}", pathstring1, filename1);
                        file.SaveAs(uploadpath1);
                        var imagesDirectoryX = string.Format("{0}\\{1}", imagesDirectory, filename1);


                        wur.FilePath = "https://brightrace.com/tms" + imagesDirectoryX;
                        //tr.BackupPath = uploadpath2;
                        wur.FileExtension = Path.GetExtension(filename1);

                        db.Entry(wur).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            catch (Exception)
            {
                isSavedSuccessfully = false;
            }

            if (isSavedSuccessfully)
            {
                return Json(new
                {
                    Message = fname
                });
            }
            else
            {
                return Json(new
                {
                    Message = "Error in Saving file"
                });
            }
        }


        [AllowAnonymous]
        public ActionResult GetUserResume(int? id)
        {
            List<WebsiteUserResume> tr = db.WebsiteUserResumes.Where(x => x.WebsiteUserID == id).ToList();

            //Get the images list from repository
            var attachmentsList = new List<AttachmentsModel>();

            foreach (var item in tr)
            {
                attachmentsList.Add(new AttachmentsModel()
                {
                    AttachmentID = item.ResumeID,
                    FileName = item.FileName,
                    Path = item.FilePath,
                    FileSize = item.FileSize,
                    FileExtension = item.FileExtension
                });

            }

            return Json(new { Data = attachmentsList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteUserResume(FormCollection form)
        {
            int fID = Convert.ToInt32(form["fID"]);
            int uID = Convert.ToInt32(form["uID"]);
            string fName = form["fName"].ToString();

            WebsiteUserResume wur = new WebsiteUserResume();

            if (fID != 0)
                wur = db.WebsiteUserResumes.Find(fID);

            else if (fID == 0)
                wur = db.WebsiteUserResumes.Where(x => x.FileName.Contains(fName) && x.WebsiteUserID == uID).First();

            db.WebsiteUserResumes.Remove(wur);
            db.SaveChanges();

            var p = wur.FilePath;

            var pResult = p.Replace("https://brightrace.com/tms", "");

            string path1 = Path.Combine(Server.MapPath("~" + pResult));

            if (System.IO.File.Exists(path1))
                System.IO.File.Delete(path1);

            return null;
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
