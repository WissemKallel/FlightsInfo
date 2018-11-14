using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightsInfo.Models;
using FlightsInfo.Repositories;
using FlightsInfo.Helpers;

namespace FlightsInfo.Services
{
    /// <summary>
    /// Defines a service to manage flights
    /// </summary>
    public interface IFlightService
    {
        /// <summary>
        /// Get list of flights to display from storage
        /// </summary>
        /// <param name="flightToDisplayList"> contains the list of flights to diplay if succeeded, otherwise null</param>
        /// <returns> true if succeess, otherwise false and an error message </returns>
        Result<bool> GetFlightsToDisplay(out List<FlightToDisplay> flightToDisplayList);

        /// <summary>
        /// Get flight to display from storage
        /// </summary>
        /// <param name="flightID"> ID of the flight to diplay </param>
        /// <param name="flightToDisplay"> contains the flight to diplay if succeeded, otherwise null </param>
        /// <returns> true if succeess, otherwise false and an error message </returns>
        Result<bool> GetFlightToDisplay(int flightID, out FlightToDisplay output);

        /// <summary>
        /// Add flight to storage
        /// </summary>
        /// <param name="flight"> flight to be added </param>
        /// <returns> result status and an optional error message </returns>
        Result<AddResultStatus> AddFlight(FlightToDisplay flight);

        /// <summary>
        /// Edit flight in storage
        /// </summary>
        /// <param name="flight"> flight to be edited </param>
        /// <returns> result status and an optional error message </returns>
        Result<EditResultStatus> EditFlight(FlightToDisplay flight);

        /// <summary>
        /// Remove flight from storage
        /// </summary>
        /// <param name="id"> ID of the flight to be removed </param>
        /// <returns> result status and an optional error message </returns>
        Result<RemoveResultStatus> RemoveFlight(int id);

        /// <summary>
        /// Get the list of Airports stored
        /// </summary>
        List<Airport> Airports { get;  }

        /// <summary>
        /// Get the list of Aircrafts stored
        /// </summary>
        List<Aircraft> Aircrafts { get; }

        /// <summary>
        /// Dispose of the storage
        /// </summary>
        void DisposeOfStorage();
    }

    public enum AddResultStatus
    {
        Success,
        AlreadyExists,
        SameAirportsChosen,
        DataError,
        Failure
    }
    public enum EditResultStatus
    {
        Success,
        AlreadyExists,
        SameAirportsChosen,
        EntriesNotChanged,
        DataError,
        Failure
    }

    public enum RemoveResultStatus
    {
        Success,
        Failure
    }
}
