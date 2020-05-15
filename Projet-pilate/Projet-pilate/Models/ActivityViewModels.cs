﻿using Projet_pilate.Entities;
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
        public double NbConge { get; set; }
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
        public string Month { get; set; }

        public string Satisfaction { get; set; }

        [Display(Name = "Commentaire")]
        public string Comment { get; set; }

        [Display(Name = "Consultant")]
        public string ConsultantName { get; set; }

        [Display(Name = "Jours travaillés")]
        // Doit être un float
        public int WorkedDays { get; set; }

    }
}