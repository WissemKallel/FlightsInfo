using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace FlightsInfo.Models
{
    public class Airport
    {
        public int AirportID { get; set; }
        public string AirportName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}