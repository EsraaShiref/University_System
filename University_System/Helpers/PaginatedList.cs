using Microsoft.EntityFrameworkCore;

namespace University_System.Helpers
{
    public interface IPagedList
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalPages { get; }
        int TotalCount { get; }
    }

    public class PaginatedList<T> : List<T>, IPagedList
    {
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            TotalCount = count;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(count / (double)pageSize));
            pageIndex = Math.Min(Math.Max(1, pageIndex), totalPages);

            var items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
