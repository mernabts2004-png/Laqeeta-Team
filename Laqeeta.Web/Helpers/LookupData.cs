namespace Laqeeta.Web.Helpers;

public record LookupItem(string Value, string Label, bool HighValue = false);

// The API stores category / line / direction as free text and matches them
// with exact equality, so the website offers fixed lists to keep them consistent.
public static class LookupData
{
    // Values are English on purpose: the API decides "high value" by English words.
    public static readonly IReadOnlyList<LookupItem> Categories = new List<LookupItem>
    {
        new("Phone", "موبايل / تليفون", true),
        new("Wallet", "محفظة", true),
        new("ID", "بطاقة هوية / رخصة", true),
        new("Bag", "شنطة"),
        new("Keys", "مفاتيح"),
        new("Laptop", "لاب توب / تابلت"),
        new("Glasses", "نظارة"),
        new("Clothes", "ملابس"),
        new("Other", "حاجة تانية")
    };

    public static readonly IReadOnlyList<LookupItem> Colors = new List<LookupItem>
    {
        new("Black", "أسود"),
        new("White", "أبيض"),
        new("Gray", "رمادي"),
        new("Brown", "بني"),
        new("Blue", "أزرق"),
        new("Red", "أحمر"),
        new("Green", "أخضر"),
        new("Yellow", "أصفر"),
        new("Other", "لون تاني")
    };

    public static readonly IReadOnlyList<LookupItem> Lines = new List<LookupItem>
    {
        new("Line 1", "الخط الأول"),
        new("Line 2", "الخط التاني"),
        new("Line 3", "الخط التالت")
    };

    public static readonly IReadOnlyList<LookupItem> Directions = new List<LookupItem>
    {
        new("Northbound", "اتجاه الشمال (Northbound)"),
        new("Southbound", "اتجاه الجنوب (Southbound)")
    };

    public static bool IsHighValue(string? category) =>
        Categories.Any(c =>
            c.HighValue &&
            string.Equals(c.Value, category?.Trim(), StringComparison.OrdinalIgnoreCase));

    public static string CategoryLabel(string value) => LabelOf(Categories, value);

    public static string ColorLabel(string value) => LabelOf(Colors, value);

    public static string LineLabel(string value) => LabelOf(Lines, value);

    public static string DirectionLabel(string value) => LabelOf(Directions, value);

    private static string LabelOf(IReadOnlyList<LookupItem> items, string value) =>
        items.FirstOrDefault(i =>
            string.Equals(i.Value, value, StringComparison.OrdinalIgnoreCase))?.Label ?? value;
}
