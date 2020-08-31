﻿using System;
using System.Collections.Generic;
using Projet_pilate.Entities;
using System.Linq;
using System.Web;
using Projet_pilate.Models;
using System.Web.Mvc;
using System.Globalization;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using System.Web.Security;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.html;
using System.Text;
using DocumentFormat.OpenXml.EMMA;
using System.Text.RegularExpressions;
using NPOI.SS.Formula.Functions;

using System.Text;
using System.Net.Mail;
using System.Net;
using DocumentFormat.OpenXml.Office.CustomUI;


namespace Projet_pilate.Controllers
{
    public class FactureController : Controller
    {
        // GET: Facture
        
        public ActionResult ListeFactures()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ViewBag.date = db.MonthActivations.Single().Periode;

            List<FactureSimpleViewModel> models = new List<FactureSimpleViewModel>();
            var factureList = db.Factures.ToList();

            foreach(var f in factureList)
            {
                if (f.payee == true || f.Emise == true || f.parentID>0)
                {
                    continue;
                }

                var M = db.Missions.Single(m => m.Name == f.mission);
                var C = db.Consultants.Single(c => c.ConsultantID == M.ConsultantID);

                FactureSimpleViewModel model = new FactureSimpleViewModel()
                {
                    ID = f.FactureID,
                    NomFacture = f.NomFacture,
                    Client = f.Client,
                    MontantHT = ((1 + f.TVA) * f.MontantHT).ToString() + " €",
                    Mission = f.mission,
                    Dernier = f.DernierEnregistrer,
                    Emettrice = f.PrincipalBC,
                    Consultant = C.FirstName + " " + C.LastName,
                    MoisSaisie = f.MoisDeFacturation.ToString("yyyy-MM"),
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

        [Route("Facture/Factures", Name = "Factures")]
        public ActionResult Factures()
        {
            OngletViewModel model = new OngletViewModel()
            {
                first = "active",
                seconde = "",
                third = "",
            };
            return View(model);
        }

        [Route("Facture/Factures2", Name = "Factures2")]
        public ActionResult Factures2()
        {
            OngletViewModel model = new OngletViewModel()
            {
                first = "",
                seconde = "active",
                third = "",
            };
            return View("Factures", model);
        }

        [Route("Facture/Factures3", Name = "Factures3")]
        public ActionResult Factures3()
        {
            OngletViewModel model = new OngletViewModel()
            {
                first = "",
                seconde = "",
                third = "active",
            };
            return View("Factures", model);
        }

        public ActionResult FacturesEmise()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ViewBag.date = db.MonthActivations.Single().Periode;

            List<FactureSimpleViewModel> models = new List<FactureSimpleViewModel>();
            var factureList = db.Factures.ToList();

            foreach (var f in factureList)
            {
                if (f.Emise==false||f.payee==true)
                {
                    continue;
                }
                var M = db.Missions.Single(m => m.Name == f.mission);
                var C = db.Consultants.Single(c => c.ConsultantID == M.ConsultantID);
                FactureSimpleViewModel model = new FactureSimpleViewModel()
                {
                    ID = f.FactureID,
                    NomFacture = f.NomFacture,
                    Client = f.Client,
                    MontantHT = ((1 + f.TVA) * f.MontantHT).ToString() + " €",
                    Mission = f.mission,
                    Dernier = f.DernierEnregistrer,
                    Emettrice = f.PrincipalBC,
                    Consultant = C.FirstName + " " + C.LastName,
                    MoisSaisie = f.MoisDeFacturation.ToString("yyyy-MM"),
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



        [Route("Facture/Modifier", Name = "Modifier")]
        public ActionResult Modifier(int id)
        {
                        ApplicationDbContext db = new ApplicationDbContext();
                        var facture = db.Factures.Single(f => f.FactureID == id);

            if (facture.Emise == true)
            {
                return RedirectToAction("Factures2");
            }
                        
                        ViewBag.TVA = facture.TVA * 100;

                        FactureCreationViewModel model = new FactureCreationViewModel();
                        model.ID = facture.FactureID;
                        model.NomEmettrice = facture.PrincipalBC;
                        var emettrice = db.Subsidiaries.Single(s => s.Name == model.NomEmettrice);
                        model.AdresseEmettrice = emettrice.Address;
                        model.VilleEmettrice = emettrice.PostaleCode + ", " + emettrice.City;
                        model.MailEmettrice = emettrice.email;
                        model.Siren = emettrice.Siren;
                        model.ClientName = facture.Client;

                        string status = "FAE";

                        if (facture.Emise)
                        {
                status = "Emise";
                        }

                        if (facture.payee)
                        {
                status = "Payee";
                        }

                        if (facture.annulee)
                        {
                status = "Annulee";
                        }


                        model.status = status;           

                        foreach(var client in db.Companies.ToList())
                        {
                            if(client.Name == facture.Client)
                            {
                                model.ClientAdresse = client.Address;
                                model.ClientVille = client.PostalCode + ", " + client.City;
                                model.ClientContact = client.MailFacturation;
                            }
                        }

                        foreach (var client in db.Subsidiaries.ToList())
                        {
                            if (client.Name == facture.Client)
                            {
                                model.ClientAdresse = client.Address;
                                model.ClientVille = client.PostaleCode + ", " + client.City;
                                model.ClientContact = client.email;
                            }
                        }

                        model.type = facture.type == null ? "facture" : facture.type;
                        model.FactureName = facture.NomFacture;
                        model.FactInfo = facture.InfoFacturation;
                        model.date = facture.MoisDeFacturation;
                        model.designation = facture.DesignationFacturation;
                        model.Quantite = facture.NombredUO;
                        model.HTunitaire = facture.TJ;
                        model.TVA = facture.TVA*100;
                        model.totalHT = facture.MontantHT;
                        model.montantTVA = facture.TVA * facture.MontantHT;
                        model.totalTTC = (1 + facture.TVA) * facture.MontantHT;
                        model.dateReglement = facture.MoisDeFacturation;
                        model.IBAN = emettrice.IBAN;
                        model.BIC = emettrice.BIC;
                        model.TVAintra = emettrice.TVAIntra;
                        model.Mention = facture.mention;
                        model.mission = facture.mission;
            model.dateReglement = facture.DateRegelement;
            model.Reference = facture.reference;
            model.ReferenceBancaire = facture.referenceBancaire;



            if (facture.parentID < 0)
            {
                model.factures = new List<Facture>();
                foreach(var f in db.Factures.ToList())
                {
                    if (f.parentID == facture.FactureID)
                    {
                        model.factures.Add(f);
                    }
                }
            }

                        return View(model);
            
        }

        [Route("Facture/Modifier")]
        [HttpPost]
        public ActionResult Modifier(FactureCreationViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();


            var facture = db.Factures.Single(f => f.FactureID == model.ID);
            string nm = Request.Form["NomEmettrice"];
            var sub = db.Subsidiaries.Single(s => s.Name == nm);


            string nomMission = Request.Form["Mission"];
            var missionItem = db.Missions.Single(m => m.Name == nomMission);
      
           
            sub.FactureID++;
            db.SaveChanges();


            facture.mission = Request.Form["Mission"];
            facture.MoisDeFacturation = model.date;
            facture.InfoFacturation = model.FactInfo;
            facture.PrincipalBC = Request.Form["NomEmettrice"];
            facture.AdresseBC = Request.Form["AdresseEmettrice"];
            facture.Client = Request.Form["ClientName"];
            facture.AdresseFacturation = Request.Form["ClientAdresse"];
            facture.type = Request.Form["Type"];
            facture.DernierEnregistrer = DateTime.Now;
            facture.Delai = missionItem.Delai;
            facture.DesignationFacturation = missionItem.DesignationFacturation;
            facture.DateRegelement = model.dateReglement;
            facture.reference = model.Reference;
            facture.referenceBancaire = model.ReferenceBancaire;
            facture.mention = model.Mention;

            
            if (facture.parentID < 0)
            {
                int i = 0;
                facture.NombredUO = 0;
                facture.TJ = 0;
                facture.TVA = (float)Int32.Parse(Request.Form["TVA0"]) / 100;
                facture.MontantHT = 0;
                foreach (var item in db.Factures.ToList())
                {
                    if (item.parentID == facture.FactureID)
                    {
                        string str = "TVA" + i.ToString();
                        var f = db.Factures.Single(ff => ff.FactureID == item.FactureID);
                        f.NombredUO = model.factures[i].NombredUO;
                        f.TJ = model.factures[i].TJ;
                        f.MontantHT = f.TJ*f.NombredUO;
                        f.TVA = (float)Int32.Parse(Request.Form[str]) / 100;
                        facture.NombredUO += f.NombredUO;
                        facture.TJ += f.TJ;
                        facture.MontantHT += f.MontantHT;
                        db.SaveChanges();
                        i++;
                    }
                    

                }

                
            }
            else
            {
                facture.NombredUO = model.Quantite;
                facture.TJ = model.HTunitaire;
                facture.TVA = (float)Int32.Parse(Request.Form["TVA"]) / 100;
                facture.MontantHT = model.HTunitaire * model.Quantite;
            }

            
            db.SaveChanges();

            return RedirectToAction("Detail", "Facture", new { @id = model.ID });
        }

            [Route("Facture/CreerFact", Name = "CreerFact")]
        public ActionResult CreerFact()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ViewBag.TVA = db.Infos.ToList().Count == 0 ? 0 : db.Infos.Single().TVA;

            FactureCreationViewModel model = new FactureCreationViewModel()
            {
                Mention = db.Infos.ToList().Count==0 ? " " : db.Infos.Single().Mention,
            };

            return View(model);
        }

        [Route("Facture/CreerFact")]
        [HttpPost]
        public ActionResult CreerFact(FactureCreationViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var fl = db.Factures.ToList();
            int id = 0;
            foreach (var item in fl)
            {
                if (item.FactureID > id)
                {
                    id = item.FactureID;
                }
            }
            id++;

            string type = Request.Form["Type"] == "Facture" ? "Fact" : "FactAvoir";
            var namefacture = Request.Form["ClientName"];

            foreach (var item in db.Subsidiaries.ToList())
            {
                if (item.Name == namefacture)
                {
                    type = type + "Int";
                    break;
                }
            }

            string nomMission = Request.Form["Mission"];
            var missionItem = db.Missions.Single(m => m.Name == nomMission);


            DateTime reglement = DateTime.Now;
            switch (missionItem.Delai)
            {
                case "30 jours":
                    reglement = db.MonthActivations.Single().Periode.AddDays(30);
                    break;
                case "30 jours fin de mois":
                    reglement = db.MonthActivations.Single().Periode.AddDays(30);
                    reglement = DateTime.Parse(reglement.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
                    break;
                case "45 jours":
                    reglement = db.MonthActivations.Single().Periode.AddDays(45);
                    break;
                case "45 jours fin de mois":
                    reglement = db.MonthActivations.Single().Periode.AddDays(45);
                    reglement = DateTime.Parse(reglement.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
                    break;
                case "60 jours":
                    reglement = db.MonthActivations.Single().Periode.AddDays(60);
                    break;
                case "60 jours fin de mois":
                    reglement = db.MonthActivations.Single().Periode.AddDays(60);
                    reglement = DateTime.Parse(reglement.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
                    break;
                default:
                    break;
            }

            Facture facture = new Facture()
            {
                mission = Request.Form["Mission"],
                FactureID = id,
                NomFacture = model.FactureName==null? "FAE-" + id : model.FactureName,
                MoisDeFacturation = db.MonthActivations.Single().Periode,
                InfoFacturation = model.FactInfo,
                PrincipalBC = Request.Form["NomEmettrice"],
                AdresseBC = Request.Form["AdresseEmettrice"],
                Client = Request.Form["ClientName"],
                AdresseFacturation = Request.Form["ClientAdresse"],
                NombredUO = model.Quantite,
                TJ = model.HTunitaire,
                TVA = (float)model.TVA/100,
                MontantHT = model.HTunitaire* model.Quantite,
                type = Request.Form["Type"],
                FAE = true,
                Emise = false,
                payee = false,
                annulee = false,
                DernierEnregistrer = DateTime.Now,
                Delai = missionItem.Delai,
                DesignationFacturation = missionItem.DesignationFacturation,
                DateRegelement = reglement,
                CraId=-1,
            };

            db.Factures.Add(facture);
            db.SaveChanges();

            return RedirectToAction("Factures", "Facture");
        }

        [Route("Facture/Detail", Name = "Detail")]
        public ActionResult Detail(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            try { 
            var facture = db.Factures.Single(f => f.FactureID == id);
            var sub = db.Subsidiaries.Single(s => s.Name == facture.PrincipalBC);
            string clientInfo = "";
            string contactClient = "";
            var slist = db.Subsidiaries.ToList();
            var clist = db.Companies.ToList();

            ViewBag.NomEmettrice = sub.Name;
            ViewBag.AdresseEmettrice = sub.Address;
            ViewBag.VilleEmettrice = sub.PostaleCode + " " + sub.City;
            ViewBag.MailEmettrice = sub.email;
            ViewBag.TVA = facture.TVA * 100;

            foreach (var s in slist)
            {
                if (s.Name==facture.Client)
                {
                    ViewBag.ClientAdresse = s.Address;
                    ViewBag.ClientVille = s.PostaleCode + " " + s.City;
                    clientInfo = s.Address + Environment.NewLine + s.PostaleCode + " " + s.City;
                    contactClient = s.email;
                }
            }
            foreach (var c in clist)
            {
                if (c.Name == facture.Client)
                {
                    ViewBag.ClientAdresse = c.Address;
                    ViewBag.ClientVille = c.PostalCode + " " + c.City;
                    clientInfo = c.Address + Environment.NewLine + c.PostalCode + " " + c.City;
                    contactClient = c.MailFacturation;
                }
            }
           
            string emetInfo = facture.PrincipalBC + Environment.NewLine + facture.AdresseBC;
            emetInfo = emetInfo + Environment.NewLine + sub.PostaleCode + " " + sub.City +  Environment.NewLine;

            ViewBag.date = facture.MoisDeFacturation.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);

            var mission = db.Missions.Single(m => m.Name == facture.mission);
            FacturePDFViewModel model = new FacturePDFViewModel() {
                ID = id,
                EmetInfo = emetInfo,
                Siren = sub.Siren,
                ClientName = facture.Client,
                ClientInfo = clientInfo,
                ClientContact = contactClient,
                FactureName = facture.NomFacture,
                FactInfo = facture.InfoFacturation,
                Quantite = facture.NombredUO,
                HTunitaire = facture.TJ,
                TVA = facture.TVA,
                type = facture.type,
                dateReglement = facture.DateRegelement.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture),
                IBAN = sub.IBAN,
                BIC = sub.BIC,
                TVAIntra = sub.TVAIntra,
                Mention = facture.mention,
                Designation = mission.DesignationFacturation,
                Reference = facture.reference,
                ReferenceBancaires = facture.referenceBancaire,

            };


            return View(model);
            }
            catch (Exception) { return RedirectToAction("Factures"); }
        }

        [Route("Facture/apercu", Name = "apercu")]
        public ActionResult apercu(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var facture = db.Factures.Single(f => f.FactureID == id);
            var sub = db.Subsidiaries.Single(s => s.Name == facture.PrincipalBC);
            string clientInfo = "";
            string contactClient = "";
            var slist = db.Subsidiaries.ToList();
            var clist = db.Companies.ToList();

            ViewBag.NomEmettrice = sub.Name;
            ViewBag.AdresseEmettrice = sub.Address;
            ViewBag.VilleEmettrice = sub.PostaleCode + " " + sub.City;
            ViewBag.MailEmettrice = sub.email;
            ViewBag.TVA = facture.TVA * 100;

            foreach (var s in slist)
            {
                if (s.Name == facture.Client)
                {
                    ViewBag.ClientAdresse = s.Address;
                    ViewBag.ClientVille = s.PostaleCode + " " + s.City;
                    clientInfo = s.Address + Environment.NewLine + s.PostaleCode + " " + s.City;
                    contactClient = s.email;
                }
            }
            foreach (var c in clist)
            {
                if (c.Name == facture.Client)
                {
                    ViewBag.ClientAdresse = c.Address;
                    ViewBag.ClientVille = c.PostalCode + " " + c.City;
                    clientInfo = c.Address + Environment.NewLine + c.PostalCode + " " + c.City;
                    contactClient = c.MailFacturation;
                }
            }

            string emetInfo = facture.PrincipalBC + Environment.NewLine + facture.AdresseBC;
            emetInfo = emetInfo + Environment.NewLine + sub.PostaleCode + " " + sub.City + Environment.NewLine;

            ViewBag.date = facture.MoisDeFacturation.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);

            var mission = db.Missions.Single(m => m.Name == facture.mission);
            FacturePDFViewModel model = new FacturePDFViewModel()
            {
                ID = id,
                EmetInfo = emetInfo,
                Siren = sub.Siren,
                ClientName = facture.Client,
                ClientInfo = clientInfo,
                ClientContact = contactClient,
                FactureName = facture.NomFacture,
                FactInfo = facture.InfoFacturation,
                Quantite = facture.NombredUO,
                HTunitaire = facture.TJ,
                TVA = facture.TVA,
                type = facture.type,
                dateReglement = facture.DateRegelement.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture),
                IBAN = sub.IBAN,
                BIC = sub.BIC,
                TVAIntra = sub.TVAIntra,
                Mention = db.Infos.ToList().Count == 0 ? "" : db.Infos.Single().Mention,
                Designation = mission.DesignationFacturation,
                Reference = facture.reference,
                ReferenceBancaires = facture.referenceBancaire,

            };


            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Export(string GridHtml)
        {
            
            string st = Request.Form["status"];
            string Tomail = Request.Form["destinataire"];
            string Objet = Request.Form["objet"];
            string Content = Request.Form["content"];
            string expediteur = Request.Form["expediteur"];
            

            string[] contents = Content.Split('\n');
            foreach (var s in contents)
            { 
                if(s==contents[0])
                {
                    Content = s + "<br/>";
                }
                else
                {
                    Content = Content + s + "<br/>";
                }
                     
            }


            int id = Int32.Parse(Request.Form["id"]);
            ApplicationDbContext db = new ApplicationDbContext();
            var facture = db.Factures.Single(f => f.FactureID == id);
            facture.Emise = true;
            if (facture.type != "Avoir" && facture.CraId>0)
            {
                var cra = db.Cras.Single(c => c.CraID == facture.CraId);
                cra.Changeable = false;
                db.SaveChanges();
            }
            

            var sub = db.Subsidiaries.Single(s => s.Name == facture.PrincipalBC);

            if(facture.CraId>0 || facture.CraId==-999)
            {
                    facture.NomFacture = "FA-" + DateTime.Now.ToString("yyyy-MM") + "-" + sub.FactureID;

            }
            

            sub.FactureID++;
            db.SaveChanges();


            //
            var suivilist = db.Suivis.ToList();
            foreach(var s in suivilist)
            {
                if(s.NomMission==facture.mission)
                {
                    s.statu = "Facture";
                }
            }
            db.SaveChanges();




            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                string arialuniTff = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");

                string imageURL = Server.MapPath(".") + "/../Images/B2DIGIT_Facture.png";
                if (facture.PrincipalBC== "DMO Conseil")
                {
                    string[] strs = facture.PrincipalBC.Split(' ');
                    if (strs.Length > 1)
                    {
                        int i = 0;
                        foreach (var s in strs)
                        {
                            if (i != 0)
                            {
                                strs[0] = strs[0] + "_" + s;
                            }
                            i++;
                        }
                    }

                    imageURL = Server.MapPath(".") + "/../Images/" + strs[0] + "_Facture.png";
                }
                

                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);

                //Resize image depend upon your need

                jpg.ScaleToFit(140f, 120f);

                //Give space before image

                jpg.SpacingBefore = 14f;

                //Give some space after the image

                jpg.SpacingAfter = 1f;

                jpg.Alignment = Element.ALIGN_LEFT;


                pdfDoc.Add(jpg);

                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                PdfContentByte cb = writer.DirectContent;
                cb.BeginText();

                cb.SetFontAndSize(BaseFont.CreateFont(), 10f);
                // we draw some text on a certain position
                cb.SetTextMatrix(10, 40);
                cb.ShowText(facture.mention);

                // we tell the contentByte, we've finished drawing text
                cb.EndText();

                pdfDoc.Close();

                
                

                string clientName = facture.Client;
                string mail = "Rien";
                foreach (var c in db.Companies.ToList())
                {
                    if (c.Name == clientName)
                    {
                        mail = c.MailFacturation;
                        break;
                    }
                }
                if (mail=="Rien")
                {
                    foreach (var s in db.Subsidiaries.ToList())
                    {
                        if (s.Name == clientName)
                        {
                            mail = s.email;
                            break;
                        }
                    }
                }

                var entreprise = db.Subsidiaries.Single(s=>s.Name==facture.PrincipalBC);

                string motdepasse = "";
                if (mail != "Rien" && mail != null && st == "true")
                {
                    motdepasse = Request.Form["mdp"];
                    string mdp = Projet_pilate.Helper.Helper.EncodePassword(motdepasse, entreprise.VCode);
                    if (! entreprise.motdepasse.Equals(mdp) )
                    {
                        var Sub = db.Subsidiaries.Single(s => s.Name == facture.PrincipalBC);
                        string clientInfo = "";
                        string contactClient = "";
                        var slist = db.Subsidiaries.ToList();
                        var clist = db.Companies.ToList();

                        ViewBag.NomEmettrice = Sub.Name;
                        ViewBag.AdresseEmettrice = Sub.Address;
                        ViewBag.VilleEmettrice = Sub.PostaleCode + " " + Sub.City;
                        ViewBag.MailEmettrice = Sub.email;
                        ViewBag.TVA = facture.TVA * 100;

                        foreach (var s in slist)
                        {
                            if (s.Name == facture.Client)
                            {
                                ViewBag.ClientAdresse = s.Address;
                                ViewBag.ClientVille = s.PostaleCode + " " + s.City;
                                clientInfo = s.Address + Environment.NewLine + s.PostaleCode + " " + s.City;
                                contactClient = s.email;
                            }
                        }
                        foreach (var c in clist)
                        {
                            if (c.Name == facture.Client)
                            {
                                ViewBag.ClientAdresse = c.Address;
                                ViewBag.ClientVille = c.PostalCode + " " + c.City;
                                clientInfo = c.Address + Environment.NewLine + c.PostalCode + " " + c.City;
                                contactClient = c.MailFacturation;
                            }
                        }

                        string emetInfo = facture.PrincipalBC + Environment.NewLine + facture.AdresseBC;
                        emetInfo = emetInfo + Environment.NewLine + Sub.PostaleCode + " " + Sub.City + Environment.NewLine;

                        ViewBag.date = facture.MoisDeFacturation.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);

                        var mission = db.Missions.Single(m => m.Name == facture.mission);
                        FacturePDFViewModel model = new FacturePDFViewModel()
                        {
                            ID = id,
                            EmetInfo = emetInfo,
                            Siren = Sub.Siren,
                            ClientName = facture.Client,
                            ClientInfo = clientInfo,
                            ClientContact = contactClient,
                            FactureName = facture.NomFacture,
                            FactInfo = facture.InfoFacturation,
                            Quantite = facture.NombredUO,
                            HTunitaire = facture.TJ,
                            TVA = facture.TVA,
                            type = facture.type,
                            dateReglement = facture.DateRegelement.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture),
                            IBAN = Sub.IBAN,
                            BIC = Sub.BIC,
                            TVAIntra = Sub.TVAIntra,
                            Mention = db.Infos.ToList().Count == 0 ? "" : db.Infos.Single().Mention,
                            Designation = mission.DesignationFacturation,
                            Reference = facture.reference,
                            ReferenceBancaires = facture.referenceBancaire,

                        };
                        string message = "C'est pas le correct mot de passe !";
                        ModelState.AddModelError(string.Empty, message);

                        return View("Detail", model);
                    }

                    MemoryStream ms = new MemoryStream(stream.ToArray());
                    System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                    System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(ms, ct);
                    attach.ContentDisposition.FileName = facture.NomFacture + ".pdf";

                    GMailer.GmailUsername = expediteur;
                    GMailer.GmailPassword = motdepasse;
                    GMailer mailer = new GMailer();
                    mailer.ToEmail = Tomail;
                    mailer.Subject = Objet;
                    mailer.Body = Content;
                    mailer.IsHtml = true;
                    mailer.attch = attach;
                    try
                    {
                        mailer.Send();
                    }catch (Exception)
                    {
                        var Sub = db.Subsidiaries.Single(s => s.Name == facture.PrincipalBC);
                        string clientInfo = "";
                        string contactClient = "";
                        var slist = db.Subsidiaries.ToList();
                        var clist = db.Companies.ToList();

                        ViewBag.NomEmettrice = Sub.Name;
                        ViewBag.AdresseEmettrice = Sub.Address;
                        ViewBag.VilleEmettrice = Sub.PostaleCode + " " + Sub.City;
                        ViewBag.MailEmettrice = Sub.email;
                        ViewBag.TVA = facture.TVA * 100;

                        foreach (var s in slist)
                        {
                            if (s.Name == facture.Client)
                            {
                                ViewBag.ClientAdresse = s.Address;
                                ViewBag.ClientVille = s.PostaleCode + " " + s.City;
                                clientInfo = s.Address + Environment.NewLine + s.PostaleCode + " " + s.City;
                                contactClient = s.email;
                            }
                        }
                        foreach (var c in clist)
                        {
                            if (c.Name == facture.Client)
                            {
                                ViewBag.ClientAdresse = c.Address;
                                ViewBag.ClientVille = c.PostalCode + " " + c.City;
                                clientInfo = c.Address + Environment.NewLine + c.PostalCode + " " + c.City;
                                contactClient = c.MailFacturation;
                            }
                        }

                        string emetInfo = facture.PrincipalBC + Environment.NewLine + facture.AdresseBC;
                        emetInfo = emetInfo + Environment.NewLine + Sub.PostaleCode + " " + Sub.City + Environment.NewLine;

                        ViewBag.date = facture.MoisDeFacturation.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);

                        var mission = db.Missions.Single(m => m.Name == facture.mission);
                        FacturePDFViewModel model = new FacturePDFViewModel()
                        {
                            ID = id,
                            EmetInfo = emetInfo,
                            Siren = Sub.Siren,
                            ClientName = facture.Client,
                            ClientInfo = clientInfo,
                            ClientContact = contactClient,
                            FactureName = facture.NomFacture,
                            FactInfo = facture.InfoFacturation,
                            Quantite = facture.NombredUO,
                            HTunitaire = facture.TJ,
                            TVA = facture.TVA,
                            type = facture.type,
                            dateReglement = facture.DateRegelement.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture),
                            IBAN = Sub.IBAN,
                            BIC = Sub.BIC,
                            TVAIntra = Sub.TVAIntra,
                            Mention = db.Infos.ToList().Count == 0 ? "" : db.Infos.Single().Mention,
                            Designation = mission.DesignationFacturation,
                            Reference = facture.reference,
                            ReferenceBancaires = facture.referenceBancaire,

                        };
                        string message = "Echec d'envoi du mail !";
                        ModelState.AddModelError(string.Empty, message);

                        return View("Detail" ,model);
                        
                    }
                    
                }

                
                

