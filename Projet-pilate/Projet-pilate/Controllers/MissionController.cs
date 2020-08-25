using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using iTextSharp.tool.xml;
using Projet_pilate.Entities;
using Projet_pilate.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
                AdresseMission = "",
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
            string nomContact="";
            string SelectedConsultant;
            string periodicite;
            string nomConsultant;
            string[] array;
            string[] array2;
            ApplicationDbContext db = new ApplicationDbContext();
            var list = db.CompanyContacts.ToList();
            var missionexist = db.Missions.ToList();
            foreach(var exist in missionexist)
            {
                if (exist.Name == model.Name)
                {
                    string message = "le nom du mission a été utilisé !";
                    ModelState.AddModelError(string.Empty, message);

                    array = Request.Form["ContactId"].ToString().Split('(');
                    array2 = array[0].Split(' ');
                    
                    foreach(var item in list)
                    {
                        if (item.FirstName == array2[0] && item.LastName == array2[1] && item.CompanyName == array[1].Split(')')[0]) 
                        {
                            nomContact = item.Mail;
                            
                        }
                    }
      
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
                array = Request.Form["ContactId"].ToString().Split('(');
                array2 = array[0].Split(' ');
                foreach (var item in list)
                {
                    if (item.FirstName == array2[0] && item.LastName == array2[1] && item.CompanyName == array[1].Split(')')[0])
                    {
                        nomContact = item.Mail;
                       
                    }
                }
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


            array = Request.Form["ContactId"].ToString().Split('(');
            array2 = array[0].Split(' ');
            foreach (var item in list)
            {
                if (item.FirstName == array2[0] && item.LastName == array2[1] && item.CompanyName == array[1].Split(')')[0])
                {
                    nomContact = item.Mail;
                    
                }
            }
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

                array = Request.Form["ContactId"].ToString().Split('(');
                array2 = array[0].Split(' ');
                foreach (var item in list)
                {
                    if (item.FirstName == array2[0] && item.LastName == array2[1] && item.CompanyName == array[1].Split(')')[0])
                    {
                        nomContact = item.Mail;
                        
                    }
                }
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

            /*
            if (model.MissionEncours.Count > 0)
            {
                array = Request.Form["ContactId"].ToString().Split('(');
                array2 = array[0].Split(' ');
                foreach (var item in list)
                {
                    if (item.FirstName == array2[0] && item.LastName == array2[1] && item.CompanyName == array[1].Split(')')[0])
                    {
                        nomContact = item.Mail;
                        
                    }
                }
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
            */

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
                AdresseMission = model.AdresseMission,
                PrincipalBCID = db.Subsidiaries.ToList()[0].SubsidiaryID,
                DesignationFacturation = model.Name,
                Delai = "30 jours",
                DateFinOdm = model.Start,
            };

            if (User.IsInRole("Manager"))
            {
                var user = db.Users.Single(u => u.UserName == User.Identity.Name);
                foreach(var m in db.Managers.ToList())
                {
                    if (m.FirstName.ToUpper() == user.FirstName.ToUpper() && m.LastName.ToUpper() == user.LastName.ToUpper())
                    {
                        foreach(var p in db.profitCenters.ToList())
                        {
                            if (p.Owner == m.ManagerID)
                            {
                                mission.ProfitCenter = p;
                                mission.ProfitCenterID = p.ProfitCenterID;
                                break;
                            }
                        }
                        foreach (var p in db.profitCenters.ToList())
                        {
                            if (p.PartOwner == m.ManagerID)
                            {
                                mission.ProfitCenter = p;
                                mission.ProfitCenterID = p.ProfitCenterID;
                                break;
                            }
                        }
                        break;
                    }
                }
            }

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
            mission.InterBC1ID = interid==6?0:interid;

            mission.TJInterBC1 = mission.InterBC1ID == 0? 0:0.93f * mission.Fee;

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
                        ClientName = mission.CompanyContact.Company.Name,
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
                AdresseMission = mission.AdresseMission,
                InfoFacturation = mission.InfoFacturation,
                Reference = mission.Reference,
                TJInterBC1 = mission.TJInterBC1,
                TJInterBC2 = mission.TJInterBC2,
                DelaiPaiement = mission.Delai,
                DesignationFacturation = mission.DesignationFacturation,
                ProfitCentre = db.profitCenters.Single(p=>p.ProfitCenterID==mission.ProfitCenterID).Name,
            };

            model.PrincipalBCID1 = mission.PrincipalBCID==0?"Aucun": db.Subsidiaries.Single(s=>s.SubsidiaryID==mission.PrincipalBCID).Name;
            model.InterBC1ID1 = mission.InterBC1ID == 0 ? "Aucun" : db.Subsidiaries.Single(s => s.SubsidiaryID == mission.InterBC1ID).Name;
            model.InterBC2ID1 = mission.InterBC2ID == 0 ? "Aucun" : db.Subsidiaries.Single(s => s.SubsidiaryID == mission.InterBC2ID).Name;


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

            if (model.PrincipalBCID1== model.InterBC1ID1)
            {
                string message = "Business Company émettrice ne doit pas être le même que Business Company Intermédiaire !";
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            if (model.PrincipalBCID1 == model.InterBC2ID1 || model.InterBC1ID1 == model.InterBC2ID1)
            {
                string message = "Business Company émettrice ne doit pas être le même que Business Company Intermédiaire !";
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            /*            if (ModelState.IsValid)
                        {*/
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
                mission.AdresseMission = model.AdresseMission;
                mission.InfoFacturation = model.InfoFacturation;
                mission.Reference = model.Reference;
                string nom = model.PrincipalBCID1;
                int pid = db.Subsidiaries.Single(s => s.Name == nom).SubsidiaryID;
                mission.PrincipalBCID = pid;
                string nomI = model.InterBC1ID1;
                int Iid = nomI=="Aucun"? 0 : db.Subsidiaries.Single(s => s.Name == nomI).SubsidiaryID;
                mission.InterBC1ID = Iid;
                mission.TJInterBC1 = model.TJInterBC1;
                string nomI2 = model.InterBC2ID1;
                int I2id = nomI2 == "Aucun" ? 0 : db.Subsidiaries.Single(s => s.Name == nomI2).SubsidiaryID;
                mission.InterBC2ID = I2id;
                mission.TJInterBC2 = model.TJInterBC2;
                mission.DesignationFacturation = model.DesignationFacturation;
                mission.Delai = model.DelaiPaiement ;

                foreach(var pc in db.profitCenters.ToList())
            {
                if (pc.Name == model.ProfitCentre)
                {
                    mission.ProfitCenterID = pc.ProfitCenterID;
                }
            }

                db.SaveChanges();

                return RedirectToAction("ListeMissions", "Mission");
            //}

            //return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager, Administrateur-ventes")]
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

        [Authorize(Roles = "Administrateur, Super-Administrateur, Administrateur-ventes")]
        [Route("Mission/Delete", Name = "Delete")]
        public ActionResult Delete(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var missionDelete = db.Missions.Single(m => m.MissionID == id);
            var monthCurrent = Int32.Parse(DateTime.Now.Month.ToString());
            var yearCurrent = Int32.Parse(DateTime.Now.Year.ToString());
            //var monthTermine = Int32.Parse(missionDelete.End.Month.ToString());
            //var yearTermine = Int32.Parse(missionDelete.End.Year.ToString());
            /*
            if (yearCurrent > yearTermine || (yearCurrent == yearTermine && monthCurrent-1 > monthTermine))
            {
                //db.Missions.Remove(missionDelete);
                //missionDelete.exist = false;



                missionDelete.inexist = true;
                db.SaveChanges();
                return RedirectToAction("ListeMissions", "Mission");
            }else
            {*/
                
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

                if (DateTime.Compare(missionDelete.End.AddMonths(1), DateTime.Now) >= 0)
            {
                recent = true;
            }

                if(!recent)
                {
                    missionDelete.inexist = true;
                    db.Missions.Remove(missionDelete);
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
                
            //}
            

        }



        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes,Consultant")]
        //GET: /Mission/DetailOdm
        [Route("Mission/DetailOdm")]
        public ActionResult DetailOdm(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var odm = db.OrdreDeMissions.Single(o => o.OrdreDeMissionID == id);

            CreationOdmModel model = new CreationOdmModel()
            {
                id = id,
                Manager = odm.manager,
                debut = odm.dateDebut,
                fin = odm.dataFin,
                nomConsultant = odm.nomConsultant,
                prenomConsultant = odm.prenomConsultant,
                nomClient = odm.nomClient,
                Mission = odm.Mission,
                contactClient = odm.contactClient,
                Adresse = odm.missionAdresse,
                Environnement = odm.environnement,
                fraisAlloue = odm.fraisAlloue,
                signature = odm.signature==null?"":odm.signature,
            };

            

            return View(model);
        }

/*        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        //GET: /Mission/DetailOdm
        [HttpPost]
        [Route("Mission/CreationOdm")]
        public ActionResult DetailOdm(CreationOdmModel model)
        {
            return View(model);
        }*/


        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        //GET: /Mission/ModifierOdm
        [Route("Mission/ModifierOdm")]
        public ActionResult ModifierOdm(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var odm = db.OrdreDeMissions.Single(o => o.OrdreDeMissionID == id);


            CreationOdmModel model = new CreationOdmModel()
            {
                id = id,
                Manager = odm.manager,
                debut = odm.dateDebut,
                fin = odm.dataFin,
                nomConsultant = odm.nomConsultant,
                prenomConsultant = odm.prenomConsultant,
                nomClient = odm.nomClient,
                Mission = odm.Mission,
                contactClient = odm.contactClient,
                Adresse = odm.missionAdresse,
                Environnement = odm.environnement,
                fraisAlloue = odm.fraisAlloue,
                signature = odm.signature == null ? "" : odm.signature,
            };
            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        //GET: /Mission/ModifierOdm
        [HttpPost]
        [Route("Mission/ModifierOdm")]
        public ActionResult ModifierOdm(CreationOdmModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var odm = db.OrdreDeMissions.Single(o => o.OrdreDeMissionID == model.id);
            odm.manager = model.Manager;
            odm.dateDebut = model.debut;
            odm.dataFin = model.fin;
            odm.nomConsultant = model.nomConsultant;
            odm.prenomConsultant = model.prenomConsultant;
            odm.nomClient = model.nomClient;
            odm.Mission = model.Mission;
            odm.contactClient = model.contactClient;
            odm.missionAdresse = model.Adresse;
            odm.environnement = model.Environnement;
            odm.fraisAlloue = Request.Form["frais"];
            model.fraisAlloue = odm.fraisAlloue;

            db.SaveChanges();
            return View("DetailOdm",model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        public ActionResult CreerODM(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var M = db.Missions.Single(m=>m.MissionID==id);
            var consultant = db.Consultants.Single(c=>c.ConsultantID== M.ConsultantID);
            

            if (consultant.Status== "Sous-Traitant")
            {
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
                            ClientName = mission.CompanyContact.Company.Name,
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

                string message = "On ne doit pas pouvoir créer un ordre de mission à partir d'une mission d'un sous-traitant !";
                ModelState.AddModelError(string.Empty, message);

                return View("ListeMissions",models);
            }
            else
            {

                var clientContact = db.CompanyContacts.Single(cc => cc.CompanyContactID == M.CompanyContactID);

                OrdreDeMission odm = new OrdreDeMission()
                {
                    ValidationConsultant = false,
                    ValidationDeAdv = false,
                    fraisAlloue = "Pass Navigo, ticket restaurant",
                    Mission = M.Name,
                    manager = M.Creator,
                    dateDebut = M.DateFinOdm,
                    dataFin = M.End,
                    nomConsultant = consultant.LastName,
                    prenomConsultant = consultant.FirstName,
                    nomClient = clientContact.CompanyName,
                    contactClient = clientContact.FirstName+" "+clientContact.LastName,
                    missionAdresse = M.AdresseMission,
                    signature="",
                    environnement = "Site client: absence d'environnement et de travaux dangereux",
                };
                M.avoirOdm = true;

                db.OrdreDeMissions.Add(odm);
                db.SaveChanges();

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
                            ClientName = mission.CompanyContact.Company.Name,
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

                string message = "La creation d'un ordre de mission a réussi !";
                ModelState.AddModelError(string.Empty, message);

                return View("ListeMissions", models);
            }

           
        }


        [Route("Mission/ListeOdm", Name = "ListeOdm")]
        public ActionResult ListeOdm()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var OdmListe = db.OrdreDeMissions.ToList();
            List<ListeOdmModel> models = new List<ListeOdmModel>();

            if (User.IsInRole("Consultant"))
            {
                var user = db.Users.Single(u => u.UserName == User.Identity.Name);
                foreach (var odm in OdmListe)
                {
                    if (user.FirstName.ToUpper()!=odm.prenomConsultant.ToUpper()||user.LastName.ToUpper()!=odm.nomConsultant.ToUpper())
                    {
                        continue;
                    }
                    ListeOdmModel model = new ListeOdmModel()
                    {
                        id = odm.OrdreDeMissionID,
                        NomMission = odm.Mission,
                        nomClient = odm.nomClient,
                        nomConsultant = odm.prenomConsultant + " " + odm.nomConsultant,
                        Status = odm.ValidationConsultant == false ? "En attente de la signature" : "Signé par le consultant",
                        debut = odm.dateDebut.ToString("yyyy-MM"),
                    };
                    models.Add(model);
                }
            }
            else
            {
                foreach (var odm in OdmListe)
                {
                    ListeOdmModel model = new ListeOdmModel()
                    {
                        id = odm.OrdreDeMissionID,
                        NomMission = odm.Mission,
                        nomClient = odm.nomClient,
                        nomConsultant = odm.prenomConsultant + " " + odm.nomConsultant,
                        Status = odm.ValidationConsultant == false ? "En attente de la signature" : "Signé par le consultant",
                        debut = odm.dateDebut.ToString("yyyy-MM"),
                    };
                    models.Add(model);
                }
            }

            
            return View(models);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes,Consultant")]
        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string GridHtml)
        {
            var id = Int32.Parse(Request.Form["id"]);
            ApplicationDbContext db = new ApplicationDbContext();
            var odm = db.OrdreDeMissions.Single(o => o.OrdreDeMissionID == id);

            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                string arialuniTff = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");

                /*int id = Int32.Parse(Request.Form["id"]);
                ApplicationDbContext db = new ApplicationDbContext();
                var facture = db.Factures.Single(f => f.FactureID == id);
                */
                string imageURL = Server.MapPath(".") + "/../Images/B2DIGIT_Facture.png";
                /*
                if (facture.PrincipalBC == "DMO Conseil")
                {
                    string[] strs = facture.PrincipalBC.Split(' ');
                    if (strs.Length > 1)
                    {
                        int i = 0;
                        foreach (var s in strs)
                        {
                            if (i != 0)
                            {
                                strs[0] = strs[0] + "_" + s;
                            }
                            i++;
                        }
                    }

                    imageURL = Server.MapPath(".") + "/../Images/" + strs[0] + "_Facture.png";
                }
                */
                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);

                //Resize image depend upon your need

                jpg.ScaleToFit(140f, 120f);

                //Give space before image

                jpg.SpacingBefore = 14f;

                //Give some space after the image

                jpg.SpacingAfter = 1f;

                jpg.Alignment = Element.ALIGN_LEFT;


                pdfDoc.Add(jpg);

                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();


                return File(stream.ToArray(), "application/pdf", "ODM-"+odm.Mission+"-"+odm.dataFin.ToString("MM-yyyy")+".pdf");
            }

        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        [Route("Mission/CreationOdm")]
        public ActionResult CreationOdm()
        {
            CreationOdmModel model = new CreationOdmModel();
            model.debut = DateTime.Now;
            model.fin = DateTime.Now;

            model.fraisAlloue = "Pass Navigo ticket restaurant";
            model.Environnement = "Site client: absence d'environnement et de travaux dangereux";

            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        //GET: /Mission/CreationOdm
        [HttpPost]
        [Route("Mission/CreationOdm")]
        public ActionResult CreationOdm(CreationOdmModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            Boolean exist = false;
            foreach(var c in db.Consultants.ToList())
            {
                if (c.FirstName==model.prenomConsultant&&c.LastName==model.nomConsultant)
                {
                    exist = true;
                    break;
                }
            }
            if(!exist)
            {
                CreationOdmModel model3 = new CreationOdmModel();
                model3.debut = DateTime.Now;
                model3.fin = DateTime.Now;

                model3.fraisAlloue = "Pass Navigo ticket restaurant";
                model3.Environnement = "Site client: absence d'environnement et de travaux dangereux";

                string message = "Ce consultant n'existe pas !";
                ModelState.AddModelError(string.Empty, message);

                return View(model3);
            }

            if (DateTime.Compare(model.debut,model.fin)>=0)
            {
                CreationOdmModel model3 = new CreationOdmModel();
                model3.debut = DateTime.Now;
                model3.fin = DateTime.Now;

                model3.fraisAlloue = "Pass Navigo ticket restaurant";
                model3.Environnement = "Site client: absence d'environnement et de travaux dangereux";

                string message = "Le date du fin doit être après le date du debut !";
                ModelState.AddModelError(string.Empty, message);

                return View(model3);
            }

            OrdreDeMission odm = new OrdreDeMission()
            {
                ValidationConsultant = false,
                ValidationDeAdv = false,
                fraisAlloue = Request.Form["frais"],
                Mission = Request.Form["Mission"],
                manager = Request.Form["Manager"],
                dateDebut = model.debut,
                dataFin = model.fin,
                nomConsultant = Request.Form["nomConsultant"],
                prenomConsultant = Request.Form["prenomConsultant"],
                nomClient = Request.Form["nomClient"],
                contactClient = Request.Form["contactClient"],
                missionAdresse = Request.Form["Adresse"],
                signature = "",
                environnement =model.Environnement,
            };

            db.OrdreDeMissions.Add(odm);
            db.SaveChanges();

            CreationOdmModel model2 = new CreationOdmModel()
            {
                id = odm.OrdreDeMissionID,
                Manager = odm.manager,
                debut = odm.dateDebut,
                fin = odm.dataFin,
                nomConsultant = odm.nomConsultant,
                prenomConsultant = odm.prenomConsultant,
                nomClient = odm.nomClient,
                Mission = odm.Mission,
                contactClient = odm.contactClient,
                Adresse = odm.missionAdresse,
                Environnement = odm.environnement,
                fraisAlloue = odm.fraisAlloue,
                signature = odm.signature == null ? "" : odm.signature,
            };
            return View("DetailOdm", model2);
        }

        [Authorize(Roles = "Consultant")]
        [Route("Mission/Signer")]
        public ActionResult Signer (int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var odm = db.OrdreDeMissions.Single(o => o.OrdreDeMissionID == id);
            odm.ValidationConsultant = true;
            odm.signature = "Lu et approuvé le "+DateTime.Now.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture)+",  " + odm.prenomConsultant+" "+odm.nomConsultant;

            var mission = db.Missions.Single(m => m.Name == odm.Mission);
            mission.avoirOdm = false;
            mission.DateFinOdm = mission.End;

            db.SaveChanges();

            return RedirectToAction("ListeOdm");
        }
    }
}

