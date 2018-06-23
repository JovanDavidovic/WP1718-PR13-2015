using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    public class Komentar
    {
        public string Opis { get; set; }
        public DateTime DatumObjave { get; set; }
        public Korisnik korisnik { get; set; }
        public Voznja voznja { get; set; }
        public int Ocena { get; set; }
    }
}