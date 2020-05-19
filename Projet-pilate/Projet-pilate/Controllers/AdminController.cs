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
    public class AdminController : Controller
    {
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

        [HttpPost]
        public ActionResult ActivationCRA(AdminViewModel model)
        {

            return View(model);
        }


        [HttpPost]
        public ActionResult ActiverCra(AdminViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var activerMois = db.MonthActivations.SingleOrDefault();

            activerMois.Periode = activerMois.Periode.AddMonths(1);

            db.SaveChanges();

            return View("ActivationSuccess", model);
        }



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
                Subsidiary = manager.Subsidiary.Name

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




        [AllowAnonymous]
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

     



        //// GET: /Admin/ConsultantList
        [Route("Admin/ConsultantList", Name = "ConsultantList")]
        public ActionResult ConsultantList()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<DetailConsultantViewModel> models = new List<DetailConsultantViewModel>();

            var consultants = db.Consultants.ToList();

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
                };

                models.Add(model);
            }

            return View(models);
        }


        // GET: /Admin/CreateManager
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
        [Route("Admin/EditConsultant", Name = "EditConsultant")]
        public ActionResult EditConsultant(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var consultant = db.Consultants.Single(m => m.ConsultantID == id);


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

            if (consultant.Status == "Sous-Traitant")
            {
                model.Cost = consultant.DailyCost;
            }
            else
            {
                model.Cost = consultant.MonthlyCost;
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


        [AllowAnonymous]
        public ActionResult DeleteConsultant(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var consultants = db.Consultants.ToList();
            List<DetailConsultantViewModel> models = new List<DetailConsultantViewModel>();
            var consultantToDelete = consultants.SingleOrDefault(c => c.ConsultantID == id);

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
                };
                models.Add(model);
            }

            return RedirectToAction("ConsultantList", "admin");
        }


        public ActionResult Facturation()
        {
          

            return View();
        }

    }



}



