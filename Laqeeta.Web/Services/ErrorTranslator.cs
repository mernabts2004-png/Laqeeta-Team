namespace Laqeeta.Web.Services;

// The API returns English plain-text errors. Known ones are shown in Arabic.
public static class ErrorTranslator
{
    private static readonly Dictionary<string, string> Messages =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Invalid email or password."] = "الإيميل أو كلمة المرور غلط.",
            ["Report not found."] = "البلاغ مش موجود، أو رقم الموبايل مش مطابق للبلاغ.",
            ["This report is closed."] = "البلاغ ده اتقفل خلاص.",
            ["Item not found."] = "الغرض مش موجود.",
            ["This item is not available for claims."] = "الغرض ده مش متاح للاستلام.",
            ["This item does not match the lost report."] = "الغرض ده مش متطابق مع بلاغ الفقدان.",
            ["This claim already exists."] = "طلب الاستلام ده اتعمل قبل كده.",
            ["Claim not found."] = "طلب الاستلام مش موجود (أو رقم الموبايل مش مطابق).",
            ["Only pending claims can be verified."] = "التحقق ممكن بس للطلبات اللي لسه في الانتظار (Pending).",
            ["No secret question is configured for this report."] = "مفيش سؤال سري متسجل للبلاغ ده.",
            ["ID verification failed."] = "لازم تتأكد من البطاقة الشخصية الأول وتعلّم على خانة التحقق.",
            ["Secret answer is required."] = "إجابة السؤال السري مطلوبة.",
            ["Secret answer is not configured."] = "مفيش إجابة سرية متسجلة للبلاغ ده.",
            ["Incorrect secret answer."] = "إجابة السؤال السري غلط.",
            ["Only pending claims can be approved."] = "الموافقة ممكن بس على الطلبات اللي في الانتظار (Pending).",
            ["Ownership verification is required."] = "لازم تتحقق من المالك الأول قبل الموافقة.",
            ["This item is no longer available."] = "الغرض ده مبقاش متاح.",
            ["This item has already been claimed."] = "الغرض ده اتوافق على استلامه قبل كده.",
            ["Another staff member is working on this item. Please try again."] = "موظف تاني بيشتغل على نفس الغرض دلوقتي. جرّب تاني.",
            ["Only pending claims can be rejected."] = "الرفض ممكن بس للطلبات اللي في الانتظار (Pending).",
            ["Only approved claims can be handed over."] = "التسليم ممكن بس للطلبات اللي اتوافق عليها (Approved).",
            ["This item is not reserved for this claim."] = "الغرض ده مش محجوز لطلب الاستلام ده.",
            ["Required fields cannot be empty."] = "كل الحقول المطلوبة لازم تتملي.",
            ["LostAt cannot be in the future."] = "وقت الفقدان مينفعش يبقى في المستقبل.",
            ["FoundAt cannot be in the future."] = "وقت اللقاء مينفعش يبقى في المستقبل.",
            ["Secret question and answer must be provided together."] = "السؤال السري وإجابته لازم يتبعتوا مع بعض.",
            ["Secret question and answer are required for high-value items."] = "السؤال السري وإجابته مطلوبين للحاجات الغالية (موبايل / محفظة / بطاقة).",
            ["Invalid station ID."] = "رقم المحطة غلط."
        };

    public static string Translate(string message)
    {
        var text = message.Trim();

        return Messages.TryGetValue(text, out var arabic) ? arabic : text;
    }
}
