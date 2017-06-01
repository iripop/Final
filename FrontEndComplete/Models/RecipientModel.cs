using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrontEndComplete.Models
{
    public class RecipientModel
    {
        public int RecipientID { get; set; }
        public System.DateTime DateOfUse { get; set; }
        public string RelatedCondition { get; set; }
        public string RecipientCodedName { get; set; }
        public Nullable<int> DonationID { get; set; }
        public bool RecipientIsDeleted { get; set; }

        //Custom attributes
        public string DonationType { get; set; }
        public bool IsDeleted { get; set; }
    }
}