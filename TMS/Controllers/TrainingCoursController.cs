using Newtonsoft.Json;
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
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TMS.Models;

namespace TMS.Controllers
{
    public class TrainingCoursController : Controller
    {
        private TMSEntities db = new TMSEntities();

        // GET: TrainingCours
        public ActionResult TrainingCoursesList(string sortOrder, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;

            //var tc = db.TrainingCourses.OrderByDescending(x => x.TrainingCourseID).ToList();
            List<GetTrainingCourses_Result> tc = db.GetTrainingCourses().ToList();

            if (tc == null)
            {
                return HttpNotFound();
            }


            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "name" ? "name" : "name_desc";
            ViewBag.TypeSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "type" ? "type" : "type_desc";
            ViewBag.StartDate1SortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "startdate1" ? "startdate1" : "startdate1_desc";
            //ViewBag.StartDate2SortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "startdate2" ? "startdate2" : "startdate2_desc";
            ViewBag.Payment1SortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "payment1" ? "payment1" : "payment1_desc";
            ViewBag.Payment2SortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "payment2" ? "payment2" : "payment2_desc";
            ViewBag.TotalSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "total" ? "total" : "total_desc";
            ViewBag.ActiveSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "active" ? "active" : "active_desc";
            ViewBag.EnrolledSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder != "enrolled" ? "enrolled" : "enrolled_desc";

            switch (sortOrder)
            {
                case "name":
                    tc = tc.OrderBy(x => x.TrainingCourseName).ToList();
                    break;

                case "name_desc":
                    tc = tc.OrderByDescending(x => x.TrainingCourseName).ToList();
                    break;

                case "type":
                    tc = tc.OrderBy(x => x.TrainingType).ToList();
                    break;

                case "type_desc":
                    tc = tc.OrderByDescending(x => x.TrainingType).ToList();
                    break;

                case "startdate1":
                    tc = tc.OrderBy(x => x.StartDate1).ToList();
                    break;

                case "startdate1_desc":
                    tc = tc.OrderByDescending(x => x.StartDate1).ToList();
                    break;

                //case "startdate2":
                //    tc = tc.OrderBy(x => x.StartDate2).ToList();
                //    break;

                //case "startdate2_desc":
                //    tc = tc.OrderByDescending(x => x.StartDate2).ToList();
                //    break;

                case "payment1":
                    tc = tc.OrderBy(x => x.FirstPayment).ToList();
                    break;

                case "payment1_desc":
                    tc = tc.OrderByDescending(x => x.FirstPayment).ToList();
                    break;

                case "payment2":
                    tc = tc.OrderBy(x => x.SecondPayment).ToList();
                    break;

                case "payment2_desc":
                    tc = tc.OrderByDescending(x => x.SecondPayment).ToList();
                    break;

                case "total":
                    tc = tc.OrderBy(x => x.Price).ToList();
                    break;

                case "total_desc":
                    tc = tc.OrderByDescending(x => x.Price).ToList();
                    break;

                case "active":
                    tc = tc.OrderBy(x => x.IsActive).ToList();
                    break;

                case "active_desc":
                    tc = tc.OrderByDescending(x => x.IsActive).ToList();
                    break;

                case "enrolled":
                    tc = tc.OrderBy(x => x.TotalEnrolled).ToList();
                    break;

                case "enrolled_desc":
                    tc = tc.OrderByDescending(x => x.TotalEnrolled).ToList();
                    break;

                default:
                    tc = tc.OrderByDescending(x => x.TrainingCourseID).ToList();
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                var tcX = tc.Where(s => s.TrainingCourseName.ToLower().Contains(searchString));
                return View(tcX.ToPagedList(pageNumber, pageSize));
            }

            return View(tc.ToPagedList(pageNumber, pageSize));
        }

        // GET: TrainingCours/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingCours trainingCours = db.TrainingCourses.Find(id);
            if (trainingCours == null)
            {
                return HttpNotFound();
            }
            return View(trainingCours);
        }

        // GET: TrainingCours/Create
        public ActionResult CreateTrainingCourse()
        {
            ViewBag.TRAINING_TYPE = new SelectList(db.TrainingTypes, "TrainingTypeID", "TrainingType1");
            ViewBag.CreatedBy = new SelectList(db.Users, "ID", "FullName");
            return View();
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult SaveTrainingCourse([Bind(Include = "TrainingCourseID,TrainingCourseName,ShortDescription,FullDescription,PayPalCode,IsActive,Price,StartDate1,EndDate1,StartDate2,EndDate2,DiscountDate1,DiscountDate2,FirstPayment,SecondPayment,Coupon1,Coupon2,Coupon3,Coupon4,Coupon5,DiscountCoupon1,DiscountCoupon2,DiscountCoupon3,DiscountCoupon4,DiscountCoupon5,PriceFullPayment,LimitFullPaymentDate,SKU,MaxDiscountPayment1,MaxDiscountTotalPayment,TrainingTypeID")] TrainingCours trainingCours, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    // extract only the filename
                    var fileName = DateTime.Now.ToString("yyyyMMdd-THHmmss") + Path.GetFileName(file.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    trainingCours.ImgName = fileName;
                    var path = Path.Combine(Server.MapPath("~/Images/trainingCourses"), fileName);
                    trainingCours.ImgPath = path;
                    file.SaveAs(path);
                }

                if(trainingCours.FullDescription != null)
                    trainingCours.FullDescriptionBlob = Encoding.ASCII.GetBytes(trainingCours.FullDescription);

                trainingCours.FullDescription = "";
                trainingCours.CreatedOn = DateTime.Now;
                trainingCours.Sessions = 1;
                trainingCours.CreatedBy = Convert.ToInt32(Session["LoginId"]);
                db.TrainingCourses.Add(trainingCours);
                db.SaveChanges();

                return RedirectToAction("TrainingCoursesList");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "ID", "FullName", trainingCours.CreatedBy);
            return View(trainingCours);
        }

        // GET: TrainingCours/Edit/5
        public ActionResult EditTrainingCourse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingCours trainingCours = db.TrainingCourses.Find(id);
            if (trainingCours == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "ID", "FullName", trainingCours.CreatedBy);
            ViewBag.TRAINING_TYPE = new SelectList(db.TrainingTypes, "TrainingTypeID", "TrainingType1", trainingCours.TrainingTypeID);

            if (trainingCours.FullDescriptionBlob != null)
                trainingCours.FullDescription = Encoding.ASCII.GetString(trainingCours.FullDescriptionBlob);

            if (trainingCours.MeetingDetailsBlob != null)
                trainingCours.MeetingDetails = Encoding.ASCII.GetString(trainingCours.MeetingDetailsBlob);

            if (trainingCours.SpecialAnnouncementsBlob != null)
                trainingCours.SpecialAnnouncements = Encoding.ASCII.GetString(trainingCours.SpecialAnnouncementsBlob);

            List<TrainingSession> ts = db.TrainingSessions.Where(x => x.TrainingCourseID == id).ToList();

            List<WebsiteusersAssignment> assignments = db.WebsiteusersAssignments.Where(x => x.TrainingCourseID == id).OrderBy(x =>x.WebsiteUser.FName).ToList();

            List<User> trainers = db.Users.Where(x => (x.Roles_id == 4 && x.ActiveInactive == true)).OrderBy(x => x.FullName).ToList();

            List<TrainingTrainer> selectedTrainers = db.TrainingTrainers.Where(x => x.TrainingCourseID == id).ToList();

            dynamic MultiModel = new ExpandoObject();
            MultiModel.Training = trainingCours;
            MultiModel.Sessions = ts;
            MultiModel.Assignments = assignments;
            MultiModel.Trainers = trainers;
            MultiModel.SelectedTrainers = selectedTrainers;

            var tName = trainingCours.TrainingCourseName;
            tName = Regex.Replace(tName, @"[^0-9a-zA-Z:,!/\.]+", "");

            TempData["tName"] = tName;
            TempData["tID"] = trainingCours.TrainingCourseID;

            TempData["updateMessage"] = TempData["updateStatus"];


