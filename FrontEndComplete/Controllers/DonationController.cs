using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FrontEndComplete.Models;

namespace FrontEndComplete.Controllers
{
    public class DonationController : Controller
    {
        public ActionResult Donation()
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();
            List<Donor> listDonor = db.Donors.ToList();
            ViewBag.DonorsList = new SelectList(listDonor.Where(x=>x.ActiveDonor=="Yes" && x.DonorIsDeleted==false), "DonorID", "DonorFirstName");
            List<Recipient> listRecipient = db.Recipients.ToList();
            ViewBag.RecipientsList = new SelectList(listRecipient, "RecipientID", "RecipientCodedName");
            List<DonationSite> listDonationSite = db.DonationSites.ToList();
            ViewBag.DonationSiteList = new SelectList(listDonationSite, "DonationSiteID", "SiteName");

            // This is for the delete operation, IsDeleted column was added in order to avoid any null exception
            List<DonationModel> listDonations = db.Donations.Where(x => x.IsDeleted == false).Select(x => new DonationModel
            {
                DonationID = x.DonationID,
                DonationType = x.DonationType,
                CrossBloodType = x.CrossBloodType,
                CrossRhFactor = x.CrossRhFactor,
                ExpirationDate = x.ExpirationDate,
                NumberOfUnits = x.NumberOfUnits,
                DonationSiteID = x.DonationSiteID,
                RecipientID = x.RecipientID,
                DonorFirstName = x.Donor.DonorFirstName,
                SiteName = x.DonationSite.SiteName,
                CreationDate = x.CreationDate

            }).ToList();

            ViewBag.DonationsList = listDonations;

            return View();

            
        }

