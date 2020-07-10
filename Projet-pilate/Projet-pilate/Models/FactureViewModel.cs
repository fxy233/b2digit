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

        [Display(Name = "FAE")]
        public string FAE { get; set; }

        [Display(Name = "Emise")]
        public string Emise { get; set; }

        [Display(Name = "Payee")]
        public string Payee { get; set; }

        [Display(Name = "Annulee")]
        public string Annulee { get; set; }

    }

}