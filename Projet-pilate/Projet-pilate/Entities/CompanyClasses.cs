using Projet_pilate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Projet_pilate.Entities
{
    public class Company
    {
        // Primary key
        public int CompanyID { get; set; }

        [Index("Name_Unique_Index", IsUnique = true)]
        [MaxLength(50)]
        public string Name { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public virtual List<CompanyContact> CompanyContacts { get; set; }
    }

    public class CompanyContact
    {
        // Primary key
        public int CompanyContactID { get; set; }

        [Index("Contact_Unique_Index", IsUnique = true, Order = 1)]
        [MaxLength(50)]
        public string Mail { get; set; }

        [Index("Contact_Unique_Index", IsUnique = true, Order = 2)]
        [MaxLength(50)]
        public string CompanyName { get; set; }

        [Index("Contact_Unique_Index", IsUnique = true, Order = 3)]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Index("Contact_Unique_Index", IsUnique = true, Order = 4)]
        [MaxLength(50)]
        public string LastName { get; set; }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }

        // Foreign keys
        public int CompanyID { get; set; }
        public int ManagerID { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; }
        public virtual Manager Manager { get; set; }
        public virtual List<Mission> Missions { get; set; }

    }



}