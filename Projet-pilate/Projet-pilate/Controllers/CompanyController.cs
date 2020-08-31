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
    public class CompanyController : Controller
    {
        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager")]
        // GET: /Company/CreationCompany
        [Route("Company/CreationCompany", Name = "CreationCompany")]
        public ActionResult CreationCompany()
        {
            RegisterCompanyViewModel model = new RegisterCompanyViewModel();
            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager")]
        // POST: /Company/CreationCompany
        [Route("Company/CreationCompany")]
        [HttpPost]
        public ActionResult CreationCompany(RegisterCompanyViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationDbContext db = new ApplicationDbContext();

            Company company = new Company()
            {
                Name = model.Name,
                Address = model.Address,
                PostalCode = model.PostalCode,
                City = model.City,
                MailFacturation = model.MailFacturation,

            };

            db.Companies.Add(company);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                string message = "Une entreprise du même nom existe déjà !";
                ModelState.AddModelError(string.Empty, message);


                return View(model);
            }

            return RedirectToAction("ListeCompanies", "Company");

        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager,Administrateur-ventes")]
        //// GET: /Business/ListeCompanies
        [Route("Company/ListeCompanies", Name = "ListeCompanies")]
        public ActionResult ListeCompanies()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var CompanyList = db.Companies.ToList();
            List<DetailCompanyViewModel> models = new List<DetailCompanyViewModel>();

            foreach (var company in CompanyList)
            {
                DetailCompanyViewModel model = new DetailCompanyViewModel()
                {
                    ID = company.CompanyID,
                    Name = company.Name,
                    Address = company.Address,
                    PostalCode = company.PostalCode,
                    City = company.City,
                    Mail = company.MailFacturation,
                };

                models.Add(model);
            }

            return View(models);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        // GET: /Company/Edit
        public ActionResult Edit(int id)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            var company = db.Companies.SingleOrDefault(c => c.CompanyID == id);

            UpdateCompanyViewModel model = new UpdateCompanyViewModel()
            {
                Id = company.CompanyID,
                Name = company.Name,
                Address = company.Address,
                PostalCode = company.PostalCode,
                City = company.City,
                MailFacturation = company.MailFacturation,
            };

            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        [HttpPost]
        public ActionResult Edit(UpdateCompanyViewModel model)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            var company = db.Companies.SingleOrDefault(c => c.CompanyID == model.Id);

            if (ModelState.IsValid)
            {
                company.Name = model.Name;
                company.Address = model.Address;
                company.PostalCode = model.PostalCode;
                company.City = model.City;
                company.MailFacturation = model.MailFacturation;
                db.SaveChanges();

                return RedirectToAction("ListeCompanies", "Company");
            }

            return View(model);
        }


        [Authorize(Roles = "Administrateur, Super-Administrateur,Administrateur-ventes")]
        public ActionResult Details(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var company = db.Companies.SingleOrDefault(c => c.CompanyID == id);

            DetailCompanyViewModel model = new DetailCompanyViewModel();
            model.ID = company.CompanyID;
            model.Name = company.Name;
            model.Address = company.Address;
            model.PostalCode = company.PostalCode;
            model.City = company.City;

            return View(model);
        }


        [Authorize(Roles = "Administrateur, Super-Administrateur")]
        public ActionResult Delete(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var CompanyList = db.Companies.ToList();
            List<DetailCompanyViewModel> models = new List<DetailCompanyViewModel>();
            var companyToDelete = CompanyList.SingleOrDefault(c => c.CompanyID == id);
            db.Entry(companyToDelete).State = EntityState.Deleted;

            foreach (var company in CompanyList)
            {
                DetailCompanyViewModel model = new DetailCompanyViewModel()
                {
                    ID = company.CompanyID,
                    Name = company.Name,
                    Address = company.Address,
                    PostalCode = company.PostalCode,
                    City = company.City,
                };

                models.Add(model);
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                string message = "Cette entreprise ne peut être supprimée car des missions ont déjà été contractées avec celle-ci !";
                ModelState.AddModelError(string.Empty, message);
                return View("ListeCompanies", models);
            }

            return RedirectToAction("ListeCompanies", "Company");
        }

    }




}