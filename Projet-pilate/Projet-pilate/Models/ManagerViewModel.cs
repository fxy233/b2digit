using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projet_pilate.Models
{
    public class ManagerViewModel
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

    }


    public class RegisterManagerViewModel
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

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date d'entrée")]
        public DateTime EntryDate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Salaire mensuel (fixe + variable)")]
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

        public string Subsidiary { get; set; }
        public string role { get; set; }

        public List<string> Subsidiaries { get; set; }

    }


    public class UpdateManagerViewModel
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

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date d'entrée")]
        public DateTime EntryDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date de sortie")]
        public DateTime DateSortie { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Salaire mensuel (fixe + variable)")]
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

        public string Subsidiary { get; set; }
        public string role { get; set; }
        public List<string> Subsidiaries { get; set; }

    }


    public class DetailManagerViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Prénom")]
        public string FirstName { get; set; }

        [Display(Name = "Nom")]
        public string LastName { get; set; }

        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date d'entrée")]
        public DateTime EntryDate { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Salaire mensuel")]
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

        [Display(Name = "Filiale")]
        public string Subsidiary { get; set; }

    }
}