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
        public ActionResult Suivis()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Users.Single(u => u.UserName == User.Identity.Name);

            var managerid = 0;

            foreach(var m in db.Managers.ToList())
            {
                if (m.FirstName.ToUpper() == user.FirstName.ToUpper() && m.LastName.ToUpper() == user.LastName.ToUpper())
                {
                    managerid = m.ManagerID;
                    break;
                }
            }

            List<int> listPC = new List<int>();

            
            foreach(var cp in db.profitCenters.ToList())
            {
                if (cp.Owner==managerid||cp.PartOwner==managerid)
                {
                    listPC.Add(cp.ProfitCenterID);
                    continue;
                }

                var cpp = cp;
                while (cpp.FatherProfitCenterID != null)
                {
                    var pere = db.profitCenters.Single(p => p.ProfitCenterID == cpp.FatherProfitCenterID);
                    if (pere.Owner==managerid||pere.PartOwner==managerid)
                    {
                        listPC.Add(cp.ProfitCenterID);
                        break;
                    }
                    cpp = pere;
                }
            }

            ViewBag.listPC = listPC;

            return View();

        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager")]
        [Route("Suivi/DetailPC", Name = "DetailPC")]
        public ActionResult DetailPC(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<SuiviDetailViewModel> models = new List<SuiviDetailViewModel>();

            var suivis = db.Suivis.Where(s=>s.ProfitCenterID==id).ToList();
            foreach(var s in suivis)
            {
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
                models.Add(model);
            }

            ViewBag.id = "table"+id;
            ViewBag.id2 = id;
            return View(models);
        }

    }
}