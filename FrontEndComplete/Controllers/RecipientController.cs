using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FrontEndComplete.Models;

namespace FrontEndComplete.Controllers
{
    public class RecipientController : Controller
    {

        public ActionResult Recipients()
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<Donation> list = db.Donations.ToList();

            ViewBag.DonationList = new SelectList(list.Where(x => x.IsDeleted == false), "DonationID", "DonationType");

            List<RecipientModel> listRec = db.Recipients.Where(x => x.RecipientIsDeleted == false).Select(x => new RecipientModel {
                RecipientCodedName = x.RecipientCodedName,
                DonationType = x.Donation.DonationType,
                DateOfUse = x.DateOfUse,
                RelatedCondition = x.RelatedCondition,
                RecipientID = x.RecipientID }).ToList();

            ViewBag.RecipientList = listRec;

            return View();

        }

        [HttpPost]
        public ActionResult Recipients(RecipientModel model)
        {
            try
            {
                BloodDonorDBEntities db = new BloodDonorDBEntities();
                List<Donation> list = db.Donations.ToList();
                ViewBag.DonationList = new SelectList(list.Where(x => x.IsDeleted == false), "DonationID", "DonationType");

                if (model.RecipientID > 0)
                {
                    //Update a recipient
                    Recipient rec = db.Recipients.SingleOrDefault(x => x.RecipientID == model.RecipientID && x.RecipientIsDeleted == false);
                   
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

        public ActionResult ShowRecipientDetail(int RecipientID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<RecipientModel> listRec = db.Recipients.Where(x => x.RecipientIsDeleted == false && x.RecipientID==RecipientID).Select(x => new RecipientModel
            {
                RecipientCodedName = x.RecipientCodedName,
                DonationType = x.Donation.DonationType,
                DateOfUse = x.DateOfUse,
                RelatedCondition = x.RelatedCondition,
                RecipientID = x.RecipientID
            }).ToList();

            ViewBag.RecipientList = listRec;

            return PartialView("ShowRecipientDetail");

        }

        public ActionResult AddEditRecipient(int RecipientID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();
            List<Donation> list = db.Donations.ToList();
            ViewBag.DonationList = new SelectList(list.Where(x=>x.IsDeleted==false), "DonationID", "DonationType");

            RecipientModel model = new RecipientModel();

            if (RecipientID > 0)
            {
                Recipient rec = db.Recipients.SingleOrDefault(x => x.RecipientID == RecipientID && x.RecipientIsDeleted == false);
                model.RecipientID = rec.RecipientID;
                model.DonationID = rec.DonationID;
                model.RecipientCodedName = rec.RecipientCodedName;
                model.DateOfUse = rec.DateOfUse;
                model.RelatedCondition = rec.RelatedCondition;

            }

            return PartialView("AddEditRecipient", model);

        }

        public ActionResult GetSearchRecipient (string SearchText)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<RecipientModel> listRec = db.Recipients.Where(x => x.RecipientIsDeleted == false && (x.RecipientCodedName.Contains(SearchText)||
            x.RelatedCondition.Contains(SearchText)||
            x.DateOfUse.ToString().Contains(SearchText)||
            x.Donation.DonationType.Contains(SearchText))).Select(x => new RecipientModel
            {
                RecipientCodedName = x.RecipientCodedName,
                DonationType = x.Donation.DonationType,
                DateOfUse = x.DateOfUse,
                RelatedCondition = x.RelatedCondition,
                RecipientID = x.RecipientID
            }).ToList();

            return PartialView("_SearchRecipient", listRec);
        }

    }
}