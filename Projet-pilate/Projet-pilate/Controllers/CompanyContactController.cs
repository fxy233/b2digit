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


        public ActionResult Delete(int id)
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


    }
}