using System;
using System.Collections.Generic;
using System.Linq;

namespace EmpLMS.Models.ViewModels
{
    // Helper class for handling table pagination
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }

        public PaginatedList()
        {
        }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize <= 0 ? 5 : pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            if (TotalPages < 1) TotalPages = 1;

            if (pageIndex < 1) pageIndex = 1;
            if (pageIndex > TotalPages && count > 0) pageIndex = TotalPages;
            PageIndex = pageIndex;

            AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public int StartItem => TotalCount == 0 ? 0 : (PageIndex - 1) * PageSize + 1;
        public int EndItem => Math.Min(PageIndex * PageSize, TotalCount);

        public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize = 5)
        {
            if (source == null)
            {
                return new PaginatedList<T>(new List<T>(), 0, 1, pageSize);
            }

            var count = source.Count();
            if (pageSize <= 0) pageSize = 5;
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (totalPages < 1) totalPages = 1;

            if (pageIndex < 1) pageIndex = 1;
            if (pageIndex > totalPages && count > 0) pageIndex = totalPages;

            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
