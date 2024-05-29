
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.Helpers
{
	public class PagedList<T>
    {
        private PagedList(IQueryable<T> items, int page, int pageSize, int totalCount)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
        public IQueryable<T> Items { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public bool HasNextPage => Page * PageSize < TotalCount;
        public bool HasPreviousPage => Page > 1;
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, int page, int pageSize)
        {
            var totalCount = query.Count();
            IQueryable<T> items = query.Skip((page - 1) * pageSize).Take(pageSize).AsQueryable<T>();
            return new(items, page, pageSize, totalCount);
        }
    }
}