        [HttpPost]
        public ActionResult Donation(DonationModel model)
        {
            try
            {
                BloodDonorDBEntities db = new BloodDonorDBEntities();
                List<Donor> listDonor = db.Donors.ToList();
                ViewBag.DonorsList = new SelectList(listDonor.Where(x => x.ActiveDonor =="Yes" && x.DonorIsDeleted == false), "DonorID", "DonorFirstName");
                List<Recipient> listRecipient = db.Recipients.ToList();
                ViewBag.RecipientsList = new SelectList(listRecipient, "RecipientID", "RecipientCodedName");
                List<DonationSite> listDonationSite = db.DonationSites.ToList();
                ViewBag.DonationSiteList = new SelectList(listDonationSite, "DonationSiteID", "SiteName");

                if (model.DonationID > 0)
                {
                    //Update a recipient
                    Donation donation = db.Donations.SingleOrDefault(x => x.DonationID == model.DonationID && x.IsDeleted == false);

                    donation.DonationType = model.DonationType;
                    donation.CrossBloodType = model.CrossBloodType;
                    donation.CrossRhFactor = model.CrossRhFactor;
                    donation.NumberOfUnits = model.NumberOfUnits;
                    donation.DonorID = model.DonorID;
                    donation.RecipientID = model.RecipientID;
                    donation.DonationSiteID = model.DonationSiteID;

                    db.SaveChanges();
                }
                else
                {
                    //Insert a recipient in database
                    Donation donation = new Donation();
                    donation.DonationType = model.DonationType;
                    donation.CrossBloodType = model.CrossBloodType;
                    donation.CrossRhFactor = model.CrossRhFactor;
                    
                    donation.NumberOfUnits = model.NumberOfUnits;
                    donation.DonorID = model.DonorID;
                    donation.RecipientID = model.RecipientID;
                    donation.DonationSiteID = model.DonationSiteID;
                    donation.IsDeleted = false;
                    donation.CreationDate = DateTime.Now;
                    donation.ExpirationDate = DateTime.Now.AddDays(90);

                    db.Donations.Add(donation);
                    db.SaveChanges();

                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public JsonResult DeleteDonation(int donationID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            bool result = false;
            Donation don = db.Donations.SingleOrDefault(x => x.IsDeleted == false && x.DonationID == donationID);

            if (don != null)
            {
                don.IsDeleted = true;
                db.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowDonationDetail(int DonationID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<DonationModel> listDonations = db.Donations.Where(x => x.IsDeleted == false && x.DonationID==DonationID).Select(x => new DonationModel
            {
                DonationID = x.DonationID,
                DonationType = x.DonationType,
                CrossBloodType = x.CrossBloodType,
                CrossRhFactor = x.CrossRhFactor,
                ExpirationDate = x.ExpirationDate,
                NumberOfUnits = x.NumberOfUnits,
                DonationSiteID = x.DonationSiteID,
                DonorID = x.DonorID,
                RecipientID = x.RecipientID,
                DonorFirstName = x.Donor.DonorFirstName,
                SiteName = x.DonationSite.SiteName,
                CreationDate=x.CreationDate
                
            }).ToList();

            ViewBag.DonationsList = listDonations;
            return PartialView("_ShowDonationDetail");
        }

        public ActionResult AddEditDonation(int DonationID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<Donor> listDonor = db.Donors.ToList();
            ViewBag.DonorsList = new SelectList(listDonor.Where(x => x.DonorIsDeleted == false), "DonorID", "DonorFirstName");
            List<Recipient> listRecipient = db.Recipients.ToList();
            ViewBag.RecipientsList = new SelectList(listRecipient, "RecipientID", "RecipientCodedName");
            List<DonationSite> listDonationSite = db.DonationSites.ToList();
            ViewBag.DonationSiteList = new SelectList(listDonationSite, "DonationSiteID", "SiteName");

            DonationModel model = new DonationModel();
            if (DonationID > 0)
            {
                Donation donation = db.Donations.SingleOrDefault(x => x.DonationID == DonationID && x.IsDeleted == false);
                model.DonationID = donation.DonationID;
                model.DonorID = donation.DonorID;
                model.DonationType = donation.DonationType;
                model.CrossBloodType = donation.CrossBloodType;
                model.CrossRhFactor = donation.CrossRhFactor;
                model.ExpirationDate = donation.ExpirationDate;
                model.NumberOfUnits = donation.NumberOfUnits;
                model.DonationSiteID = donation.DonationSiteID;
                model.RecipientID = donation.RecipientID;
                model.CreationDate = donation.CreationDate;
                
            }

            return PartialView("_AddEditDonation", model);
        }

        public ActionResult GetSearchDonation(string SearchText)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<Donor> listDonor = db.Donors.ToList();
            ViewBag.DonorsList = new SelectList(listDonor.Where(x => x.ActiveDonor == "Yes" && x.DonorIsDeleted == false), "DonorID", "DonorFirstName");
            List<Recipient> listRecipient = db.Recipients.ToList();
            ViewBag.RecipientsList = new SelectList(listRecipient, "RecipientID", "RecipientCodedName");
            List<DonationSite> listDonationSite = db.DonationSites.ToList();
            ViewBag.DonationSiteList = new SelectList(listDonationSite, "DonationSiteID", "SiteName");

            List<DonationModel> listDonations = db.Donations.Where(x => x.IsDeleted == false && x.DonationType.Contains(SearchText)|| 
            x.CrossBloodType.Contains(SearchText)||
            x.CrossRhFactor.Contains(SearchText)||
            x.NumberOfUnits.ToString().Contains(SearchText)||
            x.DonationSite.SiteName.ToString().Contains(SearchText) ||
            x.Recipient.RecipientCodedName.Contains(SearchText) ||
            x.Donor.DonorFirstName.Contains(SearchText)||
            x.DonationID.ToString().Contains(SearchText)).Select(x => new DonationModel
            {
                DonationID = x.DonationID,
                DonationType = x.DonationType,
                CrossBloodType = x.CrossBloodType,
                CrossRhFactor = x.CrossRhFactor,
                ExpirationDate = x.ExpirationDate,
                NumberOfUnits = x.NumberOfUnits,
                DonationSiteID = x.DonationSiteID,
                DonorID = x.DonorID,
                RecipientID = x.RecipientID,
                DonorFirstName = x.Donor.DonorFirstName,
                SiteName = x.DonationSite.SiteName,
                CreationDate = x.CreationDate

            }).ToList();

            return PartialView("_SearchDonation", listDonations);
        }

    }
}