using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeaderPush.Models;

namespace LeaderPush.Controllers
{
    public class PremiumController : Controller
    {
        PushDBEntities db = new PushDBEntities();
        // GET: Premium
        public ActionResult Index()
        {
            return View();
        }

        public async System.Threading.Tasks.Task<ActionResult> Buy() {

            string myShopifyUrl = Session["shop"].ToString();
            string accessToken = db.ShopLinks.Where(w => w.Shop == myShopifyUrl).FirstOrDefault().Token;

           
            try
            {
                var service = new RecurringChargeService(myShopifyUrl, accessToken);
                var charge = new RecurringCharge()
                {
                    Name = "Premium Account",
                    Price = 7,
                   /* Test= true,*/
                    //Test = true, //Marks this charge as a test, meaning it won't charge the shop owner.
                    ReturnUrl = "https://www.leaderpush.com/Premium/Confirm"
                };

                charge = await service.CreateAsync(charge);

                ShopLink link = db.ShopLinks.Where(w => w.Shop == myShopifyUrl).FirstOrDefault();
                link.PremiumID = charge.Id;
                db.Entry(link).Property(w => w.PremiumID).IsModified = true;
                db.SaveChanges();

                return Redirect(charge.ConfirmationUrl);
            }
            catch
            {

                return RedirectToAction("UrlBreak", "Home");
            }
        }

        public async System.Threading.Tasks.Task<ActionResult> Confirm(long charge_id) {

            string myShopifyUrl = db.ShopLinks.Where(w => w.PremiumID == charge_id).FirstOrDefault().Shop;
            string accessToken = db.ShopLinks.Where(w => w.PremiumID == charge_id).FirstOrDefault().Token;



            try
            {
                var service = new RecurringChargeService(myShopifyUrl, accessToken);

                var charge = await service.GetAsync(charge_id);

                if (charge.Status == "accepted")
                {

                    await service.ActivateAsync(charge_id);

                    ShopLink link = db.ShopLinks.Where(w => w.Shop == myShopifyUrl).FirstOrDefault();
                    link.IsPremium = true;
                    db.Entry(link).Property(w => w.IsPremium).IsModified = true;
                    db.SaveChanges();
                }
                else if (charge.Status == "declined")
                {

                    return RedirectToAction("Failed", "Premium");
                }

                return RedirectToAction("Install", "Home", new { shop = myShopifyUrl});
            }
            catch
            {

                return RedirectToAction("UrlBreak", "Home");
            }
        }

        public ActionResult Done() {

            return View();
        }
        public ActionResult Failed()
        {

            return View();
        }


    }
}