using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FlightsInfo.Models;
using FlightsInfo.Services;

namespace FlightsInfo.Controllers
{
    public class FlightsController : Controller
    {
        private IFlightService flightService;

        public FlightsController(IFlightService flightService)
        {
            this.flightService = flightService ?? throw new ArgumentNullException("flightService");
        }

        // GET: Flights
        public ActionResult Index()
        {
            var res = flightService.GetFlightsToDisplay(out List<FlightToDisplay> flightsToDisplay)
                    ?? throw new ArgumentNullException("res");

            if (!res.Status)
                return HttpNotFound(res.Info);

            return View(flightsToDisplay);
        }

        public ActionResult Report()
        {
            var res = flightService.GetFlightsToDisplay(out List<FlightToDisplay> flightsToDisplay)
                    ?? throw new ArgumentNullException("res");

            if (!res.Status)
                return HttpNotFound(res.Info);

            return View(flightsToDisplay);
        }

        // GET: Flights/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var res = flightService.GetFlightToDisplay(id.Value, out FlightToDisplay flightToDisplay)
                    ?? throw new ArgumentNullException("res");

            if (!res.Status)
                return HttpNotFound(res.Info);

            return View(flightToDisplay);
        }

        // GET: Flights/Create
        public ActionResult Create()
        {
            ViewBag.SelectableAirports = flightService.Airports;
            ViewBag.SelectableAircrafts = flightService.Aircrafts;
            return View();
        }

        // POST: Flights/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartureAirportID,DestinationAirportID,AircraftID")] FlightToDisplay flightInput)
        {
            if (ModelState.IsValid)
            {
                var res = flightService.AddFlight(flightInput)
                    ?? throw new ArgumentNullException("res");

                switch (res.Status)
                {
                    case AddResultStatus.SameAirportsChosen:
                        //The airports chosen are the same, retry
                        ModelState.AddModelError("", "Please choose different airports");
                        break;

                    case AddResultStatus.AlreadyExists:
                        //Flight exists in the DB, retry
                        ModelState.AddModelError("", "The entered flight already exists, please choose another one");
                        break;

                    case AddResultStatus.Success:
                        //New entry successfully added
                        return RedirectToAction("Index");

                    default:
                        //Internal Error
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Unexpected Internal Error: "+ res.Info);
                }
            }

            ViewBag.SelectableAirports = flightService.Airports;
            ViewBag.SelectableAircrafts = flightService.Aircrafts;
            return View(flightInput);
        }

        // GET: Flights/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var res = flightService.GetFlightToDisplay(id.Value, out FlightToDisplay output)
                    ?? throw new ArgumentNullException("res");

            if (!res.Status)
                return HttpNotFound(res.Info);

            ViewBag.SelectableAirports = flightService.Airports;
            ViewBag.SelectableAircrafts = flightService.Aircrafts;
            return View(output);
        }

        // POST: Flights/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FlightID,DepartureAirportID,DestinationAirportID,AircraftID")] FlightToDisplay flightInput)
        {
            if (ModelState.IsValid)
            {
                var res = flightService.EditFlight(flightInput)
                    ?? throw new ArgumentNullException("res");

                switch (res.Status)
                {
                    case EditResultStatus.SameAirportsChosen:
                        //The airports chosen are the same, retry
                        ModelState.AddModelError("", "Please choose different airports");
                        break;

                    case EditResultStatus.AlreadyExists:
                        //Flight exists in the DB, retry
                        ModelState.AddModelError("", "The entered flight already exists, please choose another one");
                        break;

                    case EditResultStatus.EntriesNotChanged:
                    case EditResultStatus.Success:
                        //Flight succesfully edited
                        return RedirectToAction("Index");

                    default:
                        //Internal error
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Unexpected Internal Error: " + res.Info);
                }
            }

            ViewBag.SelectableAirports = flightService.Airports;
            ViewBag.SelectableAircrafts = flightService.Aircrafts;
            return View(flightInput);
        }

        // GET: Flights/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var res = flightService.GetFlightToDisplay(id.Value, out FlightToDisplay output)
                    ?? throw new ArgumentNullException("res");

            if (!res.Status)
                return HttpNotFound(res.Info);

            return View(output);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var res = flightService.RemoveFlight(id) ?? throw new ArgumentNullException("res");

            if (res.Status == RemoveResultStatus.Success)
                return RedirectToAction("Index");
            else
                return HttpNotFound(res.Info);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                flightService.DisposeOfStorage();
            }
            base.Dispose(disposing);
        }
    }
}
