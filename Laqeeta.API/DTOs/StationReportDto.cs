namespace Laqeeta.API.DTOs
{
    public class StationReportDto
    {
        public int StationId { get; set; }

        public string StationName { get; set; } = string.Empty;

        public int FoundItemsCount { get; set; }

        public int ReturnedItemsCount { get; set; }

        public decimal ReturnRate { get; set; }
    }
}