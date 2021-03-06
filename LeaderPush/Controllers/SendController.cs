﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeaderPush.Models;
using Newtonsoft.Json;
using ShopifySharp;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using WebPush;

namespace LeaderPush.Controllers
{
    public class SendController : Controller
    {
        PushDBEntities db = new PushDBEntities();
        // GET: Send
        public ActionResult Getjson(string endpoint, string p256dh, string auth, string domain)
        {
            if (db.Users.Where(w => w.Auth == auth).Count() > 0)
            {

            }
            else {
                LeaderPush.Models.User user = new LeaderPush.Models.User();
                user.PushEndpoint = endpoint;
                user.P256dh = p256dh;
                user.Auth = auth;
                user.Store = domain;

                db.Users.Add(user);
                db.SaveChanges();
            }


            return Content("User is saved in DB");
        }

        public ActionResult Index() {

            List<ShopLink> shopLinks = db.ShopLinks.ToList();

            return View(shopLinks);
        }

        public ActionResult Premium() {



            return View();
        }

        public string filename;
        public string filename2;
        public ActionResult Go(string Title, string Body, string Link, HttpPostedFileBase Upload) {
            string shop = Session["shop"].ToString();
              
              
            if (db.ShopLinks.Where(w => w.Shop == shop).FirstOrDefault().SendLimit > 0) {

                
                if (Upload.ContentLength > 0)
                {
                    string nameOfFile = DateTime.Now.ToString("yyyyMMddHHmmss") + Upload.FileName;
                    var path = Server.MapPath("~/Public/images/" + Path.GetFileName(nameOfFile));
                    Upload.SaveAs(path);
                    filename = nameOfFile;
                }


                List<LeaderPush.Models.User> users = db.Users.Where(w => w.Store == shop).ToList();



                foreach (var item in users)
                {
                    Json json = new Json();
                    json.title = Title;
                    json.body = Body;
                    json.image = "images/" + filename;
                    json.link = "//" + Link;

                    string output = JsonConvert.SerializeObject(json);




                    var pushEndpoint = item.PushEndpoint;
                    var p256dh = item.P256dh;
                    var auth = item.Auth;

                    var subject = @"mailto:tuncayhuseynov@gmail.com";
                    var publicKey = @"BAS21PmINNueC0awLLy-VE6FMManBBJMZT6NUAwb3Ou0gsAuuMrYwiJGnW4f__ew1yIh1VnXVwCNTpFDig6i7eA";
                    var privateKey = @"mXjwDyDRBb48nDdUyUtWFpLfenacZP7sHJJjNUsXM2Q";

                    var subscription = new PushSubscription(pushEndpoint, p256dh, auth);
                    var vapidDetails = new VapidDetails(subject, publicKey, privateKey);
                    var gcmAPIKey = @"AIzaSyBH2nwvAeBZ_lRPPmxNkS8YF6_noPzAUfM";

                    var webPushClient = new WebPushClient();
                    try
                    {
                        webPushClient.SendNotification(subscription, output, vapidDetails);
                        webPushClient.SendNotification(subscription, output, gcmAPIKey);



                        
                    }
                    catch (WebPushException exception)
                    {
                        Console.WriteLine("Http STATUS code" + exception.StatusCode);
                    }
                }

                ShopLink link = db.ShopLinks.Where(w => w.Shop == shop).FirstOrDefault();
                link.SendLimit -= 1;
                db.Entry(link).Property(w => w.SendLimit).IsModified = true;

                db.SaveChanges();
                return RedirectToAction("Done", "Send");
                // return Content(filename);
            }


            return RedirectToAction("Failed", "Send");
        }


