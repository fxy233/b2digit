using Projet_pilate.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projet_pilate.Models
{
    public class ProfitCenterViewModel
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Le nom ne peut pas comprendre plus de 50 caractères")]
        [Display(Name = "Libellé")]
        public string Name { get; set; }

        public List<string> Owners { get; set; }
        public List<string> PartOwners { get; set; }
        public List<string> FatherProfitCenters { get; set; }

    }


    public class DetailProfitCenterViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Libellé")]
        public string Name { get; set; }

        [Display(Name = "Proprietaire")]
        public string Owner { get; set; }

        [Display(Name = "Co-proprietaire")]
        public string PartOwner { get; set; }

        [Display(Name = "Centre père")]
        public string FatherProfitCenter { get; set; }


        [DataType(DataType.Currency)]
        [Display(Name = "Coût")]
        public double Cost { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Chiffre d'affaire")]
        public double Turnover { get; set; }


    }
}