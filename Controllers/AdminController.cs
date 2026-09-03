using KerlaVlogs.Data;
using KerlaVlogs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KerlaVlogs.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================
        // DASHBOARD
        // =========================

        public IActionResult Index()
        {
            var destinations = _context.Destinations.ToList();

            return View(destinations);
        }


        // =========================
        // CREATE DESTINATION
        // =========================

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Destination destination)
        {
            if (ModelState.IsValid)
            {
                _context.Destinations.Add(destination);

                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(destination);
        }


        // =========================
        // EDIT DESTINATION
        // =========================

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var destination = _context.Destinations.Find(id);

            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Destination destination)
        {
            if (id != destination.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Destinations.Update(destination);

                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(destination);
        }


        // =========================
        // DELETE DESTINATION
        // =========================

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var destination = _context.Destinations.Find(id);

            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var destination = _context.Destinations.Find(id);

            if (destination == null)
            {
                return NotFound();
            }

            _context.Destinations.Remove(destination);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        // =========================
        // ADD PHOTO - SHOW FORM
        // =========================

        [HttpGet]
        public IActionResult AddPhoto()
        {
            var itinerary = _context.Itineraries
                .OrderBy(x => x.DayNumber)
                .ToList();

            ViewBag.Itinerary = itinerary;

            return View();
        }


        // =========================
        // ADD PHOTO - SAVE
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoto(
            int ItineraryId,
            IFormFile photo,
            string Caption)
        {
            // Check photo
            if (photo == null || photo.Length == 0)
            {
                return BadRequest("Please select a photo.");
            }


            // Check itinerary
            var itinerary = await _context.Itineraries
                .FindAsync(ItineraryId);

            if (itinerary == null)
            {
                return BadRequest("Invalid itinerary selected.");
            }


            // Allowed file extensions
            var allowedExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };


            var extension = Path
                .GetExtension(photo.FileName)
                .ToLowerInvariant();


            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(
                    "Only JPG, JPEG, PNG and WEBP images are allowed.");
            }


            // Maximum 5 MB
            if (photo.Length > 5 * 1024 * 1024)
            {
                return BadRequest(
                    "Image size must be less than 5 MB.");
            }


            // Generate unique file name
            var fileName =
                Guid.NewGuid().ToString() + extension;


            // Image folder
            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images");


            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }


            // Complete file path
            var filePath = Path.Combine(
                folderPath,
                fileName);


            // Save image
            using (var stream = new FileStream(
                filePath,
                FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }


            // Save database record
            var gallery = new Gallery
            {
                ItineraryId = ItineraryId,
                ImagePath = "/images/" + fileName,
                Caption = Caption ?? ""
            };


            _context.Galleries.Add(gallery);

            await _context.SaveChangesAsync();


            return RedirectToAction("Index", "Admin");
        }


        // =========================
        // MANAGE PHOTOS
        // =========================

        [HttpGet]
        public IActionResult Photos()
        {
            var photos = _context.Galleries
                .Where(x => x.ItineraryId != null)
                .Include(x => x.Itinerary)
                .OrderBy(x => x.Itinerary!.DayNumber)
                .ThenBy(x => x.Id)
                .ToList();

            return View(photos);
        }


        // =========================
        // DELETE PHOTO
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePhoto(int id)
        {
            var photo = _context.Galleries.Find(id);

            if (photo == null)
            {
                return NotFound();
            }


            // Delete physical image
            var imagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                photo.ImagePath
                    .TrimStart('/')
                    .Replace(
                        "/",
                        Path.DirectorySeparatorChar.ToString()
                    )
            );


            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }


            // Delete database record
            _context.Galleries.Remove(photo);

            _context.SaveChanges();


            return RedirectToAction("Photos");
        }
    }
}