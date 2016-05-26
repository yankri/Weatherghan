using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace MVCWeatherghan.Models
{
    public class Weatherdata
    {
        public string AirportCode { get; set; }
        public string Year { get; set; }
        public string MaxTempData { get; set; }
        public string CloudCoverData { get; set; }
        public string ZipCode { get; set; }

        public static string GetRowColor(int temp)
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
            else if (temp <= 78 && temp <= 88)
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
        public static Dictionary<int, string> PatternData (List<string> weatherData)
        {
            List<string> MaxTempData = new List<string>();

            for (int i = 0; i < weatherData.Count; i++)
            {
                string[] splits = weatherData[i].Split(',');

                MaxTempData.Add(splits[1]);
            }

            Dictionary<int, string> RowNumColor = new Dictionary<int, string>();
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
                    RowNumColor.Add(i + 1, GetRowColor(int.Parse(MaxTempData[i])));
                }
            }

            return RowNumColor;
        }
        public static Dictionary<int, string> PatternDataForAlreadyinDBData(List<string> weatherData)
        {
            List<string> MaxTempData = new List<string>();

            for (int i = 0; i < weatherData.Count; i++)
            {
                string[] splits = weatherData[i].Split(',');

                MaxTempData.Add(splits[0]);
            }

            Dictionary<int, string> RowNumColor = new Dictionary<int, string>();
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
                    RowNumColor.Add(i + 1, GetRowColor(int.Parse(MaxTempData[i])));
                }
            }

            return RowNumColor;
        }

        public static string ZipHttpGet(string zip)
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

        public static List<string> HttpGetWeatherData(string url)
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

        public static string GetAirportCode(string zipcode)
        {
            string zipcodePageData = ZipHttpGet(zipcode);
            string pattern = @"(K{1}[A-Z]{3})";

            Match airport = Regex.Match(zipcodePageData, pattern);

            return airport.ToString();
        }
    }
}
