using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using FlightsInfo.Models;
using FlightsInfo.Helpers;
using FlightsInfo.Repositories;
using FlightsInfo.Services;
using System.Data.Entity;

namespace FlightsInfo
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            container.RegisterType<IFlightCalculator, FlightCalculator>();
            container.RegisterType<IFlightService, FlightService>();
            container.RegisterType<IFlightRepository, FlighRepository>();
            container.RegisterType<IFlightDBContext, FlightDBContext>();           

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}