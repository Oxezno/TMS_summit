using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMS.Models;

namespace TMS.Controllers
{
    public class SettingsController : Controller
    {

        #region Reason
        [HttpPost]
        public ActionResult ReasonSearch(FormCollection form)
        {
            string str = form["txtSearch"].ToString();
            return RedirectToAction("ReasonList", "Settings", new { searchString = str });
        }

        public ActionResult ReasonList(string sortOrder, string searchString, int? page)
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

            List<GetReasonList_Result> Result = (new TMSEntities()).GetReasonList(pageNumber, pageSize, sortOrder, searchString.Trim()).ToList<GetReasonList_Result>();

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
            return View(new StaticPagedList<GetReasonList_Result>(Result, Convert.ToInt32(pageNumber), Convert.ToInt32(pageSize), (int)Total));
        }


        [HttpGet]
        public ActionResult ConformResonStatuspopup(int id)
        {
            ViewBag.ID = id;

            return PartialView("_ConformReasonStatus");
        }

        #endregion

        #region Position
        [HttpPost]
        public ActionResult PositionSearch(FormCollection form)
        {
            string str = form["txtSearch"].ToString();
            return RedirectToAction("PositionList", "Settings", new { searchString = str });
        }

        public ActionResult PositionList(string sortOrder, string searchString, int? page)
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

            List<GetPositionList_Result> Result = (new TMSEntities()).GetPositionList(pageNumber, pageSize, sortOrder, searchString.Trim()).ToList<GetPositionList_Result>();

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
            return View(new StaticPagedList<GetPositionList_Result>(Result, Convert.ToInt32(pageNumber), Convert.ToInt32(pageSize), (int)Total));
        }

        [HttpGet]
        public ActionResult ConformPositionStatuspopup(int id)
        {
            ViewBag.ID = id;

            return PartialView("_ConformPositionStatus");
        }

        #endregion

        [HttpGet]
        public ActionResult UserRoles()
        {
            return View((new TMSEntities()).RolePermissions.ToList());
        }

        [HttpPost]
        public ActionResult UserRoles(IList<RolePermission> roles)
        {
            TMSEntities DbCompany = new TMSEntities();
            if (roles != null)
            {
                using (TMSEntities dc = new TMSEntities())
                {
                    foreach (var i in roles)
                    {
                        var c = dc.RolePermissions.Where(a => a.Id.Equals(i.Id)).FirstOrDefault();
                        if (c != null)
                        {
                            c.Settings = i.Settings;
                            c.AdminWebsite = i.AdminWebsite;
                            c.WebsiteDashboard = i.WebsiteDashboard;
                        }
                    }
                    dc.SaveChanges();
                }
            }
            DbCompany.SaveChanges();

            TempData["ErrorMessage"] = "Role Permissions Updated Successfully";
            TempData["color"] = "green";

            return RedirectToAction("UserRoles", "Settings");
        }

    }
}