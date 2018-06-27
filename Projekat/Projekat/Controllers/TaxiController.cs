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
            int i = 0;
            if (!getKorisnik.ContainsKey(korisnickoIme))
            {
                if (getAdministratori.ContainsKey(korisnickoIme))
                {
                    if (!getAdministratori[korisnickoIme].Lozinka.Equals(lozinka))
                    {
                        return View("LozinkaError");
                    }
                    Session["Ulogovan"] = getAdministratori[korisnickoIme];
                    i = 0;
                    foreach (KeyValuePair<string, Voznja> kv in getVoznje)
                    {
                        try
                        {
                            if (kv.Value.dispecer.KorisnickoIme.Equals(getAdministratori[korisnickoIme].KorisnickoIme))
                            {
                                i++;
                            }
                        }
                        catch { }
                    }
                    ViewBag.broj = i;
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
            i = 0;
            foreach (KeyValuePair<string, Voznja> kv in getVoznje)
            {
                if (kv.Value.musterija.KorisnickoIme.Equals(getKorisnik[korisnickoIme].KorisnickoIme))
                {
                    i++;
                }
            }
            ViewBag.broj = i;
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
            if (getKorisnik.ContainsKey(v.KorisnickoIme) || getAdministratori.ContainsKey(v.KorisnickoIme) || getVozaci.ContainsKey(v.KorisnickoIme))
                return View("KreiranjeVozacaError");
            a.vozac = v;
            v.automobil = a;
            l.adresa = ad;
            v.lokacija = l;
            v.Zauzet = false;
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
            v.vozac = new Vozac();
            v.komentar = new Komentar();
            v.komentar.korisnik = new Korisnik();
            getVoznje.Add(v.VremePorudzbine.ToString(), v);
            return View("PregledSvihVoznjiMusterije");
        }
        [HttpPost]
        public ActionResult Otkazivanje(string date)
        {
            bool test = false;
            if (getVoznje[date].StatusVoznje == STATUS.NACEKANJU)
            {
                getVoznje[date].StatusVoznje = STATUS.OTKAZANA;
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
            int i = 0;
            foreach (KeyValuePair<string, Voznja> kv in getVoznje)
            {
                if (kv.Value.musterija.KorisnickoIme.Equals((((Korisnik)Session["Ulogovan"]).KorisnickoIme)))
                {
                    i++;
                }
            }
            ViewBag.broj = i;
            return View("HomepageMusterija");
        }
        public ActionResult GoToHomeAdmin()
        {
            int i = 0;
            foreach (KeyValuePair<string, Voznja> kv in getVoznje)
            {
                try
                {
                    if (kv.Value.dispecer.KorisnickoIme.Equals((((Korisnik)Session["Ulogovan"]).KorisnickoIme)))
                    {
                        i++;
                    }
                }
                catch { }
            }
            ViewBag.broj = i;
            return View("HomepageAdministrator");
        }
        public ActionResult GoToHomeVozac()
        {
            return View("HomepageVozac");
        }
        [HttpPost]
        public ActionResult IzmeniVoznju(string date)
        {
            ViewBag.voznja = getVoznje[date];
            return View("IzmenaVoznje");
        }
        [HttpPost]
        public ActionResult OstavljanjeKomentara(string date)
        {
            ViewBag.voznja = getVoznje[date];
            return View("Komentar");
        }
        [HttpPost]
        public ActionResult PrikaziVoznju(string date)
        {
            ViewBag.voznja = getVoznje[date];
            return View("PrikazVoznje");
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
        public ActionResult Komentarisi(string date, string komentar, int ocena)
        {
            getVoznje[date].komentar = new Komentar();
            getVoznje[date].komentar.DatumObjave = DateTime.Now;            
            getVoznje[date].komentar.Opis = komentar;
            getVoznje[date].komentar.voznja = getVoznje[date];
            getVoznje[date].komentar.Ocena = ocena;
            try
            {
                getVoznje[date].komentar.korisnik = (Vozac)Session["Ulogovan"];
                return View("HomepageVozac");               
            }
            catch
            {
                getVoznje[date].komentar.korisnik = (Korisnik)Session["Ulogovan"];
                return View("PregledSvihVoznjiMusterije");
            }
        }

        public ActionResult GoToKreiranjeVoznje()
        {
            ViewBag.vozaci = getVozaci;
            return View("KreirajVoznju");
        }
        public ActionResult KreirajVoznju(Lokacija l, Adresa ad, TIPAUTOMOBILA tip, string vozac)
        {
            l.adresa = ad;
            Voznja v = new Voznja();
            v.LokacijaDolaskaTaksija = l;
            v.VremePorudzbine = DateTime.Now;
            v.ZeljenoVozilo = tip;
            v.StatusVoznje = STATUS.FORMIRANA;
            v.dispecer = (Korisnik)Session["Ulogovan"];
            v.vozac = getVozaci[vozac];
            getVozaci[vozac].Zauzet = true;
            v.komentar = new Komentar();
            v.komentar.korisnik = new Korisnik();
            v.musterija = new Korisnik();
            v.musterija.KorisnickoIme = "";
            getVoznje.Add(v.VremePorudzbine.ToString(), v);
            return View("HomepageAdministrator");
        }

        public ActionResult GoToProveraVoznji()
        {
            ViewBag.voznje = getVoznje;
            return View("ProveriVoznje");
        }
        [HttpPost]
        public ActionResult DodeliVoznju(string date)
        {
            getVoznje[date].dispecer = (Korisnik)Session["Ulogovan"];
            ViewBag.voznja = getVoznje[date].VremePorudzbine.ToString();
            ViewBag.vozaci = getVozaci;
            ViewBag.auto = getVoznje[date].ZeljenoVozilo;
            return View("DodeliVozacaVoznji");
        }
        [HttpPost]
        public ActionResult DodelaVozacaVoznji(string vozac, string date)
        {
            getVozaci[vozac].Zauzet = true;
            getVoznje[date].vozac = getVozaci[vozac];
            getVoznje[date].StatusVoznje = STATUS.OBRADJENA;
            return View("HomepageAdministrator");
        }
        public ActionResult GoToPromenaStatusaVoznje()
        {
            if(((Vozac)Session["Ulogovan"]).Zauzet)
                return View("PromeniStatusVoznje");
            return View("PromeniStatusVoznjeError");
        }
        [HttpPost]
        public ActionResult PromenaStatusaVoznje(STATUS status)
        {
            foreach (KeyValuePair<string, Voznja> kv in getVoznje)
            {
                if (kv.Value.vozac.KorisnickoIme != null)
                {
                    if (kv.Value.vozac.KorisnickoIme.Equals(((Vozac)Session["Ulogovan"]).KorisnickoIme))
                    {
                        kv.Value.StatusVoznje = status;
                        ((Vozac)Session["Ulogovan"]).Zauzet = false;
                        if (status == STATUS.NEUSPESNA)
                        {
                            ViewBag.voznja = kv.Value;
                            return View("Komentar");
                        }
                        else
                        {
                            return View("OdredisteUnos");
                        }
                    }
                }
            }
            return View("HomepageVozac");
        }
        [HttpPost]
        public ActionResult OdredisteIznos(Lokacija l, Adresa ad, double cena)
        {
            l.adresa = ad;
            foreach (KeyValuePair<string, Voznja> kv in getVoznje)
            {
                if (kv.Value.vozac.KorisnickoIme != null)
                {
                    if (kv.Value.vozac.KorisnickoIme.Equals(((Vozac)Session["Ulogovan"]).KorisnickoIme))
                    {
                        kv.Value.Odrediste = l;
                        kv.Value.Iznos = cena;
                        break;
                    }
                }
            }
            return View("HomepageVozac");
        }

        public ActionResult GoToSveVoznje()
        {
            ViewBag.voznje = getVoznje;
            return View("IzlistajSveVoznje");
        }
    }
}