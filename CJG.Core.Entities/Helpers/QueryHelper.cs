using CJG.Core.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CJG.Core.Entities.Helpers
{
	/// <summary>
	/// QueryHelper static class, provides extension methods for IQueryable<T>.
	/// </summary>
	public static class QueryHelper
	{
		#region Variables
		private static readonly string[] _orderBy = new[] { "asc", "ascending" };
		private static readonly string[] _orderByDescending = new[] { "desc", "descending" };
		private static readonly MethodInfo OrderByMethod = typeof(Queryable).GetMethods().Single(method => method.Name == "OrderBy" && method.GetParameters().Length == 2);
		private static readonly MethodInfo OrderByDescendingMethod = typeof(Queryable).GetMethods().Single(method => method.Name == "OrderByDescending" && method.GetParameters().Length == 2);
		private static MethodInfo GeneratePropertyPathLambdaMethod = typeof(QueryHelper).GetMethod("GeneratePropertyPathLambda", BindingFlags.NonPublic | BindingFlags.Static);
		#endregion

		/// <summary>
		/// Check if the specified property exists in the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		private static bool PropertyExists<T>(string propertyName)
		{
			return typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
		}

		/// <summary>
		/// Order the query results by the specified property names.
		/// Each property can also specify the direction of the sort (i.e. "Name asc", "Name ascending", "Name desc", "Name descending").
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static IQueryable<T> OrderByProperty<T>(this IQueryable<T> source, params string[] propertyName)
		{
			if (propertyName == null) return source;
			var query = source;
			foreach (var prop in propertyName)
			{
				var parts = prop?.Split(' ') ?? throw new ArgumentNullException(nameof(propertyName));

				if (parts.Length > 2) throw new ArgumentOutOfRangeException(nameof(propertyName), "Argument 'propertyName' must not have more than two parts (i.e. 'Name asc' or 'Name desc')");
				if (parts.Length == 2 && _orderByDescending.Contains(parts[1].ToLower()))
				{
					query = query.OrderByPropertyDescending(parts[0]);
				}
				else
				{
					var rType = GetPropertyPathType<T>(parts[0]);
					var convertMethod = GeneratePropertyPathLambdaMethod.MakeGenericMethod(new[] { typeof(T), rType });
					var orderExpression = convertMethod.Invoke(null, new[] { parts[0] });
					if (orderExpression != null)
					{
						var genericMethod = OrderByMethod.MakeGenericMethod(typeof(T), rType);
						query = (IQueryable<T>)genericMethod.Invoke(null, new[] { query, orderExpression });
					}
					else
					{
						if (!PropertyExists<T>(parts[0])) continue;
						ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
						Expression orderByProperty = Expression.Property(paramterExpression, parts[0]);
						LambdaExpression lambda = Expression.Lambda(orderByProperty, paramterExpression);
						MethodInfo genericMethod = OrderByMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
						query = (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, lambda });
					}
				}

			}
			return query;
		}

		/// <summary>
		/// Order the query results by the specified property names.
		/// Any property name that doesn't exist will be ignored.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static IQueryable<T> OrderByPropertyDescending<T>(this IQueryable<T> source, params string[] propertyName)
		{
			if (propertyName == null) return source;
			var query = source;
			foreach (var prop in propertyName)
			{
				var rType = GetPropertyPathType<T>(prop);
				var convertMethod = GeneratePropertyPathLambdaMethod.MakeGenericMethod(new[] { typeof(T), rType });
				var orderExpression = convertMethod.Invoke(null, new[] { prop });
				if (orderExpression != null)
				{
					var genericMethod = OrderByDescendingMethod.MakeGenericMethod(typeof(T), rType);
					query = (IQueryable<T>)genericMethod.Invoke(null, new[] { query, orderExpression });
				}
				else
				{
					if (!PropertyExists<T>(prop)) return query;
					ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
					Expression orderByProperty = Expression.Property(paramterExpression, prop);
					LambdaExpression lambda = Expression.Lambda(orderByProperty, paramterExpression);
					MethodInfo genericMethod = OrderByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
					query = (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, lambda });
				}
			}
			return query;
		}

		private static Type GetPropertyPathType<T>(string path)
		{
			var type = typeof(T);
			foreach(var part in path.Split('.'))
			{
				var prop = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(p => p.Name == part) ?? throw new ArgumentException("The argument 'path' contains in invalid property name.", nameof(path));
				type = prop.PropertyType.IsEnumerable() ? prop.PropertyType.GetItemType() : prop.PropertyType;
			}

			return type;
		}

		private static LambdaExpression MakeSelector(Type objectType, string path)
		{
			var parameter = Expression.Parameter(objectType, "x");
			var body = path.Split('.').Aggregate((Expression)parameter, Expression.PropertyOrField);
			return Expression.Lambda(body, parameter);
		}

		private static Expression<Func<T, RT>> GeneratePropertyPathLambda<T, RT>(string path)
			where T : class
		{
			if (!path.Contains('.')) return null;

			var parameter = Expression.Parameter(typeof(T), "x");
			return Expression.Lambda<Func<T, RT>>(path.Split('.').Aggregate((Expression)parameter, Expression.PropertyOrField), parameter);
		}
	}
}
