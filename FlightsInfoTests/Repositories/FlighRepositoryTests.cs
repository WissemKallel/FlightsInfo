using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightsInfo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.QualityTools.Testing.Fakes;
using FlightsInfo.Models;
using System.Data.Entity;
using System.Collections.ObjectModel;

namespace FlightsInfo.Repositories.Tests
{
    [TestClass()]
    public class FlighRepositoryTests
    {
        private FlighRepository sut;
        private Helpers.Fakes.StubIFlightDBContext fakeDBContext;

        public class FakeDbSet<T> : IDbSet<T> where T : class
        {
            ObservableCollection<T> _data;
            IQueryable _query;

            public FakeDbSet()
            {
                _data = new ObservableCollection<T>();
                _query = _data.AsQueryable();
            }

            public virtual T Find(params object[] keyValues)
            {
                return _data.First();
            }

            public T Add(T item)
            {
                _data.Add(item);
                return item;
            }

            public T Remove(T item)
            {
                _data.Remove(item);
                return item;
            }

            public T Attach(T item)
            {
                _data.Add(item);
                return item;
            }

            public T Detach(T item)
            {
                _data.Remove(item);
                return item;
            }

            public T Create()
            {
                return Activator.CreateInstance<T>();
            }

            public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
            {
                return Activator.CreateInstance<TDerivedEntity>();
            }

            public ObservableCollection<T> Local
            {
                get { return _data; }
            }
            public int Count()
            {
                return _data.Count;
            }

            Type IQueryable.ElementType
            {
                get { return _query.ElementType; }
            }

            System.Linq.Expressions.Expression IQueryable.Expression
            {
                get { return _query.Expression; }
            }

            IQueryProvider IQueryable.Provider
            {
                get { return _query.Provider; }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _data.GetEnumerator();
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return _data.GetEnumerator();
            }
        }

        [TestInitialize()]
        public void Setup()
        {
            var fakeAirports = new FakeDbSet<Airport>();
            var fakeAircrafts = new FakeDbSet<Aircraft>();
            var fakeFlights = new FakeDbSet<Flight>();

            fakeDBContext = new Helpers.Fakes.StubIFlightDBContext()
            {
                Dispose = () => { },          
                EntryObject = (x) => { return new System.Data.Entity.Infrastructure.Fakes.ShimDbEntityEntry(); },
                SaveChanges = () => { return 1; },
                FlightsGet = () => { return fakeFlights; },
                AirportsGet = () => { return fakeAirports; },
                AircraftsGet = () => { return fakeAircrafts; },
            };

            sut = new FlighRepository(fakeDBContext);
        }

        #region AddFlight tests
        [TestMethod()]
        ///This test expects AddFlight to Succeed for default fake conditions
        public void AddFlight_Should_Succeed()
        {
            //Arrange

            //Act
            var res = sut.AddFlight(new Flight());

            //Assert
            Assert.AreEqual(true, res.Status);
            Assert.AreEqual(1, sut.Flights.Count);
        }

        [TestMethod()]
        ///This test expects AddFlight to Fail if SaveChanges faults
        public void AddFlight_Should_Fail_If_SaveChanges_Faults()
        {
            //Arrange
            fakeDBContext.SaveChanges = () => { throw new InvalidOperationException(); };

            //Act
            var res = sut.AddFlight(new Flight());

            //Assert
            Assert.AreEqual(false, res.Status);
        }
        #endregion

        #region RemoveFlight Tests
        [TestMethod()]
        ///This test expects RemoveFlight to Succeed for default fake conditions
        public void RemoveFlight_Should_Succeed()
        {
            //Arrange

            //Act
            sut.AddFlight(new Flight());
            var res = sut.RemoveFlight(1);

            //Assert
            Assert.AreEqual(true, res.Status);
            Assert.AreEqual(0, sut.Flights.Count);
        }

        [TestMethod()]
        ///This test expects RemoveFlight to Fail if SaveChanges faults
        public void RemoveFlight_Should_Fail_If_SaveChanges_Faults()
        {
            //Arrange
            fakeDBContext.SaveChanges = () => { throw new InvalidOperationException(); };

            //Act
            sut.AddFlight(new Flight());
            var res = sut.RemoveFlight(1);

            //Assert
            Assert.AreEqual(false, res.Status);
        }

        [TestMethod()]
        ///This test expects RemoveFlight to Fail if flight not found
        public void RemoveFlight_Should_Fail_If_Flight_Not_Found()
        {
            //Arrange

            //Act
            var res = sut.RemoveFlight(1);

            //Assert
            Assert.AreEqual(false, res.Status);
        }
        #endregion

        #region EditFlight tests
        [TestMethod()]
        ///This test expects EditFlight to Succeed for default fake conditions
        public void EditFlight_Should_Succeed()
        {
            using (ShimsContext.Create())
            {
                //Arrange
                var flight = new Flight();
                System.Data.Entity.Infrastructure.Fakes.ShimDbEntityEntry.AllInstances.StateGet = (@this) => { return EntityState.Unchanged; };
                System.Data.Entity.Infrastructure.Fakes.ShimDbEntityEntry.AllInstances.StateSetEntityState = (@this,x) => { };

                //Act
                sut.AddFlight(flight);
                var res = sut.EditFlight(flight);

                //Assert
                Assert.AreEqual(true, res.Status);
                Assert.AreEqual(1, sut.Flights.Count);
            }
        }

