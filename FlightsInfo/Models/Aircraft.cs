using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightsInfo.Models
{
    public class Aircraft
    {
        public int AircraftID { get; set; }
        public string AircraftName { get; set; }
        public double AverageConsumptionPerKmInLitre { get; set; }
        public double AverageTakeoffEffortInLitre { get; set; }
    }
}