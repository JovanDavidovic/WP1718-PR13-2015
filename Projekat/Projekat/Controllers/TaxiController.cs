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
        private string putanja = "C:/FAX/WEB/Projekat/";
        private void IspisVozaca()
        {
            System.IO.StreamWriter file2 = new System.IO.StreamWriter(putanja + "WP1718-PR13-2015/Projekat/packages/Vozaci.txt");

            foreach (KeyValuePair<string, Vozac> kv in getVozaci)
            {
                file2.WriteLine(kv.Value.KorisnickoIme + " " + kv.Value.Ime + " " + kv.Value.Prezime + " " + kv.Value.Lozinka + " " + kv.Value.Email + " " + kv.Value.Telefon.Replace(" ", string.Empty) + " " + kv.Value.JMBG + " " + kv.Value.Pol + " " + kv.Value.automobil.BrojVozila + " " + kv.Value.automobil.Godiste +  " " + kv.Value.automobil.RegistarskaOznaka + " " + kv.Value.automobil.TipAutomobila + " " + kv.Value.lokacija.Xkoordinata + " " + kv.Value.lokacija.Ykoordinata +  " " + kv.Value.lokacija.adresa.PozivniBroj + " " + kv.Value.lokacija.adresa.Broj + " " + kv.Value.lokacija.adresa.Mesto + " " + kv.Value.lokacija.adresa.Ulica);
            }
            file2.Close();
        }

        private void IspisKorisnici()
        {
            System.IO.StreamWriter file3 = new System.IO.StreamWriter(putanja + "WP1718-PR13-2015/Projekat/packages/Korisnici.txt");

            foreach (KeyValuePair<string, Korisnik> kv in getKorisnik)
            {
                file3.WriteLine(kv.Value.KorisnickoIme + " " + kv.Value.Ime + " " + kv.Value.Prezime + " " + kv.Value.Lozinka + " " + kv.Value.Email + " " + kv.Value.Telefon.Replace(" ", string.Empty) + " " + kv.Value.JMBG + " " + kv.Value.Pol);
            }
            file3.Close();
        }
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
            IspisKorisnici();
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
                    i = 0;
                    foreach (KeyValuePair<string, Voznja> kv in getVoznje)
                    {
                        try
                        {
                            if (kv.Value.vozac.KorisnickoIme.Equals((((Korisnik)Session["Ulogovan"]).KorisnickoIme)))
                            {
                                i++;
                            }
                        }
                        catch { }
                    }
                    ViewBag.broj = i;
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
                try
                {
                    if (kv.Value.musterija.KorisnickoIme.Equals(getKorisnik[korisnickoIme].KorisnickoIme))
                    {
                        i++;
                    }
                }
                catch { }
            }
            ViewBag.broj = i;
            return View("HomepageMusterija");
        }

        public ActionResult Izmena()
        {
            return View("Izmena");
        }
        public ActionResult IzmenaVozac()
        {
            return View("IzmenaVozac");
        }
        public ActionResult IzmenaAdmin()
        {
            return View("IzmenaAdmin");
        }

        public ActionResult Izmeni(Korisnik k)
        {
            if (!((Korisnik)Session["Ulogovan"]).KorisnickoIme.Equals(k.KorisnickoIme) && (getKorisnik.ContainsKey(k.KorisnickoIme) || getAdministratori.ContainsKey(k.KorisnickoIme) || getVozaci.ContainsKey(k.KorisnickoIme)))
            {
                ViewBag.korisnik = k;
                return View("SignUpError2");
            }
            if (!((Korisnik)Session["Ulogovan"]).KorisnickoIme.Equals(k.KorisnickoIme))
            {
                getKorisnik.Remove(((Korisnik)Session["Ulogovan"]).KorisnickoIme);
            }
            k.Uloga = ULOGA.MUSTERIJA;
            getKorisnik[k.KorisnickoIme] = k;
            Session["Ulogovan"] = getKorisnik[k.KorisnickoIme];
            int i = 0;
            foreach (KeyValuePair<string, Voznja> kv in getVoznje)
            {
                if (kv.Value.musterija.KorisnickoIme.Equals((((Korisnik)Session["Ulogovan"]).KorisnickoIme)))
                {
                    i++;
                }
            }
            IspisKorisnici();
            ViewBag.broj = i;
            return View("HomepageMusterija");
        }

        public ActionResult IzmeniAdmin(Korisnik k)
        {
            if (!((Korisnik)Session["Ulogovan"]).KorisnickoIme.Equals(k.KorisnickoIme) && (getKorisnik.ContainsKey(k.KorisnickoIme) || getAdministratori.ContainsKey(k.KorisnickoIme) || getVozaci.ContainsKey(k.KorisnickoIme)))
            {
                ViewBag.korisnik = k;
                return View("SignUpError");
            }
            if (!((Korisnik)Session["Ulogovan"]).KorisnickoIme.Equals(k.KorisnickoIme))
            {
                getAdministratori.Remove(((Korisnik)Session["Ulogovan"]).KorisnickoIme);
            }
            k.Uloga = ULOGA.DISPECER;
            getAdministratori[k.KorisnickoIme] = k;
            Session["Ulogovan"] = getAdministratori[k.KorisnickoIme];
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

        public ActionResult IzmeniVozac(Vozac v, Automobil a, Lokacija l, Adresa ad)
        {
            a.vozac = v;
            v.automobil = a;
            l.adresa = ad;
            v.lokacija = l;
            v.Zauzet = false;
            v.Uloga = ULOGA.VOZAC;
            if (!((Vozac)Session["Ulogovan"]).KorisnickoIme.Equals(v.KorisnickoIme) && (getKorisnik.ContainsKey(v.KorisnickoIme) || getAdministratori.ContainsKey(v.KorisnickoIme) || getVozaci.ContainsKey(v.KorisnickoIme)))
            {
                ViewBag.vozac = v;
                return View("IzmenaVozacaError");
            }
            if (!((Vozac)Session["Ulogovan"]).KorisnickoIme.Equals(v.KorisnickoIme))
            {
                getVozaci.Remove(((Vozac)Session["Ulogovan"]).KorisnickoIme);
            }
            getVozaci[v.KorisnickoIme] = v;
            Session["Ulogovan"] = getVozaci[v.KorisnickoIme];
            int i = 0;
            foreach (KeyValuePair<string, Voznja> kv in getVoznje)
            {
                try
                {
                    if (kv.Value.vozac.KorisnickoIme.Equals((((Korisnik)Session["Ulogovan"]).KorisnickoIme)))
                    {
                        i++;
                    }
                }
                catch { }
            }
            IspisVozaca();
            ViewBag.broj = i;
            return View("HomepageVozac");
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
            v.Zauzet = false;
            v.Uloga = ULOGA.VOZAC;
            if (getKorisnik.ContainsKey(v.KorisnickoIme) || getAdministratori.ContainsKey(v.KorisnickoIme) || getVozaci.ContainsKey(v.KorisnickoIme))
            {
                ViewBag.vozac = v;
                return View("KreiranjeVozacaError");
            }
            getVozaci.Add(v.KorisnickoIme, v);
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
            IspisVozaca();
            ViewBag.broj = i;
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
            int i = 0;
            foreach (KeyValuePair<string, Voznja> kv in getVoznje)
            {
                try
                {
                    if (kv.Value.vozac.KorisnickoIme.Equals((((Korisnik)Session["Ulogovan"]).KorisnickoIme)))
                    {
                        i++;
                    }
                }
                catch { }
            }
            ViewBag.broj = i;
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
            int i = 0;
            foreach (KeyValuePair<string, Voznja> kv in getVoznje)
            {
                try
                {
                    if (kv.Value.vozac.KorisnickoIme.Equals((((Korisnik)Session["Ulogovan"]).KorisnickoIme)))
                    {
                        i++;
                    }
                }
                catch { }
            }
            ViewBag.broj = i;
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
            if (getVoznje[date].StatusVoznje == STATUS.NACEKANJU)
            {
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
        public ActionResult Komentarisi(int ocena, string date, string komentar)
        {
            getVoznje[date].komentar = new Komentar();
            getVoznje[date].komentar.DatumObjave = DateTime.Now;
            getVoznje[date].komentar.Opis = komentar;
            getVoznje[date].komentar.voznja = getVoznje[date];
            getVoznje[date].komentar.Ocena = ocena;
            try
            {
                getVoznje[date].komentar.korisnik = (Vozac)Session["Ulogovan"];
                int i = 0;
                foreach (KeyValuePair<string, Voznja> kv in getVoznje)
                {
                    try
                    {
                        if (kv.Value.vozac.KorisnickoIme.Equals((((Korisnik)Session["Ulogovan"]).KorisnickoIme)))
                        {
                            i++;
                        }
                    }
                    catch { }
                }
                ViewBag.broj = i;
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
        public ActionResult GoToProveraVoznjiVozac()
        {
            if (((Vozac)Session["Ulogovan"]).Zauzet)
            {
                return View("VozacPreuzimaError");
            }
            ViewBag.voznje = getVoznje;
            ViewBag.tip = ((Vozac)Session["Ulogovan"]).automobil.TipAutomobila;
            return View("ProveriVoznjeVozac");
        }
        [HttpPost]
        public ActionResult DodeliVoznjuVozac(string date)
        {
            getVoznje[date].vozac = (Vozac)Session["Ulogovan"];
            getVozaci[((Vozac)Session["Ulogovan"]).KorisnickoIme].Zauzet = true;
            getVoznje[date].StatusVoznje = STATUS.PRIHVACENA;
            int i = 0;
            foreach (KeyValuePair<string, Voznja> kv in getVoznje)
            {
                try
                {
                    if (kv.Value.vozac.KorisnickoIme.Equals((((Korisnik)Session["Ulogovan"]).KorisnickoIme)))
                    {
                        i++;
                    }
                }
                catch { }
            }
            ViewBag.broj = i;
            return View("HomepageVozac");
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
            if (((Vozac)Session["Ulogovan"]).Zauzet)
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
                    if (kv.Value.vozac.KorisnickoIme.Equals(((Vozac)Session["Ulogovan"]).KorisnickoIme) && !(kv.Value.StatusVoznje == STATUS.USPESNA || kv.Value.StatusVoznje == STATUS.NEUSPESNA))
                    {

                        ((Vozac)Session["Ulogovan"]).Zauzet = false;
                        if (status == STATUS.NEUSPESNA)
                        {
                            kv.Value.StatusVoznje = status;
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
            int i = 0;
            foreach (KeyValuePair<string, Voznja> kv in getVoznje)
            {
                try
                {
                    if (kv.Value.vozac.KorisnickoIme.Equals((((Korisnik)Session["Ulogovan"]).KorisnickoIme)))
                    {
                        i++;
                    }
                }
                catch { }
            }
            ViewBag.broj = i;
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
                    if (kv.Value.vozac.KorisnickoIme.Equals(((Vozac)Session["Ulogovan"]).KorisnickoIme) && (kv.Value.StatusVoznje == STATUS.PRIHVACENA || kv.Value.StatusVoznje == STATUS.OBRADJENA || kv.Value.StatusVoznje == STATUS.FORMIRANA))
                    {
                        kv.Value.StatusVoznje = STATUS.USPESNA;
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

        public ActionResult FilterMusterija(STATUS status)
        {
            Dictionary<string, Voznja> voznje = new Dictionary<string, Voznja>();
            foreach (KeyValuePair<string, Voznja> kv in getVoznje)
            {
                try
                {
                    if (kv.Value.StatusVoznje == status && kv.Value.musterija.KorisnickoIme.Equals(((Korisnik)Session["Ulogovan"]).KorisnickoIme))
                    {
                        voznje.Add(kv.Key, kv.Value);
                    }
                }
                catch { }
            }
            ViewBag.voznje = voznje;
            ViewBag.broj = voznje.Count();
            return View("Filtrirano");
        }
        public ActionResult SortiranjeMusterija(string sortiranje)
        {
            Dictionary<string, Voznja> voznje = new Dictionary<string, Voznja>();
            if (sortiranje.Equals("ocena"))
                voznje = getVoznje.ToList().OrderByDescending(o => o.Value.komentar.Ocena).ToDictionary(x => x.Key, y => y.Value);
            else
                voznje = getVoznje.ToList().OrderByDescending(o => o.Value.VremePorudzbine).ToDictionary(x => x.Key, y => y.Value);
            ViewBag.voznje = voznje;
            ViewBag.broj = voznje.Count();
            return View("Filtrirano");
        }
        public ActionResult Pretraga()
        {
            return View("Pretraga");
        }
        [HttpPost]
        public ActionResult PretragaParametri(int ocena1, int ocena2, double cena1, double cena2, DateTime? datum1 = null, DateTime? datum2 = null)
        {
            Dictionary<string, Voznja> voznje = new Dictionary<string, Voznja>(getVoznje);

            foreach (KeyValuePair<string, Voznja> kv in getVoznje)
            {

                try
                {
                    if (kv.Value.musterija.KorisnickoIme.Equals(((Korisnik)Session["Ulogovan"]).KorisnickoIme))
                    {
                        if (datum1 == null && datum2 != null && datum2.Value.Date < kv.Value.VremePorudzbine.Date)
                        {
                            voznje.Remove(kv.Key);
                        }
                        else if (datum1 != null && datum2 == null && datum1.Value.Date > kv.Value.VremePorudzbine.Date)
                        {
                            voznje.Remove(kv.Key);
                        }
                        else if (datum1 != null && datum2 != null && (datum1.Value.Date > kv.Value.VremePorudzbine.Date || datum2.Value.Date < kv.Value.VremePorudzbine.Date))
                        {
                            voznje.Remove(kv.Key);
                        }

                        if (ocena1 == 0 && ocena2 != 0 && ocena2 < kv.Value.komentar.Ocena)
                        {
                            voznje.Remove(kv.Key);
                        }
                        else if (ocena1 != 0 && ocena2 == 0 && ocena1 > kv.Value.komentar.Ocena)
                        {
                            voznje.Remove(kv.Key);
                        }
                        else if (ocena1 != 0 && ocena2 != 0 && (ocena1 > kv.Value.komentar.Ocena || ocena2 < kv.Value.komentar.Ocena))
                        {
                            voznje.Remove(kv.Key);
                        }

                        if (cena1 == 0 && cena2 != 0 && cena2 < kv.Value.Iznos)
                        {
                            voznje.Remove(kv.Key);
                        }
                        else if (cena1 != 0 && cena2 == 0 && cena1 > kv.Value.Iznos)
                        {
                            voznje.Remove(kv.Key);
                        }
                        else if (cena1 != 0 && cena2 != 0 && (cena1 > kv.Value.Iznos || cena2 < kv.Value.Iznos))
                        {
                            voznje.Remove(kv.Key);
                        }
                    }
                }
                catch { }
            }

            ViewBag.voznje = voznje;
            ViewBag.broj = voznje.Count();
            return View("Filtrirano");
        }

        public ActionResult PretragaAdmin()
        {
            return View("PretragaAdmin");
        }
        public ActionResult PretragaParametriAdmin(string vozacIme, string vozacPrezime, string musterijaIme, string musterijaPrezime)
        {
            if (vozacIme == null)
                vozacIme = "-";
            if (vozacPrezime == null)
                vozacPrezime = "-";
            if (musterijaIme == null)
                musterijaIme = "-";
            if (musterijaPrezime == null)
                musterijaPrezime = "-";
            Dictionary<string, Voznja> voznje = new Dictionary<string, Voznja>();
            foreach (KeyValuePair<string, Voznja> kv in getVoznje)
            {
                try
                {
                    if (kv.Value.vozac.Ime.Equals(vozacIme) || kv.Value.vozac.Prezime.Equals(vozacPrezime) || kv.Value.musterija.Ime.Equals(musterijaIme) || kv.Value.musterija.Prezime.Equals(musterijaPrezime))
                    {
                        voznje.Add(kv.Key, kv.Value);
                    }
                }
                catch { }
            }
            ViewBag.voznje = voznje;
            ViewBag.broj = voznje.Count();
            return View("FiltriranoAdmin");
        }
    }
}