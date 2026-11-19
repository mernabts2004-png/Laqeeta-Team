namespace Laqeeta.API.Models
{
    public class Station
    {
        public int StationId { get; set; }

        public string Name { get; set; } = string.Empty;

        // Navigation Property
        public ICollection<Item> FoundItems { get; set; } = new List<Item>();
    }
}