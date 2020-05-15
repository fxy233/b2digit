using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projet_pilate.Models
{
    public class ConsultantManagerActivationViewModel
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
        [Display(Name = "Date d'entréé")]
        public DateTime EntryDate { get; set; }

    }
}