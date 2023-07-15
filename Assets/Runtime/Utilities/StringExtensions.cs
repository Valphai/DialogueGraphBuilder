using System.Collections.Generic;
using System.Linq;

namespace Chocolate4.Dialogue.Runtime.Utilities
{
	public static class StringExtensions
	{
        public static int GetMemberIndex(string member)
        {
            int index = 0;
            IEnumerable<char> digits = member.Where(char.IsDigit);

            if (digits.IsNullOrEmpty())
            {
                return index;
            }

            string digitsCombined = new string(digits.ToArray());
            return int.Parse(digitsCombined);
        }

        public static string Sanitize(this string entry)
        {
            return entry
                .Replace("\n", string.Empty)
                .Replace("\t", string.Empty)
                .Replace("\r", string.Empty)
                .Replace(" ", string.Empty);
        }
    }
}