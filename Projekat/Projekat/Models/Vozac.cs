using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    public class Vozac : Korisnik
    {
        public bool Zauzet { get; set; }
        public Lokacija lokacija { get; set; }
        public Automobil automobil { get; set; }
    }
}