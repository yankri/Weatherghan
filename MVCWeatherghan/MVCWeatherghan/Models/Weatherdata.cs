using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCWeatherghan.Models
{
    public class Weatherdata
    {
        public string AirportCode { get; set; }
        public string Year { get; set; }
        public string MaxTempData { get; set; }
        public string CloudCoverData { get; set; }

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

            for (int i = 2; i < weatherData.Count; i++)
            {
                string[] splits = weatherData[i].Split(',');

                if (splits[1] == "")
                {
                    continue;
                }

                MaxTempData.Add(splits[1]);
            }

            Dictionary<int, string> RowNumColor = new Dictionary<int, string>();

            for (int i = 0; i < MaxTempData.Count; i++)
            {
                RowNumColor.Add(i + 1, GetRowColor(int.Parse(MaxTempData[i])));
            }

            return RowNumColor;
        }
    }
}