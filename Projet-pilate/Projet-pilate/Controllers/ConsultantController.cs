using Microsoft.AspNet.Identity;
using Projet_pilate.Entities;
using Projet_pilate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using System.Data;
using DocumentFormat.OpenXml.Office.CustomUI;
using iTextSharp.text.pdf.qrcode;
using System.Web.Hosting;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.SS.Formula.Functions;
using Projet_pilate.Migrations;

namespace Projet_pilate.Controllers
{

    public class ConsultantController : Controller
    {
        //// GET: /Consultant/CRA
        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes,Manager,Consultant")]
        [Route("Consultant/CRA", Name = "CRA")]
        public ActionResult CRA()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            RegisterActivityViewModel model = new RegisterActivityViewModel();

            var currentMonth = db.MonthActivations.Single();
            ViewBag.date = currentMonth.Periode;
            int month = currentMonth.Periode.Month;
            int year = currentMonth.Periode.Year;
            int lastDayOfMonth = System.DateTime.DaysInMonth(year, month);
            
            //

            DateTime endMonth = new DateTime(year, month, lastDayOfMonth);
            DateTime startMonth = currentMonth.Periode;

            //


            int dayNumber = System.DateTime.DaysInMonth(year, month) + 1;
         
            string id = User.Identity.Name;

            ApplicationUser user = db.Users.Single(u => u.UserName == id);
            ViewData["moisActuel"] = currentMonth.Periode.ToString("MMMM");
            ViewData["nom"] = user.FirstName + " " + user.LastName;

       
            List<Mission> missionsTotal = db.Missions.Where(m => m.Consultant.Email == id).ToList();


            /*
            List<Mission> missionsEnCours = missionsTotal.Where(m =>

            m.Start < startMonth && m.End > endMonth||
            m.Start < startMonth && m.End < endMonth||
            m.Start == startMonth && m.End < endMonth||
            m.Start > startMonth && m.End < endMonth ||
            m.Start > startMonth && m.End == endMonth ||
            m.Start > startMonth && m.End > endMonth).ToList();
            */
            List<Mission> missionsEnCours = missionsTotal.Where(m => m.Start < endMonth && m.End > startMonth).ToList();
            List<string> nomMissions = new List<string>();


            Cra cra = new Cra();

            model.MoisActivation = currentMonth.Periode.ToString("MMMM");


            ViewData["moisActif"] = model.MoisActivation;

            foreach (var item in missionsEnCours)
            {
                nomMissions.Add(item.Name);
            }

            model.MissionsList = nomMissions;

            model.Dates = new List<DateTime>();
            for (int i = 1; i < dayNumber; i++)
            {
                DateTime date = new DateTime(year, month, i);

                model.Dates.Add(date);
            }

            string mois = currentMonth.Periode.ToString("MMMM");
            string annee = currentMonth.Periode.Year.ToString();

            cra = db.Cras.SingleOrDefault(c => c.Consultant.Email == User.Identity.Name
                                                       && c.Month == mois && c.year == annee);


            if (cra != null && cra.Changeable == true)
            {


                DetailActivityViewModel detailModel = new DetailActivityViewModel();

                var activities = cra.Activities.ToList();

                detailModel.Cra = cra;
                detailModel.Activities = activities;

                var matins = cra.Activities.Select(m => m.Morning).ToList();
                var apresMidi = cra.Activities.Select(m => m.Afternoon).ToList();

                detailModel.NbJoursOuvres = matins.Count;

                foreach (var item in matins)
                {
                    switch (item)
                    {
                        case "IC":
                            detailModel.NbIc += 0.5;
                            break;
                        case "Formation":
                            detailModel.NbFormation += 0.5;
                            break;
                        case "Maladie":
                            detailModel.NbMaladie += 0.5;
                            break;
                        case "Congés":
                            detailModel.NbConges += 0.5;
                            break;
                        default:
                            detailModel.NbMission += 0.5;
                            break;
                    }
                }

                foreach (var item in apresMidi)
                {
                    switch (item)
                    {
                        case "IC":
                            detailModel.NbIc += 0.5;
                            break;
                        case "Formation":
                            detailModel.NbFormation += 0.5;
                            break;
                        case "Maladie":
                            detailModel.NbMaladie += 0.5;
                            break;
                        case "Congés":
                            detailModel.NbConges += 0.5;
                            break;
                        default:
                            detailModel.NbMission += 0.5;
                            break;
                    }
                }

                detailModel.MoisActivation = currentMonth.Periode.ToString("MMMM");


                return View("CRAReadOnly", detailModel);

            }


            if (model.MissionsList.Count == 0)
            {
                return View("CraSansProjet", model);
            }


