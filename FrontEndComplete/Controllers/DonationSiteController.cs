using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FrontEndComplete.Models;
//using PagedList;

namespace FrontEndComplete.Controllers
{
    [Authorize]
    public class DonationSiteController : Controller
    {
        #region DonationSite GET
        public ActionResult DonationSite()
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<string> mobile = new List<string>(new string[] { "True", "False" });
            ViewBag.MobileSiteList = new SelectList(mobile);

            List<DonationSiteModel> listDonSite = db.DonationSites.Where(x => x.IsArchived == false).Select(x => new DonationSiteModel
            {
                DonationSiteID = x.DonationSiteID,
                SiteName = x.SiteName,
                EventStartDate = x.EventStartDate,
                EventEndDate = x.EventEndDate,
                RegistrationEmail = x.RegistrationEmail,
                RegistrationPhone = x.RegistrationPhone,
                Address = x.Address,
                City = x.City,
                Zip = x.Zip,
                StaffingRequired = x.StaffingRequired,
                MobileSite = x.MobileSite


            }).ToList();

            ViewBag.DonationSiteList = listDonSite;

            return View();

        }
        #endregion

        #region Recipients POST
        [HttpPost]
        public ActionResult DonationSite(DonationSiteModel model)
        {
            try
            {
                BloodDonorDBEntities db = new BloodDonorDBEntities();

                List<string> mobile = new List<string>(new string[] { "True", "False" });
                ViewBag.MobileSiteList = new SelectList(mobile);

                if (model.DonationSiteID > 0)
                {
                    //Update a donation site
                    DonationSite don = db.DonationSites.SingleOrDefault(x => x.DonationSiteID == model.DonationSiteID && x.IsArchived == false);

                    don.SiteName = model.SiteName;
                    don.EventStartDate = model.EventStartDate;
                    don.EventEndDate = model.EventEndDate;
                    don.RegistrationEmail = model.RegistrationEmail;
                    don.RegistrationPhone = model.RegistrationPhone;
                    don.Address = model.Address;
                    don.City = model.City;
                    don.Zip = model.Zip;
                    don.StaffingRequired = model.StaffingRequired;
                    don.MobileSite = model.MobileSite;

                    db.SaveChanges();

                }
                else
                {
                    //Insert a recipient in database
                    DonationSite don = new DonationSite();
                    don.SiteName = model.SiteName;
                    don.EventStartDate = model.EventStartDate;
                    don.EventEndDate = model.EventEndDate;
                    don.RegistrationEmail = model.RegistrationEmail;
                    don.RegistrationPhone = model.RegistrationPhone;
                    don.Address = model.Address;
                    don.City = model.City;
                    don.Zip = model.Zip;
                    don.StaffingRequired = model.StaffingRequired;
                    don.MobileSite = model.MobileSite;
                    don.IsArchived = false;

                    db.DonationSites.Add(don);
                    db.SaveChanges();

                    int latestDonationSiteID = don.DonationSiteID;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        #endregion

        #region Delete recipients
        public JsonResult DeleteDonationSite(int donsiteID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            bool result = false;
            DonationSite don = db.DonationSites.SingleOrDefault(x => x.IsArchived == false && x.DonationSiteID == donsiteID);

            if (don != null)
            {
                don.IsArchived = true;
                db.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Recipient details
        public ActionResult ShowDonationSiteDetail(int DonationSiteID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<DonationSiteModel> listDonationSites = db.DonationSites.Where(x => x.IsArchived == false && x.DonationSiteID == DonationSiteID).Select(x => new DonationSiteModel
            {
                DonationSiteID = x.DonationSiteID,
                SiteName = x.SiteName,
                EventStartDate = x.EventStartDate,
                EventEndDate = x.EventEndDate,
                RegistrationEmail = x.RegistrationEmail,
                RegistrationPhone = x.RegistrationPhone,
                Address = x.Address,
                City = x.City,
                Zip = x.Zip,
                StaffingRequired = x.StaffingRequired,
                MobileSite = x.MobileSite

            }).ToList();

            ViewBag.DonationSitesList = listDonationSites;

            return PartialView("_ShowDonationSiteDetail");

        }
        #endregion

        #region Add or edit recipient
        public ActionResult AddEditDonationSite(int DonationSiteID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<string> mobile = new List<string>(new string[] { "True", "False" });
            ViewBag.MobileSiteList = new SelectList(mobile);

            DonationSiteModel model = new DonationSiteModel();

            if (DonationSiteID > 0)
            {
                DonationSite don = db.DonationSites.SingleOrDefault(x => x.DonationSiteID == DonationSiteID && x.IsArchived == false);
                model.DonationSiteID = don.DonationSiteID;
                model.SiteName = don.SiteName;
                model.EventStartDate = don.EventStartDate;
                model.EventEndDate = don.EventEndDate;
                model.RegistrationEmail = don.RegistrationEmail;
                model.RegistrationPhone = don.RegistrationPhone;
                model.Address = don.Address;
                model.City = don.City;
                model.Zip = don.Zip;
                model.StaffingRequired = don.StaffingRequired;
                model.MobileSite = don.MobileSite;

            }

            return PartialView("_AddEditDonationSite", model);

        }
        #endregion

        #region Search
        public ActionResult GetSearchDonationSite(string SearchText)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();
            List<DonationSiteModel> list = db.DonationSites.Where(x => x.SiteName.Contains(SearchText) ||
            x.Address.Contains(SearchText) ||
            x.City.Contains(SearchText) ||
            x.Zip.Contains(SearchText) ||
            x.EventStartDate.ToString().Contains(SearchText)||
            x.EventEndDate.ToString().Contains(SearchText) ||
            x.RegistrationEmail.Contains(SearchText)||
            x.RegistrationPhone.Contains(SearchText)||
            x.MobileSite.ToString().Contains(SearchText)).Select(x => new DonationSiteModel
            {
                SiteName = x.SiteName,
                Address = x.Address,
                City = x.City,
                Zip = x.Zip,
                MobileSite = x.MobileSite,
                EventStartDate=x.EventStartDate,
                EventEndDate=x.EventEndDate,
                RegistrationPhone=x.RegistrationPhone,
                RegistrationEmail=x.RegistrationEmail
                

            }).ToList();

            return PartialView("_SearchDonationSite", list);
        }
        #endregion
    }
}