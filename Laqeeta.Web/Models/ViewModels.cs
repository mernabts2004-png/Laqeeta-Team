using System.ComponentModel.DataAnnotations;
using Laqeeta.Web.Helpers;

namespace Laqeeta.Web.Models;

// ---------- Account ----------

public class LoginViewModel
{
    [Required(ErrorMessage = "الإيميل مطلوب")]
    [EmailAddress(ErrorMessage = "صيغة الإيميل غير صحيحة")]
    [Display(Name = "الإيميل")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [DataType(DataType.Password)]
    [Display(Name = "كلمة المرور")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

// ---------- Shared item fields (lost report + found item) ----------

public abstract class ItemFormBase
{
    [Required(ErrorMessage = "اختار نوع الحاجة")]
    [MaxLength(50)]
    [Display(Name = "نوع الحاجة")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "اختار اللون")]
    [MaxLength(50)]
    [Display(Name = "اللون")]
    public string Color { get; set; } = string.Empty;

    [Required(ErrorMessage = "اختار الخط")]
    [MaxLength(50)]
    [Display(Name = "الخط")]
    public string Line { get; set; } = string.Empty;

    [Required(ErrorMessage = "اختار الاتجاه")]
    [MaxLength(100)]
    [Display(Name = "الاتجاه")]
    public string Direction { get; set; } = string.Empty;

    // Lost time for passengers, found time for staff (Egypt local time).
    [Required(ErrorMessage = "الوقت مطلوب")]
    [Display(Name = "الوقت")]
    public DateTime EventAt { get; set; }

    [Required(ErrorMessage = "اكتب وصف للحاجة")]
    [MaxLength(500, ErrorMessage = "الوصف أطول من 500 حرف")]
    [Display(Name = "الوصف")]
    public string Description { get; set; } = string.Empty;
}

// ---------- Passenger ----------

public class CreateReportViewModel : ItemFormBase, IValidatableObject
{
    [Required(ErrorMessage = "رقم الموبايل مطلوب")]
    [RegularExpression(@"^[0-9٠-٩+\-\s()]{8,20}$", ErrorMessage = "رقم الموبايل غير صحيح")]
    [Display(Name = "رقم الموبايل")]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "السؤال أطول من 200 حرف")]
    [Display(Name = "السؤال السري")]
    public string? SecretQuestion { get; set; }

    [MaxLength(200, ErrorMessage = "الإجابة أطول من 200 حرف")]
    [Display(Name = "إجابة السؤال السري")]
    public string? SecretAnswer { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (LookupData.IsHighValue(Category))
        {
            if (string.IsNullOrWhiteSpace(SecretQuestion))
            {
                yield return new ValidationResult(
                    "السؤال السري مطلوب للحاجات الغالية (موبايل / محفظة / بطاقة).",
                    new[] { nameof(SecretQuestion) });
            }

            if (string.IsNullOrWhiteSpace(SecretAnswer))
            {
                yield return new ValidationResult(
                    "إجابة السؤال السري مطلوبة للحاجات الغالية.",
                    new[] { nameof(SecretAnswer) });
            }
        }
    }
}

public class TrackRequestViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "رقم البلاغ لازم يكون رقم صحيح")]
    [Display(Name = "رقم البلاغ")]
    public int ReportId { get; set; }

    [Required(ErrorMessage = "رقم الموبايل مطلوب")]
    [RegularExpression(@"^[0-9٠-٩+\-\s()]{8,20}$", ErrorMessage = "رقم الموبايل غير صحيح")]
    [Display(Name = "رقم الموبايل")]
    public string PhoneNumber { get; set; } = string.Empty;
}

public class TrackResultViewModel
{
    public int ReportId { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public List<PossibleMatchResponse> Matches { get; set; } = new();

    public string? MatchesError { get; set; }
}

public class ClaimStatusLookupViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "رقم الطلب لازم يكون رقم صحيح")]
    [Display(Name = "رقم طلب الاستلام")]
    public int ClaimId { get; set; }

    [Required(ErrorMessage = "رقم الموبايل مطلوب")]
    [RegularExpression(@"^[0-9٠-٩+\-\s()]{8,20}$", ErrorMessage = "رقم الموبايل غير صحيح")]
    [Display(Name = "رقم الموبايل")]
    public string PhoneNumber { get; set; } = string.Empty;
}

public class ClaimResultViewModel
{
    public ClaimResponse? Claim { get; set; }

    public string? Error { get; set; }

    // Shown when a claim has just been created.
    public bool JustCreated { get; set; }
}

// ---------- Staff ----------

public class FoundItemViewModel : ItemFormBase
{
    [Required(ErrorMessage = "اختار المحطة")]
    [Display(Name = "المحطة")]
    public int? StationId { get; set; }
}

public class StaffClaimViewModel
{
    public int ClaimId { get; set; }

    public string? SecretQuestion { get; set; }

    // Explains why there is no secret question (claim not pending, etc.).
    public string? Note { get; set; }
}
