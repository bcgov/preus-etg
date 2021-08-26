using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq.Expressions;

namespace CJG.Core.Interfaces.Service
{
	public interface IService
	{
		void Commit();
		void CommitTransaction();

		IEnumerable<ValidationResult> Validate<TEntity>(TEntity entity) where TEntity : class;

		void Convert<TEntity>(object source, TEntity result, bool ignoreCase = false) where TEntity : class;
		void ConvertAndValidate<TEntity>(object source, TEntity result, ICollection<ValidationResult> validationResults, bool ignoreCase = false) where TEntity : class;
		TEntity Convert<TEntity>(object source, bool ignoreCase = false) where TEntity : class;
		TEntity ConvertAndValidate<TEntity>(object source, ICollection<ValidationResult> validationResults, bool ignoreCase = false) where TEntity : class;

		TEntity Find<TEntity>(params object[] keyValues) where TEntity : class;
		TEntity Get<TEntity>(params object[] keyValues) where TEntity : class;

		void Modify<TEntity>(TEntity entity) where TEntity : class;

		void Add<TEntity>(TEntity entity) where TEntity : class;

		void Remove<TEntity>(TEntity entity) where TEntity : class;

		TEntity Attach<TEntity>(TEntity entity, EntityState state = EntityState.Unchanged) where TEntity : class;

		P OriginalValue<T, P>(T model, Expression<Func<T, P>> property) where T : class;
	}
}
