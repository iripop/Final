using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrontEndComplete.Models
{
    public class DonorModel
    {
        public int DonorID { get; set; }
        public string ActiveDonor { get; set; }
        public string DonorFirstName { get; set; }
        public string DonorLastName { get; set; }
        public string BloodType { get; set; }
        public string RhFactor { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<double> Wight { get; set; }
        public string DonorEmail { get; set; }
        public string DonorPhoneNumber { get; set; }
        public Nullable<System.DateTime> LastScreeningDate { get; set; }
        public Nullable<bool> DonorIsDeleted { get; set; }
    }
}