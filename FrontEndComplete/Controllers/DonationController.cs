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
    public class DonationController : Controller
    {
        #region GET donation
        public ActionResult Donation()
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<Donor> listDonor = db.Donors.ToList();
            ViewBag.DonorsList = new SelectList(listDonor.Where(x=>x.ActiveDonor=="Yes" && x.DonorIsDeleted==false), "DonorID", "DonorFullName");

            List<Recipient> listRecipient = db.Recipients.ToList();
            ViewBag.RecipientsList = new SelectList(listRecipient, "RecipientID", "RecipientCodedName");

            List<DonationSite> listDonationSite = db.DonationSites.ToList();
            ViewBag.DonationSiteList = new SelectList(listDonationSite, "DonationSiteID", "SiteName");

            List<string> listDonationTypes = new List<string>(new string[] { "Whole Blood", "Packed Red Blood Cells", "Platelets", "Plasma" });
            ViewBag.DonationTypeList = new SelectList(listDonationTypes);

            List<string> bloodType = new List<string>(new string[] { "A", "AB", "B", "0" });
            ViewBag.BloodTypeList = new SelectList(bloodType);

            List<string> rhFactor = new List<string>(new string[] { "+(positive)", "-(negative)" });
            ViewBag.RhFactorList = new SelectList(rhFactor);

            List<string> accepted = new List<string>(new string[] { "True", "False" });
            ViewBag.IsAcceptedList = new SelectList(accepted);

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
                RecipientCodedName = x.Recipient.RecipientCodedName,
                DonorFullName = x.Donor.DonorFullName,
                SiteName = x.DonationSite.SiteName,
                CreationDate = x.CreationDate,
                Accepted=x.Accepted,
                ReasonForRejection=x.ReasonForRejection
                

            }).ToList();

            //PagedList<DonationModel> model = new PagedList<DonationModel>(listDonations, page, pageSize);
            //ViewBag.DonationsList = model;
            ViewBag.DonationsList = listDonations;

            return View();

            
        }
        #endregion

        #region POST donations
        [HttpPost]
        public ActionResult Donation(DonationModel model)
        {
            try
            {
                BloodDonorDBEntities db = new BloodDonorDBEntities();

                List<Donor> listDonor = db.Donors.ToList();
                ViewBag.DonorsList = new SelectList(listDonor.Where(x => x.ActiveDonor =="Yes" && x.DonorIsDeleted == false), "DonorID", "DonorFullName");

                List<Recipient> listRecipient = db.Recipients.ToList();
                ViewBag.RecipientsList = new SelectList(listRecipient, "RecipientID", "RecipientCodedName");

                List<DonationSite> listDonationSite = db.DonationSites.ToList();
                ViewBag.DonationSiteList = new SelectList(listDonationSite, "DonationSiteID", "SiteName");

                List<string> listDonationTypes = new List<string>(new string[] { "Whole Blood", "Packed Red Blood Cells", "Platelets", "Plasma" });
                ViewBag.DonationTypeList = new SelectList(listDonationTypes);

                List<string> bloodType = new List<string>(new string[] { "A", "AB", "B", "0" });
                ViewBag.BloodTypeList = new SelectList(bloodType);

                List<string> rhFactor = new List<string>(new string[] { "+(positive)", "-(negative)" });
                ViewBag.RhFactorList = new SelectList(rhFactor);

                List<string> accepted = new List<string>(new string[] { "True", "False" });
                ViewBag.IsAcceptedList = new SelectList(accepted);

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
                    donation.Accepted = model.Accepted;

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
        #endregion

        #region Delete donation
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
        #endregion

        #region Donation details
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
                DonorFullName = x.Donor.DonorFullName,
                SiteName = x.DonationSite.SiteName,
                CreationDate=x.CreationDate,
                Accepted=x.Accepted,
                ReasonForRejection=x.ReasonForRejection
                
                
            }).ToList();

            ViewBag.DonationsList = listDonations;
            return PartialView("_ShowDonationDetail");
        }
        #endregion

        #region Add or edit donation
        public ActionResult AddEditDonation(int DonationID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<Donor> listDonor = db.Donors.ToList();
            ViewBag.DonorsList = new SelectList(listDonor.Where(x => x.DonorIsDeleted == false), "DonorID", "DonorFullName");

            List<Recipient> listRecipient = db.Recipients.ToList();
            ViewBag.RecipientsList = new SelectList(listRecipient, "RecipientID", "RecipientCodedName");

            List<DonationSite> listDonationSite = db.DonationSites.ToList();
            ViewBag.DonationSiteList = new SelectList(listDonationSite, "DonationSiteID", "SiteName");

            List<string> listDonationTypes = new List<string>(new string[] { "Whole Blood", "Packed Red Blood Cells", "Platelets", "Plasma" });
            ViewBag.DonationTypeList = new SelectList(listDonationTypes);

            List<string> bloodType = new List<string>(new string[] { "A", "AB", "B", "0" });
            ViewBag.BloodTypeList = new SelectList(bloodType);

            List<string> rhFactor = new List<string>(new string[] { "+(positive)", "-(negative)" });
            ViewBag.RhFactorList = new SelectList(rhFactor);

            List<string> accepted = new List<string>(new string[] { "True", "False" });
            ViewBag.IsAcceptedList = new SelectList(accepted);

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
                model.Accepted = donation.Accepted;
                model.ReasonForRejection = donation.ReasonForRejection;
                
            }

            return PartialView("_AddEditDonation", model);
        }
        #endregion

        #region Search
        public ActionResult GetSearchDonation(string SearchText)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<Donor> listDonor = db.Donors.ToList();
            ViewBag.DonorsList = new SelectList(listDonor.Where(x => x.ActiveDonor == "Yes" && x.DonorIsDeleted == false), "DonorID", "DonorFullName");
            List<Recipient> listRecipient = db.Recipients.ToList();
            ViewBag.RecipientsList = new SelectList(listRecipient, "RecipientID", "RecipientCodedName");
            List<DonationSite> listDonationSite = db.DonationSites.ToList();
            ViewBag.DonationSiteList = new SelectList(listDonationSite, "DonationSiteID", "SiteName");
            List<string> accepted = new List<string>(new string[] { "True", "False" });
            ViewBag.IsAcceptedList = new SelectList(accepted);

            List<DonationModel> listDonations = db.Donations.Where(x => x.IsDeleted == false && x.DonationType.Contains(SearchText)|| 
            x.CrossBloodType.Contains(SearchText)||
            x.CrossRhFactor.Contains(SearchText)||
            x.NumberOfUnits.ToString().Contains(SearchText)||
            x.DonationSite.SiteName.ToString().Contains(SearchText) ||
            x.Recipient.RecipientCodedName.Contains(SearchText) ||
            x.Donor.DonorFullName.Contains(SearchText)||
            x.Accepted.ToString().Contains(SearchText)||
            x.ReasonForRejection.Contains(SearchText)||
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
                DonorFullName = x.Donor.DonorFullName,
                SiteName = x.DonationSite.SiteName,
                CreationDate = x.CreationDate,
                Accepted=x.Accepted,
                ReasonForRejection=x.ReasonForRejection

            }).ToList();

            return PartialView("_SearchDonation", listDonations);
        }
        #endregion
    }
}