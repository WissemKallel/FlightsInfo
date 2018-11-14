using FlightsInfo.Models;

namespace FlightsInfo.Helpers
{
    /// <summary>
    /// Defines a helper class to calculate flight info
    /// </summary>
    public interface IFlightCalculator
    {
        /// <summary>
        /// Calculate distance between earth coordinates (latitude 1, longitude 1) and (latitide 2, longitude 2) in Kilometres
        /// The method throws an ArgumentException if the arguments are out of the specified range
        /// </summary>
        /// <param name="lat1"> latitude 1, in degrees (between -90 and 90) </param>
        /// <param name="lon1"> longitude 1, in degrees (between -180 and 180) </param>
        /// <param name="lat2"> latitide 2, in degrees (between -90 and 90) </param>
        /// <param name="lon2"> longitude 2, in degrees (between -180 and 180) </param>
        /// <returns></returns>
        double CalculateDistanceBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2);

        /// <summary>
        /// Calculates the average fuel consumption of the aircraft for the specified flight distance
        /// The method throws an ArgumentException if the arguments are out of the specified range
        /// </summary>
        /// <param name="aircraft"> The aircraft (AverageTakeoffEffortInLitre and AverageConsumptionPerKmInLitre should be strictly positive) </param>
        /// <param name="distanceInKm"> Distance (in Km) of the flight (should be strictly positive) </param>
        /// <returns></returns>
        double CalculateFlightFuelConsumption(Aircraft aircraft, double distanceInKm);
    }
}