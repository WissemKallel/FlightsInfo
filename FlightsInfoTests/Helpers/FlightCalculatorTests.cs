using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightsInfo.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightsInfo.Models;

namespace FlightsInfo.Helpers.Tests
{
    [TestClass()]
    public class FlightCalculatorTests
    {
        private readonly FlightCalculator sut = new FlightCalculator();

        #region CalculateDistanceBetweenCoordinates tests
        [TestMethod()]
        ///This test expects the distance to return zero for the same coordinates
        public void CalculateDistance_Should_Return_Zero_For_Same_Coordinates()
        {
            //Arrange
            double lat1 = 20;
            double lon1 = -15;
            double lat2 = lat1;
            double lon2 = lon1;

            //Act
            var dist = sut.CalculateDistanceBetweenCoordinates(lat1, lon1, lat2, lon2);

            //Assert
            Assert.AreEqual(0, dist);
        }

        [TestMethod()]
        ///This test expects the distances to be equal if the coordinates are interchanged
        public void CalculateDistance_Should_Return_Same_Values_If_Coordinates_Interchanged()
        {
            //Arrange
            double lat1 = 20;
            double lon1 = 50;
            double lat2 = -8;
            double lon2 = 3;

            //Act
            var dist1 = sut.CalculateDistanceBetweenCoordinates(lat1, lon1, lat2, lon2);
            var dist2 = sut.CalculateDistanceBetweenCoordinates(lat2, lon2, lat1, lon1);

            //Assert
            Assert.AreEqual(dist1, dist2);
        }

        [TestMethod()]
        ///This test expects the distances to be equal if we use symmetrical coordinates 
        public void CalculateDistance_Should_Return_Same_Values_If_Coordinates_Symmetrical()
        {
            //Arrange
            double lat1 = 0;
            double lon1 = 0;
            double lat2 = 25;
            double lon2 = -54;

            //Act
            var dist1 = sut.CalculateDistanceBetweenCoordinates(lat1, lon1, lat2, lon2);
            var dist2 = sut.CalculateDistanceBetweenCoordinates(lat1, lon1, -lat2, -lon2);

            //Assert
            Assert.AreEqual(dist1, dist2);
        }

