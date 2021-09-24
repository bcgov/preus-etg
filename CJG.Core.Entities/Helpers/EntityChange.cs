using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CJG.Core.Entities.Helpers
{
	public class EntityChange
	{
		#region Variables
		private static readonly string[] valueNames = new[] { "Caption", "Title", "Name", "FileName" };
		private readonly DbContext _context;
		#endregion

		#region Properties
		/// <summary>
		/// get - The name of the entity that has been changed.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// get - The entry that has been changed.
		/// </summary>
		public DbEntityEntry Entry { get; }

		/// <summary>
		/// get - The type of the entity.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// get - The state of the entity.
		/// </summary>
		public EntityState State { get; }

		/// <summary>
		/// get - A collection of changes made to the entity.
		/// </summary>
		public Dictionary<string, IList<PropertyChange>> Changes { get; }

		/// <summary>
		/// get - Whether the entity has been changed.
		/// </summary>
		public bool IsChanged { get { return this.Changes.Any(); } }

		/// <summary>
		/// get - Whether the entity was added.
		/// </summary>
		public bool IsAdded { get { return this.State == EntityState.Added; } }

		/// <summary>
		/// get - Whether the entity was deleted.
		/// </summary>
		public bool IsDeleted { get { return this.State == EntityState.Deleted; } }

		/// <summary>
		/// get - The change performed on the specified property name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public IEnumerable<PropertyChange> this[string name]
		{
			get
			{
				var found = this.Changes.TryGetValue(name, out IList<PropertyChange> change);
				if (found) return change.ToArray();
				return new PropertyChange[0];
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of an EntityChange struct.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="entry"></param>
		/// <param name="ignorePropertyNames"></param>
		public EntityChange(DbContext context, DbEntityEntry entry, IEnumerable<string> ignorePropertyNames = null)
		{
			_context = context;
			var parent = FindParent(context, entry);
			this.Entry = entry;
			this.Type = (parent ?? entry).Entity.GetType().GetProxyType();
			this.Name = parent != null ? GenerateEntityName(parent, entry) : GenerateEntityName(entry);
			this.State = entry.State;
			this.Changes = new Dictionary<string, IList<PropertyChange>>();

			// No need to do more if it's being deleted.
			if (entry.State == EntityState.Deleted) return;

			if (ignorePropertyNames == null) ignorePropertyNames = new string[0];
			var type = entry.Entity.GetType().GetProxyType();
			var entityProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

			foreach (var property in entry.CurrentValues.PropertyNames.Where(p => !ignorePropertyNames.Contains(p)))
			{
				var propertyName = GetDisplayName(type.GetProperty(property));
				if (entry.State == EntityState.Added && entry.CurrentValues[property] == null) continue;
				// Test each property to see if it has changed.
				if (entry.State == EntityState.Added || !(entry.OriginalValues[property]?.Equals(entry.CurrentValues[property]) ?? entry.OriginalValues[property] == entry.CurrentValues[property]))
				{
					// Check if this property is a foreign key.
					var foreignProperty = entityProperties.FirstOrDefault(p => p.GetCustomAttribute<ForeignKeyAttribute>()?.Name == property);
					if (foreignProperty != null)
					{
						var currentObject = foreignProperty.GetValue(entry.Entity);
						if (currentObject != null)
						{
							if (entry.State == EntityState.Added)
							{
								var currentValue = GetDisplayName(currentObject) ?? GetDefaultText(currentObject) ?? GetDefaultText(currentObject, valueNames) ?? entry.CurrentValues[property];
								this.AddChange(GetDisplayName(foreignProperty), null, currentValue);
							}
							else
							{
								var originalObject = GetEntity(context, currentObject.GetType(), entry.OriginalValues[property]);
								var originalValue = GetDisplayName(originalObject) ?? GetDefaultText(originalObject) ?? GetDefaultText(originalObject, valueNames) ?? entry.OriginalValues[property];
								var currentValue = GetDisplayName(currentObject) ?? GetDefaultText(currentObject) ?? GetDefaultText(currentObject, valueNames) ?? entry.CurrentValues[property];

								this.AddChange(GetDisplayName(foreignProperty), originalValue, currentValue);
							}
						}
					}
					else if ((type == typeof(Document) || type == typeof(VersionedDocument)) && property == "Body")
					{
						this.AddChange(propertyName, "...", "...");
					}
					else if ((type == typeof(Attachment) || type == typeof(VersionedAttachment)) && property == "AttachmentData")
					{
						var originalKB = entry.State != EntityState.Added && entry.OriginalValues[property] != null ? ((byte[])entry.OriginalValues[property]).Length / 1024 : 0;
						var currentKB = entry.CurrentValues[property] != null ? ((byte[])entry.CurrentValues[property]).Length / 1024 : 0;
						this.AddChange(propertyName, $"{originalKB:##.##} KB", $"{currentKB:##.##} KB");
					}
					else
					{
						this.AddChange(propertyName, entry.State == EntityState.Added ? null : entry.OriginalValues[property], entry.CurrentValues[property]);
					}
				}
			}
		}

		/// <summary>
		/// Creates a new instance of an EntityChange struct.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="entry"></param>
		public EntityChange(DbContext context, DbEntityEntry entry)
		{
			var parent = FindParent(context, entry);
			this.Entry = entry;
			this.Type = (parent ?? entry).Entity.GetType().GetProxyType();
			this.Name = parent != null ? GenerateEntityName(parent, entry) : GenerateEntityName(entry);
			this.State = entry.State;
			this.Changes = new Dictionary<string, IList<PropertyChange>>();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the property name, but check if there is a DisplayNameAttribute first.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		private static string GetDisplayName(PropertyInfo property)
		{
			var attr = property.GetCustomAttribute<DisplayNameAttribute>();
			return attr != null ? attr.DisplayName : property.Name;
		}

		/// <summary>
		/// Find the parent entity of the child.
		/// This is useful to help describe a change and associate it with the parent entity.
		/// Without this an ApplicationAddress doesn't have any related data.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="child"></param>
		/// <returns></returns>
		private static DbEntityEntry FindParent(DbContext context, DbEntityEntry child)
		{
			var childType = child.Entity.GetType().GetProxyType();
			foreach (var entry in context.ChangeTracker.Entries().Where(e => (new[] { EntityState.Modified, EntityState.Added, EntityState.Deleted }).Contains(e.State)))
			{
				var entryType = entry.Entity.GetType().GetProxyType();
				if (childType == typeof(ApplicationAddress))
				{
					var address = child.Entity as ApplicationAddress;
					if (entryType == typeof(TrainingProvider))
					{
						var trainingProvider = entry.Entity as TrainingProvider;
						if (trainingProvider.TrainingAddressId == address.Id || trainingProvider.TrainingAddress == address)
						     { return entry; }
						else if (trainingProvider.TrainingProviderAddressId == address.Id || trainingProvider.TrainingProviderAddress == address)
						         { return entry; }

					}
					
					else if (entryType == typeof(GrantApplication))
					{
						var grantApplication = entry.Entity as GrantApplication;
						if (grantApplication.ApplicantPhysicalAddressId == address.Id || grantApplication.ApplicantPhysicalAddress == address) return entry;
						else if (grantApplication.ApplicantMailingAddressId == address.Id || grantApplication.ApplicantMailingAddress == address) return entry;
						else if (grantApplication.OrganizationAddressId == address.Id || grantApplication.OrganizationAddress == address) return entry;
					}
				}
				else if (childType == typeof(Attachment))
				{
					var attachment = child.Entity as Attachment;
					if (entryType == typeof(TrainingProvider))
					{
						var trainingProvider = entry.Entity as TrainingProvider;
						if (trainingProvider.CourseOutlineDocumentId == attachment.Id
							|| trainingProvider.BusinessCaseDocumentId == attachment.Id
							|| trainingProvider.ProofOfQualificationsDocumentId == attachment.Id) return entry;
					}
					else if (entryType == typeof(Claim))
					{
						var claim = entry.Entity as Claim;
						if (claim.Receipts.Where(r => r.Id == attachment.Id).Count() > 0)
						{
							return entry;
						}
					}
				}
			}

			return null;
		}

		private static T GetEntity<T>(DbContext context, params object[] key)
		{
			return (T)GetEntity(context, typeof(T), key);
		}

		private static object GetEntity(DbContext context, Type type, params object[] key)
		{
			var entityType = type.GetProxyType();
			var setMethod = context.GetType().GetMethods().Where(m => m.Name == "Set" && m.GetGenericArguments().Any()).FirstOrDefault();
			var gSetMethod = setMethod.MakeGenericMethod(entityType);
			var dbSet = gSetMethod.Invoke(context, null);
			var findMethod = dbSet.GetType().GetMethod("Find");
			return key != null ? findMethod.Invoke(dbSet, new object[] { key }) : null;
		}

		/// <summary>
		/// Get the display name of the entity object.
		/// This will either be the DisplayNameAttribute, or a function called GetDisplayName() on the entity object.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		private static string GetDisplayName(object entity)
		{
			if (entity == null) return null;
			var entityType = entity.GetType().GetProxyType();
			var getDisplayNameMethod = entityType.GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(m => m.Name == "GetDisplayName" && !m.GetParameters().Any());
			var attr = entityType.GetCustomAttribute<DisplayNameAttribute>();
			return getDisplayNameMethod != null ? (string)getDisplayNameMethod.Invoke(entity, null) : attr?.DisplayName;
		}

		/// <summary>
		/// Get the default text for the entity object.
		/// This will call the ToString() function on the entity object.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		private static string GetDefaultText(object entity)
		{
			if (entity == null) return null;
			var entityType = entity.GetType().GetProxyType();
			var toStringMethod = entityType.GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(m => m.Name == "ToString" && !m.GetParameters().Any() && m.DeclaringType == entityType);
			return toStringMethod != null ? (string)toStringMethod.Invoke(entity, null) : null;
		}

		/// <summary>
		/// Get the value of the property on the specified entity object.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="propertyNames"></param>
		/// <returns></returns>
		private static string GetDefaultText(object entity, string[] propertyNames)
		{
			if (entity == null) return null;
			var entityType = entity.GetType().GetProxyType();
			var textProperty = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(p => propertyNames.Contains(p.Name));
			return textProperty != null ? (string)textProperty?.GetValue(entity) : null;
		}

		/// <summary>
		/// Generate a name to identify this entity.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="child"></param>
		/// <returns></returns>
		private string GenerateEntityName(DbEntityEntry entry, DbEntityEntry child = null)
		{
			string description = null;

			if (entry.State == EntityState.Deleted)
			{
				if (this.Type == typeof(EligibleCost))
				{
					var expenseType = GetEntity<EligibleExpenseType>(_context, entry.OriginalValues[nameof(EligibleCost.EligibleExpenseTypeId)]);
					description = $" - {expenseType.Caption}";
				}
				else if (this.Type == typeof(EligibleCostBreakdown))
				{
					var cost = GetEntity<EligibleCost>(_context, entry.OriginalValues[nameof(EligibleCostBreakdown.EligibleCostId)]);
					description = $" - {cost.EligibleExpenseType.Caption}";
				}
				else if (this.Type == typeof(TrainingProvider))
				{
					description = $" - {((TrainingProvider)entry.Entity).Name}";
				}
				else if (this.Type == typeof(TrainingProgram))
				{
					description = $" - {((TrainingProgram)entry.Entity).CourseTitle}";
				}
				else if (this.Type == typeof(Document))
				{
					var document = entry.Entity as Document;
					var template = GetEntity<DocumentTemplate>(_context, entry.OriginalValues[nameof(Document.DocumentTemplateId)]);
					description = $" - {template.DocumentType.ToString()} - {document.Title}";
				}
			}
			else
			{
				if (this.Type == typeof(EligibleCost))
				{
					description = $" - {((EligibleCost)entry.Entity).EligibleExpenseType.Caption}";
				}
				else if (this.Type == typeof(EligibleCostBreakdown))
				{
					description = $" - {((EligibleCostBreakdown)entry.Entity).EligibleCost.EligibleExpenseType.Caption}";
				}
				else if (this.Type == typeof(TrainingProvider))
				{
					var provider = entry.Entity as TrainingProvider;
					if (child != null)
					{
						var childType = child.Entity.GetType().GetProxyType();

						if (childType == typeof(ApplicationAddress))
						{
							description = $" - {SplitCamelCase(nameof(provider.TrainingAddress))}";
						}
						else if (childType == typeof(Attachment))
						{
							var attachment = child.Entity as Attachment;
							if (provider.CourseOutlineDocumentId == attachment.Id)
								description = $" - {SplitCamelCase(nameof(provider.CourseOutlineDocument))}";
							else if (provider.BusinessCaseDocumentId == attachment.Id)
								description = $" - {SplitCamelCase(nameof(provider.BusinessCaseDocument))}";
							else if (provider.ProofOfQualificationsDocumentId == attachment.Id)
								description = $" - {SplitCamelCase(nameof(provider.ProofOfQualificationsDocument))}";
						}
						else
						{
							description = $" - {SplitCamelCase(GetDisplayName(child.Entity) ?? childType.Name)}";
						}
					}
					else
					{
						description = $" - {((TrainingProvider)entry.Entity).Name}";
					}
				}
				else if (this.Type == typeof(TrainingProgram))
				{
					description = $" - {((TrainingProgram)entry.Entity).CourseTitle}";
				}
				else if (this.Type == typeof(Document))
				{
					var document = entry.Entity as Document;
					description = $" - {document.DocumentTemplate.DocumentType.ToString()} - {document.Title}";
				}
				else if (this.Type == typeof(GrantApplication) && child != null)
				{
					var childType = child.Entity.GetType().GetProxyType();
					if (childType == typeof(ApplicationAddress))
					{
						var address = child.Entity as ApplicationAddress;
						var application = entry.Entity as GrantApplication;
						if (application.ApplicantPhysicalAddress == address)
						{
							return $"{SplitCamelCase(nameof(GrantApplication.ApplicantPhysicalAddress))}";
						}
						else if (application.ApplicantMailingAddress == address)
						{
							return $"{SplitCamelCase(nameof(GrantApplication.ApplicantMailingAddress))}";
						}
						else if (application.OrganizationAddress == address)
						{
							return $"{SplitCamelCase(nameof(GrantApplication.OrganizationAddress))}";
						}
					}
					else
					{
						description = $" - {SplitCamelCase(GetDisplayName(child.Entity) ?? childType.Name)}";
					}
				}
			}

			return $"{SplitCamelCase(GetDisplayName(entry.Entity) ?? this.Type.Name)}{description}";
		}

		/// <summary>
		/// Add a change to the specified property name.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		/// <param name="state"></param>
		public void AddChange(string propertyName, object oldValue, object newValue, EntityState state = EntityState.Modified)
		{
			var found = this.Changes.TryGetValue(propertyName, out IList<PropertyChange> changes);
			if (!found) this.Changes.Add(propertyName, new List<PropertyChange>(new[] { new PropertyChange(propertyName, oldValue, newValue, state) }));
			else changes.Add(new PropertyChange(propertyName, oldValue, newValue, state));
		}

		/// <summary>
		/// Add a change for the specified entry.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="state"></param>
		public void AddChange(DbEntityEntry entry, EntityState state = EntityState.Modified)
		{
			var type = ObjectContext.GetObjectType(entry.Entity.GetType());
			var propertyName = type.Name;
			var valueProperty = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(p => valueNames.Contains(p.Name));
			var value = valueProperty.GetValue(entry.Entity);

			var found = this.Changes.TryGetValue(propertyName, out IList<PropertyChange> changes);
			if (!found) this.Changes.Add(propertyName, new List<PropertyChange>(new[] { new PropertyChange(propertyName, null, value, state) }));
			else changes.Add(new PropertyChange(propertyName, null, value, state));
		}

		/// <summary>
		/// Determine if the property has a change to it.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public bool HasChanged(string propertyName)
		{
			return this.Changes.ContainsKey(propertyName);
		}

		/// <summary>
		/// Convert this entity change into JSON.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var message = new StringBuilder();
			var state = this.State == EntityState.Modified ? "modified" : this.State == EntityState.Added ? "added" : "deleted";
			message.Append($"{{ \"name\": \"{this.Name}\", \"state\": \"{state}\", \"changes\": [");
			var next = false;
			foreach (var changes in this.Changes)
			{
				foreach (var change in changes.Value)
				{
					message.Append($"{(next ? "," : "")}{change.ToString()}");
					next = true;
				}
			}
			message.Append("] }");
			return message.ToString();
		}

		/// <summary>
		/// Split the name into separate words.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static string SplitCamelCase(string value)
		{
			if (String.IsNullOrWhiteSpace(value)) return value;
			return Regex.Replace(value, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
		}
		#endregion
	}
}
