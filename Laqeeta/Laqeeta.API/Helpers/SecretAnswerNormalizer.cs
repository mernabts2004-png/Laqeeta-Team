using System.Text.RegularExpressions;

namespace Laqeeta.API.Helpers
{
    public static class SecretAnswerNormalizer
    {
        public static string Normalize(string answer)
        {
            var trimmed = answer.Trim();

            // Collapse repeated spaces into a single space.
            var collapsed = Regex.Replace(trimmed, @"\s+", " ");

            return collapsed.ToLowerInvariant();
        }
    }
}