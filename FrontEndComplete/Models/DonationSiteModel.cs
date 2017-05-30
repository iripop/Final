using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrontEndComplete.Models
{
    public class DonationSiteModel
    {
        public int DonationSiteID { get; set; }
        public string SiteName { get; set; }
        public Nullable<System.DateTime> EventStartDate { get; set; }
        public Nullable<System.DateTime> EventEndDate { get; set; }
        public string RegistrationEmail { get; set; }
        public string RegistrationPhone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public Nullable<int> StaffingRequired { get; set; }
        public bool MobileSite { get; set; }
    }
}