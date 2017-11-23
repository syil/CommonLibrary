using CommonLibrary.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Extensions
{
    public static class EnumerableExtensions
    {
        public static PagedList<T> Pagify<T>(this IEnumerable<T> collection, int count, int page)
        {
            int totalCount = collection.Count();
            return new PagedList<T>(collection.Skip((page - 1) * count).Take(count), totalCount, page);
        }

        public static IEnumerable<T> Descendants<T>(this T root)
            where T : IEnumerable<T>
        {
            var items = new Stack<T>(new[] { root });
            while (items.Any())
            {
                T node = items.Pop();
                yield return node;

                foreach (var n in node)
                    items.Push(n);
            }
        }
    }
}
