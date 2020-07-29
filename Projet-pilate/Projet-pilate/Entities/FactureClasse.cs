using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Projet_pilate.Entities
{
    public class Facture
    {
        // Primary Key
        public int FactureID { get; set; }

        public DateTime DernierEnregistrer { get; set; }
        public DateTime MoisDeFacturation { get; set; }

        public DateTime DateRegelement { get; set; }

        
        [MaxLength(50)]
        public string NomFacture { get; set; }

        public string mission { get; set; }
        public string InfoFacturation { get; set; }
        public string PrincipalBC { get; set; }
        public string AdresseBC { get; set; }
        public string Client { get; set; }
        public string AdresseFacturation { get; set; }

        public float NombredUO { get; set; }
        public float TJ { get; set; }
        public float TVA { get; set; }
        public float MontantHT { get; set; }
        public bool FAE { get; set; }
        public bool Emise { get; set; }
        public bool payee { get; set; }
        public bool annulee { get; set; }
        public string Delai { get; set; }
        public string DesignationFacturation { get; set; }
        public string type { get; set; }
        public int parentID { get; set; }
        public string reference { get; set; }
        public string referenceBancaire { get; set; }
        public int CraId { get; set; }

    }

    
}