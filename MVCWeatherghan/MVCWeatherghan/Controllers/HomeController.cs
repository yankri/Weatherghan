using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Rotativa;
using MVCWeatherghan.Models;

namespace MVCWeatherghan.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() //homepage
        {
            Weatherdata WD = new Weatherdata();
            ViewBag.Title = "Home";
            return View(WD);
        }
        public ActionResult NewHome()
        {
            return View();
        }
        public ActionResult DisplayPattern(string zipcode, string year) //parameters are query strings from the index view input boxes
        {
            Weatherdata wd = new Weatherdata();
            wd.ZipCode = zipcode;
            wd.Year = year;
            wd.PatternRows = wd.GetPattern(wd.ZipCode, wd.Year);
            return View(wd.PatternRows);
        }

        public ActionResult DisplayPatternPDFView (string zipcode, string year)//parameters are query strings from the index view input boxes
        {
            Weatherdata wd = new Weatherdata();
            wd.ZipCode = zipcode;
            wd.Year = year;
            wd.PatternRows = wd.PatternPDFView(wd.ZipCode, wd.Year);
            return View(wd.PatternRows);

        }
        public ActionResult Patterns() //pattern suggestions page
        {
            return View();
        }

        public ActionResult Resources ()
        {
            return View();
        }
        public ActionResult About() //not used yet
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() //not used yet
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
           
        public ActionResult PatternToPDF(string zip, string viewyear)
        {
            return new ActionAsPdf("DisplayPatternPDFView", new { zipcode = zip, year = viewyear }) { FileName = "MyWeatherghanPattern.pdf" };
        }

        public ActionResult Gallery() //gallery page
        {
            return View();
        }
    }
}