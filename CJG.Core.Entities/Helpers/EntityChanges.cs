using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CJG.Core.Entities.Helpers
{
	public class EntityChanges : IEnumerable<EntityChange>
	{
		#region Variables
		private static readonly string[] valueNames = new[] { "Caption", "Title", "Name", "FileName" };
		private readonly IDictionary<Type, IList<EntityChange>> _types = new Dictionary<Type, IList<EntityChange>>();
		#endregion

		#region Properties
		public bool IsChanged { get { return _types.Any(); } }
		public bool IsModified { get { return _types.Any(t => t.Value.Any(c => c.State == EntityState.Modified)); } }
		public bool IsAdded { get { return _types.Any(t => t.Value.Any(c => c.State == EntityState.Added)); } }
		public bool IsDeleted { get { return _types.Any(t => t.Value.Any(c => c.State == EntityState.Deleted)); } }

		public IEnumerable<EntityChange> this[Type type]
		{
			get
			{
				var entityType = type.GetProxyType();
				var found = _types.TryGetValue(entityType, out IList<EntityChange> changes);
				if (found) return changes.ToArray();
				return new EntityChange[0];
			}
		}

		public EntityChange this[object entity]
		{
			get
			{
				var entityType = entity.GetType().GetProxyType();
				var found = _types.TryGetValue(entityType, out IList<EntityChange> changes);
				if (found) return changes.FirstOrDefault(c => c.Entry.Entity.Equals(entity));
				return null;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of an EntityChanges object and initializes it with the specified DbContext changes.
		/// </summary>
		/// <param name="context"></param>
		public EntityChanges(DbContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			if (context.ChangeTracker.HasChanges())
			{
				var ignore = new[] { "Id", nameof(EntityBase.DateUpdated), nameof(EntityBase.DateAdded), nameof(EntityBase.RowVersion) };
				// Get all the updated entries and update their DateAdded or DateUpdated based on their state.
				foreach (var entry in context.ChangeTracker.Entries().Where(e => (new[] { EntityState.Modified, EntityState.Added, EntityState.Deleted }).Contains(e.State)))
				{
					var track = new EntityChange(context, entry, ignore); //GetDifferences(context, entry, ignore);
					if (track.IsChanged || track.IsAdded || track.IsDeleted) Add(track);
				}

				// Many-to-many relationships are not expressed in the change tracker.
				foreach (var relationship in context.GetAddedRelationships())
				{
					var type = relationship.Item1.GetType().GetProxyType();
					var entry = context.Entry(relationship.Item1);
					var found = _types.TryGetValue(type, out IList<EntityChange> entities);
					if (found)
					{
						var track = entities.FirstOrDefault(e => e.State == EntityState.Added) ?? entities.First();
						track.AddChange(context.Entry(relationship.Item2), EntityState.Added);
					}
					else
					{
						var track = new EntityChange(context, entry);
						track.AddChange(context.Entry(relationship.Item2), EntityState.Added);
						Add(track);
					}
				}

				// Many-to-many relationships are not expressed in the change tracker.
				foreach (var relationship in context.GetDeletedRelationships())
				{
					var type = relationship.Item1.GetType().GetProxyType();
					var entry = context.Entry(relationship.Item1);
					var found = _types.TryGetValue(type, out IList<EntityChange> entities);

					var changeEntry = context.Entry(relationship.Item2);
					var changeType = ObjectContext.GetObjectType(changeEntry.Entity.GetType());
					var propertyName = changeType.Name;
					var valueProperty = changeType.GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(p => valueNames.Contains(p.Name));
					var value = valueProperty.GetValue(changeEntry.Entity);

					if (found)
					{
						var track = entities.FirstOrDefault(e => e.State == EntityState.Deleted) ?? entities.First();
						track.AddChange(propertyName, value, null, EntityState.Deleted);
					}
					else
					{
						var track = new EntityChange(context, entry);
						track.AddChange(propertyName, value, null, EntityState.Deleted);
						Add(track);
					}
				}

				// Many-to-many relationships are not expressed in the change tracker.
				foreach (var relationship in context.GetModifiedRelationships())
				{
					var type = relationship.Item1.GetType().GetProxyType();
					var entry = context.Entry(relationship.Item1);
					var found = _types.TryGetValue(type, out IList<EntityChange> entities);
					if (found)
					{
						var track = entities.FirstOrDefault(e => e.State == EntityState.Modified) ?? entities.First();
						track.AddChange(context.Entry(relationship.Item2));
					}
					else
					{
						var track = new EntityChange(context, entry);
						track.AddChange(context.Entry(relationship.Item2));
						Add(track);
					}
				}
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Add the entity change object to the collection.
		/// This will group changes by the type.
		/// </summary>
		/// <param name="entity"></param>
		public void Add(EntityChange entity)
		{
			var entityType = entity.Type;
			var found = _types.TryGetValue(entityType, out IList<EntityChange> entities);
			if (!found)
			{
				entities = new List<EntityChange>();
				entities.Add(entity);
				_types.Add(entityType, entities);
			}
			else
			{
				entities.Add(entity);
			}
		}

		/// <summary>
		/// Add the entity change object to the collection.
		/// This will group changes by the type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="entity"></param>
		public void Add(Type type, EntityChange entity)
		{
			var entityType = type.GetProxyType();
			var found = _types.TryGetValue(entityType, out IList<EntityChange> entities);
			if (!found)
			{
				entities = new List<EntityChange>();
				entities.Add(entity);
				_types.Add(entityType, entities);
			}
			else
			{
				entities.Add(entity);
			}
		}

		/// <summary>
		/// Determine if a change has been made to the specified type and properties.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="propertyNames"></param>
		/// <returns></returns>
		public bool HasChanged(Type type, params string[] propertyNames)
		{
			return this[type].Any(t => propertyNames.Any(p => t.HasChanged(p)));
		}

		/// <summary>
		/// Determine if a change has been made to the specified entity and properties.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="propertyNames"></param>
		/// <returns></returns>
		public bool HasChanged(Object entity, params string[] propertyNames)
		{
			var changes = this[entity];
			if (changes == null) return false;
			return propertyNames.Any(p => changes.HasChanged(p));
		}

		public bool HasChanged(Type type)
		{
			return this[type].Any();
		}

		public bool HasAdded(Type type)
		{
			return this[type].Any(t => t.State == EntityState.Added);
		}

		public bool HasDeleted(Type type)
		{
			return this[type].Any(t => t.State == EntityState.Deleted);
		}

		public bool HasModified(Type type)
		{
			return this[type].Any(t => t.State == EntityState.Modified);
		}

		public bool HasModified(object entity)
		{
			var result = this[entity];
			if (result != null) return this[entity].State == EntityState.Modified;
			return false;
		}

		public IEnumerator<EntityChange> GetEnumerator()
		{
			return _types.SelectMany(t => t.Value).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _types.SelectMany(t => t.Value).GetEnumerator();
		}

		/// <summary>
		/// Convert the entity changes to JSON.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var message = new StringBuilder();
			message.Append("[");
			var next = false;
			foreach (var entity in this)
			{
				message.Append($"{(next ? ",": "")}{entity.ToString()}");
				next = true;
			}
			message.Append("]");
			return message.ToString();
		}
		#endregion
	}
}
