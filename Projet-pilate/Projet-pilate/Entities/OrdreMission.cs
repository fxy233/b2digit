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
        public string NomMission { get; set; }
        public string typeProjet { get; set; }

    }
}