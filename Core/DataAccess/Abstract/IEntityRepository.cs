using Core.Extensions;
using Core.Models;
using System.Linq.Expressions;

namespace Core.DataAccess.Abstract
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        int Count(Expression<Func<T, bool>>? filter = null);

        List<T> GetAll(Expression<Func<T, bool>>? filter = null, bool? asNoTracking = false, EntitySortModel<T>? sort = null);
        T? Get(Expression<Func<T, bool>> filter, bool? asNoTracking = false, EntitySortModel<T>? sort = null);

        Task<Paginate<T>> GetListPaginateAsync(Expression<Func<T, bool>>? filter = null, int index = 0, int size = 10,
            EntitySortModel<T>? sort = null, bool? asNoTracking = false);

        Paginate<T> GetListPaginate(Expression<Func<T, bool>>? filter = null, int index = 0, int size = 10,
            EntitySortModel<T>? sort = null, bool? asNoTracking = false);

        T? FirstOrDefault(Expression<Func<T, object>> sort);
        T? LastOrDefault(Expression<Func<T, object>> sort);

        void Add(T entity);
        void AddRange(IEnumerable<T> entities);

        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

        // async
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool? asNoTracking = false, EntitySortModel<T>? sort = null);
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, bool? asNoTracking = false, EntitySortModel<T>? sort = null);

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);

        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);
    }

    public record EntitySortModel<TEntity>
    {
        public Expression<Func<TEntity, object>> sort { get; set; } = null!;
        public SortType sortType { get; set; }
    }
}