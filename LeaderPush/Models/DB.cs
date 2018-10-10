using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LeaderPush.Models;

namespace LeaderPush.Models
{

    public class DB
    {
        public static PushDBEntities data = new PushDBEntities();

        public List<ShopLink> shopLinks { get; set; }
        public List<User> users { get; set; }
    }
}