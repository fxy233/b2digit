using Projet_pilate.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projet_pilate.Models
{
    public class RegisterActivityViewModel
    {
        public List<DateTime> Dates { get; set; }
        public string Satisfaction { get; set; }
        public string Comment { get; set; }
        public List<string> MissionsList { get; set; }
        public Cra Cra { get; set; }

        public string MoisActivation { get; set; }
    }

    public class DetailActivityViewModel
    {
        public List<Activity> Activities { get; set; }
        public double NbMission { get; set; }
        public double NbConges { get; set; }
        public double NbFormation { get; set; }
        public double NbIc { get; set; }
        public double NbMaladie { get; set; }

       
        public string MoisActivation { get; set; }
        public Cra Cra { get; set; }

        public double NbJoursOuvres { get; set; }

    }


    public class ActivityViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Période du Cra")]
        public string Date { get; set; }

        [Display(Name = "Satisfaction")]
        public string Satisfaction { get; set; }

        [Display(Name = "Commentaire")]
        public string Comment { get; set; }

        [Display(Name = "Consultant")]
        public string ConsultantName { get; set; }

        [Display(Name = "Mission")]
        public string MissionName { get; set; }

        [Display(Name = "Jours travaillés")]
        // Doit être un float
        public float WorkedDays { get; set; }

        [Display(Name = "Jours non facturés")]
        // Doit être un float
        public float NoBillDays { get; set; }

        [Display(Name = "Jours travaillés en WE")]
        // Doit être un float
        public float WorkedDaysWE { get; set; }

    }

    public class ActivityExportModel
    {
        public int id { get; set; }
        public string consultant { get; set; }
        public string societe { get; set; }

        public DateTime date { get; set; }

        public Dictionary<string, string> manager { get; set; }

        //public string client { get; set; }
 
        public string periode { get; set; }

        //public Dictionary<string, List<string>> missionParClient { get; set; }

        public Dictionary<string, Dictionary<string, double[] > > activityParClient { get; set; }


    }

    public class CreationOdmModel
    {
        public int id { get; set; }
        public string Manager { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime debut { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime fin { get; set; }
        public string nomConsultant { get; set; }
        public string prenomConsultant { get; set; }
        public string nomClient { get; set; }
        public string contactClient { get; set; }
        public string Adresse { get; set; }
        public string Environnement { get; set; }
        public string Mission { get; set; }
        public string fraisAlloue { get; set; }
        public string signature { get; set; }
    }

    public class ListeOdmModel
    {
        public int id { get; set; }

        [Display(Name = "Nom de la mission")]
        public string NomMission { get; set; }

        [Display(Name = "Client")]
        public string nomConsultant { get; set; }

        [Display(Name = "Consultant")]
        public string nomClient { get; set; }

        [Display(Name = "Statut")]
        public string Status { get; set; }


    }



}