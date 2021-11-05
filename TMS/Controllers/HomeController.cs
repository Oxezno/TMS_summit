using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using TMS.Models;

namespace TMS.Controllers
{
    public class HomeController : Controller
    {
        private TMSEntities db = new TMSEntities();

        private IEnumerable<SelectListItem> GrossMarginRptOpt()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "In-House", Value = "1" });
            items.Add(new SelectListItem { Text = "On-Project", Value = "2" });
            List<SelectListItem> sl = new SelectList(items, "Value", "Text").ToList();
            return new SelectList(sl, "Value", "Text");
        }

        private IEnumerable<SelectListItem> getProfileListFilter()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Total Profiles", Value = "1" });
            items.Add(new SelectListItem { Text = "Offers Accepted", Value = "2" });
            items.Add(new SelectListItem { Text = "Appeared in 1st Day", Value = "3" });
            items.Add(new SelectListItem { Text = "Appeared in Evaluation", Value = "4" });
            items.Add(new SelectListItem { Text = "After Evaluation", Value = "5" });
            items.Add(new SelectListItem { Text = "Left SWT", Value = "6" });
            List<SelectListItem> sl = new SelectList(items, "Value", "Text").ToList();
            return new SelectList(sl, "Value", "Text");
        }

        private IEnumerable<SelectListItem> getUsersList()
        {
            using (TMSEntities db = new TMSEntities())
            {
                var data = from p in db.Users.ToList()
                           where p.ActiveInactive == true
                           select new
                           {
                               Text = p.FullName,
                               Value = p.ID
                           };

                List<SelectListItem> sl = new SelectList(data, "Value", "Text").ToList();
                return new SelectList(sl, "Value", "Text");
            }
        }

        private IEnumerable<SelectListItem> getUsersARTS()
        {
            using (TMSEntities db = new TMSEntities())
            {
                var data = from p in db.Users.ToList()
                           where p.ActiveInactive == true && (p.Roles_id == 2 || p.Roles_id == 3 || p.Roles_id == 4 || p.Roles_id == 5)
                           select new
                           {
                               Text = p.FullName,
                               Value = p.ID
                           };

                List<SelectListItem> sl = new SelectList(data, "Value", "Text").ToList();
                return new SelectList(sl, "Value", "Text");
            }
        }

        private IEnumerable<SelectListItem> getUsersAS()
        {
            using (TMSEntities db = new TMSEntities())
            {
                var data = from p in db.Users.ToList()
                           where p.ActiveInactive == true && (p.Roles_id == 2 || p.Roles_id == 5)
                           select new
                           {
                               Text = p.FullName,
                               Value = p.ID
                           };

                List<SelectListItem> sl = new SelectList(data, "Value", "Text").ToList();
                return new SelectList(sl, "Value", "Text");
            }
        }

        private IEnumerable<SelectListItem> getProfileInteractionResult()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "A",
                Value = "A"
            });
            items.Add(new SelectListItem
            {
                Text = "R",
                Value = "R"
            });
            List<SelectListItem> sl = new SelectList(items, "Value", "Text").ToList();
            return new SelectList(sl, "Value", "Text");
        }

        private IEnumerable<SelectListItem> GetRoles()
        {
            using (TMSEntities db = new TMSEntities())
            {
                var data = from p in db.RolePermissions.ToList()
                           select new
                           {
                               Text = p.Role,
                               Value = p.Id
                           };

                List<SelectListItem> sl = new SelectList(data, "Value", "Text").ToList();
                return new SelectList(sl, "Value", "Text");
            }
        }

        [AllowAnonymous]
        public ActionResult NoAccess()
        { return View(); }

        public ActionResult Index()
        {
            getDashBoard_Result res = (new TMSEntities()).getDashBoard().SingleOrDefault();

            DashBoaradInfo info = new DashBoaradInfo();

            info.InEval = res.InEval;
            info.SelTrain = res.SelTrain;
            info.ClientPlaced = res.ClientPlaced;
            info.InMarket = res.InMarket;

            info.Market = (new TMSEntities()).getDashBoardMarket().ToList<getDashBoardMarket_Result>();

            info.Training = (new TMSEntities()).getDashBoardTraining().ToList<getDashBoardTraining_Result>();

            return View(info);
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            UserLogin u = new UserLogin();
            if (Request.Cookies["username"] != null)
            {
                u.username = Request.Cookies["username"].Value;
                u.Remember = true;
            }
            return View(u);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin ul)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Users u = (from p in (new TMSEntities()).Users.ToList()
                               join r in (new TMSEntities()).RolePermissions.ToList() on p.Roles_id equals r.Id
                               where p.username == ul.username && p.password == (new ValidateModels()).Encrypt(ul.password) && p.ActiveInactive == true
                               select new Users
                               {
                                   FullName = p.FullName,
                                   ID = p.ID,
                                   Email = p.Email,
                                   username = p.username,
                                   Roles_id = p.Roles_id,
                                   RoleName = r.Role,
                                   Settings = r.Settings,
                                   WebsiteDashboard = r.WebsiteDashboard,
                                   AdminWebsite = r.AdminWebsite
                               }).SingleOrDefault();

                    if (u != null)
                    {
                        Session["LoginFullName"] = u.FullName;
                        Session["LoginId"] = u.ID;
                        Session["LoginEmail"] = u.Email;
                        Session["RolesId"] = u.Roles_id;
                        Session["RoleName"] = u.RoleName;
                        Session["Dashboard"] = u.Dashboard;
                        Session["BatchList"] = u.BatchList;
                        Session["Placements"] = u.Placements;
                        Session["TraineePortal"] = u.TraineePortal;
                        Session["AdminPortal"] = u.AdminPortal;
                        Session["Settings"] = u.Settings;
                        Session["Marketing"] = u.Marketing;
                        Session["WebsiteDashboard"] = u.WebsiteDashboard;
                        Session["AdminWebsite"] = u.AdminWebsite;

                        FormsAuthentication.SetAuthCookie(ul.username, ul.Remember);

                        if (ul.Remember)
                        {
                            Response.Cookies["username"].Expires = DateTime.Now.AddDays(30);
                        }
                        else
                        {
                            Response.Cookies["username"].Expires = DateTime.Now.AddDays(-1);
                        }

                        Response.Cookies["username"].Value = u.username.Trim();

                        if (u.Dashboard)
                        { return RedirectToAction("Index", "WebsiteDashboard"); }
                        else if (u.BatchList)
                        { return RedirectToAction("BatchesList", "Home"); }
                        else if (u.Placements)
                        { return RedirectToAction("PlacementList", "Home"); }
                        else if (u.TraineePortal)
                        { return RedirectToAction("TraineePortal", "Trainee"); }
                        else if (u.AdminPortal)
                        { return RedirectToAction("AdminPortal", "Home"); }
                        else if (u.AdminWebsite)
                        { return RedirectToAction("Index", "WebsiteDashboard"); }
                        else if (u.WebsiteDashboard)
                        { return RedirectToAction("Index", "WebsiteDashboard"); }
                        else if (u.Settings)
                        { return RedirectToAction("UsersList", "Home"); }
                        else
                        { return RedirectToAction("NoAccess", "Home"); }
                    }
                    else
                    {
                        TempData["LoginError"] = "Username/Password is incorrect, please try again.";
                    }
                    return View(ul);
                }
                else
                {
                    return View(ul);
                }

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            if (Session != null)
            {
                if (Session["RolesId"].ToString() == "6")
                { return RedirectToAction("Login", "Trainee"); }
                else
                { return RedirectToAction("Login", "Home"); }
            }
            else
            { return RedirectToAction("Login", "Home"); }
        }

        [HttpPost]
        public ActionResult UserSearch(FormCollection form)
        {
            string str = form["txtSearch"].ToString();
            return RedirectToAction("UsersList", "Home", new { searchString = str });
        }


        public ActionResult UsersList(string sortOrder, string searchString, int? page)
        {
            Session.Remove("PFlag");
            try { } catch { }

            int pageSize = Convert.ToInt32(20);
            int pageNumber = (page ?? 1);
            ViewBag.PageNum = pageNumber;
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = "";
            }
            ViewBag.Search = searchString;
            TempData.Keep("Search");

            List<GetUsersList_Result> Result = (new TMSEntities()).GetUsersList(pageNumber, pageSize, sortOrder, searchString.Trim()).ToList<GetUsersList_Result>();

            int count = Result.Count;
            int? Total;
            if (count > 0)
            {
                Total = (int?)Result[0].TotalCount;
            }
            else
            {

                Total = 0;
            }
            return View(new StaticPagedList<GetUsersList_Result>(Result, Convert.ToInt32(pageNumber), Convert.ToInt32(pageSize), (int)Total));
        }

        [HttpGet]
        public ActionResult UserSetup(int Id)
        {
            ViewBag.Roles = GetRoles();

            var item = (new TMSEntities()).Users.SingleOrDefault(x => x.ID == Id);

            Users u = new Users();

            if (item != null)
            {
                u.ID = item.ID;
                u.FullName = item.FullName;
                u.username = item.username;
                u.password = item.password;
                u.Email = item.Email;
                u.Roles_id = item.Roles_id;
                u.ActiveInactive = item.ActiveInactive;
                u.LastLoginDate = item.LastLoginDate;
            }
            else
            {
                u.ActiveInactive = true;
            }
            return View(u);
        }

        [HttpPost]
        public ActionResult UserSetup(Users item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TMSEntities Context = new TMSEntities())
                    {
                        if (item.ID > 0)
                        {
                            User u = (from p in Context.Users
                                      where p.ID == item.ID
                                      select p).SingleOrDefault();
                            u.FullName = item.FullName;
                            u.username = item.username;
                            u.password = (new ValidateModels()).Encrypt(item.password);
                            u.Email = item.Email;
                            u.Roles_id = item.Roles_id;
                            u.ActiveInactive = item.ActiveInactive;
                            Context.SaveChanges();
                        }
                        else
                        {
                            User ord = new User
                            {
                                FullName = item.FullName,
                                username = item.username,
                                password = (new ValidateModels()).Encrypt(item.password),
                                Email = item.Email,
                                Roles_id = item.Roles_id,
                                ActiveInactive = item.ActiveInactive
                            };
                            Context.Users.Add(ord);
                            Context.SaveChanges();
                        }
                    }
                    TempData["ErrorMessage"] = "User saved Successfully";
                    TempData["color"] = "green";

                    return RedirectToAction("UsersList", "Home");
                }
                else
                {
                    ViewBag.Roles = GetRoles();
                    return View(item);
                }

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        [HttpGet]
        public ActionResult ConformUserStatuspopup(int id)
        {
            ViewBag.ID = id;

            return PartialView("_ConformUserStatus");
        }

        [HttpPost]
        public ActionResult ConUserStatus(int id)
        {
            using (TMSEntities Context = new TMSEntities())
            {
                if (id > 0)
                {
                    User result = (from p in Context.Users
                                   where p.ID == id
                                   select p).SingleOrDefault();
                    result.ActiveInactive = (result.ActiveInactive == true ? false : true);
                    Context.SaveChanges();
                }
            }

            TempData["ErrorMessage"] = "User Status Changed Successfully";
            TempData["color"] = "green";

            return RedirectToAction("UsersList", "Home");
        }


        [HttpGet]
        public ActionResult ChangePwd(int iFlag)
        {
            ChangePwd cp = new ChangePwd();
            cp.Flag = iFlag;

            return View(cp);
        }


        [HttpPost]
        public ActionResult BatchSearch(FormCollection form)
        {
            string str = form["txtSearch"].ToString();
            return RedirectToAction("BatchesList", "Home", new { searchString = str });
        }


        public ActionResult BatchesList(string sortOrder, string searchString, int? page)
        {
            Session.Remove("PFlag");
            try { } catch { }

            int pageSize = Convert.ToInt32(20);
            int pageNumber = (page ?? 1);
            ViewBag.PageNum = pageNumber;
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = "";
            }
            ViewBag.Search = searchString;
            TempData.Keep("Search");

            List<GetBatchesList_Result> Result = (new TMSEntities()).GetBatchesList(pageNumber, pageSize, sortOrder, searchString.Trim()).ToList<GetBatchesList_Result>();

            int count = Result.Count;
            int? Total;
            if (count > 0)
            {
                Total = (int?)Result[0].TotalCount;
            }
            else
            {

                Total = 0;
            }
            return View(new StaticPagedList<GetBatchesList_Result>(Result, Convert.ToInt32(pageNumber), Convert.ToInt32(pageSize), (int)Total));
        }

        [HttpGet]
        public ActionResult OpenAttendanceConfirm(int id, int BId, int Dayflag, int? page, string sortOrder, string searchString)
        {
            ViewBag.ID = id;
            ViewBag.BID = BId;
            ViewBag.Dayflag = Dayflag;
            ViewBag.Page = page;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchString = searchString;

            return PartialView("_ProfileAttendance");
        }

        //[HttpPost]
        //public ActionResult PAttend(int PID, bool attendedFlag)
        //{
        //    using (TMSEntities Context = new TMSEntities())
        //    {

        //        Profile_users result = (from p in Context.Profile_users
        //                                where p.PID == PID
        //                                select p).SingleOrDefault();
        //        result.FirstDayShowup = attendedFlag;
        //        Context.SaveChanges();
        //        TempData["ErrorMessage"] = "Offer Accepted Status Changed Successfully";
        //        TempData["color"] = "green";
        //        return RedirectToAction("ProfileList", "Home", new { BatchId = PID });
        //    }
        //}

        [HttpGet]

        public ActionResult ConformBatchStatuspopup(int id)
        {
            ViewBag.ID = id;

            return PartialView("_ConformBatchStatus");
        }

        [HttpGet]

        public ActionResult EditOfferLetterPopup(int id)
        {
            ViewBag.ID = id;

            return PartialView("_EditOfferLetter");
        }

        
        
        [HttpGet]
        public void MemberProfileExcel(int BatchId)
        {
            var data = db.GetBatchCandidates(BatchId);
            var dataClassroom = data;

            int row = 2;
            int rowClassroom = 2;
            int rowOnline = 2;
            //if(data.Count() > 0  )
            {
                var bName = "";
                var bDate = "";

                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Candidates List");
                ExcelWorksheet SheetClassroom = Ep.Workbook.Worksheets.Add("Classroom Candidates");
                ExcelWorksheet SheetOnline = Ep.Workbook.Worksheets.Add("Online Candidates");

                Sheet.Cells["A1"].Value = "Created On";
                Sheet.Cells["B1"].Value = "#";
                Sheet.Cells["C1"].Value = "First Name";
                Sheet.Cells["D1"].Value = "Last Name";
                Sheet.Cells["E1"].Value = "Phone Number";
                Sheet.Cells["F1"].Value = "Email";
                Sheet.Cells["G1"].Value = "SkypeID";
                Sheet.Cells["H1"].Value = "Birthday";
                Sheet.Cells["I1"].Value = "Age";
                Sheet.Cells["J1"].Value = "Gender";
                Sheet.Cells["K1"].Value = "Address";
                Sheet.Cells["L1"].Value = "Nationality";
                Sheet.Cells["M1"].Value = "Immigration Status";
                Sheet.Cells["N1"].Value = "VisaHelp";
                Sheet.Cells["O1"].Value = "Applied_Via";
                Sheet.Cells["P1"].Value = "Recruiter";
                Sheet.Cells["Q1"].Value = "Company";
                Sheet.Cells["R1"].Value = "Education";
                Sheet.Cells["S1"].Value = "ExperienceYears";
                Sheet.Cells["T1"].Value = "Criminal Records";
                Sheet.Cells["U1"].Value = "BackgroundCheck";
                Sheet.Cells["V1"].Value = "AccommodationHelp";
                Sheet.Cells["W1"].Value = "Willing to Relocate";
                Sheet.Cells["X1"].Value = "Training Mode";
                Sheet.Cells["Y1"].Value = "Experience Before(1-5)";


                Sheet.Cells["A1"].Style.Font.Bold = true;
                Sheet.Cells["B1"].Style.Font.Bold = true;
                Sheet.Cells["C1"].Style.Font.Bold = true;
                Sheet.Cells["D1"].Style.Font.Bold = true;
                Sheet.Cells["E1"].Style.Font.Bold = true;
                Sheet.Cells["F1"].Style.Font.Bold = true;
                Sheet.Cells["G1"].Style.Font.Bold = true;
                Sheet.Cells["H1"].Style.Font.Bold = true;
                Sheet.Cells["I1"].Style.Font.Bold = true;
                Sheet.Cells["J1"].Style.Font.Bold = true;
                Sheet.Cells["K1"].Style.Font.Bold = true;
                Sheet.Cells["L1"].Style.Font.Bold = true;
                Sheet.Cells["M1"].Style.Font.Bold = true;
                Sheet.Cells["N1"].Style.Font.Bold = true;
                Sheet.Cells["O1"].Style.Font.Bold = true;
                Sheet.Cells["P1"].Style.Font.Bold = true;
                Sheet.Cells["Q1"].Style.Font.Bold = true;
                Sheet.Cells["R1"].Style.Font.Bold = true;
                Sheet.Cells["S1"].Style.Font.Bold = true;
                Sheet.Cells["T1"].Style.Font.Bold = true;
                Sheet.Cells["U1"].Style.Font.Bold = true;
                Sheet.Cells["V1"].Style.Font.Bold = true;
                Sheet.Cells["W1"].Style.Font.Bold = true;
                Sheet.Cells["X1"].Style.Font.Bold = true;
                Sheet.Cells["Y1"].Style.Font.Bold = true;

                SheetClassroom.Cells["A1"].Value = "Created On";
                SheetClassroom.Cells["B1"].Value = "#";
                SheetClassroom.Cells["C1"].Value = "First Name";
                SheetClassroom.Cells["D1"].Value = "Last Name";
                SheetClassroom.Cells["E1"].Value = "Phone Number";
                SheetClassroom.Cells["F1"].Value = "Email";
                SheetClassroom.Cells["G1"].Value = "SkypeID";
                SheetClassroom.Cells["H1"].Value = "Birthday";
                SheetClassroom.Cells["I1"].Value = "Age";
                SheetClassroom.Cells["J1"].Value = "Gender";
                SheetClassroom.Cells["K1"].Value = "Address";
                SheetClassroom.Cells["L1"].Value = "Nationality";
                SheetClassroom.Cells["M1"].Value = "Immigration Status";
                SheetClassroom.Cells["N1"].Value = "VisaHelp";
                SheetClassroom.Cells["O1"].Value = "Applied_Via";
                SheetClassroom.Cells["P1"].Value = "Recruiter";
                SheetClassroom.Cells["Q1"].Value = "Company";
                SheetClassroom.Cells["R1"].Value = "Education";
                SheetClassroom.Cells["S1"].Value = "ExperienceYears";
                SheetClassroom.Cells["T1"].Value = "Criminal Records";
                SheetClassroom.Cells["U1"].Value = "BackgroundCheck";
                SheetClassroom.Cells["V1"].Value = "AccommodationHelp";
                SheetClassroom.Cells["W1"].Value = "Willing to Relocate";
                SheetClassroom.Cells["X1"].Value = "Training Mode";
                SheetClassroom.Cells["Y1"].Value = "Experience Before(1-5)";

                SheetClassroom.Cells["A1"].Style.Font.Bold = true;
                SheetClassroom.Cells["B1"].Style.Font.Bold = true;
                SheetClassroom.Cells["C1"].Style.Font.Bold = true;
                SheetClassroom.Cells["D1"].Style.Font.Bold = true;
                SheetClassroom.Cells["E1"].Style.Font.Bold = true;
                SheetClassroom.Cells["F1"].Style.Font.Bold = true;
                SheetClassroom.Cells["G1"].Style.Font.Bold = true;
                SheetClassroom.Cells["H1"].Style.Font.Bold = true;
                SheetClassroom.Cells["I1"].Style.Font.Bold = true;
                SheetClassroom.Cells["J1"].Style.Font.Bold = true;
                SheetClassroom.Cells["K1"].Style.Font.Bold = true;
                SheetClassroom.Cells["L1"].Style.Font.Bold = true;
                SheetClassroom.Cells["M1"].Style.Font.Bold = true;
                SheetClassroom.Cells["N1"].Style.Font.Bold = true;
                SheetClassroom.Cells["O1"].Style.Font.Bold = true;
                SheetClassroom.Cells["P1"].Style.Font.Bold = true;
                SheetClassroom.Cells["Q1"].Style.Font.Bold = true;
                SheetClassroom.Cells["R1"].Style.Font.Bold = true;
                SheetClassroom.Cells["S1"].Style.Font.Bold = true;
                SheetClassroom.Cells["T1"].Style.Font.Bold = true;
                SheetClassroom.Cells["U1"].Style.Font.Bold = true;
                SheetClassroom.Cells["V1"].Style.Font.Bold = true;
                SheetClassroom.Cells["W1"].Style.Font.Bold = true;
                SheetClassroom.Cells["X1"].Style.Font.Bold = true;
                SheetClassroom.Cells["Y1"].Style.Font.Bold = true;

                SheetOnline.Cells["A1"].Value = "Created On";
                SheetOnline.Cells["B1"].Value = "#";
                SheetOnline.Cells["C1"].Value = "First Name";
                SheetOnline.Cells["D1"].Value = "Last Name";
                SheetOnline.Cells["E1"].Value = "Phone Number";
                SheetOnline.Cells["F1"].Value = "Email";
                SheetOnline.Cells["G1"].Value = "SkypeID";
                SheetOnline.Cells["H1"].Value = "Birthday";
                SheetOnline.Cells["I1"].Value = "Age";
                SheetOnline.Cells["J1"].Value = "Gender";
                SheetOnline.Cells["K1"].Value = "Address";
                SheetOnline.Cells["L1"].Value = "Nationality";
                SheetOnline.Cells["M1"].Value = "Immigration Status";
                SheetOnline.Cells["N1"].Value = "VisaHelp";
                SheetOnline.Cells["O1"].Value = "Applied_Via";
                SheetOnline.Cells["P1"].Value = "Recruiter";
                SheetOnline.Cells["Q1"].Value = "Company";
                SheetOnline.Cells["R1"].Value = "Education";
                SheetOnline.Cells["S1"].Value = "ExperienceYears";
                SheetOnline.Cells["T1"].Value = "Criminal Records";
                SheetOnline.Cells["U1"].Value = "BackgroundCheck";
                SheetOnline.Cells["V1"].Value = "AccommodationHelp";
                SheetOnline.Cells["W1"].Value = "Willing to Relocate";
                SheetOnline.Cells["X1"].Value = "Training Mode";
                SheetOnline.Cells["Y1"].Value = "Experience Before(1-5)";

                SheetOnline.Cells["A1"].Style.Font.Bold = true;
                SheetOnline.Cells["B1"].Style.Font.Bold = true;
                SheetOnline.Cells["C1"].Style.Font.Bold = true;
                SheetOnline.Cells["D1"].Style.Font.Bold = true;
                SheetOnline.Cells["E1"].Style.Font.Bold = true;
                SheetOnline.Cells["F1"].Style.Font.Bold = true;
                SheetOnline.Cells["G1"].Style.Font.Bold = true;
                SheetOnline.Cells["H1"].Style.Font.Bold = true;
                SheetOnline.Cells["I1"].Style.Font.Bold = true;
                SheetOnline.Cells["J1"].Style.Font.Bold = true;
                SheetOnline.Cells["K1"].Style.Font.Bold = true;
                SheetOnline.Cells["L1"].Style.Font.Bold = true;
                SheetOnline.Cells["M1"].Style.Font.Bold = true;
                SheetOnline.Cells["N1"].Style.Font.Bold = true;
                SheetOnline.Cells["O1"].Style.Font.Bold = true;
                SheetOnline.Cells["P1"].Style.Font.Bold = true;
                SheetOnline.Cells["Q1"].Style.Font.Bold = true;
                SheetOnline.Cells["R1"].Style.Font.Bold = true;
                SheetOnline.Cells["S1"].Style.Font.Bold = true;
                SheetOnline.Cells["T1"].Style.Font.Bold = true;
                SheetOnline.Cells["U1"].Style.Font.Bold = true;
                SheetOnline.Cells["V1"].Style.Font.Bold = true;
                SheetOnline.Cells["W1"].Style.Font.Bold = true;
                SheetOnline.Cells["X1"].Style.Font.Bold = true;
                SheetOnline.Cells["Y1"].Style.Font.Bold = true;

                foreach (var item in data)
                {
                    var visa = "N/A";
                    var background = "N/A";
                    var accommodation = "N/A";
                    var records = "N/A";
                    var relocate = "N/A";

                    if (item.VisaHelp == true)
                        visa = "Yes";
                    else if (item.VisaHelp == false)
                        visa = "No";

                    if (item.BackgroundCheck == true)
                        background = "Yes";
                    else if (item.BackgroundCheck == false)
                        background = "No";

                    if (item.AccommodationHelp == true)
                        accommodation = "Yes";
                    else if (item.AccommodationHelp == false)
                        accommodation = "No";

                    if (item.CriminalRecords == true)
                        records = "Yes";
                    else if (item.CriminalRecords == false)
                        records = "No";

                    if (item.WillingToRelocate == true)
                        relocate = "Yes";
                    else if (item.WillingToRelocate == false)
                        relocate = "No";

                    bName = item.Position;
                    bDate = item.StartDate.ToString();

                    double Age = (DateTime.Now - Convert.ToDateTime(item.Birthday)).Days / 365;

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.created_On.ToString();
                    Sheet.Cells[string.Format("B{0}", row)].Value = row - 1;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Fname;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.LName;
                    //Sheet.Cells[string.Format("E{0}", row)].Value = String.Format("{0:###-###-####}", Convert.ToInt64(item.Contact));
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Contact;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Email;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.SkypeID;
                    Sheet.Cells[string.Format("H{0}", row)].Value = Convert.ToDateTime(item.Birthday).ToString("MM/dd/yyyy");
                    Sheet.Cells[string.Format("I{0}", row)].Value = Age;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.Gender;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.Address;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Country;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.VisaStatus;
                    Sheet.Cells[string.Format("N{0}", row)].Value = visa;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.Applied_Via;
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.Recruiter;
                    Sheet.Cells[string.Format("Q{0}", row)].Value = item.COMPANY_NAME;
                    Sheet.Cells[string.Format("R{0}", row)].Value = item.Education;
                    Sheet.Cells[string.Format("S{0}", row)].Value = item.ExperienceYears;
                    Sheet.Cells[string.Format("T{0}", row)].Value = records;
                    Sheet.Cells[string.Format("U{0}", row)].Value = background;
                    Sheet.Cells[string.Format("V{0}", row)].Value = accommodation;
                    Sheet.Cells[string.Format("W{0}", row)].Value = relocate;
                    Sheet.Cells[string.Format("X{0}", row)].Value = item.TrainingMode;
                    Sheet.Cells[string.Format("Y{0}", row)].Value = item.CodingExperienceBefore;

                    if (item.TrainingMode == "Classroom")
                    {
                        SheetClassroom.Cells[string.Format("A{0}", rowClassroom)].Value = item.created_On.ToString();
                        SheetClassroom.Cells[string.Format("B{0}", rowClassroom)].Value = rowClassroom - 1;
                        SheetClassroom.Cells[string.Format("C{0}", rowClassroom)].Value = item.Fname;
                        SheetClassroom.Cells[string.Format("D{0}", rowClassroom)].Value = item.LName;
                        SheetClassroom.Cells[string.Format("E{0}", rowClassroom)].Value = item.Contact;
                        SheetClassroom.Cells[string.Format("F{0}", rowClassroom)].Value = item.Email;
                        SheetClassroom.Cells[string.Format("G{0}", rowClassroom)].Value = item.SkypeID;
                        SheetClassroom.Cells[string.Format("H{0}", rowClassroom)].Value = Convert.ToDateTime(item.Birthday).ToString("MM/dd/yyyy");
                        SheetClassroom.Cells[string.Format("I{0}", rowClassroom)].Value = Age;
                        SheetClassroom.Cells[string.Format("J{0}", rowClassroom)].Value = item.Gender;
                        SheetClassroom.Cells[string.Format("K{0}", rowClassroom)].Value = item.Address;
                        SheetClassroom.Cells[string.Format("L{0}", rowClassroom)].Value = item.Country;
                        SheetClassroom.Cells[string.Format("M{0}", rowClassroom)].Value = item.VisaStatus;
                        SheetClassroom.Cells[string.Format("N{0}", rowClassroom)].Value = visa;
                        SheetClassroom.Cells[string.Format("O{0}", rowClassroom)].Value = item.Applied_Via;
                        SheetClassroom.Cells[string.Format("P{0}", rowClassroom)].Value = item.Recruiter;
                        SheetClassroom.Cells[string.Format("Q{0}", rowClassroom)].Value = item.COMPANY_NAME;
                        SheetClassroom.Cells[string.Format("R{0}", rowClassroom)].Value = item.Education;
                        SheetClassroom.Cells[string.Format("S{0}", rowClassroom)].Value = item.ExperienceYears;
                        SheetClassroom.Cells[string.Format("T{0}", rowClassroom)].Value = records;
                        SheetClassroom.Cells[string.Format("U{0}", rowClassroom)].Value = background;
                        SheetClassroom.Cells[string.Format("V{0}", rowClassroom)].Value = accommodation;
                        SheetClassroom.Cells[string.Format("W{0}", rowClassroom)].Value = relocate;
                        SheetClassroom.Cells[string.Format("X{0}", rowClassroom)].Value = item.TrainingMode;
                        SheetClassroom.Cells[string.Format("Y{0}", rowClassroom)].Value = item.CodingExperienceBefore;

                        rowClassroom++;

                    }

                    if (item.TrainingMode == "Online")
                    {
                        SheetOnline.Cells[string.Format("A{0}", rowOnline)].Value = item.created_On.ToString();
                        SheetOnline.Cells[string.Format("B{0}", rowOnline)].Value = rowOnline - 1;
                        SheetOnline.Cells[string.Format("C{0}", rowOnline)].Value = item.Fname;
                        SheetOnline.Cells[string.Format("D{0}", rowOnline)].Value = item.LName;
                        SheetOnline.Cells[string.Format("E{0}", rowOnline)].Value = item.Contact;
                        SheetOnline.Cells[string.Format("F{0}", rowOnline)].Value = item.Email;
                        SheetOnline.Cells[string.Format("G{0}", rowOnline)].Value = item.SkypeID;
                        SheetOnline.Cells[string.Format("H{0}", rowOnline)].Value = Convert.ToDateTime(item.Birthday).ToString("MM/dd/yyyy");
                        SheetOnline.Cells[string.Format("I{0}", rowOnline)].Value = Age;
                        SheetOnline.Cells[string.Format("J{0}", rowOnline)].Value = item.Gender;
                        SheetOnline.Cells[string.Format("K{0}", rowOnline)].Value = item.Address;
                        SheetOnline.Cells[string.Format("L{0}", rowOnline)].Value = item.Country;
                        SheetOnline.Cells[string.Format("M{0}", rowOnline)].Value = item.VisaStatus;
                        SheetOnline.Cells[string.Format("N{0}", rowOnline)].Value = visa;
                        SheetOnline.Cells[string.Format("O{0}", rowOnline)].Value = item.Applied_Via;
                        SheetOnline.Cells[string.Format("P{0}", rowOnline)].Value = item.Recruiter;
                        SheetOnline.Cells[string.Format("Q{0}", rowOnline)].Value = item.COMPANY_NAME;
                        SheetOnline.Cells[string.Format("R{0}", rowOnline)].Value = item.Education;
                        SheetOnline.Cells[string.Format("S{0}", rowOnline)].Value = item.ExperienceYears;
                        SheetOnline.Cells[string.Format("T{0}", rowOnline)].Value = records;
                        SheetOnline.Cells[string.Format("U{0}", rowOnline)].Value = background;
                        SheetOnline.Cells[string.Format("V{0}", rowOnline)].Value = accommodation;
                        SheetOnline.Cells[string.Format("W{0}", rowOnline)].Value = relocate;
                        SheetOnline.Cells[string.Format("X{0}", rowOnline)].Value = item.TrainingMode;
                        SheetOnline.Cells[string.Format("Y{0}", rowOnline)].Value = item.CodingExperienceBefore;

                        rowOnline++;
                    }
                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                SheetClassroom.Cells["A:AZ"].AutoFitColumns();
                SheetOnline.Cells["A:AZ"].AutoFitColumns();
                Response.ClearContent();

                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=" + bName + "_" + DateTime.Parse(bDate).ToShortDateString() + "_Report.xlsx");
                Response.ContentType = "application/ms-excel";
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
            }

        }

        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API .NET Quickstart";

        [HttpGet]
        public void MemberProfileGoogle(int BatchId)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            String spreadsheetId = "1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms";
            String range = "Class Data!A2:E";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                Console.WriteLine("Name, Major");
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("{0}, {1}", row[0], row[4]);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
            Console.Read();

        }

        
        [HttpPost]
        public ActionResult ProfileSearch(FormCollection form)
        {
            string str = form["txtSearch"].ToString();
            string strBatchId = form["txtBatchId"].ToString();
            string strffilter = form["txtffilter"].ToString();
            return RedirectToAction("ProfileList", "Home", new { searchString = str, BatchId = strBatchId, filter = strffilter });
        }

        [HttpPost]
        public ActionResult ProfileFilter(FormCollection form)
        {
            string str = form["ddlFilter"].ToString();
            string strBatchId = form["txtBId"].ToString();
            string strfSearch = form["txtfSearch"].ToString();
            return RedirectToAction("ProfileList", "Home", new { filter = str, BatchId = strBatchId, searchString = strfSearch });
        }


        [HttpGet]
        public ActionResult ProfileSalStartDt(int id, int BatchId, string Filter)
        {
            ViewBag.ID = id;
            ViewBag.BatchId = BatchId;
            ViewBag.filter = Filter;

            ProfileUser user = new ProfileUser();
            user.Salary = 1200.00M;
            return PartialView("_SalStartDt", user);
        }


        [HttpGet]
        public ActionResult LeftOfficePopup(int id, int BId, int page, string sortOrder, string searchString, string Filter)
        {
            ViewBag.ID = id;
            ViewBag.BID = BId;
            ViewBag.Page = page;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchString = searchString;
            ViewBag.filter = Filter;

            return PartialView("_LeftOffice");
        }


        //[HttpPost]
        //public ActionResult ProfileList(getProfileList_Result user)
        //{
        //    //foreach(getProfileList_Result pu in user)
        //    //{
        //    //    using (TMSEntities Context = new TMSEntities())
        //    //    {
        //    //        Profile_users result = (from p in Context.Profile_users
        //    //                                where p.PID == pu.PID
        //    //                                select p).SingleOrDefault();
        //    //        result.FirstDayShowup = pu.attended;
        //    //        Context.SaveChanges();
        //    //    }
        //    //}

        //    return View();
        //}


        public ActionResult PlacementList(string sortOrder, string searchString, int? page)
        {
            Session.Remove("PFlag");
            try { } catch { }

            int pageSize = Convert.ToInt32(20);
            int pageNumber = (page ?? 1);
            ViewBag.PageNum = pageNumber;
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = "";
            }
            ViewBag.Search = searchString;
            TempData.Keep("Search");

            List<GetPlacementList_Result> Result = (new TMSEntities()).GetPlacementList(pageNumber, pageSize, sortOrder, searchString.Trim()).ToList<GetPlacementList_Result>();

            int count = Result.Count;
            int? Total;
            if (count > 0)
            {
                Total = (int?)Result[0].TotalCount;
            }
            else
            {

                Total = 0;
            }
            return View(new StaticPagedList<GetPlacementList_Result>(Result, Convert.ToInt32(pageNumber), Convert.ToInt32(pageSize), (int)Total));
        }

        [HttpPost]
        public ActionResult PlacementListSearch(FormCollection form)
        {
            string str = form["txtSearch"].ToString();
            return RedirectToAction("PlacementList", "Home", new { searchString = str });
        }


        private void sendProfileEmail(string strProfileName, string strPID, string strEmail)
        {
            try
            {
                string strBody = string.Empty;
                if (strEmail != null)
                {
                    strBody += "Dear " + strProfileName + "<br/>";
                    strBody += "Welcome to SummitWorks Tech Inc." + "<br/>";
                    strBody += "Your profile has been created with SummitWorks Portal. Please click on the below link to complete your profile." + "<br/><br/>";
                    strBody += "<a href='http://104.238.111.129:96/Trainee/pwd/" + strPID + "'>Click Here</a><br/><br/><br/><br/>";
                    strBody += "Thank you" + "<br/>";

                    SendMail(strEmail, "Profile created in SummitWorks Portal", strBody, "");
                }
            }
            catch (Exception ex)
            { throw ex; }
        }

        private int SendMail(string strToEmail, string strSubject, string strBody, string attachPath)
        {
            try
            {
                if (strToEmail.ToString().Trim() != "")
                {
                    System.Net.Mail.MailMessage Msg = new System.Net.Mail.MailMessage();
                    Msg.To.Add(strToEmail);
                    Msg.Subject = strSubject;
                    Msg.Body = strBody;
                    if (attachPath != string.Empty)
                    {
                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(attachPath);
                        Msg.Attachments.Add(attachment);
                    }
                    Msg.IsBodyHtml = true;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Send(Msg);
                }
                return 1;
            }
            catch (Exception)
            { throw; }
        }


        [HttpGet]
        public ActionResult GrossMarginReport()
        {
            ViewBag.GMRptOpt = GrossMarginRptOpt();

            GrossMarginRpt rpt = new GrossMarginRpt();
            rpt.StartDt = DateTime.Now;
            rpt.EndDt = DateTime.Now;
            rpt.RptList = (new TMSEntities()).getGrossMarginRpt(rpt.StartDt, rpt.EndDt, rpt.Flag).ToList<getGrossMarginRpt_Result>();
            return View(rpt);
        }

        [HttpPost]
        public ActionResult GrossMarginReport(GrossMarginRpt rpt)
        {
            ViewBag.GMRptOpt = GrossMarginRptOpt();

            if (ModelState.IsValid)
            {
                rpt.RptList = (new TMSEntities()).getGrossMarginRpt(rpt.StartDt, rpt.EndDt, rpt.Flag).ToList<getGrossMarginRpt_Result>();
            }

            return View(rpt);
        }

        [HttpGet]
        public ActionResult BatchRpt(int id)
        {
            BatRpt rpt = new BatRpt();
            rpt.RptList = (new TMSEntities()).AtteRptbyBatch(id).ToList<AtteRptbyBatch_Result>();
            return View(rpt);
        }

        [HttpGet]
        public ActionResult PayingTrainee()
        {
            PayingTrainees rpt = new PayingTrainees();
            rpt.RptList = (new TMSEntities()).GetPayingTrainees().ToList<GetPayingTrainees_Result>();
            return View(rpt);
        }


        public ActionResult AccessDenied()
        {
            return View("NoAccess");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            TempData["Status"] = TempData["ForgerStatus"];
            TempData["Message"] = TempData["ForgetResult"];
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> ForgotPassword(User user)
        {
            var u = db.Users.Where(x => x.Email == user.Email && x.ActiveInactive == true).ToList();

            if (u.Count != 0)
            {
                var code = GenCode(7);

                u[0].RecoveryCode = code;
                db.Users.Add(u[0]);
                db.Entry(u[0]).State = EntityState.Modified;
                db.SaveChanges();

                var fName = u[0].FullName;
                var href = this.Url.Action("ResetPassword", "Home", new { }, Request.Url.Scheme);
                var uName = u[0].username;

                var body = $@"<body style=""text-align:center; font-family: Arial, Helvetica, sans-serif; "">
                                <div style=""position:relative; 
    `                                           padding:50px 0px 50px; 
                                                background-position:center center; 
                                                background-repeat:no-repeat; 
                                                background-size:cover; "">
                                    <h1>TMS Recovery Code</h1>
                                    <table style=""margin-left:auto; margin-right:auto; width:450px; border:none; text-align:justify; border-radius:0px;"" cellpadding=""0"" cellspacing=""0"">
                                        <tbody style=""background-color:f7f8fd;""> 
                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;"" > Hi <b>{fName}</b>,</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;"" > We received your request to reset your TMS password</td>
                                        </tr>

                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;"" >
                                                User name: <b> {uName} </b>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;"" > Your recovery code is: </td>
                                        </tr>

                                        <tr>
                                            <td style=""text-align:center;"">
                                                <h2> 
                                                    {code} 
                                                </h2>
                                            </td>
                                        </tr>

                                        
                                        <tr>
                                            <td style=""text-align:center;"">
                                                <h3> 
                                                    <a href="" { href } "" title=""Confirm Email"">  Click here to Reset your Password  </a>
                                                </h3>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style=""padding: 10px 30px 10px 30px;""> If you did not request to reset your password, please ignore this email and log in with your existing password.</td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>  
                        </ body >";

                var subject = "TMS Recovery Password Code";

                await sendEmailAsync(subject, body, u[0].Email);

                TempData["ForgerStatus"] = "OK";
                TempData["ForgetResult"] = "Please check your email to Reset your password.";
            }

            else
            {
                TempData["ForgerStatus"] = "Error";
                TempData["ForgetResult"] = "Email not found";
            }


            return RedirectToAction("ForgotPassword", "Home");
            //return View();
        }

        public string GenCode(int length)
        {
            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }

            return str_build.ToString();
        }

        public async System.Threading.Tasks.Task sendEmailAsync(string subject, string body, string email)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(email));  // replace with valid value 
            message.From = new MailAddress(WebConfigurationManager.AppSettings["MyMailFrom2"]);  // replace with valid value
            message.Subject = subject;
            message.Body = string.Format(body, message.Subject, WebConfigurationManager.AppSettings["MyMailFromPWD2"], message);
            //message.Body = string.Format(body, subject, cOMPANY.CREDENTIAL_PASSWORD, message);

            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = WebConfigurationManager.AppSettings["MyMailFrom2"],  // replace with valid value
                    Password = WebConfigurationManager.AppSettings["MyMailFromPWD2"]  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = WebConfigurationManager.AppSettings["SMTPServer"];
                smtp.Port = Convert.ToInt32(WebConfigurationManager.AppSettings["SMTPPort"]);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);

            }
        }

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            TempData["Status"] = TempData["ResetStatus"];
            TempData["Message"] = TempData["ResetResult"];
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(User user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.RecoveryCode) || string.IsNullOrEmpty(user.password))
            {
                TempData["ResetStatus"] = "Error";
                TempData["ResetResult"] = "Email/Code/Password can't be empty";

                return RedirectToAction("ResetPassword", "Home");

            }

            else
            {
                var userData = db.Users.Where(x => x.Email == user.Email).ToList();

                if (userData.Count == 0)
                {
                    TempData["ResetStatus"] = "Error";
                    TempData["ResetResult"] = "Email not found.";
                    return RedirectToAction("ResetPassword", "Home");
                }
                else if (userData[0].RecoveryCode != user.RecoveryCode)
                {
                    TempData["ResetStatus"] = "Error";
                    TempData["ResetResult"] = "Code not valid!";
                    return RedirectToAction("ResetPassword", "Home");
                }
                else
                {

                    userData[0].password = (new ValidateModels()).Encrypt(user.password);
                    userData[0].RecoveryCode = null;
                    db.Users.Add(userData[0]);
                    db.Entry(userData[0]).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["ResetStatus"] = "OK";
                    TempData["ResetResult"] = "Password updated successfully!";
                    return RedirectToAction("ResetPassword", "Home");
                }
            }
        }
    }
}