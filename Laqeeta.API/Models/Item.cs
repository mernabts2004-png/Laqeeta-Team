namespace Laqeeta.API.Models
{
    public class Item
    {
        public int ItemId { get; set; }

        public string Category { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public string Line { get; set; } = string.Empty;

        public string Direction { get; set; } = string.Empty;

        public DateTime FoundAt { get; set; }

        public string Description { get; set; } = string.Empty;

        public int FoundAtStationId { get; set; }

        public Station FoundAtStation { get; set; } = null!;

        public string StorageLocation { get; set; } = "Station";

        public DateTime? MovedToCentralAt { get; set; }

        public string Status { get; set; } = "Found";
    }
}