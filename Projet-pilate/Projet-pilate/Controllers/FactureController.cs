using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Projet_pilate.Models;
using System.Web.Mvc;
using System.Globalization;

namespace Projet_pilate.Controllers
{
    public class FactureController : Controller
    {
        // GET: Facture
        [Route("Facture/ListeFactures", Name = "ListeFactures")]
        public ActionResult ListeFactures()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ViewBag.date = db.MonthActivations.Single().Periode;

            List<FactureSimpleViewModel> models = new List<FactureSimpleViewModel>();
            var factureList = db.Factures.ToList();

            foreach(var f in factureList)
            {
                FactureSimpleViewModel model = new FactureSimpleViewModel()
                {
                    ID = f.FactureID,
                    NomFacture = f.NomFacture,
                    Client = f.Client,
                    MontantHT = f.MontantHT.ToString(),
                    Mission = f.mission,
                    Dernier = f.DernierEnregistrer,
                };

                string s = "FAE";

                if (f.Emise)
                {
                    s = "Emise";
                }

                if (f.payee)
                {
                    s = "Payee";
                }

                if (f.annulee)
                {
                    s = "Annulee";
                }

                model.Status = s;

                models.Add(model);
            
            }


            return View(models);
        }

/*        [Route("Facture/Return", Name = "Return")]
        public ActionResult Return()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ClotureViewModel model = new ClotureViewModel();
            var Mois = db.MonthActivations.Single();
            model.MoisaCloturer = Mois.Periode.ToString("MMMM", CultureInfo.CurrentCulture) +
                              " " + Mois.Periode.Year.ToString();

            ModelState.AddModelError(string.Empty, "Mois a été clôturé");
            return View("ClotureMois","Admin", model);
        }*/

        /*[Route("Facture/ListeFactures", Name = "ListeFactures")]
        public ActionResult ListeFactures()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            ViewBag.date = db.MonthActivations.Single().Periode;
            return View();
        }*/

        [Route("Facture/Suivis", Name = "Suivis")]
        public ActionResult Suivis()
        {
            return View();
        }

        [Route("Facture/Modifier", Name = "Modifier")]
        public ActionResult Modifier()
        {
            return View();
        }

        [Route("Facture/Detail", Name = "Detail")]
        public ActionResult Detail()
        {
            return View();
        }


    }
}
