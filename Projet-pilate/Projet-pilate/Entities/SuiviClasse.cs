using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projet_pilate.Entities
{
    public class Suivi
    {
        //Primary key
        public int SuiviID { get; set; }
        public DateTime MoisDeFacturation { get; set; }
        public int SubsidiaryID { get; set; }
        public int ProfitCenterID { get; set; }
        public string NomMission { get; set; }
        public string Consultant { get; set; }
        public float NombredUO { get; set; }
        public float TJ { get; set; }
        public float cout { get; set; }
        public float CJ { get; set; }
        public float Frais { get; set; }
    }
}