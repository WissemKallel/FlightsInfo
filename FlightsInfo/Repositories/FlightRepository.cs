using FlightsInfo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using FlightsInfo.Helpers;

namespace FlightsInfo.Repositories
{
    public class FlighRepository : IFlightRepository
    {
        private readonly IFlightDBContext db;

        public FlighRepository(IFlightDBContext db)
        {
            this.db = db ?? throw new ArgumentNullException("db");
        }

        public Result<bool> AddFlight(Flight flight)
        {
            if (flight == null) throw new ArgumentNullException("flight");

            //Add the flight
            db.Flights.Add(flight);
            try
            {
                db.SaveChanges();
            }
            catch(Exception e)
            {
                return new Result<bool>(false, "Caught exception when saving changes to the data base while adding flight: " + e.Message);
            }

            return new Result<bool>(true);
        }

        public Result<bool> RemoveFlight(int id)
        {
            if (id < 0) throw new ArgumentException("id is negative");

            //Find the flight
            var res = FindFlight(id, false, out Flight flight);
            if (res == null) throw new ArgumentNullException("res");

            if (!res.Status)
                return new Result<bool>(false, "Error when removing flight from data base: " + res.Info);
            if (flight == null) throw new ArgumentNullException("flight");

            //Remove the flight
            db.Flights.Remove(flight);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return new Result<bool>(false, "Caught exception when saving changes to the data base while removing flight: " + e.Message);
            }

            return new Result<bool>(true);
        }

        public Result<bool> EditFlight(Flight flight)
        {
            if (flight == null) throw new ArgumentNullException("flight");

            //Update the flight state
            try
            {
                db.Entry(flight).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return new Result<bool>(false, "Caught exception when saving changes to the data base while editing flight: " + e.Message);
            }

            return new Result<bool>(true);
        }

        public List<Flight> Flights => db.Flights.ToList();

        public List<Airport> Airports => db.Airports.ToList();

        public List<Aircraft> Aircrafts => db.Aircrafts.ToList();

        public bool CheckFlightEntriesAlreadyExist(FlightToDisplay flight)
        {
            if (flight == null) throw new ArgumentNullException("flight");

            return db.Flights.Any(f => (
                                        f.AircraftID == flight.AircraftID &&
                                        f.DepartureAirportID == flight.DepartureAirportID &&
                                        f.DestinationAirportID == flight.DestinationAirportID)
                                        );
        }
        public Result<bool> FindFlight(int id, bool noTracking, out Flight flight)
        {
            if (id < 0) throw new ArgumentException("id is negative");

            if (noTracking)
            {
                flight = db.Flights.AsNoTracking().FirstOrDefault(x => x.FlightID == id);
                return new Result<bool>(flight !=null);
            }
            else
            {
                try
                {
                    flight = db.Flights.Find(id);
                }
                catch (InvalidOperationException e)
                {
                    flight = null;
                    return new Result<bool>(false, "Caught exception when trying to find Flight of ID = " + id + " : "+ e.Message);
                }

                return new Result<bool>(true);
            }
        }

        public Result<bool> FindAirport(int id, out Airport airport)
        {
            try
            {
                airport = db.Airports.Find(id);
            }
            catch (InvalidOperationException e)
            {
                airport = null;
                return new Result<bool>(false, "Caught exception when trying to find Airport of ID = " + id + " : " + e.Message);
            }

            return new Result<bool>(true);
        }

        public Result<bool> FindAircraft(int id, out Aircraft aircraft)
        {
            try
            {
                aircraft = db.Aircrafts.Find(id);
            }
            catch (InvalidOperationException e)
            {
                aircraft = null;
                return new Result<bool>(false, "Caught exception when trying to find Aircraft of ID = " + id + " : " + e.Message);
            }

            return new Result<bool>(true);
        }

        public void DisposeOfDataBase()
        {
            db.Dispose();
        }
    }
}