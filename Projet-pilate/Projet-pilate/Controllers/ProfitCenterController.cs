using Projet_pilate.Entities;
using Projet_pilate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projet_pilate.Controllers
{
    public class ProfitCenterController : Controller
    {

        // GET: /ProfitCenter/ProfitCenter
        [Route("ProfitCenter/ProfitCenterList", Name = "ProfitCenterList")]
        public ActionResult ProfitCenterList()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<DetailProfitCenterViewModel> models = new List<DetailProfitCenterViewModel>();
            var profitCenters = db.profitCenters.ToList();
           

            foreach (var profitCenter in profitCenters)
            {
                try
                {
                    DetailProfitCenterViewModel model = new DetailProfitCenterViewModel();
                    model.ID = profitCenter.ProfitCenterID;
                    model.Name = profitCenter.Name;
                    model.Owner = profitCenter.Owner;

                    if (profitCenter.PartOwner == null)
                    {
                        model.PartOwner = "Aucun";
                    }
                    else
                    {
                        model.PartOwner = profitCenter.PartOwner;
                    }

                    if (profitCenter.FatherProfitCenter == null)
                    {
                        model.FatherProfitCenter = "Aucun";
                    }
                    else
                    {
                        model.FatherProfitCenter = profitCenter.FatherProfitCenter.Name;
                    }
                   
                    models.Add(model);
                }
                catch (Exception){}
            }

            return View(models);
        }



        //GET: ProfitCenter/CreateProfitCenter
        [Route("ProfitCenter/CreateProfitCenter", Name = "CreateProfitCenter")]
        public ActionResult CreateProfitCenter()
        {
            ProfitCenterViewModel model = new ProfitCenterViewModel();
            ApplicationDbContext db = new ApplicationDbContext();
            List<string> managerNames = new List<string>();
            List<string> profitCenterNames = new List<string>();
            var managers = db.Managers.ToList();
            //
            var ProfitCenters = db.profitCenters.ToList();

            foreach (var manager in managers)
            {
                managerNames.Add(manager.FirstName + " " + manager.LastName);
            }

            foreach (var ProfitCenter in ProfitCenters)
            {
                profitCenterNames.Add(ProfitCenter.Name);
            }

            model.Owners = managerNames;
            model.PartOwners = managerNames;
            model.FatherProfitCenters = profitCenterNames;

            return View(model);
        }


        // POST: ProfitCenter/CreateProfitCenter
        [HttpPost]
        [Route("ProfitCenter/CreateProfitCenter")]
        public ActionResult CreateProfitCenter(ProfitCenterViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            if (!ModelState.IsValid)
            {

                List<string> managerNames = new List<string>();
                var managers = db.Managers.ToList();

                foreach (var item in managers)
                {
                    managerNames.Add(item.FirstName + " " + item.LastName);
                }

                model.Owners = managerNames;
                model.PartOwners = managerNames;
                return View(model);
            }

            ProfitCenter profitCenter = new ProfitCenter()
            {
                Name = model.Name,
            };

            string ownerFirstName = Request.Form["Owner"].ToString().Split(' ')[0];
            string ownerLastName = Request.Form["Owner"].ToString().Split(' ')[1];
            Manager owner = db.Managers.SingleOrDefault(m => m.FirstName == ownerFirstName && m.LastName == ownerLastName);
            profitCenter.Owner = Request.Form["Owner"].ToString();
            owner.ProfitCenters.Add(profitCenter);

            try
            {
                string partOwnerFirstName = Request.Form["PartOwner"].ToString().Split(' ')[0];
                string partOwnerLastName = Request.Form["PartOwner"].ToString().Split(' ')[1];
                Manager partOwner = db.Managers.SingleOrDefault(m => m.FirstName == partOwnerFirstName && m.LastName == partOwnerLastName);
                profitCenter.PartOwner = Request.Form["PartOwner"].ToString();
                partOwner.ProfitCenters.Add(profitCenter);
            }
            catch (Exception){}

            try
            {
                string fatherProfitCenterName = Request.Form["FatherProfitCenter"].ToString().Split(' ')[0];
                ProfitCenter fatherProfitCenter = db.profitCenters.SingleOrDefault(p => p.Name == fatherProfitCenterName);
                profitCenter.FatherProfitCenter = fatherProfitCenter;
              
            }
            catch (Exception) { }


            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                var managers = db.Managers.ToList();
                List<string> managerNames = new List<string>();
                foreach (var manager in managers)
                {
                    managerNames.Add(manager.FirstName + " " + manager.LastName);
                }

                model.Owners = managerNames;
                model.PartOwners = managerNames;
                ViewData["Owner"] = Request.Form["Owner"].ToString();
                ViewData["PartOwner"] = Request.Form["PartOwner"].ToString();

                string message = "Un centre de profit du même nom existe déjà !";
                ModelState.AddModelError(string.Empty, message);

                return View(model);
            }

            return RedirectToAction("ProfitCenterList", "ProfitCenter");
        }
    }
}

