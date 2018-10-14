using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeaderPush.Models;
using ShopifySharp.Enums;

namespace LeaderPush.Controllers
{
    public class HomeController : Controller
    {
        string API = ConfigurationManager.AppSettings.Get("APIkey").ToString();
        string Secret = ConfigurationManager.AppSettings.Get("SecretKey").ToString();
        PushDBEntities db = new PushDBEntities();

        public ActionResult Index()
        {
            


            return View();
        }

        public ActionResult MainMenu() {
            DB list = new DB();
            list.shopLinks = db.ShopLinks.ToList();
            list.users = db.Users.ToList();
            

            return View(list);
        }

        public ActionResult Install(string shop)
        {
            string usersMyShopifyUrl = shop;

            string redirectUrl = "https://leaderpush.com/home/auth";


            var scopes = new List<AuthorizationScope>(){
                AuthorizationScope.ReadCustomers,
                AuthorizationScope.WriteCustomers,
                AuthorizationScope.WriteScriptTags
            };




            string authUrl = AuthorizationService.BuildAuthorizationUrl(scopes, usersMyShopifyUrl, API, redirectUrl).ToString();


            return Redirect(authUrl);

        }

        public async System.Threading.Tasks.Task<ActionResult> auth() {
            DB list = new DB();
            string code = Request.QueryString["code"];
            string myShopifyUrl = Request.QueryString["shop"];
            string accessToken = await AuthorizationService.Authorize(code, myShopifyUrl, API, Secret);
            Session["token"] = accessToken;
            var qs = Request.QueryString.ToKvps();

            if (AuthorizationService.IsAuthenticRequest(qs, Secret))
            {
                Session["shop"] = myShopifyUrl;
                if (db.ShopLinks.Where(w => w.Shop == myShopifyUrl).Count() > 0)
                {
                    list.shopLinks = db.ShopLinks.ToList();
                    if(db.ShopLinks.Where(w => w.Shop == myShopifyUrl).FirstOrDefault().IsPremium == true)
                    {

                        Session["premium"] = true;
                    }
                    list.users = db.Users.ToList();
                }
                else {
                    ShopLink NewShop = new ShopLink();
                    NewShop.Shop = myShopifyUrl;
                    NewShop.Token = accessToken;
                    NewShop.InstallDate = DateTime.Now;
                    NewShop.IsPremium = false;
                    NewShop.SendLimit = 500;

                    db.ShopLinks.Add(NewShop);
                    db.SaveChanges();

                    list.shopLinks = db.ShopLinks.ToList();
                    list.users = db.Users.ToList();

                    var service = new ScriptTagService(myShopifyUrl, accessToken);
                    var tag = new ScriptTag()
                    {
                        Event = "onload",
                        Src = "https://www.leaderpush.com/public/scripts/main.js",
                    };

                    tag = await service.CreateAsync(tag);
                }
            }
            else
            {
                //Request is not authentic and should not be acted on.
                return Content("Error, Please Try Again...");
            }

            return View(list);
        }

        public async System.Threading.Tasks.Task<ActionResult> SMS() {

            string myShopifyUrl = Session["shop"].ToString();
            string accessToken = Session["token"].ToString();

            var service = new CustomerService(myShopifyUrl, accessToken);
            IEnumerable<Customer> customers = await service.ListAsync();

            List<Customer> listOfCus = customers.Where(w=>w.Phone != null).ToList();

            return View(listOfCus);
        }


    }
}