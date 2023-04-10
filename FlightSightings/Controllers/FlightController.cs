using FlightSightings.Data;
using FlightSightings.Interfaces;
using FlightSightings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightSightings.Controllers
{
    public class FlightController : Controller
    {
        private readonly FlightSpotterContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IFlight _flightRepo;

        public FlightController(FlightSpotterContext context, IWebHostEnvironment hostEnvironment, IFlight flightRepo)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
            _flightRepo = flightRepo;
        }

        public IActionResult Index(int pg = 1, string sortExpression = "", string SearchText = "")
        {
            SortOrder sortOrder;
            string sortProperty;
            //async

            ViewData["SortParamMake"] = "make";
            ViewData["SortParamModel"] = "model";

            ViewData["SortIconMake"] = "";
            ViewData["SortIconModel"] = "";

            switch (sortExpression.ToLower())
            {
                case "make_desc":
                    sortOrder = SortOrder.Descending;
                    sortProperty = "make";
                    ViewData["SortParamMake"] = "make";
                    ViewData["SortIconMake"] = "fa fa-arrow-up";
                    break;
                case "model":
                    sortOrder = SortOrder.Ascending;
                    sortProperty = "model";
                    ViewData["SortParamModel"] = "model_desc";
                    ViewData["SortIconModel"] = "fa fa-arrow-down";
                    break;
                case "model_desc":
                    sortOrder = SortOrder.Descending;
                    sortProperty = "model";
                    ViewData["SortParamModel"] = "model";
                    ViewData["SortIconModel"] = "fa fa-arrow-up";
                    break;
                default:
                    sortOrder = SortOrder.Ascending;
                    sortProperty = "make";
                    ViewData["SortParamMake"] = "make_desc";
                    ViewData["SortIconMake"] = "fa fa-arrow-down";
                    break;
            }

            ViewBag.SearchText = SearchText;
            List<Flight> flight = _flightRepo.GetItems(sortProperty, sortOrder, SearchText);

            const int pageSize = 5;
            if (pg < 1)
                pg = 1;

            int recsCnt = _context.Flights.Count();
            var pager = new Pager(recsCnt, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = flight.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;

            return View(data);
        }

        public IActionResult Details(int Id)
        {
            Flight flight = _flightRepo.Details(Id);
            return View(flight);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            Flight flight = _flightRepo.Edit(Id);
            return View(flight);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Flight flight)
        {
            try
            {
                await _flightRepo.Edit(flight);
                TempData["editMessage"] = "Flight Reigster Edited Successfully";
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int Id)
        {
            Flight flight = _flightRepo.Delete(Id);
            return View(flight);
        }

        [HttpPost]
        public IActionResult Delete(Flight flight)
        {
            try
            {
                flight = _flightRepo.Delete(flight);
                TempData["deleteMessage"] = "Flight Reigster Deleted Successfully";
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            Flight flight = new Flight();
            return View(flight);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Flight flight)
        {
            try
            {
                await _flightRepo.Create(flight);
                TempData["successMessage"] = "Flight Reigster Created Successfully";
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Flight Reigster Has An Error, Please Contact Admin...!";
            }
            return RedirectToAction("Index");
        }
    }
}
