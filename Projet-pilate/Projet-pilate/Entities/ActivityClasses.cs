using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Projet_pilate.Entities
{

    public class MonthActivation
    {
        // Primary key
        public int MonthActivationID { get; set; }
        public DateTime Periode { get; set; }
    }

    public class Message
    {
        //Primary key
        public int messageID { get; set; }
        public string message { get; set; }
    }


    public class Activity
    {
        // Primary key
        public int ActivityID { get; set; }
        public DateTime Date { get; set; }
        public string Morning { get; set; }
        public string Afternoon { get; set; }

        // Foreign key
        public int CraID { get; set; }

        // Navigation properties  
        public virtual Cra Cra { get; set; }

    }


    public class Cra
    {
        // Primary key
        public int CraID { get; set; }
        public Boolean Changeable { get; set; }
        public string Month { get; set; }
        public string year { get; set; }
        public string Satisfaction { get; set; }
        public string Comment { get; set; }

        // Foreign key
        public int ConsultantID { get; set; }

        // Navigation properties    
        public virtual Consultant Consultant { get; set; }
        public virtual List<Activity> Activities { get; set; }

    }

    public class Mission
    {
        // Primary key
        public int MissionID { get; set; }

        [Index("MissionName_Unique_Index", IsUnique = true, Order = 1)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Index("Mission_Unique_Index", IsUnique = true, Order = 1)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }

        [Index("Mission_Unique_Index", IsUnique = true, Order = 2)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }
        public float Fee { get; set; }
        public int FreeDay { get; set; }
        public string Periodicity { get; set; }
        public string Comment { get; set; }

        public string Creator { get; set; }

        // Foreign key

        [Index("Mission_Unique_Index", IsUnique = true, Order = 3)]
        public int ConsultantID { get; set; }

        [Index("Mission_Unique_Index", IsUnique = true, Order = 4)]
        public int CompanyContactID { get; set; }

        [Index("Mission_Unique_Index", IsUnique = true, Order = 5)]
        public int ProfitCenterID { get; set; }

        // Navigation properties   
        public virtual Consultant Consultant { get; set; }
        public virtual CompanyContact CompanyContact { get; set; }
        public virtual ProfitCenter ProfitCenter { get; set; }

    }
}