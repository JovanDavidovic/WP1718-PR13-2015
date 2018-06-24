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
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start()
        {
            HttpContext.Current.Session["RegistrovaniKorisnici"] = new Dictionary<string, Korisnik>();
            Dictionary<string, Korisnik> administratori = new Dictionary<string, Korisnik>();
            string admin;
            if (!File.Exists("Administratori.txt"))
            {
                File.Create("Administratori.txt");
            }
            System.IO.StreamReader file = new System.IO.StreamReader("C:/FAX/WEB/Projekat/WP1718-PR13-2015/Projekat/packages/Administratori.txt");
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
            HttpContext.Current.Session["Vozaci"] = new Dictionary<string, Vozac>();
        }
    }
}
