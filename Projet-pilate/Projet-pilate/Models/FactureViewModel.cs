using Projet_pilate.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projet_pilate.Models
{
    public class FactureSimpleViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Nom de la facture")]
        public string NomFacture { get; set; }

        [Display(Name = "Client")]
        public string Client { get; set; }

        [Display(Name = "TTC")]
        public string MontantHT { get; set; }

        [Display(Name = "Mission")]
        public string Mission { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Dernier Enregistrement")]
        public DateTime Dernier { get; set; }

        [Display(Name = "Société émettrice")]
        public string Emettrice { get; set; }

        [Display(Name = "Consultant")]
        public string Consultant { get; set; }

        [Display(Name = "Mois de saisie")]
        public string MoisSaisie { get; set; }
        public DateTime date { get; set; }
    }

    public class FacturePDFViewModel
    {
        public int ID { get; set; }
        public string EmetInfo { get; set; }
        public string Siren { get; set; }
        public string ClientName { get; set; }
        public string ClientInfo { get; set; }
        public string ClientContact { get; set; }
        public string FactureName { get; set; }
        public string FactInfo { get; set; }
        public float Quantite { get; set; }
        public float HTunitaire { get; set; }
        public float TVA { get; set; }

        public string type { get; set; }
        public string dateReglement { get; set; }
        public string IBAN { get; set; }
        public string BIC { get; set; }
        public string TVAIntra { get; set; }
        public string Mention { get; set; }
        public string Designation { get; set; }
        public string Reference { get; set; }
        public string ReferenceBancaires { get; set; }


    }

    public class InfoViewModel
    {
        [Display(Name = "TVA(%)")]
        public double TVA{ get; set; }

        [Display(Name = "Mention obligatoire")]
        public string mention { get; set; }
        
        [Display(Name = "Temps pour passer au historique")]
        public int historique { get; set; }
    }

    public class OngletViewModel
    {
        public string first { get; set; }
        public string seconde { get; set; }
        public string third { get; set; }
    }

    public class FactureCreationViewModel
    {
        public int ID { get; set; }
        public string NomEmettrice { get; set; }
        public string AdresseEmettrice { get; set; }
        public string VilleEmettrice { get; set; }
        public string MailEmettrice { get; set; }
        public string Siren { get; set; }
        public string ClientName { get; set; }
        public string ClientAdresse { get; set; }
        public string ClientVille { get; set; }
        public string ClientContact { get; set; }
        public string type { get; set; }
        
        public string FactureName { get; set; }
        public string FactInfo { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime date { get; set; }
        public string designation { get; set; }
        public float Quantite { get; set; }
        public float HTunitaire { get; set; }
        public float totalHT { get; set; }
        public float montantTVA { get; set; }
        public float totalTTC { get; set; }

        public double TVA { get; set; }
        public string IBAN { get; set; }
        public string BIC { get; set; }
        public string TVAintra { get; set; }
        public string Mention { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime dateReglement { get; set; }

        public string mission { get; set; }

        public string status { get; set; }
        public string Reference { get; set; }
        public string ReferenceBancaire { get; set; }

        public List<Facture> factures { get; set; }
    }

}