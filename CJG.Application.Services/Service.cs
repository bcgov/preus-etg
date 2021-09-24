using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="Service"/> abstract class, provides a base implementation for services.
	/// </summary>
	public abstract class Service : IService
	{
		#region Properties
		protected readonly ILogger _logger;
		protected readonly IDataContext _dbContext;
		protected readonly HttpContextBase _httpContext;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="Service"/> class.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public Service(IDataContext context, HttpContextBase httpContext, ILogger logger)
		{
			_dbContext = context;
			_dbContext.HttpContext = httpContext; // This is a hack to pass the current HttpContext to the CJGContext so that it's available during validation.
			_httpContext = httpContext;
			_logger = logger;
		}
		#endregion

		#region Methods
		//TODO This might need to be introduced from somewhere other than here. The Service layer shouldn't contain Entity specific info.
		public int GetDefaultGrantProgramId()
		{
			var defaultGrantProgram = _dbContext.GrantPrograms
				.FirstOrDefault(gp => gp.ProgramCode == "ETG");

			return defaultGrantProgram?.Id ?? 0;
		}

		/// <summary>
		/// Commit the unit of work to the datasource.
		/// </summary>
		public void Commit()
		{
			_dbContext.Commit();
		}

		/// <summary>
		/// Commit the unit of work in a transaction to the datasource.
		/// </summary>
		public void CommitTransaction()
		{
			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Call the specified 'update' function and then save as a single transaction.
		/// </summary>
		/// <param name="update">Function to call before saving changes to DB.</param>
		/// <returns></returns>
		public int CommitTransaction(Func<int> update)
		{
			using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.Serializable)) {
				try {
					var result = update();
					_dbContext.SaveChanges();
					transaction.Commit();
					return result;
				} catch {
					transaction.Rollback();
					throw;
				}
			}
		}

		/// <summary>
		/// Get a collection of validation results for the specified entity.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		public IEnumerable<ValidationResult> Validate<TEntity>(TEntity entity) where TEntity : class
		{
			return _dbContext.Validate(entity);
		}

		/// <summary>
		/// Convert one object into the specified result type. Updates the collection of validation results with any validation errors.
		/// Use this to update the resulting object.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="source"></param>
		/// <param name="result"></param>
		/// <param name="validationResults"></param>
		/// <param name="ignoreCase"></param>
		public void ConvertAndValidate<TEntity>(object source, TEntity result, ICollection<ValidationResult> validationResults, bool ignoreCase = false) where TEntity : class
		{
			_dbContext.ConvertAndValidate(source, result, validationResults, ignoreCase);
		}

		/// <summary>
		/// Convert one object into the specified result type.  Updates the collection of validation results with any validation errors.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="source"></param>
		/// <param name="validationResults"></param>
		/// <param name="ignoreCase"></param>
		/// <returns>Creates a new instance of a <typeparamref name="TEntity"/> object.</returns>
		public TEntity ConvertAndValidate<TEntity>(object source, ICollection<ValidationResult> validationResults, bool ignoreCase = false) where TEntity : class
		{
			return _dbContext.ConvertAndValidate<TEntity>(source, validationResults, ignoreCase);
		}

		/// <summary>
		/// Convert one object into the specified result type.
		/// Use this to update the resulting object.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="source"></param>
		/// <param name="result"></param>
		/// <param name="ignoreCase"></param>
		public void Convert<TEntity>(object source, TEntity result, bool ignoreCase = false) where TEntity : class
		{
			_dbContext.Convert(source, result, ignoreCase);
		}

		/// <summary>
		/// Convert one object into the specified result type.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="source"></param>
		/// <param name="ignoreCase"></param>
		/// <returns>Creates a new instance of a <typeparamref name="TEntity"/> object.</returns>
		public TEntity Convert<TEntity>(object source, bool ignoreCase = false) where TEntity : class
		{
			return _dbContext.Convert<TEntity>(source, ignoreCase);
		}

		/// <summary>
		/// Find the entity for the specified key(s).
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="keyValues"></param>
		/// <returns></returns>
		public TEntity Find<TEntity>(params object[] keyValues) where TEntity : class
		{
			return _dbContext.Set<TEntity>().Find(keyValues);
		}

		/// <summary>
		/// Get the entity for the specified key(s) and throw NoContentException if it doesnt exist.
		/// </summary>
		/// <exception cref="NoContentException">If the entity does not exist</exception>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="keyValues"></param>
		/// <returns></returns>
		public TEntity Get<TEntity>(params object[] keyValues) where TEntity : class
		{
			var entityName = typeof(TEntity).Name;
			return _dbContext.Set<TEntity>().Find(keyValues) ?? throw new NoContentException($"Unable to find the {entityName} for the specified id {String.Join(":", keyValues)}.");
		}

		/// <summary>
		/// Set the entity state to modified.  It requires a Commit to apply to the datasource.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		public void Modify<TEntity>(TEntity entity) where TEntity : class
		{
			var entry = _dbContext.Entry(entity);
			entry.State = EntityState.Modified;
			UpdateRowVersion(entry);
		}

		/// <summary>
		/// Add the entity from the in-memory collection but not from the datasource.  It requires a Commit to apply to the datasource.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		public void Add<TEntity>(TEntity entity) where TEntity : class
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));
			_dbContext.Set<TEntity>().Add(entity);
		}

		/// <summary>
		/// Removes the entity from the in-memory collection but not from the datasource.  It requires a Commit to apply to the datasource.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		public void Remove<TEntity>(TEntity entity) where TEntity : class
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));
			_dbContext.Set<TEntity>().Remove(entity);
		}

		/// <summary>
		/// Attach an unattached entity object to the DbContext.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		public TEntity Attach<TEntity>(TEntity entity, EntityState state = EntityState.Unchanged) where TEntity : class
		{
			var current = _dbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(e => e.Entity.GetKeyValues().SequenceEqual(entity.GetKeyValues()));
			if (current == null)
			{
				var entry = _dbContext.Entry<TEntity>(entity);
				if (entry.State == EntityState.Detached)
				{
					_dbContext.Set<TEntity>().Attach(entity);
				}
				return entity;
			}
			else
			{
				current.CurrentValues.SetValues(entity);
				current.State = state;
				return current.Entity;
			}
		}

		/// <summary>
		/// Compile and generate output with the Razor engine.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="model"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		public string ParseDocumentTemplate<T>(T model, string content)
		{
			return Engine.Razor.RunCompile(content, System.Guid.NewGuid().ToString(), null, model);
		}

		/// <summary>
		/// We have to manually do this because we are manually setting the State.
		/// This will ensure optimistic concurrency is working.
		/// </summary>
		/// <param name="entry"></param>
		private void UpdateRowVersion(DbEntityEntry entry)
		{
			// Only entries that have been modified need to be handled.
			if (entry.State == EntityState.Modified) {
				var obj = entry.Entity as EntityBase;
				entry.OriginalValues[nameof(EntityBase.RowVersion)] = obj.RowVersion;
			}
		}

		/// <summary>
		/// Get the original value of the specified entity from the datasource.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="P"></typeparam>
		/// <param name="model"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public P OriginalValue<T, P>(T model, Expression<Func<T, P>> property)
			where T : class
		{
			var expression = (MemberExpression)property.Body;
			return (P)_dbContext.Entry(model).OriginalValues[expression.Member.Name];
		}
		#endregion
	}
}
