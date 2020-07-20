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
    public class CompanyContactController : Controller
    {
        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager")]
        // GET: /CompanyContact/CreateCompanyContact
        [Route("CompanyContact/CreateCompanyContact", Name = "CompanyContact")]
        public ActionResult CreateCompanyContact()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Manager> managers = db.Managers.ToList();
            List<string> managerNames = new List<string>();

            foreach (var manager in managers)
            {
                managerNames.Add(manager.FirstName + " " + manager.LastName);
            }

            RegisterCompanyContactViewModel model = new RegisterCompanyContactViewModel
            {
                CompanyNames = db.Companies.Select(c => c.Name).ToList(),
                ManagerNames = managerNames,

            };

            return View(model);
        }


        //// POST: /CompanyContact/CreateCompanyContact
        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager")]
        [Route("CompanyContact/CreateCompanyContact")]
        [HttpPost]
        public ActionResult CreateCompanyContact(RegisterCompanyContactViewModel model)
        {
            string companyName;
            string[] managerName;
            string managerFirstName;
            string managerLastName;
            ApplicationDbContext db = new ApplicationDbContext();

            if (!ModelState.IsValid)
            {
                companyName = Request.Form["CompanyId"].ToString();
                managerName = Request.Form["ManagerId"].ToString().Split(' ');
                managerFirstName = managerName[0];
                managerLastName = managerName[1];

                List<Manager> managers = db.Managers.ToList();
                List<string> managerNames = new List<string>();

                foreach (var man in managers)
                {
                    managerNames.Add(man.FirstName + " " + man.LastName);
                }

                model.CompanyNames = db.Companies.Select(c => c.Name).ToList();

                model.ManagerNames = managerNames;

                ViewData["companyName"] = companyName;
                ViewData["managerName"] = managerFirstName + " " + managerLastName;

                return View(model);
            }

            companyName = Request.Form["CompanyId"].ToString();
            managerName = Request.Form["ManagerId"].ToString().Split(' ');
            managerFirstName = managerName[0];
            managerLastName = managerName[1];


            Company company = db.Companies.SingleOrDefault(c => c.Name == companyName);
            Manager manager = db.Managers.SingleOrDefault(c => c.FirstName == managerFirstName
                                      && c.LastName == managerLastName);


            CompanyContact companyContact = new CompanyContact()
            {
                Mail = model.Mail,
                CompanyName = companyName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Position = model.Position,
                PhoneNumber = model.Phone,
            };

            company.CompanyContacts.Add(companyContact);
            manager.CompanyContacts.Add(companyContact);


            db.CompanyContacts.Add(companyContact);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                model.CompanyNames = db.Companies.Select(c => c.Name).ToList();

                List<Manager> managers = db.Managers.ToList();
                List<string> managerNames = new List<string>();

                foreach (var item in managers)
                {
                    managerNames.Add(item.FirstName + " " + item.LastName);
                }

                model.ManagerNames = managerNames;

                ViewData["companyName"] = companyName;
                ViewData["managerName"] = managerFirstName + " " + managerLastName;

                string message = "Un client du même nom existe déjà !";
                ModelState.AddModelError(string.Empty, message);


                return View(model);
            }

            return RedirectToAction("CompanyContactList", "CompanyContact");

        }

        [Authorize(Roles = "Administrateur, Super-Administrateur,Manager,Administrateur-ventes")]
        [Route("CompanyContact/CompanyContactList", Name = "CompanyContactList")]
        public ActionResult CompanyContactList()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var ContactsList = db.CompanyContacts.ToList();
            List<DetailCompanyContactViewModel> models = new List<DetailCompanyContactViewModel>();

            foreach (var contact in ContactsList)
            {
                DetailCompanyContactViewModel model = new DetailCompanyContactViewModel()
                {
                    ID = contact.CompanyContactID,
                    Mail = contact.Mail,
                    CompanyName = contact.CompanyName,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Position = contact.Position,
                    Phone = contact.PhoneNumber,
                };

                models.Add(model);
            }

            return View(models);
            //return View(db.CompanyContacts.ToList());
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur")]
        public ActionResult DeleteCompanyContact(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var ContactsList = db.CompanyContacts.ToList();
            List<DetailCompanyContactViewModel> models = new List<DetailCompanyContactViewModel>();
            var contactToDelete = ContactsList.SingleOrDefault(c => c.CompanyContactID == id);
            db.Entry(contactToDelete).State = EntityState.Deleted;

            foreach (var contact in ContactsList)
            {
                DetailCompanyContactViewModel model = new DetailCompanyContactViewModel()
                {
                    ID = contact.CompanyContactID,
                    Mail = contact.Mail,
                    CompanyName = contact.CompanyName,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Position = contact.Position,
                    Phone = contact.PhoneNumber,
                };

                models.Add(model);
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                string message = "Ce contact ne peut être supprimé car des missions ont déjà été contractées avec celui-ci !";
                ModelState.AddModelError(string.Empty, message);
                return View("CompanyContactList", models);
            }

            return RedirectToAction("CompanyContactList", "CompanyContact");
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur, Administrateur-ventes")]
        // GET: /CompanyContact/EditCompanyContact
        [Route("CompanyContact/EditCompanyContact", Name = "EditCompanyContact")]
        public ActionResult EditCompanyContact(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var companyContact = db.CompanyContacts.Single(c => c.CompanyContactID == id);

            UpdateCompanyContactViewModel model = new UpdateCompanyContactViewModel
            {
                 Id = companyContact.CompanyContactID,
                 CompanyName = companyContact.CompanyName,
                 Mail = companyContact.Mail,
                 FirstName = companyContact.FirstName,
                 LastName = companyContact.LastName,
                 Position = companyContact.Position,
                 Phone = companyContact.PhoneNumber,

            };
            
            return View(model);
        }

        [Authorize(Roles = "Administrateur, Super-Administrateur, Administrateur-ventes")]
        //// POST: /CompanyContact/EditCompanyContact
        [Route("CompanyContact/EditCompanyContact")]
        [HttpPost]
        public ActionResult EditCompanyContact(UpdateCompanyContactViewModel model)
        {

            ApplicationDbContext db = new ApplicationDbContext();

            var companyContact = db.CompanyContacts.Single(c => c.CompanyContactID == model.Id);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var companylist = db.Companies.ToList();
            Boolean exist = false;
            foreach(var c in companylist)
            {
                if(model.CompanyName==c.Name)
                {
                    exist = true;
                    break;
                }
            }

            if(!exist)
            {
                string message = "Vérifier que le société exist !";
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            companyContact.CompanyName = model.CompanyName;
            companyContact.Mail = model.Mail;
            companyContact.FirstName = model.FirstName;
            companyContact.LastName = model.LastName;
            companyContact.Position = model.Position;
            companyContact.PhoneNumber = model.Phone;

            db.SaveChanges();

            return RedirectToAction("CompanyContactList", "CompanyContact");

        }

    }
}