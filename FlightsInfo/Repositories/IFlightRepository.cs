using System.Collections.Generic;
using System.Data.Entity;
using FlightsInfo.Models;
using FlightsInfo.Helpers;

namespace FlightsInfo.Repositories
{
    public interface IFlightRepository
    {
        /// <summary>
        /// Get the list of Flights from the DB
        /// </summary>
        List<Flight> Flights { get; }

        /// <summary>
        /// Get the list of Aircrafts from the DB
        /// </summary>
        List<Aircraft> Aircrafts { get; }

        /// <summary>
        /// Get the list of Airports from the DB
        /// </summary>
        List<Airport> Airports { get; }

        /// <summary>
        /// Add Flight to the DB
        /// </summary>
        /// <param name="flight"></param>
        /// <returns> true if successful, otherwise false and an error message </returns>
        Result<bool> AddFlight(Flight flight);

        /// <summary>
        /// Edit the flight in the DB with the new values
        /// </summary>
        /// <param name="flight"></param>
        /// <returns> true if successful, otherwise false and an error message </returns>
        Result<bool> EditFlight(Flight flight);

        /// <summary>
        /// Remove Flight using ID from the DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns> true if successful, otherwise false and an error message </returns>
        Result<bool> RemoveFlight(int id);

        /// <summary>
        /// Find the Aircraft from ID in the DB
        /// </summary>
        /// <param name="id"> Aircraft ID </param>
        /// <param name="aircraft"> contains the aircraft object, if found, otherwise null </param>
        /// <returns> true if aircraft found, otherwise false and an error message </returns>
        Result<bool> FindAircraft(int id, out Aircraft aircraft);

        /// <summary>
        /// Find the Airport from ID in the DB
        /// </summary>
        /// <param name="id"> Airpport ID </param>
        /// <param name="airport"> contains the airport object, if found, otherwise null </param>
        /// <returns> true if airport found, otherwise false and an error message </returns>
        Result<bool> FindAirport(int id, out Airport airport);

        /// <summary>
        /// Find the Flight from ID in the DB
        /// </summary>
        /// <param name="id"> Flight ID </param>
        /// <param name="flight"> contains the Flight object, if found, otherwise null </param>
        /// <returns> true if flight found, otherwise false and an error message </returns>
        Result<bool> FindFlight(int id, bool noTracking, out Flight flight);

        /// <summary>
        /// Check if the flight entries already exist in the DB
        /// </summary>
        /// <param name="flight"></param>
        /// <returns> true if the same entries are found in the DB, otherwise false</returns>
        bool CheckFlightEntriesAlreadyExist(FlightToDisplay flight);

        /// <summary>
        /// Dispose of the data base
        /// </summary>
        void DisposeOfDataBase();
    }
}