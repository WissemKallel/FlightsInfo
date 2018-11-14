using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightsInfo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightsInfo.Helpers;
using FlightsInfo.Repositories;
using FlightsInfo.Models;

namespace FlightsInfo.Services.Tests
{
    [TestClass()]
    public class FlightServiceTests
    {
        private FlightService sut;
        private int flightsCount = 0;
        private Helpers.Fakes.StubIFlightCalculator fakeFlightCalculator;
        private Repositories.Fakes.StubIFlightRepository fakeFlightRepository;

        [TestInitialize()]
        public void Setup()
        {
            var fakeListFlights = new List<Flight>() { new Flight() { DepartureAirportID = 1, DestinationAirportID = 2 } };
            var fakeListAircrafts = new List<Aircraft>() { new Aircraft() };
            var fakeListAirports = new List<Airport>() { new Airport() };

            fakeFlightCalculator = new Helpers.Fakes.StubIFlightCalculator()
            {
                CalculateDistanceBetweenCoordinatesDoubleDoubleDoubleDouble = (a,b,c,d) => { return 10; },
                CalculateFlightFuelConsumptionAircraftDouble = (a,b) => { return 5; }
            };

            fakeFlightRepository = new Repositories.Fakes.StubIFlightRepository
            {
                AddFlightFlight = (f) => { flightsCount++; return new Result<bool>(true);},
                RemoveFlightInt32 = (f) => { flightsCount--; return new Result<bool>(true);},
                EditFlightFlight = (f) => { return new Result<bool>(true); },

                CheckFlightEntriesAlreadyExistFlightToDisplay = (f) => { return false; },

                FindFlightInt32BooleanFlightOut = (int id, bool track, out Flight f) =>
                {
                    f = new Flight() { DepartureAirportID = 1, DestinationAirportID = 2 };
                    return new Result<bool>(true);
                },
                FindAircraftInt32AircraftOut = (int id, out Aircraft a) =>
                {
                    a = new Aircraft();
                    return new Result<bool>(true);
                },
                FindAirportInt32AirportOut = (int id, out Airport a) =>
                {
                    if (id>0)
                    {
                        a = new Airport();
                        return new Result<bool>(true);
                    }
                    else
                    {
                        a = null;
                        return new Result<bool>(false);
                    }
                },

                FlightsGet = () => { return fakeListFlights; },
                AircraftsGet = () => { return fakeListAircrafts;  },
                AirportsGet = () => { return fakeListAirports; },
            };

            sut = new FlightService(fakeFlightCalculator, fakeFlightRepository);
        }

        #region GetFlightToDisplay Tests
        [TestMethod()]
        //This test expects GetFlightToDisplay to succeed for default fake conditions
        public void GetFlightToDisplay_Should_Succeed()
        {
            //Arrange

            //Act
            var res = sut.GetFlightToDisplay(1, out FlightToDisplay output);

            //Assert
            Assert.AreEqual(true, res.Status);
            Assert.AreNotEqual(null, output);
        }

        [TestMethod()]
        //This test expects GetFlightToDisplay to fail if flight not found
        public void GetFlightToDisplay_Should_Fail_If_Flight_Not_Found()
        {
            //Arrange
            fakeFlightRepository.FindFlightInt32BooleanFlightOut = (int id, bool track, out Flight f) =>
            {
                f = null;
                return new Result<bool>(false);
            };

            //Act
            var res = sut.GetFlightToDisplay(1, out FlightToDisplay output);

            //Assert
            Assert.AreEqual(false, res.Status);
        }

        [TestMethod()]
        //This test expects GetFlightToDisplay to fail if departure airport not found
        public void GetFlightToDisplay_Should_Fail_If_Departure_Airport_Not_Found()
        {
            //Arrange
            fakeFlightRepository.FindFlightInt32BooleanFlightOut = (int id, bool track, out Flight f) =>
            {
                f = new Flight() { DepartureAirportID = -1, DestinationAirportID = 1 } ;
                return new Result<bool>(true);
            };

            //Act
            var res = sut.GetFlightToDisplay(1, out FlightToDisplay output);

            //Assert
            Assert.AreEqual(false, res.Status);
        }

        [TestMethod()]
        //This test expects GetFlightToDisplay to fail if destination airport not found
        public void GetFlightToDisplay_Should_Fail_If_Destination_Airport_Not_Found()
        {
            //Arrange
            fakeFlightRepository.FindFlightInt32BooleanFlightOut = (int id, bool track, out Flight f) =>
            {
                f = new Flight() { DepartureAirportID = 1, DestinationAirportID = -1 };
                return new Result<bool>(true);
            };

            //Act
            var res = sut.GetFlightToDisplay(1, out FlightToDisplay output);

            //Assert
            Assert.AreEqual(false, res.Status);
        }

        [TestMethod()]
        //This test expects GetFlightToDisplay to fail if aircraft not found
        public void GetFlightToDisplay_Should_Fail_If_Aircraft_Not_Found()
        {
            //Arrange
            fakeFlightRepository.FindAircraftInt32AircraftOut = (int id, out Aircraft a) =>
            {
                a = null;
                return new Result<bool>(false);
            };

            //Act
            var res = sut.GetFlightToDisplay(1, out FlightToDisplay output);

            //Assert
            Assert.AreEqual(false, res.Status);
        }
        #endregion

