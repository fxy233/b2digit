using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projet_pilate.Models
{
    public class ConsultantViewModel
    {
        public int ID { get; set; }
        public string UserID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas comprendre plus de 50 caractères")]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Le nom ne peut pas comprendre plus de 50 caractères")]
        [Display(Name = "Nom")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date d'entrée")]
        public DateTime EntryDate { get; set; }

        public string Status { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Coût")]
        public double DailyCost { get; set; }

        public List<string> ProfitCenters { get; set; }
    }


    public class RegisterConsultantViewModel
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas comprendre plus de 50 caractères")]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Le nom ne peut pas comprendre plus de 50 caractères")]
        [Display(Name = "Nom")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date d'entrée")]
        public DateTime EntryDate { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Coût")]
        public double Cost { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Frais de repas")]
        public double MealCost { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Forfait de déplacement")]
        public double TravelPackage { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Coût exceptionnel")]
        public double ExceptionalCost { get; set; }

        public List<string> ProfitCenters { get; set; }

        public List<string> Subsidiaries { get; set; }
    }


    public class DetailConsultantViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Prénom")]
        public string FirstName { get; set; }

        [Display(Name = "Nom")]
        public string LastName { get; set; }

        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date d'entrée")]
        public DateTime EntryDate { get; set; }

        [Display(Name = "Statut")]
        public string Status { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Coût journalier")]
        public double DailyCost { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Coût mensuel")]
        public double MonthlyCost { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Repas")]
        public double MealCost { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Forfait déplacement")]
        public double TravelPackage { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Coût exceptionnel")]
        public double ExceptionalCost { get; set; }

        [Display(Name = "Centre de profit")]
        public string ProfitCenter { get; set; }

        [Display(Name = "Filiale")]
        public string Subsidiary { get; set; }
    }

    public class UpdateConsultantViewModel
    {
        public int ID { get; set; }


        [Required]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "Nom")]
        public string LastName { get; set; }


        [Required]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date d'entrée")]
        public DateTime EntryDate { get; set; }

        [Display(Name = "Statut")]
        public string Status { get; set; }

     
        [DataType(DataType.Currency)]
        [Display(Name = "Coût")]
        public double Cost { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Repas")]
        public double MealCost { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Forfait déplacement")]
        public double TravelPackage { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Coût exceptionnel")]
        public double ExceptionalCost { get; set; }


        public string SubsidiaryName { get; set; }
        public string ProfitCenterName { get; set; }

        public List<string> Subsidiaries { get; set; }
        public List<string> ProfitCenters { get; set; }
       
    }

    public class ConsultantCraModel
    {
        public int ID { get; set; }

        [Required]
    
        public string Consultant { get; set; }


        public List<string> MissionsList { get; set; }

        public Dictionary<String,double[]> NbParMission { get; set; }
        public Dictionary<String, int[]> DureeMission { get; set; }



    }

    public class ConsultantCraModelNew
    {
        public int ID { get; set; }

        [Required]

        public string Consultant { get; set; }


        public List<string> MissionsList { get; set; }

        public Dictionary<String, Dictionary<String,double[]>> NbParMission { get; set; }
        public Dictionary<String, int[]> DureeMission { get; set; }

    }

}