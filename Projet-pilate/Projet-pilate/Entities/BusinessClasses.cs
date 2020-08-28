using Projet_pilate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Projet_pilate.Entities
{
    public class Consultant
    {
        // Primary key
        public int ConsultantID { get; set; }

        [MaxLength(128)]
        public string UserID { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [Index("Consultant_Unique_Index", IsUnique = true)]
        [MaxLength(256)]
        public string Email { get; set; }


        [MaxLength(50)]
        public string Status { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EntryDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfDeparture { get; set; }

        public double DailyCost { get; set; }
        public double MonthlyCost { get; set; }
        public double MealCost { get; set; }
        public double TravelPackage { get; set; }
        public double ExceptionalCost { get; set; }
        public Boolean quit { get; set; }

        // Foreign key
        public int ProfitCenterID { get; set; }
        public int SubsidiaryID { get; set; }


        // Navigation properties
        public virtual ProfitCenter ProfitCenter { get; set; }
        public virtual List<Cra> Cras { get; set; }
        public virtual List<Mission> Missions { get; set; }
        public virtual Subsidiary Subsidiary { get; set; }
    }

    public class Manager
    {
        // Primary key
        public int ManagerID { get; set; }

        [MaxLength(128)]
        public string UserID { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [Index("Manager_Unique_Index", IsUnique = true)]
        [MaxLength(50)]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EntryDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfDeparture { get; set; }

        public double MonthlyCost { get; set; }
        public double MealCost { get; set; }
        public double TravelPackage { get; set; }
        public double ExceptionalCost { get; set; }
        public string role { get; set; }

        public Boolean quit { get; set; }


        // Foreign Key
        public int SubsidiaryID { get; set; }


        // Navigation properties
        public virtual List<CompanyContact> CompanyContacts { get; set; }
        public virtual List<ProfitCenter> ProfitCenters { get; set; }
        public virtual Subsidiary Subsidiary { get; set; }

    }

    public class Subsidiary
    {
        // Primary key
        public int SubsidiaryID { get; set; }

        [Index("Subsidiary_Unique_Index", IsUnique = true, Order = 1)]
        [MaxLength(50)]
        public string Siren { get; set; }


        [Index("Subsidiary_Unique_Index", IsUnique = true, Order = 2)]
        [Index("Subsidiary_Unique_Name", IsUnique = true)]
        [MaxLength(50)]
        public string Name { get; set; }
        public string Address { get; set; }
        public string PostaleCode { get; set; }
        public string City { get; set; }
        public string ManagerFirstName { get; set; }
        public string ManagerLastName { get; set; }
        public string email { get; set; }
        public string motdepasse { get; set; }
        public string VCode { get; set; }

        [MaxLength(27)]
        public string IBAN { get; set; }

        [MaxLength(11)]
        public string BIC { get; set; }
        public string TVAIntra { get; set; }
        public int FactureID { get; set; }


        // Navigation properties
        public virtual List<Manager> Managers { get; set; }
        public virtual List<Consultant> Consultants { get; set; }

    }
}