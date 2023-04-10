using FlightSightings.Data;
using FlightSightings.Interfaces;
using FlightSightings.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightSightings.Repositories
{
    public class FlightRepository : IFlight
    {
        private readonly FlightSpotterContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public FlightRepository(FlightSpotterContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public Flight Details(int Id)
        {
            Flight flight = _context.Flights.Where(f => f.Id == Id).FirstOrDefault() ?? throw new ArgumentException("Null Record Found");
            return flight;
        }

        public Flight Edit(int Id)
        {
            Flight flight = _context.Flights.Where(f => f.Id == Id).FirstOrDefault() ?? throw new ArgumentException("Null Record Found");
            return flight;
        }

        public async Task<Flight> Edit(Flight flight)
        {
            List<string> imgProp = new List<string>();
            if (flight.ImageFile != null)
            {
                imgProp = SaveUploadImage(flight.ImageFile.FileName);
            }

            if (imgProp.Count() > 0)
            {
                using (var fileStream = new FileStream(imgProp[0], FileMode.Create))
                {
                    if (flight.ImageFile != null)
                    {
                        await flight.ImageFile.CopyToAsync(fileStream);
                    }
                }
                flight.ImageName = imgProp[1];
            }

            if (flight.ImageFile != null)
            {
                flight.ImageData = GetByteArrayFromImage(flight.ImageFile);
            }

            _context.Attach(flight);
            _context.Entry(flight).State = EntityState.Modified;
            _context.SaveChanges();
            return flight;
        }

        public List<Flight> GetItems(string SortProperty, SortOrder sortOrder, string SearchText = "")
        {
            List<Flight> flights;

            if (SearchText != "" && SearchText != null)
            {
                flights = _context.Flights.Where(n => n.Make.Contains(SearchText) || n.Model.Contains(SearchText) || n.Registration.Contains(SearchText)).ToList();
            }
            else
            {
                flights = _context.Flights.ToList();
            }

            //List<Flight> flights = _context.Flights.ToList();

            if(SortProperty.ToLower() == "make")
            {
                if (sortOrder == SortOrder.Ascending)
                    flights = flights.OrderBy(n => n.Make).ToList();
                else
                    flights = flights.OrderByDescending(n => n.Make).ToList();
            }
            else if (SortProperty.ToLower() == "model")
            {
                if (sortOrder == SortOrder.Ascending)
                    flights = flights.OrderBy(m => m.Model).ToList();
                else
                    flights = flights.OrderByDescending(m => m.Model).ToList();
            }
            else
            {
                if (sortOrder == SortOrder.Ascending)
                    flights = flights.OrderBy(k => k.Registration).ToList();
                else
                    flights = flights.OrderByDescending(k => k.Registration).ToList();
            }

            return flights;
        }

        private List<string> SaveUploadImage(string uploadImg)
        {
            List<string> imagesProp = new List<string>();
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(uploadImg);
            string extension = Path.GetExtension(uploadImg);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwRootPath + "/images", fileName);

            imagesProp.Add(path);
            imagesProp.Add(fileName);
            return imagesProp;
        }

        private byte[] GetByteArrayFromImage(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }

        public Flight Delete(int Id)
        {
            Flight flight = _context.Flights.Where(f => f.Id == Id).FirstOrDefault() ?? throw new ArgumentException("Null Record Found");
            return flight;
        }

        public Flight Delete(Flight flight)
        {
            _context.Attach(flight);
            _context.Entry(flight).State = EntityState.Deleted;
            _context.SaveChanges();
            return flight;
        }

        public async Task<Flight> Create(Flight flight)
        {
            List<string> imgProp = new List<string>();
            if (flight.ImageFile != null)
            {
                imgProp = SaveUploadImage(flight.ImageFile.FileName);
            }

            using (var fileStream = new FileStream(imgProp[0], FileMode.Create))
            {
                if (flight.ImageFile != null)
                {
                    await flight.ImageFile.CopyToAsync(fileStream);
                }
            }

            if (flight.ImageFile != null)
            {
                flight.ImageData = GetByteArrayFromImage(flight.ImageFile);
            }

            flight.ImageName = imgProp[1];
            _context.Attach(flight);
            _context.Entry(flight).State = EntityState.Added;
            _context.SaveChanges();
            return flight;
        }
    }
}
