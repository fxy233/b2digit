using Microsoft.Ajax.Utilities;
using Projet_pilate.Entities;
using Projet_pilate.Migrations;
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
        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager, Administrateur-ventes")]
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
                    var owner = db.Managers.Single(m=>m.ManagerID==profitCenter.Owner);
                    model.Owners = owner.FirstName + " " +owner.LastName;
                     
                    if (profitCenter.PartOwner == 0 )
                    {
                        model.PartOwners = "Aucun";
                    }
                    else
                    {
                        try { 
                        var partowner = db.Managers.Single(m => m.ManagerID == profitCenter.PartOwner);
                        model.PartOwners = partowner.FirstName + " " + partowner.LastName;
                        }
                        catch (Exception)
                        {
                            model.PartOwners = "Aucun";
                        }
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


        [Authorize(Roles = "Administrateur, Super-Administrateur")]
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

            foreach(var item in db.Managers.ToList())
            {
                if (item.FirstName+" "+item.LastName== Request.Form["Owner"])
                {
                    profitCenter.Owner = item.ManagerID;
                    item.ProfitCenters.Add(profitCenter);
                    break;
                }
            }



            
            if (Request.Form["PartOwner"].ToString()!= "Aucun")
            {
                try
                {
                    foreach (var item in db.Managers.ToList())
                    {
                        if (item.FirstName + " " + item.LastName == Request.Form["PartOwner"])
                        {
                            profitCenter.PartOwner = item.ManagerID;
                            item.ProfitCenters.Add(profitCenter);
                            break;
                        }
                    }


                    
                }
                catch (Exception) { }
            }
            

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

        [Authorize(Roles = "Administrateur, Super-Administrateur")]
        // GET: ProfitCenter/Edit
        public ActionResult EditProfitCenter(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var profitcenter = db.profitCenters.Single(p => p.ProfitCenterID == id);
            
            var father = profitcenter.FatherProfitCenterID==null ? null : db.profitCenters.Single(p => p.ProfitCenterID == profitcenter.FatherProfitCenterID);

            var owner = db.Managers.Single(m => m.ManagerID == profitcenter.Owner);
            UpdateProfitCenterViewModel model = new UpdateProfitCenterViewModel();
            try
            {
                var partowner = profitcenter.PartOwner == 0 ? null : db.Managers.Single(m => m.ManagerID == profitcenter.PartOwner);
                model = new UpdateProfitCenterViewModel()
                {
                    ID = profitcenter.ProfitCenterID,
                    Name = profitcenter.Name,
                    Owner = owner.FirstName + " " + owner.LastName,
                    PartOwner = partowner == null ? "Aucun" : partowner.FirstName + " " + partowner.LastName,
                    FatherProfitCenterID = profitcenter.FatherProfitCenterID,
                    FatherProfitCenter = father == null ? "Aucun" : father.Name,
                    Cost = profitcenter.Cost,
                    Turnover = profitcenter.Turnover

                };
            }
            catch (Exception)
            {
                model = new UpdateProfitCenterViewModel()
                {
                    ID = profitcenter.ProfitCenterID,
                    Name = profitcenter.Name,
                    Owner = owner.FirstName + " " + owner.LastName,
                    PartOwner = "Aucun",
                    FatherProfitCenterID = profitcenter.FatherProfitCenterID,
                    FatherProfitCenter = father == null ? "Aucun" : father.Name,
                    Cost = profitcenter.Cost,
                    Turnover = profitcenter.Turnover

                };
            }

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

        
        // POST: ProfitCenter/Edit
        [HttpPost]
        public ActionResult EditProfitCenter(UpdateProfitCenterViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var profitcenter = db.profitCenters.Single(p => p.ProfitCenterID == model.ID);

            


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var plist = db.profitCenters.ToList();
            foreach (var p in plist)
            {
                if (p.Name==profitcenter.Name)
                {
                    continue;
                }
                if(p.Name==model.Name)
                {
                    string message = "Vérifier qu'une filiale ayant le même siren ou le même nom n'existe pas déjà !";
                    ModelState.AddModelError(string.Empty, message);
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
                    model.ListOwners = managerNames;
                    model.ListOwnersID = managerID;
                    model.ListPartOwners = managerNames;
                    model.ListFatherProfitCenters = profitCenterNames;
                    return View(model);
                }

            }

            int owner = 0;
            int partowner = 0;
            foreach(var m in db.Managers.ToList())
            {
                if (m.FirstName+" " + m.LastName == model.Owner)
                {
                    owner = m.ManagerID; 
                }
                if (m.FirstName + " " + m.LastName == model.PartOwner)
                {
                    partowner = m.ManagerID;
                }
            }

            profitcenter.Name = model.Name;
            profitcenter.Owner = owner;
            profitcenter.PartOwner = partowner;
            model.FatherProfitCenterID = model.FatherProfitCenter == "Aucun" ? (int?)null : db.profitCenters.Single(p => p.Name == model.FatherProfitCenter).ProfitCenterID;
            profitcenter.FatherProfitCenterID = model.FatherProfitCenterID;
            profitcenter.FatherProfitCenter = profitcenter.FatherProfitCenterID==null?null:db.profitCenters.Single(p=> p.ProfitCenterID==profitcenter.FatherProfitCenterID);

            if (profitcenter.Name == (profitcenter.FatherProfitCenter==null? "Aucun": profitcenter.FatherProfitCenter.Name))
            {
                string message = "La société père d'une filiale ne peut pas être la sienne.";
                ModelState.AddModelError(string.Empty, message);
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
                model.ListOwners = managerNames;
                model.ListOwnersID = managerID;
                model.ListPartOwners = managerNames;
                model.ListFatherProfitCenters = profitCenterNames;
                return View(model);
            }

            if (profitcenter.Owner == profitcenter.PartOwner && profitcenter.PartOwner!=0)
            {
                string message = "Un centre de profit ne peut pas avoir son propriétaire en associé.";
                ModelState.AddModelError(string.Empty, message);
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
                model.ListOwners = managerNames;
                model.ListOwnersID = managerID;
                model.ListPartOwners = managerNames;
                model.ListFatherProfitCenters = profitCenterNames;
                return View(model);
            }

            profitcenter.Cost = model.Cost;
            profitcenter.Turnover = model.Turnover;

            


            db.SaveChanges();
            
            
            

            
            return RedirectToAction("ProfitCenterList", "ProfitCenter");
    }

        [Authorize(Roles = "Administrateur, Super-Administrateur")]
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
                    var owner = db.Managers.Single(m => m.ManagerID == profitCenter.Owner);
                    model.Owners = owner.FirstName+" " +owner.LastName;

                    if (profitCenter.PartOwner == 0)
                    {
                        model.PartOwners = "Aucun";
                    }
                    else
                    {
                        var partOwners = db.Managers.Single(m => m.ManagerID == profitCenter.PartOwner);
                        model.PartOwners = partOwners.FirstName + " " + partOwners.LastName;
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

        [Route("ProfitCenter/Rapports", Name = "Rapports")]
        public ActionResult Rapports()
        {
            return View();
        }

    }
}

