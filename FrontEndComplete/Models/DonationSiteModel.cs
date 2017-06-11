using System;
using System.ComponentModel.DataAnnotations;

namespace FrontEndComplete.Models
{
    public class DonationSiteModel
    {
        public int DonationSiteID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "SiteName")]
        public string SiteName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public Nullable<System.DateTime> EventStartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public Nullable<System.DateTime> EventEndDate { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        public string RegistrationEmail { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Contact is required")]
        public string RegistrationPhone { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Zip code is required")]
        public string Zip { get; set; }

        public Nullable<int> StaffingRequired { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile site is required")]
        public bool MobileSite { get; set; }
        public Nullable<bool> IsArchived { get; set; }
    }
}