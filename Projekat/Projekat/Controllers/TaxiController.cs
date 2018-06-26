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
        private Dictionary<string, Voznja> getVoznje
        {
            get
            {
                return (Dictionary<string, Voznja>)Session["Voznje"];
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
        public ActionResult MusterijaZahtev()
        {
            return View("MusterijaZahtev");
        }
        [HttpPost]
        public ActionResult KreiranjeVoznje(Lokacija l, Adresa ad, TIPAUTOMOBILA tip)
        {
            l.adresa = ad;
            Voznja v = new Voznja();
            v.LokacijaDolaskaTaksija = l;
            v.VremePorudzbine = DateTime.Now;
            v.ZeljenoVozilo = tip;
            v.StatusVoznje = STATUS.NACEKANJU;
            v.musterija = (Korisnik)Session["Ulogovan"];
            getVoznje.Add(v.VremePorudzbine.ToString(), v);
            return View("PregledSvihVoznjiMusterije");
        }
        [HttpPost]
        public ActionResult Otkazivanje(string date)
        {
            bool test = false;
            if (getVoznje[date].StatusVoznje == STATUS.NACEKANJU)
            {
                getVoznje[date].StatusVoznje = STATUS.OTKAZANA; ;
                test = true;
            }
            if (test)
            {
                ViewBag.voznja = getVoznje[date];
                return View("Komentar");
            }
            else
                return View("OtkazivanjeError");
        }
        public ActionResult Pregledaj()
        {
            return View("PregledSvihVoznjiMusterije");
        }
        public ActionResult GoToHome()
        {
            return View("HomepageMusterija");
        }
        [HttpPost]
        public ActionResult IzmeniVoznju(string date)
        {
            ViewBag.voznja = getVoznje[date];
            return View("IzmenaVoznje");
        }
        [HttpPost]
        public ActionResult IzmenaVoznje(Lokacija l, Adresa ad, TIPAUTOMOBILA tip, string date)
        {
            bool test = false;
            if (getVoznje[date].StatusVoznje == STATUS.NACEKANJU) {
                l.adresa = ad;
                Voznja v = new Voznja();
                v.LokacijaDolaskaTaksija = l;
                v.VremePorudzbine = DateTime.Parse(date);
                if (!tip.Equals(""))
                {
                    v.ZeljenoVozilo = tip;
                }
                v.StatusVoznje = STATUS.NACEKANJU;
                v.musterija = (Korisnik)Session["Ulogovan"];
                getVoznje[date] = v;
                test = true;
            }

            if (test)
                return View("PregledSvihVoznjiMusterije");
            else
                return View("OtkazivanjeError");
        }

        [HttpPost]
        public ActionResult Komentarisi(string date, string komentar)
        {
            getVoznje[date].komentar = new Komentar();
            getVoznje[date].komentar.DatumObjave = DateTime.Now;
            getVoznje[date].komentar.korisnik = (Korisnik)Session["Ulogovan"];
            getVoznje[date].komentar.Opis = komentar;
            getVoznje[date].komentar.voznja = getVoznje[date];
            getVoznje[date].komentar.Ocena = 0;
            return View("PregledSvihVoznjiMusterije");
        }
    }
}