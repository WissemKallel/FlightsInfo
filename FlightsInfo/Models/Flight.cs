using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using FlightsInfo.Helpers;

namespace FlightsInfo.Models
{
    public class Flight
    {
        public int FlightID { get; set; }
        public int DepartureAirportID { get; set; }
        public int DestinationAirportID { get; set; }
        public int AircraftID { get; set; }
        public string Distance { get; set; }
        public string FuelConsumption { get; set; }
    }

    /// <summary>
    /// Class holding data to display to user
    /// </summary>
    public class FlightToDisplay : Flight
    {
        public string DepartureAirportName { get; set; }
        public string DestinationAirportName { get; set; }
        public string AircraftName { get; set; }
    }

    public class FlightDBContext : DbContext, IFlightDBContext
    {
        public IDbSet<Flight> Flights { get; set; }
        public IDbSet<Airport> Airports { get; set; }
        public IDbSet<Aircraft> Aircrafts { get; set; }
    }
}