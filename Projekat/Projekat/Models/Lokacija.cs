using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    public class Lokacija
    {
        public double Xkoordinata { get; set; }
        public double Ykoordinata { get; set; }
        public Adresa adresa { get; set; }
    }
}