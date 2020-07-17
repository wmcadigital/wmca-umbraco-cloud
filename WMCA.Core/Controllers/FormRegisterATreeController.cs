using FormRegisterATree.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Forms.Core.Models;
using System.Windows.Forms;

namespace FormRegisterATree.Controllers
{
    public class FormRegisterATreeController : SurfaceController
    {
        public IContentService _contentService { get; set; }
        public FormRegisterATreeController(IContentService contentService)
        {
            _contentService = contentService;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public static bool ReCaptchaPassed(string gRecaptchaResponse)
        {
            HttpClient httpClient = new HttpClient();
            var res = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret=6Ld5rdEUAAAAAKfFg3KLGJ68eLqDUWX7XuTCS4vD&response={gRecaptchaResponse}").Result;
            if (res.StatusCode != HttpStatusCode.OK)
                return false;

            string JSONres = res.Content.ReadAsStringAsync().Result;
            dynamic JSONdata = JObject.Parse(JSONres);
            if (JSONdata.success != "true")
                return false;

            return true;
        }

        public ActionResult Submit(RegisterTreeFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                WebImage photo = null;
                var newFileName = "";
                var imagePath = "";

                if (model.Files != null)
                {
                    photo = WebImage.GetImageFromRequest();
                    if (photo != null)
                    {
                        newFileName = Path.GetFileName(photo.FileName);
                        imagePath = @"\UploadedFiles\" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + newFileName;
                        photo.Save(@"~\" + imagePath);
                        TempData["imgLoc"] = imagePath.Replace("\\", "/");
                    }
                }
                SendEmail(model) ;
                CreateNode(model);
                TempData["ContactSuccess"] = true;
                return RedirectToCurrentUmbracoPage();
            } else
            {
                if (!ReCaptchaPassed(Request.Form["foo"]))
                {
                    ModelState.AddModelError(string.Empty, "You failed the CAPTCHA.");
                    return CurrentUmbracoPage();
                }
                return CurrentUmbracoPage();
            }
        }

        private void CreateNode(RegisterTreeFormViewModel model)
        {
            var parent = _contentService.GetById(1694);
            var udiId = parent.GetUdi();
            
            var request = _contentService.CreateContent($"Tree - {model.DatePlanted}", udiId, "tree");
            request.SetValue("who", model.WhoType);
            request.SetValue("firstName", model.FirstName);
            request.SetValue("lastName", model.LastName);
            request.SetValue("organisationName", model.OrgName);
            request.SetValue("datePlanted", model.DatePlanted);
            request.SetValue("howManyTreesDidYouPlant", model.TreesPlanted);
            request.SetValue("location", model.Location);
            request.SetValue("latitude", model.Latitude);
            request.SetValue("longitude", model.Longitude);
            request.SetValue("treeType", model.TreeType);
            request.SetValue("image", TempData["imgLoc"]);

            _contentService.Save(request);
        }

        private void SendEmail(RegisterTreeFormViewModel model)
        {
            MailAddress from = new MailAddress("website@wmca.org.uk", "West Midlands Combined Authority");
            MailAddress to = new MailAddress("westmidsca.webmaster@gmail.com", "West Midlands Combined Authority");
            MailMessage message = new MailMessage(from, to); 
            message.Subject = string.Format("Someone has planted a tree");
            var imgLoc = "https://beta.wmca.org.uk" + TempData["imgLoc"];
            message.Body = "<p><b>Name: </b>" + model.FirstName + "&nbsp;" + model.LastName + "</p><p><b>Email: </b>" + model.Email + "</p><p><b>Location: </b>" + model.Location + "</p><p><b>Latitude: </b>" + model.Latitude + "</p><p><b>Longitude: </b>" + model.Longitude + "</p><p><b>Image: </b>" + "<a href=\"" + imgLoc + "\" />" + imgLoc + "</a>" + "</p><p><b>Join Mailing List: </b>" + model.MailingList + "</p>";
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            client.Host = "in-v3.mailjet.com";
            client.Credentials = new System.Net.NetworkCredential("66b3bdc8b4d8c8607a56e3a66ffc0c46", "22a429a0d57ececf742ac017898e8bf7");
            //SmtpClient client = new SmtpClient("127.0.0.1", 25);
            client.Send(message);
        }
    }
}