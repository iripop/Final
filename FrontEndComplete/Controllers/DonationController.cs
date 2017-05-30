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

                Donation don = new Donation();
                don.DonorID = model.DonorID;
                don.DonationType = model.DonationType;
                don.CrossBloodType = model.CrossBloodType;
                don.CrossRhFactor = model.CrossRhFactor;
                don.ExpirationDate = model.ExpirationDate;
                don.NumberOfUnits = model.NumberOfUnits;
                don.DonationSiteID = model.DonationSiteID;
                don.RecipientID = model.RecipientID;

                db.Donations.Add(don);
                db.SaveChanges();

                int latestDonId = don.DonationID;
            }
            catch(Exception e)
            {
                throw e;
            }

            return RedirectToAction("SelectDonorRecipientDonationSite");
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
    }
}