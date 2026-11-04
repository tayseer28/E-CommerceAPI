using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Data;
using E_Commerce.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Repositories
{
    internal class GenericRepository<TEntity, TKey>(StoreDbContext dbContext) : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private readonly DbSet<TEntity> _set = dbContext.Set<TEntity>();
        public void Add(TEntity entity) => _set.Add(entity);


        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
            => await _set.ToListAsync(ct);

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specification, CancellationToken ct = default)
        {
            var query = SpecificationEvaluator.CreateQuery(_set.AsQueryable(), specification);
            return await query.ToListAsync(ct);
        }

        public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default)
            => await _set.FindAsync(id, ct); //search locally first then the database

        public async Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specification, CancellationToken ct = default)
        {
            var query = SpecificationEvaluator.CreateQuery(_set.AsQueryable(), specification);
            return await query.FirstOrDefaultAsync(ct);
        }

        public void Remove(TEntity entity) => _set.Remove(entity);

        public void Update(TEntity entity) => _set.Update(entity);
        //private IQueryable<TEntity> ApplySpecification(ISpecifications<TEntity, TKey> specification)
        //{
        //    IQueryable<TEntity> query = _set.AsQueryable();
        //    if (specification != null)
        //    {
        //        if (specification.Includes.Any())
        //        {
        //            foreach (var include in specification.Includes)
        //            {
        //                query = query.Include(include);
        //            }
        //        }
        //    }
        //    return query;
        //}
    }
}
