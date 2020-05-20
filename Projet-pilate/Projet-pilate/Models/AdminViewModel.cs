using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projet_pilate.Models
{
    public class AdminViewModel
    {
        public string MoisActif { get; set; }

        [Display(Name = "Mois à activer")]
        public string MoisProchain { get; set; }

    }

    public class MessageViewModel
    {
        public string Message { get; set; }


    }

}