        #region GetFlightsToDisplay Tests
        [TestMethod()]
        //This test expects GetFlightsToDisplay to succeed for default fake conditions
        public void GetFlightsToDisplay_Should_Succeed()
        {
            //Arrange

            //Act
            var res = sut.GetFlightsToDisplay(out List<FlightToDisplay> output);

            //Assert
            Assert.AreEqual(true, res.Status);
            Assert.AreNotEqual(null, output);
        }

        [TestMethod()]
        //This test expects GetFlightsToDisplay to fail if a flight fails to display
        public void GetFlightsToDisplay_Should_Fail_If_Flight_Fails_To_Display()
        {
            //Arrange
            fakeFlightRepository.FindFlightInt32BooleanFlightOut = (int id, bool track, out Flight f) =>
            {
                f = null;
                return new Result<bool>(false);
            };

            //Act
            var res = sut.GetFlightsToDisplay(out List<FlightToDisplay> output);

            //Assert
            Assert.AreEqual(false, res.Status);
        }
        #endregion

        #region RemoveFlight Tests
        [TestMethod()]
        //This test expects RemoveFlight to succeed for default fake conditions
        public void RemoveFlight_Should_Succeed()
        {
            //Arrange

            //Act
            var oldFlightsCount = flightsCount;
            var res = sut.RemoveFlight(1);

            //Assert
            Assert.AreEqual(RemoveResultStatus.Success, res.Status);
            Assert.AreEqual(oldFlightsCount - 1, flightsCount);
        }

        [TestMethod()]
        //This test expects RemoveFlight to fails if actual removing from DB fails
        public void RemoveFlight_Should_Fail_If_Actual_Removing_From_DB_Fails()
        {
            //Arrange
            fakeFlightRepository.RemoveFlightInt32 = (f) => { return new Result<bool>(false); };

            //Act
            var oldFlightsCount = flightsCount;
            var res = sut.RemoveFlight(1);

            //Assert
            Assert.AreEqual(RemoveResultStatus.Failure, res.Status);
            Assert.AreEqual(oldFlightsCount, flightsCount);
        }
        #endregion

        #region AddFlight Tests
        [TestMethod()]
        //This test expects AddFlight to succeed for default fake conditions
        public void AddFlight_Should_Succeed()
        {
            //Arrange
            var input = new FlightToDisplay() { DepartureAirportID = 1, DestinationAirportID = 2 };

            //Act
            var oldFlightsCount = flightsCount;
            var res = sut.AddFlight(input);

            //Assert
            Assert.AreEqual(AddResultStatus.Success, res.Status);
            Assert.AreEqual(oldFlightsCount + 1, flightsCount);
        }

        [TestMethod()]
        //This test expects AddFlight to fail if flight distance calculation fails
        public void AddFlight_Should_Fail_If_Distance_Calculation_Faults()
        {
            //Arrange
            var input = new FlightToDisplay() { DepartureAirportID = 1, DestinationAirportID = 2 };
            fakeFlightCalculator.CalculateDistanceBetweenCoordinatesDoubleDoubleDoubleDouble = (a, b, c, d) => 
            {
                throw new ArgumentException();
            };

            //Act
            var oldFlightsCount = flightsCount;
            var res = sut.AddFlight(input);

            //Assert
            Assert.AreEqual(AddResultStatus.DataError, res.Status);
            Assert.AreEqual(oldFlightsCount, flightsCount);
        }

        [TestMethod()]
        //This test expects AddFlight to fail if flight's fuel consumption calculation fails
        public void AddFlight_Should_Fail_If_Fuel_Calculation_Faults()
        {
            //Arrange
            var input = new FlightToDisplay() { DepartureAirportID = 1, DestinationAirportID = 2 };
            fakeFlightCalculator.CalculateFlightFuelConsumptionAircraftDouble = (a, b) =>
            {
                throw new ArgumentException();
            };

            //Act
            var oldFlightsCount = flightsCount;
            var res = sut.AddFlight(input);

            //Assert
            Assert.AreEqual(AddResultStatus.DataError, res.Status);
            Assert.AreEqual(oldFlightsCount, flightsCount);
        }

        [TestMethod()]
        //This test expects AddFlight to fail if same airports are chosen
        public void AddFlight_Should_Fail_If_Same_Airports_Chosen()
        {
            //Arrange
            var input = new FlightToDisplay() { DepartureAirportID = 1, DestinationAirportID = 1 };

            //Act
            var oldFlightsCount = flightsCount;
            var res = sut.AddFlight(input);

            //Assert
            Assert.AreEqual(AddResultStatus.SameAirportsChosen, res.Status);
            Assert.AreEqual(oldFlightsCount, flightsCount);
        }

