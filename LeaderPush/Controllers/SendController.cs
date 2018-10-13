using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeaderPush.Models;
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
                LeaderPush.Models.User user = new User();
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

        public ActionResult Go(string Title, string Body, string Link, HttpPostedFileBase Upload) {
            string shop = Session["shop"].ToString();
            if (db.ShopLinks.Where(w => w.Shop == shop).FirstOrDefault().SendLimit> 0) {
                var pushEndpoint = @"https://fcm.googleapis.com/fcm/send/efz_TLX_rLU:APA91bE6U0iybLYvv0F3mf6uDLB6....";
                var p256dh = @"BKK18ZjtENC4jdhAAg9OfJacySQiDVcXMamy3SKKy7FwJcI5E0DKO9v4V2Pb8NnAPN4EVdmhO............";
                var auth = @"fkJatBBEl...............";

                var subject = @"mailto:example@example.com";
                var publicKey = @"BAS21PmINNueC0awLLy-VE6FMManBBJMZT6NUAwb3Ou0gsAuuMrYwiJGnW4f__ew1yIh1VnXVwCNTpFDig6i7eA";
                var privateKey = @"mXjwDyDRBb48nDdUyUtWFpLfenacZP7sHJJjNUsXM2Q";

                var subscription = new PushSubscription(pushEndpoint, p256dh, auth);
                var vapidDetails = new VapidDetails(subject, publicKey, privateKey);
                var gcmAPIKey = @"[your key here]";

                var webPushClient = new WebPushClient();
                try
                {
                    webPushClient.SendNotification(subscription, "payload", vapidDetails);
                    webPushClient.SendNotification(subscription, "payload", gcmAPIKey);
                }
                catch (WebPushException exception)
                {
                    Console.WriteLine("Http STATUS code" + exception.StatusCode);
                }
            }


            return Content("");
        }


        public ActionResult GoPremium() {


            return Content("");
        }


        
    }
}