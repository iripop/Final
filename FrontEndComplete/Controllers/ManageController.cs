using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using FrontEndComplete.Models;

namespace FrontEndComplete.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        #region Users GET
        public ActionResult Users()
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            List<UserModel> listUsers = db.Users.Where(x => x.UserIsDeleted == false).Select(x => new UserModel
            {
                UserID = x.UserID,
                FirstName = x.FirstName,
                LastName = x.LastName,
                EmailAddress = x.EmailAddress,
                PhoneNumber = x.PhoneNumber,
                IsAdmin=x.IsAdmin 

            }).ToList();
            ViewBag.UsersList = listUsers;

            return View();

        }
        #endregion

        #region Users POST
        [HttpPost]
        public ActionResult Users(User model)
        {
            try
            {
                BloodDonorDBEntities db = new BloodDonorDBEntities();

                if (model.UserID > 0)
                {
                    //Update a recipient
                    User u = db.Users.SingleOrDefault(x => x.UserID == model.UserID && x.UserIsDeleted == false);

                    u.UserID = model.UserID;
                    u.FirstName = model.FirstName;
                    u.LastName = model.LastName;
                    u.EmailAddress = model.EmailAddress;
                    u.PhoneNumber = model.PhoneNumber;
                    u.IsAdmin = model.IsAdmin;

                    db.SaveChanges();
                }
                else
                {
                  
                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }
        #endregion

        #region Delete user
        public JsonResult DeleteUser(int userID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            bool result = false;
            User rec = db.Users.SingleOrDefault(x => x.UserIsDeleted == false && x.UserID == userID);

            if (rec != null)
            {
                rec.UserIsDeleted = true;
                db.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Add or edit user
        public ActionResult AddEditUser(int UserID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();

            UserModel model = new UserModel();

            if (UserID > 0)
            {
                User u = db.Users.SingleOrDefault(x => x.UserID == UserID && x.UserIsDeleted == false);
                model.UserID = u.UserID;
                model.FirstName = u.FirstName;
                model.LastName = u.LastName;
                model.EmailAddress = u.EmailAddress;
                model.PhoneNumber = u.PhoneNumber;
                model.IsAdmin = u.IsAdmin;

            }

            return PartialView("_AddEditUser", model);

        }
        #endregion

        #region Search
        public ActionResult GetSearchUser(string SearchText)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();


            List<UserModel> listUsers = db.Users.Where(x => x.UserIsDeleted == false && (x.FirstName.Contains(SearchText) ||
            x.LastName.Contains(SearchText) ||
            x.EmailAddress.ToString().Contains(SearchText) ||
            x.PhoneNumber.Contains(SearchText)||
            x.IsAdmin.ToString().Contains(SearchText))).Select(x => new UserModel
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                EmailAddress = x.EmailAddress,
                PhoneNumber=x.PhoneNumber,
                IsAdmin = x.IsAdmin,
            }).ToList();

            return PartialView("_SearchUser", listUsers);
        }
        #endregion

        public ActionResult ShowUserDetail(int Userid)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();
            List<User> listUserDetails = db.Users.Where(x=>x.UserID ==Userid).Select(x => new User
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                EmailAddress = x.EmailAddress,
                PhoneNumber = x.PhoneNumber,
                IsAdmin = x.IsAdmin

            }).ToList();

            ViewBag.DetailsList = listUserDetails;

            return View();

        }

        #region Change Password
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(UserPasswordChangeModel model)
        {
            bool Status = false;
            string message = "";

            //Model Validation
            if (ModelState.IsValid)
            {
                #region Check if Current Password is correct 
                var IsCorrect = IsPasswordCorrect(model.Password);
                if (!IsCorrect)
                {
                    ModelState.AddModelError("EmailExist", "Password is incorrect");
                    return View(model);
                }
                #endregion

                #region Password hashing
                model.NewPassword = Crypto.Hash(model.NewPassword);
                model.ConfirmPassword = Crypto.Hash(model.ConfirmPassword);
                #endregion

                #region Save data to database
                using (BloodDonorDBEntities db = new BloodDonorDBEntities())
                {
                    db.SaveChanges();
                    Status = true;
                }
                #endregion
            }
            else
            {
                message = "Invalid request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(model);


        }
        #endregion

        #region Forgot Password
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(User model)
        {
            bool Status = false;
            string message = "";

            //Model Validation
            if (ModelState.IsValid)
            {
                #region Check if Email already exists
                var IsExist = IsEmailExist(model.EmailAddress);
                if (!IsExist)
                {
                    ModelState.AddModelError("EmailExist", "Email does not exists");
                    return View(model);
                }
                #endregion

                #region Generate new password
                model.Password = Membership.GeneratePassword(12, 1);
                #endregion

                #region Save data to database
                using (BloodDonorDBEntities db = new BloodDonorDBEntities())
                {
                    db.SaveChanges();
                    Status = true;

                    SendPasswordResetEmail(model.EmailAddress, model.Password.ToString());
                    message = "Password reset successful. The new password" +
                        " has been sent to your email address:" + model.EmailAddress;
                    Status = true;
                }
                #endregion
            }
            else
            {
                message = "Invalid request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(model);

        }

        #endregion

        [NonAction]
        public bool IsPasswordCorrect(string userpass)
        {
            using (BloodDonorDBEntities db = new BloodDonorDBEntities())
            {
                var v = db.Users.Where(a => a.Password==userpass).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public bool IsEmailExist(string userEmail)
        {
            using (BloodDonorDBEntities db = new BloodDonorDBEntities())
            {
                var v = db.Users.Where(a => a.EmailAddress == userEmail).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        protected void SendPasswordResetEmail(string userEmail, string pass)
        {
            var verifyUrl = "/User/Login/";
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("pop.irina1@gmail.com", "Blood donations management system");
            var toEmail = new MailAddress(userEmail);
            var fromEmailPassword = "Frunzulitza.1"; //Replace with actual password
            string subject = "Your password was reset";
            string body = "<br/><b/r>We are excited to tell you that your Blood donations management system password was" +
                " successfully reset.Your new password is" +pass+ " Please click on the link below to log in into your account" +
                "<b/r><br/> <a href='" + link + "'>" + link + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)

            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
    }
}