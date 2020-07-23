using iTextSharp.text.pdf.qrcode;
using Projet_pilate.Entities;
using Projet_pilate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projet_pilate.Controllers
{
    public class SubsidiaryController : Controller
    {
        // GET: Subsidiary/CreateSubsidiary
        [Route("Subsidiary/CreateSubsidiary", Name = "CreateSubsidiary")]
        public ActionResult CreateSubsidiary()
        {
            return View();
        }

        // POST: Subsidiary/CreateSubsidiary
        [HttpPost]
        [Route("Subsidiary/CreateSubsidiary")]
        public ActionResult CreateSubsidiary(SubsidiaryViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Subsidiary subsidiary = new Subsidiary()
            {
                Siren = model.Siren,
                Name = model.Name,
                Address = model.Address,
                PostaleCode = model.PostaleCode,
                City = model.City,
                //
                ManagerFirstName = model.ManagerFirstName,
                ManagerLastName = model.ManagerLastName,
                IBAN = model.IBAN,
                BIC = model.BIC,
                TVAIntra = model.TVAIntra,
                email = model.email,
            };

            db.Subsidiaries.Add(subsidiary);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                string message = "Une filiale du même nom a déjà été crée !";
                ModelState.AddModelError(string.Empty, message);

                return View(model);
            }

            return RedirectToAction("SubsidiaryList", "Subsidiary");
        }


        // GET: Subsidiary/SubsidiaryList
        [Route("Subsidiary/SubsidiaryList", Name = "SubsidiaryList")]
        public ActionResult SubsidiaryList()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var SubsidiaryList = db.Subsidiaries.ToList();
            List<DetailSubsidiaryViewModel> models = new List<DetailSubsidiaryViewModel>();

            foreach (var subsidiary in SubsidiaryList)
            {
                DetailSubsidiaryViewModel model = new DetailSubsidiaryViewModel()
                {
                    ID = subsidiary.SubsidiaryID,
                    Siren = subsidiary.Siren,
                    Name = subsidiary.Name,
                    Address = subsidiary.Address,
                    PostaleCode = subsidiary.PostaleCode,
                    City = subsidiary.City,
                    ManagerFirstName = subsidiary.ManagerFirstName,
                    ManagerLastName = subsidiary.ManagerLastName,
                };
                models.Add(model);
            }

            return View(models);
        }


        [Authorize(Roles = "Administrateur, Super-Administrateur")]
        // GET: Subsidiary/Edit
        public ActionResult Edit(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var subsidiary = db.Subsidiaries.Single(s => s.SubsidiaryID == id);
            UpdateSubsidiaryViewModel model = new UpdateSubsidiaryViewModel()
            {
                ID = subsidiary.SubsidiaryID,
                Name = subsidiary.Name,
                Siren = subsidiary.Siren,
                Address = subsidiary.Address,
                PostaleCode = subsidiary.PostaleCode,
                City = subsidiary.City,
                ManagerFirstName = subsidiary.ManagerFirstName,
                ManagerLastName = subsidiary.ManagerLastName,
                IBAN = subsidiary.IBAN,
                BIC = subsidiary.BIC,
                TVAIntra = subsidiary.TVAIntra,
                email = subsidiary.email,
            };

            return View(model);
        }


        [Authorize(Roles = "Administrateur, Super-Administrateur")]
        // POST: Subsidiary/Edit
        [HttpPost]
        public ActionResult Edit(UpdateSubsidiaryViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Subsidiary subsidiary = db.Subsidiaries.Single(s => s.SubsidiaryID == model.ID);


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            subsidiary.Name = model.Name;
            subsidiary.Siren = model.Siren;
            subsidiary.Address = model.Address;
            subsidiary.PostaleCode = model.PostaleCode;
            subsidiary.City = model.City;
            subsidiary.ManagerFirstName = model.ManagerFirstName;
            subsidiary.ManagerLastName = model.ManagerLastName;
            subsidiary.IBAN = model.IBAN;
            subsidiary.BIC = model.BIC;
            subsidiary.TVAIntra = model.TVAIntra;
            subsidiary.email = model.email;

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                string message = "Vérifier qu'une filiale ayant le même siren ou le même nom n'existe pas déjà !";
                ModelState.AddModelError(string.Empty, message);
                return View("Edit", model);
            }
            

            return RedirectToAction("SubsidiaryList", "Subsidiary");
        }


        [Authorize(Roles = "Administrateur, Super-Administrateur")]
        public ActionResult Delete(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var subsidiaryList = db.Subsidiaries.ToList();
            var subsidiary = db.Subsidiaries.Single(s => s.SubsidiaryID == id);
            List<DetailSubsidiaryViewModel> models = new List<DetailSubsidiaryViewModel>();
            var subsidiaryToDelete = subsidiaryList.SingleOrDefault(c => c.SubsidiaryID == id);
            db.Entry(subsidiaryToDelete).State = EntityState.Deleted;

            foreach (var sub in subsidiaryList)
            {
                DetailSubsidiaryViewModel model = new DetailSubsidiaryViewModel()
                {
                    ID = sub.SubsidiaryID,
                    Siren = sub.Siren,
                    Name = sub.Name,
                    Address = sub.Address,
                    PostaleCode = sub.PostaleCode,
                    City = sub.City,
                    ManagerFirstName = sub.ManagerFirstName,
                    ManagerLastName = sub.ManagerLastName,
                };

                models.Add(model);
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                string message = "Cette filiale ne peut être supprimée car plusieurs collaborateurs dépendent de celle-ci !";
                ModelState.AddModelError(string.Empty, message);
                return View("SubsidiaryList", models);
            }

            return RedirectToAction("SubsidiaryList", "Subsidiary");
        }


    }
}