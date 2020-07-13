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

        [Display(Name = "Nom du Facture")]
        public string NomFacture { get; set; }

        [Display(Name = "Client")]
        public string Client { get; set; }

        [Display(Name = "Montant")]
        public string MontantHT { get; set; }

        [Display(Name = "Mission")]
        public string Mission { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Dernier Enregistrement")]
        public DateTime Dernier { get; set; }

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



    }
}