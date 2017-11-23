using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Collections
{
    public interface IPagedList<T> : IList<T>
    {
        int TotalCount { get; }
        int CurrentPage { get; }
    }
}
