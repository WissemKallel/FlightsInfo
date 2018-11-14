using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightsInfo.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightsInfo.Helpers;
using FlightsInfo.Services;
using FlightsInfo.Models;
using System.Web.Mvc;
using System.Net;

namespace FlightsInfo.Controllers.Tests
{
    [TestClass()]
    public class FlightsControllerTests
    {
        private FlightsController sut;
        private Services.Fakes.StubIFlightService fakeService;

        [TestInitialize()]
        public void Setup()
        {
            var flight = new FlightToDisplay() { DepartureAirportID = 1, DestinationAirportID = 2, AircraftID = 1 };
            var listFlights = new List<FlightToDisplay>() { flight };
            fakeService = new Services.Fakes.StubIFlightService()
            {
                DisposeOfStorage = () => { },
                AddFlightFlightToDisplay = (f) => { return new Result<AddResultStatus>(AddResultStatus.Success); },
                RemoveFlightInt32 = (f) => { return new Result<RemoveResultStatus>(RemoveResultStatus.Success); },
                EditFlightFlightToDisplay = (f) => { return new Result<EditResultStatus>(EditResultStatus.Success); },
                GetFlightsToDisplayListOfFlightToDisplayOut = (out List<FlightToDisplay> f) => 
                {
                    f = listFlights;
                    return new Result<bool>(true);
                },
                GetFlightToDisplayInt32FlightToDisplayOut = (int id, out FlightToDisplay f) =>
                {
                    f = flight;
                    return new Result<bool>(true);
                },
            };

            sut = new FlightsController(fakeService);
        }

        #region Index Tests
        [TestMethod()]
        /// This test expects Index to show the list of flights to display
        public void Index_Should_Show_List_Of_Flights()
        {
            //Arrange

            //Act
            var result = sut.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var flights = result.Model as List<FlightToDisplay>;
            Assert.IsNotNull(flights);
            Assert.AreEqual(1, flights.Count);
        }

        [TestMethod()]
        /// This test expects Index to show an error page if the list of flights is not found
        public void Index_Should_Show_Error_If_Unable_Get_Flights()
        {
            //Arrange
            fakeService.GetFlightsToDisplayListOfFlightToDisplayOut = (out List<FlightToDisplay> f) =>
            {
                f = null;
                return new Result<bool>(false);
            };

            //Act
            var result = sut.Index() as HttpNotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }
        #endregion

        #region Report Tests
        [TestMethod()]
        /// This test expects Report to show the list of flights to display
        public void Report_Should_Show_FlightsToDisplay()
        {
            //Arrange

            //Act
            var result = sut.Report() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var flights = result.Model as List<FlightToDisplay>;
            Assert.IsNotNull(flights);
            Assert.AreEqual(1, flights.Count);
        }

        [TestMethod()]
        /// This test expects Report to show an error page if the list of flights is not found
        public void Report_Should_Show_Error_If_Unable_Get_Flights()
        {
            //Arrange
            fakeService.GetFlightsToDisplayListOfFlightToDisplayOut = (out List<FlightToDisplay> f) =>
            {
                f = null;
                return new Result<bool>(false);
            };

            //Act
            var result = sut.Report() as HttpNotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }
        #endregion