            return View(model);

        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes,Manager,Consultant")]
        [HttpPost]
        [Route("Consultant/CRA")]
        public ActionResult CRA(RegisterActivityViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var dCra = db.Cras.Where(c => c.Consultant.Email == User.Identity.Name).ToList();
            List<Activity> activities = new List<Activity>();

            var currentMonth = db.MonthActivations.Single();
            int month = currentMonth.Periode.Month;
            string yearStr = currentMonth.Periode.Year.ToString();
            int yearInt = currentMonth.Periode.Year;


            string am = Request["matin"].ToString();
            string pm = Request["apres-midi"].ToString();


            string[] matinees = am.Split(',');
            string[] apresMidis = pm.Split(',');


            //
            Dictionary<string,double> missionlist = new Dictionary<string, double>();
            //


            if (matinees.Count() == apresMidis.Count())
            {
                for (int i = 0; i < matinees.Length; i++)
                {
                    Activity activity = new Activity();
                    string numeroJour = matinees[i].Substring(matinees[i].IndexOf('-')).Replace('-', ' ');
                    int jour = Int32.Parse(numeroJour);
                    activity.Date = new DateTime(yearInt, month, jour);
                    activity.Morning = matinees[i].Substring(0, matinees[i].IndexOf('-'));
                    activity.Afternoon = apresMidis[i].Substring(0, apresMidis[i].IndexOf('-'));
                    activities.Add(activity);

                    //
                    if (missionlist.ContainsKey(activity.Morning))
                    {
                        missionlist[activity.Morning] += 0.5;
                    }
                    else
                    {
                        missionlist.Add(activity.Morning,0.5);
                    }

                    if (missionlist.ContainsKey(activity.Afternoon))
                    {
                        missionlist[activity.Afternoon] += 0.5;
                    }
                    else
                    {
                        missionlist.Add(activity.Afternoon, 0.5);
                    }
                    //
                }
            }

            if (matinees.Count() > apresMidis.Count())
            {
                for (int i = 0; i < matinees.Length; i++)
                {
                    Activity activity = new Activity();
                    string numeroJour = matinees[i].Substring(matinees[i].IndexOf('-')).Replace('-', ' ');
                    int jour = Int32.Parse(numeroJour);
                    activity.Date = new DateTime(yearInt, month, jour);
                    activity.Morning = matinees[i].Substring(0, matinees[i].IndexOf('-'));

                    //
                    if (missionlist.ContainsKey(activity.Morning))
                    {
                        missionlist[activity.Morning] += 0.5;
                    }
                    else
                    {
                        missionlist.Add(activity.Morning, 0.5);
                    }
                    //

                    for (int j = 0; j < apresMidis.Length; j++)
                    {
                        if (matinees[i] == apresMidis[j])
                        {
                            activity.Afternoon = apresMidis[j].Substring(0, apresMidis[j].IndexOf('-'));
                            //

                            if (missionlist.ContainsKey(activity.Afternoon))
                            {
                                missionlist[activity.Afternoon] += 0.5;
                            }
                            else
                            {
                                missionlist.Add(activity.Afternoon, 0.5);
                            }
                            //
                        }
                    }

                    activities.Add(activity);
                }

            }

            if (matinees.Count() < apresMidis.Count())
            {
                for (int i = 0; i < apresMidis.Length; i++)
                {
                    Activity activity = new Activity();
                    string numeroJour = apresMidis[i].Substring(apresMidis[i].IndexOf('-')).Replace('-', ' ');
                    int jour = Int32.Parse(numeroJour);
                    activity.Date = new DateTime(yearInt, month, jour);
                    activity.Afternoon = apresMidis[i].Substring(0, apresMidis[i].IndexOf('-'));

                    //

                    if (missionlist.ContainsKey(activity.Afternoon))
                    {
                        missionlist[activity.Afternoon] += 0.5;
                    }
                    else
                    {
                        missionlist.Add(activity.Afternoon, 0.5);
                    }
                    //

                    for (int j = 0; j < matinees.Length; j++)
                    {
                        if (apresMidis[i] == matinees[j])
                        {
                            activity.Morning = matinees[j].Substring(0, matinees[j].IndexOf('-'));

                            //
                            if (missionlist.ContainsKey(activity.Morning))
                            {
                                missionlist[activity.Morning] += 0.5;
                            }
                            else
                            {
                                missionlist.Add(activity.Morning, 0.5);
                            }

                            //
                        }
                    }


                    activities.Add(activity);
                }
            }


            string moisConcerne = Request["moisConcerne"].ToString();

            Cra cra = new Cra()
            {

                Satisfaction = Request["satisfaction"].ToString(),
                Comment = Request["commentaire"].ToString(),

                Changeable = true,
                Month = moisConcerne,
                year = yearStr,

            };


            string id = User.Identity.Name;
            Consultant consultant = db.Consultants.Single(u => u.Email == id);


            double coutM = consultant.DailyCost == 0 ? consultant.MonthlyCost  : consultant.DailyCost * 218 / 12;
            ;
            /*if (consultant.Status== "Salarié")
            {
                coutM = consultant.DailyCost == 0 ? consultant.MonthlyCost * 1.5 + consultant.MealCost + consultant.ExceptionalCost + consultant.TravelPackage : (consultant.MonthlyCost * 1.5 + consultant.MealCost + consultant.ExceptionalCost + consultant.TravelPackage)*218/12 ;
            }
            else
            {
                coutM = consultant.DailyCost == 0 ? consultant.MonthlyCost + consultant.MealCost + consultant.ExceptionalCost + consultant.TravelPackage : (consultant.MonthlyCost + consultant.MealCost + consultant.ExceptionalCost + consultant.TravelPackage) * 218 / 12;
            }
*/

            cra.Consultant = consultant;
            cra.Activities = activities;

            foreach(var itemCra in dCra)
            if (itemCra != null && itemCra.Month== cra.Month && itemCra.year==cra.year)
            {
                return View("info");
            }

            db.Cras.Add(cra);

            db.SaveChanges();

            int CraID = db.Cras.Single(c => c.ConsultantID == consultant.ConsultantID && c.Month == moisConcerne && c.year == yearStr).CraID;
            //
            var time = DateTime.Parse( db.MonthActivations.Single().Periode.ToString("yyyy-MM-01") ).AddMonths(1).AddDays(-1);
            var missionNames = db.Missions.Select(m => m.Name).ToList();


            float missionTotal = 0;
            float missionHorsPC = 0;

            foreach(KeyValuePair<string, double> pair in missionlist)
            {
                if (!missionNames.Contains(pair.Key))
                {
                    continue;
                }
                
                

                var mission = db.Missions.Single(m => m.Name == pair.Key);
                var BC = db.Subsidiaries.Single(s => s.SubsidiaryID == mission.PrincipalBCID);
                var contact = db.CompanyContacts.Single(c => c.CompanyContactID == mission.CompanyContactID);
                var company = db.Companies.Single(c => c.CompanyID == contact.CompanyID);

                float montant = 0f;
                float nbUO = 0f;
                switch (mission.Periodicity)
                {
                    case "Jours":
                        nbUO = (float)pair.Value;
                        montant = nbUO * mission.Fee;
                        break;
                    case "Mois":
                        nbUO = 1;
                        montant = nbUO * mission.Fee;
                        break;
                    case "Trimestre":
                        nbUO = 1.0f / 3;
                        montant = nbUO * mission.Fee;
                        break;
                    default:
                        break;
                }

                DateTime reglement = DateTime.Now;
                switch (mission.Delai)
                {
                    case "30 jours":
                        reglement = time.AddDays(30);
                        break;
                    case "30 jours fin de mois":
                        reglement = time.AddDays(30);
                        reglement = DateTime.Parse(reglement.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
                        break;
                    case "45 jours":
                        reglement = time.AddDays(45);
                        break;
                    case "45 jours fin de mois":
                        reglement = time.AddDays(45);
                        reglement = DateTime.Parse(reglement.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
                        break;
                    case "60 jours":
                        reglement = time.AddDays(60);
                        break;
                    case "60 jours fin de mois":
                        reglement = time.AddDays(60);
                        reglement = DateTime.Parse(reglement.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
                        break;
                    default:
                        break;
                }

                int Id = 0;
                foreach (var item in db.Factures.ToList())
                {
                    if (item.FactureID > Id)
                    {
                        Id = item.FactureID;
                    }
                }
                Id++;

                Entities.Facture facture = new Entities.Facture()
                {
                    mission = mission.Name,
                    FactureID = Id,
                    NomFacture = "FAE" + Id,
                    MoisDeFacturation = time,
                    InfoFacturation = mission.InfoFacturation,
                    PrincipalBC = BC.Name,
                    AdresseBC = BC.Address,
                    Client = company.Name,
                    AdresseFacturation = company.Address,
                    NombredUO = nbUO,
                    TJ = mission.Fee,
                    TVA = db.Infos.ToList().Count == 0 ? 0.2f : (float)db.Infos.Single().TVA,
                    MontantHT = montant,
                    FAE = true,
                    Emise = false,
                    payee = false,
                    annulee = false,
                    DernierEnregistrer = DateTime.Now,
                    Delai = mission.Delai,
                    DesignationFacturation = mission.DesignationFacturation,
                    DateRegelement = reglement,
                    type = "Facture",
                    reference = mission.Reference,
                    referenceBancaire = BC.Name,
                    CraId = CraID,
                    mention = db.Infos.ToList().Count == 0 ? "" : db.Infos.Single().Mention,
                    ConsultantId = mission.ConsultantID,
                };

                db.Factures.Add(facture);


                

                if (mission.InterBC1ID != 0)
                {
                    Id++;
                    var c = db.Subsidiaries.Single(s => s.SubsidiaryID == mission.InterBC1ID);
                    Entities.Facture factureInt = new Entities.Facture()
                    {
                        mission = mission.Name,
                        FactureID = Id,
                        NomFacture = "FAE"+Id,
                        MoisDeFacturation = time,
                        InfoFacturation = mission.InfoFacturation,
                        PrincipalBC = c.Name,
                        AdresseBC = c.Address,
                        Client = BC.Name,
                        AdresseFacturation = BC.Address,
                        NombredUO = nbUO,
                        TJ = mission.TJInterBC1,
                        TVA = db.Infos.ToList().Count == 0 ? 0.2f : (float)db.Infos.Single().TVA,
                        MontantHT = nbUO*mission.TJInterBC1,
                        FAE = true,
                        Emise = false,
                        payee = false,
                        annulee = false,
                        DernierEnregistrer = DateTime.Now,
                        Delai = mission.Delai,
                        DesignationFacturation = mission.DesignationFacturation,
                        DateRegelement = reglement,
                        type = "Facture",
                        reference = mission.Reference,
                        referenceBancaire = BC.Name,
                        CraId = CraID,
                        mention = db.Infos.ToList().Count == 0 ? "" : db.Infos.Single().Mention,
                        ConsultantId = mission.ConsultantID,
                    };
                    db.Factures.Add(factureInt);
                    
                }

                if (mission.InterBC2ID != 0)
                {
                    Id++;
                    var c = db.Subsidiaries.Single(s => s.SubsidiaryID == mission.InterBC2ID);
                    var bc = db.Subsidiaries.Single(s => s.SubsidiaryID == mission.InterBC1ID);
                    Entities.Facture factureInt = new Entities.Facture()
                    {
                        mission = mission.Name,
                        FactureID = Id,
                        NomFacture = "FAE" + Id,
                        MoisDeFacturation = time,
                        InfoFacturation = mission.InfoFacturation,
                        PrincipalBC = c.Name,
                        AdresseBC = c.Address,
                        Client = bc.Name,
                        AdresseFacturation = bc.Address,
                        NombredUO = nbUO,
                        TJ = mission.TJInterBC2,
                        TVA = db.Infos.ToList().Count == 0 ? 0.2f : (float)db.Infos.Single().TVA,
                        MontantHT = nbUO * mission.TJInterBC2,
                        FAE = true,
                        Emise = false,
                        payee = false,
                        annulee = false,
                        DernierEnregistrer = DateTime.Now,
                        Delai = mission.Delai,
                        DesignationFacturation = mission.DesignationFacturation,
                        DateRegelement = reglement,
                        type = "Facture",
                        reference = mission.Reference,
                        referenceBancaire = BC.Name,
                        CraId = CraID,
                        mention = db.Infos.ToList().Count == 0 ? "" : db.Infos.Single().Mention,
                        ConsultantId = mission.ConsultantID,

                    };
                    db.Factures.Add(factureInt);
                    
                }



                //
                missionTotal += (float)pair.Value;
                var pcid = db.Consultants.Single(c => c.ConsultantID == mission.ConsultantID).ProfitCenterID;
                if(mission.ProfitCenterID != pcid)
                {
                    missionHorsPC += (float)pair.Value;
                }

                //





                db.SaveChanges();



            }


            var pc = db.profitCenters.Single(p => p.ProfitCenterID == consultant.ProfitCenterID);
            var manager = db.Managers.Single(m => m.ManagerID == pc.Owner);

            if ( missionTotal == 0)
            {
                Entities.Suivi suivi = new Entities.Suivi()
                {
                    statu = "Facture",
                    ProfitCenterID = consultant.ProfitCenterID,
                    NomMission = "IC",
                    Consultant = consultant.FirstName + " " + consultant.LastName,
                    NombredUO = 0,
                    TJ = 0,
                    mensuelConsultant = (float)coutM,
                    mensuelManager = (float)manager.MonthlyCost,
                    fraisConsultant = (float)(consultant.MealCost + consultant.TravelPackage + consultant.ExceptionalCost),
                    fraisManager = (float)(manager.MealCost + manager.TravelPackage + manager.ExceptionalCost),
                    craID = CraID,
                    date = db.MonthActivations.Single().Periode,
                    client = "Aucun",
                };
                db.Suivis.Add(suivi);
                db.SaveChanges();
            }
            else
            {
                if (missionHorsPC == 0)
                {
                    foreach (KeyValuePair<string, double> pair in missionlist)
                    {
                        if (!missionNames.Contains(pair.Key))
                        {
                            continue;
                        }
                        var mission = db.Missions.Single(m => m.Name == pair.Key);
                        float montant = 0f;
                        float nbUO = 0f;
                        switch (mission.Periodicity)
                        {
                            case "Jours":
                                nbUO = (float)pair.Value;
                                montant = nbUO * mission.Fee;
                                break;
                            case "Mois":
                                nbUO = 1;
                                montant = nbUO * mission.Fee;
                                break;
                            case "Trimestre":
                                nbUO = 1.0f / 3;
                                montant = nbUO * mission.Fee;
                                break;
                            default:
                                break;
                        }

                        Entities.Suivi suivi = new Entities.Suivi()
                        {
                            statu = "FAE",
                            ProfitCenterID = consultant.ProfitCenterID,
                            NomMission = pair.Key,
                            Consultant = consultant.FirstName + " " + consultant.LastName,
                            NombredUO = nbUO,
                            TJ = mission.Fee,
                            mensuelConsultant = (float)(coutM * pair.Value / missionTotal),
                            mensuelManager = (float)manager.MonthlyCost,
                            fraisConsultant = (float)((consultant.MealCost + consultant.TravelPackage + consultant.ExceptionalCost) * pair.Value / missionTotal),
                            fraisManager = (float)(manager.MealCost + manager.TravelPackage + manager.ExceptionalCost),
                            craID = CraID,
                            date = db.MonthActivations.Single().Periode,
                        };
                        CompanyContact cc = db.Missions.Single(m => m.Name == suivi.NomMission).CompanyContact;
                        suivi.client = cc.CompanyName;
                        db.Suivis.Add(suivi);
                        db.SaveChanges();
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, double> pair in missionlist)
                    {
                        if (!missionNames.Contains(pair.Key))
                        {
                            continue;
                        }
                        var mission = db.Missions.Single(m => m.Name == pair.Key);
                        float montant = 0f;
                        float nbUO = 0f;
                        switch (mission.Periodicity)
                        {
                            case "Jours":
                                nbUO = (float)pair.Value;
                                montant = nbUO * mission.Fee;
                                break;
                            case "Mois":
                                nbUO = 1;
                                montant = nbUO * mission.Fee;
                                break;
                            case "Trimestre":
                                nbUO = 1.0f / 3;
                                montant = nbUO * mission.Fee;
                                break;
                            default:
                                break;
                        }

                        if(mission.ProfitCenterID != consultant.ProfitCenterID)
                        {
                            Entities.Suivi suivi = new Entities.Suivi()
                            {
                                statu = "FAE",
                                ProfitCenterID = mission.ProfitCenterID,
                                NomMission = pair.Key,
                                Consultant = consultant.FirstName + " " + consultant.LastName,
                                NombredUO = nbUO,
                                TJ = mission.Fee/2,
                                mensuelConsultant = (float)(coutM*pair.Value/(2* missionTotal)),
                                mensuelManager = (float)manager.MonthlyCost,
                                fraisConsultant = (float)((consultant.MealCost + consultant.TravelPackage + consultant.ExceptionalCost) * pair.Value / (2 * missionTotal)),
                                fraisManager = (float)(manager.MealCost + manager.TravelPackage + manager.ExceptionalCost),
                                craID = CraID,
                                date = db.MonthActivations.Single().Periode,
                            };
                            CompanyContact cc = db.Missions.Single(m => m.Name == suivi.NomMission).CompanyContact;
                            suivi.client = cc.CompanyName;
                            Entities.Suivi suivi2 = new Entities.Suivi()
                            {
                                statu = "FAE",
                                ProfitCenterID = consultant.ProfitCenterID,
                                NomMission = pair.Key,
                                Consultant = consultant.FirstName + " " + consultant.LastName,
                                NombredUO = nbUO,
                                TJ = mission.Fee / 2,
                                mensuelConsultant = (float)(coutM * pair.Value / (2 * missionTotal)),
                                mensuelManager = (float)manager.MonthlyCost,
                                fraisConsultant = (float)((consultant.MealCost + consultant.TravelPackage + consultant.ExceptionalCost) * pair.Value / (2 * missionTotal)),
                                fraisManager = (float)(manager.MealCost + manager.TravelPackage + manager.ExceptionalCost),
                                craID = CraID,
                                date = db.MonthActivations.Single().Periode,
                            };
                            CompanyContact cc2 = db.Missions.Single(m => m.Name == suivi2.NomMission).CompanyContact;
                            suivi2.client = cc2.CompanyName;

                            db.Suivis.Add(suivi);
                            db.Suivis.Add(suivi2);
                            db.SaveChanges();
                        }
                        else
                        {
                            Entities.Suivi suivi = new Entities.Suivi()
                            {
                                statu = "FAE",
                                ProfitCenterID = consultant.ProfitCenterID,
                                NomMission = pair.Key,
                                Consultant = consultant.FirstName + " " + consultant.LastName,
                                NombredUO = nbUO,
                                TJ = mission.Fee,
                                mensuelConsultant = (float)(coutM*pair.Value/missionTotal),
                                mensuelManager = (float)manager.MonthlyCost,
                                fraisConsultant = (float)((consultant.MealCost + consultant.TravelPackage + consultant.ExceptionalCost)*pair.Value/missionTotal),
                                fraisManager = (float)(manager.MealCost + manager.TravelPackage + manager.ExceptionalCost),
                                craID = CraID,
                                date = db.MonthActivations.Single().Periode,
                            };
                            CompanyContact cc = db.Missions.Single(m => m.Name == suivi.NomMission).CompanyContact;
                            suivi.client = cc.CompanyName;
                            db.Suivis.Add(suivi);
                            db.SaveChanges();
                        }
                        
                    }
                }
            }



            

                //



                return RedirectToAction("CRA");

        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes,Manager")]
        [Route("Consultant/ListeCra", Name = "ListeCra")]
        public ActionResult ListeCra()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<ActivityViewModel> models = new List<ActivityViewModel>();

            List<Cra> cras = db.Cras.ToList();
            List<string> consultantCra = new List<string>();

            var currentMonth = db.MonthActivations.Single().Periode.ToString("MMMM", CultureInfo.CurrentCulture);
            ViewBag.date = db.MonthActivations.Single().Periode;

            foreach (var cra in cras)
            {
                if (User.IsInRole("Manager"))
                {

                    Consultant consultant = cra.Consultant;
                    

                    var email = User.Identity.Name;
                    var user = db.Users.Single(u => u.Email == email);
                    string name = user.FirstName + " " + user.LastName;
                    List<int> pIdlist = new List<int>();
                    foreach (var obj in db.profitCenters.ToList())
                    {
                        var manager = db.Managers.Single(m => m.ManagerID == obj.Owner);

                        if (manager.FirstName + " " + manager.LastName == name)
                        {
                            pIdlist.Add(obj.ProfitCenterID);
                        }
                        try
                        {
                            var manager2 = db.Managers.Single(m => m.ManagerID == obj.PartOwner);

                            if (manager2.FirstName + " " + manager2.LastName == name)
                            {
                                pIdlist.Add(obj.ProfitCenterID);
                            }
                        }
                        catch (Exception) { }


                    }

                    Boolean ability = false;
                    var pc = db.profitCenters.Single(pp => pp.ProfitCenterID == consultant.ProfitCenterID);
                    if (pIdlist.Count != 0)
                    {
                        foreach (var p in pIdlist)
                        {
                            if (pc.ProfitCenterID == p)
                            {
                                ability = true;
                                break;
                            }
                            if (p == pc.FatherProfitCenterID)
                            {
                                ability = true;
                                break;
                            }
                        }
                    }
                    if (!ability)
                    {
                       continue;
                    }

                }


                if (cra.Month == currentMonth)
                {
                    var nbProjetMatin = cra.Activities.Select(a => a.Morning).ToList();
                    var nbProjetApreMidi = cra.Activities.Select(a => a.Afternoon).ToList();
                    var nbDay = cra.Activities.ToArray();
                    List<string> Missions = new List<string>(); // création de la liste de mission
                    List<int> NBJT = new List<int>();
                    List<int> NBJTWE = new List<int>();
                    int noBill = 0;

                    foreach (var item in nbDay)
                    {
                        if (item.Date.DayOfWeek == DayOfWeek.Saturday || item.Date.DayOfWeek == DayOfWeek.Sunday)
                        {//on est en WE
                            if ((item.Morning != "Congés" && item.Morning != "IC" && item.Morning != "Formation" && item.Morning != "Maladie"))
                            {
                                var IndexMission = Missions.IndexOf(item.Morning);
                                if (IndexMission < 0)
                                {
                                    Missions.Add(item.Morning.ToString());
                                    NBJT.Add(0);
                                    NBJTWE.Add(1);
                                }
                                else
                                {
                                    NBJTWE[IndexMission]++;
                                }
                            }
                            if ((item.Afternoon != "Congés" && item.Afternoon != "IC" && item.Afternoon != "Formation" && item.Afternoon != "Maladie"))
                            {
                                var IndexMission = Missions.IndexOf(item.Afternoon);
                                if (IndexMission < 0)
                                {
                                    Missions.Add(item.Afternoon.ToString());
                                    NBJT.Add(0);
                                    NBJTWE.Add(1);
                                }
                                else
                                {
                                    NBJTWE[IndexMission]++;
                                }
                            }
                        }
                        else
                        { // on est en jours ouvrés
                            if ((item.Morning != "Congés" && item.Morning != "IC" && item.Morning != "Formation" && item.Morning != "Maladie"))
                            {
                                var IndexMission = Missions.IndexOf(item.Morning);
                                if (IndexMission < 0)
                                {
                                    Missions.Add(item.Morning.ToString());
                                    NBJT.Add(1);
                                    NBJTWE.Add(0);
                                }
                                else
                                {
                                    NBJT[IndexMission]+=1;
                                }
                            }

                            if ((item.Afternoon != "Congés" && item.Afternoon != "IC" && item.Afternoon != "Formation" && item.Afternoon != "Maladie"))
                            {
                                var IndexMission = Missions.IndexOf(item.Afternoon);
                                if (IndexMission < 0)
                                {
                                    Missions.Add(item.Afternoon.ToString());
                                    NBJT.Add(1);
                                    NBJTWE.Add(0);
                                }
                                else
                                {
                                    NBJT[IndexMission] += 1;
                                }
                            }

                            if ((item.Morning == "Congés" || item.Morning == "IC" || item.Morning == "Formation" || item.Morning == "Maladie"))
                            {
                                noBill++;
                            }

                            if ((item.Afternoon == "Congés" || item.Afternoon == "IC" || item.Afternoon == "Formation" || item.Afternoon == "Maladie"))
                            {
                                noBill++;
                            }

                        }
                    }


                    foreach (var mission in Missions)
                    {
                        var IndexOfMission = Missions.IndexOf(mission);
                        ActivityViewModel model = new ActivityViewModel()
                        {
                            ID = cra.CraID,
                            Date = cra.Month + " " + cra.year,
                            ConsultantName = cra.Consultant.FirstName + " " + cra.Consultant.LastName,
                            Satisfaction = cra.Satisfaction,
                            MissionName = mission,
                            WorkedDays = (float)NBJT[IndexOfMission]/2.0f,
                            NoBillDays = (float)noBill/2.0f,
                            WorkedDaysWE = (float)NBJTWE[IndexOfMission] / 2.0f,
                        };
                        consultantCra.Add(model.ConsultantName);
                        models.Add(model);
                    }

                    if (Missions.Count == 0)
                    {
                        ActivityViewModel model = new ActivityViewModel()
                        {
                            ID = cra.CraID,
                            Date = cra.Month + " " + cra.year,
                            ConsultantName = cra.Consultant.FirstName + " " + cra.Consultant.LastName,
                            Satisfaction = cra.Satisfaction,
                            MissionName = "pas de mission",
                            WorkedDays = 0,
                            NoBillDays = (float)noBill / 2.0f,
                            WorkedDaysWE = 0,
                        };
                        consultantCra.Add(model.ConsultantName);
                        models.Add(model);
                    }
                }
                
            }

            var consultantlist = db.Consultants.ToList();
            foreach (var c in consultantlist)
            {
                if (User.IsInRole("Manager"))
                {



                    var email = User.Identity.Name;
                    var user = db.Users.Single(u => u.Email == email);
                    string name = user.FirstName + " " + user.LastName;
                    List<int> pIdlist = new List<int>();
                    foreach (var obj in db.profitCenters.ToList())
                    {
                        var manager = db.Managers.Single(m => m.ManagerID == obj.Owner);

                        if (manager.FirstName + " " + manager.LastName == name)
                        {
                            pIdlist.Add(obj.ProfitCenterID);
                        }
                        try
                        {
                            var manager2 = db.Managers.Single(m => m.ManagerID == obj.PartOwner);

                            if (manager2.FirstName + " " + manager2.LastName == name)
                            {
                                pIdlist.Add(obj.ProfitCenterID);
                            }
                        }
                        catch (Exception) { }


                    }

                    Boolean ability = false;
                    var pc = db.profitCenters.Single(pp => pp.ProfitCenterID == c.ProfitCenterID);
                    if (pIdlist.Count != 0)
                    {
                        foreach (var p in pIdlist)
                        {
                            if (pc.ProfitCenterID == p)
                            {
                                ability = true;
                                break;
                            }
                            if (p == pc.FatherProfitCenterID)
                            {
                                ability = true;
                                break;
                            }
                        }
                    }
                    if (!ability)
                    {
                       continue;
                    }

                }
                if (!consultantCra.Contains(c.FirstName + " " + c.LastName))
                {
                    ActivityViewModel model = new ActivityViewModel()
                    {
                        ConsultantName = c.FirstName + " " + c.LastName,
                        MissionName = "Rien",
                    };
                    models.Add(model);
                }
            }


            
            
            return View(models);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        //[Route("Consultant/DeleteCra")]
        public ActionResult DeleteCra(int id)
        {
            
            ApplicationDbContext db = new ApplicationDbContext();
            var craD = db.Cras.Single(c => c.CraID == id);

            if (craD.Changeable)
            {
                db.Entry(craD).State = EntityState.Deleted;

                var facture = db.Factures.Where(f => f.CraId == craD.CraID).ToList();
                foreach(var f in facture)
                {
                    db.Factures.Remove(f);
                }

                var suivi = db.Suivis.Where(f => f.craID == craD.CraID).ToList();
                foreach (var s in suivi)
                {
                    db.Suivis.Remove(s);
                }


                db.SaveChanges();

                return RedirectToAction("ListeCra", "Consultant");
            } else
            {
                List<ActivityViewModel> models = new List<ActivityViewModel>();

                List<Cra> cras = db.Cras.ToList();
                List<string> consultantCra = new List<string>();

                var currentMonth = db.MonthActivations.Single().Periode.ToString("MMMM", CultureInfo.CurrentCulture);
                ViewBag.date = db.MonthActivations.Single().Periode;

                foreach (var cra in cras)
                {
                    if (cra.Month == currentMonth)
                    {
                        var nbProjetMatin = cra.Activities.Select(a => a.Morning).ToList();
                        var nbProjetApreMidi = cra.Activities.Select(a => a.Afternoon).ToList();
                        var nbDay = cra.Activities.ToArray();
                        List<string> Missions = new List<string>(); // création de la liste de mission
                        List<int> NBJT = new List<int>();
                        List<int> NBJTWE = new List<int>();
                        int noBill = 0;

                        foreach (var item in nbDay)
                        {
                            if (item.Date.DayOfWeek == DayOfWeek.Saturday || item.Date.DayOfWeek == DayOfWeek.Sunday)
                            {//on est en WE
                                if ((item.Morning != "Congés" && item.Morning != "IC" && item.Morning != "Formation" && item.Morning != "Maladie"))
                                {
                                    var IndexMission = Missions.IndexOf(item.Morning);
                                    if (IndexMission < 0)
                                    {
                                        Missions.Add(item.Morning.ToString());
                                        NBJT.Add(0);
                                        NBJTWE.Add(1);
                                    }
                                    else
                                    {
                                        NBJTWE[IndexMission]++;
                                    }
                                }
                                if ((item.Afternoon != "Congés" && item.Afternoon != "IC" && item.Afternoon != "Formation" && item.Afternoon != "Maladie"))
                                {
                                    var IndexMission = Missions.IndexOf(item.Afternoon);
                                    if (IndexMission < 0)
                                    {
                                        Missions.Add(item.Afternoon.ToString());
                                        NBJT.Add(0);
                                        NBJTWE.Add(1);
                                    }
                                    else
                                    {
                                        NBJTWE[IndexMission]++;
                                    }
                                }
                            }
                            else
                            { // on est en jours ouvrés
                                if ((item.Morning != "Congés" && item.Morning != "IC" && item.Morning != "Formation" && item.Morning != "Maladie"))
                                {
                                    var IndexMission = Missions.IndexOf(item.Morning);
                                    if (IndexMission < 0)
                                    {
                                        Missions.Add(item.Morning.ToString());
                                        NBJT.Add(1);
                                        NBJTWE.Add(0);
                                    }
                                    else
                                    {
                                        NBJT[IndexMission] += 1;
                                    }
                                }

                                if ((item.Afternoon != "Congés" && item.Afternoon != "IC" && item.Afternoon != "Formation" && item.Afternoon != "Maladie"))
                                {
                                    var IndexMission = Missions.IndexOf(item.Afternoon);
                                    if (IndexMission < 0)
                                    {
                                        Missions.Add(item.Afternoon.ToString());
                                        NBJT.Add(1);
                                        NBJTWE.Add(0);
                                    }
                                    else
                                    {
                                        NBJT[IndexMission] += 1;
                                    }
                                }

                                if ((item.Morning == "Congés" || item.Morning == "IC" || item.Morning == "Formation" || item.Morning == "Maladie"))
                                {
                                    noBill++;
                                }

                                if ((item.Afternoon == "Congés" || item.Afternoon == "IC" || item.Afternoon == "Formation" || item.Afternoon == "Maladie"))
                                {
                                    noBill++;
                                }

                            }
                        }


                        foreach (var mission in Missions)
                        {
                            var IndexOfMission = Missions.IndexOf(mission);
                            ActivityViewModel model = new ActivityViewModel()
                            {
                                ID = cra.CraID,
                                Date = cra.Month + " " + cra.year,
                                ConsultantName = cra.Consultant.FirstName + " " + cra.Consultant.LastName,
                                Satisfaction = cra.Satisfaction,
                                MissionName = mission,
                                WorkedDays = (float)NBJT[IndexOfMission] / 2.0f,
                                NoBillDays = (float)noBill / 2.0f,
                                WorkedDaysWE = (float)NBJTWE[IndexOfMission] / 2.0f,
                            };
                            consultantCra.Add(model.ConsultantName);
                            models.Add(model);
                        }
                    }

                }

                var consultantlist = db.Consultants.ToList();
                foreach (var c in consultantlist)
                {
                    if (!consultantCra.Contains(c.FirstName + " " + c.LastName))
                    {
                        ActivityViewModel model = new ActivityViewModel()
                        {
                            ConsultantName = c.FirstName + " " + c.LastName,
                            MissionName = "Rien",
                        };
                        models.Add(model);
                    }
                }
                string message = "Vous pouvez pas réinitialiser, ce facture a été émise.";
                ModelState.AddModelError(string.Empty, message);

                return View("ListeCra", models);

            }
            
        }

        [Route("Consultant/CRAReadOnly")]
        public ActionResult CRAReadOnly(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var currentMonth = db.MonthActivations.Single();
            ViewBag.date = currentMonth.Periode;

            var cra = db.Cras.Single(c => c.CraID == id);

            var consultant = db.Consultants.Single(c => c.ConsultantID == cra.ConsultantID);
            ViewBag.nom = consultant.FirstName + " " + consultant.LastName;

            DetailActivityViewModel detailModel = new DetailActivityViewModel();

            var activities = cra.Activities.ToList();

            detailModel.Cra = cra;
            detailModel.Activities = activities;

            var matins = cra.Activities.Select(m => m.Morning).ToList();
            var apresMidi = cra.Activities.Select(m => m.Afternoon).ToList();

            detailModel.NbJoursOuvres = matins.Count;

            foreach (var item in matins)
            {
                switch (item)
                {
                    case "IC":
                        detailModel.NbIc += 0.5;
                        break;
                    case "Formation":
                        detailModel.NbFormation += 0.5;
                        break;
                    case "Maladie":
                        detailModel.NbMaladie += 0.5;
                        break;
                    case "Congés":
                        detailModel.NbConges += 0.5;
                        break;
                    default:
                        detailModel.NbMission += 0.5;
                        break;
                }
            }

            foreach (var item in apresMidi)
            {
                switch (item)
                {
                    case "IC":
                        detailModel.NbIc += 0.5;
                        break;
                    case "Formation":
                        detailModel.NbFormation += 0.5;
                        break;
                    case "Maladie":
                        detailModel.NbMaladie += 0.5;
                        break;
                    case "Congés":
                        detailModel.NbConges += 0.5;
                        break;
                    default:
                        detailModel.NbMission += 0.5;
                        break;
                }
            }

            detailModel.MoisActivation = db.MonthActivations.Single().Periode.ToString("MMMM");


            return View(detailModel);
        }


        /*[Route("Consultant/SuiviCra", Name = "SuiviCra")]
        public ActionResult SuiviCra()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ConsultantCraModel> models = new List<ConsultantCraModel>();

            var consultants = db.Consultants.ToList();

            foreach (var consultant in consultants)
            {

                ConsultantCraModel model = new ConsultantCraModel()
                {
                    ID = consultant.ConsultantID,
                    Email = consultant.Email,
                    MissionsList = new List<string>(),
                    NbParMission = new Dictionary<string, double[]>(),
                    DureeMission = new Dictionary<string, int[]>(),
                };
                var missionList = db.Missions.ToList();
                foreach (var mission in missionList)
                {
                    if (mission.ConsultantID == consultant.ConsultantID)
                    {
                        model.MissionsList.Add(mission.Name);
                        double[] list = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                        model.NbParMission.Add(mission.Name, list);
                        model.DureeMission.Add(mission.Name, new int[] { Int32.Parse(mission.Start.Year.ToString()), Int32.Parse(mission.Start.Month.ToString()), Int32.Parse(mission.End.Year.ToString()), Int32.Parse(mission.End.Month.ToString()) });
                    }
                }

                var cras = db.Cras.ToList();

                foreach (var cra in cras)
                {
                    if (cra.ConsultantID == consultant.ConsultantID)
                    {
                        var activities = db.Activities.ToList();

                        foreach (var activity in activities)
                        {
                            if (activity.CraID == cra.CraID)
                            {
                                switch (activity.Morning)
                                {
                                    case "IC":
                                        break;
                                    case "Formation":
                                        ;
                                        break;
                                    case "Maladie":
                                        break;
                                    case "Congés":
                                        break;
                                    default:
                                        *//*if ( !model.NbParMission.ContainsKey(activity.Morning))
                                        {
                                            model.MissionsList.Add(activity.Morning);
                                            double[] list = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                                            model.NbParMission.Add(activity.Morning, list);
                                            var mitem = db.Missions.Single(m => m.Name == activity.Morning);
                                            model.DureeMission.Add(mitem.Name, new int[] { Int32.Parse(mitem.Start.Year.ToString()), Int32.Parse(mitem.Start.Month.ToString()), Int32.Parse(mitem.End.Year.ToString()), Int32.Parse(mitem.End.Month.ToString()) });
                                        }*//*
                                        model.NbParMission[activity.Morning][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;
                                        break;
                                }
                                switch (activity.Afternoon)
                                {
                                    case "IC":
                                        break;
                                    case "Formation":
                                        break;
                                    case "Maladie":
                                        break;
                                    case "Congés":
                                        break;
                                    default:
                                        *//*if (!model.NbParMission.ContainsKey(activity.Afternoon))
                                        {
                                            model.MissionsList.Add(activity.Afternoon);
                                            double[] list = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                                            model.NbParMission.Add(activity.Afternoon, list);
                                            var mitem = db.Missions.Single(m => m.Name == activity.Afternoon);
                                            model.DureeMission.Add(mitem.Name, new int[] { Int32.Parse(mitem.Start.Year.ToString()), Int32.Parse(mitem.Start.Month.ToString()), Int32.Parse(mitem.End.Year.ToString()), Int32.Parse(mitem.End.Month.ToString()) });
                                        }*//*
                                        model.NbParMission[activity.Afternoon][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;
                                        break;
                                }
                            }

                        }
                    }
                }


                models.Add(model);
            }

            return View(models);

        }*/



        //[Route("Consultant/SuiviCra_mission", Name = "SuiviCra_mission")]
        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes,Manager")]
        public ActionResult SuiviCra_mission()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            ViewBag.listMonth = new string[]{ "Jan", "Fév", "Mar", "Avr", "Mai", "Juin", "Juil", "Aou", "Sep", "Oct", "Nov", "Dec"};

            List<ConsultantCraModel> models = new List<ConsultantCraModel>();

            var consultants = db.Consultants.ToList();

            foreach (var consultant in consultants)
            {
                
                ConsultantCraModel model = new ConsultantCraModel()
                {
                    ID = consultant.ConsultantID,
                    Consultant = consultant.FirstName+" "+consultant.LastName,
                    MissionsList = new List<string>(),
                    NbParMission = new Dictionary<string, double[]>(),
                    DureeMission = new Dictionary<string, int[]>(),
                };
                var missionList = db.Missions.ToList();
                foreach (var mission in missionList)
                {
                    if (mission.ConsultantID == consultant.ConsultantID)
                    {
                        model.MissionsList.Add(mission.Name);
                        double[] list = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                        model.NbParMission.Add(mission.Name, list);
                        model.DureeMission.Add(mission.Name, new int[] { Int32.Parse(mission.Start.Year.ToString()), Int32.Parse(mission.Start.Month.ToString()), Int32.Parse(mission.End.Year.ToString()), Int32.Parse(mission.End.Month.ToString()) });
                    }
                }

                var cras = db.Cras.ToList();

                foreach (var cra in cras)
                {
                    if (cra.ConsultantID == consultant.ConsultantID)
                    {
                        var activities = db.Activities.ToList();

                        foreach (var activity in activities)
                        {
                            if (activity.CraID == cra.CraID)
                            {
                                switch (activity.Morning)
                                {
                                    case "IC":
                                        break;
                                    case "Formation":
                                        
                                        break;
                                    case "Maladie":
                                        break;
                                    case "Congés":
                                        break;
                                    default:
                                        if (model.MissionsList.Contains(activity.Morning))
                                        {
                                            model.NbParMission[activity.Morning][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;
                                        }
                                        
                                        break;
                                }
                                switch (activity.Afternoon)
                                {
                                    case "IC":
                                        break;
                                    case "Formation":
                                        break;
                                    case "Maladie":
                                        break;
                                    case "Congés":
                                        break;
                                    default:
                                        if (model.MissionsList.Contains(activity.Afternoon))
                                        {
                                            model.NbParMission[activity.Afternoon][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;
                                        }
                                        break;
                                }
                            }

                        }
                    }
                }


                models.Add(model);
            }

            return View(models);

        }

        [Route("Consultant/Retour", Name = "Retour")]
        public ActionResult Retour()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string id = User.Identity.Name;

            var user = db.Users.Single(u => u.Email == id);

            if (user.Position == "Consultant")
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("ListeCra", "Consultant");
        }



        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes,Consultant")]
        //problème de manager
        [Route("Consultant/Export", Name = "Export")]
        public ActionResult Export(int id)
        {

            var model = this.GetData(id);
            ViewBag.id = id;
            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes,Consultant")]
        public ActivityExportModel GetData(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var cra = db.Cras.Single(c => c.CraID == id);
            //List<ActivityExportModel> models = new List<ActivityExportModel>(); 
            ActivityExportModel model = new ActivityExportModel();

            model.id = id;
            model.periode = cra.Month + " " + cra.year;
            model.activityParClient = new Dictionary<string, Dictionary<string, double[]>>();
            model.manager = new Dictionary<string, string>();
            model.societe = "rien";
            var consultant = db.Consultants.Single(c => c.ConsultantID == cra.ConsultantID);
            model.consultant = consultant.FirstName[0] + "." + consultant.LastName;
            var activities = cra.Activities.ToList();

            foreach (var activity in activities)
            {
                model.date = activity.Date;
                int jour = Int32.Parse(activity.Date.Day.ToString());
                var morning = activity.Morning;
                var afternoon = activity.Afternoon;

                if (morning != "Formation" && morning!= "Maladie" && morning != "Congés" && morning != "IC")
                {
                    var missionM = db.Missions.Single(m => m.Name == morning);

                    if(model.societe=="rien")
                    {
                        model.societe = db.Subsidiaries.Single(s => s.SubsidiaryID == missionM.PrincipalBCID).Name;
                    }
                    var compM = db.CompanyContacts.Single(c => c.CompanyContactID == missionM.CompanyContactID);

                    if (!model.manager.ContainsKey(missionM.Name))
                    {
                        string[] words = missionM.Creator.Split(' ');
                        string name = "";
                        foreach(string item in words)
                        {
                            if (name == "")
                            {
                                name = item[0] + ".";
                            }
                            else
                            {
                                name += item + " ";
                            }
                        }
                        model.manager.Add(missionM.Name, name);
                    }

                    var dictAPC = model.activityParClient;

                    if (dictAPC.ContainsKey(compM.CompanyName))
                    {
                        var dictM = dictAPC[compM.CompanyName];
                        if (dictM.ContainsKey(missionM.Name))
                        {
                            dictM[missionM.Name][jour - 1] += 0.5;
                        }
                        else
                        {
                            var tab = new double[31];
                            dictM.Add(missionM.Name, tab);
                            dictM[missionM.Name][jour - 1] += 0.5;
                        }
                    }
                    else
                    {
                        dictAPC.Add(compM.CompanyName, new Dictionary<string, double[]>());
                        var dictM = dictAPC[compM.CompanyName];
                        var tab = new double[31];
                        dictM.Add(missionM.Name, tab);
                        dictM[missionM.Name][jour - 1] += 0.5;
                    }
                }


                if (afternoon != "Formation" && afternoon != "Maladie" && afternoon != "Congés" && afternoon != "IC")
                {
                    var missionA = db.Missions.Single(m => m.Name == afternoon);

                    var compA = db.CompanyContacts.Single(c => c.CompanyContactID == missionA.CompanyContactID);

                    if (!model.manager.ContainsKey(missionA.Name))
                    {
                        model.manager.Add(missionA.Name, missionA.Creator);
                    }

                    var dictAPC = model.activityParClient;

                    if (dictAPC.ContainsKey(compA.CompanyName))
                    {
                        var dictA = dictAPC[compA.CompanyName];
                        if (dictA.ContainsKey(missionA.Name))
                        {
                            dictA[missionA.Name][jour - 1] += 0.5;
                        }
                        else
                        {
                            var tab = new double[31];
                            dictA.Add(missionA.Name, tab);
                            dictA[missionA.Name][jour - 1] += 0.5;
                        }
                    }
                    else
                    {
                        dictAPC.Add(compA.CompanyName, new Dictionary<string, double[]>());
                        var dictA = dictAPC[compA.CompanyName];
                        var tab = new double[31];
                        dictA.Add(missionA.Name, tab);
                        dictA[missionA.Name][jour - 1] += 0.5;
                    }
                }
            }

            return model;
            /*models.Add(model);

            return models;*/
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes,Consultant")]
        [HttpPost]
        public FileResult ExportT(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            using (XLWorkbook wb = new XLWorkbook())
            {
            ActivityExportModel exp = this.GetData(id); ;

            List<int> grays = new List<int>();
            string[] Day = new string[] { "D", "L", "M", "M", "J", "V", "S" };
            int week = Convert.ToInt32(exp.date.DayOfWeek.ToString("d"));
            int jour = Int32.Parse(exp.date.Day.ToString());
            int index = (week +7-(jour - 1) % 7)%7;
            int days = DateTime.DaysInMonth(exp.date.Year, exp.date.Month);

            int x = 1;

            foreach (var item in exp.activityParClient)
            {
                DataTable dt = new DataTable(exp.consultant +"-"+ exp.periode+ "-CRA");
                dt.Columns.AddRange(new DataColumn[2] { new DataColumn("Periode"), new DataColumn(exp.periode) });
                    dt.Rows.Add("Periode", exp.periode);
                    dt.Rows.Add("Client", item.Key);
                    dt.Rows.Add("Société émettrice", "B2DIGIT");
                    dt.Rows.Add("Destinataire", item.Key);
                


                    string[] weekdays = new string[days+1];
                //DataColumn[] daterow = new DataColumn[days];
                DataColumn[] daterow2 = new DataColumn[days+1];
                daterow2[0] = new DataColumn(" ");
                weekdays[0] = "";
                int nb = item.Value.Count;
                for (int i=1;i<=days;++i)
                {
                    daterow2[i] = new DataColumn(i.ToString());
                    weekdays[i]=Day[(index+i-1)%7];
                    
                }

                    DataColumn[] daterow3 = new DataColumn[days + 1];
                    daterow3[0] = new DataColumn("Activité");
                    weekdays[0] = "Activité";
                    nb = item.Value.Count;
                    for (int i = 1; i <= days; ++i)
                    {
                        daterow3[i] = new DataColumn(i.ToString());
                        weekdays[i] = Day[(index + i - 1) % 7];
                        if (weekdays[i]=="D"|| weekdays[i] == "S")
                        {
                            grays.Add(i);
                        }

                    }

                    double total = 0;
                    string mission = "";

                    DataTable dt4 = new DataTable(exp.consultant + "-" + exp.periode + "-CRA");
                    dt4.Columns.AddRange(daterow3);
                    dt4.Rows.Add(weekdays);
                    dt4.Rows.Add(daterow3);
                    foreach (var obj in item.Value)
                    {
                        string[] output = new string[days + 1];
                        output[0] = obj.Key;
                        mission = obj.Key;
                        for (int t = 1; t <= days; ++t)
                        {
                            output[t] = obj.Value[t - 1].ToString();
                            total += obj.Value[t - 1];
                        }
                        dt4.Rows.Add(output);
                    }

                    DataTable dt5 = new DataTable(exp.consultant + "-" + exp.periode + "-CRA");
                    dt5.Columns.AddRange(new DataColumn[2] { new DataColumn("total"), new DataColumn("quantite") });
                    dt5.Rows.Add("TOTAL",total);


                    var imagepath = HostingEnvironment.MapPath("~/Images/B2DIGIT_Facture.png");
                    if (exp.societe == "DMO Conseil")
                    {
                        string[] strs = exp.societe.Split(' ');
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

                        imagepath = HostingEnvironment.MapPath("~/Images/" + strs[0] + "_Facture.png");
                    }

                    

                    var ws = wb.Worksheets.Add(x);
                    if (exp.societe == "DMO Conseil")
                    {
                        var image = ws.AddPicture(imagepath).MoveTo(ws.Cell("B3")).Scale(.1);
                    }
                    else
                    {

                        var image = ws.AddPicture(imagepath).MoveTo(ws.Cell("A2")).Scale(.2);
                    }
                   

                    //ws.AddPicture();
                    ws.Columns("B").Width = 30;
                    ws.Columns("C").Width = 30;

                    ws.Cell(8,2).InsertTable(dt);

                    ws.Rows(8, 8).Hide();
                    ws.Rows(14,14).Hide();
                    ws.Cell(14, 3).InsertTable(dt4);



                    double an = DateTime.Now.Year;
                    var G = an % 19;
                    var C = Math.Floor(an / 100);
                    var H = (C - Math.Floor(C / 4) - Math.Floor((8 * C + 13) / 25) + 19 * G + 15) % 30;
                    var I = H - Math.Floor(H / 28) * (1 - Math.Floor(H / 28) * Math.Floor(29 / (H + 1)) * Math.Floor((21 - G) / 11));
                    var J = (an * 1 + Math.Floor(an / 4) + I + 2 - C + Math.Floor(C / 4)) % 7;
                    var L = I - J;

                    var MoisPaques = 3 + Math.Floor((L + 40) / 44);
                    var JourPaques = L + 28 - 31 * Math.Floor(MoisPaques / 4);

                    /*var Ascension = new Date(an, MoisPaques - 1, JourPaques + 39).toLocaleDateString(undefined, options);
                    var Pentecote = new Date(an, MoisPaques - 1, JourPaques + 49).toLocaleDateString(undefined, options);
                    var LundiPentecote = new Date(an, MoisPaques - 1, JourPaques + 50).toLocaleDateString(undefined, options);
*/
                    int month = db.MonthActivations.Single().Periode.Month;
                    if (month==1)
                    {
                        grays.Add(1);
                    }
                    if (month == MoisPaques)
                    {
                        grays.Add((int)JourPaques);
                        grays.Add((int)JourPaques+1);
                    }
                    if (month == 5)
                    {
                        grays.Add(1);
                        grays.Add(8);
                    }
                    if (month == 7)
                    {
                        grays.Add(14);
                    }
                    if (month == 8)
                    {
                        grays.Add(15);
                    }
                    if (month == 11)
                    {
                        grays.Add(1);
                        grays.Add(11);
                    }
                    if (month == 12)
                    {
                        grays.Add(25);
                    }

                    for(int i=1; i<50; i++)
                    {
                        for (int j=1; j<50; ++j)
                        {
                            ws.Cell(i, j).Style.Fill.BackgroundColor = XLColor.White;
                        }
                    }

                    foreach (int i in grays)
                    {
                        ws.Cell(15, 3+i).Style.Fill.BackgroundColor = XLColor.LightGray;
                        ws.Cell(16, 3 + i).Style.Fill.BackgroundColor = XLColor.LightGray;
                        ws.Cell(17, 3 + i).Style.Fill.BackgroundColor = XLColor.LightGray;
                    }

                    ws.Cell(19, 3).InsertTable(dt5);
                    ws.Rows(19, 19).Hide();

                    DataTable dt6 = new DataTable(exp.consultant + "-" + exp.periode + "-CRA");
                    dt6.Columns.AddRange(new DataColumn[1] { new DataColumn("consultant") });
                    dt6.Rows.Add("Signature du consultant");
                    dt6.Rows.Add(exp.consultant);

                    DataTable dt7 = new DataTable(exp.consultant + "-" + exp.periode + "-CRA");
                    dt7.Columns.AddRange(new DataColumn[1] { new DataColumn("manager")});
                    dt7.Rows.Add("Signature du manager");
                    dt7.Rows.Add(exp.manager[mission]);

                    DataTable dt8 = new DataTable(exp.consultant + "-" + exp.periode + "-CRA");
                    dt8.Columns.AddRange(new DataColumn[1] { new DataColumn("client") });
                    dt8.Rows.Add("Signature du client");

                    var client = db.CompanyContacts.Single(c=>c.CompanyName== item.Key);
                    dt8.Rows.Add(client.FirstName[0]+"."+client.LastName);

                    ws.Cell(24, 2).InsertTable(dt6);
                    ws.Cell(24, 3).InsertTable(dt7);
                    ws.Cell(32, 2).InsertTable(dt8);
                    ws.Rows(24, 24).Hide();
                    ws.Rows(32, 32).Hide();

                    ws.Range("B9:C12").Style.Border.OutsideBorderColor = XLColor.Black; 
                    ws.Range("B9:C12").Style.Border.InsideBorderColor = XLColor.Black;
                    ws.Range("B9:C12").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    ws.Range("B9:C12").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                    if (days==31)
                    {
                        ws.Range("C15:AH17").Style.Border.OutsideBorderColor = XLColor.Black;
                        ws.Range("C15:AH17").Style.Border.InsideBorderColor = XLColor.Black;
                        ws.Range("C15:AH17").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                        ws.Range("C15:AH17").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                    }
                    if (days == 30)
                    {
                        ws.Range("C15:AG17").Style.Border.OutsideBorderColor = XLColor.Black;
                        ws.Range("C15:AG17").Style.Border.InsideBorderColor = XLColor.Black;
                        ws.Range("C15:AG17").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                        ws.Range("C15:AG17").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                    }
                    if (days == 29)
                    {
                        ws.Range("C15:AF17").Style.Border.OutsideBorderColor = XLColor.Black;
                        ws.Range("C15:AF17").Style.Border.InsideBorderColor = XLColor.Black;
                        ws.Range("C15:AF17").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                        ws.Range("C15:AF17").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                    }
                    if (days == 28)
                    {
                        ws.Range("C15:AE17").Style.Border.OutsideBorderColor = XLColor.Black;
                        ws.Range("C15:AE17").Style.Border.InsideBorderColor = XLColor.Black;
                        ws.Range("C15:AE17").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                        ws.Range("C15:AE17").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                    }
                    ws.Range("C20:D20").Style.Border.OutsideBorderColor = XLColor.Black;
                    ws.Range("C20:D20").Style.Border.InsideBorderColor = XLColor.Black;
                    ws.Range("C20:D20").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    ws.Range("C20:D20").Style.Border.InsideBorder = XLBorderStyleValues.Medium;

                    ws.Range("B25:C34").Style.Border.OutsideBorderColor = XLColor.White;
                    ws.Range("B25:C34").Style.Border.InsideBorderColor = XLColor.White;
                    ws.Range("B25:C34").Style.Border.OutsideBorder = XLBorderStyleValues.None;
                    ws.Range("B25:C34").Style.Border.InsideBorder = XLBorderStyleValues.None;

                    x++;
                }


            
                
                //wb.Worksheets.Add(dt2);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", exp.consultant + "-" + exp.periode + "-CRA" + ".xlsx");
                }
            }
        }

        /*
        public ActionResult ExportToExcel(int id)
        {
            Response.ClearHeaders();
            var gv = new GridView();
            gv.DataSource = this.GetData(id);
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("Export", this.GetData(id));
        }
        

        public ActionResult ExportToExcel(int id)
        {
            
            using (XLWorkbook wb = new XLWorkbook())
            {

                var data = this.GetData(id);
                var ws = wb.Worksheets.Add("cusdata", 1);
            
                ws.Cell(1, 1).InsertData(data[0]);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    
                    return this.File(memoryStream.ToArray(), "application/vnd.ms-excel", "Download.xlsx");
                }
            }
        }
        */
        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes,Manager")]
        [Route("Consultant/SuiviCra", Name = "SuiviCra")]
        public ActionResult SuiviCra()
        {
            return View();
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes,Manager")]
        //[Route("Consultant/test", Name = "test")]
        public ActionResult test()
        {
            ViewBag.listMonth = new string[] { " Jan ", " Fév ", " Mar ", " Avr ", " Mai ", " Juin ", " Juil ", " Aou ", " Sep ", " Oct ", " Nov ", " Dec " };
            ApplicationDbContext db = new ApplicationDbContext();
            List<ConsultantCraModelNew> models = new List<ConsultantCraModelNew>();
            ViewBag.debut = db.MonthActivations.Single().Periode;

            var consultants = db.Consultants.ToList();

            foreach (var consultant in consultants)
            {
                if (User.IsInRole("Manager"))
                {
                    var email = User.Identity.Name;
                    var user = db.Users.Single(u => u.Email == email);
                    string name = user.FirstName + " " + user.LastName;
                    List<int> pIdlist = new List<int>();
                    foreach (var obj in db.profitCenters.ToList())
                    {
                        var manager = db.Managers.Single(m => m.ManagerID == obj.Owner);

                        if (manager.FirstName + " " + manager.LastName == name)
                        {
                            pIdlist.Add(obj.ProfitCenterID);
                        }
                        try
                        {
                            var manager2 = db.Managers.Single(m => m.ManagerID == obj.PartOwner);

                            if (manager2.FirstName + " " + manager2.LastName == name)
                            {
                                pIdlist.Add(obj.ProfitCenterID);
                            }
                        }
                        catch (Exception) { }


                    }

                    Boolean ability = false;
                    var pc = db.profitCenters.Single(pp => pp.ProfitCenterID == consultant.ProfitCenterID);
                    if (pIdlist.Count != 0)
                    {
                        foreach (var p in pIdlist)
                        {
                            if (pc.ProfitCenterID == p)
                            {
                                ability = true;
                                break;
                            }
                            if (p == pc.FatherProfitCenterID)
                            {
                                ability = true;
                                break;
                            }
                        }
                    }
                    if (!ability)
                    {
                        continue;
                    }
                }
                ConsultantCraModelNew model = new ConsultantCraModelNew()
                {
                    ID = consultant.ConsultantID,
                    Consultant = consultant.FirstName+" "+consultant.LastName,
                    MissionsList = new List<string>(),
                    NbParMission = new Dictionary<string, Dictionary<string, double[]>>(),
                    DureeMission = new Dictionary<string, int[]>(),
                };
                var missionList = db.Missions.ToList();
                
                foreach (var mission in missionList)
                {
                    if (mission.ConsultantID == consultant.ConsultantID)
                    {
                        if (mission.Start < ViewBag.debut)
                        {
                            ViewBag.debut = mission.Start;
                        }

                        model.MissionsList.Add(mission.Name);
                        int end = Int32.Parse(mission.End.Year.ToString());
                        int start = Int32.Parse(mission.Start.Year.ToString());
                        
                        var dict = new Dictionary<string, double[]>();
                        for (int i=start;i<=end;++i)
                        {
                            double[] list = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                            dict.Add(i.ToString(), list);
                        }
                        model.NbParMission.Add(mission.Name, dict);
                        model.DureeMission.Add(mission.Name, new int[] { Int32.Parse(mission.Start.Year.ToString()), Int32.Parse(mission.Start.Month.ToString()), Int32.Parse(mission.End.Year.ToString()), Int32.Parse(mission.End.Month.ToString()) });
                    }
                }

                var cras = db.Cras.ToList();

                foreach (var cra in cras)
                {
                    if (cra.ConsultantID == consultant.ConsultantID)
                    {
                        var activities = db.Activities.ToList();

                        foreach (var activity in activities)
                        {
                            
                                if (activity.CraID == cra.CraID)
                                {
                                switch (activity.Morning)
                                {
                                    case "IC":
                                        break;
                                    case "Formation":
                                        break;
                                    case "Maladie":
                                        break;
                                    case "Congés":
                                        break;
                                    default:
                                        if (model.MissionsList.Contains(activity.Morning))
                                        {
                                            model.NbParMission[activity.Morning][activity.Date.Year.ToString()][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;

                                        }
                                        
                                        break;
                                }
                                switch (activity.Afternoon)
                                {
                                    case "IC":
                                        break;
                                    case "Formation":
                                        break;
                                    case "Maladie":
                                        break;
                                    case "Congés":
                                        break;
                                    default:
                                        if (model.MissionsList.Contains(activity.Afternoon))
                                        {
                                            model.NbParMission[activity.Afternoon][activity.Date.Year.ToString()][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;

                                        }
                                        break;
                                }
                            }

                        }
                    }
                }


                models.Add(model);
            }

            return View(models);

        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes,Manager")]
        //[Route("Consultant/SuiviCra_nonfacture", Name = "SuiviCra_nonfacture")]
        public ActionResult SuiviCra_nonfacture()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            ViewBag.listMonth = new string[] { "Jan", "Fév", "Mar", "Avr", "Mai", "Juin", "Juil", "Aou", "Sep", "Oct", "Nov", "Dec" };

            List<ConsultantCraModel> models = new List<ConsultantCraModel>();

            var consultants = db.Consultants.ToList();

            foreach (var consultant in consultants)
            {
                if (User.IsInRole("Manager"))
                {
                    var email = User.Identity.Name;
                    var user = db.Users.Single(u => u.Email == email);
                    string name = user.FirstName + " " + user.LastName;
                    List<int> pIdlist = new List<int>();
                    foreach (var obj in db.profitCenters.ToList())
                    {
                        var manager = db.Managers.Single(m => m.ManagerID == obj.Owner);

                        if (manager.FirstName + " " + manager.LastName == name)
                        {
                            pIdlist.Add(obj.ProfitCenterID);
                        }
                        try
                        {
                            var manager2 = db.Managers.Single(m => m.ManagerID == obj.PartOwner);

                            if (manager2.FirstName + " " + manager2.LastName == name)
                            {
                                pIdlist.Add(obj.ProfitCenterID);
                            }
                        }
                        catch (Exception) { }


                    }

                    Boolean ability = false;
                    var pc = db.profitCenters.Single(pp => pp.ProfitCenterID == consultant.ProfitCenterID);
                    if (pIdlist.Count != 0)
                    {
                        foreach (var p in pIdlist)
                        {
                            if (pc.ProfitCenterID == p)
                            {
                                ability = true;
                                break;
                            }
                            if (p == pc.FatherProfitCenterID)
                            {
                                ability = true;
                                break;
                            }
                        }
                    }
                    if (!ability)
                    {
                        continue;
                    }
                }

                ConsultantCraModel model = new ConsultantCraModel()
                {
                    ID = consultant.ConsultantID,
                    Consultant = consultant.FirstName + " " + consultant.LastName,
                    MissionsList = new List<string>(),
                    NbParMission = new Dictionary<string, double[]>(),
                };
                var missionList = db.Missions.ToList();
                double[] lista = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                model.NbParMission.Add("nonfacture", lista);
                foreach (var mission in missionList)
                {
                    if (mission.ConsultantID == consultant.ConsultantID)
                    {
                        model.MissionsList.Add(mission.Name);
                        double[] list = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                        model.NbParMission.Add(mission.Name, list);
                    }
                }

                var cras = db.Cras.ToList();

                foreach (var cra in cras)
                {
                    if (cra.ConsultantID == consultant.ConsultantID)
                    {
                        var activities = db.Activities.ToList();

                        foreach (var activity in activities)
                        {
                            if (activity.CraID == cra.CraID)
                            {
                                switch (activity.Morning)
                                {
                                    case "IC":
                                        model.NbParMission["nonfacture"][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;
                                        break;
                                    case "Formation":
                                        model.NbParMission["nonfacture"][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;
                                        break;
                                    case "Maladie":
                                        model.NbParMission["nonfacture"][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;
                                        break;
                                    case "Congés":
                                        model.NbParMission["nonfacture"][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;
                                        break;
                                    default:
                                       
                                        break;
                                }
                                switch (activity.Afternoon)
                                {
                                    case "IC":
                                        model.NbParMission["nonfacture"][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;
                                        break;
                                    case "Formation":
                                        model.NbParMission["nonfacture"][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;
                                        break;
                                    case "Maladie":
                                        model.NbParMission["nonfacture"][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;
                                        break;
                                    case "Congés":
                                        model.NbParMission["nonfacture"][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;
                                        break;
                                    default:
                                        break;
                                }
                            }

                        }
                    }
                }


                models.Add(model);
            }

            return View(models);

        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes,Manager")]
        //[Route("Consultant/test", Name = "test")]
        public ActionResult test2()
        {
            ViewBag.listMonth = new string[] { " Jan ", " Fév ", " Mar ", " Avr ", " Mai ", " Juin ", " Juil ", " Aou ", " Sep ", " Oct ", " Nov ", " Dec " };
            ApplicationDbContext db = new ApplicationDbContext();
            List<ConsultantCraModelNew> models = new List<ConsultantCraModelNew>();
            ViewBag.debut = db.MonthActivations.Single().Periode;

            var consultants = db.Consultants.ToList();

            foreach (var consultant in consultants)
            {
                if (User.IsInRole("Manager"))
                {
                    var email = User.Identity.Name;
                    var user = db.Users.Single(u => u.Email == email);
                    string name = user.FirstName + " " + user.LastName;
                    List<int> pIdlist = new List<int>();
                    foreach (var obj in db.profitCenters.ToList())
                    {
                        var manager = db.Managers.Single(m => m.ManagerID == obj.Owner);

                        if (manager.FirstName + " " + manager.LastName == name)
                        {
                            pIdlist.Add(obj.ProfitCenterID);
                        }
                        try
                        {
                            var manager2 = db.Managers.Single(m => m.ManagerID == obj.PartOwner);

                            if (manager2.FirstName + " " + manager2.LastName == name)
                            {
                                pIdlist.Add(obj.ProfitCenterID);
                            }
                        }
                        catch (Exception) { }


                    }

                    Boolean ability = false;
                    var pc = db.profitCenters.Single(pp => pp.ProfitCenterID == consultant.ProfitCenterID);
                    if (pIdlist.Count != 0)
                    {
                        foreach (var p in pIdlist)
                        {
                            if (pc.ProfitCenterID == p)
                            {
                                ability = true;
                                break;
                            }
                            if (p == pc.FatherProfitCenterID)
                            {
                                ability = true;
                                break;
                            }
                        }
                    }
                    if (!ability)
                    {
                        continue;
                    }
                }
                ConsultantCraModelNew model = new ConsultantCraModelNew()
                {
                    ID = consultant.ConsultantID,
                    Consultant = consultant.FirstName + " " + consultant.LastName,
                    MissionsList = new List<string>(),
                    NbParMission = new Dictionary<string, Dictionary<string, double[]>>(),
                    DureeMission = new Dictionary<string, int[]>(),
                };
                var missionList = db.Missions.ToList();

                foreach (var mission in missionList)
                {
                    if (mission.ConsultantID == consultant.ConsultantID)
                    {
                        if (mission.Start < ViewBag.debut)
                        {
                            ViewBag.debut = mission.Start;
                        }

                        model.MissionsList.Add(mission.Name);
                        int end = Int32.Parse(mission.End.Year.ToString());
                        int start = Int32.Parse(mission.Start.Year.ToString());

                        var dict = new Dictionary<string, double[]>();
                        for (int i = start; i <= end; ++i)
                        {
                            double[] list = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                            dict.Add(i.ToString(), list);
                        }
                        model.NbParMission.Add(mission.Name, dict);
                        model.DureeMission.Add(mission.Name, new int[] { Int32.Parse(mission.Start.Year.ToString()), Int32.Parse(mission.Start.Month.ToString()), Int32.Parse(mission.End.Year.ToString()), Int32.Parse(mission.End.Month.ToString()) });
                    }
                }

                var cras = db.Cras.ToList();

                foreach (var cra in cras)
                {
                    if (cra.ConsultantID == consultant.ConsultantID)
                    {
                        var activities = db.Activities.ToList();

                        foreach (var activity in activities)
                        {

                            if (activity.CraID == cra.CraID)
                            {
                                switch (activity.Morning)
                                {
                                    case "IC":
                                        break;
                                    case "Formation":
                                        break;
                                    case "Maladie":
                                        break;
                                    case "Congés":
                                        break;
                                    default:
                                        if (model.MissionsList.Contains(activity.Morning))
                                        {
                                            model.NbParMission[activity.Morning][activity.Date.Year.ToString()][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;

                                        }

                                        break;
                                }
                                switch (activity.Afternoon)
                                {
                                    case "IC":
                                        break;
                                    case "Formation":
                                        break;
                                    case "Maladie":
                                        break;
                                    case "Congés":
                                        break;
                                    default:
                                        if (model.MissionsList.Contains(activity.Afternoon))
                                        {
                                            model.NbParMission[activity.Afternoon][activity.Date.Year.ToString()][Int32.Parse(activity.Date.Month.ToString()) - 1] += 0.5;

                                        }
                                        break;
                                }
                            }

                        }
                    }
                }


                models.Add(model);
            }

            return View(models);

        }


    }
}


