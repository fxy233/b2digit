using Projet_pilate.Entities;
using Projet_pilate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projet_pilate.Controllers
{
    public class MissionController : Controller
    {

        //GET: /Mission/CreationMission
        [Route("Mission/CreationMission", Name = "CreationMission")]
        public ActionResult CreationMission()
        {
            ApplicationDbContext db = new ApplicationDbContext();


            List<Consultant> consultants = db.Consultants.ToList();

            List<string> consultantsNames = new List<string>();

            foreach (var consultant in consultants)
            {
                consultantsNames.Add(consultant.FirstName + " " + consultant.LastName);
            }

            RegisterMissionViewModel model = new RegisterMissionViewModel
            {
                ContactMail = db.CompanyContacts.Select(c => c.Mail).ToList(),
                ConsultantNames = consultantsNames,
                Start = DateTime.Today,
                End = DateTime.Today,
            };

            ViewData["periodicite"] = "jours";

            return View(model);
        }


        // POST: /Mission/CreationMission
        [HttpPost]
        [Route("Mission/CreationMission")]
        public ActionResult CreationMission(RegisterMissionViewModel model)
        {
            string nomContact;
            string SelectedConsultant;
            string periodicite;
            string nomConsultant;
            ApplicationDbContext db = new ApplicationDbContext();

            //Averti l'utilisateur en cas d'erreur de saisie
            if (!ModelState.IsValid)
            {
                nomContact = Request.Form["ContactId"].ToString();
                SelectedConsultant = Request.Form["ConsultantId"].ToString();
                periodicite = Request.Form["PeriodeId"].ToString();

                List<Consultant> consultants = db.Consultants.ToList();
                List<string> consultantsNames = new List<string>();
                foreach (var consult in consultants)
                {
                    consultantsNames.Add(consult.FirstName + " " + consult.LastName);
                }

                model.ContactMail = db.CompanyContacts.Select(c => c.Mail).ToList();
                model.ConsultantNames = consultantsNames;

                ViewData["contact"] = nomContact;
                ViewData["consultant"] = SelectedConsultant;
                ViewData["periodicite"] = periodicite;

                return View(model);
            }


            nomContact = Request.Form["ContactId"].ToString();
            SelectedConsultant = Request.Form["ConsultantId"].ToString();
            var tabPrenomNom = SelectedConsultant.Split(' ');
            nomConsultant = tabPrenomNom[0];
            periodicite = Request.Form["PeriodeId"].ToString();

            List<Consultant> employees = new List<Consultant>();

            Consultant consultant = db.Consultants.SingleOrDefault(u => u.FirstName == nomConsultant);
            ApplicationUser sessionUser = db.Users.SingleOrDefault(b => b.UserName == User.Identity.Name);

            List<Mission> missionsConsutant = consultant.Missions.ToList();

            //Gère conflit date d'entrée salarié et dates de mission du consultant
                if (consultant.EntryDate > model.Start)
                {

                nomContact = Request.Form["ContactId"].ToString();
                SelectedConsultant = Request.Form["ConsultantId"].ToString();
                periodicite = Request.Form["PeriodeId"].ToString();

                List<Consultant> consultants = db.Consultants.ToList();
                List<string> consultantsNames = new List<string>();
                foreach (var consult in consultants)
                {
                    consultantsNames.Add(consult.FirstName + " " + consult.LastName);
                }

                model.ContactMail = db.CompanyContacts.Select(c => c.Mail).ToList();
                model.ConsultantNames = consultantsNames;

                ViewData["contact"] = nomContact;
                ViewData["consultant"] = SelectedConsultant;
                ViewData["periodicite"] = periodicite;

       
                string message = "Vérifier que la période de la mission ne prècède pas la date d'entrée du consultant !";
                ModelState.AddModelError(string.Empty, message);

                return View(model);

                }


            List<Mission> missionEncours = new List<Mission>();

            foreach (var item in missionsConsutant)
            {

                if ((model.Start > item.Start && model.End < item.End) ||
                   (model.Start < item.Start && model.End > item.End) ||
                   (model.Start < item.Start && model.End > item.Start) ||
                   (model.Start < item.End && model.End > item.End) ||
                   (model.Start == item.Start && model.End < item.End) ||
                   (model.Start < item.End && model.End == item.End))
                {
                    missionEncours.Add(item);
                }
            }

            model.MissionEncours = missionEncours;

            if (model.MissionEncours.Count > 0)
            {
                nomContact = Request.Form["ContactId"].ToString();
                SelectedConsultant = Request.Form["ConsultantId"].ToString();
                periodicite = Request.Form["PeriodeId"].ToString();

                List<Consultant> consultants = db.Consultants.ToList();
                List<string> consultantsNames = new List<string>();
                foreach (var consult in consultants)
                {
                    consultantsNames.Add(consult.FirstName + " " + consult.LastName);
                }

                model.ContactMail = db.CompanyContacts.Select(c => c.Mail).ToList();
                model.ConsultantNames = consultantsNames;

                ViewData["contact"] = nomContact;
                ViewData["consultant"] = SelectedConsultant;
                ViewData["periodicite"] = periodicite;

                return View(model);
            }

            employees.Add(consultant);

            Mission mission = new Mission()
            {
                Name = model.Name,
                Fee = model.Fee,
                FreeDay = model.FreeDay,
                Comment = model.Comment,
                Start = model.Start,
                End = model.End,
                Periodicity = periodicite,
                CompanyContact = db.CompanyContacts.SingleOrDefault(c => c.Mail == nomContact),
                ProfitCenter = consultant.ProfitCenter,
                Creator = sessionUser.FirstName + " " + sessionUser.LastName,
            };

            consultant.Missions.Add(mission);


            db.Missions.Add(mission);
            db.SaveChanges();


            return RedirectToAction("ListeMissions", "Mission");

        }


        [Route("Mission/ListeMissions", Name = "ListeMissions")]
        public ActionResult ListeMissions()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<DetailsMissionViewModel> models = new List<DetailsMissionViewModel>();

            var missions = db.Missions.ToList();

            foreach (var mission in missions)
            {
                DetailsMissionViewModel model = new DetailsMissionViewModel()
                {
                    Id = mission.MissionID,
                    Name = mission.Name,
                    ContactEmail = mission.CompanyContact.Mail,
                    ClientName = mission.CompanyContact.FirstName + " " + mission.CompanyContact.LastName,
                    Start = mission.Start,
                    End = mission.End,
                    Fee = mission.Fee,
                    FreeDay = mission.FreeDay,
                    Periodicity = mission.Periodicity,
                    Comment = mission.Comment,
                    ConsultantName = mission.Consultant.FirstName + " " + mission.Consultant.LastName,
                    CreatorName = mission.Creator,
                    ProfitCenter = mission.ProfitCenter.Name,
                };

                models.Add(model);
            }

            return View(models);
        }


        // GET: /Mission/CreationReussi
        public ActionResult CreationReussi()
        {
            return View();
        }



        [AllowAnonymous]
        public ActionResult Edit(int id)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            var mission = db.Missions.SingleOrDefault(m => m.MissionID == id);

            UpdateMissionViewModel model = new UpdateMissionViewModel()
            {
                ID = mission.MissionID,
                Start = mission.Start,
                End = mission.End,

            };

            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(UpdateMissionViewModel model)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            var mission = db.Missions.SingleOrDefault(m => m.MissionID == model.ID);

            if (ModelState.IsValid)
            {

                mission.End = model.NewEnd;
                db.SaveChanges();

                return RedirectToAction("ListeMissions", "Mission");
            }

            return View(model);
        }


        public ActionResult ValidationMissions()
        {
            string name = Request.Form["name"].ToString();
            string client = Request.Form["client"].ToString();
            string nomConsultant = Request.Form["consultant"].ToString().Split(' ')[0];
            string start = Request.Form["start"].ToString();
            string end = Request.Form["end"].ToString();
            string fee = Request.Form["fee"].ToString();
            string periodicity = Request.Form["periodicity"].ToString();
            string freeday = Request.Form["freeday"].ToString();
            string comment = Request.Form["comment"].ToString();

            ApplicationDbContext db = new ApplicationDbContext();
            List<Consultant> employees = new List<Consultant>();

            Consultant consultant = db.Consultants.SingleOrDefault(u => u.FirstName == nomConsultant);
            ApplicationUser sessionUser = db.Users.SingleOrDefault(b => b.UserName == User.Identity.Name);

            employees.Add(consultant);

            Mission mission = new Mission()
            {
                Name = name,
                Fee = float.Parse(fee),
                FreeDay = int.Parse(freeday),
                Comment = comment,
                Start = DateTime.Parse(start),
                End = DateTime.Parse(end),
                Periodicity = periodicity,
                CompanyContact = db.CompanyContacts.SingleOrDefault(c => c.Mail == client),
                ProfitCenter = consultant.ProfitCenter,
                Creator = sessionUser.FirstName + " " + sessionUser.LastName,
     
            };

            consultant.Missions.Add(mission);

            db.Missions.Add(mission);

            //!!Créer un viewModel pour cette exception !!!!!!!
            db.SaveChanges();

            
            return RedirectToAction("ListeMissions", "Mission");
        }
    }
}

