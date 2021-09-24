using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace CJG.Core.Interfaces
{
    /// <summary>
    /// <typeparamref name="DbSetExtensions"/> static class, provides extension methods for <typeparamref name="DbSet"/> types.
    /// </summary>
    public static class DbSetExtensions
    {
        public static IQueryable<TEntity> Includes<TEntity, TProperty>(this DbSet<TEntity> set, params Expression<Func<TEntity, TProperty>>[] paths) where TEntity : class
        {
            var query = set.AsQueryable();
            foreach (var path in paths)
            {
                query = query.Include(path);
            }

            return query;
        }

        public static IQueryable<TEntity> Includes<TEntity>(this DbSet<TEntity> set, params string[] paths) where TEntity : class
        {
            var query = set.AsQueryable();
            foreach (var path in paths)
            {
                query = query.Include(path);
            }

            return query;
        }
	}
}
