using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FrontEndComplete.Models;

namespace FrontEndComplete.Controllers
{
    public class ManageController : Controller
    {
        // GET: Manage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserProfile()
        {
            return View();
        }

        public JsonResult ImageUpload(UserProfile model)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();
            int ImageID = 0;

            var file = model.ImageFile;
            byte[] imagebyte = null;

            if (file != null)
            {
                //var fileName = Path.GetFileName(file.FileName);
                //var extension = Path.GetExtension(file.FileName);
                //var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                file.SaveAs(Server.MapPath("/UploadedImage/" + file.FileName));

                BinaryReader reader = new BinaryReader(file.InputStream);
                imagebyte = reader.ReadBytes(file.ContentLength);
                Image img = new Image();
                img.ImageName = file.FileName;
                img.ImageByte = imagebyte;
                img.ImagePath = "/UploadedImage/" + file.FileName;
                img.IsDeleted = false;

                db.Images.Add(img);
                db.SaveChanges();

                ImageID = img.ImageID;
               

            }

            return Json(ImageID, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImageRetrieve(int imgID)
        {
            BloodDonorDBEntities db = new BloodDonorDBEntities();
            var img=db.Images.SingleOrDefault(x => x.ImageID == imgID && x.IsDeleted==false);
            return File(img.ImageByte, "Image/jpg");
        }
    }
}