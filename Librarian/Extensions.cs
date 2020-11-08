using System.Collections.Generic;
using System.Linq;

namespace Librarian
{
    public static class Extensions
    {
        public static void RemoveAll<T>(this ICollection<T> list, IEnumerable<T> toRemove)
        {
            foreach (var element in toRemove.ToList())
            {
                list.Remove(element);
            }
        }
    }
}
