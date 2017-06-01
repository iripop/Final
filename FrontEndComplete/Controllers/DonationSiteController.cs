using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FrontEndComplete.Models;
using System.Linq.Dynamic;

namespace FrontEndComplete.Controllers
{
    public class DonationSiteController : Controller
    {
        [Authorize]
        public ActionResult DonationSite(int page = 1, string sort = "SiteName", string sortdir = "asc", string search = "")
        {
            int pageSize = 10;
            int totalRecord = 0;
            if (page < 1) page = 1;
            int skip = (page * pageSize) - pageSize;
            var data = GetDonationSites(search, sort, sortdir, skip, pageSize, out totalRecord);
            ViewBag.TotalRows = totalRecord;
            ViewBag.search = search;
            return View(data);
        }

        public List<DonationSite> GetDonationSites(string search, string sort, string sortdir, int skip, int pageSize, out int totalRecord)
        {
            using (BloodDonorDBEntities db = new BloodDonorDBEntities())
            {
                var v = (from a in db.DonationSites
                         where
                         a.SiteName.Contains(search) ||
                         a.EventStartDate.ToString().Contains(search) ||
                         a.EventEndDate.ToString().Contains(search) ||
                         a.RegistrationEmail.Contains(search) ||
                         a.RegistrationPhone.Contains(search) ||
                         a.Address.Contains(search) ||
                         a.City.Contains(search) ||
                         a.Zip.Contains(search) ||
                         a.StaffingRequired.ToString().Contains(search) ||
                         a.MobileSite.ToString().Contains(search)
                         select a
                       );
                totalRecord = v.Count();
                v = v.OrderBy(sort + " " + sortdir);
                if (pageSize > 0)
                {
                    v = v.Skip(skip).Take(pageSize);
                }
                return v.ToList();
            }
        }

        public ActionResult SaveDonationSiteRecord(DonationSiteModel model)
        {
            try
            {
                BloodDonorDBEntities db = new BloodDonorDBEntities();

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

                db.DonationSites.Add(don);
                db.SaveChanges();

                int latestDonId = don.DonationSiteID;
            }
            catch (Exception e)
            {
                throw e;
            }

            return RedirectToAction("DonationSite");
        }
    }
}