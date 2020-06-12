using Projet_pilate.Entities;
using Projet_pilate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projet_pilate.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MessageViewModel model = new MessageViewModel();

            var messageAccueil = db.Messages.Single();
            model.Message = messageAccueil.message.ToString();

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}