        [TestMethod()]
        ///This test expects EditFlight to Fail if SaveChanges faults
        public void EditFlight_Should_Fail_If_SaveChanges_Faults()
        {
            using (ShimsContext.Create())
            {
                //Arrange
                fakeDBContext.SaveChanges = () => { throw new InvalidOperationException(); };
                System.Data.Entity.Infrastructure.Fakes.ShimDbEntityEntry.AllInstances.StateGet = (@this) => { return EntityState.Unchanged; };
                System.Data.Entity.Infrastructure.Fakes.ShimDbEntityEntry.AllInstances.StateSetEntityState = (@this, x) => { };

                //Act
                var res = sut.EditFlight(new Flight());

                //Assert
                Assert.AreEqual(false, res.Status);
            }
        }

        [TestMethod()]
        ///This test expects EditFlight to Fail if Entry faults
        public void EditFlight_Should_Fail_If_Entry_Faults()
        {
            using (ShimsContext.Create())
            {
                //Arrange
                fakeDBContext.SaveChanges = () => { throw new InvalidOperationException(); };
                System.Data.Entity.Infrastructure.Fakes.ShimDbEntityEntry.AllInstances.StateGet = (@this) => { throw new InvalidOperationException(); };
                System.Data.Entity.Infrastructure.Fakes.ShimDbEntityEntry.AllInstances.StateSetEntityState = (@this, x) => { };

                //Act
                var res = sut.EditFlight(new Flight());

                //Assert
                Assert.AreEqual(false, res.Status);
            }
        }
        #endregion

        #region CheckFlightEntriesAlreadyExist Tests
        [TestMethod()]
        ///This test expects CheckFlightEntriesAlreadyExist to return false for new entries
        public void CheckFlightEntriesAlreadyExist_Should_Return_False_If_New_Entries()
        {
            //Arrange

            //Act
            sut.AddFlight(new Flight() { DepartureAirportID = 1, DestinationAirportID =2, AircraftID =1 });
            var res = sut.CheckFlightEntriesAlreadyExist(new FlightToDisplay() { DepartureAirportID = 1, DestinationAirportID = 2, AircraftID = 10 });

            //Assert
            Assert.AreEqual(false, res);
        }

        [TestMethod()]
        ///This test expects CheckFlightEntriesAlreadyExist to return true for same entries
        public void CheckFlightEntriesAlreadyExist_Should_Return_True_If_Same_Entries()
        {
            //Arrange

            //Act
            sut.AddFlight(new Flight() { DepartureAirportID = 1, DestinationAirportID = 2, AircraftID = 1 });
            var res = sut.CheckFlightEntriesAlreadyExist(new FlightToDisplay() { DepartureAirportID = 1, DestinationAirportID = 2, AircraftID = 1 });

            //Assert
            Assert.AreEqual(true, res);
        }
        #endregion

        #region FindFlight Tests
        [TestMethod()]
        ///This test expects FindFlight to succeed for default fakes behaviour
        public void FindFlight_Should_Succeed()
        {
            //Arrange

            //Act
            sut.AddFlight(new Flight());
            var res = sut.FindFlight(1, false, out Flight flight);

            //Assert
            Assert.AreEqual(true, res.Status);
            Assert.AreNotEqual(null, flight);
        }

        [TestMethod()]
        ///This test expects FindFlight without tracking to succeed for default fakes behaviour
        public void FindFlight_No_Tracking_Should_Succeed()
        {
            //Arrange

            //Act
            sut.AddFlight(new Flight() { FlightID = 1 });
            var res = sut.FindFlight(1, true, out Flight flight);

            //Assert
            Assert.AreEqual(true, res.Status);
            Assert.AreNotEqual(null, flight);
        }

        [TestMethod()]
        ///This test expects FindFlight without tracking to fail if the supplied flight ID is not found
        public void FindFlight_No_Tracking_Should_Fail_Not_Found_Flight_ID()
        {
            //Arrange

            //Act
            sut.AddFlight(new Flight() { FlightID = -1 });
            var res = sut.FindFlight(1, true, out Flight flight);

            //Assert
            Assert.AreEqual(false, res.Status);
        }

        [TestMethod()]
        ///This test expects FindFlight to fail if the supplied flight ID is not found
        public void FindFlight_Should_Fail_Not_Found_Flight_ID()
        {
            //Arrange

            //Act
            var res = sut.FindFlight(1, false, out Flight flight);

            //Assert
            Assert.AreEqual(false, res.Status);
        }
        #endregion

        #region FindAirport Tests      
        [TestMethod()]
        ///This test expects FindAirport to fail if the supplied flight ID is not found
        public void FindAirport_Should_Fail_Not_Found_Airport_ID()
        {
            //Arrange

            //Act
            var res = sut.FindAirport(1, out Airport airport);

            //Assert
            Assert.AreEqual(false, res.Status);
        }
        #endregion

        #region FindAircraft Tests      
        [TestMethod()]
        ///This test expects FindAircraft to fail if the supplied flight ID is not found
        public void FindAircraft_Should_Fail_Not_Found_Aircraft_ID()
        {
            //Arrange

            //Act
            var res = sut.FindAircraft(1, out Aircraft aircraft);

            //Assert
            Assert.AreEqual(false, res.Status);
        }
        #endregion
    }
}