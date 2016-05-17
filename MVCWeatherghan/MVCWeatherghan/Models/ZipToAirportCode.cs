using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace MVCWeatherghan.Models
{
    public class ZipToAirportCode
    {
        public string ZipCode { get; set; }
        public string AirportCode { get; set; }


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
            return weatherData;
        }

        public static string GetAirportCode (string zipcode)
        {
            string zipcodePageData = ZipHttpGet(zipcode);
            string pattern = @"(K{1}[A-Z]{3})";

            Match airport = Regex.Match(zipcodePageData, pattern);

            return airport.ToString();
        }
    }

    
}