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
        public ActionResult SelectDonorRecipientDonationSite()
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();
            List<Donor> listDonor = db.Donors.ToList();
            ViewBag.DonorsList = new SelectList(listDonor, "DonorID", "DonorFirstName");
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
                DonorID = x.DonorID,
                RecipientID = x.RecipientID,
                DonorFirstName = x.Donor.DonorFirstName,
                SiteName = x.DonationSite.SiteName
            }).ToList();

            ViewBag.DonationsList = listDonations;

            return View();

            
        }

        [HttpPost]
        public ActionResult SelectDonorRecipientDonationSite(DonationModel model)
        {
            if(ModelState.IsValid== true)
            {

            }
            BloodDonorDBEntities db = new BloodDonorDBEntities();
            List<Donor> listDonor = db.Donors.ToList();
            ViewBag.DonorsList = new SelectList(listDonor, "DonorID", "DonorFirstName");
            List<Recipient> listRecipient = db.Recipients.ToList();
            ViewBag.RecipientsList = new SelectList(listRecipient, "RecipientID", "RecipientCodedName");
            List<DonationSite> listDonationSite = db.DonationSites.ToList();
            ViewBag.DonationSiteList = new SelectList(listDonationSite, "DonationSiteID", "SiteName");
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveDonationRecord(DonationModel model)
        {
            try
            {
                BloodDonorDBEntities db = new BloodDonorDBEntities();
                List<Donor> listDonor = db.Donors.ToList();
                ViewBag.DonorsList = new SelectList(listDonor, "DonorID", "DonorFirstName");

                List<Recipient> listRecipient = db.Recipients.ToList();
                ViewBag.RecipientsList = new SelectList(listRecipient, "RecipientID", "RecipientCodedName");

                List<DonationSite> listDonationSite = db.DonationSites.ToList();
                ViewBag.DonationSiteList = new SelectList(listDonationSite, "DonationSiteID", "SiteName");

               if (model.DonationID > 0)
                {
                    //Update a donation
                    Donation don = db.Donations.SingleOrDefault(x => x.DonationID == model.DonationID && x.IsDeleted == false);
                    don.DonationID = model.DonationID;
                    don.DonorID = model.DonorID;
                    don.DonationType = model.DonationType;
                    don.CrossBloodType = model.CrossBloodType;
                    don.CrossRhFactor = model.CrossRhFactor;
                    don.ExpirationDate = model.ExpirationDate;
                    don.NumberOfUnits = model.NumberOfUnits;
                    don.DonationSiteID = model.DonationSiteID;
                    don.RecipientID = model.RecipientID;
                    db.SaveChanges();
                }
                else
                {
                    // Insert donation
                    Donation don = new Donation();
                    don.DonorID = model.DonorID;
                    don.DonationType = model.DonationType;
                    don.CrossBloodType = model.CrossBloodType;
                    don.CrossRhFactor = model.CrossRhFactor;
                    don.ExpirationDate = model.ExpirationDate;
                    don.NumberOfUnits = model.NumberOfUnits;
                    don.DonationSiteID = model.DonationSiteID;
                    don.RecipientID = model.RecipientID;
                    don.IsDeleted = false;

                    db.Donations.Add(don);
                    db.SaveChanges();

                    int latestDonId = don.DonationID;
                }
                return View(model);
            }
            catch(Exception e)
            {
                throw e;
            }

            //return RedirectToAction("SelectDonorRecipientDonationSite");
        }

        public ActionResult Donation()
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<Donation> donationList = db.Donations.ToList();

            DonationModel donationModel = new DonationModel();

            List<DonationModel> donationModelList = donationList.Select(x => new DonationModel
            {
                DonationID = x.DonationID,
                DonationType = x.DonationType,
                CrossBloodType = x.CrossBloodType,
                CrossRhFactor = x.CrossRhFactor,
                ExpirationDate=x.ExpirationDate,
                NumberOfUnits=x.NumberOfUnits,
                DonationSiteID=x.DonationSiteID,
                DonorID=x.DonorID,
                RecipientID=x.RecipientID,
                DonorFirstName=x.Donor.DonorFirstName,
                SiteName=x.DonationSite.SiteName
            }).ToList();

            return View(donationModelList);
        }

        public ActionResult DonationDetail(int donationID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            Donation donation = db.Donations.SingleOrDefault(x => x.DonationID == donationID);

            DonationModel donationModel = new DonationModel();

            donationModel.DonationType = donation.DonationType;
            donationModel.CrossBloodType = donation.CrossBloodType;
            donationModel.CrossRhFactor = donation.CrossRhFactor;
            donationModel.DonorFirstName = donation.Donor.DonorFirstName;
            donationModel.SiteName = donation.DonationSite.SiteName;

            return View(donationModel);
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

        public ActionResult ShowDonation(int DonationID)
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
                SiteName = x.DonationSite.SiteName
            }).ToList();

            ViewBag.DonationsList = listDonations;
            return PartialView("Partial");
        }

        public ActionResult AddEditDonation(int DonationID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<Donor> listDonor = db.Donors.ToList();
            ViewBag.DonorsList = new SelectList(listDonor, "DonorID", "DonorFirstName");
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
                
            }

            return PartialView("Partial2", model);
        }

    }
}