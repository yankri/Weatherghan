using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.IO;
using Rotativa;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace MVCWeatherghan.Models
{
    public class Weatherdata
    {
        [Key, Column(Order = 0)]
        public string AirportCode { get; set; }
        [Key, Column(Order = 1)]
        [Required(ErrorMessage = "A year is required")]
        public string Year { get; set; }

        [RegularExpression(@"^\d{5}$")]
        [StringLength(5)]
        [Required(ErrorMessage = "A zip  is required")]
        public string ZipCode { get; set; }
        public string MaxTempData { get; set; }
        public string CloudCoverData { get; set; }
        public Dictionary<int,string> PatternRows { get; set; }

        public Weatherdata (){
            ZipCode = "";
            Year = "";
            PatternRows = new Dictionary<int, string>();
        }
        public string GetRowColor(int temp)
        {
            string color;

            if (temp <= 22)
            {
                color = "purple";
                return color;
            }
            else if (temp >= 23 && temp <= 32)
            {
                color = "teal";
                return color;
            }
            else if (temp >= 33 && temp <= 43)
            {
                color = "darkturquoise";
                return color;
            }
            else if (temp >= 44 && temp <= 53)
            {
                color = "aqua";
                return color;
            }
            else if (temp >= 54 && temp <= 66)
            {
                color = "green";
                return color;
            }
            else if (temp >= 67 && temp <= 77)
            {
                color = "yellow";
                return color;
            }
            else if (temp >= 78 && temp <= 88)
            {
                color = "orange";
                return color;
            }
            else
            {
                color = "red";
                return color;
            }
        }
        public Dictionary<int, string> PatternData (List<string> weatherData)
        {
            List<string> MaxTempData = new List<string>();

            for (int i = 0; i < weatherData.Count; i++)
            {
                string[] splits = weatherData[i].Split(',');

                MaxTempData.Add(splits[1]);
            }

            PatternRows = new Dictionary<int, string>();
            int num;
            for (int i = 0; i < MaxTempData.Count; i++)
            {
                bool first = int.TryParse(MaxTempData[i], out num);
                if (!first)
                {
                    continue;
                }
                else
                {
                    PatternRows.Add(i + 1, GetRowColor(int.Parse(MaxTempData[i])));
                }
            }

            return PatternRows;
        }
        public Dictionary<int, string> PatternDataForAlreadyinDBData(List<string> weatherData)
        {
            List<string> MaxTempData = new List<string>();

            for (int i = 0; i < weatherData.Count; i++)
            {
                string[] splits = weatherData[i].Split(',');

                MaxTempData.Add(splits[0]);
            }

            int num;
            for (int i = 0; i < MaxTempData.Count; i++)
            {
                bool first = int.TryParse(MaxTempData[i], out num);
                if (!first)
                {
                    continue;
                }
                else
                {
                    PatternRows.Add(i + 1, GetRowColor(int.Parse(MaxTempData[i])));
                }
            }

            return PatternRows;
        }

        public string ZipHttpGet(string zip)
        {
            string zipurl = @"http://www.travelmath.com/nearest-airport/" + zip;

            StringBuilder sb = new StringBuilder();

            HttpWebRequest req = WebRequest.Create(zipurl)
                                 as HttpWebRequest;
            string result = null;
            using (HttpWebResponse resp = req.GetResponse()
                                          as HttpWebResponse)
            {
                StreamReader reader =
                    new StreamReader(resp.GetResponseStream());
                while (!reader.EndOfStream)
                {
                    result = reader.ReadLine();
                    sb.Append(result);
                }
            }
            return sb.ToString();
        }

        public List<string> HttpGetWeatherData(string url)
        {
            List<string> weatherData = new List<string>();

            HttpWebRequest req = WebRequest.Create(url)
                                 as HttpWebRequest;
            string result = null;
            using (HttpWebResponse resp = req.GetResponse()
                                          as HttpWebResponse)
            {
                StreamReader reader =
                    new StreamReader(resp.GetResponseStream());
                while (!reader.EndOfStream)
                {
                    result = reader.ReadLine();
                    weatherData.Add(result);
                }
            }
            weatherData.RemoveAt(0);
            weatherData.RemoveAt(0);
            return weatherData;
        }

        public string HTTPGetAirportCode(string zipcode)
        {
            string zipcodePageData = ZipHttpGet(zipcode);
            string pattern = @"(K{1}[A-Z]{3})";

            Match airport = Regex.Match(zipcodePageData, pattern);

            return airport.ToString();
        }

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

        public IEnumerable<Weatherdata> GetWeatherdataFromDB(string airportcode, string year) //gets the weatherdata from the database
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
                        yield return new Weatherdata
                        {
                            AirportCode = reader.GetString(reader.GetOrdinal("AirportCode")),
                            Year = reader.GetString(reader.GetOrdinal("Year")),
                            MaxTempData = reader.GetString(reader.GetOrdinal("MaxTempData")),
                        };
                    };
                }
            }
        }

        public string GetAirportCodeFromDB(string zipcode) //gets the airport code from the database using the zipcode
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

        public Dictionary<int, string> GetPattern(string zipcode, string year) //displays the pattern given a zipcode and year. also takes care of adding data in to the database if needed after retrieving. 
        {
            string airportcode = GetAirportCodeFromDB(zipcode); //get data from database

            if (String.IsNullOrEmpty(airportcode)) //if it's not in the database... do this
            {
                airportcode = HTTPGetAirportCode(zipcode); //gets airport code
                InsertAiportCodeInfo(zipcode, airportcode); //adds new code and zipcode to database
            }

            var Weatherdata = GetWeatherdataFromDB(airportcode, year).ToList(); //get data from database

            if (Weatherdata.Count() == 0) //if the data isn't in the database... do this 
            {
                string url = "http://www.wunderground.com/history/airport/" + airportcode + "/" + year + "/1/1/CustomHistory.html?dayend=31&monthend=12&yearend=" + year + "&req_city=&req_state=&req_statename=&reqdb.zip=&reqdb.magic=&reqdb.wmo=&format=1";
                List<string> weatherData = HttpGetWeatherData(url); //gets weather data from wunderground
                PatternRows = PatternData(weatherData);
                string maxtemp = ConvertListToString(weatherData);
                InsertWeatherdata(airportcode, year, maxtemp); //inserts new data into the database
                return PatternRows;
            }
            else
            {
                string[] splits = Weatherdata[0].MaxTempData.ToString().Split(',');
                List<string> weatherdatas = new List<string>();
                foreach (var piece in splits)
                {
                    weatherdatas.Add(piece);
                }
                PatternRows = PatternDataForAlreadyinDBData(weatherdatas);
                return PatternRows;
            }
        }

        //make below an actionresult in controller
        public Dictionary<int, string> PatternPDFView(string zipcode, string year) //gets pattern data from database for pdf view
        {
            string airportcode = GetAirportCodeFromDB(zipcode); //get data from database

            var Weatherdata = GetWeatherdataFromDB(airportcode, year).ToList(); //get data from database

            string[] splits = Weatherdata.First().MaxTempData.ToString().Split(',');
            List<string> weatherdatas = new List<string>();

            foreach (var piece in splits)
            {
                weatherdatas.Add(piece);
            }

            PatternRows = PatternDataForAlreadyinDBData(weatherdatas);
            return PatternRows;
        }
    }
}
