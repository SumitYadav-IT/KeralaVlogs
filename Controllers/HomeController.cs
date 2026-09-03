using KerlaVlogs.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KerlaVlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var destinations = _context.Destinations
                .Take(3)
                .ToList();

            var itinerary = _context.Itineraries
                .OrderBy(x => x.DayNumber)
                .ToList();

            ViewBag.Destinations = destinations;
            ViewBag.Itinerary = itinerary;

            return View();
        }
    }
}