using Foolproof;
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
        public List<int> OwnersID { get; set; }
        public List<string> PartOwners { get; set; }
        public List<string> FatherProfitCenters { get; set; }

    }

    public class DetailProfitCenterViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Libellé")]
        public string Name { get; set; }

        [Display(Name = "Proprietaire")]
        public string Owners { get; set; }

        [Display(Name = "Co-proprietaire")]
        public string PartOwners { get; set; }

        [Display(Name = "Centre père")]
        public string FatherProfitCenters { get; set; }


        [DataType(DataType.Currency)]
        [Display(Name = "Coût")]
        public double Cost { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Chiffre d'affaire")]
        public double Turnover { get; set; }


    }
    public class UpdateProfitCenterViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Libellé")]
        public string Name { get; set; }

        [Display(Name = "Proprietaire")]
        public string Owner { get; set; }

        [Display(Name = "Co-proprietaire")]
        public string PartOwner { get; set; }

        [Display(Name = "Centre père")]
        //public string FatherProfitCenter { get; set; }
        public int? FatherProfitCenterID { get; set; }
        public string FatherProfitCenter { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Coût")]
        public double Cost { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Chiffre d'affaire")]
        public double Turnover { get; set; }

        public List<string> ListOwners { get; set; }
        public List<int> ListOwnersID { get; set; }
        public List<string> ListPartOwners { get; set; }
        public List<string> ListFatherProfitCenters { get; set; }


    }

    public class SuiviDetailViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Consultant")]
        public string Consultant { get; set; }

        [Display(Name = "Mission")]
        public string Mission { get; set; }

        [Display(Name = "Client")]
        public string Client { get; set; }

        [Display(Name = "NBJ")]
        public float Nbj { get; set; }

        [Display(Name = "TJ")]
        public float Tj { get; set; }

        [Display(Name = "CA")]
        public float CA { get; set; }

        public float Cout { get; set; }

    }

    public class PeriodeViewModel
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime debut { get; set; }

        [DataType(DataType.Date)]
        [GreaterThan("debut", ErrorMessage = "La date de fin ne peut être inférieure ou égale à la date de début de recherche !")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime fin { get; set; }
    }
 }