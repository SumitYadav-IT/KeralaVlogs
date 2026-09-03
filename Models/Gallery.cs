using KerlaVlogs.Models;

namespace KerlaVlogs.Models
{
    public class Gallery
    {
        public int Id { get; set; }

        public int DestinationId { get; set; }

        public int? ItineraryId { get; set; }

        public string ImagePath { get; set; } = "";

        public string Caption { get; set; } = "";

        public Itinerary? Itinerary { get; set; }
    }
}