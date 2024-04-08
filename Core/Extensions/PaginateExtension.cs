using Core.Helpers;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Extensions
{
    public class PageRequest
    {
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }

    public class Paginate<T>
    {
        public int From { get; set; } = 0;
        public int Index { get; set; } = 0;
        public int Size { get; set; } = 0;
        public int Count { get; set; } = 0;
        public int Pages { get; set; } = 0;
        public List<T> Items { get; set; } = [];
        public bool HasPrevious { get; set; } = false;
        public bool HasNext { get; set; } = false;
    }

    public static class PaginateExtension
    {
        public static Paginate<T> ToPaginate<T>(this IQueryable<T> source, int index, int size, int from = 0)
        {
            try
            {
                int count = source.Count(); // paginate harici datanın sayısı çeker
                var pages = (int)Math.Ceiling(count / (double)size);

                return new Paginate<T>()
                {
                    Index = index,
                    Size = size,
                    From = from,
                    Count = count,
                    Items = [.. source.Skip((index - from) * size).Take(size)], // paginate datasını çeker
                    Pages = pages,
                    HasPrevious = (index - from) > 0,
                    HasNext = (index - from + 1) < pages
                };
            }
            catch (Exception e)
            {
                GeneralStaticHelper.LogWrite($"ToPaginate() => entity type: {typeof(T)}", LogType.ERROR, e);

                return new Paginate<T>();
            }
        }
        public static async Task<Paginate<T>> ToPaginateAsync<T>(this IQueryable<T> source, int index, int size, int from = 0)
        {
            try
            {
                int count = await source.CountAsync(); // paginate harici datanın sayısı çeker
                var pages = (int)Math.Ceiling(count / (double)size);

                var items = await source.Skip((index - from) * size).Take(size).ToListAsync();  // paginate datasını çeker

                return new Paginate<T>()
                {
                    Index = index,
                    Size = size,
                    From = from,
                    Count = count,
                    Items = items,
                    Pages = pages,
                    HasPrevious = (index - from) > 0,
                    HasNext = (index - from + 1) < pages
                };
            }
            catch (Exception e)
            {
                GeneralStaticHelper.LogWrite($"ToPaginate() => entity type: {typeof(T)}", LogType.ERROR, e);

                return new Paginate<T>();
            }
        }
    }
}