        [TestMethod()]
        ///This test expects the returned distance to be strictly positive 
        public void CalculateDistance_Should_Return_Stricly_Positive_Value()
        {
            //Arrange
            double lat1 = 45;
            double lon1 = -87;
            double lat2 = -25;
            double lon2 = 54;

            //Act
            var dist1 = sut.CalculateDistanceBetweenCoordinates(lat1, lon1, lat2, lon2);

            //Assert
            Assert.IsTrue(dist1 > 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        //This test expects an ArgumentException to be thrown if Latitude 1 is beyond the lower limit
        public void CalculateDistance_Should_Fault_If_Latitude1_Beyond_LowerLimit()
        {
            //Arrange
            double lat1 = -91;
            double lon1 = 5;
            double lat2 = 8;
            double lon2 = 3;

            //Act
            var dist1 = sut.CalculateDistanceBetweenCoordinates(lat1, lon1, lat2, lon2);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        //This test expects an ArgumentException to be thrown if Latitude 1 is beyond the upper limit
        public void CalculateDistance_Should_Fault_If_Latitude1_Beyond_UpperLimit()
        {
            //Arrange
            double lat1 = 91;
            double lon1 = 5;
            double lat2 = 8;
            double lon2 = 3;

            //Act
            var dist1 = sut.CalculateDistanceBetweenCoordinates(lat1, lon1, lat2, lon2);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        //This test expects an ArgumentException to be thrown if Longitude 1 is beyond the lower limit
        public void CalculateDistance_Should_Fault_If_Longitude1_Beyond_LowerLimit()
        {
            //Arrange
            double lat1 = 5;
            double lon1 = -181;
            double lat2 = 8;
            double lon2 = 3;

            //Act
            var dist1 = sut.CalculateDistanceBetweenCoordinates(lat1, lon1, lat2, lon2);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        //This test expects an ArgumentException to be thrown if Longitude 1 is beyond the upper limit
        public void CalculateDistance_Should_Fault_If_Longitude1_Beyond_UpperLimit()
        {
            //Arrange
            double lat1 = 5;
            double lon1 = 181;
            double lat2 = 8;
            double lon2 = 3;

            //Act
            var dist1 = sut.CalculateDistanceBetweenCoordinates(lat1, lon1, lat2, lon2);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        //This test expects an ArgumentException to be thrown if Latitude 2 is beyond the lower limit
        public void CalculateDistance_Should_Fault_If_Latitude2_Beyond_LowerLimit()
        {
            //Arrange
            double lat1 = 5;
            double lon1 = 5;
            double lat2 = -91;
            double lon2 = 3;

            //Act
            var dist1 = sut.CalculateDistanceBetweenCoordinates(lat1, lon1, lat2, lon2);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        //This test expects an ArgumentException to be thrown if Latitude 2 is beyond the upper limit
        public void CalculateDistance_Should_Fault_If_Latitude2_Beyond_UpperLimit()
        {
            //Arrange
            double lat1 = 5;
            double lon1 = 40;
            double lat2 = 91;
            double lon2 = 3;

            //Act
            var dist1 = sut.CalculateDistanceBetweenCoordinates(lat1, lon1, lat2, lon2);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        //This test expects an ArgumentException to be thrown if Longitude 2 is beyond the lower limit
        public void CalculateDistance_Should_Fault_If_Longitude2_Beyond_LowerLimit()
        {
            //Arrange
            double lat1 = 5;
            double lon1 = 5;
            double lat2 = 50;
            double lon2 = -181;

            //Act
            var dist1 = sut.CalculateDistanceBetweenCoordinates(lat1, lon1, lat2, lon2);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        //This test expects an ArgumentException to be thrown if Longitude 2 is beyond the upper limit
        public void CalculateDistance_Should_Fault_If_Longitude2_Beyond_UpperLimit()
        {
            //Arrange
            double lat1 = 5;
            double lon1 = 40;
            double lat2 = 54;
            double lon2 = 181;

            //Act
            var dist1 = sut.CalculateDistanceBetweenCoordinates(lat1, lon1, lat2, lon2);

            //Assert
        }
        #endregion

        #region CalculateFlightFuelConsumption Tests
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        ///This test expects CalculateFlightFuelConsumption to throw an ArgumentNullException if aircraft is null
        public void CalculateConsumption_Should_Fault_If_Aircraft_Null()
        {
            //Arrange

            //Act
            sut.CalculateFlightFuelConsumption(null, 25);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        ///This test expects CalculateFlightFuelConsumption to throw an ArgumentException if the distance is negative
        public void CalculateConsumption_Should_Fault_If_Negative_Distance()
        {
            //Arrange
            var aircraft = new Aircraft()
            {
                AverageConsumptionPerKmInLitre = 1,
                AverageTakeoffEffortInLitre = 2
            };

            //Act
            sut.CalculateFlightFuelConsumption(aircraft, 0);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        ///This test expects CalculateFlightFuelConsumption to throw an ArgumentException if the aircraft's average consumption is negative
        public void CalculateConsumption_Should_Fault_If_Negative_Aircraft_Consumption()
        {
            //Arrange
            var aircraft = new Aircraft()
            {
                AverageConsumptionPerKmInLitre = 0,
                AverageTakeoffEffortInLitre = 1
            };


            //Act
            sut.CalculateFlightFuelConsumption(aircraft, 5);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        ///This test expects CalculateFlightFuelConsumption to throw an ArgumentException if the aircraft's take-off effor is negative
        public void CalculateConsumption_Should_Fault_If_Negative_TakeOffEffort()
        {
            //Arrange
            var aircraft = new Aircraft()
            {
                AverageConsumptionPerKmInLitre = 1,
                AverageTakeoffEffortInLitre = 0
            };

            //Act
            sut.CalculateFlightFuelConsumption(aircraft, 5);

            //Assert
        }

        [TestMethod()]
        ///This test expects CalculateFlightFuelConsumption to return a strictly positive value
        public void CalculateConsumption_Should_Return_Strictly_Positive_Value()
        {
            //Arrange
            var aircraft = new Aircraft()
            {
                AverageConsumptionPerKmInLitre = 1,
                AverageTakeoffEffortInLitre = 2
            };

            //Act
            var consumption = sut.CalculateFlightFuelConsumption(aircraft, 5);

            //Assert
            Assert.IsTrue(consumption > 0);
        }
        #endregion
    }
}