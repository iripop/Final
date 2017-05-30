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
        
        public ActionResult Recipient()
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();
            List<Recipient> recipientList = db.Recipients.ToList();

            RecipientModel recipientM = new RecipientModel();
            List<RecipientModel> recipientMList = recipientList.Select(x => new RecipientModel
            { RecipientCodedName = x.RecipientCodedName,
                RecipientID = x.RecipientID,
                RelatedCondition = x.RelatedCondition,
                DateOfUse = x.DateOfUse,
                DonationID = x.DonationID,
                DonationType = x.Donation.DonationType }).ToList();

            return View(recipientMList);
        }

        public ActionResult Index()
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();
            List<Recipient> recipientList = db.Recipients.ToList();
            RecipientModel recipientM = new RecipientModel();
            List<RecipientModel> recipientMList = recipientList.Select(x => new RecipientModel
            {
                RecipientID = x.RecipientID,
                RecipientCodedName = x.RecipientCodedName,
                DonationID = x.DonationID,
            }).ToList();

            return View(recipientMList);
        }

        public ActionResult RecipientDetail(int recipientID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            Recipient recipient = db.Recipients.SingleOrDefault(x => x.RecipientID == recipientID);

            RecipientModel recipientM = new RecipientModel();

            recipientM.RecipientCodedName = recipient.RecipientCodedName;
            recipientM.RelatedCondition = recipient.RelatedCondition;
            recipientM.DateOfUse = recipient.DateOfUse;
            recipientM.DonationType = recipient.Donation.DonationType;
            return View(recipientM);
        }

        public ActionResult HtmlHelpers()
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();
            List<Donation> list = db.Donations.ToList();
            ViewBag.DonationsList = new SelectList(list, "DonationID", "DonationType");
            return View();
        }

    }
}