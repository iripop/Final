using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FrontEndComplete.Models;

namespace FrontEndComplete.Controllers
{
    [Authorize]
    public class RecipientController : Controller
    {
        #region Recipients GET
        public ActionResult Recipients()
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<Donation> donationlist = db.Donations.ToList();
            ViewBag.DonationList = new SelectList(donationlist.Where(x => x.IsDeleted == false), "DonationID", "DonationType");

            List<Donor> donorlist = db.Donors.ToList();
            ViewBag.DonorList = new SelectList(donorlist.Where(x => x.DonorIsDeleted == false), "DonorID", "DonorFullName");

            List<RecipientModel> listRec = db.Recipients.Where(x => x.RecipientIsDeleted == false).Select(x => new RecipientModel {
                RecipientCodedName = x.RecipientCodedName,
                DonorFullName= x.Donor.DonorFullName,
                DonationType = x.Donation.DonationType,
                DateOfUse = x.DateOfUse,
                RelatedCondition = x.RelatedCondition,
                RecipientID = x.RecipientID }).ToList();
            ViewBag.RecipientList = listRec;

            return View();

        }
        #endregion

        #region Recipients POST
        [HttpPost]
        public ActionResult Recipients(RecipientModel model)
        {
            try
            {
                BloodDonorDBEntities db = new BloodDonorDBEntities();
                List<Donation> list = db.Donations.ToList();
                ViewBag.DonationList = new SelectList(list.Where(x => x.IsDeleted == false), "DonationID", "DonationType");

                List<Donor> donorlist = db.Donors.ToList();
                ViewBag.DonorList = new SelectList(donorlist.Where(x => x.DonorIsDeleted == false), "DonorID", "DonorFullName");

                if (model.RecipientID > 0)
                {
                    //Update a recipient
                    Recipient rec = db.Recipients.SingleOrDefault(x => x.RecipientID == model.RecipientID && x.RecipientIsDeleted == false);

                    rec.DonorID = model.DonorID;
                    rec.DonationID = model.DonationID;
                    rec.RecipientCodedName = model.RecipientCodedName;
                    rec.DateOfUse = model.DateOfUse;
                    rec.RelatedCondition = model.RelatedCondition;

                    db.SaveChanges();
                }
                else
                {
                    //Insert a recipient in database
                    Recipient rec = new Recipient();
                    rec.DateOfUse = model.DateOfUse;
                    rec.RelatedCondition = model.RelatedCondition;
                    rec.RecipientCodedName = model.RecipientCodedName;
                    rec.DonationID = model.DonationID;
                    rec.DonorID = model.DonorID;
                    rec.RecipientIsDeleted = false;

                    db.Recipients.Add(rec);
                    db.SaveChanges();

                    int latestRecipientID = rec.RecipientID;
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
        public JsonResult DeleteRecipient(int recipientID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            bool result = false;
            Recipient rec = db.Recipients.SingleOrDefault(x => x.RecipientIsDeleted == false && x.RecipientID == recipientID);

            if (rec != null)
            {
                rec.RecipientIsDeleted = true;
                db.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Recipient details
        public ActionResult ShowRecipientDetail(int RecipientID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<RecipientModel> listRec = db.Recipients.Where(x => x.RecipientIsDeleted == false && x.RecipientID==RecipientID).Select(x => new RecipientModel
            {
                RecipientCodedName = x.RecipientCodedName,
                DonorFullName=x.Donor.DonorFullName,
                DonationType = x.Donation.DonationType,
                ExpirationDate=x.Donation.ExpirationDate,
                CrossBloodType=x.Donation.CrossBloodType,
                CrossRhFactor=x.Donation.CrossRhFactor,
                DateOfUse = x.DateOfUse,
                RelatedCondition = x.RelatedCondition,
                RecipientID = x.RecipientID
            }).ToList();

            ViewBag.RecipientList = listRec;

            return PartialView("ShowRecipientDetail");

        }
        #endregion

        #region Add or edit recipient
        public ActionResult AddEditRecipient(int RecipientID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();
            List<Donor> donorlist = db.Donors.ToList();
            ViewBag.DonorList = new SelectList(donorlist.Where(x => x.DonorIsDeleted == false), "DonorID", "DonorFullName");

            List<Donation> list = db.Donations.ToList();
            ViewBag.DonationList = new SelectList(list.Where(x=>x.IsDeleted==false), "DonationID", "DonationType");

            

            RecipientModel model = new RecipientModel();

            if (RecipientID > 0)
            {
                Recipient rec = db.Recipients.SingleOrDefault(x => x.RecipientID == RecipientID && x.RecipientIsDeleted == false);
                model.RecipientID = rec.RecipientID;
                model.DonorID = rec.DonorID;
                model.DonationID = rec.DonationID;
                model.RecipientCodedName = rec.RecipientCodedName;
                model.DateOfUse = rec.DateOfUse;
                model.RelatedCondition = rec.RelatedCondition;

            }

            return PartialView("AddEditRecipient", model);

        }
        #endregion

        #region Search
        public ActionResult GetSearchRecipient (string SearchText)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<RecipientModel> listRec = db.Recipients.Where(x => x.RecipientIsDeleted == false && (x.RecipientCodedName.Contains(SearchText) ||
            x.RelatedCondition.Contains(SearchText) ||
            x.DateOfUse.ToString().Contains(SearchText) ||
            x.Donor.DonorFullName.Contains(SearchText) ||
            x.Donation.DonationType.Contains(SearchText))).Select(x => new RecipientModel
            {
                RecipientCodedName = x.RecipientCodedName,
                DonorFullName = x.Donor.DonorFullName,
                DonationType = x.Donation.DonationType,
                DateOfUse = x.DateOfUse,
                RelatedCondition = x.RelatedCondition,
                RecipientID = x.RecipientID
            }).ToList();

            return PartialView("_SearchRecipient", listRec);
        }
        #endregion

    }
}