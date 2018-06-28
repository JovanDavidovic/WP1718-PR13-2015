using Projekat.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Projekat
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private string putanja = "C:/FAX/WEB/Projekat/";
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start()
        {
            
            Dictionary<string, Korisnik> administratori = new Dictionary<string, Korisnik>();
            string admin;
            if (!File.Exists(putanja + "WP1718-PR13-2015/Projekat/packages/Administratori.txt"))
            {
                File.Create(putanja + "WP1718-PR13-2015/Projekat/packages/Administratori.txt");
            }
            System.IO.StreamReader file = new System.IO.StreamReader(putanja + "WP1718-PR13-2015/Projekat/packages/Administratori.txt");
            while ((admin = file.ReadLine()) != null)
            {
                Korisnik administrator = new Korisnik();
                administrator.KorisnickoIme = admin.Split(' ')[0];
                administrator.Ime = admin.Split(' ')[1];
                administrator.Prezime = admin.Split(' ')[2];
                administrator.Lozinka = admin.Split(' ')[3];
                administrator.Email = admin.Split(' ')[4];
                administrator.Telefon = admin.Split(' ')[5];
                administrator.JMBG = admin.Split(' ')[6];
                administrator.Uloga = ULOGA.DISPECER;
                if (admin.Split(' ')[7].Equals("m"))
                {
                    administrator.Pol = POL.MUSKI;
                }
                else
                {
                    administrator.Pol = POL.ZENSKI;
                }
                administratori.Add(administrator.KorisnickoIme, administrator);
            }
            HttpContext.Current.Session["Administratori"] = administratori;
            file.Close();



            Dictionary<string, Vozac> vozaci = new Dictionary<string, Vozac>();
            if (!File.Exists(putanja + "WP1718-PR13-2015/Projekat/packages/Vozaci.txt"))
            {
                File.Create(putanja + "WP1718-PR13-2015/Projekat/packages/Vozaci.txt");
            }
            System.IO.StreamReader file2 = new System.IO.StreamReader(putanja + "WP1718-PR13-2015/Projekat/packages/Vozaci.txt");
            while ((admin = file2.ReadLine()) != null)
            {
                Vozac vozac = new Vozac();
                vozac.KorisnickoIme = admin.Split(' ')[0];
                vozac.Ime = admin.Split(' ')[1];
                vozac.Prezime = admin.Split(' ')[2];
                vozac.Lozinka = admin.Split(' ')[3];
                vozac.Email = admin.Split(' ')[4];
                vozac.Telefon = admin.Split(' ')[5];
                vozac.JMBG = admin.Split(' ')[6];
                vozac.Uloga = ULOGA.VOZAC;
                if (admin.Split(' ')[7].Equals("MUSKI"))
                {
                    vozac.Pol = POL.MUSKI;
                }
                else
                {
                    vozac.Pol = POL.ZENSKI;
                }
                vozac.automobil = new Automobil();
                vozac.automobil.BrojVozila = Int32.Parse(admin.Split(' ')[8]);
                vozac.automobil.Godiste = Int32.Parse(admin.Split(' ')[9]);
                vozac.automobil.RegistarskaOznaka = admin.Split(' ')[10];
                vozac.Uloga = ULOGA.VOZAC;
                if (admin.Split(' ')[11].Equals("PUTNICKI"))
                {
                    vozac.automobil.TipAutomobila = TIPAUTOMOBILA.PUTNICKI;
                }
                else
                {
                    vozac.automobil.TipAutomobila = TIPAUTOMOBILA.KOMBI;
                }
                vozac.lokacija = new Lokacija();
                vozac.lokacija.Xkoordinata = double.Parse(admin.Split(' ')[12]);
                vozac.lokacija.Ykoordinata = double.Parse(admin.Split(' ')[13]);
                vozac.lokacija.adresa = new Adresa();
                vozac.lokacija.adresa.PozivniBroj = double.Parse(admin.Split(' ')[14]);
                vozac.lokacija.adresa.Broj= Int32.Parse(admin.Split(' ')[15]);
                vozac.lokacija.adresa.Mesto = admin.Split(' ')[16];
                vozac.lokacija.adresa.Ulica = admin.Split(' ')[17];
                vozac.Zauzet = false;
                vozaci.Add(vozac.KorisnickoIme, vozac);
            }
            HttpContext.Current.Session["Vozaci"] = vozaci;
            file2.Close();

            Dictionary<string, Korisnik> korisnici = new Dictionary<string, Korisnik>();
            if (!File.Exists(putanja + "WP1718-PR13-2015/Projekat/packages/Korisnici.txt"))
            {
                File.Create(putanja + "WP1718-PR13-2015/Projekat/packages/Korisnici.txt");
            }
            System.IO.StreamReader file3 = new System.IO.StreamReader(putanja + "WP1718-PR13-2015/Projekat/packages/Korisnici.txt");
            while ((admin = file3.ReadLine()) != null)
            {
                Korisnik korisnik = new Korisnik();
                korisnik.KorisnickoIme = admin.Split(' ')[0];
                korisnik.Ime = admin.Split(' ')[1];
                korisnik.Prezime = admin.Split(' ')[2];
                korisnik.Lozinka = admin.Split(' ')[3];
                korisnik.Email = admin.Split(' ')[4];
                korisnik.Telefon = admin.Split(' ')[5];
                korisnik.JMBG = admin.Split(' ')[6];
                korisnik.Uloga = ULOGA.MUSTERIJA;
                if (admin.Split(' ')[7].Equals("MUSKI"))
                {
                    korisnik.Pol = POL.MUSKI;
                }
                else
                {
                    korisnik.Pol = POL.ZENSKI;
                }
                korisnici.Add(korisnik.KorisnickoIme, korisnik);
            }

            file3.Close();
            HttpContext.Current.Session["RegistrovaniKorisnici"] = korisnici;

            HttpContext.Current.Session["Voznje"] = new Dictionary<string, Voznja>();
        }
    }
}