            return View(MultiModel);
        }


        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditTrainingCourse([Bind(Include = "TrainingCourseID,TrainingCourseName,ShortDescription,FullDescription,PayPalCode,IsActive,Price,StartDate1,EndDate1,StartDate2,EndDate2,DiscountDate1,DiscountDate2,FirstPayment,SecondPayment,Coupon1,Coupon2,Coupon3,Coupon4,Coupon5,DiscountCoupon1,DiscountCoupon2,DiscountCoupon3,DiscountCoupon4,DiscountCoupon5,PriceFullPayment,LimitFullPaymentDate,SKU,MaxDiscountPayment1,MaxDiscountTotalPayment,TrainingTypeID")] TrainingCours trainingCours, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    // extract only the filename
                    var fileName = DateTime.Now.ToString("yyyyMMdd-THHmmss") + Path.GetFileName(file.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    trainingCours.ImgName = fileName;
                    var path = Path.Combine(Server.MapPath("~/Images/trainingCourses"), fileName);
                    trainingCours.ImgPath = path;
                    file.SaveAs(path);
                }

                if (trainingCours.FullDescription != null)
                    trainingCours.FullDescriptionBlob = Encoding.ASCII.GetBytes(trainingCours.FullDescription);
                else
                    trainingCours.FullDescriptionBlob = Encoding.ASCII.GetBytes("");

                trainingCours.FullDescription = "";
                trainingCours.LastUpdate = DateTime.Now;

                db.Entry(trainingCours).State = EntityState.Modified;
                db.Entry(trainingCours).Property(x => x.MeetingDetailsBlob).IsModified = false;
                db.Entry(trainingCours).Property(x => x.SpecialAnnouncementsBlob).IsModified = false;
                db.Entry(trainingCours).Property(x => x.Sessions).IsModified = false;
                db.SaveChanges();

                TempData["updateStatus"] = "OK";

                return RedirectToAction("EditTrainingCourse", new { id = trainingCours.TrainingCourseID });
                //return RedirectToAction("TrainingCoursesList");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "ID", "FullName", trainingCours.CreatedBy);
            TempData["updateStatus"] = "Error";
            return RedirectToAction("EditTrainingCourse", new { id = trainingCours.TrainingCourseID });
            //return View(trainingCours);
        }

        // GET: TrainingCours/Delete/5
        public ActionResult DeleteTrainingCourse(int? id)
        {
            ViewBag.ID = id;

            return PartialView("_ConfirmDeleteTrainingCourse");
        }

        // POST: TrainingCours/Delete/5
        [HttpPost]
        public ActionResult ConfirmDeleteTrainingCourse(int id)
        {
            TrainingCours trainingCours = db.TrainingCourses.Find(id);
            db.TrainingCourses.Remove(trainingCours);
            db.SaveChanges();
            return RedirectToAction("TrainingCoursesList");
        }

        public ActionResult ViewTrainingCourse(int? id, int? tab=0)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TrainingCours trainingCours = db.TrainingCourses.Find(id);
            if (trainingCours == null)
            {
                return HttpNotFound();
            }
            ViewBag.Tab = tab;
            ViewBag.CreatedBy = new SelectList(db.Users, "ID", "FullName", trainingCours.CreatedBy);

            if (trainingCours.FullDescriptionBlob != null)
                trainingCours.FullDescription = Encoding.ASCII.GetString(trainingCours.FullDescriptionBlob);

            if (trainingCours.MeetingDetailsBlob != null)
                trainingCours.MeetingDetails = Encoding.ASCII.GetString(trainingCours.MeetingDetailsBlob);

            var usersTrainingAll = db.WebsiteUserTrainings.Include(u => u.WebsiteUser).Where(x => x.TrainingCourseID == id).OrderBy(x => x.WebsiteUser.FName).OrderByDescending(x => x.PaymentDate).ToList();

            usersTrainingAll = usersTrainingAll.GroupBy(x => x.WebsiteUserID).Select(g => g.First()).ToList();

            ViewBag.allUsers = usersTrainingAll.Count();
            ViewBag.fullUsers = usersTrainingAll.Where(x => x.PaymentType == 3).Count();
            ViewBag.fUsers = usersTrainingAll.Where(x => x.PaymentType == 1).Count();
            ViewBag.sUsers = usersTrainingAll.Where(x => x.PaymentType == 2).Count();
            ViewBag.nUsers = usersTrainingAll.Where(x => x.PaymentType == 4).Count();
            ViewBag.TrainingCourseID = trainingCours.TrainingCourseID;
            ViewBag.TotalAppear = usersTrainingAll.Where(x => x.WebsiteUser.TakeEvaluation == true).Count();
            ViewBag.TotalPass = usersTrainingAll.Where(x => x.WebsiteUser.EvaluationStatus == true).Count();

            var usersTrainingA = usersTrainingAll.OrderBy(x => x.WebsiteUser.FName).Where(x => x.TrainingGroup == "A").ToList();
            var usersTrainingB = usersTrainingAll.OrderBy(x => x.WebsiteUser.FName).Where(x => x.TrainingGroup == "B").ToList();
            var usersTrainingC = usersTrainingAll.OrderBy(x => x.WebsiteUser.FName).Where(x => x.TrainingGroup == "C").ToList();

            List<WebsiteUserAttendance> attendance = db.WebsiteUserAttendances.Where(x => x.TrainingCourseID == id).ToList();

            for (int i = 1; i <= 50; i++)
            {
                ViewBag.allUsersA = usersTrainingAll.OrderBy(x => x.WebsiteUser.FName).Where(x => x.TrainingGroup == "A").Count();
                ViewData["A_TodayDay" + i] = attendance.Where(x => x.Day == i && x.Attended == true && x.TrainingGroup == "A").Count();
            }

            if (usersTrainingB.Count > 0)
            {
                ViewBag.allUsersB = usersTrainingAll.OrderBy(x => x.WebsiteUser.FName).Where(x => x.TrainingGroup == "B").Count();
                for (int i = 1; i <= 50; i++)
                {
                    ViewData["B_TodayDay" + i] = attendance.Where(x => x.Day == i && x.Attended == true && x.TrainingGroup == "B").Count();
                }
            }

            if (usersTrainingC.Count > 0)
            {
                ViewBag.allUsersC = usersTrainingAll.Where(x => x.TrainingGroup == "C").Count();
                for (int i = 1; i <= 50; i++)
                {
                    ViewData["C_TodayDay" + i] = attendance.OrderBy(x => x.WebsiteUser.FName).Where(x => x.Day == i && x.Attended == true && x.TrainingGroup == "C").Count();
                }
            }

            List<TrainingTrainer> trainers = db.TrainingTrainers.Where(x => x.TrainingCourseID == id).ToList();

            ViewBag.TRAINING_COURSE_ID = new SelectList(db.TrainingCourses.OrderByDescending(s => s.TrainingCourseID), "TrainingCourseID", "TrainingCourseName");

            dynamic MultiModel = new ExpandoObject();
            MultiModel.Training = trainingCours;
            MultiModel.UsersAll = usersTrainingAll;
            MultiModel.UsersA = usersTrainingA;
            MultiModel.UsersB = usersTrainingB;
            MultiModel.UsersC = usersTrainingC;
            MultiModel.Attendance = attendance;
            MultiModel.Trainers = trainers;

            return View(MultiModel);
        }
        

       [HttpPost]
        public ActionResult TrainingCoursesSearch(FormCollection form)
        {
            string str = form["txtSearch"].ToString();
            return RedirectToAction("TrainingCoursesList", "TrainingCours", new { searchString = str });
        }

        [HttpGet]
        public void TrainingProgramsExcel()
        {
            List<TrainingCours> data = db.TrainingCourses.OrderByDescending(x => x.TrainingCourseID).ToList();

            int row = 2;
            int rowA = 2;
            int rowI = 2;
            //if(data.Count() > 0  )
            {
                var xDate = DateTime.Now.ToShortDateString();

                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Programs = Ep.Workbook.Worksheets.Add("All Programs");
                ExcelWorksheet ProgramsA = Ep.Workbook.Worksheets.Add(">"+xDate);
                ExcelWorksheet ProgramsI = Ep.Workbook.Worksheets.Add("<"+xDate);

                Programs.Cells["A1"].Value = "#";
                Programs.Cells["B1"].Value = "SKU";
                Programs.Cells["C1"].Value = "Training Program Name";
                Programs.Cells["D1"].Value = "Short Description";
                Programs.Cells["E1"].Value = "Start Date 1";
                Programs.Cells["F1"].Value = "End Date 1";
                Programs.Cells["G1"].Value = "Discount Date 1";
                Programs.Cells["H1"].Value = "Start Date 2";
                Programs.Cells["I1"].Value = "End Date 2";
                Programs.Cells["J1"].Value = "Discount Date 2";
                Programs.Cells["K1"].Value = "First Payment";
                Programs.Cells["L1"].Value = "Second Payment";
                Programs.Cells["M1"].Value = "Total Payment";
                Programs.Cells["N1"].Value = "Full Payment";
                Programs.Cells["O1"].Value = "Limit Date Full Payment";
                Programs.Cells["P1"].Value = "Coupon 1";
                Programs.Cells["Q1"].Value = "Discount Coupon 1";
                Programs.Cells["R1"].Value = "Coupon 2";
                Programs.Cells["S1"].Value = "Discount Coupon 2";
                Programs.Cells["T1"].Value = "Coupon 3";
                Programs.Cells["U1"].Value = "Discount Coupon 3";
                Programs.Cells["V1"].Value = "Coupon 4";
                Programs.Cells["W1"].Value = "Discount Coupon 4";
                Programs.Cells["X1"].Value = "Coupon 5";
                Programs.Cells["Y1"].Value = "Discount Coupon 5";
                Programs.Cells["Z1"].Value = "Active";

                Programs.Cells["A1"].Style.Font.Bold = true;
                Programs.Cells["B1"].Style.Font.Bold = true;
                Programs.Cells["C1"].Style.Font.Bold = true;
                Programs.Cells["D1"].Style.Font.Bold = true;
                Programs.Cells["E1"].Style.Font.Bold = true;
                Programs.Cells["F1"].Style.Font.Bold = true;
                Programs.Cells["G1"].Style.Font.Bold = true;
                Programs.Cells["H1"].Style.Font.Bold = true;
                Programs.Cells["I1"].Style.Font.Bold = true;
                Programs.Cells["J1"].Style.Font.Bold = true;
                Programs.Cells["K1"].Style.Font.Bold = true;
                Programs.Cells["L1"].Style.Font.Bold = true;
                Programs.Cells["M1"].Style.Font.Bold = true;
                Programs.Cells["N1"].Style.Font.Bold = true;
                Programs.Cells["O1"].Style.Font.Bold = true;
                Programs.Cells["P1"].Style.Font.Bold = true;
                Programs.Cells["Q1"].Style.Font.Bold = true;
                Programs.Cells["R1"].Style.Font.Bold = true;
                Programs.Cells["S1"].Style.Font.Bold = true;
                Programs.Cells["T1"].Style.Font.Bold = true;
                Programs.Cells["U1"].Style.Font.Bold = true;
                Programs.Cells["V1"].Style.Font.Bold = true;
                Programs.Cells["W1"].Style.Font.Bold = true;
                Programs.Cells["X1"].Style.Font.Bold = true;
                Programs.Cells["Y1"].Style.Font.Bold = true;
                Programs.Cells["Z1"].Style.Font.Bold = true;

                ProgramsA.Cells["A1"].Value = "#";
                ProgramsA.Cells["B1"].Value = "SKU";
                ProgramsA.Cells["C1"].Value = "Training Program Name";
                ProgramsA.Cells["D1"].Value = "Short Description";
                ProgramsA.Cells["E1"].Value = "Start Date 1";
                ProgramsA.Cells["F1"].Value = "End Date 1";
                ProgramsA.Cells["G1"].Value = "Discount Date1";
                ProgramsA.Cells["H1"].Value = "Start Date 2";
                ProgramsA.Cells["I1"].Value = "End Date 2";
                ProgramsA.Cells["J1"].Value = "Discount Date2";
                ProgramsA.Cells["K1"].Value = "First Payment";
                ProgramsA.Cells["L1"].Value = "Second Payment";
                ProgramsA.Cells["M1"].Value = "Total Payment";
                ProgramsA.Cells["N1"].Value = "Full Payment";
                ProgramsA.Cells["O1"].Value = "Limit Date Full Payment";
                ProgramsA.Cells["P1"].Value = "Coupon 1";
                ProgramsA.Cells["Q1"].Value = "Discount Coupon 1";
                ProgramsA.Cells["R1"].Value = "Coupon 2";
                ProgramsA.Cells["S1"].Value = "Discount Coupon 2";
                ProgramsA.Cells["T1"].Value = "Coupon 3";
                ProgramsA.Cells["U1"].Value = "Discount Coupon 3";
                ProgramsA.Cells["V1"].Value = "Coupon 4";
                ProgramsA.Cells["W1"].Value = "Discount Coupon 4";
                ProgramsA.Cells["X1"].Value = "Coupon 5";
                ProgramsA.Cells["Y1"].Value = "Discount Coupon 5";
                ProgramsA.Cells["Z1"].Value = "Active";

                ProgramsA.Cells["A1"].Style.Font.Bold = true;
                ProgramsA.Cells["B1"].Style.Font.Bold = true;
                ProgramsA.Cells["C1"].Style.Font.Bold = true;
                ProgramsA.Cells["D1"].Style.Font.Bold = true;
                ProgramsA.Cells["E1"].Style.Font.Bold = true;
                ProgramsA.Cells["F1"].Style.Font.Bold = true;
                ProgramsA.Cells["G1"].Style.Font.Bold = true;
                ProgramsA.Cells["H1"].Style.Font.Bold = true;
                ProgramsA.Cells["I1"].Style.Font.Bold = true;
                ProgramsA.Cells["J1"].Style.Font.Bold = true;
                ProgramsA.Cells["K1"].Style.Font.Bold = true;
                ProgramsA.Cells["L1"].Style.Font.Bold = true;
                ProgramsA.Cells["M1"].Style.Font.Bold = true;
                ProgramsA.Cells["N1"].Style.Font.Bold = true;
                ProgramsA.Cells["O1"].Style.Font.Bold = true;
                ProgramsA.Cells["P1"].Style.Font.Bold = true;
                ProgramsA.Cells["Q1"].Style.Font.Bold = true;
                ProgramsA.Cells["R1"].Style.Font.Bold = true;
                ProgramsA.Cells["S1"].Style.Font.Bold = true;
                ProgramsA.Cells["T1"].Style.Font.Bold = true;
                ProgramsA.Cells["U1"].Style.Font.Bold = true;
                ProgramsA.Cells["V1"].Style.Font.Bold = true;
                ProgramsA.Cells["W1"].Style.Font.Bold = true;
                ProgramsA.Cells["X1"].Style.Font.Bold = true;
                ProgramsA.Cells["Y1"].Style.Font.Bold = true;
                ProgramsA.Cells["Z1"].Style.Font.Bold = true;

                ProgramsI.Cells["A1"].Value = "#";
                ProgramsI.Cells["B1"].Value = "SKU";
                ProgramsI.Cells["C1"].Value = "Training Program Name";
                ProgramsI.Cells["D1"].Value = "Short Description";
                ProgramsI.Cells["E1"].Value = "Start Date 1";
                ProgramsI.Cells["F1"].Value = "End Date 1";
                ProgramsI.Cells["G1"].Value = "Discount Date 1";
                ProgramsI.Cells["H1"].Value = "Start Date 2";
                ProgramsI.Cells["I1"].Value = "End Date 2";
                ProgramsI.Cells["J1"].Value = "Discount Date 2";
                ProgramsI.Cells["K1"].Value = "First Payment";
                ProgramsI.Cells["L1"].Value = "Second Payment";
                ProgramsI.Cells["M1"].Value = "Total Payment";
                ProgramsI.Cells["N1"].Value = "Full Payment";
                ProgramsI.Cells["O1"].Value = "Limit Date Full Payment";
                ProgramsI.Cells["P1"].Value = "Coupon 1";
                ProgramsI.Cells["Q1"].Value = "Discount Coupon 1";
                ProgramsI.Cells["R1"].Value = "Coupon 2";
                ProgramsI.Cells["S1"].Value = "Discount Coupon 2";
                ProgramsI.Cells["T1"].Value = "Coupon 3";
                ProgramsI.Cells["U1"].Value = "Discount Coupon 3";
                ProgramsI.Cells["V1"].Value = "Coupon 4";
                ProgramsI.Cells["W1"].Value = "Discount Coupon 4";
                ProgramsI.Cells["X1"].Value = "Coupon 5";
                ProgramsI.Cells["Y1"].Value = "Discount Coupon 5";
                ProgramsI.Cells["Z1"].Value = "Active";

                ProgramsI.Cells["A1"].Style.Font.Bold = true;
                ProgramsI.Cells["B1"].Style.Font.Bold = true;
                ProgramsI.Cells["C1"].Style.Font.Bold = true;
                ProgramsI.Cells["D1"].Style.Font.Bold = true;
                ProgramsI.Cells["E1"].Style.Font.Bold = true;
                ProgramsI.Cells["F1"].Style.Font.Bold = true;
                ProgramsI.Cells["G1"].Style.Font.Bold = true;
                ProgramsI.Cells["H1"].Style.Font.Bold = true;
                ProgramsI.Cells["I1"].Style.Font.Bold = true;
                ProgramsI.Cells["J1"].Style.Font.Bold = true;
                ProgramsI.Cells["K1"].Style.Font.Bold = true;
                ProgramsI.Cells["L1"].Style.Font.Bold = true;
                ProgramsI.Cells["M1"].Style.Font.Bold = true;
                ProgramsI.Cells["N1"].Style.Font.Bold = true;
                ProgramsI.Cells["O1"].Style.Font.Bold = true;
                ProgramsI.Cells["P1"].Style.Font.Bold = true;
                ProgramsI.Cells["Q1"].Style.Font.Bold = true;
                ProgramsI.Cells["R1"].Style.Font.Bold = true;
                ProgramsI.Cells["S1"].Style.Font.Bold = true;
                ProgramsI.Cells["T1"].Style.Font.Bold = true;
                ProgramsI.Cells["U1"].Style.Font.Bold = true;
                ProgramsI.Cells["V1"].Style.Font.Bold = true;
                ProgramsI.Cells["W1"].Style.Font.Bold = true;
                ProgramsI.Cells["X1"].Style.Font.Bold = true;
                ProgramsI.Cells["Y1"].Style.Font.Bold = true;
                ProgramsI.Cells["Z1"].Style.Font.Bold = true;

                foreach (var item in data)
                {
                    var sdate1 = "N/A";
                    var sdate2 = "N/A";
                    var edate1 = "N/A";
                    var edate2 = "N/A";
                    var ldate = "N/A";
                    var active = "N/A";

                    if (item.StartDate1 != null)
                        sdate1 = Convert.ToDateTime(item.StartDate1).ToString("MM/dd/yyyy");

                    if (item.StartDate2 != null)
                        sdate2 = Convert.ToDateTime(item.StartDate2).ToString("MM/dd/yyyy");

                    if (item.EndDate1 != null)
                        edate1 = Convert.ToDateTime(item.EndDate1).ToString("MM/dd/yyyy");

                    if (item.EndDate2 != null)
                        edate2 = Convert.ToDateTime(item.EndDate2).ToString("MM/dd/yyyy");

                    if (item.LimitFullPaymentDate != null)
                        ldate = Convert.ToDateTime(item.LimitFullPaymentDate).ToString("MM/dd/yyyy");

                    if (item.IsActive == true)
                        active = "Yes";
                    else if (item.IsActive == false)
                        active = "No";

                    Programs.Cells[string.Format("A{0}", row)].Value = row - 1;
                    Programs.Cells[string.Format("B{0}", row)].Value = item.SKU;
                    Programs.Cells[string.Format("C{0}", row)].Value = item.TrainingCourseName;
                    Programs.Cells[string.Format("D{0}", row)].Value = item.ShortDescription;
                    Programs.Cells[string.Format("E{0}", row)].Value = sdate1;
                    Programs.Cells[string.Format("F{0}", row)].Value = edate1;
                    Programs.Cells[string.Format("G{0}", row)].Value = item.DiscountDate1+"%";
                    Programs.Cells[string.Format("H{0}", row)].Value = sdate2;
                    Programs.Cells[string.Format("I{0}", row)].Value = edate2;
                    Programs.Cells[string.Format("J{0}", row)].Value = item.DiscountDate2+"%";
                    Programs.Cells[string.Format("K{0}", row)].Value = item.FirstPayment;
                    Programs.Cells[string.Format("L{0}", row)].Value = item.SecondPayment;
                    Programs.Cells[string.Format("M{0}", row)].Value = item.Price;
                    Programs.Cells[string.Format("N{0}", row)].Value = item.PriceFullPayment;
                    Programs.Cells[string.Format("O{0}", row)].Value = ldate;
                    Programs.Cells[string.Format("P{0}", row)].Value = item.Coupon1;
                    Programs.Cells[string.Format("Q{0}", row)].Value = item.DiscountCoupon1 + "%";
                    Programs.Cells[string.Format("R{0}", row)].Value = item.Coupon2;
                    Programs.Cells[string.Format("S{0}", row)].Value = item.DiscountCoupon2 + "%";
                    Programs.Cells[string.Format("T{0}", row)].Value = item.Coupon3;
                    Programs.Cells[string.Format("U{0}", row)].Value = item.DiscountCoupon3 + "%";
                    Programs.Cells[string.Format("V{0}", row)].Value = item.Coupon4;
                    Programs.Cells[string.Format("W{0}", row)].Value = item.DiscountCoupon4 + "%";
                    Programs.Cells[string.Format("X{0}", row)].Value = item.Coupon5;
                    Programs.Cells[string.Format("Y{0}", row)].Value = item.DiscountCoupon5 + "%";
                    Programs.Cells[string.Format("Z{0}", row)].Value = active;

                    if (Convert.ToDateTime(item.StartDate1) > Convert.ToDateTime(xDate))
                    {
                        ProgramsA.Cells[string.Format("A{0}", rowA)].Value = rowA - 1;
                        ProgramsA.Cells[string.Format("B{0}", rowA)].Value = item.SKU;
                        ProgramsA.Cells[string.Format("C{0}", rowA)].Value = item.TrainingCourseName;
                        ProgramsA.Cells[string.Format("D{0}", rowA)].Value = item.ShortDescription;
                        ProgramsA.Cells[string.Format("E{0}", rowA)].Value = sdate1;
                        ProgramsA.Cells[string.Format("F{0}", rowA)].Value = edate1;
                        ProgramsA.Cells[string.Format("G{0}", rowA)].Value = item.DiscountDate1 + "%";
                        ProgramsA.Cells[string.Format("H{0}", rowA)].Value = sdate2;
                        ProgramsA.Cells[string.Format("I{0}", rowA)].Value = edate2;
                        ProgramsA.Cells[string.Format("J{0}", rowA)].Value = item.DiscountDate2 + "%";
                        ProgramsA.Cells[string.Format("K{0}", rowA)].Value = item.FirstPayment;
                        ProgramsA.Cells[string.Format("L{0}", rowA)].Value = item.SecondPayment;
                        ProgramsA.Cells[string.Format("M{0}", rowA)].Value = item.Price;
                        ProgramsA.Cells[string.Format("N{0}", rowA)].Value = item.PriceFullPayment;
                        ProgramsA.Cells[string.Format("O{0}", rowA)].Value = ldate;
                        ProgramsA.Cells[string.Format("P{0}", rowA)].Value = item.Coupon1;
                        ProgramsA.Cells[string.Format("Q{0}", rowA)].Value = item.DiscountCoupon1 + "%";
                        ProgramsA.Cells[string.Format("R{0}", rowA)].Value = item.Coupon2;
                        ProgramsA.Cells[string.Format("S{0}", rowA)].Value = item.DiscountCoupon2 + "%";
                        ProgramsA.Cells[string.Format("T{0}", rowA)].Value = item.Coupon3;
                        ProgramsA.Cells[string.Format("U{0}", rowA)].Value = item.DiscountCoupon3 + "%";
                        ProgramsA.Cells[string.Format("V{0}", rowA)].Value = item.Coupon4;
                        ProgramsA.Cells[string.Format("W{0}", rowA)].Value = item.DiscountCoupon4 + "%";
                        ProgramsA.Cells[string.Format("X{0}", rowA)].Value = item.Coupon5;
                        ProgramsA.Cells[string.Format("Y{0}", rowA)].Value = item.DiscountCoupon5 + "%";
                        ProgramsA.Cells[string.Format("Z{0}", rowA)].Value = active;

                        rowA++;

                    }

                    if (Convert.ToDateTime(item.StartDate1) < Convert.ToDateTime(xDate))
                    {
                        ProgramsI.Cells[string.Format("A{0}", rowI)].Value = rowI - 1;
                        ProgramsI.Cells[string.Format("B{0}", rowI)].Value = item.SKU;
                        ProgramsI.Cells[string.Format("C{0}", rowI)].Value = item.TrainingCourseName;
                        ProgramsI.Cells[string.Format("D{0}", rowI)].Value = item.ShortDescription;
                        ProgramsI.Cells[string.Format("E{0}", rowI)].Value = sdate1;
                        ProgramsI.Cells[string.Format("F{0}", rowI)].Value = edate1;
                        ProgramsI.Cells[string.Format("G{0}", rowI)].Value = item.DiscountDate1 + "%";
                        ProgramsI.Cells[string.Format("H{0}", rowI)].Value = sdate2;
                        ProgramsI.Cells[string.Format("I{0}", rowI)].Value = edate2;
                        ProgramsI.Cells[string.Format("J{0}", rowI)].Value = item.DiscountDate2 + "%";
                        ProgramsI.Cells[string.Format("K{0}", rowI)].Value = item.FirstPayment;
                        ProgramsI.Cells[string.Format("L{0}", rowI)].Value = item.SecondPayment;
                        ProgramsI.Cells[string.Format("M{0}", rowI)].Value = item.Price;
                        ProgramsI.Cells[string.Format("N{0}", rowI)].Value = item.PriceFullPayment;
                        ProgramsI.Cells[string.Format("O{0}", rowI)].Value = ldate;
                        ProgramsI.Cells[string.Format("P{0}", rowI)].Value = item.Coupon1;
                        ProgramsI.Cells[string.Format("Q{0}", rowI)].Value = item.DiscountCoupon1 + "%";
                        ProgramsI.Cells[string.Format("R{0}", rowI)].Value = item.Coupon2;
                        ProgramsI.Cells[string.Format("S{0}", rowI)].Value = item.DiscountCoupon2 + "%";
                        ProgramsI.Cells[string.Format("T{0}", rowI)].Value = item.Coupon3;
                        ProgramsI.Cells[string.Format("U{0}", rowI)].Value = item.DiscountCoupon3 + "%";
                        ProgramsI.Cells[string.Format("V{0}", rowI)].Value = item.Coupon4;
                        ProgramsI.Cells[string.Format("W{0}", rowI)].Value = item.DiscountCoupon4 + "%";
                        ProgramsI.Cells[string.Format("X{0}", rowI)].Value = item.Coupon5;
                        ProgramsI.Cells[string.Format("Y{0}", rowI)].Value = item.DiscountCoupon5 + "%";
                        ProgramsI.Cells[string.Format("Z{0}", rowI)].Value = active;

                        rowI++;
                    }
                    row++;
                }

                Programs.Cells["A:AZ"].AutoFitColumns();
                ProgramsA.Cells["A:AZ"].AutoFitColumns();
                ProgramsI.Cells["A:AZ"].AutoFitColumns();
                Response.ClearContent();

                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=TrainingPrograms_" + DateTime.Parse(xDate).ToShortDateString() + "_Report.xlsx");
                Response.ContentType = "application/ms-excel";
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
            }

        }

        [HttpGet]
        public void TrainingProgramUsersExcel(int? id)
        {

            List<WebsiteUserTraining> data = db.WebsiteUserTrainings.Where(x => x.TrainingCourseID ==  id).OrderBy(x => x.WebsiteUser.FName).OrderByDescending(x => x.WebsiteUserTrainingID).ToList();

            data = data.GroupBy(x => x.WebsiteUserID).Select(g => g.First()).OrderBy(x => x.WebsiteUser.FName).ToList();

            int rowAll = 2;
            int rowFull = 2;
            int rowF = 2;
            int rowS = 2;
            int rowN = 2;
            //if(data.Count() > 0  )
            {
                var xDate = DateTime.Now.ToShortDateString();
                //var pName = "";

                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet TrainingUsers = Ep.Workbook.Worksheets.Add("All Enrolled Users");
                ExcelWorksheet TrainingUsersFull = Ep.Workbook.Worksheets.Add("Paid in Full");
                ExcelWorksheet TrainingUsersF = Ep.Workbook.Worksheets.Add("1st Payment");
                ExcelWorksheet TrainingUsersS = Ep.Workbook.Worksheets.Add("2nd Payment");
                ExcelWorksheet TrainingUsersN = Ep.Workbook.Worksheets.Add("Not Paid");

                TrainingUsers.Cells["A1"].Value = "#";
                TrainingUsers.Cells["B1"].Value = "First Name";
                TrainingUsers.Cells["C1"].Value = "Last Name";
                TrainingUsers.Cells["D1"].Value = "Phone Number";
                TrainingUsers.Cells["E1"].Value = "Email";
                TrainingUsers.Cells["F1"].Value = "SkypeID";
                TrainingUsers.Cells["G1"].Value = "How did you hear about us?";
                TrainingUsers.Cells["H1"].Value = "Why are you interested in Brightrace?";
                TrainingUsers.Cells["I1"].Value = "Accept Privacy Policy";
                TrainingUsers.Cells["J1"].Value = "Receive Updates";
                TrainingUsers.Cells["K1"].Value = "Registration Date";
                TrainingUsers.Cells["L1"].Value = "Active";
                TrainingUsers.Cells["M1"].Value = "Payment Status";
                TrainingUsers.Cells["N1"].Value = "Enrolled Date";

                TrainingUsersFull.Cells["A1"].Value = "#";
                TrainingUsersFull.Cells["B1"].Value = "First Name";
                TrainingUsersFull.Cells["C1"].Value = "Last Name";
                TrainingUsersFull.Cells["D1"].Value = "Phone Number";
                TrainingUsersFull.Cells["E1"].Value = "Email";
                TrainingUsersFull.Cells["F1"].Value = "SkypeID";
                TrainingUsersFull.Cells["G1"].Value = "How did you hear about us?";
                TrainingUsersFull.Cells["H1"].Value = "Why are you interested in Brightrace?";
                TrainingUsersFull.Cells["I1"].Value = "Accept Privacy Policy";
                TrainingUsersFull.Cells["J1"].Value = "Receive Updates";
                TrainingUsersFull.Cells["K1"].Value = "Registration Date";
                TrainingUsersFull.Cells["L1"].Value = "Active";
                TrainingUsersFull.Cells["M1"].Value = "Payment Status";
                TrainingUsersFull.Cells["N1"].Value = "Enrolled Date";

                TrainingUsersF.Cells["A1"].Value = "#";
                TrainingUsersF.Cells["B1"].Value = "First Name";
                TrainingUsersF.Cells["C1"].Value = "Last Name";
                TrainingUsersF.Cells["D1"].Value = "Phone Number";
                TrainingUsersF.Cells["E1"].Value = "Email";
                TrainingUsersF.Cells["F1"].Value = "SkypeID";
                TrainingUsersF.Cells["G1"].Value = "How did you hear about us?";
                TrainingUsersF.Cells["H1"].Value = "Why are you interested in Brightrace?";
                TrainingUsersF.Cells["I1"].Value = "Accept Privacy Policy";
                TrainingUsersF.Cells["J1"].Value = "Receive Updates";
                TrainingUsersF.Cells["K1"].Value = "Registration Date";
                TrainingUsersF.Cells["L1"].Value = "Active";
                TrainingUsersF.Cells["M1"].Value = "Payment Status";
                TrainingUsersF.Cells["N1"].Value = "Enrolled Date";

                TrainingUsersS.Cells["A1"].Value = "#";
                TrainingUsersS.Cells["B1"].Value = "First Name";
                TrainingUsersS.Cells["C1"].Value = "Last Name";
                TrainingUsersS.Cells["D1"].Value = "Phone Number";
                TrainingUsersS.Cells["E1"].Value = "Email";
                TrainingUsersS.Cells["F1"].Value = "SkypeID";
                TrainingUsersS.Cells["G1"].Value = "How did you hear about us?";
                TrainingUsersS.Cells["H1"].Value = "Why are you interested in Brightrace?";
                TrainingUsersS.Cells["I1"].Value = "Accept Privacy Policy";
                TrainingUsersS.Cells["J1"].Value = "Receive Updates";
                TrainingUsersS.Cells["K1"].Value = "Registration Date";
                TrainingUsersS.Cells["L1"].Value = "Active";
                TrainingUsersS.Cells["M1"].Value = "Payment Status";
                TrainingUsersS.Cells["N1"].Value = "Enrolled Date";

                TrainingUsersN.Cells["A1"].Value = "#";
                TrainingUsersN.Cells["B1"].Value = "First Name";
                TrainingUsersN.Cells["C1"].Value = "Last Name";
                TrainingUsersN.Cells["D1"].Value = "Phone Number";
                TrainingUsersN.Cells["E1"].Value = "Email";
                TrainingUsersN.Cells["F1"].Value = "SkypeID";
                TrainingUsersN.Cells["G1"].Value = "How did you hear about us?";
                TrainingUsersN.Cells["H1"].Value = "Why are you interested in Brightrace?";
                TrainingUsersN.Cells["I1"].Value = "Accept Privacy Policy";
                TrainingUsersN.Cells["J1"].Value = "Receive Updates";
                TrainingUsersN.Cells["K1"].Value = "Registration Date";
                TrainingUsersN.Cells["L1"].Value = "Active";
                TrainingUsersN.Cells["M1"].Value = "Payment Status";
                TrainingUsersN.Cells["N1"].Value = "Enrolled Date";

                TrainingUsers.Cells["A1"].Style.Font.Bold = true;
                TrainingUsers.Cells["B1"].Style.Font.Bold = true;
                TrainingUsers.Cells["C1"].Style.Font.Bold = true;
                TrainingUsers.Cells["D1"].Style.Font.Bold = true;
                TrainingUsers.Cells["E1"].Style.Font.Bold = true;
                TrainingUsers.Cells["F1"].Style.Font.Bold = true;
                TrainingUsers.Cells["G1"].Style.Font.Bold = true;
                TrainingUsers.Cells["H1"].Style.Font.Bold = true;
                TrainingUsers.Cells["I1"].Style.Font.Bold = true;
                TrainingUsers.Cells["J1"].Style.Font.Bold = true;
                TrainingUsers.Cells["K1"].Style.Font.Bold = true;
                TrainingUsers.Cells["L1"].Style.Font.Bold = true;
                TrainingUsers.Cells["M1"].Style.Font.Bold = true;
                TrainingUsers.Cells["N1"].Style.Font.Bold = true;

                TrainingUsersFull.Cells["A1"].Style.Font.Bold = true;
                TrainingUsersFull.Cells["B1"].Style.Font.Bold = true;
                TrainingUsersFull.Cells["C1"].Style.Font.Bold = true;
                TrainingUsersFull.Cells["D1"].Style.Font.Bold = true;
                TrainingUsersFull.Cells["E1"].Style.Font.Bold = true;
                TrainingUsersFull.Cells["F1"].Style.Font.Bold = true;
                TrainingUsersFull.Cells["G1"].Style.Font.Bold = true;
                TrainingUsersFull.Cells["H1"].Style.Font.Bold = true;
                TrainingUsersFull.Cells["I1"].Style.Font.Bold = true;
                TrainingUsersFull.Cells["J1"].Style.Font.Bold = true;
                TrainingUsersFull.Cells["K1"].Style.Font.Bold = true;
                TrainingUsersFull.Cells["L1"].Style.Font.Bold = true;
                TrainingUsersFull.Cells["M1"].Style.Font.Bold = true;
                TrainingUsersFull.Cells["N1"].Style.Font.Bold = true;

                TrainingUsersF.Cells["A1"].Style.Font.Bold = true;
                TrainingUsersF.Cells["B1"].Style.Font.Bold = true;
                TrainingUsersF.Cells["C1"].Style.Font.Bold = true;
                TrainingUsersF.Cells["D1"].Style.Font.Bold = true;
                TrainingUsersF.Cells["E1"].Style.Font.Bold = true;
                TrainingUsersF.Cells["F1"].Style.Font.Bold = true;
                TrainingUsersF.Cells["G1"].Style.Font.Bold = true;
                TrainingUsersF.Cells["H1"].Style.Font.Bold = true;
                TrainingUsersF.Cells["I1"].Style.Font.Bold = true;
                TrainingUsersF.Cells["J1"].Style.Font.Bold = true;
                TrainingUsersF.Cells["K1"].Style.Font.Bold = true;
                TrainingUsersF.Cells["L1"].Style.Font.Bold = true;
                TrainingUsersF.Cells["M1"].Style.Font.Bold = true;
                TrainingUsersF.Cells["N1"].Style.Font.Bold = true;

                TrainingUsersS.Cells["A1"].Style.Font.Bold = true;
                TrainingUsersS.Cells["B1"].Style.Font.Bold = true;
                TrainingUsersS.Cells["C1"].Style.Font.Bold = true;
                TrainingUsersS.Cells["D1"].Style.Font.Bold = true;
                TrainingUsersS.Cells["E1"].Style.Font.Bold = true;
                TrainingUsersS.Cells["F1"].Style.Font.Bold = true;
                TrainingUsersS.Cells["G1"].Style.Font.Bold = true;
                TrainingUsersS.Cells["H1"].Style.Font.Bold = true;
                TrainingUsersS.Cells["I1"].Style.Font.Bold = true;
                TrainingUsersS.Cells["J1"].Style.Font.Bold = true;
                TrainingUsersS.Cells["K1"].Style.Font.Bold = true;
                TrainingUsersS.Cells["L1"].Style.Font.Bold = true;
                TrainingUsersS.Cells["M1"].Style.Font.Bold = true;
                TrainingUsersS.Cells["N1"].Style.Font.Bold = true;

                TrainingUsersN.Cells["A1"].Style.Font.Bold = true;
                TrainingUsersN.Cells["B1"].Style.Font.Bold = true;
                TrainingUsersN.Cells["C1"].Style.Font.Bold = true;
                TrainingUsersN.Cells["D1"].Style.Font.Bold = true;
                TrainingUsersN.Cells["E1"].Style.Font.Bold = true;
                TrainingUsersN.Cells["F1"].Style.Font.Bold = true;
                TrainingUsersN.Cells["G1"].Style.Font.Bold = true;
                TrainingUsersN.Cells["H1"].Style.Font.Bold = true;
                TrainingUsersN.Cells["I1"].Style.Font.Bold = true;
                TrainingUsersN.Cells["J1"].Style.Font.Bold = true;
                TrainingUsersN.Cells["K1"].Style.Font.Bold = true;
                TrainingUsersN.Cells["L1"].Style.Font.Bold = true;
                TrainingUsersN.Cells["M1"].Style.Font.Bold = true;
                TrainingUsersN.Cells["N1"].Style.Font.Bold = true;

                foreach (var item in data)
                {
                    var privacy = "N/A";
                    var updates = "N/A";
                    var active = "N/A";
                    var date = "N/A";
                    var edate = "N/A";
                    string payment = item.PaymentStatus;
                    //pName = item.TrainingCours.TrainingCourseName;

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

                    if (item.PaymentDate != null)
                        edate = Convert.ToDateTime(item.PaymentDate).ToString("MM/dd/yyyy");

                    if (item.PaymentType == 1)
                    {
                        payment = "Paid Registration Fee";
                    }

                    else if (item.PaymentType == 2)
                    {
                        payment = "Paid Last installment";
                    }

                    else if (item.PaymentType == 3)
                    {
                        payment = "Paid in Full";
                    }

                    else if (item.PaymentType == 4)
                    {
                        payment = "Not Paid";
                    }

                    else if (payment.Contains("Free"))
                    {
                        payment = "Not Paid";
                    }


                    TrainingUsers.Cells[string.Format("A{0}", rowAll)].Value = rowAll - 1;
                    TrainingUsers.Cells[string.Format("B{0}", rowAll)].Value = item.WebsiteUser.FName;
                    TrainingUsers.Cells[string.Format("C{0}", rowAll)].Value = item.WebsiteUser.LName;
                    TrainingUsers.Cells[string.Format("D{0}", rowAll)].Value = item.WebsiteUser.Phone;
                    TrainingUsers.Cells[string.Format("E{0}", rowAll)].Value = item.WebsiteUser.Email;
                    TrainingUsers.Cells[string.Format("F{0}", rowAll)].Value = item.WebsiteUser.SkypeID;
                    TrainingUsers.Cells[string.Format("G{0}", rowAll)].Value = item.WebsiteUser.AboutUs;
                    TrainingUsers.Cells[string.Format("H{0}", rowAll)].Value = item.WebsiteUser.WhyInterested;
                    TrainingUsers.Cells[string.Format("I{0}", rowAll)].Value = privacy;
                    TrainingUsers.Cells[string.Format("J{0}", rowAll)].Value = updates;
                    TrainingUsers.Cells[string.Format("K{0}", rowAll)].Value = date;
                    TrainingUsers.Cells[string.Format("L{0}", rowAll)].Value = active;
                    TrainingUsers.Cells[string.Format("M{0}", rowAll)].Value = payment;
                    TrainingUsers.Cells[string.Format("N{0}", rowAll)].Value = edate;

                    rowAll++;

                    if (item.PaymentType == 3)
                    {
                        TrainingUsersFull.Cells[string.Format("A{0}", rowFull)].Value = rowFull - 1;
                        TrainingUsersFull.Cells[string.Format("B{0}", rowFull)].Value = item.WebsiteUser.FName;
                        TrainingUsersFull.Cells[string.Format("C{0}", rowFull)].Value = item.WebsiteUser.LName;
                        TrainingUsersFull.Cells[string.Format("D{0}", rowFull)].Value = item.WebsiteUser.Phone;
                        TrainingUsersFull.Cells[string.Format("E{0}", rowFull)].Value = item.WebsiteUser.Email;
                        TrainingUsersFull.Cells[string.Format("F{0}", rowFull)].Value = item.WebsiteUser.SkypeID;
                        TrainingUsersFull.Cells[string.Format("G{0}", rowFull)].Value = item.WebsiteUser.AboutUs;
                        TrainingUsersFull.Cells[string.Format("H{0}", rowFull)].Value = item.WebsiteUser.WhyInterested;
                        TrainingUsersFull.Cells[string.Format("I{0}", rowFull)].Value = privacy;
                        TrainingUsersFull.Cells[string.Format("J{0}", rowFull)].Value = updates;
                        TrainingUsersFull.Cells[string.Format("K{0}", rowFull)].Value = date;
                        TrainingUsersFull.Cells[string.Format("L{0}", rowFull)].Value = active;
                        TrainingUsersFull.Cells[string.Format("M{0}", rowFull)].Value = payment;
                        TrainingUsersFull.Cells[string.Format("N{0}", rowFull)].Value = edate;

                        rowFull++;
                    }

                    if (item.PaymentType == 1)
                    {
                        TrainingUsersF.Cells[string.Format("A{0}", rowF)].Value = rowF - 1;
                        TrainingUsersF.Cells[string.Format("B{0}", rowF)].Value = item.WebsiteUser.FName;
                        TrainingUsersF.Cells[string.Format("C{0}", rowF)].Value = item.WebsiteUser.LName;
                        TrainingUsersF.Cells[string.Format("D{0}", rowF)].Value = item.WebsiteUser.Phone;
                        TrainingUsersF.Cells[string.Format("E{0}", rowF)].Value = item.WebsiteUser.Email;
                        TrainingUsersF.Cells[string.Format("F{0}", rowF)].Value = item.WebsiteUser.SkypeID;
                        TrainingUsersF.Cells[string.Format("G{0}", rowF)].Value = item.WebsiteUser.AboutUs;
                        TrainingUsersF.Cells[string.Format("H{0}", rowF)].Value = item.WebsiteUser.WhyInterested;
                        TrainingUsersF.Cells[string.Format("I{0}", rowF)].Value = privacy;
                        TrainingUsersF.Cells[string.Format("J{0}", rowF)].Value = updates;
                        TrainingUsersF.Cells[string.Format("K{0}", rowF)].Value = date;
                        TrainingUsersF.Cells[string.Format("L{0}", rowF)].Value = active;
                        TrainingUsersF.Cells[string.Format("M{0}", rowF)].Value = payment;
                        TrainingUsersF.Cells[string.Format("N{0}", rowF)].Value = edate;

                        rowF++;
                    }

                    if (item.PaymentType == 2)
                    {
                        TrainingUsersS.Cells[string.Format("A{0}", rowS)].Value = rowS - 1;
                        TrainingUsersS.Cells[string.Format("B{0}", rowS)].Value = item.WebsiteUser.FName;
                        TrainingUsersS.Cells[string.Format("C{0}", rowS)].Value = item.WebsiteUser.LName;
                        TrainingUsersS.Cells[string.Format("D{0}", rowS)].Value = item.WebsiteUser.Phone;
                        TrainingUsersS.Cells[string.Format("E{0}", rowS)].Value = item.WebsiteUser.Email;
                        TrainingUsersS.Cells[string.Format("F{0}", rowS)].Value = item.WebsiteUser.SkypeID;
                        TrainingUsersS.Cells[string.Format("G{0}", rowS)].Value = item.WebsiteUser.AboutUs;
                        TrainingUsersS.Cells[string.Format("H{0}", rowS)].Value = item.WebsiteUser.WhyInterested;
                        TrainingUsersS.Cells[string.Format("I{0}", rowS)].Value = privacy;
                        TrainingUsersS.Cells[string.Format("J{0}", rowS)].Value = updates;
                        TrainingUsersS.Cells[string.Format("K{0}", rowS)].Value = date;
                        TrainingUsersS.Cells[string.Format("L{0}", rowS)].Value = active;
                        TrainingUsersS.Cells[string.Format("M{0}", rowS)].Value = payment;
                        TrainingUsersS.Cells[string.Format("N{0}", rowS)].Value = edate;

                        rowS++;
                    }

                    if (item.PaymentType == 4)
                    {
                        TrainingUsersN.Cells[string.Format("A{0}", rowN)].Value = rowN - 1;
                        TrainingUsersN.Cells[string.Format("B{0}", rowN)].Value = item.WebsiteUser.FName;
                        TrainingUsersN.Cells[string.Format("C{0}", rowN)].Value = item.WebsiteUser.LName;
                        TrainingUsersN.Cells[string.Format("D{0}", rowN)].Value = item.WebsiteUser.Phone;
                        TrainingUsersN.Cells[string.Format("E{0}", rowN)].Value = item.WebsiteUser.Email;
                        TrainingUsersN.Cells[string.Format("F{0}", rowN)].Value = item.WebsiteUser.SkypeID;
                        TrainingUsersN.Cells[string.Format("G{0}", rowN)].Value = item.WebsiteUser.AboutUs;
                        TrainingUsersN.Cells[string.Format("H{0}", rowN)].Value = item.WebsiteUser.WhyInterested;
                        TrainingUsersN.Cells[string.Format("I{0}", rowN)].Value = privacy;
                        TrainingUsersN.Cells[string.Format("J{0}", rowN)].Value = updates;
                        TrainingUsersN.Cells[string.Format("K{0}", rowN)].Value = date;
                        TrainingUsersN.Cells[string.Format("L{0}", rowN)].Value = active;
                        TrainingUsersN.Cells[string.Format("M{0}", rowN)].Value = payment;
                        TrainingUsersN.Cells[string.Format("N{0}", rowN)].Value = edate;

                        rowN++;
                    }

                    
                }

                TrainingUsers.Cells["A:AZ"].AutoFitColumns();
                Response.ClearContent();

                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=TraininProgramUsers_"+ DateTime.Parse(xDate).ToShortDateString() + "_Report.xlsx");
                Response.ContentType = "application/ms-excel";
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
            }
        }

        [HttpGet]
        public void RegularTrainingProgramUsersExcel(int? id)
        {
            List<WebsiteUserTraining> data = db.WebsiteUserTrainings.Where(x => x.TrainingCourseID == id).OrderBy(x => x.WebsiteUser.FName).OrderByDescending(x => x.WebsiteUserTrainingID).ToList();

            data = data.GroupBy(x => x.WebsiteUserID).Select(g => g.First()).OrderBy(x => x.WebsiteUser.FName).ToList();

            int rowAll = 2;
            int rowAttend = 2;
            int rowPass = 2;
            //if(data.Count() > 0  )
            {
                var xDate = DateTime.Now.ToShortDateString();
                //var pName = "";

                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet TrainingUsers = Ep.Workbook.Worksheets.Add("All Enrolled Users");
                ExcelWorksheet TrainingUsersAttend = Ep.Workbook.Worksheets.Add("Appeared");
                ExcelWorksheet TrainingUsersPass = Ep.Workbook.Worksheets.Add("Passed");
                

                TrainingUsers.Cells["A1"].Value = "#";
                TrainingUsers.Cells["B1"].Value = "First Name";
                TrainingUsers.Cells["C1"].Value = "Last Name";
                TrainingUsers.Cells["D1"].Value = "Phone Number";
                TrainingUsers.Cells["E1"].Value = "Email";
                TrainingUsers.Cells["F1"].Value = "SkypeID";
                TrainingUsers.Cells["G1"].Value = "How did you hear about us?";
                TrainingUsers.Cells["H1"].Value = "Why are you interested in Brightrace?";
                TrainingUsers.Cells["I1"].Value = "Payment Status";
                TrainingUsers.Cells["J1"].Value = "Enrolled Date";
                TrainingUsers.Cells["K1"].Value = "Highest education";
                TrainingUsers.Cells["L1"].Value = "Completion year";
                TrainingUsers.Cells["M1"].Value = "Experience (IT)";
                TrainingUsers.Cells["N1"].Value = "Experience (Non-IT)";
                TrainingUsers.Cells["O1"].Value = "Currently Employed?";
                TrainingUsers.Cells["P1"].Value = "Authorized to work in the US?";
                TrainingUsers.Cells["Q1"].Value = "Visa status";
                TrainingUsers.Cells["R1"].Value = "Address";
                TrainingUsers.Cells["S1"].Value = "City";
                TrainingUsers.Cells["T1"].Value = "State";
                TrainingUsers.Cells["U1"].Value = "ZIP Code";
                TrainingUsers.Cells["V1"].Value = "Open to relocate";
                TrainingUsers.Cells["W1"].Value = "Recruiter";
                TrainingUsers.Cells["X1"].Value = "Appear on Evaluation";
                TrainingUsers.Cells["Y1"].Value = "Pass/Fail Evaluation	";
                TrainingUsers.Cells["Z1"].Value = "Signed Contract";


                TrainingUsers.Cells["A1"].Style.Font.Bold = true;
                TrainingUsers.Cells["B1"].Style.Font.Bold = true;
                TrainingUsers.Cells["C1"].Style.Font.Bold = true;
                TrainingUsers.Cells["D1"].Style.Font.Bold = true;
                TrainingUsers.Cells["E1"].Style.Font.Bold = true;
                TrainingUsers.Cells["F1"].Style.Font.Bold = true;
                TrainingUsers.Cells["G1"].Style.Font.Bold = true;
                TrainingUsers.Cells["H1"].Style.Font.Bold = true;
                TrainingUsers.Cells["I1"].Style.Font.Bold = true;
                TrainingUsers.Cells["J1"].Style.Font.Bold = true;
                TrainingUsers.Cells["K1"].Style.Font.Bold = true;
                TrainingUsers.Cells["L1"].Style.Font.Bold = true;
                TrainingUsers.Cells["M1"].Style.Font.Bold = true;
                TrainingUsers.Cells["N1"].Style.Font.Bold = true;
                TrainingUsers.Cells["O1"].Style.Font.Bold = true;
                TrainingUsers.Cells["P1"].Style.Font.Bold = true;
                TrainingUsers.Cells["Q1"].Style.Font.Bold = true;
                TrainingUsers.Cells["R1"].Style.Font.Bold = true;
                TrainingUsers.Cells["S1"].Style.Font.Bold = true;
                TrainingUsers.Cells["T1"].Style.Font.Bold = true;
                TrainingUsers.Cells["U1"].Style.Font.Bold = true;
                TrainingUsers.Cells["V1"].Style.Font.Bold = true;
                TrainingUsers.Cells["W1"].Style.Font.Bold = true;
                TrainingUsers.Cells["X1"].Style.Font.Bold = true;
                TrainingUsers.Cells["Y1"].Style.Font.Bold = true;
                TrainingUsers.Cells["Z1"].Style.Font.Bold = true;


                TrainingUsersAttend.Cells["A1"].Value = "#";
                TrainingUsersAttend.Cells["B1"].Value = "First Name";
                TrainingUsersAttend.Cells["C1"].Value = "Last Name";
                TrainingUsersAttend.Cells["D1"].Value = "Phone Number";
                TrainingUsersAttend.Cells["E1"].Value = "Email";
                TrainingUsersAttend.Cells["F1"].Value = "SkypeID";
                TrainingUsersAttend.Cells["G1"].Value = "How did you hear about us?";
                TrainingUsersAttend.Cells["H1"].Value = "Why are you interested in Brightrace?";
                TrainingUsersAttend.Cells["I1"].Value = "Payment Status";
                TrainingUsersAttend.Cells["J1"].Value = "Enrolled Date";
                TrainingUsersAttend.Cells["K1"].Value = "Highest education";
                TrainingUsersAttend.Cells["L1"].Value = "Completion year";
                TrainingUsersAttend.Cells["M1"].Value = "Experience (IT)";
                TrainingUsersAttend.Cells["N1"].Value = "Experience (Non-IT)";
                TrainingUsersAttend.Cells["O1"].Value = "Currently Employed?";
                TrainingUsersAttend.Cells["P1"].Value = "Authorized to work in the US?";
                TrainingUsersAttend.Cells["Q1"].Value = "Visa status";
                TrainingUsersAttend.Cells["R1"].Value = "Address";
                TrainingUsersAttend.Cells["S1"].Value = "City";
                TrainingUsersAttend.Cells["T1"].Value = "State";
                TrainingUsersAttend.Cells["U1"].Value = "ZIP Code";
                TrainingUsersAttend.Cells["V1"].Value = "Open to relocate";
                TrainingUsersAttend.Cells["W1"].Value = "Recruiter";
                TrainingUsersAttend.Cells["X1"].Value = "Appear on Evaluation";
                TrainingUsersAttend.Cells["Y1"].Value = "Pass/Fail Evaluation	";
                TrainingUsersAttend.Cells["Z1"].Value = "Signed Contract";

                TrainingUsersAttend.Cells["A1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["B1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["C1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["D1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["E1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["F1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["G1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["H1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["I1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["J1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["K1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["L1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["M1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["N1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["O1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["P1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["Q1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["R1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["S1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["T1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["U1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["V1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["W1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["X1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["Y1"].Style.Font.Bold = true;
                TrainingUsersAttend.Cells["Z1"].Style.Font.Bold = true;

                TrainingUsersPass.Cells["A1"].Value = "#";
                TrainingUsersPass.Cells["B1"].Value = "First Name";
                TrainingUsersPass.Cells["C1"].Value = "Last Name";
                TrainingUsersPass.Cells["D1"].Value = "Phone Number";
                TrainingUsersPass.Cells["E1"].Value = "Email";
                TrainingUsersPass.Cells["F1"].Value = "SkypeID";
                TrainingUsersPass.Cells["G1"].Value = "How did you hear about us?";
                TrainingUsersPass.Cells["H1"].Value = "Why are you interested in Brightrace?";
                TrainingUsersPass.Cells["I1"].Value = "Payment Status";
                TrainingUsersPass.Cells["J1"].Value = "Enrolled Date";
                TrainingUsersPass.Cells["K1"].Value = "Highest education";
                TrainingUsersPass.Cells["L1"].Value = "Completion year";
                TrainingUsersPass.Cells["M1"].Value = "Experience (IT)";
                TrainingUsersPass.Cells["N1"].Value = "Experience (Non-IT)";
                TrainingUsersPass.Cells["O1"].Value = "Currently Employed?";
                TrainingUsersPass.Cells["P1"].Value = "Authorized to work in the US?";
                TrainingUsersPass.Cells["Q1"].Value = "Visa status";
                TrainingUsersPass.Cells["R1"].Value = "Address";
                TrainingUsersPass.Cells["S1"].Value = "City";
                TrainingUsersPass.Cells["T1"].Value = "State";
                TrainingUsersPass.Cells["U1"].Value = "ZIP Code";
                TrainingUsersPass.Cells["V1"].Value = "Open to relocate";
                TrainingUsersPass.Cells["W1"].Value = "Recruiter";
                TrainingUsersPass.Cells["X1"].Value = "Appear on Evaluation";
                TrainingUsersPass.Cells["Y1"].Value = "Pass/Fail Evaluation	";
                TrainingUsersPass.Cells["Z1"].Value = "Signed Contract";

                TrainingUsersPass.Cells["A1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["B1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["C1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["D1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["E1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["F1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["G1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["H1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["I1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["J1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["K1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["L1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["M1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["N1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["O1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["P1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["Q1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["R1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["S1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["T1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["U1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["V1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["W1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["X1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["Y1"].Style.Font.Bold = true;
                TrainingUsersPass.Cells["Z1"].Style.Font.Bold = true;

                foreach (var item in data)
                {
                    var edate = "N/A";
                    string payment = item.PaymentStatus;
                    string take = "No";
                    string evalStatus = "Failed";
                    string contract = "Not Signed";
                    //pName = item.TrainingCours.TrainingCourseName;

                    if (item.PaymentDate != null)
                        edate = Convert.ToDateTime(item.PaymentDate).ToString("MM/dd/yyyy");

                    if (item.PaymentType == 1)
                    {
                        payment = "Paid Registration Fee";
                    }

                    else if (item.PaymentType == 2)
                    {
                        payment = "Paid Last installment";
                    }

                    else if (item.PaymentType == 3)
                    {
                        payment = "Paid in Full";
                    }

                    else if (item.PaymentType == 4)
                    {
                        payment = "Not Paid";
                    }

                    else if (item.PaymentType == 5)
                    {
                        payment = "NPR";
                    }

                    else if (payment.Contains("Free"))
                    {
                        payment = "Not Paid";
                    }

                    if(item.WebsiteUser.TakeEvaluation == true)
                    {
                        take = "Yes";
                    }

                    if (item.WebsiteUser.EvaluationStatus == true)
                    {
                        evalStatus = "Passed";
                    }

                    if (item.WebsiteUser.SignedContract == true)
                    {
                        contract = "Signed";
                    }


                    TrainingUsers.Cells[string.Format("A{0}", rowAll)].Value = rowAll - 1;
                    TrainingUsers.Cells[string.Format("B{0}", rowAll)].Value = item.WebsiteUser.FName;
                    TrainingUsers.Cells[string.Format("C{0}", rowAll)].Value = item.WebsiteUser.LName;
                    TrainingUsers.Cells[string.Format("D{0}", rowAll)].Value = item.WebsiteUser.Phone;
                    TrainingUsers.Cells[string.Format("E{0}", rowAll)].Value = item.WebsiteUser.Email;
                    TrainingUsers.Cells[string.Format("F{0}", rowAll)].Value = item.WebsiteUser.SkypeID;
                    TrainingUsers.Cells[string.Format("G{0}", rowAll)].Value = item.WebsiteUser.AboutUs;
                    TrainingUsers.Cells[string.Format("H{0}", rowAll)].Value = item.WebsiteUser.WhyInterested;
                    TrainingUsers.Cells[string.Format("I{0}", rowAll)].Value = payment;
                    TrainingUsers.Cells[string.Format("J{0}", rowAll)].Value = edate;

                    foreach (var item2 in item.WebsiteUser.WebsiteUserDetails)
                    {
                        var employed = "No";
                        var authorized = "No";
                        var relocate = "No";

                        if (item2.CurrentlyEmployed == 1)
                            employed = "Yes";

                        if (item2.AuthorizedWorkUSA == 1)
                            authorized = "Yes";

                        if (item2.OpenToRelocate == 1)
                            relocate = "Yes";

                        TrainingUsers.Cells[string.Format("K{0}", rowAll)].Value = item2.HighestEducation;
                        TrainingUsers.Cells[string.Format("L{0}", rowAll)].Value = item2.CompletionYear;
                        TrainingUsers.Cells[string.Format("M{0}", rowAll)].Value = item2.WorkExperienceIT +" year(s)";
                        TrainingUsers.Cells[string.Format("N{0}", rowAll)].Value = item2.WorkExperienceNonIT + " year(s)";
                        TrainingUsers.Cells[string.Format("O{0}", rowAll)].Value = employed;
                        TrainingUsers.Cells[string.Format("P{0}", rowAll)].Value = authorized;
                        TrainingUsers.Cells[string.Format("Q{0}", rowAll)].Value = item2.VisaStatus;
                        TrainingUsers.Cells[string.Format("R{0}", rowAll)].Value = item2.Address;
                        TrainingUsers.Cells[string.Format("S{0}", rowAll)].Value = item2.City;
                        TrainingUsers.Cells[string.Format("T{0}", rowAll)].Value = item2.CountryState.State;
                        TrainingUsers.Cells[string.Format("U{0}", rowAll)].Value = item2.ZIPCode;
                        TrainingUsers.Cells[string.Format("V{0}", rowAll)].Value = relocate;
                        TrainingUsers.Cells[string.Format("V{0}", rowAll)].Value = relocate;
                    }

                    TrainingUsers.Cells[string.Format("W{0}", rowAll)].Value = item.WebsiteUser.RecruiterName;
                    TrainingUsers.Cells[string.Format("X{0}", rowAll)].Value = take;
                    TrainingUsers.Cells[string.Format("Y{0}", rowAll)].Value = evalStatus;
                    TrainingUsers.Cells[string.Format("Z{0}", rowAll)].Value = contract;

                    rowAll++;

                    if (item.WebsiteUser.TakeEvaluation == true)
                    {
                        TrainingUsersAttend.Cells[string.Format("A{0}", rowAttend)].Value = rowAttend - 1;
                        TrainingUsersAttend.Cells[string.Format("B{0}", rowAttend)].Value = item.WebsiteUser.FName;
                        TrainingUsersAttend.Cells[string.Format("C{0}", rowAttend)].Value = item.WebsiteUser.LName;
                        TrainingUsersAttend.Cells[string.Format("D{0}", rowAttend)].Value = item.WebsiteUser.Phone;
                        TrainingUsersAttend.Cells[string.Format("E{0}", rowAttend)].Value = item.WebsiteUser.Email;
                        TrainingUsersAttend.Cells[string.Format("F{0}", rowAttend)].Value = item.WebsiteUser.SkypeID;
                        TrainingUsersAttend.Cells[string.Format("G{0}", rowAttend)].Value = item.WebsiteUser.AboutUs;
                        TrainingUsersAttend.Cells[string.Format("H{0}", rowAttend)].Value = item.WebsiteUser.WhyInterested;
                        TrainingUsersAttend.Cells[string.Format("I{0}", rowAttend)].Value = payment;
                        TrainingUsersAttend.Cells[string.Format("J{0}", rowAttend)].Value = edate;

                        foreach (var item2 in item.WebsiteUser.WebsiteUserDetails)
                        {
                            var employed = "No";
                            var authorized = "No";
                            var relocate = "No";

                            if (item2.CurrentlyEmployed == 1)
                                employed = "Yes";

                            if (item2.AuthorizedWorkUSA == 1)
                                authorized = "Yes";

                            if (item2.OpenToRelocate == 1)
                                relocate = "Yes";

                            TrainingUsersAttend.Cells[string.Format("K{0}", rowAttend)].Value = item2.HighestEducation;
                            TrainingUsersAttend.Cells[string.Format("L{0}", rowAttend)].Value = item2.CompletionYear;
                            TrainingUsersAttend.Cells[string.Format("M{0}", rowAttend)].Value = item2.WorkExperienceIT + " year(s)";
                            TrainingUsersAttend.Cells[string.Format("N{0}", rowAttend)].Value = item2.WorkExperienceNonIT + " year(s)";
                            TrainingUsersAttend.Cells[string.Format("O{0}", rowAttend)].Value = employed;
                            TrainingUsersAttend.Cells[string.Format("P{0}", rowAttend)].Value = authorized;
                            TrainingUsersAttend.Cells[string.Format("Q{0}", rowAttend)].Value = item2.VisaStatus;
                            TrainingUsersAttend.Cells[string.Format("R{0}", rowAttend)].Value = item2.Address;
                            TrainingUsersAttend.Cells[string.Format("S{0}", rowAttend)].Value = item2.City;
                            TrainingUsersAttend.Cells[string.Format("T{0}", rowAttend)].Value = item2.CountryState.State;
                            TrainingUsersAttend.Cells[string.Format("U{0}", rowAttend)].Value = item2.ZIPCode;
                            TrainingUsersAttend.Cells[string.Format("V{0}", rowAttend)].Value = relocate;
                        }

                        TrainingUsersAttend.Cells[string.Format("W{0}", rowAttend)].Value = item.WebsiteUser.RecruiterName;
                        TrainingUsersAttend.Cells[string.Format("X{0}", rowAttend)].Value = take;
                        TrainingUsersAttend.Cells[string.Format("Y{0}", rowAttend)].Value = evalStatus;
                        TrainingUsersAttend.Cells[string.Format("Z{0}", rowAttend)].Value = contract;

                        rowAttend++;
                    }

                    if (item.WebsiteUser.EvaluationStatus == true)
                    {
                        TrainingUsersPass.Cells[string.Format("A{0}", rowPass)].Value = rowPass - 1;
                        TrainingUsersPass.Cells[string.Format("B{0}", rowPass)].Value = item.WebsiteUser.FName;
                        TrainingUsersPass.Cells[string.Format("C{0}", rowPass)].Value = item.WebsiteUser.LName;
                        TrainingUsersPass.Cells[string.Format("D{0}", rowPass)].Value = item.WebsiteUser.Phone;
                        TrainingUsersPass.Cells[string.Format("E{0}", rowPass)].Value = item.WebsiteUser.Email;
                        TrainingUsersPass.Cells[string.Format("F{0}", rowPass)].Value = item.WebsiteUser.SkypeID;
                        TrainingUsersPass.Cells[string.Format("G{0}", rowPass)].Value = item.WebsiteUser.AboutUs;
                        TrainingUsersPass.Cells[string.Format("H{0}", rowPass)].Value = item.WebsiteUser.WhyInterested;
                        TrainingUsersPass.Cells[string.Format("I{0}", rowPass)].Value = payment;
                        TrainingUsersPass.Cells[string.Format("J{0}", rowPass)].Value = edate;

                        foreach (var item2 in item.WebsiteUser.WebsiteUserDetails)
                        {
                            var employed = "No";
                            var authorized = "No";
                            var relocate = "No";

                            if (item2.CurrentlyEmployed == 1)
                                employed = "Yes";

                            if (item2.AuthorizedWorkUSA == 1)
                                authorized = "Yes";

                            if (item2.OpenToRelocate == 1)
                                relocate = "Yes";

                            TrainingUsersPass.Cells[string.Format("K{0}", rowPass)].Value = item2.HighestEducation;
                            TrainingUsersPass.Cells[string.Format("L{0}", rowPass)].Value = item2.CompletionYear;
                            TrainingUsersPass.Cells[string.Format("M{0}", rowPass)].Value = item2.WorkExperienceIT + " year(s)";
                            TrainingUsersPass.Cells[string.Format("N{0}", rowPass)].Value = item2.WorkExperienceNonIT + " year(s)";
                            TrainingUsersPass.Cells[string.Format("O{0}", rowPass)].Value = employed;
                            TrainingUsersPass.Cells[string.Format("P{0}", rowPass)].Value = authorized;
                            TrainingUsersPass.Cells[string.Format("Q{0}", rowPass)].Value = item2.VisaStatus;
                            TrainingUsersPass.Cells[string.Format("R{0}", rowPass)].Value = item2.Address;
                            TrainingUsersPass.Cells[string.Format("S{0}", rowPass)].Value = item2.City;
                            TrainingUsersPass.Cells[string.Format("T{0}", rowPass)].Value = item2.CountryState.State;
                            TrainingUsersPass.Cells[string.Format("U{0}", rowPass)].Value = item2.ZIPCode;
                            TrainingUsersPass.Cells[string.Format("V{0}", rowPass)].Value = relocate;
                        }

                        TrainingUsersPass.Cells[string.Format("W{0}", rowPass)].Value = item.WebsiteUser.RecruiterName;
                        TrainingUsersPass.Cells[string.Format("X{0}", rowPass)].Value = take;
                        TrainingUsersPass.Cells[string.Format("Y{0}", rowPass)].Value = evalStatus;
                        TrainingUsersPass.Cells[string.Format("Z{0}", rowPass)].Value = contract;

                        rowPass++;
                    }

                }

                TrainingUsers.Cells["A:AZ"].AutoFitColumns();
                TrainingUsersAttend.Cells["A:AZ"].AutoFitColumns();
                TrainingUsersPass.Cells["A:AZ"].AutoFitColumns();
                Response.ClearContent();

                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=TraininProgramUsers_" + DateTime.Parse(xDate).ToShortDateString() + "_Report.xlsx");
                Response.ContentType = "application/ms-excel";
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
            }
        }

        [HttpGet]
        public void AttendanceListExcelA(int? id)
        {
            TrainingCours trainingCours = db.TrainingCourses.Find(id);

            var data = db.WebsiteUserTrainings.Include(u => u.WebsiteUser).Where(x => x.TrainingCourseID == id).OrderBy(x => x.WebsiteUser.FName).OrderByDescending(x => x.WebsiteUserTrainingID).ToList();
            data = data.GroupBy(x => x.WebsiteUserID).Select(g => g.First()).OrderBy(x => x.WebsiteUser.FName).ToList();

            var dataA = data.Where(x => x.TrainingGroup == "A").ToList();

            List<WebsiteUserAttendance> attendance = db.WebsiteUserAttendances.Where(x => x.TrainingCourseID == id).ToList();

            for (int i = 1; i <= 5; i++)
            {
                ViewData["A_TodayDay" + i] = attendance.Where(x => x.Day == i && x.Attended == true && x.TrainingGroup == "A").Count();
            }

            
            int rowAll = 2;
            var xDate = DateTime.Now.ToShortDateString();
            //var pName = "";

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet TrainingUsers = Ep.Workbook.Worksheets.Add("Attendance Week 1");

            TrainingUsers.Cells["A1"].Value = "#";
            TrainingUsers.Cells["B1"].Value = "First Name";
            TrainingUsers.Cells["C1"].Value = "Last Name";
            TrainingUsers.Cells["D1"].Value = "Phone Number";
            TrainingUsers.Cells["E1"].Value = "Email";
            TrainingUsers.Cells["F1"].Value = "SkypeID";
            TrainingUsers.Cells["G1"].Value = "Recruiter";
            TrainingUsers.Cells["H1"].Value = "Day 1";
            TrainingUsers.Cells["I1"].Value = "Day 2";
            TrainingUsers.Cells["J1"].Value = "Day 3";
            TrainingUsers.Cells["K1"].Value = "Day 4";
            TrainingUsers.Cells["L1"].Value = "Day 5";

            TrainingUsers.Cells["A1"].Style.Font.Bold = true;
            TrainingUsers.Cells["B1"].Style.Font.Bold = true;
            TrainingUsers.Cells["C1"].Style.Font.Bold = true;
            TrainingUsers.Cells["D1"].Style.Font.Bold = true;
            TrainingUsers.Cells["E1"].Style.Font.Bold = true;
            TrainingUsers.Cells["F1"].Style.Font.Bold = true;
            TrainingUsers.Cells["G1"].Style.Font.Bold = true;
            TrainingUsers.Cells["H1"].Style.Font.Bold = true;
            TrainingUsers.Cells["I1"].Style.Font.Bold = true;
            TrainingUsers.Cells["J1"].Style.Font.Bold = true;
            TrainingUsers.Cells["K1"].Style.Font.Bold = true;
            TrainingUsers.Cells["L1"].Style.Font.Bold = true;


            foreach (var item in dataA)
            {
                TrainingUsers.Cells[string.Format("A{0}", rowAll)].Value = rowAll - 1;
                TrainingUsers.Cells[string.Format("B{0}", rowAll)].Value = item.WebsiteUser.FName;
                TrainingUsers.Cells[string.Format("C{0}", rowAll)].Value = item.WebsiteUser.LName;
                TrainingUsers.Cells[string.Format("D{0}", rowAll)].Value = item.WebsiteUser.Phone;
                TrainingUsers.Cells[string.Format("E{0}", rowAll)].Value = item.WebsiteUser.Email;
                TrainingUsers.Cells[string.Format("F{0}", rowAll)].Value = item.WebsiteUser.SkypeID;
                TrainingUsers.Cells[string.Format("G{0}", rowAll)].Value = item.WebsiteUser.RecruiterName;
                TrainingUsers.Cells[string.Format("H{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("H{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                TrainingUsers.Cells[string.Format("I{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("I{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                TrainingUsers.Cells[string.Format("J{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("J{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                TrainingUsers.Cells[string.Format("K{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("K{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                TrainingUsers.Cells[string.Format("L{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("L{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);

                foreach (var itemAttendance in attendance)
                {
                    if(itemAttendance.WebsiteUserID == item.WebsiteUserID)
                    {
                        if(itemAttendance.Day == 1 && itemAttendance.Attended ==true)
                        {
                            TrainingUsers.Cells[string.Format("H{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("H{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                        if (itemAttendance.Day == 2 && itemAttendance.Attended == true)
                        {
                            TrainingUsers.Cells[string.Format("I{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("I{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                        if (itemAttendance.Day == 3 && itemAttendance.Attended == true)
                        {
                            TrainingUsers.Cells[string.Format("J{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("J{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                        if (itemAttendance.Day == 4 && itemAttendance.Attended == true)
                        {
                            TrainingUsers.Cells[string.Format("K{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("K{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                        if (itemAttendance.Day == 5 && itemAttendance.Attended == true)
                        {
                            TrainingUsers.Cells[string.Format("L{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("L{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                    }
                }

                rowAll++;
                TrainingUsers.Cells[string.Format("G{0}", rowAll)].Value = "Total:";
                TrainingUsers.Cells[string.Format("H{0}", rowAll)].Value = ViewData["A_TodayDay1"];
                TrainingUsers.Cells[string.Format("I{0}", rowAll)].Value = ViewData["A_TodayDay2"];
                TrainingUsers.Cells[string.Format("J{0}", rowAll)].Value = ViewData["A_TodayDay3"];
                TrainingUsers.Cells[string.Format("K{0}", rowAll)].Value = ViewData["A_TodayDay4"];
                TrainingUsers.Cells[string.Format("L{0}", rowAll)].Value = ViewData["A_TodayDay5"];
            }

            

            TrainingUsers.Cells["A:L"].AutoFitColumns();
            Response.ClearContent();

            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=AttendanceList_" + DateTime.Parse(xDate).ToShortDateString() + "_Report.xlsx");
            Response.ContentType = "application/ms-excel";
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        [HttpGet]
        public void AttendanceListExcelB(int? id)
        {
            TrainingCours trainingCours = db.TrainingCourses.Find(id);

            var data = db.WebsiteUserTrainings.Include(u => u.WebsiteUser).Where(x => x.TrainingCourseID == id).OrderBy(x => x.WebsiteUser.FName).OrderByDescending(x => x.WebsiteUserTrainingID).ToList();
            data = data.GroupBy(x => x.WebsiteUserID).Select(g => g.First()).OrderBy(x => x.WebsiteUser.FName).ToList();

            var dataA = data.Where(x => x.TrainingGroup == "B").ToList();

            List<WebsiteUserAttendance> attendance = db.WebsiteUserAttendances.Where(x => x.TrainingCourseID == id).ToList();

            for (int i = 1; i <= 5; i++)
            {
                ViewData["B_TodayDay" + i] = attendance.Where(x => x.Day == i && x.Attended == true && x.TrainingGroup == "B").Count();
            }


            int rowAll = 2;
            var xDate = DateTime.Now.ToShortDateString();
            //var pName = "";

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet TrainingUsers = Ep.Workbook.Worksheets.Add("Attendance Week 1");

            TrainingUsers.Cells["A1"].Value = "#";
            TrainingUsers.Cells["B1"].Value = "First Name";
            TrainingUsers.Cells["C1"].Value = "Last Name";
            TrainingUsers.Cells["D1"].Value = "Phone Number";
            TrainingUsers.Cells["E1"].Value = "Email";
            TrainingUsers.Cells["F1"].Value = "SkypeID";
            TrainingUsers.Cells["G1"].Value = "Recruiter";
            TrainingUsers.Cells["H1"].Value = "Day 1";
            TrainingUsers.Cells["I1"].Value = "Day 2";
            TrainingUsers.Cells["J1"].Value = "Day 3";
            TrainingUsers.Cells["K1"].Value = "Day 4";
            TrainingUsers.Cells["L1"].Value = "Day 5";

            TrainingUsers.Cells["A1"].Style.Font.Bold = true;
            TrainingUsers.Cells["B1"].Style.Font.Bold = true;
            TrainingUsers.Cells["C1"].Style.Font.Bold = true;
            TrainingUsers.Cells["D1"].Style.Font.Bold = true;
            TrainingUsers.Cells["E1"].Style.Font.Bold = true;
            TrainingUsers.Cells["F1"].Style.Font.Bold = true;
            TrainingUsers.Cells["G1"].Style.Font.Bold = true;
            TrainingUsers.Cells["H1"].Style.Font.Bold = true;
            TrainingUsers.Cells["I1"].Style.Font.Bold = true;
            TrainingUsers.Cells["J1"].Style.Font.Bold = true;
            TrainingUsers.Cells["K1"].Style.Font.Bold = true;
            TrainingUsers.Cells["L1"].Style.Font.Bold = true;


            foreach (var item in dataA)
            {
                TrainingUsers.Cells[string.Format("A{0}", rowAll)].Value = rowAll - 1;
                TrainingUsers.Cells[string.Format("B{0}", rowAll)].Value = item.WebsiteUser.FName;
                TrainingUsers.Cells[string.Format("C{0}", rowAll)].Value = item.WebsiteUser.LName;
                TrainingUsers.Cells[string.Format("D{0}", rowAll)].Value = item.WebsiteUser.Phone;
                TrainingUsers.Cells[string.Format("E{0}", rowAll)].Value = item.WebsiteUser.Email;
                TrainingUsers.Cells[string.Format("F{0}", rowAll)].Value = item.WebsiteUser.SkypeID;
                TrainingUsers.Cells[string.Format("G{0}", rowAll)].Value = item.WebsiteUser.RecruiterName;
                TrainingUsers.Cells[string.Format("H{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("H{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                TrainingUsers.Cells[string.Format("I{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("I{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                TrainingUsers.Cells[string.Format("J{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("J{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                TrainingUsers.Cells[string.Format("K{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("K{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                TrainingUsers.Cells[string.Format("L{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("L{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);

                foreach (var itemAttendance in attendance)
                {
                    if (itemAttendance.WebsiteUserID == item.WebsiteUserID)
                    {
                        if (itemAttendance.Day == 1 && itemAttendance.Attended == true)
                        {
                            TrainingUsers.Cells[string.Format("H{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("H{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                        if (itemAttendance.Day == 2 && itemAttendance.Attended == true)
                        {
                            TrainingUsers.Cells[string.Format("I{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("I{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                        if (itemAttendance.Day == 3 && itemAttendance.Attended == true)
                        {
                            TrainingUsers.Cells[string.Format("J{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("J{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                        if (itemAttendance.Day == 4 && itemAttendance.Attended == true)
                        {
                            TrainingUsers.Cells[string.Format("K{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("K{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                        if (itemAttendance.Day == 5 && itemAttendance.Attended == true)
                        {
                            TrainingUsers.Cells[string.Format("L{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("L{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                    }
                }

                rowAll++;
                TrainingUsers.Cells[string.Format("G{0}", rowAll)].Value = "Total:";
                TrainingUsers.Cells[string.Format("H{0}", rowAll)].Value = ViewData["B_TodayDay1"];
                TrainingUsers.Cells[string.Format("I{0}", rowAll)].Value = ViewData["B_TodayDay2"];
                TrainingUsers.Cells[string.Format("J{0}", rowAll)].Value = ViewData["B_TodayDay3"];
                TrainingUsers.Cells[string.Format("K{0}", rowAll)].Value = ViewData["B_TodayDay4"];
                TrainingUsers.Cells[string.Format("L{0}", rowAll)].Value = ViewData["B_TodayDay5"];
            }



            TrainingUsers.Cells["A:L"].AutoFitColumns();
            Response.ClearContent();

            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=AttendanceListB_" + DateTime.Parse(xDate).ToShortDateString() + "_Report.xlsx");
            Response.ContentType = "application/ms-excel";
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        [HttpGet]
        public void AttendanceListExcelC(int? id)
        {
            TrainingCours trainingCours = db.TrainingCourses.Find(id);

            var data = db.WebsiteUserTrainings.Include(u => u.WebsiteUser).Where(x => x.TrainingCourseID == id).OrderBy(x => x.WebsiteUser.FName).OrderByDescending(x => x.WebsiteUserTrainingID).ToList();
            data = data.GroupBy(x => x.WebsiteUserID).Select(g => g.First()).OrderBy(x => x.WebsiteUser.FName).ToList();

            var dataA = data.Where(x => x.TrainingGroup == "C").ToList();

            List<WebsiteUserAttendance> attendance = db.WebsiteUserAttendances.Where(x => x.TrainingCourseID == id).ToList();

            for (int i = 1; i <= 5; i++)
            {
                ViewData["C_TodayDay" + i] = attendance.Where(x => x.Day == i && x.Attended == true && x.TrainingGroup == "C").Count();
            }


            int rowAll = 2;
            var xDate = DateTime.Now.ToShortDateString();
            //var pName = "";

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet TrainingUsers = Ep.Workbook.Worksheets.Add("Attendance Week 1");

            TrainingUsers.Cells["A1"].Value = "#";
            TrainingUsers.Cells["B1"].Value = "First Name";
            TrainingUsers.Cells["C1"].Value = "Last Name";
            TrainingUsers.Cells["D1"].Value = "Phone Number";
            TrainingUsers.Cells["E1"].Value = "Email";
            TrainingUsers.Cells["F1"].Value = "SkypeID";
            TrainingUsers.Cells["G1"].Value = "Recruiter";
            TrainingUsers.Cells["H1"].Value = "Day 1";
            TrainingUsers.Cells["I1"].Value = "Day 2";
            TrainingUsers.Cells["J1"].Value = "Day 3";
            TrainingUsers.Cells["K1"].Value = "Day 4";
            TrainingUsers.Cells["L1"].Value = "Day 5";

            TrainingUsers.Cells["A1"].Style.Font.Bold = true;
            TrainingUsers.Cells["B1"].Style.Font.Bold = true;
            TrainingUsers.Cells["C1"].Style.Font.Bold = true;
            TrainingUsers.Cells["D1"].Style.Font.Bold = true;
            TrainingUsers.Cells["E1"].Style.Font.Bold = true;
            TrainingUsers.Cells["F1"].Style.Font.Bold = true;
            TrainingUsers.Cells["G1"].Style.Font.Bold = true;
            TrainingUsers.Cells["H1"].Style.Font.Bold = true;
            TrainingUsers.Cells["I1"].Style.Font.Bold = true;
            TrainingUsers.Cells["J1"].Style.Font.Bold = true;
            TrainingUsers.Cells["K1"].Style.Font.Bold = true;
            TrainingUsers.Cells["L1"].Style.Font.Bold = true;


            foreach (var item in dataA)
            {
                TrainingUsers.Cells[string.Format("A{0}", rowAll)].Value = rowAll - 1;
                TrainingUsers.Cells[string.Format("B{0}", rowAll)].Value = item.WebsiteUser.FName;
                TrainingUsers.Cells[string.Format("C{0}", rowAll)].Value = item.WebsiteUser.LName;
                TrainingUsers.Cells[string.Format("D{0}", rowAll)].Value = item.WebsiteUser.Phone;
                TrainingUsers.Cells[string.Format("E{0}", rowAll)].Value = item.WebsiteUser.Email;
                TrainingUsers.Cells[string.Format("F{0}", rowAll)].Value = item.WebsiteUser.SkypeID;
                TrainingUsers.Cells[string.Format("G{0}", rowAll)].Value = item.WebsiteUser.RecruiterName;
                TrainingUsers.Cells[string.Format("H{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("H{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                TrainingUsers.Cells[string.Format("I{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("I{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                TrainingUsers.Cells[string.Format("J{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("J{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                TrainingUsers.Cells[string.Format("K{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("K{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                TrainingUsers.Cells[string.Format("L{0}", rowAll)].Value = "Absent ✘";
                TrainingUsers.Cells[string.Format("L{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Red);

                foreach (var itemAttendance in attendance)
                {
                    if (itemAttendance.WebsiteUserID == item.WebsiteUserID)
                    {
                        if (itemAttendance.Day == 1 && itemAttendance.Attended == true)
                        {
                            TrainingUsers.Cells[string.Format("H{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("H{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                        if (itemAttendance.Day == 2 && itemAttendance.Attended == true)
                        {
                            TrainingUsers.Cells[string.Format("I{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("I{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                        if (itemAttendance.Day == 3 && itemAttendance.Attended == true)
                        {
                            TrainingUsers.Cells[string.Format("J{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("J{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                        if (itemAttendance.Day == 4 && itemAttendance.Attended == true)
                        {
                            TrainingUsers.Cells[string.Format("K{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("K{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                        if (itemAttendance.Day == 5 && itemAttendance.Attended == true)
                        {
                            TrainingUsers.Cells[string.Format("L{0}", rowAll)].Value = "Present ✔";
                            TrainingUsers.Cells[string.Format("L{0}", rowAll)].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }

                    }
                }

                rowAll++;
                TrainingUsers.Cells[string.Format("G{0}", rowAll)].Value = "Total:";
                TrainingUsers.Cells[string.Format("H{0}", rowAll)].Value = ViewData["C_TodayDay1"];
                TrainingUsers.Cells[string.Format("I{0}", rowAll)].Value = ViewData["C_TodayDay2"];
                TrainingUsers.Cells[string.Format("J{0}", rowAll)].Value = ViewData["C_TodayDay3"];
                TrainingUsers.Cells[string.Format("K{0}", rowAll)].Value = ViewData["C_TodayDay4"];
                TrainingUsers.Cells[string.Format("L{0}", rowAll)].Value = ViewData["C_TodayDay5"];
            }



            TrainingUsers.Cells["A:L"].AutoFitColumns();
            Response.ClearContent();

            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=AttendanceListC_" + DateTime.Parse(xDate).ToShortDateString() + "_Report.xlsx");
            Response.ContentType = "application/ms-excel";
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        [HttpPost]
        public ActionResult UploadMaterials(FormCollection form)
        {
            int trainingID = Convert.ToInt32(form["TrainingID"].ToString());
            string sessionN = form["SessionN"].ToString();

            TrainingCours tc = db.TrainingCourses.Find(trainingID);

            var tName = tc.TrainingCourseName;
            tName = Regex.Replace(tName, @"[^0-9a-zA-Z:,!/\.]+", "");
            
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
                        TrainingResource tr = new TrainingResource();
                        tr.TrainingCourseID = tc.TrainingCourseID;
                        tr.Type = "Materials";
                        //tr.Path = "https://brightrace.com/tms" + imagesDirectoryX;
                        //tr.BackupPath = uploadpath2;
                        tr.SessionN = sessionN;
                        tr.DateRecord = DateTime.Now;
                        tr.CreatedBy = Convert.ToInt32(Session["LoginId"]);
                        tr.OriginalFileName = file.FileName;
                        tr.FileSize = file.ContentLength;
                        //tr.FileExtension = Path.GetExtension(filename1);

                        db.TrainingResources.Add(tr);
                        db.SaveChanges();

                        string path1 = Path.Combine(Server.MapPath("~/BrightraceResources/" + tName+tr.TrainingCourseID+ "/Materials/" + sessionN));
                        //string path2 = "I:\\BrightraceResourcesBackup\\" + tName + tr.TrainingCourseID + "\\Materials\\" + sessionN;

                        string relPath = "/BrightraceResources/" + tName + tr.TrainingCourseID + "/Materials/" + sessionN;
                        string imagesDirectory = Path.Combine(Environment.CurrentDirectory, relPath);

                        string pathstring1 = Path.Combine(path1.ToString());
                        //string pathstring2 = Path.Combine(path2.ToString());

                        //string filename1 = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        string filename1 = file.FileName;
                        bool isexist1 = Directory.Exists(pathstring1);
                        //bool isexist2 = Directory.Exists(pathstring2);
                        if (!isexist1)
                        {
                            Directory.CreateDirectory(pathstring1);
                        }
                        //if (!isexist2)
                        //{
                        //    Directory.CreateDirectory(pathstring2);
                        //}

                        string uploadpath1 = string.Format("{0}\\{1}", pathstring1, filename1);
                        file.SaveAs(uploadpath1);
                        var imagesDirectoryX = string.Format("{0}\\{1}", imagesDirectory, filename1);

                        //string uploadpath2 = string.Format("{0}\\{1}", pathstring2, filename1);
                        //file.SaveAs(uploadpath2);

                        tr.Path = "https://brightrace.com/tms" + imagesDirectoryX;
                        //tr.BackupPath = uploadpath2;
                        tr.FileExtension = Path.GetExtension(filename1);

                        db.Entry(tr).State = EntityState.Modified;
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

        [HttpPost]
        public ActionResult UploadRecordings(FormCollection form)
        {
            int trainingID = Convert.ToInt32(form["TrainingID"].ToString());
            string sessionN = form["SessionN"].ToString();

            TrainingCours tc = db.TrainingCourses.Find(trainingID);

            var tName = tc.TrainingCourseName;
            tName = Regex.Replace(tName, @"[^0-9a-zA-Z:,!/\.]+", "");

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
                        TrainingResource tr = new TrainingResource();
                        tr.TrainingCourseID = tc.TrainingCourseID;
                        tr.Type = "Recordings";
                        //tr.Path = "https://brightrace.com/tms" + imagesDirectoryX;
                        //tr.BackupPath = uploadpath2;
                        tr.SessionN = sessionN;
                        tr.DateRecord = DateTime.Now;
                        tr.CreatedBy = Convert.ToInt32(Session["LoginId"]);
                        tr.OriginalFileName = file.FileName;
                        tr.FileSize = file.ContentLength;
                        //tr.FileExtension = Path.GetExtension(filename1);

                        db.TrainingResources.Add(tr);
                        db.SaveChanges();

                        string path1 = Path.Combine(Server.MapPath("~/BrightraceResources/" + tName + tr.TrainingCourseID + "/Recordings/" + sessionN));
                        //string path2 = "I:\\BrightraceResourcesBackup\\" + tName + tr.TrainingCourseID + "\\Recordings\\" + sessionN;

                        string relPath = "/BrightraceResources/" + tName + tr.TrainingCourseID + "/Recordings/" + sessionN;
                        string imagesDirectory = Path.Combine(Environment.CurrentDirectory, relPath);

                        string pathstring1 = Path.Combine(path1.ToString());
                        //string pathstring2 = Path.Combine(path2.ToString());

                        //string filename1 = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        string filename1 = file.FileName;
                        bool isexist1 = Directory.Exists(pathstring1);
                        //bool isexist2 = Directory.Exists(pathstring2);
                        if (!isexist1)
                        {
                            Directory.CreateDirectory(pathstring1);
                        }
                        //if (!isexist2)
                        //{
                        //    Directory.CreateDirectory(pathstring2);
                        //}

                        string uploadpath1 = string.Format("{0}\\{1}", pathstring1, filename1);
                        file.SaveAs(uploadpath1);
                        var imagesDirectoryX = string.Format("{0}\\{1}", imagesDirectory, filename1);

                        //string uploadpath2 = string.Format("{0}\\{1}", pathstring2, filename1);
                        //file.SaveAs(uploadpath2);

                        tr.Path = "https://brightrace.com/tms" + imagesDirectoryX;
                        //tr.BackupPath = uploadpath2;
                        tr.FileExtension = Path.GetExtension(filename1);

                        db.Entry(tr).State = EntityState.Modified;
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

        [HttpPost]
        public ActionResult UploadAssignments(FormCollection form)
        {
            int trainingID = Convert.ToInt32(form["TrainingID"].ToString());
            string sessionN = form["SessionN"].ToString();

            TrainingCours tc = db.TrainingCourses.Find(trainingID);

            var tName = tc.TrainingCourseName;
            tName = Regex.Replace(tName, @"[^0-9a-zA-Z:,!/\.]+", "");

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
                        TrainingResource tr = new TrainingResource();
                        tr.TrainingCourseID = tc.TrainingCourseID;
                        tr.Type = "Assignments";
                        //tr.Path = "https://brightrace.com/tms" + imagesDirectoryX;
                        //tr.BackupPath = uploadpath2;
                        tr.SessionN = sessionN;
                        tr.DateRecord = DateTime.Now;
                        tr.CreatedBy = Convert.ToInt32(Session["LoginId"]);
                        tr.OriginalFileName = file.FileName;
                        tr.FileSize = file.ContentLength;
                        //tr.FileExtension = Path.GetExtension(filename1);

                        db.TrainingResources.Add(tr);
                        db.SaveChanges();

                        string path1 = Path.Combine(Server.MapPath("~/BrightraceResources/" + tName + tr.TrainingCourseID + "/Assignments/" + sessionN));
                        //string path2 = "I:\\BrightraceResourcesBackup\\" + tName + tr.TrainingCourseID + "\\Assignments\\" + sessionN;

                        string relPath = "/BrightraceResources/" + tName + tr.TrainingCourseID + "/Assignments/" + sessionN;
                        string imagesDirectory = Path.Combine(Environment.CurrentDirectory, relPath);

                        string pathstring1 = Path.Combine(path1.ToString());
                        //string pathstring2 = Path.Combine(path2.ToString());

                        //string filename1 = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        string filename1 = file.FileName;
                        bool isexist1 = Directory.Exists(pathstring1);
                        //bool isexist2 = Directory.Exists(pathstring2);
                        if (!isexist1)
                        {
                            Directory.CreateDirectory(pathstring1);
                        }
                        //if (!isexist2)
                        //{
                        //    Directory.CreateDirectory(pathstring2);
                        //}

                        string uploadpath1 = string.Format("{0}\\{1}", pathstring1, filename1);
                        file.SaveAs(uploadpath1);
                        var imagesDirectoryX = string.Format("{0}\\{1}", imagesDirectory, filename1);

                        //string uploadpath2 = string.Format("{0}\\{1}", pathstring2, filename1);
                        //file.SaveAs(uploadpath2);

                        tr.Path = "https://brightrace.com/tms" + imagesDirectoryX;
                        //tr.BackupPath = uploadpath2;
                        tr.FileExtension = Path.GetExtension(filename1);

                        db.Entry(tr).State = EntityState.Modified;
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

        [HttpPost]
        public ActionResult UploadOthers(FormCollection form)
        {
            int trainingID = Convert.ToInt32(form["TrainingID"].ToString());
            string sessionN = form["SessionN"].ToString();

            TrainingCours tc = db.TrainingCourses.Find(trainingID);

            var tName = tc.TrainingCourseName;
            tName = Regex.Replace(tName, @"[^0-9a-zA-Z:,!/\.]+", "");

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
                        TrainingResource tr = new TrainingResource();
                        tr.TrainingCourseID = tc.TrainingCourseID;
                        tr.Type = "Others";
                        //tr.Path = "https://brightrace.com/tms" + imagesDirectoryX;
                        //tr.BackupPath = uploadpath2;
                        tr.SessionN = sessionN;
                        tr.DateRecord = DateTime.Now;
                        tr.CreatedBy = Convert.ToInt32(Session["LoginId"]);
                        tr.OriginalFileName = file.FileName;
                        tr.FileSize = file.ContentLength;
                        //tr.FileExtension = Path.GetExtension(filename1);

                        db.TrainingResources.Add(tr);
                        db.SaveChanges();

                        string path1 = Path.Combine(Server.MapPath("~/BrightraceResources/" + tName + tr.TrainingCourseID + "/Others/" + sessionN));
                        //string path2 = "I:\\BrightraceResourcesBackup\\" + tName + tr.TrainingCourseID + "\\Others\\" + sessionN;

                        string relPath = "/BrightraceResources/" + tName + tr.TrainingCourseID + "/Others/" + sessionN;
                        string imagesDirectory = Path.Combine(Environment.CurrentDirectory, relPath);

                        string pathstring1 = Path.Combine(path1.ToString());
                        //string pathstring2 = Path.Combine(path2.ToString());

                        //string filename1 = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        string filename1 = file.FileName;
                        bool isexist1 = Directory.Exists(pathstring1);
                        //bool isexist2 = Directory.Exists(pathstring2);
                        if (!isexist1)
                        {
                            Directory.CreateDirectory(pathstring1);
                        }
                        //if (!isexist2)
                        //{
                        //    Directory.CreateDirectory(pathstring2);
                        //}

                        string uploadpath1 = string.Format("{0}\\{1}", pathstring1, filename1);
                        file.SaveAs(uploadpath1);
                        var imagesDirectoryX = string.Format("{0}\\{1}", imagesDirectory, filename1);

                        //string uploadpath2 = string.Format("{0}\\{1}", pathstring2, filename1);
                        //file.SaveAs(uploadpath2);

                        tr.Path = "https://brightrace.com/tms" + imagesDirectoryX;
                        //tr.BackupPath = uploadpath2;
                        tr.FileExtension = Path.GetExtension(filename1);

                        db.Entry(tr).State = EntityState.Modified;
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

        [HttpPost]
        public ActionResult ReturnTabIndex(int tabIndex, int trainingID)
        {
            
            TempData["trainingID"] = trainingID;

            TrainingCours tc = db.TrainingCourses.Find(trainingID);

            tc.Sessions = tc.Sessions + 1;
            db.Entry(tc).State = EntityState.Modified;
            db.SaveChanges();

            TrainingSession ts = new TrainingSession();
            ts.SessionTitle = "Session " + (tc.Sessions-1);
            ts.TrainingCourseID = tc.TrainingCourseID;
            ts.SessionID = (tc.Sessions - 1);
            db.TrainingSessions.Add(ts);
            db.SaveChanges();

            TempData["tabIndex"] = tc.Sessions;

            var tName = tc.TrainingCourseName;
            tName = Regex.Replace(tName, @"[^0-9a-zA-Z:,!/\.]+", "");

            TempData["tName"] = tName;
            return PartialView("_sessionTab");

            //return Json(res);
        }

        // GET: TrainingCours/Delete/5
        public ActionResult DeleteTrainingSession(int? id)
        {
            TrainingCours tc = db.TrainingCourses.Find(id);
            var tName = tc.TrainingCourseName;
            tName = Regex.Replace(tName, @"[^0-9a-zA-Z:,!/\.]+", "");

            int SessionID = Convert.ToInt32(tc.Sessions) - 1;

            string path1 = Path.Combine(Server.MapPath("~/BrightraceResources/" + tName + tc.TrainingCourseID + "/Materials/Session" + SessionID));
            string path2 = Path.Combine(Server.MapPath("~/BrightraceResources/" + tName + tc.TrainingCourseID + "/Recordings/Session" + SessionID));
            string path3 = Path.Combine(Server.MapPath("~/BrightraceResources/" + tName + tc.TrainingCourseID + "/Assignments/Session" + SessionID));
            string path4 = Path.Combine(Server.MapPath("~/BrightraceResources/" + tName + tc.TrainingCourseID + "/Others/Session" + SessionID));

            //string path5 = "C:\\BrightraceResources\\" + tName+tc.TrainingCourseID + "\\Materials\\Session" + SessionID;
            //string path6 = "C:\\BrightraceResources\\" + tName+tc.TrainingCourseID + "\\Recordings\\Session" + SessionID;
            //string path7 = "C:\\BrightraceResources\\" + tName+tc.TrainingCourseID + "\\Assignments\\Session" + SessionID;
            //string path8 = "C:\\BrightraceResources\\" + tName+tc.TrainingCourseID + "\\Others\\Session" + SessionID;

            var dir1 = new DirectoryInfo(path1);
            var dir2 = new DirectoryInfo(path2);
            var dir3 = new DirectoryInfo(path3);
            var dir4 = new DirectoryInfo(path4);
            //var dir5 = new DirectoryInfo(path5);
            //var dir6 = new DirectoryInfo(path6);
            //var dir7 = new DirectoryInfo(path7);
            //var dir8 = new DirectoryInfo(path8);

            if (Directory.Exists(path1))
                dir1.Delete(true);
            if (Directory.Exists(path2))
                dir2.Delete(true);
            if (Directory.Exists(path3))
                dir3.Delete(true);
            if (Directory.Exists(path4))
                dir4.Delete(true);
            //if (Directory.Exists(path5))
            //    dir5.Delete(true);
            //if (Directory.Exists(path6))
            //    dir6.Delete(true);
            //if (Directory.Exists(path7))
            //    dir7.Delete(true);
            //if (Directory.Exists(path8))
            //    dir8.Delete(true);

            List<TrainingResource> tr = db.TrainingResources.Where(x => x.TrainingCourseID == tc.TrainingCourseID && x.SessionN == "Session" + SessionID).ToList();

            foreach (var i in tr)
            {
                db.TrainingResources.Remove(i);
                db.SaveChanges();
            }

            tc.Sessions = tc.Sessions - 1;
            db.Entry(tc).State = EntityState.Modified;
            db.SaveChanges();

            List<TrainingSession> ts = db.TrainingSessions.Where(x => x.SessionID == tc.Sessions && x.TrainingCourseID == tc.TrainingCourseID).ToList();

            foreach (var j in ts)
            {
                db.TrainingSessions.Remove(j);
                db.SaveChanges();
            }
            

            TempData["tabIndex"] = SessionID;

            return null;
            //return PartialView("_ConfirmDeleteTrainingSession");
        }

        [AllowAnonymous]
        public ActionResult GetAttachments(int? id, int? id2, string type)
        {

            List<TrainingResource> tr = db.TrainingResources.Where(x => x.TrainingCourseID == id && x.SessionN == "Session"+id2 && x.Type == type).ToList();

            //Get the images list from repository
            var attachmentsList = new List<AttachmentsModel>();

            foreach (var item in tr)
            {
                attachmentsList.Add(new AttachmentsModel()
                {
                    AttachmentID = item.TrainingResourcesID,
                    FileName = item.OriginalFileName,
                    Path = item.Path,
                    FileSize = item.FileSize,
                    FileExtension = item.FileExtension
                });

            }

            return Json(new { Data = attachmentsList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteAttachments(FormCollection form)
        {
            int fID = Convert.ToInt32(form["fID"]);
            int tID = Convert.ToInt32(form["tID"]);
            string fName = form["fName"].ToString();

            TrainingResource tr = new TrainingResource();

            if (fID != 0)
                tr = db.TrainingResources.Find(fID);

            else if (fID == 0)
                tr = db.TrainingResources.Where(x => x.OriginalFileName.Contains(fName) && x.TrainingCourseID == tID).First();
             
            db.TrainingResources.Remove(tr);
            db.SaveChanges();

            var p = tr.Path;

            var pResult = p.Replace("https://brightrace.com/tms", "");

            string path1 = Path.Combine(Server.MapPath("~"+pResult));
            //string path2 = Path.Combine(Server.MapPath(tr.BackupPath));
            //string path2 = tr.BackupPath;
            //var dir1 = new DirectoryInfo(path1);

            if (System.IO.File.Exists(path1))
                System.IO.File.Delete(path1);

            //if (System.IO.File.Exists(path2))
            //    System.IO.File.Delete(path2);

            //return RedirectToAction("TrainingCoursesList", "TrainingCours", new { searchString = str });
            return null;
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditMeetingInfo([Bind(Include = "TrainingCourseID,MeetingDetails")] TrainingCours trainingCours, HttpPostedFileBase file)
        {
            var tc = db.TrainingCourses.Find(trainingCours.TrainingCourseID);

            if (trainingCours.MeetingDetails != null)
                tc.MeetingDetailsBlob = Encoding.ASCII.GetBytes(trainingCours.MeetingDetails);
            else
                tc.MeetingDetailsBlob = Encoding.ASCII.GetBytes("");

            tc.MeetingDetails = "";
            tc.LastUpdate = DateTime.Now;
            db.Entry(tc).State = EntityState.Modified;
            db.SaveChanges();

            TempData["updateStatus"] = "OK";
            return RedirectToAction("EditTrainingCourse", new { id = trainingCours.TrainingCourseID });
            //return RedirectToAction("TrainingCoursesList");
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditAnnouncements([Bind(Include = "TrainingCourseID,SpecialAnnouncements")] TrainingCours trainingCours, HttpPostedFileBase file)
        {
            var tc = db.TrainingCourses.Find(trainingCours.TrainingCourseID);

            if (trainingCours.SpecialAnnouncements != null)
                tc.SpecialAnnouncementsBlob = Encoding.ASCII.GetBytes(trainingCours.SpecialAnnouncements);
            else
                tc.SpecialAnnouncementsBlob = Encoding.ASCII.GetBytes("");

            tc.SpecialAnnouncements = "";
            tc.LastUpdate = DateTime.Now;
            db.Entry(tc).State = EntityState.Modified;
            db.SaveChanges();

            TempData["updateStatus"] = "OK";
            return RedirectToAction("EditTrainingCourse", new { id = trainingCours.TrainingCourseID });
            //return RedirectToAction("TrainingCoursesList");
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditTrainingSession([Bind(Include = "TrainingCourseID,SessionID,SessionTitle,SessionNotes")] TrainingSession trainingSession, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {

                List<TrainingSession> ts = db.TrainingSessions.Where(x => x.TrainingCourseID == trainingSession.TrainingCourseID && x.SessionID == trainingSession.SessionID).ToList();

                //if (trainingSession.SessionNotes != null)
                //    ts[0].SessionNotesBlob = Encoding.ASCII.GetBytes(trainingSession.SessionNotes);
                //else
                //    ts[0].SessionNotesBlob = Encoding.ASCII.GetBytes("");

                //ts[0].SessionNotes = "";

                ts[0].SessionTitle = trainingSession.SessionTitle;

                db.Entry(ts[0]).State = EntityState.Modified;
                db.SaveChanges();
                TempData["updateStatus"] = "OK";
                return RedirectToAction("EditTrainingCourse", new { id = trainingSession.TrainingCourseID } );
            }
            TempData["updateStatus"] = "Error";
            return View(trainingSession);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditTrainingSession2(int sessionID, int TrainingID, string Title)
        {
            List<TrainingSession> ts = db.TrainingSessions.Where(x => x.TrainingCourseID == TrainingID && x.SessionID == sessionID).ToList();

            ts[0].SessionTitle = Title;

            db.Entry(ts[0]).State = EntityState.Modified;
            db.SaveChanges();
            TempData["updateStatus"] = "OK";
            return null;
        }

        public ActionResult CloneTrainingCourse(int? id)
        {
            ViewBag.ID = id;

            return PartialView("_ConfirmCloneTrainingCourse");
        }

        // POST: TrainingCours/Delete/5
        [HttpPost]
        public ActionResult ConfirmCloneTrainingCourse(int id)
        {
            TrainingCours trainingCours = db.TrainingCourses.Find(id);

            string s = trainingCours.TrainingCourseName;
            string firstWord = s.Split(' ').First();
            var num = new Random();

            trainingCours.StartDate1 = null;
            trainingCours.StartDate2 = null;
            trainingCours.EndDate1 = null;
            trainingCours.EndDate2 = null;
            trainingCours.LimitFullPaymentDate = null;
            trainingCours.IsActive = false;
            trainingCours.SKU = firstWord + num.Next(0,99);
            trainingCours.Sessions = 1;
            trainingCours.CreatedOn = DateTime.Now;
            trainingCours.CreatedBy = Convert.ToInt32(Session["LoginId"]);

            db.TrainingCourses.Add(trainingCours);
            db.SaveChanges();

            return RedirectToAction("TrainingCoursesList");
        }

        public FileResult DownloadAssignment(int? id)
        {
            if (id == null)
            {
                return null;
            }

            WebsiteusersAssignment data = db.WebsiteusersAssignments.Find(id);

            var path = data.Path;

            byte[] fileBytes = System.IO.File.ReadAllBytes(data.Path);
            string fileName = data.OriginalFileName;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        public ActionResult SaveTrainingTrainer(int trainerID, int TrainingID)
        {
            
            TrainingTrainer data = new TrainingTrainer();

            data.TrainingCourseID = TrainingID;
            data.TrainerID = trainerID;

            db.TrainingTrainers.Add(data);
            db.SaveChanges();

            return null;
        }

        [HttpPost]
        public ActionResult DeleteTrainingTrainer(int trainerID, int TrainingID)
        {

            List<TrainingTrainer> data = db.TrainingTrainers.Where(x => x.TrainerID == trainerID && x.TrainingCourseID == TrainingID).ToList();

            if(data.Count > 0)
            {
                for (var i = 0; i <= data.Count; i++)
                {
                    db.TrainingTrainers.Remove(data[i]);
                    db.SaveChanges();
                }
            }

            return null;
        }

        // GET: TrainingCours/Delete/5
        public ActionResult SendMessage(int? id)
        {
            //ViewBag.tempID = id;

            WebsiteUser wu = db.WebsiteUsers.Find(id);
            if(wu.FName != "")
            {
                TempData["ID"] = wu.WebsiteUserID;
                TempData["UserName"] = wu.FName + " " + wu.LName;
                TempData["Email"] = wu.Email;
            }
            
            return PartialView("_SendMessage");
        }

        [ValidateInput(false)]
        [HttpPost]
        public async System.Threading.Tasks.Task<string> ConfirmSendMessageInternal(string subject, string message, int userID)
        {
            try
            {
                //Message data = new Message();
                //data.FromInternalID = Convert.ToInt32(Session["LoginId"]);
                //data.ToInternalID = toID;
                //data.FromExternalID = null;
                //data.ToExternalID = null;
                //data.Subject = subject;
                //data.Message1 = message;
                //data.CreateDate = DateTime.Now;
                //data.Status = "Sent";
                //data.ReplyMessageID = null;
                //data.MessageBlob = Encoding.ASCII.GetBytes(message);

                //db.Messages.Add(data);
                //db.SaveChanges();

                WebsiteUser wu = db.WebsiteUsers.Find(userID);
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
                wu.Status = "Resume Preparation";
                wu.Ready4Resume = DateTime.Now;

                db.SaveChanges();

                var fName = wu.FName + " " + wu.LName;

                var fromTrainer = db.Users.Find(Convert.ToInt32(Session["LoginId"]));

                List<User> trainers = db.Users.Where(x => ((x.Roles_id == 4 || x.Roles_id == 2) && x.ActiveInactive == true)).ToList();

                for (int i = 0; i < trainers.Count(); i++)
                {
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
                                            <td style=""padding: 10px 30px 10px 30px;"" > Hi <b> {trainers[i].FullName} </b>,</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;"" > { fromTrainer.FullName } approves {fName} to start working on Resume preparation.</td>
                                        </tr>
                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;"" > <b>Message:</b> {message}</td>
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


                    await se.sendEmailAsync(subjectEmail, body, trainers[i].Email, null);
                }

                return null;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public ActionResult DeleteUserFromTraining(int? userID, int? tID)
        {
            
            WebsiteUser wu = db.WebsiteUsers.Find(userID);

            ViewBag.userID = userID;
            ViewBag.tID = tID;
            ViewBag.fName = wu.FName+" "+wu.LName;

            return PartialView("_ConfirmDeleteUserFromTraining");
        }

        public ActionResult DeleteUserFromTraining2(int? userID, int? tID)
        {

            WebsiteUser wu = db.WebsiteUsers.Find(userID);

            ViewBag.userID = userID;
            ViewBag.tID = tID;
            ViewBag.fName = wu.FName + " " + wu.LName;

            return PartialView("_ConfirmDeleteUserFromTraining2");
        }

        public ActionResult DeleteEnrolledUserFromTraining(int? userID, int? tID)
        {

            WebsiteUser wu = db.WebsiteUsers.Find(userID);

            ViewBag.userID = userID;
            ViewBag.tID = tID;
            ViewBag.fName = wu.FName + " " + wu.LName;

            return PartialView("_ConfirmDeleteEnrolledUserFromTraining");
        }

        public ActionResult DeleteMockInterview(int? mockID)
        {
            ViewBag.mockID = mockID;
            //MockInterview mi = db.MockInterviews.Find(mockID);

            return PartialView("_ConfirmDeleteMock");
        }

        [HttpPost]
        public ActionResult ConfirmDeleteMock(int? mockID)
        {
            MockInterview mi = db.MockInterviews.Find(mockID);
            
            db.MockInterviews.Remove(mi);
            db.SaveChanges();

            return null;
        }

        // POST: TrainingCours/Delete/5
        [HttpPost]
        public ActionResult ConfirmDeleteUserFromTraining(int? uID, int? tID)
        {
            List<WebsiteUserTraining> wut= db.WebsiteUserTrainings.Where(x => x.WebsiteUserID == uID && x.TrainingCourseID == tID).ToList();
            for (int i = 0; i < wut.Count; i++)
            {
                db.WebsiteUserTrainings.Remove(wut[i]);
                db.SaveChanges();
            }       
            
            List<WebsiteUserAttendance> wua = db.WebsiteUserAttendances.Where(x => (x.WebsiteUserID == uID && x.TrainingCourseID == tID)).ToList();
            for (int i = 0; i < wua.Count; i++)
            {
                db.WebsiteUserAttendances.Remove(wua[i]);
                db.SaveChanges();
            }

            return RedirectToAction("ViewTrainingCourse", new { id = tID });
        }

        [HttpPost]
        public ActionResult ConfirmDeleteUserFromTraining2(int? uID, int? tID)
        {
            List<WebsiteUserTraining> wut = db.WebsiteUserTrainings.Where(x => x.WebsiteUserID == uID && x.TrainingCourseID == tID).ToList();
            for (int i = 0; i < wut.Count; i++)
            {
                db.WebsiteUserTrainings.Remove(wut[i]);
                db.SaveChanges();
            }

            List<WebsiteUserAttendance> wua = db.WebsiteUserAttendances.Where(x => (x.WebsiteUserID == uID && x.TrainingCourseID == tID)).ToList();
            for (int i = 0; i < wua.Count; i++)
            {
                db.WebsiteUserAttendances.Remove(wua[i]);
                db.SaveChanges();
            }

            return null;
        }

        [HttpPost]
        public ActionResult ConfirmDeleteEnrolledUserFromTraining(int? uID, int? tID)
        {
            List<WebsiteUserTraining> wut = db.WebsiteUserTrainings.Where(x => x.WebsiteUserID == uID && x.TrainingCourseID == tID).ToList();
            for (int i = 0; i < wut.Count; i++)
            {
                db.WebsiteUserTrainings.Remove(wut[i]);
                db.SaveChanges();
            }

            List<WebsiteUserAttendance> wua = db.WebsiteUserAttendances.Where(x => (x.WebsiteUserID == uID && x.TrainingCourseID == tID)).ToList();
            for (int i = 0; i < wua.Count; i++)
            {
                db.WebsiteUserAttendances.Remove(wua[i]);
                db.SaveChanges();
            }

            return null;
        }

        public ActionResult GetEvaluationList(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TrainingCours trainingCours = db.TrainingCourses.Find(id);
            if (trainingCours == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "ID", "FullName", trainingCours.CreatedBy);

            if (trainingCours.FullDescriptionBlob != null)
                trainingCours.FullDescription = Encoding.ASCII.GetString(trainingCours.FullDescriptionBlob);

            var usersTrainingAll = db.WebsiteUserTrainings.Include(u => u.WebsiteUser).Where(x => x.TrainingCourseID == id).OrderBy(x => x.WebsiteUser.FName).OrderByDescending(x => x.WebsiteUserTrainingID).ToList();

            usersTrainingAll = usersTrainingAll.GroupBy(x => x.WebsiteUserID).Select(g => g.First()).OrderBy(x => x.WebsiteUser.FName).ToList();

            ViewBag.allUsers = usersTrainingAll.Count();
            ViewBag.fullUsers = usersTrainingAll.Where(x => x.PaymentType == 3).Count();
            ViewBag.fUsers = usersTrainingAll.Where(x => x.PaymentType == 1).Count();
            ViewBag.sUsers = usersTrainingAll.Where(x => x.PaymentType == 2).Count();
            ViewBag.nUsers = usersTrainingAll.Where(x => x.PaymentType == 4).Count();
            ViewBag.TrainingCourseID = trainingCours.TrainingCourseID;
            ViewBag.TotalAppear = usersTrainingAll.Where(x => x.WebsiteUser.TakeEvaluation == true).Count();
            ViewBag.TotalPass = usersTrainingAll.Where(x => x.WebsiteUser.EvaluationStatus == true).Count();

            var usersTrainingA = usersTrainingAll.Where(x => x.TrainingGroup == "A").ToList();
            var usersTrainingB = usersTrainingAll.Where(x => x.TrainingGroup == "B").ToList();
            var usersTrainingC = usersTrainingAll.Where(x => x.TrainingGroup == "C").ToList();

            List<WebsiteUserAttendance> attendance = db.WebsiteUserAttendances.Where(x => x.TrainingCourseID == id).ToList();

            for (int i = 1; i <= 50; i++)
            {
                ViewData["TodayDay" + i] = attendance.Where(x => x.Day == i && x.Attended == true).Count();
            }

            ViewBag.TRAINING_COURSE_ID = new SelectList(db.TrainingCourses.OrderByDescending(s => s.TrainingCourseID), "TrainingCourseID", "TrainingCourseName");

            dynamic MultiModel = new ExpandoObject();
            MultiModel.Training = trainingCours;
            MultiModel.UsersAll = usersTrainingAll;
            MultiModel.UsersA = usersTrainingA;
            MultiModel.UsersB = usersTrainingB;
            MultiModel.UsersC = usersTrainingC;
            MultiModel.Attendance = attendance;

            return PartialView(MultiModel);
        }

        public ActionResult GetEnrolledUserList(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            

            TrainingCours trainingCours = db.TrainingCourses.Find(id);
            if (trainingCours == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "ID", "FullName", trainingCours.CreatedBy);

            if (trainingCours.FullDescriptionBlob != null)
                trainingCours.FullDescription = Encoding.ASCII.GetString(trainingCours.FullDescriptionBlob);

            var usersTrainingAll = db.WebsiteUserTrainings.Include(u => u.WebsiteUser).Where(x => x.TrainingCourseID == id).OrderBy(x => x.WebsiteUser.FName).OrderByDescending(x => x.WebsiteUserTrainingID).ToList();

            usersTrainingAll = usersTrainingAll.GroupBy(x => x.WebsiteUserID).Select(g => g.First()).OrderBy(x => x.WebsiteUser.FName).ToList();

            ViewBag.allUsers = usersTrainingAll.Count();
            ViewBag.fullUsers = usersTrainingAll.Where(x => x.PaymentType == 3).Count();
            ViewBag.fUsers = usersTrainingAll.Where(x => x.PaymentType == 1).Count();
            ViewBag.sUsers = usersTrainingAll.Where(x => x.PaymentType == 2).Count();
            ViewBag.nUsers = usersTrainingAll.Where(x => x.PaymentType == 4).Count();
            ViewBag.TrainingCourseID = trainingCours.TrainingCourseID;
            ViewBag.TotalAppear = usersTrainingAll.Where(x => x.WebsiteUser.TakeEvaluation == true).Count();
            ViewBag.TotalPass = usersTrainingAll.Where(x => x.WebsiteUser.EvaluationStatus == true).Count();

            var usersTrainingA = usersTrainingAll.Where(x => x.TrainingGroup == "A").ToList();
            var usersTrainingB = usersTrainingAll.Where(x => x.TrainingGroup == "B").ToList();
            var usersTrainingC = usersTrainingAll.Where(x => x.TrainingGroup == "C").ToList();

            List<WebsiteUserAttendance> attendance = db.WebsiteUserAttendances.Where(x => x.TrainingCourseID == id).ToList();

            for (int i = 1; i <= 50; i++)
            {
                ViewData["TodayDay" + i] = attendance.Where(x => x.Day == i && x.Attended == true).Count();
            }

            ViewBag.TRAINING_COURSE_ID = new SelectList(db.TrainingCourses.OrderByDescending(s => s.TrainingCourseID), "TrainingCourseID", "TrainingCourseName");

            dynamic MultiModel = new ExpandoObject();
            MultiModel.Training = trainingCours;
            MultiModel.UsersAll = usersTrainingAll;
            MultiModel.UsersA = usersTrainingA;
            MultiModel.UsersB = usersTrainingB;
            MultiModel.UsersC = usersTrainingC;
            MultiModel.Attendance = attendance;

            return PartialView(MultiModel);
        }

        public ActionResult GetAttendanceListA(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TrainingCours trainingCours = db.TrainingCourses.Find(id);
            if (trainingCours == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "ID", "FullName", trainingCours.CreatedBy);

            if (trainingCours.FullDescriptionBlob != null)
                trainingCours.FullDescription = Encoding.ASCII.GetString(trainingCours.FullDescriptionBlob);

            var usersTrainingAll = db.WebsiteUserTrainings.Include(u => u.WebsiteUser).Where(x => x.TrainingCourseID == id).OrderBy(x => x.WebsiteUser.FName).OrderByDescending(x => x.WebsiteUserTrainingID).ToList();

            usersTrainingAll = usersTrainingAll.GroupBy(x => x.WebsiteUserID).Select(g => g.First()).OrderBy(x => x.WebsiteUser.FName).ToList();

            ViewBag.allUsers = usersTrainingAll.Count();
            ViewBag.fullUsers = usersTrainingAll.Where(x => x.PaymentType == 3).Count();
            ViewBag.fUsers = usersTrainingAll.Where(x => x.PaymentType == 1).Count();
            ViewBag.sUsers = usersTrainingAll.Where(x => x.PaymentType == 2).Count();
            ViewBag.nUsers = usersTrainingAll.Where(x => x.PaymentType == 4).Count();
            ViewBag.TrainingCourseID = trainingCours.TrainingCourseID;
            ViewBag.TotalAppear = usersTrainingAll.Where(x => x.WebsiteUser.TakeEvaluation == true).Count();
            ViewBag.TotalPass = usersTrainingAll.Where(x => x.WebsiteUser.EvaluationStatus == true).Count();

            var usersTrainingA = usersTrainingAll.Where(x => x.TrainingGroup == "A").ToList();
            var usersTrainingB = usersTrainingAll.Where(x => x.TrainingGroup == "B").ToList();
            var usersTrainingC = usersTrainingAll.Where(x => x.TrainingGroup == "C").ToList();

            List<WebsiteUserAttendance> attendance = db.WebsiteUserAttendances.Where(x => x.TrainingCourseID == id).ToList();

            for (int i = 1; i <= 50; i++)
            {
                ViewData["A_TodayDay" + i] = attendance.Where(x => x.Day == i && x.Attended == true && x.TrainingGroup == "A").Count();
            }

            List<TrainingTrainer> trainers = db.TrainingTrainers.Where(x => x.TrainingCourseID == id).ToList();

            dynamic MultiModel = new ExpandoObject();
            MultiModel.Training = trainingCours;
            MultiModel.UsersAll = usersTrainingAll;
            MultiModel.UsersA = usersTrainingA;
            MultiModel.UsersB = usersTrainingB;
            MultiModel.UsersC = usersTrainingC;
            MultiModel.Attendance = attendance;
            MultiModel.Trainers = trainers;

            return PartialView(MultiModel);
        }

        public ActionResult GetAttendanceListB(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TrainingCours trainingCours = db.TrainingCourses.Find(id);
            if (trainingCours == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "ID", "FullName", trainingCours.CreatedBy);

            if (trainingCours.FullDescriptionBlob != null)
                trainingCours.FullDescription = Encoding.ASCII.GetString(trainingCours.FullDescriptionBlob);

            var usersTrainingAll = db.WebsiteUserTrainings.Include(u => u.WebsiteUser).Where(x => x.TrainingCourseID == id).OrderBy(x => x.WebsiteUser.FName).OrderByDescending(x => x.WebsiteUserTrainingID).ToList();

            usersTrainingAll = usersTrainingAll.GroupBy(x => x.WebsiteUserID).Select(g => g.First()).OrderBy(x => x.WebsiteUser.FName).ToList();

            ViewBag.allUsers = usersTrainingAll.Count();
            ViewBag.fullUsers = usersTrainingAll.Where(x => x.PaymentType == 3).Count();
            ViewBag.fUsers = usersTrainingAll.Where(x => x.PaymentType == 1).Count();
            ViewBag.sUsers = usersTrainingAll.Where(x => x.PaymentType == 2).Count();
            ViewBag.nUsers = usersTrainingAll.Where(x => x.PaymentType == 4).Count();
            ViewBag.TrainingCourseID = trainingCours.TrainingCourseID;
            ViewBag.TotalAppear = usersTrainingAll.Where(x => x.WebsiteUser.TakeEvaluation == true).Count();
            ViewBag.TotalPass = usersTrainingAll.Where(x => x.WebsiteUser.EvaluationStatus == true).Count();

            var usersTrainingA = usersTrainingAll.Where(x => x.TrainingGroup == "A").ToList();
            var usersTrainingB = usersTrainingAll.Where(x => x.TrainingGroup == "B").ToList();
            var usersTrainingC = usersTrainingAll.Where(x => x.TrainingGroup == "C").ToList();

            List<WebsiteUserAttendance> attendance = db.WebsiteUserAttendances.Where(x => x.TrainingCourseID == id).ToList();

            for (int i = 1; i <= 50; i++)
            {
                ViewData["B_TodayDay" + i] = attendance.Where(x => x.Day == i && x.Attended == true && x.TrainingGroup == "B").Count();
            }

            dynamic MultiModel = new ExpandoObject();
            MultiModel.Training = trainingCours;
            MultiModel.UsersAll = usersTrainingAll;
            MultiModel.UsersA = usersTrainingA;
            MultiModel.UsersB = usersTrainingB;
            MultiModel.UsersC = usersTrainingC;
            MultiModel.Attendance = attendance;

            return PartialView(MultiModel);
        }

        public ActionResult GetAttendanceListC(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TrainingCours trainingCours = db.TrainingCourses.Find(id);
            if (trainingCours == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "ID", "FullName", trainingCours.CreatedBy);

            if (trainingCours.FullDescriptionBlob != null)
                trainingCours.FullDescription = Encoding.ASCII.GetString(trainingCours.FullDescriptionBlob);

            var usersTrainingAll = db.WebsiteUserTrainings.Include(u => u.WebsiteUser).Where(x => x.TrainingCourseID == id).OrderBy(x => x.WebsiteUser.FName).OrderByDescending(x => x.WebsiteUserTrainingID).ToList();

            usersTrainingAll = usersTrainingAll.GroupBy(x => x.WebsiteUserID).Select(g => g.First()).OrderBy(x => x.WebsiteUser.FName).ToList();

            ViewBag.allUsers = usersTrainingAll.Count();
            ViewBag.fullUsers = usersTrainingAll.Where(x => x.PaymentType == 3).Count();
            ViewBag.fUsers = usersTrainingAll.Where(x => x.PaymentType == 1).Count();
            ViewBag.sUsers = usersTrainingAll.Where(x => x.PaymentType == 2).Count();
            ViewBag.nUsers = usersTrainingAll.Where(x => x.PaymentType == 4).Count();
            ViewBag.TrainingCourseID = trainingCours.TrainingCourseID;
            ViewBag.TotalAppear = usersTrainingAll.Where(x => x.WebsiteUser.TakeEvaluation == true).Count();
            ViewBag.TotalPass = usersTrainingAll.Where(x => x.WebsiteUser.EvaluationStatus == true).Count();

            var usersTrainingA = usersTrainingAll.Where(x => x.TrainingGroup == "A").ToList();
            var usersTrainingB = usersTrainingAll.Where(x => x.TrainingGroup == "B").ToList();
            var usersTrainingC = usersTrainingAll.Where(x => x.TrainingGroup == "C").ToList();

            List<WebsiteUserAttendance> attendance = db.WebsiteUserAttendances.Where(x => x.TrainingCourseID == id).ToList();

            for (int i = 1; i <= 50; i++)
            {
                ViewData["C_TodayDay" + i] = attendance.Where(x => x.Day == i && x.Attended == true && x.TrainingGroup == "A").Count();
            }

            dynamic MultiModel = new ExpandoObject();
            MultiModel.Training = trainingCours;
            MultiModel.UsersAll = usersTrainingAll;
            MultiModel.UsersA = usersTrainingA;
            MultiModel.UsersB = usersTrainingB;
            MultiModel.UsersC = usersTrainingC;
            MultiModel.Attendance = attendance;

            return PartialView(MultiModel);
        }

        public ActionResult CreateInterviewDetails(int? id)
        {

            List<WebsiteUserTraining> tc = db.WebsiteUserTrainings.Where(x => x.WebsiteUserID == id).OrderByDescending(x => x.TrainingCourseID).ToList();
            tc = tc.GroupBy(x => x.TrainingCourseID).Select(g => g.First()).ToList();

            List<User> users = db.Users.Where(x => (x.Roles_id == 4 && x.ActiveInactive == true)).OrderBy(x => x.FullName).ToList();
            List<Performance> per = new List<Performance>();
            per.Add(new Performance() { ID = 1, PerformanceDesc = "★ Bad" });
            per.Add(new Performance() { ID = 2, PerformanceDesc = "★★ Poor " });
            per.Add(new Performance() { ID = 3, PerformanceDesc = "★★★ Regular"});
            per.Add(new Performance() { ID = 4, PerformanceDesc = "★★★★ Good" });
            per.Add(new Performance() { ID = 5, PerformanceDesc = "★★★★★ Excellent" });

            ViewBag.ID_USER = new SelectList(users, "ID", "FullName");
            ViewBag.ID_USER2 = new SelectList(users, "ID", "FullName");
            ViewBag.ID_USER3 = new SelectList(users, "ID", "FullName");
            ViewBag.PERFORMANCE = new SelectList(per, "ID", "PerformanceDesc");
            
            ViewBag.ID = id;

            var model = new MockInterview();

            model.InterviewDate = DateTime.Today;
            
            model.InterviewedBy = Convert.ToInt32(Session["LoginId"]);

            if(tc.Count > 0)
            {
                ViewBag.ID_TRAINING = new SelectList(tc, "TrainingCourseID", "TrainingCours.TrainingCourseName");
                ViewBag.TRAINING = tc[0].TrainingCours.TrainingCourseName;
                model.TrainingCourseID = tc[0].TrainingCourseID;
            }

            else
            {
                var dictionary = new Dictionary<string, string>
                {
                   {"0", "Direct Marketing"},
                   {"1", "Evaluation Interview"}
                };

                //ViewBag.ID_TRAINING = new SelectList(new[] { "Direct Marketing" });
                ViewBag.ID_TRAINING = new SelectList(dictionary, "Key", "Value");
                ViewBag.TRAINING = "Direct Marketing";
                //model.TrainingCourseID = 0;
            }
            

            return PartialView("_CreateInterviewDetails", model);
        }

        [HttpPost, ValidateInput(false)]
        public async System.Threading.Tasks.Task<string> SaveMockInterviewDetails(int tcID, string iDate, int interviewBy, int interviewBy2, int interviewBy3, string performance, string notes, int wuID, string technology, string status)
        {
            try
            {
                MockInterview mi = new MockInterview();

                if (tcID != 0)
                    mi.TrainingCourseID = tcID;
                else
                    mi.TrainingCourseID = null;

                mi.Technology = technology;
                mi.InterviewDate = Convert.ToDateTime(iDate);
                mi.InterviewedBy = interviewBy;
                mi.Performance = performance;
                mi.Status = status;
                mi.Notes = notes;
                mi.WebsiteUserID = wuID;
                mi.DateCreated = DateTime.Now;
                mi.CreatedBy = Convert.ToInt32(Session["LoginId"]);

                if(interviewBy2 == 0)
                    mi.InterviewedBy2 = null;
                else
                    mi.InterviewedBy2 = interviewBy2;

                if (interviewBy3 == 0)
                    mi.InterviewedBy3 = null;
                else
                    mi.InterviewedBy3 = interviewBy3;

                db.MockInterviews.Add(mi);
                db.SaveChanges();
                ////////////////////////////////////////////////////
                
                List<User> trainers = db.Users.Where(x => ((x.Roles_id ==4 || x.Roles_id == 2) && x.ActiveInactive == true)).ToList();

                List<WebsiteUser> websiteUser = db.WebsiteUsers.Where(x => x.WebsiteUserID == wuID).ToList();

                List<TrainingCours> training = db.TrainingCourses.Where(x => x.TrainingCourseID == tcID).ToList();

                var interviewers = "";
                List<User> interviewer = db.Users.Where(x => (x.Roles_id == 4 && x.ActiveInactive == true && x.ID== interviewBy)).ToList();
                interviewers = interviewer[0].FullName;

                if (interviewBy2 != 0)
                {
                    List<User> interviewer2 = db.Users.Where(x => (x.Roles_id == 4 && x.ActiveInactive == true && x.ID == interviewBy2)).ToList();
                    interviewers = interviewers+", "+ interviewer2[0].FullName;
                }

                if (interviewBy3 != 0)
                {
                    List<User> interviewer3 = db.Users.Where(x => (x.Roles_id == 4 && x.ActiveInactive == true && x.ID == interviewBy3)).ToList();
                    interviewers = interviewers + ", " + interviewer3[0].FullName;
                }

                

                if(training.Count()>0)
                {
                    for (int i = 0; i < trainers.Count(); i++)
                    {
                        var body = $@"<body style=""text-align:center; font-family: Arial, Helvetica, sans-serif; "">
                                <div style=""position:relative; 
    `                                           padding:50px 0px 50px; 
                                                background-position:center center; 
                                                background-repeat:no-repeat; 
                                                background-size:cover; 
                                                background-image:url('https://brightrace.com/ui/assets/images/inner-bg.jpg');"">
                                    <img src=""https://brightrace.com/ui/assets/images/logo-2.png"" alt=""Brightrace"" style=""width:200px"">
                                    <h1>Interview Report</h1><br/>
                                    <table style=""margin-left:auto; margin-right:auto; width:450px; border:none; text-align:justify; border-radius:0px;"" cellpadding=""0"" cellspacing=""0"">
                                    <tbody style=""background-color:f7f8fd;""> 
                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;"" > Hi <b> {trainers[i].FullName} </b>,</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;"" > <b>Candidate:</b> { websiteUser[0].FName + " " + websiteUser[0].LName }</td>
                                        </tr>
                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;"" > <b>Training Program: </b> {training[0].TrainingCourseName}</td>
                                        </tr>

                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;"" > <b>Technology: </b> {technology}</td>
                                        </tr>
                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;"" > <b>Interviewed on: </b> {Convert.ToDateTime(iDate).ToString("MM/dd/yyyy")}</td>
                                        </tr>
                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;"" > <b>Interviewed by: </b> {interviewers} </td>
                                        </tr>
                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;"" > <b>Performance: </b> {performance}</td>
                                        </tr>
                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;"" > <b>Notes/Comments: </b> {notes}</td>
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

                        var subject = "Interview Report - " + websiteUser[0].FName + " " + websiteUser[0].LName;

                        SendEmail se = new SendEmail();
                        await se.sendEmailAsync(subject, body, trainers[i].Email, null);
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                return e.ToString();
            }


        }

        public ActionResult Ready4Resume(int? id)
        {
            List<User> users = db.Users.Where(x => (x.Roles_id == 2 || x.Roles_id == 3 || x.Roles_id == 4 || x.Roles_id == 5) && x.ActiveInactive == true ).OrderBy(x => x.FullName).ToList();
            List<WebsiteUserTraining> tc = db.WebsiteUserTrainings.Where(x => x.WebsiteUserID == id).OrderByDescending(x => x.TrainingCourseID).ToList();
            WebsiteUser wu = db.WebsiteUsers.Find(id);

            ViewBag.ID_TRAINING = new SelectList(tc, "TrainingCourseID", "TrainingCours.TrainingCourseName");
            ViewBag.ID_USER = new SelectList(users, "ID", "FullName");
            ViewBag.TRAINING = tc[0].TrainingCours.TrainingCourseName;
            ViewBag.NAME = wu.FName + " " + wu.LName;
            ViewBag.ID = id;

            return PartialView("_NotifyReady4Resume");
        }

        [HttpPost]
        public ActionResult AttendEvaluation(int userID, int status)
        {
            WebsiteUser user = db.WebsiteUsers.Find(userID);
            db.Entry(user).State = EntityState.Modified;
            user.TakeEvaluation = Convert.ToBoolean(status);
            db.SaveChanges();

            return null;
        }

        [HttpPost]
        public ActionResult PassFailEvaluation(int userID, int status)
        {
            WebsiteUser user = db.WebsiteUsers.Find(userID);
            db.Entry(user).State = EntityState.Modified;
            user.EvaluationStatus = Convert.ToBoolean(status);
            db.SaveChanges();

            return null;
        }

        [HttpPost]
        public ActionResult SignedContract(int userID, int status)
        {
            WebsiteUser user = db.WebsiteUsers.Find(userID);
            db.Entry(user).State = EntityState.Modified;
            user.SignedContract = Convert.ToBoolean(status);
            db.SaveChanges();

            return null;
        }

        public ActionResult EnrrollNewUser(int? id)
        {
            ViewBag.tID = id;
            return PartialView("_SearchUserToEnrroll");
        }

        [HttpPost]
        public ActionResult SearchUserToEnrroll(int? tID, string searchString)
        {
            var userList = db.WebsiteUsers.Where(x => (x.FName.Contains(searchString) || x.LName.Contains(searchString) || x.Email.Contains(searchString))).ToList();

            ViewBag.tID = tID;

            if (userList.Count > 0)
                return PartialView("GetUserList", userList);
            else
                return null;
        }

        [HttpPost]
        public ActionResult EnrrollSelectedUser(int? uID, int? tID)
        {
            WebsiteUserTraining data = new WebsiteUserTraining();
            data.TrainingCourseID = Convert.ToInt32(tID);
            data.WebsiteUserID = Convert.ToInt32(uID);
            data.Amount = 0;
            data.PaymentStatus = "NO PAYMENT REQUIRED(NPR)";
            data.PaymentDate = DateTime.Now;
            data.TrainingStatus = "Enrolled";
            data.PaymentType = 5;
            data.Discount = 100;
            data.TrainingGroup = "A";

            db.WebsiteUserTrainings.Add(data);
            db.SaveChanges();


            return null;
        }

        [HttpPost]
        public ActionResult SaveTrainingAttendance(int? uID, int? tID, int? day, string attended, string group)
        {
            WebsiteUserAttendance data = new WebsiteUserAttendance();

            List<WebsiteUserAttendance> attendance = db.WebsiteUserAttendances.Where(x => (x.WebsiteUserID == uID && x.TrainingCourseID == tID && x.Day == day)).ToList();

            data.TrainingCourseID = Convert.ToInt32(tID);
            data.WebsiteUserID = Convert.ToInt32(uID);
            data.DateRecord = DateTime.Now;
            data.CreatedBy = Convert.ToInt32(Session["LoginId"]);
            data.Day = day;
            data.TrainingGroup = group;

            if (attended == "Yes")
            {
                data.Attended = true;
            }
                
            if (attended == "No")
            {
                data.Attended = false;
            }
                

            if (attendance.Count == 0)
            {
                db.WebsiteUserAttendances.Add(data);
                db.SaveChanges();
            }

            else
            {
                
                attendance[0].WebsiteUserAttendanceID = attendance[0].WebsiteUserAttendanceID;
                attendance[0].TrainingCourseID = Convert.ToInt32(tID);
                attendance[0].WebsiteUserID = Convert.ToInt32(uID);

                attendance[0].DateRecord = DateTime.Now;
                attendance[0].CreatedBy = Convert.ToInt32(Session["LoginId"]);
                attendance[0].Day = day;
                attendance[0].TrainingGroup = group;

                if (attended.Equals("Yes"))
                {
                    attendance[0].Attended = true;

                    db.Entry(attendance[0]).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (attended.Equals("No"))
                {
                    attendance[0].Attended = false;

                    db.Entry(attendance[0]).State = EntityState.Deleted;
                    db.SaveChanges();
                }

                
                
                
            }

            var usersTrainingAll = db.WebsiteUserTrainings.Where(x => x.TrainingCourseID == tID).ToList();
            usersTrainingAll = usersTrainingAll.GroupBy(x => x.WebsiteUserID).Select(g => g.First()).OrderBy(x => x.WebsiteUser.FName).ToList();

            var usersTrainingA = usersTrainingAll.Where(x => x.TrainingGroup == "A").ToList();
            var usersTrainingB = usersTrainingAll.Where(x => x.TrainingGroup == "B").ToList();
            var usersTrainingC = usersTrainingAll.Where(x => x.TrainingGroup == "C").ToList();

            var totalDay = 0;

            List<WebsiteUserAttendance> totalAttendance = db.WebsiteUserAttendances.Where(x => x.TrainingCourseID == tID).ToList();

            if (group == "A")
            {
                totalDay = totalAttendance.Where(x => x.Day == day && x.Attended == true && x.TrainingGroup == "A").Count();
            }

            if (group == "B")
            {
                totalDay = totalAttendance.Where(x => x.Day == day && x.Attended == true && x.TrainingGroup == "B").Count();
            }

            if (group == "C")
            {
                totalDay = totalAttendance.Where(x => x.Day == day && x.Attended == true && x.TrainingGroup == "C").Count();
            }

            return Content(totalDay.ToString());
        }

        [HttpPost]
        public ActionResult UpdateUserGroup(int? uID, int? tID, string group)
        {
            List<WebsiteUserTraining> wut = db.WebsiteUserTrainings.Where(x => (x.WebsiteUserID == uID && x.TrainingCourseID == tID)).ToList();

            for (int i = 0; i < wut.Count; i++)
            {
                wut[i].TrainingGroup = group;
                db.Entry(wut[i]).State = EntityState.Modified;
                db.SaveChanges();
            }

            List<WebsiteUserAttendance> wua = db.WebsiteUserAttendances.Where(x => (x.WebsiteUserID == uID && x.TrainingCourseID == tID)).ToList();

            for (int i = 0; i < wua.Count; i++)
            {
                wua[i].TrainingGroup = group;
                db.Entry(wua[i]).State = EntityState.Modified;
                db.SaveChanges();
            }

            return null;
        }

        [HttpPost]
        public ActionResult SwitchUserTraining(int? uID, int? tID, int currentTID)
        {
            List<WebsiteUserTraining> wut = db.WebsiteUserTrainings.Where(x => (x.WebsiteUserID == uID && x.TrainingCourseID == currentTID)).ToList();

            for (int i = 0; i < wut.Count; i++)
            {
                wut[i].TrainingCourseID = Convert.ToInt32(tID);
                db.Entry(wut[i]).State = EntityState.Modified;
                db.SaveChanges();
            }

            List<WebsiteUserAttendance> wua = db.WebsiteUserAttendances.Where(x => (x.WebsiteUserID == uID && x.TrainingCourseID == currentTID)).ToList();

            for (int i = 0; i < wua.Count; i++)
            {
                wua[i].TrainingCourseID = Convert.ToInt32(tID);
                db.Entry(wua[i]).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("ViewTrainingCourse", new { id = currentTID });
        }

        public ActionResult GetScheduleView(int? id)
        {
            if(id != null)
                ViewBag.TrainingCourseID = id;
            else
                ViewBag.TrainingCourseID = 0; 

            return PartialView("GetScheduler");
        }

        public ActionResult GetScheduleByTraining(int? id)
        {
            List<Schedule> sc = new List<Schedule>();
            if (id != 0)
                sc = db.Schedules.Where(x => x.TrainingCourseID == id).ToList();
            else
                sc = db.Schedules.ToList();

            var schedule = new List<ScheduleCustom>();
            //var schedule = db.Schedules.ToList();

            foreach (var item in sc)
            {
                var type = "";
                var color = "";

                if (item.ScheduleType == 1)
                {
                    type = "Mock Interview (Backend)";
                    color = "orange";
                }

                else if (item.ScheduleType == 2)
                {
                    type = "Mock Interview (Frontend)";
                    color = "blue";
                }

                else if (item.ScheduleType == 3)
                {
                    type = "Client Interview";
                    color = "red";
                }

                else if (item.ScheduleType == 4)
                {
                    type = "Project Presentation";
                    color = "green";
                }

                else if (item.ScheduleType == 5)
                {
                    type = "Session";
                    color = "purple";
                }

                else if (item.ScheduleType == 6)
                {
                    type = "Other";
                    color = "grey";
                }


                schedule.Add(new ScheduleCustom()
                {
                    ScheduleID = item.ScheduleID,
                    TrainingCourseName = item.TrainingCours.TrainingCourseName,
                    WebsiteUserName = item.WebsiteUser.FName+" "+item.WebsiteUser.LName,
                    scheduledBy = item.User.FullName,
                    Description = item.Description,
                    Subject = item.Subject,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    ThemeColor = color,
                    IsFullDay = item.IsFullDay,
                    Type = type
                });

            }

            return Json(new { Data = schedule }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddtoScheduler(int? id)
        {

            TrainingCours tc = db.TrainingCourses.Find(id);

            var users = db.WebsiteUserTrainings.Where(x => x.TrainingCourseID == id).Select(x => new 
            {
                WebsiteUserID = x.WebsiteUser.WebsiteUserID,
                payerID = x.WebsiteUser.FName + " " + x.WebsiteUser.LName
            }).OrderBy(x => x.payerID).ToList();

            users.Add(new { WebsiteUserID = 0, payerID = "**All Candidates**" });

            users = users.OrderBy(x => x.payerID).ToList();

            List<ScheduleCustom> sch = new List<ScheduleCustom>();
            sch.Add(new ScheduleCustom() { ScheduleID = 1, Description = "Mock Interview (Backend)" });
            sch.Add(new ScheduleCustom() { ScheduleID = 2, Description = "Mock Interview (Frontend)" });
            sch.Add(new ScheduleCustom() { ScheduleID = 3, Description = "Client Interview" });
            sch.Add(new ScheduleCustom() { ScheduleID = 4, Description = "Project Presentation" });
            sch.Add(new ScheduleCustom() { ScheduleID = 5, Description = "Session" });
            sch.Add(new ScheduleCustom() { ScheduleID = 6, Description = "Other" });

            ViewBag.ID_USER = new SelectList(users, "WebsiteUserID", "payerID");
            ViewBag.SCHEDULE_FOR = new SelectList(sch, "ScheduleID", "Description");

            ViewBag.ID = id;
            ViewBag.TRAINING = tc.TrainingCourseName;

            return PartialView("_CreateScheduleDetails");
        }

        [HttpPost, ValidateInput(false)]
        public async System.Threading.Tasks.Task<string> SaveScheduleDetails(int tcID, int wuID, int scType, string sDate, string eDate, string notes)
        {
            try
            {
                
                var type = "";
                var color = "";

                if (scType == 1)
                {
                    type = "Mock Interview (Backend)";
                    color = "orange";
                }

                else if (scType == 2)
                {
                    type = "Mock Interview (Frontend)";
                    color = "blue";
                }

                else if (scType == 3)
                {
                    type = "Client Interview";
                    color = "red";
                }

                else if (scType == 4)
                {
                    type = "Project Presentation";
                    color = "green";
                }

                else if (scType == 5)
                {
                    type = "Session";
                    color = "purple";
                }

                else if (scType == 6)
                {
                    type = "Other";
                    color = "grey";
                }

                List<WebsiteUser> candidate = new List<WebsiteUser>();

                if (wuID != 0)
                {
                    candidate = db.WebsiteUsers.Where(x => x.WebsiteUserID == wuID).ToList();
                }
                else
                {
                    List<WebsiteUserTraining> wut = new List<WebsiteUserTraining>();
                    wut = db.WebsiteUserTrainings.Include(x => x.WebsiteUser).Where(x => x.TrainingCourseID == tcID).ToList();

                    for (int i = 0; i < wut.Count; i++)
                    {
                        candidate.Add(new WebsiteUser { WebsiteUserID = wut[i].WebsiteUser.WebsiteUserID, FName = wut[i].WebsiteUser.FName, LName = wut[i].WebsiteUser.LName, Email = wut[i].WebsiteUser.Email });
                    }

                }

                User trainer = db.Users.Find(Convert.ToInt32(Session["LoginId"]));
                SendEmail se = new SendEmail();
                var subject = "";
                var body = "";

                for (int i = 0; i < candidate.Count; i++)
                {
                    Schedule sc = new Schedule();

                    sc.TrainingCourseID = tcID;
                    sc.WebsiteUserID = candidate[i].WebsiteUserID;
                    sc.ScheduleType = scType;
                    sc.StartDate = Convert.ToDateTime(sDate);
                    sc.EndDate = Convert.ToDateTime(eDate);
                    sc.CreatedOn = DateTime.Now;
                    sc.CreatedBy = Convert.ToInt32(Session["LoginId"]);
                    sc.Subject = candidate[i].FName + " " + candidate[i].LName + " " + type;
                    sc.Description = notes;
                    sc.IsFullDay = false;
                    sc.ThemeColor = color;

                    db.Schedules.Add(sc);
                    db.SaveChanges();
                    ////////////////////////////////////////////////////
                    
                    TrainingCours training = db.TrainingCourses.Find(tcID);

                    body = $@"<body style=""text-align:center; font-family: Arial, Helvetica, sans-serif; "">
                        <div style=""position:relative; 
`                                           padding:50px 0px 50px; 
                                        background-position:center center; 
                                        background-repeat:no-repeat; 
                                        background-size:cover; 
                                        background-image:url('https://brightrace.com/ui/assets/images/inner-bg.jpg');"">
                            <img src=""https://brightrace.com/ui/assets/images/logo-2.png"" alt=""Brightrace"" style=""width:200px"">
                            <h1>{sc.Subject}</h1><br/>
                            <table style=""margin-left:auto; margin-right:auto; width:450px; border:none; text-align:justify; border-radius:0px;"" cellpadding=""0"" cellspacing=""0"">
                            <tbody style=""background-color:f7f8fd;""> 
                                <tr>
                                    <td style=""padding: 10px 30px 10px 30px;"" > <b>Candidate:</b> { candidate[i].FName + " " + candidate[i].LName }</td>
                                </tr>
                                <tr>
                                    <td style=""padding: 10px 30px 10px 30px;"" > <b>Training Program: </b> {training.TrainingCourseName}</td>
                                </tr>

                                <tr>
                                    <td style=""padding: 10px 30px 10px 30px;"" > <b>Scheduled for: </b> {type}</td>
                                </tr>
                                <tr>
                                    <td style=""padding: 10px 30px 10px 30px;"" > <b>Date: </b> {Convert.ToDateTime(sc.StartDate)} - {Convert.ToDateTime(sc.EndDate)}</td>
                                </tr>
                                <tr>
                                    <td style=""padding: 10px 30px 10px 30px;"" > <b>Scheduled by: </b> {trainer.FullName} </td>
                                </tr>
                                <tr>
                                    <td style=""padding: 10px 30px 10px 30px;"" > <b>Notes/Comments: </b> {notes}</td>
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

                subject = sc.Subject;
                await se.sendEmailAsync(subject, body, candidate[i].Email, null);
                await se.sendEmailAsync(subject, body, trainer.Email, null);
                }

                


                return null;
            }
            catch (Exception e)
            {
                return e.ToString();
            }


        }

        public ActionResult DeleteScheduledEvent(int? id, int? tID)
        {
            ViewBag.ID = id;
            ViewBag.tID = tID;

            return PartialView("_ConfirmDeleteScheduledEvent");
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> ConfirmDeleteScheduledEvent(int id)
        {
            Schedule sch = db.Schedules.Find(id);
            User trainer = db.Users.Find(Convert.ToInt32(Session["LoginId"]));
            TrainingCours training = db.TrainingCourses.Find(sch.TrainingCourseID);
            WebsiteUser candidate = db.WebsiteUsers.Find(sch.WebsiteUserID);

            db.Schedules.Remove(sch);
            db.SaveChanges();

            var type = "Other";
            if (sch.ScheduleType == 1)
            {
                type = "Backend Mock Interview";
            }

            else if (sch.ScheduleType == 2)
            {
                type = "Frontend Mock Interview";
            }

            else if (sch.ScheduleType == 3)
            {
                type = "Presentation";
            }

            var body = $@"<body style=""text-align:center; font-family: Arial, Helvetica, sans-serif; "">
                        <div style=""position:relative; 
`                                           padding:50px 0px 50px; 
                                        background-position:center center; 
                                        background-repeat:no-repeat; 
                                        background-size:cover; 
                                        background-image:url('https://brightrace.com/ui/assets/images/inner-bg.jpg');"">
                            <img src=""https://brightrace.com/ui/assets/images/logo-2.png"" alt=""Brightrace"" style=""width:200px"">
                            <h1>Canceled: {sch.Subject}</h1><br/>
                            <table style=""margin-left:auto; margin-right:auto; width:450px; border:none; text-align:justify; border-radius:0px;"" cellpadding=""0"" cellspacing=""0"">
                            <tbody style=""background-color:f7f8fd;""> 
                                <tr>
                                    <td style=""padding: 10px 30px 10px 30px;"" > <b>Candidate:</b> { candidate.FName + " " + candidate.LName }</td>
                                </tr>
                                <tr>
                                    <td style=""padding: 10px 30px 10px 30px;"" > <b>Training Program: </b> {training.TrainingCourseName}</td>
                                </tr>

                                <tr>
                                    <td style=""padding: 10px 30px 10px 30px;"" > <b>Canceled for: </b> {type}</td>
                                </tr>
                                <tr>
                                    <td style=""padding: 10px 30px 10px 30px;"" > <b>Date: </b> {Convert.ToDateTime(sch.StartDate)} - {Convert.ToDateTime(sch.EndDate)}</td>
                                </tr>
                                <tr>
                                    <td style=""padding: 10px 30px 10px 30px;"" > <b>Canceled by: </b> {trainer.FullName} </td>
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

            var subject = "Canceled "+sch.Subject;

            SendEmail se = new SendEmail();
            await se.sendEmailAsync(subject, body, trainer.Email, null);
            await se.sendEmailAsync(subject, body, candidate.Email, null);

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

