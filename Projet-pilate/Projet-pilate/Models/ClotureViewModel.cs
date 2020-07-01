using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projet_pilate.Models
{
    public class ClotureViewModel
    {
        public int MoisaCloturerID { get; set; }

        [Display(Name = "Mois à clôturer")]
        public string MoisaCloturer { get; set; }

    }

}