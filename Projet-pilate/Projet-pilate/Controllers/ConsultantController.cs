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

    public class ConsultantController : Controller
    {
        //// GET: /Consultant/CRA
        [Route("Consultant/CRA", Name = "CRA")]
        public ActionResult CRA()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            RegisterActivityViewModel model = new RegisterActivityViewModel();

            var currentMonth = db.MonthActivations.Single();
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


            List<Mission> missionsEnCours = missionsTotal.Where(m =>

            m.Start < startMonth && m.End > endMonth||
            m.Start < startMonth && m.End < endMonth||
            m.Start == startMonth && m.End < endMonth||
            m.Start > startMonth && m.End < endMonth ||
            m.Start > startMonth && m.End == endMonth ||
            m.Start > startMonth && m.End > endMonth).ToList();
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


            if (cra != null && cra.Changeable == false)
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
                        case "ic":
                            detailModel.NbIc += 0.5;
                            break;
                        case "formation":
                            detailModel.NbFormation += 0.5;
                            break;
                        case "maladie":
                            detailModel.NbMaladie += 0.5;
                            break;
                        case "conge":
                            detailModel.NbConge += 0.5;
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
                        case "ic":
                            detailModel.NbIc += 0.5;
                            break;
                        case "formation":
                            detailModel.NbFormation += 0.5;
                            break;
                        case "maladie":
                            detailModel.NbMaladie += 0.5;
                            break;
                        case "conge":
                            detailModel.NbConge += 0.5;
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

        [HttpPost]
        [Route("Consultant/CRA")]
        public ActionResult CRA(RegisterActivityViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Activity> activities = new List<Activity>();

            var currentMonth = db.MonthActivations.Single();
            int month = currentMonth.Periode.Month;
            string yearStr = currentMonth.Periode.Year.ToString();
            int yearInt = currentMonth.Periode.Year;


            string am = Request["matin"].ToString();
            string pm = Request["apres-midi"].ToString();


            string[] matinees = am.Split(',');
            string[] apresMidis = pm.Split(',');


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

                    for (int j = 0; j < apresMidis.Length; j++)
                    {
                        if (matinees[i] == apresMidis[j])
                        {
                            activity.Afternoon = apresMidis[j].Substring(0, apresMidis[j].IndexOf('-'));
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

                    for (int j = 0; j < matinees.Length; j++)
                    {
                        if (apresMidis[i] == matinees[j])
                        {
                            activity.Morning = matinees[j].Substring(0, matinees[j].IndexOf('-'));
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

                Changeable = false,
                Month = moisConcerne,
                year = yearStr,

            };


            string id = User.Identity.Name;
            Consultant consultant = db.Consultants.Single(u => u.Email == id);

            cra.Consultant = consultant;
            cra.Activities = activities;

            db.Cras.Add(cra);

            db.SaveChanges();


            return RedirectToAction("CRA");

        }



        [Route("Consultant/ListeCra",Name = "ListeCra")]
        public ActionResult ListeCra()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<ActivityViewModel> models = new List<ActivityViewModel>();

            List<Cra> cras = db.Cras.ToList();

            foreach (var cra in cras)
            {
                var nbProjetMatin = cra.Activities.Select(a => a.Morning).ToList();
                var nbProjetApreMidi = cra.Activities.Select(a => a.Afternoon).ToList();
                int projetMatin = 0;
                int projetApresMidi = 0;
                int noBillMatin = 0;
                int noBillApresMidi = 0;
                


                foreach (var item in nbProjetMatin)
                {
                    if (item != "conge" && item != "ic" && item != "formation" && item != "maladie")
                    {
                        projetMatin++;
                    }
                }

                foreach (var item in nbProjetApreMidi)
                {
                    if (item != "conge" && item != "ic" && item != "formation" && item != "maladie")
                    {
                        projetApresMidi++;
                    }
                }

                foreach (var item in nbProjetMatin)
                {
                    if (item == "conge" || item == "ic" || item == "formation" || item == "maladie")
                    {
                        noBillMatin++;
                    }
                }

                foreach (var item in nbProjetApreMidi)
                {
                    if (item == "conge" || item == "ic" || item == "formation" || item == "maladie")
                    {
                       noBillApresMidi++;
                    }
                }


                ActivityViewModel model = new ActivityViewModel()
                {
                    ID = cra.CraID,
                    Date =  cra.Month + " " + cra.year,
                    ConsultantName = cra.Consultant.FirstName + " " + cra.Consultant.LastName,
                    //MissionName = cra.Activities.Select(m => m.Morning).ToString(),
                    Satisfaction = cra.Satisfaction,
                    WorkedDays = (projetMatin + projetApresMidi) /2,
                    NoBillDays = (noBillMatin + noBillApresMidi) /2,
                    //WorkedDaysWE = (projetMatin + projetApresMidi) / 2,
                };
                    
               

                models.Add(model);
            }

            return View(models);
        }


        [Route("Consultant/DeleteCra")]
        public ActionResult DeleteCra(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var cra = db.Cras.Single(c => c.CraID == id);

            db.Entry(cra).State = EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("ListeCra","Consultant");
        }
    }
}


