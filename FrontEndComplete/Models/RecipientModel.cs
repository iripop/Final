using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FrontEndComplete.Models
{
    public class RecipientModel
    {
        public int RecipientID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Date of use is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public System.DateTime DateOfUse { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Related condition is required")]
        public string RelatedCondition { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Recipient is required")]
        public string RecipientCodedName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a donation")]
        public Nullable<int> DonationID { get; set; }
        public bool RecipientIsDeleted { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a donor")]
        public Nullable<int> DonorID { get; set; }

        //Custom attributes
        public string CrossBloodType { get; set; }
        public string CrossRhFactor { get; set; }
        public System.DateTime ExpirationDate { get; set; }
        public string DonorFullName { get; set; }
        public string DonationType { get; set; }
        public bool IsDeleted { get; set; }
    }
}