                //return File(stream.ToArray(), "application/pdf", m[0]+".pdf");
                return File(stream.ToArray(), "application/pdf", facture.NomFacture + ".pdf");
            }
            /*OngletViewModel model = new OngletViewModel()
            {
                first = "",
                seconde = "active",
                third = "",
            };
            return View("Factures", model);*/
        }


        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export2(string GridHtml)
        {
            int id = Int32.Parse(Request.Form["id"]);
            ApplicationDbContext db = new ApplicationDbContext();
            var facture = db.Factures.Single(f => f.FactureID == id);

            string pattern = @"<p style=""font-family:Calibri""><strong style=""font-family:Calibri"">";
            string[] mc = Regex.Split(GridHtml, pattern);
            string[] m = Regex.Split(mc[1], @"</strong></p>");
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                string arialuniTff = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");

                
                string imageURL = Server.MapPath(".") + "/../Images/B2DIGIT_Facture.png";
                if (facture.PrincipalBC == "DMO Conseil")
                {
                    string[] strs = facture.PrincipalBC.Split(' ');
                    if (strs.Length > 1)
                    {
                        int i = 0;
                        foreach (var s in strs)
                        {
                            if (i != 0)
                            {
                                strs[0] = strs[0] + "_" + s;
                            }
                            i++;
                        }
                    }

                    imageURL = Server.MapPath(".") + "/../Images/" + strs[0] + "_Facture.png";
                }

                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);

                //Resize image depend upon your need

                jpg.ScaleToFit(140f, 120f);


                //Give some space after the image
                jpg.SpacingBefore = 10f;
                jpg.SpacingAfter = 2f;

                jpg.Alignment = Element.ALIGN_LEFT;


                pdfDoc.Add(jpg);

                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);


                // we grab the ContentByte and do some stuff with it
                PdfContentByte cb = writer.DirectContent;

                // we tell the ContentByte we're ready to draw text
                cb.BeginText();

                cb.SetFontAndSize(BaseFont.CreateFont() ,10f);
                // we draw some text on a certain position
                cb.SetTextMatrix(10, 40);
                cb.ShowText(facture.mention);

                // we tell the contentByte, we've finished drawing text
                cb.EndText();


                pdfDoc.Close();

               
                return File(stream.ToArray(), "application/pdf", m[0] + ".pdf");
            }

        }

        public ActionResult Annulee(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var facture = db.Factures.Single(f => f.FactureID == id);
            facture.annulee = true;


            var sub = db.Subsidiaries.Single(s => s.Name == facture.PrincipalBC);
            int Id = 0;
            foreach (var item in db.Factures.ToList())
            {
                if (item.FactureID > Id)
                {
                    Id = item.FactureID;
                }
            }
            Id++;
            Facture factureAvoir = new Facture()
            {
                mission = facture.mission,
                FactureID = Id,
                NomFacture = "FAE-Avoir-"+Id,
                MoisDeFacturation = DateTime.Now,
                InfoFacturation = facture.InfoFacturation,
                PrincipalBC = facture.PrincipalBC,
                AdresseBC = facture.AdresseBC,
                Client = facture.Client,
                AdresseFacturation = facture.AdresseFacturation,
                NombredUO = facture.NombredUO,
                TJ = -facture.TJ,
                TVA = facture.TVA,
                MontantHT = -facture.MontantHT,
                FAE = true,
                Emise = false,
                payee = false,
                annulee = false,
                DernierEnregistrer = DateTime.Now,
                Delai = facture.Delai,
                DesignationFacturation = facture.DesignationFacturation,
                DateRegelement = facture.DateRegelement,
                type = "Avoir",
                

            };
            db.Factures.Add(factureAvoir);
            sub.FactureID++;
            db.SaveChanges();

            OngletViewModel model = new OngletViewModel()
            {
                first = "",
                seconde = "active",
                third = "",
            };

            return View("Factures", model);

        }


        public ActionResult Payee(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var facture = db.Factures.Single(f => f.FactureID == id);
            facture.payee = true;
            
            db.SaveChanges();

            OngletViewModel model = new OngletViewModel()
            {
                first = "",
                seconde = "active",
                third = "",
            };

            return View("Factures", model);

        }

        [HttpPost]
        public ActionResult fusionner()
        {

            string selection = Request["select"]==null?"": Request["select"].ToString();
            string type = Request["type"]==null?"":Request["type"].ToString();

            if (selection==""||type=="")
            {
                string message = "Vous devez choisir factures à combiner et la méthode de combiner !";
                ModelState.AddModelError(string.Empty, message);
                OngletViewModel model = new OngletViewModel()
                {
                    first = "active",
                    seconde = "",
                    third = "",
                };

                return View("Factures", model);
            }

            string[] selectionlist = selection.Split(',');
            string[] typelist = type.Split(',');

            if (typelist.Length > 1)
            {
                string message = "Vous pouver choisir qu'une méthode pour les combiner !";
                ModelState.AddModelError(string.Empty, message);
                OngletViewModel model = new OngletViewModel()
                {
                    first = "active",
                    seconde = "",
                    third = "",
                };
                return View("Factures", model);
            }


            ApplicationDbContext db = new ApplicationDbContext();

            Boolean able = true;
            string nomEmettrice = "";
            string client = "";
            foreach (var item in selectionlist)
            {
                int itemid = Int32.Parse(item);
                var f = db.Factures.Single(fa => fa.FactureID == itemid);
                if (nomEmettrice == "")
                {
                    nomEmettrice = f.PrincipalBC;
                    client = f.Client;
                } else
                {
                    if (f.PrincipalBC != nomEmettrice || f.Client != client)
                    {
                        able = false;
                    }
                }
            }

            if (!able)
            {
                string message = "Vous pouver pas combiner ces factures ! Vérifier que ils ont les même entreprise émettrices et clients.";
                ModelState.AddModelError(string.Empty, message);
                OngletViewModel model = new OngletViewModel()
                {
                    first = "active",
                    seconde = "",
                    third = "",
                };
                return View("Factures",model);
            }


            var fl = db.Factures.ToList();
            int id = 0;
            foreach (var item in fl)
            {
                if (item.FactureID > id)
                {
                    id = item.FactureID;
                }
            }
            id++;

            int sid = Int32.Parse(selectionlist[0]);
            var facturefusionner = db.Factures.Single(f => f.FactureID == sid);
            DateTime reglement = DateTime.Now;
            switch (facturefusionner.Delai)
            {
                case "30 jours":
                    reglement = db.MonthActivations.Single().Periode.AddDays(30);
                    break;
                case "30 jours fin de mois":
                    reglement = db.MonthActivations.Single().Periode.AddDays(30);
                    reglement = DateTime.Parse(reglement.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
                    break;
                case "45 jours":
                    reglement = db.MonthActivations.Single().Periode.AddDays(45);
                    break;
                case "45 jours fin de mois":
                    reglement = db.MonthActivations.Single().Periode.AddDays(45);
                    reglement = DateTime.Parse(reglement.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
                    break;
                case "60 jours":
                    reglement = db.MonthActivations.Single().Periode.AddDays(60);
                    break;
                case "60 jours fin de mois":
                    reglement = db.MonthActivations.Single().Periode.AddDays(60);
                    reglement = DateTime.Parse(reglement.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
                    break;
                default:
                    break;
            }

            if (typelist[0] == "fusionner")
            {


                Facture facture = new Facture()
                {
                    mission = facturefusionner.mission,
                    FactureID = id,
                    NomFacture = "FAE-" + id,
                    MoisDeFacturation = db.MonthActivations.Single().Periode,
                    InfoFacturation = facturefusionner.InfoFacturation,
                    PrincipalBC = facturefusionner.PrincipalBC,
                    AdresseBC = facturefusionner.AdresseBC,
                    Client = facturefusionner.Client,
                    AdresseFacturation = facturefusionner.AdresseFacturation,
                    NombredUO = facturefusionner.NombredUO,
                    TJ = facturefusionner.TJ,
                    TVA = facturefusionner.TVA,
                    MontantHT = facturefusionner.MontantHT,
                    type = facturefusionner.type,
                    FAE = true,
                    Emise = false,
                    payee = false,
                    annulee = false,
                    DernierEnregistrer = DateTime.Now,
                    Delai = facturefusionner.Delai,
                    DesignationFacturation = facturefusionner.DesignationFacturation,
                    DateRegelement = reglement,
                    CraId = -999,
                };

                foreach(var item in selectionlist)
                {
                    int itemid = Int32.Parse(item);
                    var f = db.Factures.Single(fa => fa.FactureID == itemid);
                    if (f.FactureID == facturefusionner.FactureID)
                    {
                        db.Factures.Remove(facturefusionner);
                        continue;
                    }
                    else
                    {
                        facture.NombredUO += f.NombredUO;
                        facture.MontantHT += f.MontantHT;
                        db.Factures.Remove(f);
                    }
                }

                db.Factures.Add(facture);
                db.SaveChanges();
            }

            if (typelist[0] == "assembler")
            {


                Facture facture = new Facture()
                {
                    mission = facturefusionner.mission,
                    FactureID = id,
                    NomFacture = "FAE-"+ id,
                    MoisDeFacturation = db.MonthActivations.Single().Periode,
                    InfoFacturation = facturefusionner.InfoFacturation,
                    PrincipalBC = facturefusionner.PrincipalBC,
                    AdresseBC = facturefusionner.AdresseBC,
                    Client = facturefusionner.Client,
                    AdresseFacturation = facturefusionner.AdresseFacturation,
                    NombredUO = facturefusionner.NombredUO,
                    TJ = facturefusionner.TJ,
                    TVA = facturefusionner.TVA,
                    MontantHT = facturefusionner.MontantHT,
                    type = facturefusionner.type,
                    FAE = true,
                    Emise = false,
                    payee = false,
                    annulee = false,
                    DernierEnregistrer = DateTime.Now,
                    Delai = facturefusionner.Delai,
                    DesignationFacturation = facturefusionner.DesignationFacturation,
                    DateRegelement = reglement,
                    CraId = -999,

                };

                foreach (var item in selectionlist)
                {
                    int itemid = Int32.Parse(item);
                    var f = db.Factures.Single(fa => fa.FactureID == itemid);
                    if (f.FactureID == facturefusionner.FactureID)
                    {
                        facturefusionner.parentID = facture.FactureID;
                        db.SaveChanges();
                        continue;
                    }
                    else
                    {
                        facture.NombredUO += f.NombredUO;
                        facture.MontantHT += f.MontantHT;
                        f.parentID = facture.FactureID;
                        db.SaveChanges();
                    }
                }

                facture.parentID = -1;
                db.Factures.Add(facture);
                db.SaveChanges();
            }


            ViewBag.first = "active";
            ViewBag.second = "";
            ViewBag.third = "";


            return RedirectToAction("Factures", "Facture");
        }

        public ActionResult FacturesPayee()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ViewBag.date = db.MonthActivations.Single().Periode;

            List<FactureSimpleViewModel> models = new List<FactureSimpleViewModel>();
            var factureList = db.Factures.ToList();

            foreach (var f in factureList)
            {
                if (f.payee == false)
                {
                    continue;
                }
                var M = db.Missions.Single(m => m.Name == f.mission);
                var C = db.Consultants.Single(c => c.ConsultantID == M.ConsultantID);
                FactureSimpleViewModel model = new FactureSimpleViewModel()
                {
                    ID = f.FactureID,
                    NomFacture = f.NomFacture,
                    Client = f.Client,
                    MontantHT = ((1+f.TVA)*f.MontantHT).ToString()+ " €",
                    Mission = f.mission,
                    Dernier = f.DernierEnregistrer,
                    Emettrice = f.PrincipalBC,
                    Consultant = C.FirstName + " " + C.LastName,
                    MoisSaisie = f.MoisDeFacturation.ToString("yyyy-MM"),
                };

                model.Status = "Payee";

                models.Add(model);

            }


            return View(models);
        }

        public ActionResult Recalculer(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var facture = db.Factures.Single(f => f.FactureID == id);

            var mission = db.Missions.Single(m => m.Name == facture.mission);
            var BC = db.Subsidiaries.Single(s => s.SubsidiaryID == mission.PrincipalBCID);
            var contact = db.CompanyContacts.Single(c => c.CompanyContactID == mission.CompanyContactID);
            var company = db.Companies.Single(c => c.CompanyID == contact.CompanyID);
            var time = DateTime.Parse(db.MonthActivations.Single().Periode.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
            var missionNames = db.Missions.Select(m => m.Name).ToList();

            facture.PrincipalBC = BC.Name;
            facture.AdresseBC = BC.Address;
            facture.Client = company.Name;
            facture.AdresseFacturation = company.Address;
            facture.TJ = mission.Fee;
            facture.MontantHT = mission.Fee * facture.NombredUO;
            facture.Delai = mission.Delai;
            facture.DesignationFacturation = mission.DesignationFacturation;
            facture.reference = mission.Reference;
            facture.referenceBancaire = BC.Name;
            facture.InfoFacturation = mission.InfoFacturation;
            var info = db.Infos.ToList().Count == 0 ? null : db.Infos.ToList()[0];
            if (info == null)
            {
                facture.TVA = (float)0.2;
                facture.mention = "Aucun";
            }
            else
            {
                facture.TVA = (float)info.TVA;
                facture.mention = info.Mention;
            }

            DateTime reglement = DateTime.Now;
            switch (mission.Delai)
            {
                case "30 jours":
                    reglement = time.AddDays(30);
                    break;
                case "30 jours fin de mois":
                    reglement = time.AddDays(30);
                    reglement = DateTime.Parse(reglement.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
                    break;
                case "45 jours":
                    reglement = time.AddDays(45);
                    break;
                case "45 jours fin de mois":
                    reglement = time.AddDays(45);
                    reglement = DateTime.Parse(reglement.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
                    break;
                case "60 jours":
                    reglement = time.AddDays(60);
                    break;
                case "60 jours fin de mois":
                    reglement = time.AddDays(60);
                    reglement = DateTime.Parse(reglement.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1);
                    break;
                default:
                    break;
            }
            facture.DateRegelement = reglement;
            db.SaveChanges();

            return RedirectToAction("Detail", "Facture", new { @id = id });

        }



    }

    public class GMailer
    {
        public static string GmailUsername { get; set; }
        public static string GmailPassword { get; set; }
        public static string GmailHost { get; set; }
        public static int GmailPort { get; set; }
        public static bool GmailSSL { get; set; }

        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string PathToAttachment { get; set; }
        public Attachment attch { get; set; }
        public bool IsHtml { get; set; }

        static GMailer()
        {
            GmailHost = "smtp.gmail.com";
            GmailPort = 25; // Gmail can use ports 25, 465 & 587; but must be 25 for medium trust environment.
            GmailSSL = true;
        }

        public void Send()
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Host = GmailHost;
            smtp.Port = GmailPort;
            smtp.EnableSsl = GmailSSL;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(GmailUsername, GmailPassword);

            using (var message = new MailMessage(GmailUsername, ToEmail))
            {
                message.Subject = Subject;
                message.Body = Body;
                message.IsBodyHtml = IsHtml;
                message.Attachments.Add(attch);
                smtp.Send(message);
            }
        }
    }
}
