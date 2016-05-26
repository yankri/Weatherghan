using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Rotativa;

namespace MVCWeatherghan.Controllers
{
    public class HomeController : Controller
    {
        public string ConvertListToString(List<string> weatherdata) //method to convert a list to a string for entering data into the database 
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < weatherdata.Count; i++)
            {
                string[] splits = weatherdata[i].Split(',');

                if (splits[1] == "")
                {
                    continue;
                }
                if (weatherdata[i] == weatherdata.Last())
                {
                    sb.Append(splits[1]);
                }
                else
                {
                    sb.Append(splits[1] + ",");
                }
            }
            return sb.ToString();
        }

        public IEnumerable<Models.Weatherdata> GetWeatherdata(string airportcode, string year) //gets the weatherdata from the database
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "SELECT * FROM Weatherdata WHERE AirportCode = '" + airportcode + "' AND Year = '" + year + "'";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new Models.Weatherdata
                        {
                            AirportCode = reader.GetString(reader.GetOrdinal("AirportCode")),
                            Year = reader.GetString(reader.GetOrdinal("Year")),
                            MaxTempData = reader.GetString(reader.GetOrdinal("MaxTempData")),
                        };
                    };
                }
            }
        }

        public string GetAirportCode(string zipcode) //gets the airport code from the database using the zipcode
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "SELECT AirportCode FROM ZipToAirportCode WHERE ZipCode = '" + zipcode + "'";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader.GetString(reader.GetOrdinal("AirportCode"));
                    };
                    return string.Empty;
                }
            }
        }
        public void InsertWeatherdata(string apcode, string year, string weatherdata) //adds the weatherdata into the database
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                string command = "INSERT INTO WeatherData (AirportCode, Year, MaxTempData) VALUES(@param1, @param2, @param3)";
                SqlCommand sqlcmd = new SqlCommand(command, conn);
                sqlcmd.Parameters.AddWithValue("@param1", apcode);
                sqlcmd.Parameters.AddWithValue("@param2", year);
                sqlcmd.Parameters.AddWithValue("@param3", weatherdata);
                sqlcmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        public void InsertAiportCodeInfo(string zipcode, string apcode) //adds airport code and zipcode into the database 
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                string command = "INSERT INTO ZipToAirportCode (ZipCode, AirportCode) VALUES(@param1, @param2)";
                SqlCommand sqlcmd = new SqlCommand(command, conn);
                sqlcmd.Parameters.AddWithValue("@param1", zipcode);
                sqlcmd.Parameters.AddWithValue("@param2", apcode);
                sqlcmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        public ActionResult Index() //homepage
        {
            return View();
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

        public ActionResult DisplayPattern(string zipcode, string year) //displays the pattern given a zipcode and year. also takes care of adding data in to the database if needed after retrieving. 
        {
            string airportcode = GetAirportCode(zipcode); //get data from database

            if (String.IsNullOrEmpty(airportcode)) //if it's not in the database... do this
            {
                airportcode = Models.Weatherdata.GetAirportCode(zipcode); //gets airport code
                InsertAiportCodeInfo(zipcode, airportcode); //adds new code and zipcode to database
            }

            var Weatherdata = GetWeatherdata(airportcode, year).ToList(); //get data from database

            if (Weatherdata.Count() == 0) //if the data isn't in the database... do this 
            {
                string url = "http://www.wunderground.com/history/airport/" + airportcode + "/" + year + "/1/1/CustomHistory.html?dayend=31&monthend=12&yearend=" + year + "&req_city=&req_state=&req_statename=&reqdb.zip=&reqdb.magic=&reqdb.wmo=&format=1";
                List<string> weatherData = Models.Weatherdata.HttpGetWeatherData(url); //gets weather data from wunderground
                Dictionary<int, string> rowsAndColors = Models.Weatherdata.PatternData(weatherData);
                string maxtemp = ConvertListToString(weatherData);
                InsertWeatherdata(airportcode, year, maxtemp); //inserts new data into the database
                return View(rowsAndColors);
            }
            else
            {
                string[] splits = Weatherdata[0].MaxTempData.ToString().Split(',');
                List<string> weatherdatas = new List<string>();
                foreach (var piece in splits)
                {
                    weatherdatas.Add(piece);
                }
                Dictionary<int, string> rowsNColors = Models.Weatherdata.PatternDataForAlreadyinDBData(weatherdatas);
                return View(rowsNColors);
            }
        }

        public ActionResult DisplayPatternPDFView(string zipcode, string year) //gets pattern data from database for pdf view
        {
            string airportcode = GetAirportCode(zipcode); //get data from database

            var Weatherdata = GetWeatherdata(airportcode, year).ToList(); //get data from database

            string[] splits = Weatherdata.First().MaxTempData.ToString().Split(',');
            List<string> weatherdatas = new List<string>();

            foreach (var piece in splits)
            {
                weatherdatas.Add(piece);
            }

            Dictionary<int, string> rowsNColors = Models.Weatherdata.PatternDataForAlreadyinDBData(weatherdatas);
            return View(rowsNColors);
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