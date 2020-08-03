using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projet_pilate.Entities
{
    public class OrdreDeMission
    {
        public int OrdreDeMissionID { get; set; }
        public Boolean ValidationDeAdv { get; set; }
        public Boolean ValidationConsultant { get; set; }
        public string Mission { get; set; }
        public string fraisAlloue { get; set; }
        public string environnement { get; set; }
        public string signature { get; set; }
        public string manager { get; set; }
        public DateTime dateDebut { get; set; }
        public DateTime dataFin { get; set; }
        public string nomConsultant { get; set; }
        public string prenomConsultant { get; set; }
        public string nomClient { get; set; }
        public string contactClient { get; set; }

        public string missionAdresse { get; set; }
    }
}