using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projekat.Controllers
{
    public class TaxiController : Controller
    {
        private Dictionary<string, Korisnik> getKorisnik
        {
            get
            {
                return (Dictionary<string, Korisnik>)Session["RegistrovaniKorisnici"];
            }
        }

        private Dictionary<string, Korisnik> getAdministratori
        {
            get
            {
                return (Dictionary<string, Korisnik>)Session["Administratori"];
            }
        }

        private Dictionary<string, Vozac> getVozaci
        {
            get
            {
                return (Dictionary<string, Vozac>)Session["Vozaci"];
            }
        }
        // GET: Taxi
        public ActionResult Index()
        {
            if (Session["Ulogovan"] != null)
                return View("HomepageMusterija");
            return View();
        }

        public ActionResult GoToSignUp()
        {
            return View("SignUp");
        }
        public ActionResult GoToLogIn()
        {
            return View("LogIn");
        }

        [HttpPost]
        public ActionResult SignUp(Korisnik k)
        {
            k.Uloga = ULOGA.MUSTERIJA;
            if (getKorisnik.ContainsKey(k.KorisnickoIme) || getAdministratori.ContainsKey(k.KorisnickoIme) || getVozaci.ContainsKey(k.KorisnickoIme))
            {
                ViewBag.korisnik = k;
                return View("SignUpError");
            }
            else
            {
                getKorisnik.Add(k.KorisnickoIme, k);
            }
            return View("Index");
        }
        [HttpPost]
        public ActionResult LogIn(string korisnickoIme, string lozinka)
        {
            if (!getKorisnik.ContainsKey(korisnickoIme))
            {
                if (getAdministratori.ContainsKey(korisnickoIme))
                {
                    if (!getAdministratori[korisnickoIme].Lozinka.Equals(lozinka))
                    {
                        return View("LozinkaError");
                    }
                    Session["Ulogovan"] = getAdministratori[korisnickoIme];
                    return View("HomepageAdministrator");
                }
                if (getVozaci.ContainsKey(korisnickoIme))
                {
                    if (!getVozaci[korisnickoIme].Lozinka.Equals(lozinka))
                    {
                        return View("LozinkaError");
                    }
                    Session["Ulogovan"] = getVozaci[korisnickoIme];
                    return View("HomepageVozac");
                }
                return View("KorisnickoImeError");
            }

            if (!getKorisnik[korisnickoIme].Lozinka.Equals(lozinka))
            {
                return View("LozinkaError");
            }

            Session["Ulogovan"] = getKorisnik[korisnickoIme];
            return View("HomepageMusterija");
        }

        public ActionResult Izmena()
        {
            return View("Izmena");
        }

        public ActionResult Izmeni(Korisnik k)
        {
            getKorisnik[k.KorisnickoIme] = k;
            Session["Ulogovan"] = getKorisnik[k.KorisnickoIme];
            return View("Izmena");
        }

        public ActionResult GoToKreiranjeVozaca()
        {
            return View("KreiranjeVozaca");
        }
        [HttpPost]
        public ActionResult KreiranjeVozaca(Vozac v, Automobil a, Lokacija l, Adresa ad)
        {
            a.vozac = v;
            v.automobil = a;
            l.adresa = ad;
            v.lokacija = l;

            getVozaci.Add(v.KorisnickoIme, v);
            return View("HomepageAdministrator");
        }

        public ActionResult PromeniLokaciju()
        {
            return View("PromenaLokacije");
        }
        [HttpPost]
        public ActionResult PromenaLokacije(string log, Lokacija l, Adresa ad)
        {
            l.adresa = ad;
            getVozaci[log].lokacija = l;
            Session["Ulogovan"] = getVozaci[log];
            return View("HomepageVozac");
        }
        public ActionResult Odjava()
        {
            Session["Ulogovan"] = null;
            return View("Index");
        }
    }
}