using System.Globalization;

namespace Laqeeta.Web.Helpers;

public record StatusBadge(string Text, string Css);

public static class UiHelper
{
    private static readonly TimeZoneInfo EgyptTimeZone = FindEgyptTimeZone();

    // ---------- Time ----------

    // The API returns UTC values without a "Z"; the website shows Egypt time.
    public static DateTime ToEgypt(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utc, DateTimeKind.Utc),
            EgyptTimeZone);

    public static string EgyptTime(DateTime utc) =>
        ToEgypt(utc).ToString("dd/MM/yyyy  HH:mm", CultureInfo.InvariantCulture);

    public static string EgyptTime(DateTime? utc) =>
        utc.HasValue ? EgyptTime(utc.Value) : "—";

    // Current Egypt time, rounded down to the minute (for datetime-local inputs).
    public static DateTime EgyptNow()
    {
        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EgyptTimeZone);

        return new DateTime(
            now.Year, now.Month, now.Day, now.Hour, now.Minute, 0,
            DateTimeKind.Unspecified);
    }

    public static string DateTimeLocalValue(DateTime value) =>
        value.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);

    // ---------- Status badges ----------

    public static StatusBadge ReportStatus(string status) => status switch
    {
        "Searching" => new("جاري البحث", "text-bg-secondary"),
        "PossibleMatch" => new("فيه أغراض ممكن تكون بتاعتك", "text-bg-warning"),
        "ReadyToCollect" => new("جاهز للاستلام", "text-bg-success"),
        "Closed" => new("تم التسليم وقفل البلاغ", "text-bg-dark"),
        _ => new(status, "text-bg-light")
    };

    public static StatusBadge ClaimStatus(string status) => status switch
    {
        "Pending" => new("في انتظار الموظف", "text-bg-warning"),
        "Approved" => new("تمت الموافقة", "text-bg-success"),
        "Completed" => new("تم التسليم", "text-bg-dark"),
        "Rejected" => new("مرفوض", "text-bg-danger"),
        _ => new(status, "text-bg-light")
    };

    private static TimeZoneInfo FindEgyptTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
        }
    }
}
