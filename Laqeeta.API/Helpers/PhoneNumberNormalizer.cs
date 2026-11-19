using System.Text;

namespace Laqeeta.API.Helpers
{
    public static class PhoneNumberNormalizer
    {
        public static string Normalize(string phoneNumber)
        {
            var result = new StringBuilder();

            foreach (var ch in phoneNumber.Trim())
            {
                if (char.IsDigit(ch))
                {
                    // Converts Arabic digits (٠١٢) to normal digits (012).
                    result.Append((int)char.GetNumericValue(ch));
                }
                else if (ch == '+')
                {
                    result.Append(ch);
                }
                // Spaces, dashes, brackets and letters are dropped.
            }

            return result.ToString();
        }
    }
}