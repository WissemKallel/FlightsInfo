using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using FlightsInfo.Models;
using FlightsInfo.Repositories;
using FlightsInfo.Helpers;

namespace FlightsInfo.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightCalculator flightCalculator;
        private readonly IFlightRepository flightRepository;

        public FlightService(IFlightCalculator flightCalculator, IFlightRepository flightRepository)
        {
            this.flightCalculator = flightCalculator ?? throw new ArgumentNullException("flightCalculator");
            this.flightRepository = flightRepository ?? throw new ArgumentNullException("flightRepository");
        }

        public Result<bool> GetFlightToDisplay(int flightID, out FlightToDisplay flightToDisplay)
        {
            flightToDisplay = null;

            //Get Flight Info
            var resFlight = flightRepository.FindFlight(flightID, false, out Flight flight)
                ?? throw new ArgumentNullException("resFlight");
            if (!resFlight.Status)
                return new Result<bool>(false, resFlight.Info);
            if (flight == null) throw new ArgumentNullException("flight");

            //Get departure Airport name
            var resDepAirport = flightRepository.FindAirport(flight.DepartureAirportID, out Airport departureAirport)
                ?? throw new ArgumentNullException("resDepAirport");
            if (!resDepAirport.Status)
                return new Result<bool>(false, resDepAirport.Info);
            if (departureAirport == null) throw new ArgumentNullException("departureAirport");

            //Get arrival Airport name
            var resDestAirport = flightRepository.FindAirport(flight.DestinationAirportID, out Airport destinationAirport)
                ?? throw new ArgumentNullException("resDestAirport");
            if (!resDestAirport.Status)
                return new Result<bool>(false, resDestAirport.Info);
            if (destinationAirport == null) throw new ArgumentNullException("destinationAirport");

            //Get Aircraft name
            var resAircraft = flightRepository.FindAircraft(flight.AircraftID, out Aircraft aircraft)
                ?? throw new ArgumentNullException("resAircraft");
            if (!resAircraft.Status)
                return new Result<bool>(false, resAircraft.Info);
            if (aircraft == null) throw new ArgumentNullException("aircraft");

            flightToDisplay = new FlightToDisplay()
            {
                FlightID = flight.FlightID,
                AircraftID = flight.AircraftID,
                DepartureAirportID = flight.DepartureAirportID,
                DestinationAirportID = flight.DestinationAirportID,
                Distance = flight.Distance,
                FuelConsumption = flight.FuelConsumption,
                DepartureAirportName = departureAirport.AirportName,
                DestinationAirportName = destinationAirport.AirportName,
                AircraftName = aircraft.AircraftName,
            };

            return new Result<bool>(true);
        }

        public Result<bool> GetFlightsToDisplay(out List<FlightToDisplay> flightToDisplayList)
        {
            flightToDisplayList = new List<FlightToDisplay>();
            Result<bool> res;
            foreach (Flight flight in flightRepository.Flights)
            {
                if (flight == null) throw new ArgumentNullException("flight");
                res = GetFlightToDisplay(flight.FlightID, out FlightToDisplay output)
                    ?? throw new ArgumentNullException("res");
                if (!res.Status)
                    return new Result<bool>(false, res.Info);

                if (output == null) throw new ArgumentNullException("output");
                flightToDisplayList.Add(output);
            }

            return new Result<bool>(true);
        }

        private Result<bool> CheckFlightEntriesChanged(Flight newflight, out bool flightEntriesChanged)
        {
            if (newflight == null) throw new ArgumentNullException("flight");
            flightEntriesChanged = true;

            var res = flightRepository.FindFlight(newflight.FlightID, true, out Flight flightInDB)
                ?? throw new ArgumentNullException("res");
            if (!res.Status)
                return new Result<bool>(false, res.Info);

            if (flightInDB == null) throw new ArgumentNullException("flightInDB");

            flightEntriesChanged = !(
                    newflight.DepartureAirportID == flightInDB.DepartureAirportID &&
                    newflight.DestinationAirportID == flightInDB.DestinationAirportID &&
                    newflight.AircraftID == flightInDB.AircraftID
                    );

            return new Result<bool>(true);
        }

        /// <summary>
        /// Get flight info from the DB.
        /// </summary>
        /// <param name="flightInput"> The chosen flight to display </param>
        /// <param name="flight"> Contains the flight retrieved from the Data Base, if retrieving successful, otherwise null </param>
        /// <returns> true if flight successfully retrieved, otherwise false </returns>
        private Result<bool> CalculateFlightInfo(FlightToDisplay flightInput, out Flight flight)
        {
            flight = null;

            var resDepAirport = flightRepository.FindAirport(flightInput.DepartureAirportID, out Airport departureAirport)
                ?? throw new ArgumentNullException("resDepAirport");
            if (!resDepAirport.Status)
                return new Result<bool>(false, resDepAirport.Info);
            if (departureAirport == null) throw new ArgumentNullException("departureAirport");

            var resDestAirport = flightRepository.FindAirport(flightInput.DestinationAirportID, out Airport destinationAirport)
                ?? throw new ArgumentNullException("resDestAirport");
            if (!resDestAirport.Status)
                return new Result<bool>(false, resDestAirport.Info);
            if (destinationAirport == null) throw new ArgumentNullException("destinationAirport");

            var resAircraft = flightRepository.FindAircraft(flightInput.AircraftID, out Aircraft aircraft)
                ?? throw new ArgumentNullException("resAircraft");
            if (!resAircraft.Status)
                return new Result<bool>(false, resAircraft.Info);
            if (aircraft == null) throw new ArgumentNullException("aircraft");

            //Calculate distance and fuel consumption        
            double distance = 0;
            double fuelConsumption = 0;
            try
            {
                distance = flightCalculator.CalculateDistanceBetweenCoordinates(departureAirport.Latitude, departureAirport.Longitude, destinationAirport.Latitude, destinationAirport.Longitude);
                fuelConsumption = flightCalculator.CalculateFlightFuelConsumption(aircraft, distance);
            }
            catch (ArgumentException e)
            {
                //At least one of the supplied arguments is out of range
                return new Result<bool>(false, "Error while calculating flight info: " + e.Message);
            }

            flight = new Flight()
            {
                FlightID = flightInput.FlightID,
                Distance = distance.ToString("0.00"),
                FuelConsumption = fuelConsumption.ToString("0.00"),
                DepartureAirportID = departureAirport.AirportID,
                DestinationAirportID = destinationAirport.AirportID,
                AircraftID = aircraft.AircraftID,
            };

            return new Result<bool>(true);
        }

        public List<Airport> Airports => flightRepository.Airports;

        public List<Aircraft> Aircrafts => flightRepository.Aircrafts;

        public void DisposeOfStorage() => flightRepository.DisposeOfDataBase();

        public Result<AddResultStatus> AddFlight(FlightToDisplay flightInput)
        {
            if (flightInput == null) throw new ArgumentNullException("flightInput");
            if (flightInput.DepartureAirportID == flightInput.DestinationAirportID)
            {
                ///The airports chosen are the same
                return new Result<AddResultStatus>(AddResultStatus.SameAirportsChosen);
            }
            else
            {
                var resCalculation = CalculateFlightInfo(flightInput, out Flight flight)
                        ?? throw new ArgumentNullException("resCalculation");
                if (!resCalculation.Status)
                    return new Result<AddResultStatus>(AddResultStatus.DataError, resCalculation.Info);

                if (flightRepository.CheckFlightEntriesAlreadyExist(flightInput))
                {
                    ///Flight exists in the DB
                    return new Result<AddResultStatus>(AddResultStatus.AlreadyExists);
                }
                else
                {
                    ///Flight does not exist in the DB, add it
                    flightRepository.AddFlight(flight);
                    return new Result<AddResultStatus>(AddResultStatus.Success);
                }
            }
        }

        public Result<EditResultStatus> EditFlight(FlightToDisplay flightInput)
        {
            if (flightInput == null) throw new ArgumentNullException("flightInput");
            if (flightInput.DepartureAirportID == flightInput.DestinationAirportID)
            {
                ///The airports chosen are the same
                return new Result<EditResultStatus>(EditResultStatus.SameAirportsChosen);
            }

            var resCheckEntries = CheckFlightEntriesChanged(flightInput, out bool entriesChanged)
                ?? throw new ArgumentNullException("resCheckEntries");
            if (!resCheckEntries.Status)
                return new Result<EditResultStatus>(EditResultStatus.Failure, resCheckEntries.Info);

            if (!entriesChanged)
            {
                ///Flight entries have not changed
                return new Result<EditResultStatus>(EditResultStatus.EntriesNotChanged);
            }

            if (flightRepository.CheckFlightEntriesAlreadyExist(flightInput))
            {
                ///Flight exists in the DB
                return new Result<EditResultStatus>(EditResultStatus.AlreadyExists);
            }

            var resCalculation = CalculateFlightInfo(flightInput, out Flight flight)
                    ?? throw new ArgumentNullException("resCalculation");
            if (!resCalculation.Status)
                return new Result<EditResultStatus>(EditResultStatus.DataError, resCalculation.Info);

            if (flight == null) throw new ArgumentNullException("flight");
            flightRepository.EditFlight(flight);
            return new Result<EditResultStatus>(EditResultStatus.Success);
        }

        public Result<RemoveResultStatus> RemoveFlight(int id)
        {
            var res = flightRepository.RemoveFlight(id) ?? throw new ArgumentNullException("res")
                ?? throw new ArgumentNullException("res");
            if (res.Status)
                return new Result<RemoveResultStatus>(RemoveResultStatus.Success);
            else
                return new Result<RemoveResultStatus>(RemoveResultStatus.Failure, res.Info);
        }
    }
}