using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrontEndComplete.Models
{
    public class UserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAdmin { get; set; }

        public Nullable<bool> UserIsDeleted { get; set; }

        public Nullable<int> ImageID { get; set; }

        public HttpPostedFileWrapper ImageFile { get; set; }
    }
}