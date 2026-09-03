namespace KerlaVlogs.Models
{
    public class Itinerary
    {
        public int Id { get; set; }

        public int DayNumber { get; set; }

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public string Location { get; set; } = "";

        public int? DestinationId { get; set; }

        public Destination? Destination { get; set; }
    }
}