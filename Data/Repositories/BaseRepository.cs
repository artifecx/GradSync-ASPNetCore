using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class BaseRepository
    {
        protected IUnitOfWork UnitOfWork { get; set; }

        protected GradSyncDbContext Context => (GradSyncDbContext)UnitOfWork.Database;

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));
            UnitOfWork = unitOfWork;
        }

        protected virtual DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        {
            return Context.Set<TEntity>();
        }

        protected virtual void SetEntityState(object entity, EntityState entityState)
        {
            Context.Entry(entity).State = entityState;
        }

        protected virtual IQueryable<T> ApplySpecification<T>(ISpecification<T> spec) where T : class
        {
            var query = Context.Set<T>().AsQueryable();

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            if (spec.OrderExpressions != null && spec.OrderExpressions.Any())
            {
                var firstOrderExpr = spec.OrderExpressions[0];
                if (firstOrderExpr.IsDescending)
                {
                    query = query.OrderByDescending(firstOrderExpr.Expression);
                }
                else
                {
                    query = query.OrderBy(firstOrderExpr.Expression);
                }

                foreach (var orderExpr in spec.OrderExpressions.Skip(1))
                {
                    if (orderExpr.IsDescending)
                    {
                        query = ((IOrderedQueryable<T>)query).ThenByDescending(orderExpr.Expression);
                    }
                    else
                    {
                        query = ((IOrderedQueryable<T>)query).ThenBy(orderExpr.Expression);
                    }
                }
            }

            if (spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            return query;
        }

        public async Task<List<T>> ListAsync<T>(ISpecification<T> spec) where T : class
        {
            var query = ApplySpecification(spec);
            return await query.ToListAsync();
        }

        public async Task<T> GetEntityWithSpecAsync<T>(ISpecification<T> spec) where T : class
        {
            var query = ApplySpecification(spec);
            return await query.FirstOrDefaultAsync();
        }
    }
}
