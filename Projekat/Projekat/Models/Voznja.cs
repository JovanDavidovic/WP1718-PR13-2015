﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    public class Voznja
    {
        public DateTime VremePorudzbine { get; set; }
        public Lokacija LokacijaDolaskaTaksija { get; set; }
        public Automobil ZeljenoVozilo { get; set; }
        public Musterija musterija { get; set; }
        public Lokacija Odrediste { get; set; }
        public Dispecer dispecer { get; set; }
        public Vozac vozac { get; set; }
        public double Iznos { get; set; }
        public Komentar komentar { get; set; } 
        public STATUS StatusVoznje { get; set; }
    }
}