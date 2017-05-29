using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using FrontEndComplete.Models;

namespace FrontEndComplete.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        #region Add  a user action
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        #endregion

        #region Add a user POST action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified, ActivationCode")] User userModel)
        {
            bool Status = false;
            string message = "";

            //Model Validation
            if (ModelState.IsValid)
            {
                #region Check if Email already exists
                var IsExist = IsEmailExist(userModel.EmailAddress);
                if (IsExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exists");
                    return View(userModel);
                }
                #endregion

                #region Generate activation code
                userModel.ActivationCode = Guid.NewGuid();
                #endregion

                #region Password hashing
                userModel.Password = Crypto.Hash(userModel.Password);
                userModel.ConfirmPassword = Crypto.Hash(userModel.ConfirmPassword);
                #endregion
                userModel.IsEmailVerified = false;

                #region Save data to database
                using (BloodDonorDBEntities db = new BloodDonorDBEntities())
                {
                    db.Users.Add(userModel);
                    db.SaveChanges();

                    //Send email to user
                    SendVerificationLinkEmail(userModel.EmailAddress, userModel.ActivationCode.ToString());
                    message = "Registration successful. Account activation link" +
                        " has been sent to your email address:" + userModel.EmailAddress;
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
            return View(userModel);


        }
        #endregion

        #region Verify account
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (BloodDonorDBEntities db = new BloodDonorDBEntities())
            {
                db.Configuration.ValidateOnSaveEnabled = false; // added to avoid confirm password does 
                                                                //not match issue on save changes
                var v = db.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    db.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }
        #endregion

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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
        protected void SendVerificationLinkEmail(string userEmail, string activationCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("pop.irina1@gmail.com", "Blood donations management system");
            var toEmail = new MailAddress(userEmail);
            var fromEmailPassword = "Frunzulitza.1"; //Replace with actual password
            string subject = "Your account is succesfully created";
            string body = "<br/><b/r>We are excited to tell you that your Blood donations management system account is" +
                " successfully created. Please click on the link below to verify your account" +
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