        [TestMethod()]
        //This test expects AddFlight to fail if flight already exists
        public void AddFlight_Should_Fail_If_Flight_Already_Exists()
        {
            //Arrange
            var input = new FlightToDisplay() { DepartureAirportID = 1, DestinationAirportID = 2 };
            fakeFlightRepository.CheckFlightEntriesAlreadyExistFlightToDisplay = (f) => { return true; };

            //Act
            var oldFlightsCount = flightsCount;
            var res = sut.AddFlight(input);

            //Assert
            Assert.AreEqual(AddResultStatus.AlreadyExists, res.Status);
            Assert.AreEqual(oldFlightsCount, flightsCount);
        }
        #endregion

        #region EditFlight Tests
        [TestMethod()]
        //This test expects EditFlight to succeed for default fake conditions
        public void EditFlight_Should_Succeed()
        {
            //Arrange
            var input = new FlightToDisplay() { DepartureAirportID = 2, DestinationAirportID = 3 };

            //Act
            var res = sut.EditFlight(input);

            //Assert
            Assert.AreEqual(EditResultStatus.Success, res.Status);
        }

        [TestMethod()]
        //This test expects EditFlight to fail if flight distance calculation fails
        public void EditFlight_Should_Fail_If_Distance_Calculation_Faults()
        {
            //Arrange
            var input = new FlightToDisplay() { DepartureAirportID = 2, DestinationAirportID = 3 };
            fakeFlightCalculator.CalculateDistanceBetweenCoordinatesDoubleDoubleDoubleDouble = (a, b, c, d) =>
            {
                throw new ArgumentException();
            };

            //Act
            var oldFlightsCount = flightsCount;
            var res = sut.EditFlight(input);

            //Assert
            Assert.AreEqual(EditResultStatus.DataError, res.Status);
            Assert.AreEqual(oldFlightsCount, flightsCount);
        }

        [TestMethod()]
        //This test expects EditFlight to fail if flight's fuel consumption calculation fails
        public void EditFlight_Should_Fail_If_Fuel_Calculation_Faults()
        {
            //Arrange
            var input = new FlightToDisplay() { DepartureAirportID = 2, DestinationAirportID = 3 };
            fakeFlightCalculator.CalculateFlightFuelConsumptionAircraftDouble = (a, b) =>
            {
                throw new ArgumentException();
            };

            //Act
            var oldFlightsCount = flightsCount;
            var res = sut.EditFlight(input);

            //Assert
            Assert.AreEqual(EditResultStatus.DataError, res.Status);
            Assert.AreEqual(oldFlightsCount, flightsCount);
        }

        [TestMethod()]
        //This test expects EditFlight to fail if same airports are chosen
        public void EditFlight_Should_Fail_If_Same_Airports_Chosen()
        {
            //Arrange
            var input = new FlightToDisplay() { DepartureAirportID = 2, DestinationAirportID = 2 };

            //Act
            var oldFlightsCount = flightsCount;
            var res = sut.EditFlight(input);

            //Assert
            Assert.AreEqual(EditResultStatus.SameAirportsChosen, res.Status);
            Assert.AreEqual(oldFlightsCount, flightsCount);
        }

        [TestMethod()]
        //This test expects EditFlight to fail if flight already exists
        public void EditFlight_Should_Fail_If_Flight_Already_Exists()
        {
            //Arrange
            var input = new FlightToDisplay() { DepartureAirportID = 2, DestinationAirportID = 3 };
            fakeFlightRepository.CheckFlightEntriesAlreadyExistFlightToDisplay = (f) => { return true; };

            //Act
            var oldFlightsCount = flightsCount;
            var res = sut.EditFlight(input);

            //Assert
            Assert.AreEqual(EditResultStatus.AlreadyExists, res.Status);
            Assert.AreEqual(oldFlightsCount, flightsCount);
        }

        [TestMethod()]
        //This test expects EditFlight to fail if the flight entries have not changed
        public void EditFlight_Should_Fail_If_Entries_Not_Changed()
        {
            //Arrange
            var input = new FlightToDisplay() { DepartureAirportID = 1, DestinationAirportID = 2 };

            //Act
            var oldFlightsCount = flightsCount;
            var res = sut.EditFlight(input);

            //Assert
            Assert.AreEqual(EditResultStatus.EntriesNotChanged, res.Status);
            Assert.AreEqual(oldFlightsCount, flightsCount);
        }

        [TestMethod()]
        //This test expects EditFlight to fail if finding the flight to check whether entries have changed fails
        public void EditFlight_Should_Fail_If_Find_Flight_To_Check_Entries_Fails()
        {
            //Arrange
            var input = new FlightToDisplay() { DepartureAirportID = 1, DestinationAirportID = 2 };
            fakeFlightRepository.FindFlightInt32BooleanFlightOut = (int id, bool track, out Flight f) =>
            {
                f = null;
                return new Result<bool>(false);
            };

            //Act
            var oldFlightsCount = flightsCount;
            var res = sut.EditFlight(input);

            //Assert
            Assert.AreEqual(EditResultStatus.Failure, res.Status);
            Assert.AreEqual(oldFlightsCount, flightsCount);
        }
        #endregion
    }
}