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
        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager")]
        [Route("Suivi/Suivis", Name = "Suivis")]
        public ActionResult Suivis(DateTime debut, DateTime fin)
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
            if (DateTime.Compare(fin,debut)<0)
            {
                DateTime fin2 = db.MonthActivations.Single().Periode;
                DateTime debut2 = new DateTime(fin.Year, 1, 1, 0, 0, 0);
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
                debut = debut,
                fin = fin,
            };



            

            return View(model);

        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager")]
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
                var mission = db.Missions.Single(m => m.Name == s.NomMission);
                var companycontact = db.CompanyContacts.Single(c => c.CompanyContactID == mission.CompanyContactID);
                SuiviDetailViewModel model = new SuiviDetailViewModel()
                {
                    ID = s.SuiviID,
                    Consultant = s.Consultant,
                    Mission = s.NomMission,
                    Client = companycontact.CompanyName,
                    Nbj = s.NombredUO,
                    Tj = s.TJ,
                    CA = s.NombredUO*s.TJ,
                };

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
                

/*                catotal += model.CA;
                cmtotal += s.mensuelConsultant;
                fraistotal += s.fraisConsultant;*/

                models.Add(model);
            }

            

            

            
            /*if (manager.role == "BM")
            {
                double CM = catotal * 0.9 - cmtotal * 1.7 - fraistotal; 
                if (statu == "FAE")
                {
                    ViewBag.message = "Rapport total pour FAE: " + CM.ToString() + " euros";
                }
                if (statu == "Facture")
                {
                    ViewBag.message = "Rapport total réel: " + CM.ToString() + " euros";
                }
                if (statu == "Produit")
                {
                    ViewBag.message = "Rapport total en théorie: " + CM.ToString() + " euros";
                }
            }
            else
            {
                double TM = catotal * 0.93 - cmtotal * 1.6 - fraistotal - manager.MonthlyCost * 1.55 - manager.MealCost - manager.TravelPackage - manager.ExceptionalCost;
                if (statu == "FAE")
                {
                    ViewBag.message = "Rapport total pour FAE: " + TM.ToString() + " euros";
                }
                if (statu == "Facture")
                {
                    ViewBag.message = "Rapport total réel: " + TM.ToString() + " euros";
                }

                if (statu == "Produit")
                {
                    ViewBag.message = "Rapport total en théorie: " + TM.ToString() + " euros";
                }
            }*/

            ViewBag.id = "table"+id+statu;
            ViewBag.id2 = id;
            ViewBag.sta = statu;
            ViewBag.debut = debut;
            ViewBag.fin = fin;

            

            return View(models);
        }


        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager")]
        [Route("Suivi/PCView", Name = "PCView")]
        public ActionResult PCView(int id, DateTime debut, DateTime fin)
        {
            ViewBag.id = id;
            ViewBag.debut = debut;
            ViewBag.fin = fin;
            return View();
        }
    }
}