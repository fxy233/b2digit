var months = ["Janvier", "Février", "Mars", "Avril", "Mai", "juin", "Juillet", "Aout", "Septembre", "Octobre", "Novembre", "Décembre"];
var d = new Date();
var monthName = months[d.getMonth()];

var actif = document.getElementById("actif");
actif.value = monthName + " " + new Date().getUTCFullYear();

var suivant = document.getElementById("suivant");
var moisSuivant = months[d.getMonth()+1];

suivant.value = moisSuivant + " " + new Date().getUTCFullYear();