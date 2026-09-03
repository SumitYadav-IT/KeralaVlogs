
using KerlaVlogs.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KerlaVlog.Controllers
{
    public class JourneyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JourneyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var destinations = _context.Destinations.ToList();

            return View(destinations);
        }

        public IActionResult Details(int id)
        {
            var destination = _context.Destinations
                                      .FirstOrDefault(x => x.Id == id);

            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }

        public IActionResult Itinerary()
        {
            var itinerary = _context.Itineraries
                .Include(x => x.Destination)
                .OrderBy(x => x.DayNumber)
                .ToList();

            ViewBag.Photos = _context.Galleries
                .Where(x => x.ItineraryId != null)
                .ToList();

            return View(itinerary);
        }
    }
}