namespace Laqeeta.API.DTOs
{
    public class CreateFoundItemResponseDto
    {
        public int ItemId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string StorageLocation { get; set; } = string.Empty;

        public DateTime FoundAt { get; set; }
    }
}