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
        public ActionResult Modifier(int id)
        {
                        ApplicationDbContext db = new ApplicationDbContext();
                        var facture = db.Factures.Single(f => f.FactureID == id);
                        ViewBag.TVA = db.Infos.ToList().Count == 0 ? 0 : db.Infos.Single().TVA;

                        FactureCreationViewModel model = new FactureCreationViewModel();
                        model.ID = facture.FactureID;
                        model.NomEmettrice = facture.PrincipalBC;
                        var emettrice = db.Subsidiaries.Single(s => s.Name == model.NomEmettrice);
                        model.AdresseEmettrice = emettrice.Address;
                        model.VilleEmettrice = emettrice.PostaleCode + ", " + emettrice.City;
                        model.MailEmettrice = emettrice.email;
                        model.Siren = emettrice.Siren;
                        model.ClientName = facture.Client;
                        
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
                        model.TVA = facture.TVA;
                        model.totalHT = facture.NombredUO * facture.TJ;
                        model.montantTVA = facture.TVA * facture.NombredUO * facture.TJ;
                        model.totalTTC = (1 + facture.TVA) * facture.NombredUO * facture.TJ;
                        model.dateReglement = facture.MoisDeFacturation;
                        model.IBAN = emettrice.IBAN;
                        model.BIC = emettrice.BIC;
                        model.TVAintra = emettrice.TVAIntra;
                        model.Mention = db.Infos.Single().Mention;
                        model.mission = facture.mission;
            model.dateReglement = facture.DateRegelement;
                        return View(model);
            
        }

        [Route("Facture/Modifier")]
        [HttpPost]
        public ActionResult Modifier(FactureCreationViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var facture = db.Factures.Single(f => f.NomFacture == model.FactureName);

            string type = Request.Form["Type"] == "facture" ? "Fact" : "FactAvoir";
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


            facture.mission = Request.Form["Mission"];
            facture.MoisDeFacturation = db.MonthActivations.Single().Periode;
            facture.InfoFacturation = model.FactInfo;
            facture.PrincipalBC = Request.Form["NomEmettrice"];
            facture.AdresseBC = Request.Form["AdresseEmettrice"];
            facture.Client = Request.Form["ClientName"];
            facture.AdresseFacturation = Request.Form["ClientAdresse"];
                facture.NombredUO = model.Quantite;
                facture.TJ = model.HTunitaire;
                facture.TVA = (float)model.TVA / 100;
                facture.MontantHT = model.HTunitaire * model.Quantite;
                facture.DernierEnregistrer = DateTime.Now;
                facture.Delai = missionItem.Delai;
                facture.DesignationFacturation = missionItem.DesignationFacturation;
            facture.DateRegelement = model.dateReglement;
            
            db.SaveChanges();

            return RedirectToAction("ListeFactures", "Facture");
        }

            [Route("Facture/CreerFact", Name = "CreerFact")]
        public ActionResult CreerFact()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ViewBag.TVA = db.Infos.ToList().Count == 0 ? 0 : db.Infos.Single().TVA;


            return View();
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

            Facture facture = new Facture()
            {
                mission = Request.Form["Mission"],
                FactureID = id,
                NomFacture = type + "-" + Request.Form["NomEmettrice"] + "-" + db.MonthActivations.Single().Periode.ToString("yyyy-MMM", System.Globalization.CultureInfo.CurrentCulture) + "-" + id,
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
                FAE = false,
                Emise = false,
                payee = false,
                annulee = false,
                DernierEnregistrer = DateTime.Now,
                Delai = missionItem.Delai,
                DesignationFacturation = missionItem.DesignationFacturation,
                DateRegelement = model.dateReglement,
            };

            db.Factures.Add(facture);
            db.SaveChanges();

            return RedirectToAction("ListeFactures", "Facture");
        }

        [Route("Facture/Detail", Name = "Detail")]
        public ActionResult Detail(int id)
        {
            ViewBag.test = "test";
            ApplicationDbContext db = new ApplicationDbContext();
            var facture = db.Factures.Single(f => f.FactureID == id);
            var sub = db.Subsidiaries.Single(s => s.Name == facture.PrincipalBC);
            var client = "";
            string clientInfo = "";
            string contactClient = "";
            var slist = db.Subsidiaries.ToList();
            var clist = db.Companies.ToList();
            foreach(var s in slist)
            {
                if (s.Name==facture.Client)
                {
                    clientInfo = s.Address + Environment.NewLine + s.PostaleCode + " " + s.City;
                    contactClient = s.email;
                }
            }
            foreach (var c in clist)
            {
                if (c.Name == facture.Client)
                {
                    clientInfo = c.Address + Environment.NewLine + c.PostalCode + " " + c.City;
                    contactClient = db.CompanyContacts.Single(t=>t.CompanyID==c.CompanyID).Mail;
                }
            }
           
            string emetInfo = facture.PrincipalBC + Environment.NewLine + facture.AdresseBC;
            emetInfo = emetInfo + Environment.NewLine + sub.PostaleCode + " " + sub.City +  Environment.NewLine;

            ViewBag.date = DateTime.Now.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);

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
                Mention = db.Infos.ToList().Count==0?"":db.Infos.Single().Mention,
                Designation = mission.DesignationFacturation,

            };


            return View(model);
        }


        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string GridHtml)
        {

            /*Document document = new Document(PageSize.A4);
            byte[] result;
            List<string> cssFiles = new List<string>();
            cssFiles.Add(@"/Content/bootstrap.css");
            using (var ms = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                writer.CloseStream = false;
                document.Open();
                HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);
                htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());

                ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
                cssFiles.ForEach(i => cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath(i), true));

                IPipeline pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, writer)));

                XMLWorker worker = new XMLWorker(pipeline, true);
                XMLParser xmlParser = new XMLParser(worker);
                xmlParser.Parse(new MemoryStream(Encoding.UTF8.GetBytes(GridHtml)));


                var mimeType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "inline; filename=" + "test.pdf");
                return File(ms.GetBuffer(), mimeType);
            }*/


            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                string arialuniTff = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");


                string imageURL = Server.MapPath(".") + "/../Images/logo transparent.png";

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
                pdfDoc.Close();


                return File(stream.ToArray(), "application/pdf", "Grid.pdf");
            }
            
        }

    }
}
