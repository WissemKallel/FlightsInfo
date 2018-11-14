using FlightsInfo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightsInfo.Helpers
{
    public interface IFlightDBContext
    {
        IDbSet<Flight> Flights { get; set; }
        IDbSet<Airport> Airports { get; set; }
        IDbSet<Aircraft> Aircrafts { get; set; }

        int SaveChanges();
        DbEntityEntry Entry(object flight);
        void Dispose();
    }
}
