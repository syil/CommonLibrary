using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Collections
{
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public int TotalCount { get; protected set; }
        public int CurrentPage { get; protected set; }

        public PagedList(IEnumerable<T> collection)
            : base(collection)
        {
            TotalCount = collection.Count();
            CurrentPage = 1;
        }

        public PagedList(IEnumerable<T> collection, int totalCount, int currentPage)
            : base(collection)
        {
            TotalCount = totalCount;
            CurrentPage = currentPage;
        }
    }
}
