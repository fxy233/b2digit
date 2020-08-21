using Foolproof;
using Projet_pilate.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projet_pilate.Models
{

    public class RegisterCompanyViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Le nom de l'entreprise doit être renseigné.")]
        [Display(Name = "Nom de l'entreprise")]
        public string Name { get; set; }

        [Required (ErrorMessage = "L'adresse doit être renseignée.")]
        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Le code postale doit être renseigné.")]
        [RegularExpression("(?:0[1-9]|[13-8][0-9]|2[ab1-9]|9[0-5])(?:[0-9]{3})?|9[78][1-9](?:[0-9]{2})?",
                            ErrorMessage = "Le code postal doit correspondre au format 00000.")]
        [Display(Name = "Code postal")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "La ville doit être renseigné.")]
        [Display(Name = "Ville")]
        public string City { get; set; }

        
        [Display(Name = "Mail de Facturation")]
        public string MailFacturation { get; set; }

    }

    public class DetailCompanyViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Nom de l'entreprise")]
        public string Name { get; set; }

        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Display(Name = "Code postal")]
        public string PostalCode { get; set; }

        [Display(Name = "Ville")]
        public string City { get; set; }

        [Display(Name = "Mail de facturation")]
        public string Mail { get; set; }

    }

    public class UpdateCompanyViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Nom de l'entreprise")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Required]
        [StringLength(5)]
        [Display(Name = "Code postal")]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Ville")]
        public string City { get; set; }

        
        [Display(Name = "Mail de Facturation")]
        public string MailFacturation { get; set; }

    }

    public class RegisterMissionViewModel
    {
        public int ID { get; set; }
        public int CreatorID { get; set; }

        [Required]
        [Display(Name = "Nom de la mission")]
        public string Name { get; set; }

        [Display(Name = "Contact")]
        public List<string> ContactMail { get; set; }

        [Display(Name = "Consultant")]
        public List<string> ConsultantNames { get; set; }

        [Display(Name = "Date de début")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }

        [Display(Name = "Date de fin")]
        [DataType(DataType.Date)]
        [GreaterThan("Start", ErrorMessage = "La date de fin ne peut être inférieure ou égale à la date de début de mission !")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(1.0, 30000.0)]
        [Display(Name = "Tarif")]
        public float Fee { get; set; }

        [Required]
        [Range(0,31)]
        [Display(Name = "Nbj Gratuité")]
        public int FreeDay { get; set; }

        [Display(Name = "Périodicité")]
        public List<string> Periodicity { get; set; }

        [Display(Name = "Adresse de mission")]
        public string AdresseMission { get; set; }


        [Display(Name = "Commentaire")]
        public string Comment { get; set; }

        public List<Mission> MissionEncours { get; set; }

    }

    public class DetailsMissionViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nom de la mission")]
        public string Name { get; set; }

        [Display(Name = "Email du contact")]
        public string ContactEmail { get; set; }

        [Display(Name = "Nom du client")]
        public string ClientName { get; set; }

        [Display(Name = "Consultant")]
        public string ConsultantName { get; set; }

        [Display(Name = "Crée par")]
        public string CreatorName { get; set; }

        [Display(Name = "Date de début")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }

        [Display(Name = "Date de fin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }

     
        [Display(Name = "Tarif")]
        public float Fee { get; set; }

        [Display(Name = "Nbj Gratuité")]
        public int FreeDay { get; set; }

        [Display(Name = "Périodicité")]
        public string Periodicity { get; set; }

        [Display(Name = "Commentaire")]
        public string Comment { get; set; }

        [Display(Name = "Centre de profit")]
        public string ProfitCenter { get; set; }
    }


    public class RegisterCompanyContactViewModel
    {
        public int ID { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email du contact")]
        public string Mail { get; set; }

        [Display(Name = "Entreprise")]
        public List<string> CompanyNames { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Nom")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Position")]
        public string Position { get; set; }
        [Required]
        [RegularExpression(@"^(0|\+33)[1-9]([-. ]?[0-9]{2}){4}$",
                        ErrorMessage = "Le numéro de téléphone doit contenir 10 chiffres.")]
        [Display(Name = "Téléphone")]
        public string Phone { get; set; }

        //

        [Display(Name = "Business Manager")]
        public List<string> ManagerNames { get; set; }
    }


    public class DetailCompanyContactViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Email")]
        public string Mail { get; set; }

        [Display(Name = "Entreprise")]
        public string CompanyName { get; set; }

        [Display(Name = "Prénom")]
        public string FirstName { get; set; }

        [Display(Name = "Nom")]
        public string LastName { get; set; }

        [Display(Name = "Position")]
        public string Position { get; set; }

        [Display(Name = "Téléphone")]
        public string Phone { get; set; }

        [Display(Name = "Manager")]
        public string Manager { get; set; }
    }

    public class UpdateCompanyContactViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Entreprise")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Mail { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Position")]
        public string Position { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Téléphone")]
        public string Phone { get; set; }

    }

    public class UpdateMissionViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Date de début")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }

        [Display(Name = "Date de fin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }

        [Required]
        [Display(Name = "Nouvelle date de fin")]
        [DataType(DataType.Date)]
        [GreaterThan("Start", ErrorMessage = "La date de fin ne peut être inférieure ou égale à la date de début de mission !")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NewEnd { get; set; }

        [Required]
        [Display(Name = "Nom de la mission")]
        public string Name { get; set; }

        [Display(Name = "Tarif")]
        public float Fee { get; set; }

        [Display(Name = "Adresse de Mission")]
        public string AdresseMission { get; set; }

        [Display(Name = "Commentaire")]
        public string Commentaire { get; set; }

        [Display(Name = "InfoFacturation")]
        public string InfoFacturation { get; set; }

        [Display(Name = "Référence")]
        public string Reference { get; set; }

        [Display(Name = "Business Company émettrice")]
        public int  PrincipalBCID { get; set; }

        [Display(Name = "Business Company Intermédiaire 1")]
        public int InterBC1ID { get; set; }

        [Display(Name = "TJ intermédiaire")]
        public float TJInterBC1 { get; set; }
        
        [Display(Name = "Business Company Intermédiaire N°2")]
        public int InterBC2ID { get; set; }

        [Display(Name = "TJ intermédiaire N°2")]
        public float TJInterBC2 { get; set; }

        [Display(Name = "Designation Facturation")]
        public string DesignationFacturation { get; set; }

        [Display(Name = "Délai de Paiement")]
        public string DelaiPaiement { get; set; }


    }

    public class SubsidiaryViewModel
    {
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Siren")]
        public string Siren { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Nom")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Required]
        [StringLength(5)]
        [Display(Name = "Code postal")]
        public string PostaleCode { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Ville")]
        public string City { get; set; }

        //

        [Required]
        [StringLength(50)]
        [Display(Name = "Prénom du responsable")]
        public string ManagerFirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Nom du responsable")]
        public string ManagerLastName { get; set; }

        [Required]
        [StringLength(27)]
        [Display(Name = "IBAN")]
        public string IBAN { get; set; }

        [Required]
        [StringLength(11)]
        [Display(Name = "BIC")]
        public string BIC { get; set; }

        [Required]
        [Display(Name = "TVA Intracommunautaire")]
        public string TVAIntra { get; set; }

        [Display(Name = "Mail de la société")]
        public string email { get; set; }

        [Display(Name = "Mot de passe du mail")]
        public string motdepasse { get; set; }

    }

    public class DetailSubsidiaryViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Siren")]
        public string Siren { get; set; }

        [Display(Name = "Nom")]
        public string Name { get; set; }

        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Display(Name = "Code postal")]
        public string PostaleCode { get; set; }

        [Display(Name = "Ville")]
        public string City { get; set; }

        //
        [Display(Name = "Prénom du responsable")]
        public string ManagerFirstName { get; set; }

        [Display(Name = "Nom du responsable")]
        public string ManagerLastName { get; set; }
    }

    public class UpdateSubsidiaryViewModel
    {
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Siren")]
        public string Siren { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Nom")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Required]
        [StringLength(5)]
        [Display(Name = "Code postal")]
        public string PostaleCode { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Ville")]
        public string City { get; set; }


        [Required]
        [StringLength(50)]
        [Display(Name = "Prénom responsable")]
        public string ManagerFirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Nom responsable")]
        public string ManagerLastName { get; set; }

        [Required]
        [StringLength(27)]
        [Display(Name = "IBAN")]
        public string IBAN { get; set; }

        [Required]
        [StringLength(11)]
        [Display(Name = "BIC")]
        public string BIC { get; set; }

        [Required]
        [Display(Name = "TVA Intracommunautaire")]
        public string TVAIntra { get; set; }

        [Display(Name = "Mail de la société")]
        public string email { get; set; }

        [Required]
        [Display(Name = "Mot de passe du mail")]
        public string motdepasse{ get; set; }

    }
}


