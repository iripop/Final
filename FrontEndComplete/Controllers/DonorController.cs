using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FrontEndComplete.Models;
using PagedList;

namespace FrontEndComplete.Controllers
{
    [Authorize]
    public class DonorController : Controller
    {
        #region Donors GET
        public ActionResult Donor(int page=1, int pageSize=4)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

           List<string> isDonorActive = new List<string>(new string[] { "Yes", "No" });
           ViewBag.IsDonorActiveList = new SelectList(isDonorActive);

           List<string> bloodType = new List<string>(new string[] { "A", "AB", "B", "0" });
           ViewBag.BloodTypeList = new SelectList(bloodType);

           List<string> rhFactor = new List<string>(new string[] { "+(positive)", "-(negative)" });
           ViewBag.RhFactorList = new SelectList(rhFactor);

            List<DonorModel> listDonors = db.Donors.Where(x => x.DonorIsDeleted == false).Select(x => new DonorModel
            {
                DonorID=x.DonorID,
                ActiveDonor = x.ActiveDonor,
                DonorFullName = x.DonorFullName,
                BloodType = x.BloodType,
                RhFactor = x.RhFactor,
                DateOfBirth=x.DateOfBirth,
                Weight=x.Weight,
                DonorEmail=x.DonorEmail,
                DonorPhoneNumber=x.DonorPhoneNumber,
                LastScreeningDate=x.LastScreeningDate

            }).ToList();

            PagedList<DonorModel> model = new PagedList<DonorModel>(listDonors, page, pageSize);
            ViewBag.DonorsList = model;

            return View(ViewBag.DonorsList);

        }
        #endregion

        #region Donors POST
        [HttpPost]
        public ActionResult Donor(DonorModel model)
        {
            try
            {
                BloodDonorDBEntities db = new BloodDonorDBEntities();

                List<string> isDonorActive = new List<string>(new string[] { "Yes", "No" });
                ViewBag.IsDonorActiveList = new SelectList(isDonorActive);

                List<string> bloodType = new List<string>(new string[] { "A", "AB", "B", "0" });
                ViewBag.BloodTypeList = new SelectList(bloodType);

                List<string> rhFactor = new List<string>(new string[] { "+(positive)", "-(negative)" });
                ViewBag.RhFactorList = new SelectList(rhFactor);

                if (model.DonorID > 0)
                {
                    //Update a donor
                    Donor don = db.Donors.SingleOrDefault(x => x.DonorID == model.DonorID && x.DonorIsDeleted == false);

                    don.ActiveDonor = model.ActiveDonor;
                    don.DonorFullName = model.DonorFullName;
                    don.BloodType = model.BloodType;
                    don.RhFactor = model.RhFactor;
                    don.DateOfBirth = model.DateOfBirth;
                    don.Weight = model.Weight;
                    don.DonorEmail = model.DonorEmail;
                    don.DonorPhoneNumber = model.DonorPhoneNumber;
                    don.LastScreeningDate = model.LastScreeningDate;

                    db.SaveChanges();
              
                }
                else
                {
                    //Insert a recipient in database
                    Donor don = new Donor();
                    don.ActiveDonor = model.ActiveDonor;
                    don.DonorFullName = model.DonorFullName;
                    don.BloodType = model.BloodType;
                    don.RhFactor = model.RhFactor;
                    don.DateOfBirth = model.DateOfBirth;
                    don.Weight = model.Weight;
                    don.DonorEmail = model.DonorEmail;
                    don.DonorPhoneNumber = model.DonorPhoneNumber;
                    don.LastScreeningDate = model.LastScreeningDate; ;
                    don.DonorIsDeleted = false;

                    db.Donors.Add(don);
                    db.SaveChanges();

                    int latestDonorID = don.DonorID;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }
        #endregion

        #region Delete donor
        public JsonResult DeleteDonor(int donorID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            bool result = false;
            Donor don = db.Donors.SingleOrDefault(x => x.DonorIsDeleted == false && x.DonorID == donorID);

            if (don != null)
            {
                don.DonorIsDeleted = true;
                db.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Donor Details
        public ActionResult ShowDonorDetail(int DonorID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<DonorModel> listDon = db.Donors.Where(x => x.DonorIsDeleted == false && x.DonorID == DonorID).Select(x => new DonorModel
            {
                DonorID = x.DonorID,
                ActiveDonor = x.ActiveDonor,
                DonorFullName = x.DonorFullName,
                BloodType = x.BloodType,
                RhFactor = x.RhFactor,
                DateOfBirth = x.DateOfBirth,
                Weight = x.Weight,
                DonorEmail = x.DonorEmail,
                DonorPhoneNumber = x.DonorPhoneNumber,
                LastScreeningDate = x.LastScreeningDate,

            }).ToList();

            ViewBag.DonorsList = listDon;

            return PartialView("_ShowDonorDetail");

        }
        #endregion

        #region Add Edit donor
        public ActionResult AddEditDonor(int DonorID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<string> isDonorActive = new List<string>(new string[] { "Yes", "No" });
            ViewBag.IsDonorActiveList = new SelectList(isDonorActive);

            List<string> bloodType = new List<string>(new string[] { "A", "AB","B", "0" });
            ViewBag.BloodTypeList = new SelectList(bloodType);

            List<string> rhFactor = new List<string>(new string[] { "+(positive)", "-(negative)" });
            ViewBag.RhFactorList = new SelectList(rhFactor);

            DonorModel model = new DonorModel();

            if (DonorID > 0)
            {
                Donor don = db.Donors.SingleOrDefault(x => x.DonorID == DonorID && x.DonorIsDeleted == false);
                model.DonorID = don.DonorID;
                model.ActiveDonor = don.ActiveDonor;
                model.DonorFullName = don.DonorFullName;
                model.BloodType = don.BloodType;
                model.RhFactor = don.RhFactor;
                model.DateOfBirth = don.DateOfBirth;
                model.Weight = don.Weight;
                model.DonorEmail = don.DonorEmail;
                model.DonorPhoneNumber = don.DonorPhoneNumber;
                model.LastScreeningDate = don.LastScreeningDate;

            }

            return PartialView("_AddEditDonor", model);

        }
        #endregion

        #region Search
        public ActionResult GetSearchDonor(string SearchText)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();
            List<DonorModel> list = db.Donors.Where(x=>x.DonorFullName.Contains(SearchText)||
            x.ActiveDonor.Contains(SearchText)||
            x.BloodType.Contains(SearchText)||
            x.RhFactor.Contains(SearchText)||
            x.DonorEmail.Contains(SearchText)).Select(x => new DonorModel
            {
                DonorID = x.DonorID,
                ActiveDonor = x.ActiveDonor,
                DonorFullName = x.DonorFullName,
                BloodType = x.BloodType,
                RhFactor = x.RhFactor,
                DateOfBirth = x.DateOfBirth,
                Weight = x.Weight,
                DonorEmail = x.DonorEmail,
                DonorPhoneNumber = x.DonorPhoneNumber,
                LastScreeningDate = x.LastScreeningDate

            }).ToList();

            return PartialView("_SearchDonor", list);
        }
        #endregion
    }
}