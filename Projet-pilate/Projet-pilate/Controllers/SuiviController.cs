using DocumentFormat.OpenXml.EMMA;
using Projet_pilate.Entities;
using Projet_pilate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projet_pilate.Controllers
{
    public class SuiviController : Controller
    {
        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager,Administrateur-ventes")]
        [Route("Suivi/Suivis", Name = "Suivis")]
        public ActionResult Suivis(DateTime start, DateTime end)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var user = db.Users.Single(u => u.UserName == User.Identity.Name);
            var managerid = -1;

            foreach (var m in db.Managers.ToList())
            {
                if (m.FirstName.ToUpper() == user.FirstName.ToUpper() && m.LastName.ToUpper() == user.LastName.ToUpper())
                {
                    managerid = m.ManagerID;
                    break;
                }
            }

            List<int> listPC = new List<int>();

            if (managerid == -1)
            {
                foreach (var cp in db.profitCenters.ToList())
                {
                    
                        listPC.Add(cp.ProfitCenterID);
                     
                }
            }
            else
            {
                foreach (var cp in db.profitCenters.ToList())
                {
                    if (cp.Owner == managerid || cp.PartOwner == managerid)
                    {
                        listPC.Add(cp.ProfitCenterID);
                        continue;
                    }

                    var cpp = cp;
                    while (cpp.FatherProfitCenterID != null)
                    {
                        var pere = db.profitCenters.Single(p => p.ProfitCenterID == cpp.FatherProfitCenterID);
                        if (pere.Owner == managerid || pere.PartOwner == managerid)
                        {
                            listPC.Add(cp.ProfitCenterID);
                            break;
                        }
                        cpp = pere;
                    }
                }
            }

            

            ViewBag.listPC = listPC;
            /*            if(debut == new DateTime()&&fin == new DateTime())
                        {
                            fin = db.MonthActivations.Single().Periode.AddMonths(1);
                            debut = new DateTime(fin.Year, 1, 1);
                        }*/
            if (DateTime.Compare(end,start)<0)
            {
                DateTime fin2 = db.MonthActivations.Single().Periode;
                DateTime debut2 = new DateTime(fin2.Year, 1, 1, 0, 0, 0);
                PeriodeViewModel model2 = new PeriodeViewModel()
                {
                    debut = debut2,
                    fin = fin2,
                };
                string message = "La date de fin ne peut être inférieure ou égale à la date de début de recherche !";
                ModelState.AddModelError(string.Empty, message);
                return View(model2);
            }

            PeriodeViewModel model = new PeriodeViewModel()
            {
                debut = start,
                fin = end,
            };



            

            return View(model);

        }

        [HttpPost]
        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager,Administrateur-ventes")]
        [Route("Suivi/Suivis")]
        public ActionResult Suivis(PeriodeViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();


            var user = db.Users.Single(u => u.UserName == User.Identity.Name);

            var managerid = -1;

            foreach (var m in db.Managers.ToList())
            {
                if (m.FirstName.ToUpper() == user.FirstName.ToUpper() && m.LastName.ToUpper() == user.LastName.ToUpper())
                {
                    managerid = m.ManagerID;
                    break;
                }
            }

            List<int> listPC = new List<int>();

            if (managerid == -1)
            {
                foreach (var cp in db.profitCenters.ToList())
                {

                    listPC.Add(cp.ProfitCenterID);

                }
            }
            else
            {
                foreach (var cp in db.profitCenters.ToList())
                {
                    if (cp.Owner == managerid || cp.PartOwner == managerid)
                    {
                        listPC.Add(cp.ProfitCenterID);
                        continue;
                    }

                    var cpp = cp;
                    while (cpp.FatherProfitCenterID != null)
                    {
                        var pere = db.profitCenters.Single(p => p.ProfitCenterID == cpp.FatherProfitCenterID);
                        if (pere.Owner == managerid || pere.PartOwner == managerid)
                        {
                            listPC.Add(cp.ProfitCenterID);
                            break;
                        }
                        cpp = pere;
                    }
                }
            }



            ViewBag.listPC = listPC;
            /*            if(debut == new DateTime()&&fin == new DateTime())
                        {
                            fin = db.MonthActivations.Single().Periode.AddMonths(1);
                            debut = new DateTime(fin.Year, 1, 1);
                        }*/
            if (DateTime.Compare(model.fin,model.debut) < 0)
            {
                DateTime fin2 = db.MonthActivations.Single().Periode;
                DateTime debut2 = new DateTime(fin2.Year, 1, 1, 0, 0, 0);
                PeriodeViewModel model2 = new PeriodeViewModel()
                {
                    debut = debut2,
                    fin = fin2,
                };
                string message = "La date de fin ne peut être inférieure ou égale à la date de début de recherche !";
                ModelState.AddModelError(string.Empty, message);
                return View(model2);
            }

            return View(model);

        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager, Administrateur-ventes")]
        [Route("Suivi/DetailPC", Name = "DetailPC")]
        public ActionResult DetailPC(int id, string statu,DateTime debut,DateTime fin)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<SuiviDetailViewModel> models = new List<SuiviDetailViewModel>();

            List<string> clist = new List<string>();
/*            float catotal = 0f;
            float cmtotal = 0f;
            float fraistotal = 0f;*/


            var user = db.Users.Single(u => u.UserName == User.Identity.Name);
            Manager manager = null;
            foreach (var m in db.Managers.ToList())
            {
                if (m.FirstName.ToUpper() == user.FirstName.ToUpper() && m.LastName.ToUpper() == user.LastName.ToUpper())
                {
                    manager = m;
                    break;
                }
            }

            

            var suivis = db.Suivis.Where(s=>s.ProfitCenterID==id).ToList();
            foreach(var s in suivis)
            {
                
                if( s.date.Year<debut.Year || (s.date.Year==debut.Year&&s.date.Month<debut.Month) )
                {
                    continue;
                }
                if (s.date.Year > fin.Year || (s.date.Year == fin.Year && s.date.Month > fin.Month))
                {
                    continue;
                }
                if (s.statu!= statu && statu!="Produit")
                {
                    continue;
                }
                SuiviDetailViewModel model = new SuiviDetailViewModel();
                if (s.NomMission == "IC")
                {
                    model.ID = s.SuiviID;
                    model.Consultant = s.Consultant;
                    model.Mission = s.NomMission;
                    model.Client = "Aucun";
                    model.Nbj = s.NombredUO;
                    model.Tj = s.TJ;
                    model.CA = s.NombredUO * s.TJ;
                    model.mois = s.date.ToString("yyyy-MM");
                }
                else
                {
                    var mission = db.Missions.Single(m => m.Name == s.NomMission);
                    var companycontact = db.CompanyContacts.Single(c => c.CompanyContactID == mission.CompanyContactID);



                    model.ID = s.SuiviID;
                    model.Consultant = s.Consultant;
                    model.Mission = s.NomMission;
                    model.Client = companycontact.CompanyName;
                    model.Nbj = s.NombredUO;
                    model.Tj = s.TJ;
                    model.CA = s.NombredUO * s.TJ;
                    model.mois = s.date.ToString("yyyy-MM");
                    
                }
                

                if (manager == null)
                {
                    model.Cout = (float)(model.CA - s.mensuelConsultant * 1.5 - s.fraisConsultant);
                }
                else
                {
                    if (manager.role == "BM")
                    {
                        model.Cout = (float)(model.CA * 0.9 - s.mensuelConsultant * 1.7 - s.fraisConsultant);

                    }
                    else
                    {
                        model.Cout = (float)(model.CA * 0.93 - s.mensuelConsultant * 1.6 - s.fraisConsultant);
                    }
                }




                models.Add(model);
            }

            int mid = db.profitCenters.Single(p => p.ProfitCenterID == id).Owner;
            var managerPC = db.Managers.Single(m => m.ManagerID == mid);
            int pid = db.profitCenters.Single(p => p.ProfitCenterID == id).PartOwner;
            Manager managerPC2 = null;
            try
            {
                managerPC2 = db.Managers.Single(m => m.ManagerID == pid);
            }
            catch (Exception)
            {

            }

            DateTime a = new DateTime(debut.Year, debut.Month, 1);
            while (DateTime.Compare(a,fin)<=0 )
            {
                
                if (manager == null )
                {
                    if (a.Year == managerPC.EntryDate.Year && a.Month == managerPC.EntryDate.Month)
                    {
                        SuiviDetailViewModel m = new SuiviDetailViewModel();
                        m.Consultant = managerPC.FirstName + " " + managerPC.LastName;
                        m.Mission = "Aucun";
                        m.Client = "Aucun";
                        m.Nbj = 0;
                        m.Tj = 0;
                        m.CA = 0;
                        m.mois = a.ToString("yyyy-MM");
                        m.Cout = (float)(-managerPC.MonthlyCost  * 1.5 - managerPC.MealCost - managerPC.TravelPackage - managerPC.ExceptionalCost)*(DateTime.DaysInMonth(a.Year, a.Month)-managerPC.EntryDate.Day+1)/DateTime.DaysInMonth(a.Year,a.Month);
                        models.Add(m);
                    }
                    if (DateTime.Compare(a, managerPC.EntryDate) > 0)
                    {
                        SuiviDetailViewModel m = new SuiviDetailViewModel();
                        m.Consultant = managerPC.FirstName + " " + managerPC.LastName;
                        m.Mission = "Aucun";
                        m.Client = "Aucun";
                        m.Nbj = 0;
                        m.Tj = 0;
                        m.CA = 0;
                        m.mois = a.ToString("yyyy-MM");
                        m.Cout = (float)(-managerPC.MonthlyCost * 1.5 - managerPC.MealCost - managerPC.TravelPackage - managerPC.ExceptionalCost);
                        models.Add(m);
                    }
                    
                    if (managerPC2 != null)
                    {
                        if (a.Year == managerPC2.EntryDate.Year && a.Month == managerPC2.EntryDate.Month)
                        {
                            SuiviDetailViewModel m = new SuiviDetailViewModel();
                            m.Consultant = managerPC2.FirstName + " " + managerPC2.LastName;
                            m.Mission = "Aucun";
                            m.Client = "Aucun";
                            m.Nbj = 0;
                            m.Tj = 0;
                            m.CA = 0;
                            m.mois = a.ToString("yyyy-MM");
                            m.Cout = (float)(-managerPC2.MonthlyCost * 1.5 - managerPC2.MealCost - managerPC2.TravelPackage - managerPC2.ExceptionalCost) * (DateTime.DaysInMonth(a.Year, a.Month) - managerPC2.EntryDate.Day + 1) / DateTime.DaysInMonth(a.Year, a.Month);
                            models.Add(m);
                        }
                        if (DateTime.Compare(a, managerPC2.EntryDate) > 0)
                        {
                            SuiviDetailViewModel m2 = new SuiviDetailViewModel();
                            m2.Consultant = managerPC2.FirstName + " " + managerPC2.LastName;
                            m2.Mission = "Aucun";
                            m2.Client = "Aucun";
                            m2.Nbj = 0;
                            m2.Tj = 0;
                            m2.CA = 0;
                            m2.mois = a.ToString("yyyy-MM");
                            m2.Cout = (float)(-managerPC2.MonthlyCost * 1.5 - managerPC2.MealCost - managerPC2.TravelPackage - managerPC2.ExceptionalCost);
                            models.Add(m2);
                        }
                        
                    }
                }
                else
                {
                    if (manager.role == "BM")
                    {

                    }
                    else
                    {
                        if (a.Year == managerPC.EntryDate.Year && a.Month == managerPC.EntryDate.Month)
                        {
                            SuiviDetailViewModel m = new SuiviDetailViewModel();
                            m.Consultant = managerPC.FirstName + " " + managerPC.LastName;
                            m.Mission = "Aucun";
                            m.Client = "Aucun";
                            m.Nbj = 0;
                            m.Tj = 0;
                            m.CA = 0;
                            m.mois = a.ToString("yyyy-MM");
                            m.Cout = (float)(-managerPC.MonthlyCost * 1.55 - managerPC.MealCost - managerPC.TravelPackage - managerPC.ExceptionalCost) * (DateTime.DaysInMonth(a.Year, a.Month) - managerPC.EntryDate.Day + 1) / DateTime.DaysInMonth(a.Year, a.Month);
                            models.Add(m);
                        }
                        if (DateTime.Compare(a, managerPC.EntryDate) > 0)
                        {
                            SuiviDetailViewModel m = new SuiviDetailViewModel();
                            m.Consultant = managerPC.FirstName + " " + managerPC.LastName;
                            m.Mission = "Aucun";
                            m.Client = "Aucun";
                            m.Nbj = 0;
                            m.Tj = 0;
                            m.CA = 0;
                            m.mois = a.ToString("yyyy-MM");
                            m.Cout = (float)(-managerPC.MonthlyCost * 1.55 - managerPC.MealCost - managerPC.TravelPackage - managerPC.ExceptionalCost);
                            models.Add(m);
                        }
                        if (managerPC2 != null)
                        {
                            if (a.Year == managerPC2.EntryDate.Year && a.Month == managerPC2.EntryDate.Month)
                            {
                                SuiviDetailViewModel m = new SuiviDetailViewModel();
                                m.Consultant = managerPC2.FirstName + " " + managerPC2.LastName;
                                m.Mission = "Aucun";
                                m.Client = "Aucun";
                                m.Nbj = 0;
                                m.Tj = 0;
                                m.CA = 0;
                                m.mois = a.ToString("yyyy-MM");
                                m.Cout = (float)(-managerPC2.MonthlyCost * 1.55 - managerPC2.MealCost - managerPC2.TravelPackage - managerPC2.ExceptionalCost) * (DateTime.DaysInMonth(a.Year, a.Month) - managerPC2.EntryDate.Day + 1) / DateTime.DaysInMonth(a.Year, a.Month);
                                models.Add(m);
                            }
                            if (DateTime.Compare(a, managerPC2.EntryDate) > 0)
                            {
                                SuiviDetailViewModel m2 = new SuiviDetailViewModel();
                                m2.Consultant = managerPC2.FirstName + " " + managerPC2.LastName;
                                m2.Mission = "Aucun";
                                m2.Client = "Aucun";
                                m2.Nbj = 0;
                                m2.Tj = 0;
                                m2.CA = 0;
                                m2.mois = a.ToString("yyyy-MM");
                                m2.Cout = (float)(-managerPC2.MonthlyCost * 1.55 - managerPC2.MealCost - managerPC2.TravelPackage - managerPC2.ExceptionalCost);
                                models.Add(m2);
                            }

                        }
                    }
                }
                a = a.AddMonths(1);
            }
            


            ViewBag.id = "table"+id+statu;
            ViewBag.id2 = id;
            ViewBag.sta = statu;
            ViewBag.debut = debut;
            ViewBag.fin = fin;

            

            return View(models);
        }


        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager,Administrateur-ventes")]
        [Route("Suivi/PCView", Name = "PCView")]
        public ActionResult PCView(int id, DateTime debut, DateTime fin)
        {
            ViewBag.id = id;
            ViewBag.debut = debut;
            ViewBag.fin = fin;
            return View();
        }


        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager,Administrateur-ventes")]
        [Route("Suivi/TotalPC", Name = "TotalPC")]
        public ActionResult TotalPC(int id, DateTime debut, DateTime fin)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<SuiviTotalViewModel> models = new List<SuiviTotalViewModel>();

            Dictionary<string, SuiviTotalViewModel> dicts = new Dictionary<string, SuiviTotalViewModel>();

            DateTime a = new DateTime(debut.Year, debut.Month,1);
            while (DateTime.Compare(a, fin) <= 0)
            {
                SuiviTotalViewModel m = new SuiviTotalViewModel()
                {
                    mois = a.ToString("yyyy-MM"),
                    coutreel = 0,
                    couttheorie = 0,
                };
                dicts.Add(a.ToString("yyyy-MM"),m);
                
                a = a.AddMonths(1);
            }

            List<string> clist = new List<string>();

            var user = db.Users.Single(u => u.UserName == User.Identity.Name);
            Manager manager = null;
            foreach (var m in db.Managers.ToList())
            {
                if (m.FirstName.ToUpper() == user.FirstName.ToUpper() && m.LastName.ToUpper() == user.LastName.ToUpper())
                {
                    manager = m;
                    break;
                }
            }



            var suivis = db.Suivis.Where(s => s.ProfitCenterID == id).ToList();
            foreach (var s in suivis)
            {

                if (s.date.Year < debut.Year || (s.date.Year == debut.Year && s.date.Month < debut.Month))
                {
                    continue;
                }
                if (s.date.Year > fin.Year || (s.date.Year == fin.Year && s.date.Month > fin.Month))
                {
                    continue;
                }
                if (s.statu == "Facture")
                {
                    if (manager == null)
                    {
                        dicts[s.date.ToString("yyyy-MM")].coutreel += (float)(s.NombredUO * s.TJ - s.mensuelConsultant * 1.5 - s.fraisConsultant);
                        dicts[s.date.ToString("yyyy-MM")].couttheorie += (float)(s.NombredUO * s.TJ - s.mensuelConsultant * 1.5 - s.fraisConsultant);

                    }
                    else
                    {
                        if (manager.role == "BM")
                        {
                            dicts[s.date.ToString("yyyy-MM")].coutreel += (float)(s.NombredUO * s.TJ * 0.9 - s.mensuelConsultant * 1.7 - s.fraisConsultant);
                            dicts[s.date.ToString("yyyy-MM")].couttheorie += (float)(s.NombredUO * s.TJ * 0.9 - s.mensuelConsultant * 1.7 - s.fraisConsultant);

                        }
                        else
                        {
                            dicts[s.date.ToString("yyyy-MM")].coutreel += (float)(s.NombredUO * s.TJ * 0.93 - s.mensuelConsultant * 1.6 - s.fraisConsultant);
                            dicts[s.date.ToString("yyyy-MM")].couttheorie += (float)(s.NombredUO * s.TJ * 0.93 - s.mensuelConsultant * 1.6 - s.fraisConsultant);

                        }
                    }
                }
                else
                {
                    if (manager == null)
                    {
                        dicts[s.date.ToString("yyyy-MM")].couttheorie += (float)(s.NombredUO * s.TJ - s.mensuelConsultant * 1.5 - s.fraisConsultant);

                    }
                    else
                    {
                        if (manager.role == "BM")
                        {
                            dicts[s.date.ToString("yyyy-MM")].couttheorie += (float)(s.NombredUO * s.TJ * 0.9 - s.mensuelConsultant * 1.7 - s.fraisConsultant);

                        }
                        else
                        {
                            dicts[s.date.ToString("yyyy-MM")].couttheorie += (float)(s.NombredUO * s.TJ * 0.93 - s.mensuelConsultant * 1.6 - s.fraisConsultant);

                        }
                    }
                }
            }

            int mid = db.profitCenters.Single(p => p.ProfitCenterID == id).Owner;
            var managerPC = db.Managers.Single(m => m.ManagerID == mid);
            int pid = db.profitCenters.Single(p => p.ProfitCenterID == id).PartOwner;
            Manager managerPC2 = null;
            try
            {
                managerPC2 = db.Managers.Single(m => m.ManagerID == pid);
            }
            catch (Exception)
            {

            }

            DateTime b = new DateTime(debut.Year, debut.Month, 1);
            while (DateTime.Compare(b, fin) <= 0)
            {
                
                if (manager == null)
                {
                    if (b.Year == managerPC.EntryDate.Year && b.Month == managerPC.EntryDate.Month)
                    {
                        dicts[b.ToString("yyyy-MM")].coutreel += (float)(-managerPC.MonthlyCost * 1.5 - managerPC.MealCost - managerPC.TravelPackage - managerPC.ExceptionalCost) * (DateTime.DaysInMonth(b.Year, b.Month) - managerPC.EntryDate.Day + 1) / DateTime.DaysInMonth(b.Year, b.Month);
                        dicts[b.ToString("yyyy-MM")].couttheorie += (float)(-managerPC.MonthlyCost * 1.5 - managerPC.MealCost - managerPC.TravelPackage - managerPC.ExceptionalCost) * (DateTime.DaysInMonth(b.Year, b.Month) - managerPC.EntryDate.Day + 1) / DateTime.DaysInMonth(b.Year, b.Month);

                    }
                    if (DateTime.Compare(b, managerPC.EntryDate) > 0)
                    {
                        dicts[b.ToString("yyyy-MM")].coutreel += (float)(-managerPC.MonthlyCost * 1.5 - managerPC.MealCost - managerPC.TravelPackage - managerPC.ExceptionalCost);
                        dicts[b.ToString("yyyy-MM")].couttheorie += (float)(-managerPC.MonthlyCost * 1.5 - managerPC.MealCost - managerPC.TravelPackage - managerPC.ExceptionalCost);
                    }
                    if (managerPC2 != null)
                    {
                        if (b.Year == managerPC2.EntryDate.Year && b.Month == managerPC2.EntryDate.Month)
                        {
                            dicts[b.ToString("yyyy-MM")].coutreel += (float)(-managerPC2.MonthlyCost * 1.5 - managerPC2.MealCost - managerPC2.TravelPackage - managerPC2.ExceptionalCost) * (DateTime.DaysInMonth(b.Year, b.Month) - managerPC2.EntryDate.Day + 1) / DateTime.DaysInMonth(b.Year, b.Month);
                            dicts[b.ToString("yyyy-MM")].couttheorie += (float)(-managerPC2.MonthlyCost * 1.5 - managerPC2.MealCost - managerPC2.TravelPackage - managerPC2.ExceptionalCost) * (DateTime.DaysInMonth(b.Year, b.Month) - managerPC2.EntryDate.Day + 1) / DateTime.DaysInMonth(b.Year, b.Month);

                        }
                        if (DateTime.Compare(b, managerPC2.EntryDate) > 0)
                        {
                            dicts[b.ToString("yyyy-MM")].coutreel += (float)(-managerPC2.MonthlyCost * 1.5 - managerPC2.MealCost - managerPC2.TravelPackage - managerPC2.ExceptionalCost);
                            dicts[b.ToString("yyyy-MM")].couttheorie += (float)(-managerPC2.MonthlyCost * 1.5 - managerPC2.MealCost - managerPC2.TravelPackage - managerPC2.ExceptionalCost);
                        }

                    }
                }
                else
                {
                    if (manager.role == "BM")
                    {

                    }
                    else
                    {
                        if (b.Year == managerPC.EntryDate.Year && b.Month == managerPC.EntryDate.Month)
                        {
                            dicts[b.ToString("yyyy-MM")].coutreel += (float)(-managerPC.MonthlyCost * 1.55 - managerPC.MealCost - managerPC.TravelPackage - managerPC.ExceptionalCost) * (DateTime.DaysInMonth(b.Year, b.Month) - managerPC.EntryDate.Day + 1) / DateTime.DaysInMonth(b.Year, b.Month);
                            dicts[b.ToString("yyyy-MM")].couttheorie += (float)(-managerPC.MonthlyCost * 1.55 - managerPC.MealCost - managerPC.TravelPackage - managerPC.ExceptionalCost) * (DateTime.DaysInMonth(b.Year, b.Month) - managerPC.EntryDate.Day + 1) / DateTime.DaysInMonth(b.Year, b.Month);

                        }
                        if (DateTime.Compare(b, managerPC.EntryDate) > 0)
                        {
                            dicts[b.ToString("yyyy-MM")].coutreel += (float)(-managerPC.MonthlyCost * 1.55 - managerPC.MealCost - managerPC.TravelPackage - managerPC.ExceptionalCost);
                            dicts[b.ToString("yyyy-MM")].couttheorie += (float)(-managerPC.MonthlyCost * 1.55 - managerPC.MealCost - managerPC.TravelPackage - managerPC.ExceptionalCost);
                        }
                        if (managerPC2 != null)
                        {
                            if (b.Year == managerPC2.EntryDate.Year && b.Month == managerPC2.EntryDate.Month)
                            {
                                dicts[b.ToString("yyyy-MM")].coutreel += (float)(-managerPC2.MonthlyCost * 1.55 - managerPC2.MealCost - managerPC2.TravelPackage - managerPC2.ExceptionalCost) * (DateTime.DaysInMonth(b.Year, b.Month) - managerPC2.EntryDate.Day + 1) / DateTime.DaysInMonth(b.Year, b.Month);
                                dicts[b.ToString("yyyy-MM")].couttheorie += (float)(-managerPC2.MonthlyCost * 1.55 - managerPC2.MealCost - managerPC2.TravelPackage - managerPC2.ExceptionalCost) * (DateTime.DaysInMonth(b.Year, b.Month) - managerPC2.EntryDate.Day + 1) / DateTime.DaysInMonth(b.Year, b.Month);

                            }
                            if (DateTime.Compare(b, managerPC2.EntryDate) > 0)
                            {
                                dicts[b.ToString("yyyy-MM")].coutreel += (float)(-managerPC2.MonthlyCost * 1.55 - managerPC2.MealCost - managerPC2.TravelPackage - managerPC2.ExceptionalCost);
                                dicts[b.ToString("yyyy-MM")].couttheorie += (float)(-managerPC2.MonthlyCost * 1.55 - managerPC2.MealCost - managerPC2.TravelPackage - managerPC2.ExceptionalCost);
                            }

                        }
                    }
                }
                b = b.AddMonths(1);
            }


            foreach(KeyValuePair<string, SuiviTotalViewModel> entry in dicts)
            {
                models.Add(entry.Value);
            }


            ViewBag.id = "table" + id + "Total";
            ViewBag.id2 = id;
            ViewBag.debut = debut;
            ViewBag.fin = fin;



            return View(models);
        }

    }
}