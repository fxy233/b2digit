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
        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager")]
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

            ViewData["periodicite"] = "Jours";

            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager")]
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

            var missionexist = db.Missions.ToList();
            foreach(var exist in missionexist)
            {
                if (exist.Name == model.Name)
                {
                    string message = "le nom du mission a été utilisé !";
                    ModelState.AddModelError(string.Empty, message);

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
            }
            

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


            nomContact = Request.Form["contactId"].ToString();
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
                //exist = true,
                PrincipalBCID = db.Subsidiaries.ToList()[0].SubsidiaryID,

            };

            var consultantList = db.Consultants.ToList();
            
            foreach(var c in consultantList)
            {
                if (c.FirstName+" "+c.LastName == SelectedConsultant)
                {
                    mission.ConsultantID = c.ConsultantID;
                }
            }

            var con = mission.ConsultantID==0?null:db.Consultants.Single(c => c.ConsultantID == mission.ConsultantID);
            int interid =con==null?0:db.Subsidiaries.Single(s => s.SubsidiaryID == con.SubsidiaryID).SubsidiaryID;
            mission.InterBC1ID = interid;

            mission.TJInterBC1 =interid==0? 0:0.07f * mission.Fee;

            db.Missions.Add(mission);
            db.SaveChanges();


            return RedirectToAction("ListeMissions", "Mission");

        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes,Manager")]
        [Route("Mission/ListeMissions", Name = "ListeMissions")]
        public ActionResult ListeMissions()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<DetailsMissionViewModel> models = new List<DetailsMissionViewModel>();

            var missions = db.Missions.ToList();

            foreach (var mission in missions)
            {
                if (!mission.inexist)
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
            }

            return View(models);
        }


        // GET: /Mission/CreationReussi
        public ActionResult CreationReussi()
        {
            return View();
        }



        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        public ActionResult Edit(int id)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            var mission = db.Missions.SingleOrDefault(m => m.MissionID == id);
            ViewBag.date = mission.End.ToString("yyyy-MM-dd");
            UpdateMissionViewModel model = new UpdateMissionViewModel()
            {
                ID = mission.MissionID,
                Start = mission.Start,
                NewEnd = mission.End,
                Name = mission.Name,
                Fee = mission.Fee,
                Commentaire = mission.Comment,
                InfoFacturation = mission.InfoFacturation,
                TJInterBC1 = mission.TJInterBC1,
                TJInterBC2 = mission.TJInterBC2,
            };

            ViewData["PrincipalBCID"] = mission.PrincipalBCID;
            ViewData["interBC1ID"] = mission.InterBC1ID;
            ViewData["interBC2ID"] = mission.InterBC2ID;


            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        [HttpPost]
        public ActionResult Edit(UpdateMissionViewModel model)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            var mission = db.Missions.SingleOrDefault(m => m.MissionID == model.ID);

            var missionexist = db.Missions.ToList();
            foreach (var exist in missionexist)
            {
                if (exist.Name == mission.Name)
                {
                    continue;
                }
                else
                {
                    if (exist.Name == model.Name)
                    {
                        string message = "Le nom de la mission doit être unique !";
                        ModelState.AddModelError(string.Empty, message);
                        ViewBag.date = mission.End.ToString("yyyy-MM-dd");
                        return View(model);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                if (mission.Name != model.Name)
                {
                    var activities = db.Activities.ToList();
                    foreach(var act in activities)
                    {
                        if(act.Morning==mission.Name)
                        {
                            act.Morning = model.Name;
                        }
                        if (act.Afternoon == mission.Name)
                        {
                            act.Afternoon = model.Name;
                        }
                        db.SaveChanges();
                    }
                }

                mission.End = model.NewEnd;
                mission.Name = model.Name;
                mission.Fee = model.Fee;
                mission.Comment = model.Commentaire;
                mission.InfoFacturation = model.InfoFacturation;
                mission.PrincipalBCID = Int32.Parse(Request.Form["PrincipalBCID"]);
                mission.InterBC1ID = Int32.Parse(Request.Form["InterBC1ID"]);
                mission.TJInterBC1 = model.TJInterBC1;
                mission.InterBC2ID = Int32.Parse(Request.Form["InterBC2ID"]);
                mission.TJInterBC2 = model.TJInterBC2;

                db.SaveChanges();

                return RedirectToAction("ListeMissions", "Mission");
            }

            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager")]
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

        [Authorize(Roles = "Administrateur, Super-Administrateur")]
        [Route("Mission/Delete", Name = "Delete")]
        public ActionResult Delete(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var missionDelete = db.Missions.Single(m => m.MissionID == id);
            var monthCurrent = Int32.Parse(DateTime.Now.Month.ToString());
            var yearCurrent = Int32.Parse(DateTime.Now.Year.ToString());
            var monthTermine = Int32.Parse(missionDelete.End.Month.ToString());
            var yearTermine = Int32.Parse(missionDelete.End.Year.ToString());

            if (yearCurrent > yearTermine || (yearCurrent == yearTermine && monthCurrent-1 > monthTermine))
            {
                //db.Missions.Remove(missionDelete);
                //missionDelete.exist = false;



                missionDelete.inexist = true;
                db.SaveChanges();
                return RedirectToAction("ListeMissions", "Mission");
            }else
            {
                
                var actlist = db.Activities.ToList();
                Boolean recent = false;
                foreach(var act in actlist)
                {
                    if (act.Morning==missionDelete.Name||act.Afternoon==missionDelete.Name)
                    {
                        if ((yearCurrent == Int32.Parse(act.Date.Year.ToString()) && monthCurrent-1 <= Int32.Parse(act.Date.Month.ToString())))
                        {
                            recent = true;
                            
                        }
                    }
                }

                if(!recent)
                {
                    missionDelete.inexist = true;
                    db.SaveChanges();
                    return RedirectToAction("ListeMissions", "Mission");
                }

                
                
                
                    List<DetailsMissionViewModel> models = new List<DetailsMissionViewModel>();

                    var missions = db.Missions.ToList();

                    foreach (var mission in missions)
                    {
                        if (!mission.inexist)
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
                    }
                    string message = "Vous pouvez pas supprimé une tâche non terminée !";
                    ModelState.AddModelError(string.Empty, message);
                    return View("ListeMissions", models);
                
            }
            

        }
    }
}

