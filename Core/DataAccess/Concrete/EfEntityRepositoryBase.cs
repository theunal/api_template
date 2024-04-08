using Core.DataAccess.Abstract;
using Core.Extensions;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.DataAccess.Concrete
{
    public class EfEntityRepositoryBase<TEntity, TContext>(TContext context) : IEntityRepository<TEntity>
         where TEntity : class, IEntity, new()
         where TContext : DbContext, new()
    {
        public int Count(Expression<Func<TEntity, bool>>? filter = null)
            => (filter is not null ? context.Set<TEntity>().Where(filter) : context.Set<TEntity>()).Count();

        public List<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null, bool? asNoTracking = false, EntitySortModel<TEntity>? sort = null) =>
             GetAllQueryable(filter, asNoTracking, sort).ToList();

        public TEntity? Get(Expression<Func<TEntity, bool>> filter, bool? asNoTracking = false, EntitySortModel<TEntity>? sort = null) =>
            GetQueryable(filter, asNoTracking, sort).FirstOrDefault();

        public TEntity? LastOrDefault(Expression<Func<TEntity, object>> sort)
            => context.Set<TEntity>().OrderByDescending(sort).FirstOrDefault();
        public TEntity? FirstOrDefault(Expression<Func<TEntity, object>> sort)
            => context.Set<TEntity>().OrderBy(sort).FirstOrDefault();

        public void Add(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Added;
            context.SaveChanges();
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            context.AddRange(entities);
            context.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            context.UpdateRange(entities);
            context.SaveChanges();
        }
        public void Delete(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Deleted;
            context.SaveChanges();
        }
        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            context.RemoveRange(entities);
            context.SaveChanges();
        }

        public Task<Paginate<TEntity>> GetListPaginateAsync(Expression<Func<TEntity, bool>>? filter = null, int index = 0, int size = 10,
        EntitySortModel<TEntity>? sort = null, bool? asNoTracking = false) => sort is null ? filter is null ?
             (asNoTracking is true ? context.Set<TEntity>().AsNoTracking() : context.Set<TEntity>()).ToPaginateAsync(index, size) :
             (asNoTracking is true ? context.Set<TEntity>().AsNoTracking().Where(filter) : context.Set<TEntity>()).ToPaginateAsync(index, size) :
             GetSortedData(sort, filter, asNoTracking).ToPaginateAsync(index, size);

        public Paginate<TEntity> GetListPaginate(Expression<Func<TEntity, bool>>? filter = null, int index = 0, int size = 10,
        EntitySortModel<TEntity>? sort = null, bool? asNoTracking = false) => sort is null ? filter is null ?
                    (asNoTracking is true ? context.Set<TEntity>().AsNoTracking() : context.Set<TEntity>()).ToPaginate(index, size) :
                    (asNoTracking is true ? context.Set<TEntity>().AsNoTracking().Where(filter) : context.Set<TEntity>()).ToPaginate(index, size) :
                    GetSortedData(sort, filter, asNoTracking).ToPaginate(index, size);

        private IQueryable<TEntity> GetSortedData(EntitySortModel<TEntity> sort, Expression<Func<TEntity, bool>>? filter = null, bool? asNoTracking = false) =>
            sort.sortType == SortType.Ascending ? GetAscending(sort, filter, asNoTracking) : GetDescending(sort, filter, asNoTracking);

        private IQueryable<TEntity> GetAscending(EntitySortModel<TEntity> sort, Expression<Func<TEntity, bool>>? filter = null, bool? asNoTracking = false) => filter is null ?
                     (asNoTracking is true ?
                     context.Set<TEntity>().AsNoTracking().OrderBy(sort.sort) : context.Set<TEntity>().OrderBy(sort.sort)) :
                     (asNoTracking is true ?
                     context.Set<TEntity>().AsNoTracking().Where(filter).OrderBy(sort.sort) : context.Set<TEntity>().Where(filter).OrderBy(sort.sort));

        private IQueryable<TEntity> GetDescending(EntitySortModel<TEntity> sort, Expression<Func<TEntity, bool>>? filter = null, bool? asNoTracking = false) => filter is null ?
                (asNoTracking is true ?
                context.Set<TEntity>().AsNoTracking().OrderByDescending(sort.sort) : context.Set<TEntity>().OrderByDescending(sort.sort)) :
                (asNoTracking is true ?
                context.Set<TEntity>().AsNoTracking().Where(filter).OrderByDescending(sort.sort) : context.Set<TEntity>().Where(filter).OrderByDescending(sort.sort));

        // async
        public Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, bool? asNoTracking = false, EntitySortModel<TEntity>? sort = null) =>
            GetAllQueryable(filter, asNoTracking, sort).ToListAsync();

        public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, bool? asNoTracking = false, EntitySortModel<TEntity>? sort = null) =>
            GetQueryable(filter, asNoTracking, sort).FirstOrDefaultAsync();

        public Task AddAsync(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Added;
            return context.SaveChangesAsync();
        }
        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await context.AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }

        public Task UpdateAsync(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            return context.SaveChangesAsync();
        }
        public Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            context.UpdateRange(entities);
            return context.SaveChangesAsync();
        }

        public Task DeleteAsync(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Deleted;
            return context.SaveChangesAsync();
        }

        public Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            context.RemoveRange(entities);
            return context.SaveChangesAsync();
        }

        private IQueryable<TEntity> GetAllQueryable(Expression<Func<TEntity, bool>>? filter = null, bool? asNoTracking = false, EntitySortModel<TEntity>? sort = null) => sort is null ? (filter == null ?
            (asNoTracking is true ? context.Set<TEntity>().AsNoTracking() : context.Set<TEntity>()) :
            (asNoTracking is true ? context.Set<TEntity>().AsNoTracking().Where(filter) : context.Set<TEntity>().Where(filter))) :
             GetSortedData(sort, filter, asNoTracking);

        private IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> filter, bool? asNoTracking = false, EntitySortModel<TEntity>? sort = null) => sort is null ?
            (asNoTracking is true ? context.Set<TEntity>().AsNoTracking().Where(filter) : context.Set<TEntity>().Where(filter)) : GetSortedData(sort, filter, asNoTracking);
    }
}
