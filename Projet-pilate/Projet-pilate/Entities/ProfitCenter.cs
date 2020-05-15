using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Projet_pilate.Entities
{
    public class ProfitCenter
    {
        // Primary key
        public int ProfitCenterID { get; set; }

        [Index("Index_Name", IsUnique = true)]
        [MaxLength(50)]
        public string Name { get; set; }
        public double Cost { get; set; }
        public double Turnover { get; set; }

        public string Owner { get; set; }
        public string PartOwner { get; set; }

        public int? FatherProfitCenterID { get; set; }

        //Foreign Key
        public ProfitCenter FatherProfitCenter { get; set; }



        // Navigation properties
        public virtual List<Manager> Managers { get; set; }
        public virtual List<Consultant> Consultants { get; set; }
        public virtual List<Mission> Missions { get; set; }
    }
}