        #region Details Tests
        [TestMethod()]
        /// This test expects Details to show the flight
        public void Details_Should_Show_Flight()
        {
            //Arrange

            //Act
            var result = sut.Details(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var flight = result.Model as FlightToDisplay;
            Assert.IsNotNull(flight);
        }

        [TestMethod()]
        /// This method expects Details to show an error page if the flight is not found
        public void Details_Should_show_Error_If_Unable_Get_Flight()
        {
            //Arrange
            fakeService.GetFlightToDisplayInt32FlightToDisplayOut = (int id, out FlightToDisplay f) =>
            {
                f = null;
                return new Result<bool>(false);
            };

            //Act
            var result = sut.Details(1) as HttpNotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        /// This method expects Details to show an error page if Id is null
        public void Details_Should_Show_Error_If_Null_Id()
        {
            //Arrange

            //Act
            var result = sut.Details(null) as HttpStatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
        }
        #endregion

        #region Create Tests
        [TestMethod()]
        /// This test expects Create to show View
        public void Create_Should_Show_View()
        {
            //Arrange

            //Act
            var result = sut.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        /// This test expects Create to redirect to view if successful
        public void Create_Should_Redirect_To_Index_If_Successful()
        {
            //Arrange
            var flight = new FlightToDisplay();

            //Act
            var result = sut.Create(flight) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod()]
        /// This test expects Create show error page if failure
        public void Create_Should_Show_Eror_Page_If_Failure()
        {
            //Arrange
            var flight = new FlightToDisplay();
            fakeService.AddFlightFlightToDisplay = (f) => { return new Result<AddResultStatus>(AddResultStatus.Failure); };

            //Act
            var result = sut.Create(flight) as HttpStatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        /// This test expects Create to show model error if same airports chosen
        public void Create_Should_Show_Model_Error_If_Same_Airports_Chosen()
        {
            //Arrange
            var flight = new FlightToDisplay();
            fakeService.AddFlightFlightToDisplay = (f) => { return new Result<AddResultStatus>(AddResultStatus.SameAirportsChosen); };

            //Act
            var result = sut.Create(flight) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model as FlightToDisplay);
            Assert.IsNotNull(result.ViewData.ModelState[""]);
            Assert.IsNotNull(result.ViewData.ModelState[""].Errors);
            Assert.IsTrue(result.ViewData.ModelState[""].Errors.Count == 1);
        }

        [TestMethod()]
        /// This test expects Create to show model error if flight already exists
        public void Create_Should_Show_Model_Error_If_Flight_Already_Exists()
        {
            //Arrange
            var flight = new FlightToDisplay();
            fakeService.AddFlightFlightToDisplay = (f) => { return new Result<AddResultStatus>(AddResultStatus.AlreadyExists); };

            //Act
            var result = sut.Create(flight) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model as FlightToDisplay);
            Assert.IsNotNull(result.ViewData.ModelState[""]);
            Assert.IsNotNull(result.ViewData.ModelState[""].Errors);
            Assert.IsTrue(result.ViewData.ModelState[""].Errors.Count == 1);
        }
        #endregion

        #region Delete Tests
        [TestMethod()]
        /// This test expects Delete to show the flight to delete
        public void Delete_Should_Show_Flight()
        {
            //Arrange

            //Act
            var result = sut.Delete(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var flight = result.Model as FlightToDisplay;
            Assert.IsNotNull(flight);
        }

        [TestMethod()]
        /// This method expects Delete to show an error page if the flight is not found
        public void Delete_Should_Show_Error_If_Unable_Get_Flight()
        {
            //Arrange
            fakeService.GetFlightToDisplayInt32FlightToDisplayOut = (int id, out FlightToDisplay f) =>
            {
                f = null;
                return new Result<bool>(false);
            };

            //Act
            var result = sut.Delete(1) as HttpNotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        /// This method expects Delete to show an error page if Id is null
        public void Delete_Should_Show_Error_If_Null_Id()
        {
            //Arrange

            //Act
            var result = sut.Delete(null) as HttpStatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        /// This method expects DeleteConfirmed to redirect to index if successful
        public void DeleteConfirmed_Should_Redirect_To_Index_If_Successful()
        {
            //Arrange

            //Act
            var result = sut.DeleteConfirmed(1) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
         } 

        [TestMethod()]
        /// This method expects DeleteConfirmed to redirect to index if successful
        public void DeleteConfirmed_Should_Display_Error_If_Failed()
        {
            //Arrange
            fakeService.RemoveFlightInt32 = (f) => { return new Result<RemoveResultStatus>(RemoveResultStatus.Failure); };

            //Act
            var result = sut.DeleteConfirmed(1) as HttpNotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }
        #endregion

        #region Edit Tests
        [TestMethod()]
        /// This test expects Delete to show the flight to delete
        public void Edit_Should_Show_Flight()
        {
            //Arrange

            //Act
            var result = sut.Edit(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var flight = result.Model as FlightToDisplay;
            Assert.IsNotNull(flight);
        }

        [TestMethod()]
        /// This method expects Delete to show an error page if the flight is not found
        public void Edit_Should_Show_Error_If_Unable_Get_Flight()
        {
            //Arrange
            fakeService.GetFlightToDisplayInt32FlightToDisplayOut = (int id, out FlightToDisplay f) =>
            {
                f = null;
                return new Result<bool>(false);
            };

            //Act
            var result = sut.Edit(1) as HttpNotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        /// This method expects Edit to show an error page if Id is null
        public void Edit_Should_Show_Error_If_Null_Id()
        {
            //Arrange
            int? id = null;

            //Act
            var result = sut.Edit(id) as HttpStatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        /// This test expects Edit to redirect to view if successful
        public void Edit_Should_Redirect_To_Index_If_Successful()
        {
            //Arrange
            var flight = new FlightToDisplay();

            //Act
            var result = sut.Edit(flight) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod()]
        /// This test expects Edit to redirect to view if successful
        public void Edit_Should_Redirect_To_Index_If_Entries_Not_Changed()
        {
            //Arrange
            var flight = new FlightToDisplay();
            fakeService.EditFlightFlightToDisplay = (f) => { return new Result<EditResultStatus>(EditResultStatus.EntriesNotChanged); };

                //Act
                var result = sut.Edit(flight) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod()]
        /// This test expects Edit show error page if failure
        public void Edit_Should_Show_Eror_Page_If_Failure()
        {
            //Arrange
            var flight = new FlightToDisplay();
            fakeService.EditFlightFlightToDisplay = (f) => { return new Result<EditResultStatus>(EditResultStatus.Failure); };

            //Act
            var result = sut.Edit(flight) as HttpStatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        /// This test expects Edit to show model error if same airports chosen
        public void Edit_Should_Show_Model_Error_If_Same_Airports_Chosen()
        {
            //Arrange
            var flight = new FlightToDisplay();
            fakeService.EditFlightFlightToDisplay = (f) => { return new Result<EditResultStatus>(EditResultStatus.SameAirportsChosen); };

            //Act
            var result = sut.Edit(flight) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model as FlightToDisplay);
            Assert.IsNotNull(result.ViewData.ModelState[""]);
            Assert.IsNotNull(result.ViewData.ModelState[""].Errors);
            Assert.IsTrue(result.ViewData.ModelState[""].Errors.Count == 1);
        }

        [TestMethod()]
        /// This test expects Edit to show model error if flight already exists
        public void Edit_Should_Show_Model_Error_If_Flight_Already_Exists()
        {
            //Arrange
            var flight = new FlightToDisplay();
            fakeService.EditFlightFlightToDisplay = (f) => { return new Result<EditResultStatus>(EditResultStatus.AlreadyExists); };

            //Act
            var result = sut.Edit(flight) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model as FlightToDisplay);
            Assert.IsNotNull(result.ViewData.ModelState[""]);
            Assert.IsNotNull(result.ViewData.ModelState[""].Errors);
            Assert.IsTrue(result.ViewData.ModelState[""].Errors.Count == 1);
        }
        #endregion
    }
}