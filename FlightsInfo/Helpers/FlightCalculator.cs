using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlightsInfo.Models;

namespace FlightsInfo.Helpers
{
    public class FlightCalculator : IFlightCalculator
    {
        /// <summary>
        /// Defined in implemented interface
        /// </summary>
        public double CalculateDistanceBetweenCoordinates(double lat1, double lon1, double lat2, double lon2)
        {
            if (lat1 < -90 || lat1 > 90) throw new ArgumentException("latitude1 value out of range");
            if (lon1 < -180 || lon1 > 180) throw new ArgumentException("longitude1 value out of range");
            if (lat2 < -90 || lat2 > 90) throw new ArgumentException("latitude2 value out of range");
            if (lon2 < -180 || lon2 > 180) throw new ArgumentException("longitude2 value out of range");

            var earthRadiusKm = 6371;

            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            lat1 = DegreesToRadians(lat1);
            lat2 = DegreesToRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadiusKm * c;
        }

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <param name="degrees"> degrees</param>
        /// <returns></returns>
        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        /// <summary>
        /// Defined in implemented interface
        /// </summary>
        public double CalculateFlightFuelConsumption(Aircraft aircraft, double distanceInKm)
        {
            if (aircraft == null) throw new ArgumentNullException("aircraft");
            if (aircraft.AverageTakeoffEffortInLitre <=0 ) throw new ArgumentException("aircraft.AverageTakeoffEffortInLitre is negative");
            if (aircraft.AverageConsumptionPerKmInLitre <= 0) throw new ArgumentException("aircraft.AverageConsumptionPerKmInLitre is negative");

            if (distanceInKm <= 0) throw new ArgumentException("distanceInKm is negative");

            return aircraft.AverageConsumptionPerKmInLitre * distanceInKm + aircraft.AverageTakeoffEffortInLitre;
        }
    }
}