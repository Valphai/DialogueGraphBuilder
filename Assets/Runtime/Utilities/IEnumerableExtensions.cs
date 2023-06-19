using System.Collections.Generic;
using System.Linq;

namespace Chocolate4.Dialogue.Runtime.Utilities
{
    public static class IEnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) => enumerable == null || enumerable.Count() == 0;
    }
}
