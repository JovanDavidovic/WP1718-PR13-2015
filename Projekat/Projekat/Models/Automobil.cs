using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    public class Automobil
    {
        public Vozac vozac { get; set; }
        public int Godiste { get; set; }
        public string RegistarskaOznaka { get; set; }
        public int BrojVozila { get; set; }
        public TIPAUTOMOBILA TipAutomobila { get; set; }
    }
}