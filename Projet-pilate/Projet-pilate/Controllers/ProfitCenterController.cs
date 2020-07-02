using Microsoft.Ajax.Utilities;
using Projet_pilate.Entities;
using Projet_pilate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projet_pilate.Controllers
{
    public class ProfitCenterController : Controller
    {

        // GET: /ProfitCenter/ProfitCenter
        [Authorize(Roles = "Administrateur, Super-Administrateur")]
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
                    model.Owners = profitCenter.Owner;

                    if (profitCenter.PartOwner == null)
                    {
                        model.PartOwners = "Aucun";
                    }
                    else
                    {
                        model.PartOwners = profitCenter.PartOwner;
                    }

                    if (profitCenter.FatherProfitCenter == null)
                    {
                        model.FatherProfitCenters = "Aucun";
                    }
                    else
                    {
                        model.FatherProfitCenters = profitCenter.FatherProfitCenter.Name;
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
            List<int> managerID = new List<int>();
            List<string> profitCenterNames = new List<string>();
            var managers = db.Managers.ToList();
            var ProfitCenters = db.profitCenters.ToList();

            foreach (var manager in managers)
            {
                managerNames.Add(manager.FirstName + " " + manager.LastName);
                managerID.Add(manager.ManagerID);

            }

            foreach (var ProfitCenter in ProfitCenters)
            {
                profitCenterNames.Add(ProfitCenter.Name);
            }

            model.Owners = managerNames;
            model.OwnersID = managerID;
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
                // string fatherProfitCenterName = Request.Form["FatherProfitCenter"].ToString().Split(' ')[0];
                string fatherProfitCenterName = Request.Form["FatherProfitCenter"].ToString();
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

        // GET: ProfitCenter/Edit
        public ActionResult EditProfitCenter(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var profitcenter = db.profitCenters.Single(p => p.ProfitCenterID == id);
            var father = db.profitCenters.Single(p => p.ProfitCenterID == profitcenter.FatherProfitCenterID);
            UpdateProfitCenterViewModel model = new UpdateProfitCenterViewModel()
            {
                ID = profitcenter.ProfitCenterID,
                Name = profitcenter.Name,
                Owner = profitcenter.Owner,
                PartOwner = profitcenter.PartOwner,
                FatherProfitCenterID = profitcenter.FatherProfitCenterID,
                FatherProfitCenter = father.Name,
                Cost = profitcenter.Cost,
                Turnover = profitcenter.Turnover
                              
            };

            List<string> managerNames = new List<string>();
            List<int> managerID = new List<int>();
            List<string> profitCenterNames = new List<string>();
            List<int> profitCenterID = new List<int>();
            var managers = db.Managers.ToList();
            var ProfitCenters = db.profitCenters.ToList();

            //

            foreach (var manager in managers)
            {
                managerNames.Add(manager.FirstName + " " + manager.LastName);
                managerID.Add(manager.ManagerID);

            }

            foreach (var ProfitCenter in ProfitCenters)
            {
                profitCenterNames.Add(ProfitCenter.Name);
                profitCenterID.Add(ProfitCenter.ProfitCenterID);
            }

            //model.FatherProfitCenter = 
            model.ListOwners= managerNames;
            model.ListOwnersID = managerID;
            model.ListPartOwners= managerNames;
            model.ListFatherProfitCenters = profitCenterNames;
            
            return View(model);
        }

        /*
        // POST: ProfitCenter/Edit
        [HttpPost]
        public ActionResult Edit(UpdateSubsidiaryViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Subsidiary subsidiary = db.Subsidiaries.Single(s => s.SubsidiaryID == model.ID);


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            subsidiary.Name = model.Name;
            subsidiary.Siren = model.Siren;
            subsidiary.Address = model.Address;
            subsidiary.PostaleCode = model.PostaleCode;
            subsidiary.City = model.City;
            subsidiary.ManagerFirstName = model.ManagerFirstName;
            subsidiary.ManagerLastName = model.ManagerLastName;

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                string message = "Vérifier qu'une filiale ayant le même siren ou le même nom n'existe pas déjà !";
                ModelState.AddModelError(string.Empty, message);
                return View("Edit", model);
            }

            
            return RedirectToAction("ProfitCenterList", "ProfitCenter");
    }*/

        [Route("ProfitCenter/DeleteProfitCenter", Name = "DeleteProfitCenter")]
        public ActionResult DeleteProfitCenter(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var profitCenters = db.profitCenters.ToList();
            List<DetailProfitCenterViewModel> models = new List<DetailProfitCenterViewModel>();
            var companyToDelete = profitCenters.Single(c => c.ProfitCenterID == id);
            db.profitCenters.Remove(companyToDelete);


            foreach (var profitCenter in profitCenters)
            {
                try
                {
                    DetailProfitCenterViewModel model = new DetailProfitCenterViewModel();
                    model.ID = profitCenter.ProfitCenterID;
                    model.Name = profitCenter.Name;
                    model.Owners = profitCenter.Owner;

                    if (profitCenter.PartOwner == null)
                    {
                        model.PartOwners = "Aucun";
                    }
                    else
                    {
                        model.PartOwners = profitCenter.PartOwner;
                    }

                    if (profitCenter.FatherProfitCenter == null)
                    {
                        model.FatherProfitCenters = "Aucun";
                    }
                    else
                    {
                        model.FatherProfitCenters = profitCenter.FatherProfitCenter.Name;
                    }

                    models.Add(model);
                }
                catch (Exception) { }
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                string message = "Un consultant ou une mission est déjà affecté(e) à ce centre de profit !";
                ModelState.AddModelError(string.Empty, message);
                return View("ProfitCenterList", models);
            }

            return RedirectToAction("ProfitCenterList", "ProfitCenter");
        }

    }
}

