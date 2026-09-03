
using KerlaVlogs.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KerlaVlogs.Controllers
{
    public class GalleryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GalleryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // All Photos
        public IActionResult Index(int? destinationId, int? itineraryId)
        {
            // Day-wise Gallery
            if (itineraryId.HasValue)
            {
                var day = _context.Itineraries
                    .FirstOrDefault(x => x.Id == itineraryId.Value);

                if (day == null)
                {
                    return NotFound();
                }

                var dayPhotos = _context.Galleries
                    .Where(x => x.ItineraryId == itineraryId.Value)
                    .OrderBy(x => x.Id)
                    .ToList();

                ViewBag.Title = $"Day {day.DayNumber} - {day.Title}";
                ViewBag.Subtitle = $"Memories from Day {day.DayNumber}";

                return View(dayPhotos);
            }

            // Destination Gallery
            if (destinationId.HasValue)
            {
                var destination = _context.Destinations
                    .FirstOrDefault(x => x.Id == destinationId.Value);

                if (destination == null)
                {
                    return NotFound();
                }

                var photos = _context.Galleries
                    .Where(x => x.DestinationId == destinationId.Value)
                    .OrderBy(x => x.Id)
                    .ToList();

                ViewBag.Title = $"{destination.Name} Gallery";
                ViewBag.Subtitle =
                    $"Memories from my {destination.Name} journey";

                return View(photos);
            }

            // ALL uploaded journey photos
            var allPhotos = _context.Galleries
                .Where(x => x.ItineraryId != null)
                .Include(x => x.Itinerary)
                .OrderBy(x => x.Itinerary!.DayNumber)
                .ThenBy(x => x.Id)
                .ToList();

            ViewBag.Title = "My Kerala Gallery";
            ViewBag.Subtitle =
                "All the beautiful memories from my 7-day Kerala journey";

            return View(allPhotos);
        }
    }
}