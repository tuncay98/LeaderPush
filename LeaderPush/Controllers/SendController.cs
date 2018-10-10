using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeaderPush.Models;

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
        
    }
}