namespace Laqeeta.Web.Configuration;

public class LaqeetaOptions
{
    public const string SectionName = "Laqeeta";

    // Base address of Laqeeta.API (must end with a slash).
    public string ApiBaseUrl { get; set; } = "https://localhost:7229/";

    // The API has no endpoint that lists stations, so the list lives here.
    public List<StationOption> Stations { get; set; } = new();
}

public class StationOption
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
