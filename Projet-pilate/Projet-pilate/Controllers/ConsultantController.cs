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

            foreach(var itemCra in dCra)
            if (itemCra != null && itemCra.Month== cra.Month && itemCra.year==cra.year)
            {
                return View("info");
            }

            db.Cras.Add(cra);

            db.SaveChanges();


            return RedirectToAction("CRA");

        }


        [Route("Consultant/ListeCra", Name = "ListeCra")]
        public ActionResult ListeCra()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<ActivityViewModel> models = new List<ActivityViewModel>();

            List<Cra> cras = db.Cras.ToList();

            var currentMonth = db.MonthActivations.Single().Periode.ToString("MMMM", CultureInfo.CurrentCulture);


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
                            if ((item.Morning != "Congés" && item.Morning != "IC" && item.Morning != "Formation" && item.Morning != "Maladie")
                                || (item.Afternoon != "Congés" && item.Afternoon != "IC" && item.Afternoon != "Formation" && item.Afternoon != "Maladie"))
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
                        }
                        else
                        { // on est en jours ouvrés
                            if ((item.Morning != "Congés" && item.Morning != "IC" && item.Morning != "Formation" && item.Morning != "Maladie")
                                || (item.Afternoon != "Congés" && item.Afternoon != "IC" && item.Afternoon != "Formation" && item.Afternoon != "Maladie"))
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
                                    NBJT[IndexMission]++;
                                }
                            }

                            if ((item.Morning == "Congés" || item.Morning == "IC" || item.Morning == "Formation" || item.Morning == "Maladie")
                                || (item.Afternoon == "Congés" || item.Afternoon == "IC" || item.Afternoon == "Formation" || item.Afternoon == "Maladie"))
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
                            WorkedDays = NBJT[IndexOfMission],
                            NoBillDays = noBill,
                            WorkedDaysWE = NBJTWE[IndexOfMission],
                        };
                        models.Add(model);
                    }
                }
                    
               

                
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

        [Route("Consultant/CRAReadOnly")]
        public ActionResult CRAReadOnly(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
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

        [Route("Consultant/SuiviCra", Name = "SuiviCra")]
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

        }



    }
}


