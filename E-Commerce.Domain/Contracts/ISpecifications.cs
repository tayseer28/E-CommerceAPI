using E_Commerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Contracts
{
    public interface ISpecifications<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        ICollection<Expression<Func<TEntity, object>>> Includes { get; }
        Expression<Func<TEntity, bool>>? Criteria { get; }

        // sorting Expression
        Expression<Func<TEntity, object>>? OrderBy { get; }
        Expression<Func<TEntity, object>>? OrderByDescending { get; }

        // pagination 
        public int Take { get; }
        public int Skip { get; }
        public bool IsPagingEnabled { get; }
    }
}
