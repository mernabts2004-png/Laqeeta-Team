namespace Laqeeta.API.DTOs
{
    public class PossibleMatchResponseDto
    {
        public int ItemId { get; set; }

        public string Category { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public string Line { get; set; } = string.Empty;

        public string Direction { get; set; } = string.Empty;

        public DateTime FoundAt { get; set; }

        public string Description { get; set; } = string.Empty;

        public string StationName { get; set; } = string.Empty;
    }
}