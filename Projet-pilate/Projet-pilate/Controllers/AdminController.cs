using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.Excel;
using Microsoft.Ajax.Utilities;
using Projet_pilate.Entities;
using Projet_pilate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace Projet_pilate.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles = "Administrateur, Super-Administrateur, Administrateur-ventes")]
        // GET: /Admin/ActivationCRA
        [Route("Admin/ActivationCRA", Name = "ActivationCRA")]
        public ActionResult ActivationCRA()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AdminViewModel model = new AdminViewModel();

            var currentMonth = db.MonthActivations.Single();

            model.MoisActif = currentMonth.Periode.ToString("MMMM", CultureInfo.CurrentCulture) +
                               " " + currentMonth.Periode.Year.ToString();

            var prochainMois = currentMonth.Periode.AddMonths(1);

            model.MoisProchain = prochainMois.ToString("MMMM", CultureInfo.CurrentCulture) +
                               " " + prochainMois.Year.ToString();

            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur, Administrateur-ventes")]
        [HttpPost]
        public ActionResult ActivationCRA(AdminViewModel model)
        {

            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur, Administrateur-ventes")]
        [HttpPost]
        public ActionResult ActiverCra(AdminViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var activerMois = db.MonthActivations.SingleOrDefault();

            activerMois.Periode = activerMois.Periode.AddMonths(1);

            db.SaveChanges();

            return View("ActivationSuccess", model);
        }


        [Authorize(Roles = "Administrateur, Super-Administrateur, Administrateur-ventes, Manager")]
        // GET: /Admin/ManagerList
        [Route("Admin/ManagerList", Name = "ManagerList")]
        public ActionResult ManagerList()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<DetailManagerViewModel> models = new List<DetailManagerViewModel>();

            var managers = db.Managers.ToList();

            foreach (var manager in managers)
            {
                DetailManagerViewModel model = new DetailManagerViewModel()
                {
                    ID = manager.ManagerID,
                    FirstName = manager.FirstName,
                    LastName = manager.LastName,
                    Email = manager.Email,
                    EntryDate = manager.EntryDate,
                    Subsidiary = manager.Subsidiary.Name,
                    MonthlyCost = manager.MonthlyCost,
                };

      
                models.Add(model);
            }

            return View(models);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur")]
        // GET: /Admin/CreateManager
        [Route("Admin/CreateManager", Name = "CreateManager")]
        public ActionResult CreateManager()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var subsidiaries = db.Subsidiaries.ToList();
            RegisterManagerViewModel model = new RegisterManagerViewModel();
            List<string> subsidiariesNames = new List<string>();

            foreach (var sub in subsidiaries)
            {
                subsidiariesNames.Add(sub.Name);
            }

            model.Subsidiaries = subsidiariesNames;

            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur")]
        // POST: /Admin/CreateManager
        [HttpPost]
        [Route("Admin/CreateManager")]
        public ActionResult CreateManager(RegisterManagerViewModel model)
        {
            string SubsidiaryName = Request.Form["Subsidiary"].ToString();
            ApplicationDbContext db = new ApplicationDbContext();
            var subsidiary = db.Subsidiaries.Single(s => s.Name == SubsidiaryName);
            DateTime? nullDateTime = null;
          

            if (!ModelState.IsValid)
            {
 
                SubsidiaryName = Request.Form["Subsidiary"].ToString();
                var subsidiaries = db.Subsidiaries.ToList();
                List<string> subsidiariesNames = new List<string>();
                foreach (var sub in subsidiaries)
                {
                    subsidiariesNames.Add(sub.Name);
                }
                model.Subsidiaries = subsidiariesNames;
                ViewData["Subsidiary"] = SubsidiaryName;

                return View(model);

            }

            Manager manager = new Manager()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                EntryDate = model.EntryDate,
                DateOfDeparture = nullDateTime,
            };

            manager.MealCost = model.MealCost;
            manager.TravelPackage = model.TravelPackage;
            manager.ExceptionalCost = model.ExceptionalCost;
            manager.MonthlyCost = model.Cost;
            manager.role = Request.Form["role"];
            subsidiary.Managers.Add(manager);
         
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
            
                SubsidiaryName = Request.Form["Subsidiary"].ToString();
                var subsidiaries = db.Subsidiaries.ToList();
                List<string> subsidiariesNames = new List<string>();
                foreach (var sub in subsidiaries)
                {
                    subsidiariesNames.Add(sub.Name);
                }
                model.Subsidiaries = subsidiariesNames;
                ViewData["Subsidiary"] = SubsidiaryName;

                string message = "Un manager ayant la même adresse email existe déjà!";
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            return RedirectToAction("ManagerList","admin") ;
        }


        [Authorize(Roles = "Administrateur, Super-Administrateur, Administrateur-ventes")]
        // GET: /Admin/EditManager
        [Route("Admin/EditManager", Name = "EditManager")]
        public ActionResult EditManager(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var manager = db.Managers.Single(m => m.ManagerID == id);


            UpdateManagerViewModel model = new UpdateManagerViewModel()
            {
                ID = manager.ManagerID,
                FirstName = manager.FirstName,
                LastName = manager.LastName,
                Email = manager.Email,
                EntryDate = manager.EntryDate,
                Cost = manager.MonthlyCost,
                MealCost = manager.MealCost,
                TravelPackage = manager.TravelPackage,
                ExceptionalCost = manager.ExceptionalCost,
                Subsidiary = manager.Subsidiary.Name,
                role = manager.role,
            };

            var subsidiaries = db.Subsidiaries.ToList();
            List<string> subsidiariesNames = new List<string>();

            foreach (var sub in subsidiaries)
            {
                subsidiariesNames.Add(sub.Name);
            }

            model.Subsidiaries = subsidiariesNames;

            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur, Administrateur-ventes")]
        // POST: /Admin/EditManager
        [HttpPost]
        [Route("Admin/EditManager")]
        public ActionResult EditManager(UpdateManagerViewModel model)
        {
            string SubsidiaryName = Request.Form["Subsidiary"].ToString();
            ApplicationDbContext db = new ApplicationDbContext();
            var manager = db.Managers.SingleOrDefault(m => m.ManagerID == model.ID);
            var NewSubsidiary = db.Subsidiaries.Single(s => s.Name == SubsidiaryName);

            if (!ModelState.IsValid)
            {

                SubsidiaryName = Request.Form["Subsidiary"].ToString();
                var subsidiaries = db.Subsidiaries.ToList();
                List<string> subsidiariesNames = new List<string>();
                foreach (var sub in subsidiaries)
                {
                    subsidiariesNames.Add(sub.Name);
                }
                model.Subsidiaries = subsidiariesNames;
                ViewData["Subsidiary"] = SubsidiaryName;

                return View(model);

            }


            manager.FirstName = model.FirstName;
            manager.LastName = model.LastName;
            manager.Email = model.Email;
            manager.EntryDate = model.EntryDate;
            manager.MealCost = model.MealCost;
            manager.TravelPackage = model.TravelPackage;
            manager.ExceptionalCost = model.ExceptionalCost;
            manager.MonthlyCost = model.Cost;
            manager.role = Request.Form["role"];

            var currentSubsidiary = manager.Subsidiary;
            currentSubsidiary.Managers.Remove(manager);
            NewSubsidiary.Managers.Add(manager);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                SubsidiaryName = Request.Form["Subsidiary"].ToString();
                var subsidiaries = db.Subsidiaries.ToList();
                List<string> subsidiariesNames = new List<string>();
                foreach (var sub in subsidiaries)
                {
                    subsidiariesNames.Add(sub.Name);
                }
                model.Subsidiaries = subsidiariesNames;
                ViewData["Subsidiary"] = SubsidiaryName;

                string message = "Ce manager existe déjà !";
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }


            return RedirectToAction("ManagerList", "admin");
        }




        [Authorize(Roles = "Administrateur, Super-Administrateur, Administrateur-ventes")]
        public ActionResult DeleteManager(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var managers = db.Managers.ToList();
            List<DetailManagerViewModel> models = new List<DetailManagerViewModel>();
            var managerToDelete = managers.SingleOrDefault(c => c.ManagerID == id);

            try
            {
                db.Entry(managerToDelete).State = EntityState.Deleted;
                db.SaveChanges();
                managers.Remove(managerToDelete);
            }
            catch (Exception)
            {
                foreach (var manager in managers)
                {
                    var subsidiary = db.Subsidiaries.Single(s => s.SubsidiaryID == manager.SubsidiaryID);

                    DetailManagerViewModel model = new DetailManagerViewModel()
                    {
                        ID = manager.ManagerID,
                        FirstName = manager.FirstName,
                        LastName = manager.LastName,
                        Email = manager.Email,
                        EntryDate = manager.EntryDate,
                        Subsidiary = subsidiary.Name,
                        MonthlyCost = manager.MonthlyCost,
                    };
                    models.Add(model);
                }

                string message = "Ce manager ne peut être supprimé car il a déjà contracté un contrat avec un client !";
                ModelState.AddModelError(string.Empty, message);
                return View("ManagerList", models);
            }

            foreach (var manager in managers)
            {
                DetailManagerViewModel model = new DetailManagerViewModel()
                {
                    ID = manager.ManagerID,
                    FirstName = manager.FirstName,
                    LastName = manager.LastName,
                    Email = manager.Email,
                    EntryDate = manager.EntryDate,
                    Subsidiary = manager.Subsidiary.Name,
                    MonthlyCost = manager.MonthlyCost,
                };
                models.Add(model);
            }

            return RedirectToAction("ManagerList", "admin");
        }




        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager,Administrateur-ventes")]
        //// GET: /Admin/ConsultantList
        [Route("Admin/ConsultantList", Name = "ConsultantList")]
        public ActionResult ConsultantList()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<DetailConsultantViewModel> models = new List<DetailConsultantViewModel>();

            var consultants = db.Consultants.ToList();

            foreach (var consultant in consultants)
            {
                double cost = 0;
                if (consultant.Status=="Consultant"||consultant.Status=="Salarié")
                {
                    if (consultant.DailyCost == 0)
                    {
                        cost = consultant.MonthlyCost * 1.5 + consultant.MealCost + consultant.ExceptionalCost + consultant.TravelPackage;
                        cost = cost * 12 / 218;
                    } else
                    {
                        cost = consultant.DailyCost * 1.5 + consultant.MealCost + consultant.ExceptionalCost + consultant.TravelPackage;
                    }
                }
                if (consultant.Status == "Sous-Traitant")
                {
                    if (consultant.DailyCost == 0)
                    {
                        cost = consultant.MonthlyCost + consultant.MealCost + consultant.ExceptionalCost + consultant.TravelPackage;
                        cost = cost * 12 / 218;
                    }
                    else
                    {
                        cost = consultant.DailyCost + consultant.MealCost + consultant.ExceptionalCost + consultant.TravelPackage;
                    }
                }
                DetailConsultantViewModel model = new DetailConsultantViewModel()
                {
                    ID = consultant.ConsultantID,
                    FirstName = consultant.FirstName,
                    LastName = consultant.LastName,
                    Email = consultant.Email,
                    EntryDate = consultant.EntryDate,
                    Status = consultant.Status,
                    DailyCost = cost,
                    //MonthlyCost = consultant.MonthlyCost,
                    ProfitCenter = db.profitCenters.Single(p => p.ProfitCenterID == consultant.ProfitCenterID).Name,
                    Subsidiary = db.Subsidiaries.Single(s=>s.SubsidiaryID==consultant.SubsidiaryID).Name,
                };

                models.Add(model);
            }

            return View(models);
        }


        // GET: /Admin/CreateManager
        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager,Administrateur-ventes")]
        [Route("Admin/CreateConsultant", Name = "CreateConsultant")]
        public ActionResult CreateConsultant()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var profitCenters = db.profitCenters.ToList();
            var subsidiaries = db.Subsidiaries.ToList();
            RegisterConsultantViewModel model = new RegisterConsultantViewModel();
            List<string> profitCenterNames = new List<string>();
            List<string> subsidiariesNames = new List<string>();

            foreach (var profitCenter in profitCenters)
            {
                profitCenterNames.Add(profitCenter.Name);
            }
            model.ProfitCenters = profitCenterNames;

            foreach (var sub in subsidiaries)
            {
                subsidiariesNames.Add(sub.Name);
            }
            model.Subsidiaries = subsidiariesNames;

            return View(model);
        }


        // POST: /Admin/CreateConsultant
        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager,Administrateur-ventes")]
        [HttpPost]
        [Route("Admin/CreateConsultant")]
        public ActionResult CreateConsultant(RegisterConsultantViewModel model)
        {
            string SubsidiaryName;
            string ProfitCenterName;
            ProfitCenter profitCenter;
            Subsidiary subsidiary;
            DateTime? nullDateTime = null;
            Consultant consultant = new Consultant();
            ApplicationDbContext db = new ApplicationDbContext();
            string selectedTypeCost;



            if (!ModelState.IsValid)
            {
                /*selectedTypeCost = Request.Form["CostType"].ToString();
                SubsidiaryName = Request.Form["Subsidiary"].ToString();
                ProfitCenterName = Request.Form["ProfitCenter"].ToString();*/
                var profitCenters = db.profitCenters.ToList();
                var subsidiaries = db.Subsidiaries.ToList();
                List<string> profitCenterNames = new List<string>();
                List<string> subsidiariesNames = new List<string>();

                foreach (var profit in profitCenters)
                {
                    profitCenterNames.Add(profit.Name);
                }
                model.ProfitCenters = profitCenterNames;

                foreach (var sub in subsidiaries)
                {
                    subsidiariesNames.Add(sub.Name);
                }
                model.Subsidiaries = subsidiariesNames;

 
                /*ViewData["Subsidiary"] = SubsidiaryName;
                ViewData["ProfitCenter"] = ProfitCenterName;
                ViewData["CostType"] = selectedTypeCost;*/

                return View(model);
            }

                selectedTypeCost = Request.Form["CostType"].ToString();
                SubsidiaryName = Request.Form["Subsidiary"].ToString();
                ProfitCenterName = Request.Form["ProfitCenter"].ToString();
                profitCenter = db.profitCenters.Single(s => s.Name == ProfitCenterName);
                subsidiary = db.Subsidiaries.Single(s => s.Name == SubsidiaryName);

                consultant.FirstName = model.FirstName;
                consultant.LastName = model.LastName;
                consultant.Email = model.Email;
                consultant.EntryDate = model.EntryDate;
                consultant.DateOfDeparture = nullDateTime;
                consultant.Status = Request.Form["Status"].ToString();
                consultant.MealCost = model.MealCost;
                consultant.TravelPackage = model.TravelPackage;
                consultant.ExceptionalCost = model.ExceptionalCost;



               if (selectedTypeCost == "MonthlyCost")
                {
                    consultant.MonthlyCost = model.Cost;
                }
                else
                {
                    consultant.DailyCost = model.Cost;
                }
          
                subsidiary.Consultants.Add(consultant);
                profitCenter.Consultants.Add(consultant);

                ViewData["Subsidiary"] = SubsidiaryName;
                ViewData["ProfitCenter"] = ProfitCenterName;
                ViewData["CostType"] = selectedTypeCost;

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                selectedTypeCost = Request.Form["CostType"].ToString();
                SubsidiaryName = Request.Form["Subsidiary"].ToString();
                ProfitCenterName = Request.Form["ProfitCenter"].ToString();
                var profitCenters = db.profitCenters.ToList();
                var subsidiaries = db.Subsidiaries.ToList();
                List<string> profitCenterNames = new List<string>();
                List<string> subsidiariesNames = new List<string>();

                foreach (var profit in profitCenters)
                {
                    profitCenterNames.Add(profit.Name);
                }
                model.ProfitCenters = profitCenterNames;

                foreach (var sub in subsidiaries)
                {
                    subsidiariesNames.Add(sub.Name);
                }
                model.Subsidiaries = subsidiariesNames;

                ViewData["Subsidiary"] = SubsidiaryName;
                ViewData["ProfitCenter"] = ProfitCenterName;
                ViewData["CostType"] = selectedTypeCost;


                string message = "Ce consultant existe déjà";
                ModelState.AddModelError(string.Empty, message);
                return View( model);
            }

            return RedirectToAction("ConsultantList", "admin");
        }



        // GET: /Admin/EditManager
        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager,Administrateur-ventes")]
        [Route("Admin/EditConsultant", Name = "EditConsultant")]
        public ActionResult EditConsultant(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var consultant = db.Consultants.Single(m => m.ConsultantID == id);

            if (User.IsInRole("Manager"))
            {
                var email = User.Identity.Name;
                var user = db.Users.Single(u => u.Email == email);
                string name = user.FirstName + " " + user.LastName;
                List<int> pIdlist = new List<int>();
                foreach (var item in db.profitCenters.ToList())
                {
                    var manager = db.Managers.Single(m => m.ManagerID == item.Owner);

                    if (manager.FirstName+" "+manager.LastName == name )
                    {
                        pIdlist.Add(item.ProfitCenterID);
                    }

                }
                var pc = db.profitCenters.Single(p => p.ProfitCenterID == consultant.ProfitCenterID);
                Boolean ability = false;
                if (pIdlist.Count != 0)
                {
                    foreach (int profitId in pIdlist)
                    {
                        if (consultant.ProfitCenterID == profitId)
                        {
                            ability = true;
                        }

                        if (profitId == pc.FatherProfitCenterID)
                        {
                            ability = true;
                        }

                    }
                }
                var partowner = pc.PartOwner==0? null: db.Managers.Single(m => m.ManagerID == pc.PartOwner);
                if (name == (partowner==null?"Aucun": partowner.FirstName+" "+ partowner.LastName))
                {
                    ability = true;
                }
                var consultants = db.Consultants.ToList();
                List<DetailConsultantViewModel> models = new List<DetailConsultantViewModel>();
                if (!ability)
                {

                    foreach (var c in consultants)
                    {
                        double cost = 0;
                        if (c.Status == "Consultant" || c.Status == "Salarié")
                        {
                            if (c.DailyCost == 0)
                            {
                                cost = c.MonthlyCost * 1.5 + c.MealCost + c.ExceptionalCost + c.TravelPackage;
                                cost = cost * 12 / 218;
                            }
                            else
                            {
                                cost = c.DailyCost * 1.5 + c.MealCost + c.ExceptionalCost + c.TravelPackage;
                            }
                        }
                        if (c.Status == "Sous-Traitant")
                        {
                            if (c.DailyCost == 0)
                            {
                                cost = c.MonthlyCost + c.MealCost + c.ExceptionalCost + c.TravelPackage;
                                cost = cost * 12 / 218;
                            }
                            else
                            {
                                cost = c.DailyCost + c.MealCost + c.ExceptionalCost + c.TravelPackage;
                            }
                        }

                        DetailConsultantViewModel m = new DetailConsultantViewModel()
                        {
                            ID = c.ConsultantID,
                            FirstName = c.FirstName,
                            LastName = c.LastName,
                            Email = c.Email,
                            EntryDate = c.EntryDate,
                            Status = c.Status,
                            DailyCost = cost,
                            //MonthlyCost = consultant.MonthlyCost,
                            ProfitCenter = db.profitCenters.Single(p => p.ProfitCenterID == c.ProfitCenterID).Name,
                            Subsidiary = db.Subsidiaries.Single(s => s.SubsidiaryID == c.SubsidiaryID).Name,
                        };

                        models.Add(m);
                    }

                    string message = "Ce consultant ne peut être modifié car il n'est pas dans votre profitcenter !";
                    ModelState.AddModelError(string.Empty, message);
                    return View("ConsultantList", models);
                }
            }


            UpdateConsultantViewModel model = new UpdateConsultantViewModel()
            {
                ID = consultant.ConsultantID,
                FirstName = consultant.FirstName,
                LastName = consultant.LastName,
                Email = consultant.Email,
                EntryDate = consultant.EntryDate,
                MealCost = consultant.MealCost,
                TravelPackage = consultant.TravelPackage,
                ExceptionalCost = consultant.ExceptionalCost,
                SubsidiaryName = consultant.Subsidiary.Name,
                ProfitCenterName = consultant.ProfitCenter.Name,
                Status = consultant.Status,
      
            };

            if (consultant.DailyCost == 0)
            {
                model.Cost = consultant.MonthlyCost;
                ViewData["CostType"] = "MonthlyCost";
            }
            else
            {
                ViewData["CostType"] = "DailyCost";
                model.Cost = consultant.DailyCost;
            }

            var subsidiaries = db.Subsidiaries.ToList();
            List<string> subsidiariesNames = new List<string>();

            foreach (var sub in subsidiaries)
            {
                subsidiariesNames.Add(sub.Name);
            }
            model.Subsidiaries = subsidiariesNames;


            var profitCenters = db.profitCenters.ToList();
            List<string> profitCentersNames = new List<string>();

            foreach (var profit in profitCenters)
            {
                profitCentersNames.Add(profit.Name);
            }
            model.ProfitCenters = profitCentersNames;



            return View(model);
        }


        // POST: /Admin/EditConsultant
        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager,Administrateur-ventes")]
        [HttpPost]
        [Route("Admin/EditConsultant")]
        public ActionResult EditConsultant(UpdateConsultantViewModel model)
        {
            string SubsidiaryName;
            string ProfitCenterName;
            ProfitCenter newProfitCenter;
            Subsidiary newSubsidiary;
            DateTime? nullDateTime = null;
            ApplicationDbContext db = new ApplicationDbContext();
            string selectedTypeCost;

            Consultant consultant = db.Consultants.SingleOrDefault(m => m.ConsultantID == model.ID);

            Boolean change = consultant.Email != Request.Form["Email"];

            if (!ModelState.IsValid)
            {
                selectedTypeCost = Request.Form["CostType"].ToString();
                SubsidiaryName = Request.Form["Subsidiary"].ToString();
                ProfitCenterName = Request.Form["ProfitCenter"].ToString();
                var profitCenters = db.profitCenters.ToList();
                var subsidiaries = db.Subsidiaries.ToList();
                List<string> profitCenterNames = new List<string>();
                List<string> subsidiariesNames = new List<string>();

                foreach (var profit in profitCenters)
                {
                    profitCenterNames.Add(profit.Name);
                }
                model.ProfitCenters = profitCenterNames;

                foreach (var sub in subsidiaries)
                {
                    subsidiariesNames.Add(sub.Name);
                }
                model.Subsidiaries = subsidiariesNames;


                ViewData["Subsidiary"] = SubsidiaryName;
                ViewData["ProfitCenter"] = ProfitCenterName;
                ViewData["CostType"] = selectedTypeCost;

                return View(model);
            }

            if (change)
            {
                selectedTypeCost = Request.Form["CostType"].ToString();
                SubsidiaryName = Request.Form["Subsidiary"].ToString();
                ProfitCenterName = Request.Form["ProfitCenter"].ToString();
                var profitCenters = db.profitCenters.ToList();
                var subsidiaries = db.Subsidiaries.ToList();
                List<string> profitCenterNames = new List<string>();
                List<string> subsidiariesNames = new List<string>();

                foreach (var profit in profitCenters)
                {
                    profitCenterNames.Add(profit.Name);
                }
                model.ProfitCenters = profitCenterNames;

                foreach (var sub in subsidiaries)
                {
                    subsidiariesNames.Add(sub.Name);
                }
                model.Subsidiaries = subsidiariesNames;


                ViewData["Subsidiary"] = SubsidiaryName;
                ViewData["ProfitCenter"] = ProfitCenterName;
                ViewData["CostType"] = selectedTypeCost;

                model.Email = consultant.Email;
                string message = "On peut pas modifier email d'un consultant";
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }



            selectedTypeCost = Request.Form["CostType"].ToString();
            SubsidiaryName = Request.Form["Subsidiary"].ToString();
            ProfitCenterName = Request.Form["ProfitCenter"].ToString();
            newProfitCenter = db.profitCenters.Single(s => s.Name == ProfitCenterName);
            newSubsidiary = db.Subsidiaries.Single(s => s.Name == SubsidiaryName);

            consultant.FirstName = model.FirstName;
            consultant.LastName = model.LastName;
            consultant.Email = model.Email;
            consultant.EntryDate = model.EntryDate;
            consultant.DateOfDeparture = nullDateTime;
            consultant.Status = Request.Form["Status"].ToString();
            consultant.MealCost = model.MealCost;
            consultant.TravelPackage = model.TravelPackage;
            consultant.ExceptionalCost = model.ExceptionalCost;
            consultant.Subsidiary = newSubsidiary;

            if (selectedTypeCost == "MonthlyCost")
            {
                consultant.MonthlyCost = model.Cost;
                consultant.DailyCost = 0;
            }
            else
            {
                consultant.DailyCost = model.Cost;
                consultant.MonthlyCost = 0;
            }

            var currentProfitCenter = consultant.ProfitCenter;
            currentProfitCenter.Consultants.Remove(consultant);
            newProfitCenter.Consultants.Add(consultant);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                selectedTypeCost = Request.Form["CostType"].ToString();
                SubsidiaryName = Request.Form["Subsidiary"].ToString();
                ProfitCenterName = Request.Form["ProfitCenter"].ToString();
                var profitCenters = db.profitCenters.ToList();
                var subsidiaries = db.Subsidiaries.ToList();
                List<string> profitCenterNames = new List<string>();
                List<string> subsidiariesNames = new List<string>();

                foreach (var profit in profitCenters)
                {
                    profitCenterNames.Add(profit.Name);
                }
                model.ProfitCenters = profitCenterNames;

                foreach (var sub in subsidiaries)
                {
                    subsidiariesNames.Add(sub.Name);
                }
                model.Subsidiaries = subsidiariesNames;

                ViewData["Subsidiary"] = SubsidiaryName;
                ViewData["ProfitCenter"] = ProfitCenterName;
                ViewData["CostType"] = selectedTypeCost;


                string message = "Un consultant ayant la même adresse email existe déjà";
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }


            return RedirectToAction("ConsultantList", "admin");
        }


        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager,Administrateur-ventes")]
        public ActionResult DeleteConsultant(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var consultants = db.Consultants.ToList();
            List<DetailConsultantViewModel> models = new List<DetailConsultantViewModel>();
            var consultantToDelete = consultants.SingleOrDefault(c => c.ConsultantID == id);
            
            if (User.IsInRole("Manager"))
            {
                var email = User.Identity.Name;
                var user = db.Users.Single(u => u.Email == email);
                string name = user.FirstName + " " + user.LastName;
                List<int> pIdlist = new List<int>();
                foreach (var item in db.profitCenters.ToList())
                {
                    var manager = db.Managers.Single(m => m.ManagerID == item.Owner);
                    if (manager.FirstName + " " + manager.LastName == name)
                    {
                        pIdlist.Add(item.ProfitCenterID);
                    }
                }
                var pc = db.profitCenters.Single(p => p.ProfitCenterID == consultantToDelete.ProfitCenterID);
                Boolean ability = false;
                if (pIdlist.Count!=0)
                {
                    foreach( int profitId in pIdlist)
                    {
                        if (consultantToDelete.ProfitCenterID == profitId)
                        {
                            ability = true;
                        }
                        
                        if (profitId == pc.FatherProfitCenterID)
                        {
                            ability = true;
                        }
                        
                    }
                }
                var partowner = db.Managers.Single(m => m.ManagerID == pc.PartOwner);
                if (name == partowner.FirstName + " " + partowner.LastName)
                {
                    ability = true;
                }

                if (!ability)
                {
                    foreach (var consultant in consultants)
                    {
                        var subsidiary = db.Subsidiaries.Single(s => s.SubsidiaryID == consultant.SubsidiaryID);

                        DetailConsultantViewModel model = new DetailConsultantViewModel()
                        {
                            ID = consultant.ConsultantID,
                            FirstName = consultant.FirstName,
                            LastName = consultant.LastName,
                            Email = consultant.Email,
                            EntryDate = consultant.EntryDate,
                            Status = consultant.Status,
                            DailyCost = consultant.DailyCost,
                            MonthlyCost = consultant.MonthlyCost,
                            ProfitCenter = db.profitCenters.Single(p => p.ProfitCenterID == consultant.ProfitCenterID).Name,
                            Subsidiary = db.Subsidiaries.Single(s => s.SubsidiaryID == consultant.SubsidiaryID).Name,
                        };
                        models.Add(model);
                    }

                    string message = "Ce consultant ne peut être supprimé car il n'est pas dans votre profitcenter !";
                    ModelState.AddModelError(string.Empty, message);
                    return View("ConsultantList", models);
                }
            }

            try
            {
                db.Entry(consultantToDelete).State = EntityState.Deleted;
                db.SaveChanges();
                consultants.Remove(consultantToDelete);
            }
            catch (Exception)
            {
                foreach (var consultant in consultants)
                {
                    var subsidiary = db.Subsidiaries.Single(s => s.SubsidiaryID == consultant.SubsidiaryID);

                    DetailConsultantViewModel model = new DetailConsultantViewModel()
                    {
                        ID = consultant.ConsultantID,
                        FirstName = consultant.FirstName,
                        LastName = consultant.LastName,
                        Email = consultant.Email,
                        EntryDate = consultant.EntryDate,
                        Status = consultant.Status,
                        DailyCost = consultant.DailyCost,
                        MonthlyCost = consultant.MonthlyCost,
                        ProfitCenter = db.profitCenters.Single(p => p.ProfitCenterID == consultant.ProfitCenterID).Name,
                        Subsidiary = db.Subsidiaries.Single(s => s.SubsidiaryID == consultant.SubsidiaryID).Name,
                    };
                    models.Add(model);
                }

                string message = "Ce consultant ne peut être supprimé car il a déjà exercé une activité au sein de B2Digit !";
                ModelState.AddModelError(string.Empty, message);
                return View("ConsultantList", models);
            }

            foreach (var consultant in consultants)
            {

                DetailConsultantViewModel model = new DetailConsultantViewModel()
                {
                    ID = consultant.ConsultantID,
                    FirstName = consultant.FirstName,
                    LastName = consultant.LastName,
                    Email = consultant.Email,
                    EntryDate = consultant.EntryDate,
                    Status = consultant.Status,
                    DailyCost = consultant.DailyCost,
                    MonthlyCost = consultant.MonthlyCost,
                    ProfitCenter = db.profitCenters.Single(p => p.ProfitCenterID == consultant.ProfitCenterID).Name,
                    Subsidiary = db.Subsidiaries.Single(s => s.SubsidiaryID == consultant.SubsidiaryID).Name,
                };
                models.Add(model);
            }

            return RedirectToAction("ConsultantList", "admin");
        }


        // GET: Admin/Message
        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        [Route("Admin/Message", Name = "Message")]
        public ActionResult Message()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MessageViewModel model = new MessageViewModel();

            var MessageAccueil = db.Messages.Single() ;
            model.Message = MessageAccueil.message.ToString();

            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        [HttpPost]
        public ActionResult Message(MessageViewModel model)
        { 

            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        [HttpPost]
        public ActionResult activerMessage(MessageViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();

           var activerMessage = db.Messages.SingleOrDefault();

           activerMessage.message = activerMessage.message.ToString();

            activerMessage.message = model.Message;

            db.SaveChanges();

            return View("messageSuccess", model);
            
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        // GET: Admin/ClotureMois
        [Route("Admin/ClotureMois", Name = "ClotureMois")]
        public ActionResult ClotureMois()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ClotureViewModel model = new ClotureViewModel();

            var Mois = db.MonthActivations.Single();
            model.MoisaCloturer = Mois.Periode.ToString("MMMM", CultureInfo.CurrentCulture) +
                              " " + Mois.Periode.Year.ToString();

            
            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        public ActionResult CloturerMois1()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var missionlist = db.Missions.ToList();
            var activitylist = db.Activities.ToList();
            var fl = db.Factures.ToList();
            int id = 0;
            foreach (var item in fl)
            {
                if (item.FactureID>id)
                {
                    id = item.FactureID;
                }
            }
            id++;
            var time = db.MonthActivations.Single().Periode;
            int month = Int32.Parse(time.Month.ToString());
            int year = Int32.Parse(time.Year.ToString());
            foreach (var m in missionlist)
            {

                

                if (Int32.Parse(m.End.Year.ToString())<year || (Int32.Parse(m.End.Year.ToString()) == year&& Int32.Parse(m.End.Month.ToString()) < month))
                {
                    continue;
                }
                float nb = 0;
                foreach(var a in activitylist)
                {
                    if (a.verrouille == 3)
                    {
                        continue;
                    }
                    if(month==Int32.Parse(a.Date.Month.ToString())&& year == Int32.Parse(a.Date.Year.ToString()))
                    {
                        
                        if (a.Morning == m.Name)
                        {
                            if (a.verrouille==0)
                            {
                                nb += 0.5f;
                                a.verrouille = 1;
                            }
                            if (a.verrouille==1)
                            {
                            }
                            if (a.verrouille==2)
                            {
                                nb += 0.5f;
                                a.verrouille = 3;
                            }
                       
                        }
                        if (a.Afternoon == m.Name)
                        {
                            if (a.verrouille == 0)
                            {
                                nb += 0.5f;
                                a.verrouille = 2;
                            }
                            if (a.verrouille == 2)
                            {
                            }
                            if (a.verrouille == 1)
                            {
                                nb += 0.5f;
                                a.verrouille = 3;
                            }
                        }
                    }
                    
                }

                if (nb == 0 )
                {
                    var flist = db.Factures.Where(f=>f.MoisDeFacturation == time).ToList();
                    Boolean exist = false;
                    foreach(var f in flist)
                    {
                        if (f.mission == m.Name)
                        {
                            exist = true;
                        }
                    }
                    if (exist == true)
                    {
                        continue;
                    } else
                    {
                        //todo
                        continue;
                    }
                    
                }

                var BC = db.Subsidiaries.Single(s => s.SubsidiaryID == m.PrincipalBCID);
                var contact = db.CompanyContacts.Single(c => c.CompanyContactID == m.CompanyContactID);
                var company = db.Companies.Single(c => c.CompanyID == contact.CompanyID);

                float montant = 0f;
                float nbUO = 0f;
                switch (m.Periodicity)
                {
                    case "Jours":
                        nbUO = nb;
                        montant = nbUO * m.Fee;
                        break;
                    case "Mois":
                        nbUO = 1;
                        montant = nbUO * m.Fee;
                        break;
                    case "Trimestre":
                        nbUO = 1.0f / 3;
                        montant = nbUO * m.Fee;
                        break;
                    default:
                        break;
                }

                /*
                Boolean exist = false;
                var factureId = 0;
                foreach(var f in db.Factures.ToList())
                {
                    if(f.mission==m.Name&&f.MoisDeFacturation==db.MonthActivations.Single().Periode)
                    {
                        exist = true;
                        factureId = f.FactureID;
                    }
                }
                */

                /*
                if (exist)
                {
                    Facture facture = db.Factures.Single(f => f.FactureID == factureId);

                    facture.InfoFacturation = m.InfoFacturation;
                    facture.PrincipalBC = BC.Name;
                    facture.AdresseBC = BC.Address;
                    facture.Client = company.Name;
                    facture.AdresseFacturation = company.Address;
                    facture.NombredUO = nbUO;
                    facture.TJ = m.Fee;
                    facture.TVA = 0.2f;
                        facture.MontantHT = montant;
                    facture.DernierEnregistrer = DateTime.Now;
                   

                    db.SaveChanges();
                }
                else
                {
                */
                    Facture facture = new Facture()
                    {
                        mission = m.Name,
                        FactureID = id,
                        NomFacture = "Fact-" + BC.Name + "-" + time.ToString("yyyy-MMM", System.Globalization.CultureInfo.CurrentCulture) + "-" + id,
                        MoisDeFacturation = time,
                        InfoFacturation = m.InfoFacturation,
                        PrincipalBC = BC.Name,
                        AdresseBC = BC.Address,
                        Client = company.Name,
                        AdresseFacturation = company.Address,
                        NombredUO = nbUO,
                        TJ = m.Fee,
                        TVA = (float)db.Infos.Single().TVA,
                        MontantHT = montant,
                        FAE = false,
                        Emise = false,
                        payee = false,
                        annulee = false,
                        DernierEnregistrer=DateTime.Now,
                        Delai = m.Delai,
                        DesignationFacturation = m.DesignationFacturation,
                        type = "Facture",
                        DateRegelement = DateTime.Now,
                    };

                    id++;

                if (m.InterBC1ID != 0)
                {
                    var c = db.Subsidiaries.Single(s => s.SubsidiaryID == m.InterBC1ID);
                    Facture factureInt = new Facture()
                    {
                        mission = m.Name,
                        FactureID = id,
                        NomFacture = "Fact-Int-" + BC.Name + "-" + time.ToString("yyyy-MMM", System.Globalization.CultureInfo.CurrentCulture) + "-" + id,
                        MoisDeFacturation = time,
                        InfoFacturation = m.InfoFacturation,
                        PrincipalBC = c.Name,
                        AdresseBC = c.Address,
                        Client = BC.Name,
                        AdresseFacturation =  BC.Address,
                        NombredUO = nbUO,
                        TJ = m.TJInterBC1,
                        TVA = (float)db.Infos.Single().TVA,
                        MontantHT = montant,
                        FAE = true,
                        Emise = false,
                        payee = false,
                        annulee = false,
                        DernierEnregistrer = DateTime.Now,
                        Delai = m.Delai,
                        DesignationFacturation = m.DesignationFacturation,
                        type = "Facture",
                        DateRegelement = DateTime.Now,

                    };
                    db.Factures.Add(factureInt);
                    id++;
                }


                db.Factures.Add(facture);
                    db.SaveChanges();
                //}

                
            }

            

            
            /*
            ClotureViewModel model = new ClotureViewModel();

            var Mois = db.MonthActivations.Single();
            model.MoisaCloturer = Mois.Periode.ToString("MMMM", CultureInfo.CurrentCulture) +
                              " " + Mois.Periode.Year.ToString();

            ModelState.AddModelError(string.Empty, "Mois a été clôturé");
            */
            return RedirectToAction("ListeFactures", "Facture");
        }

        [HttpPost]
        public ActionResult MoisClot(ClotureViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();

           // var activerMessage = db.Messages.SingleOrDefault();

            //activerMessage.message = activerMessage.message.ToString();

            //activerMessage.message = model.Message;

            //db.SaveChanges();

            return View("messageSuccess", model);

        }


        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        // GET: Admin/InfoGeneral
        [Route("Admin/InfoGeneral", Name = "InfoGeneral")]
        public ActionResult InfoGeneral()
        {
            return View();
        }



        // GET: /Admin/EditInfo
        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        [Route("Admin/EditInfo", Name = "EditInfo")]
        public ActionResult EditInfo()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var Infolist = db.Infos.ToList();

            InfoViewModel model = new InfoViewModel();

            if (Infolist.Count==0)
            {
                model.TVA = 0;
                model.mention = "Rien";
            }
            else
            {
                model.TVA = Infolist[0].TVA*100;
                model.mention = Infolist[0].Mention;
            }
            return View(model);
        }


        // POST: /Admin/EditConsultant
        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        [HttpPost]
        [Route("Admin/EditInfo")]
        public ActionResult EditInfo(InfoViewModel model)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            var Infolist = db.Infos.ToList();
            if (Infolist.Count == 0)
            {
                Info info = new Info()
                {
                    TVA = model.TVA/100,
                    Mention = model.mention,
                };
                db.Infos.Add(info);
                db.SaveChanges();
            }
            else
            {
                var info = db.Infos.Single();
                info.TVA = model.TVA/100; 
                info.Mention = model.mention;
                db.SaveChanges();
            }

            return RedirectToAction("InfoGeneral", "Admin");
        }
    }



}