        public ActionResult GoPremium(string Title, string Body, string Link, HttpPostedFileBase Upload, HttpPostedFileBase Upload2) {

            string shop = Session["shop"].ToString();

            if (db.ShopLinks.Where(w => w.Shop == shop).FirstOrDefault().IsPremium == true)
            {


                if (Upload.ContentLength > 0)
                {
                    string nameOfFile = DateTime.Now.ToString("yyyyMMddHHmmss") + Upload.FileName;
                    var path = Server.MapPath("~/Public/images/" + Path.GetFileName(nameOfFile));
                    Upload.SaveAs(path);

                    filename = nameOfFile;
                }
                if (Upload2.ContentLength > 0)
                {
                    string nameOfFile2 = DateTime.Now.ToString("yyyyMMddHHmmss") + Upload2.FileName;
                    var path = Server.MapPath("~/Public/images/" + Path.GetFileName(nameOfFile2));
                    Upload2.SaveAs(path);

                    filename2 = nameOfFile2;
                }


                List<LeaderPush.Models.User> users = db.Users.Where(w => w.Store == shop).ToList();



                foreach (var item in users)
                {
                    Json json = new Json();
                    json.title = Title;
                    json.body = Body;
                    json.image = "images/" + filename; ;
                    json.link = "//" + Link;
                    json.bigimage = "images/" + filename2;
                    json.Vib = "500,110,500,110,450,110,200,110,170,40,450,110,200,110,170,40,500";

                    string output = JsonConvert.SerializeObject(json);




                    var pushEndpoint = item.PushEndpoint;
                    var p256dh = item.P256dh;
                    var auth = item.Auth;

                    var subject = @"mailto:tuncayhuseynov@gmail.com";
                    var publicKey = @"BAS21PmINNueC0awLLy-VE6FMManBBJMZT6NUAwb3Ou0gsAuuMrYwiJGnW4f__ew1yIh1VnXVwCNTpFDig6i7eA";
                    var privateKey = @"mXjwDyDRBb48nDdUyUtWFpLfenacZP7sHJJjNUsXM2Q";

                    var subscription = new PushSubscription(pushEndpoint, p256dh, auth);
                    var vapidDetails = new VapidDetails(subject, publicKey, privateKey);
                    var gcmAPIKey = @"AIzaSyBH2nwvAeBZ_lRPPmxNkS8YF6_noPzAUfM";

                    var webPushClient = new WebPushClient();
                    try
                    {
                        webPushClient.SendNotification(subscription, output, vapidDetails);
                        webPushClient.SendNotification(subscription, output, gcmAPIKey);
                    }
                    catch (WebPushException exception)
                    {
                        Console.WriteLine("Http STATUS code" + exception.StatusCode);
                    }
                }

                return RedirectToAction("Done", "Send");

            }


            return RedirectToAction("Failed", "Send");
        }

        public ActionResult Done() {

            return View();
        }

        public ActionResult Failed() {

            return View();
        }
        public ActionResult NumFailed() {
            return View();
        }

      

        public async System.Threading.Tasks.Task<ActionResult> SendSMS(string text) {

            const string accountSid = "ACf90f4d897bbf6d580be050be95d19a87";
            const string authToken = "3882c2bac7be8144ac0e9130e5996de0";

            TwilioClient.Init(accountSid, authToken);

            string myShopifyUrl = Session["shop"].ToString();
            string accessToken = db.ShopLinks.Where(w => w.Shop == myShopifyUrl).FirstOrDefault().Token;



            try
            {
                var service = new CustomerService(myShopifyUrl, accessToken);
                IEnumerable<Customer> customers = await service.ListAsync();

                List<Customer> listOfCus = customers.Where(w => w.Phone != null).ToList();

                if (listOfCus.Count > 0)
                {
                    foreach (var item in listOfCus)
                    {
                        try
                        {
                            var message = MessageResource.Create(
                           body: text,
                           from: new Twilio.Types.PhoneNumber("+18506608313"),
                           to: new Twilio.Types.PhoneNumber("+994773005533")
                           );
                        }
                        catch
                        {
                            return RedirectToAction("NumFailed", "Send");
                        }
                    }
                    return RedirectToAction("Done", "Send");
                }


                return RedirectToAction("NoNum", "Send");
            }
            catch
            {

                return RedirectToAction("UrlBreak", "Home");
            }

          
        }

        public ActionResult NoNum() {
            return View();
        }


    }

    public class Json {

        public string title;
        public string body;
        public string image;
        public string bigimage;
        public string link;
        public string Vib;
    }
}