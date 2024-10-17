using CJG.Core.Entities;
using CJG.Core.Entities.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;

namespace CJG.Infrastructure.EF
{
    /// <summary>
    /// <typeparamref name="DbContextExtensions"/> static class, provides extension methods for <typeparamref name="DbContext"/> object.
    /// </summary>
    internal static class DbContextExtensions
    {
        private readonly static ConcurrentDictionary<KeyValuePair<Type, string>, MethodInfo> _methods = new ConcurrentDictionary<KeyValuePair<Type, string>, MethodInfo>();

        /// <summary>
        /// Creates a new instance of <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TEntity CreateInstance<TEntity>(object source)
        {
            TEntity result;
            var result_type = typeof(TEntity);
            if (result_type.IsClass && result_type != typeof(string))
            {
                result = Activator.CreateInstance<TEntity>();
            }
            else if (result_type.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                var list_item_type = result_type.GetGenericArguments()[0];
                var list_type = typeof(List<>);
                var con = list_type.MakeGenericType(list_item_type);
                result = (TEntity)Activator.CreateInstance(con);
            }
            else
            {
                result = (TEntity)System.Convert.ChangeType(source, result_type);
            }
            return result;
        }

        /// <summary>
        /// Validates the specified entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IEnumerable<ValidationResult> Validate<TEntity>(this DbContext context, TEntity entity, string memberName = null) where TEntity : class
        {
            var vEntity = entity as IValidatableObject;
            if (vEntity == null)
                return Enumerable.Empty<ValidationResult>();

            var items = new Dictionary<object, object>
            {
                { "DbContext", new DataContext(context as CJGContext) }
            };
            DbEntityEntry entry = context.Entry(entity);
            items.Add("Entry", entry);
            var results = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(entity, items);
            // call the entity validation manually
            Validator.TryValidateObject(entity, validationContext, results, true);
            return results.Select(e => new ValidationResult(e.ErrorMessage, e.MemberNames));
        }


        /// <summary>
        /// Creates a new instance of <typeparamref name="TEntity"/> and populates it with the source property values.
        /// Updates the collection of <typeparamref name="ValidationResult"/> objects if there were validation errors.
        /// This method uses reflection and data annotation to map between the source and destination objects.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static TEntity Convert<TEntity>(this DbContext context, object source, bool ignoreCase = false) where TEntity : class
        {
            var result = CreateInstance<TEntity>(source);

            var validationResults = new List<ValidationResult>();

            context._Convert(source, result, validationResults, ignoreCase);

            return result;
        }

        /// <summary>
        /// Populates the result with property values from the source.
        /// This method uses reflection and data annotation to map between the source and destination objects.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="result"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static TEntity Convert<TEntity>(this DbContext context, object source, TEntity result, bool ignoreCase = false) where TEntity : class
        {
            var validationResults = new List<ValidationResult>();

            context._Convert(source, result, validationResults, ignoreCase);

            return result;
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="TEntity"/> and populates it with the source property values.
        /// Updates the collection of <typeparamref name="ValidationResult"/> objects if there were validation errors.
        /// This method uses reflection and data annotation to map between the source and destination objects.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="validationResults"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static TEntity ConvertAndValidate<TEntity>(this DbContext context, object source, ICollection<ValidationResult> validationResults, bool ignoreCase = false) where TEntity : class
        {
            var result = CreateInstance<TEntity>(source);

            context._Convert(source, result, validationResults, ignoreCase);

            // Validate the entity object now that it has been populated.
            foreach (var validation in context.Validate(result, source.GetType().Name))
            {
                validationResults.Add(validation);
            }

            return result;
        }

        /// <summary>
        /// Populates the result with property values from the source.
        /// Updates the collection of <typeparamref name="ValidationResult"/> objects if there were validation errors.
        /// This method uses reflection and data annotation to map between the source and destination objects.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="result"></param>
        /// <param name="validationResults"></param>
        /// <param name="ignoreCase"></param>
        public static void ConvertAndValidate<TEntity>(this DbContext context, object source, TEntity result, ICollection<ValidationResult> validationResults, bool ignoreCase = false) where TEntity : class
        {
            context._Convert(source, result, validationResults, ignoreCase);

            // Validate the entity object now that it has been populated.
            foreach (var validation in context.Validate(result, source.GetType().Name))
            {
                validationResults.Add(validation);
            }
        }

        private static void _Convert<TEntity>(this DbContext context, object source, TEntity result, ICollection<ValidationResult> validationResults = null, bool ignoreCase = false) where TEntity : class  //NOSONAR
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            if (source == null)
                result = default(TEntity);

            var source_type = source.GetType();
            var result_type = typeof(TEntity);

            if (validationResults == null)
                validationResults = new List<ValidationResult>();

            var props = result_type.GetCachedProperties();
            var source_props = source_type.GetCachedProperties();

            var _except = _methods.GetOrAdd(new KeyValuePair<Type, string>(typeof(Enumerable), "Except"), (MethodInfo)typeof(System.Linq.Enumerable).GetMember("Except", MemberTypes.Method, BindingFlags.Static | BindingFlags.Public).FirstOrDefault(m => ((MethodInfo)m).GetParameters().Count() == 2));

            foreach (var prop in props)
            {
                // Find the matching property in the source.
                var source_prop = source_props.FirstOrDefault(p =>
                (String.Compare(prop.Name, p.Name, ignoreCase) == 0
                    || p.GetCustomAttribute<ConvertMapAttribute>()?.PropertyName == prop.Name)
                && p.GetCustomAttribute<NotMappedAttribute>() == null);

                var prop_result_value = prop.GetValue(result);

                // If the source doesn't contain the property but it's required in the result add a validation error.
                if (source_prop == null)
                {
                    var attr = prop.GetCustomAttribute<RequiredAttribute>();
                    // If the result has a value it means the source just isn't providing it.  In this case do not include the required error since the result will keep it's original value.
                    if (attr != null && prop_result_value == null)
                    {
                        validationResults.Add(new ValidationResult(!String.IsNullOrEmpty(attr.ErrorMessage) ? attr.ErrorMessage : $"The property '{prop.Name}' is required.", new[] { $"{source_type.Name}.{prop.Name}" }));
                    }

                    continue;
                }

                // If the property has a NotMappedAttribute ignore it.
                if (source_prop.GetCustomAttribute<NotMappedAttribute>() != null)
                    continue;

                var c_map = source_prop.GetCustomAttribute<ConvertMapAttribute>();

                object value;

                // If there is a ConvertMapAttribute then use it to convert the source property value.
                if (c_map != null)
                {
                    // If there is a ConvertMapAttribute it may result in having multiple fields return a value.
                    if (c_map.Sources != null)
                    {
                        var values = from s in c_map.Sources
                                        join sp in source_props on s equals sp.Name
                                        select sp.GetValue(source);

                        // Merge the resulting values into the entity property.
                        value = c_map.ConvertTo(values.ToArray(), prop.PropertyType);
                    }
                    else
                    {
                        value = c_map.ConvertTo(source_prop.GetValue(source), prop.PropertyType);
                    }
                }
                else
                {
                    value = source_prop.GetValue(source);
                }

                // If the destination entity is not nullable and required but the source property is null, then we must add a validation error.
                if (value == null && Nullable.GetUnderlyingType(prop.PropertyType) == null)
                {
                    var attr = prop.GetCustomAttribute<RequiredAttribute>();

                    // If it's not required it means it can have a default value.
                    if (attr != null)
                    {
                        validationResults.Add(new ValidationResult(!String.IsNullOrEmpty(attr.ErrorMessage) ? attr.ErrorMessage : $"The property '{prop.Name}' is required and is not nullable.", new[] { $"{source_type.Name}.{prop.Name}" }));
                    }
                    else
                    {   //clear dest field to blank when updating
                        if (prop.PropertyType == typeof(string) && prop_result_value != null)
                        {
                            prop.SetValue(result, value);
                        }
                        continue;
                    }
                }

                // Three is no need to copy a duplicate value.
                if ((value == null && prop_result_value == null)
                    || ReferenceEquals(value, prop_result_value)
                    || value == prop_result_value)
                    continue;

                // The source property isn't assignable to the result, attempt to convert it.
                if (!prop.PropertyType.IsAssignableFrom(source_prop.PropertyType) 
                    && prop.PropertyType != Nullable.GetUnderlyingType(source_prop.PropertyType) 
                    && c_map == null
                    && value != null)
                {
                    try
                    {
                        // We need to initialize the result.
                        if (prop_result_value == null)
                        {
                            prop_result_value = Activator.CreateInstance(prop.PropertyType);
                        }

                        var method = _methods.GetOrAdd(new KeyValuePair<Type, string>(prop.PropertyType, "ConvertAndValidate"), typeof(DbContextExtensions).GetMethods().FirstOrDefault(m => m.Name == "ConvertAndValidate" && m.ReturnType == typeof(void)).MakeGenericMethod(prop.PropertyType));
                        method.Invoke(null, new[] { context, value, prop_result_value, validationResults, ignoreCase });

                        // If entity property is a string attempt to cast.
                        if (prop.PropertyType == typeof(string))
                        {
                            prop.SetValue(result, prop_result_value.ToString());
                        }
                        else
                        {
                            prop.SetValue(result, prop_result_value);
                        }
                    }
                    catch (Exception)
                    {
                        // A failure should add a validation error
                        validationResults.Add(new ValidationResult($"Failed to convert source property '{prop.Name}' value from type '{source_prop.PropertyType.FullName}' to '{prop.PropertyType.FullName}'.", new[] { $"{source_type.Name}.{prop.Name}" }));
                    }
                    continue;
                }

                // Collections need to be manually copied.
                if (prop.PropertyType != typeof(string)
                    && !prop.PropertyType.IsArray
                    && prop.PropertyType.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    // Get Add method from collection.
                    var item_type = prop.PropertyType.GetGenericArguments()[0];
                    var except = _except.MakeGenericMethod(item_type);
                    var add = _methods.GetOrAdd(new KeyValuePair<Type, string>(prop.PropertyType, "Add"), prop.PropertyType.GetMethod("Add"));
                    var remove = _methods.GetOrAdd(new KeyValuePair<Type, string>(prop.PropertyType, "Remove"), prop.PropertyType.GetMethod("Remove"));
                    var castTo = _methods.GetOrAdd(new KeyValuePair<Type, string>(typeof(EntityExtensions), $"CastTo{item_type.Name}"), typeof(EntityExtensions).GetMethod("CastTo", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(item_type));

                    dynamic need_to_add = castTo.Invoke(null, new[] { except.Invoke(null, new[] { value, prop_result_value }) });
                    dynamic need_to_remove = castTo.Invoke(null, new[] { except.Invoke(null, new[] { prop_result_value, value }) });

                    for (var i = 0; i < need_to_remove.Count; i++)
                    {
                        remove.Invoke(prop_result_value, new[] { need_to_remove[i] });
                    }

                    for (var i = 0; i < need_to_add.Count; i++)
                    {
                        // For each item that needs to be added, go and get it from the DB first.
                        // This must be done or EF will try and insert new records.
                        var item = need_to_add[i];
                        item = context.Set(item_type).Find(item.GetKeyValues());
                        add.Invoke(prop_result_value, new[] { item ?? need_to_add[i] });
                    }

                    continue;
                }

                prop.SetValue(result, value);
            }
        }

        /// <summary>
        /// Copy the property values from the source to the destination object.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="context"></param>
        public static void CopyTo<TEntity>(this DbContext context, TEntity source, TEntity destination) where TEntity : class //NOSONAR
        {
            if (ReferenceEquals(source, destination))
                return;

            var _except = _methods.GetOrAdd(new KeyValuePair<Type, string>(typeof(Enumerable), "Except"), (MethodInfo)typeof(System.Linq.Enumerable).GetMember("Except", MemberTypes.Method, BindingFlags.Static | BindingFlags.Public).FirstOrDefault(m => ((MethodInfo)m).GetParameters().Count() == 2));

            var props = typeof(TEntity).GetCachedProperties();

            foreach (var prop in props)
            {
                var dest_value = prop.GetValue(destination);
                var source_value = prop.GetValue(source);

                if ((source_value == null && dest_value == null)
                    || ReferenceEquals(source_value, dest_value)
                    || dest_value.Equals(source_value))
                    continue;

                // Collections need to be manually copied.
                if (prop.PropertyType != typeof(string)
                    && !prop.PropertyType.IsArray
                    && prop.PropertyType.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    // Get Add method from collection.
                    var item_type = prop.PropertyType.GetGenericArguments()[0];
                    var except = _except.MakeGenericMethod(item_type);
                    var add = _methods.GetOrAdd(new KeyValuePair<Type, string>(prop.PropertyType, "Add"), prop.PropertyType.GetMethod("Add"));
                    var remove = _methods.GetOrAdd(new KeyValuePair<Type, string>(prop.PropertyType, "Remove"), prop.PropertyType.GetMethod("Remove"));
                    var castTo = _methods.GetOrAdd(new KeyValuePair<Type, string>(typeof(EntityExtensions), $"CastTo{item_type.Name}"), typeof(EntityExtensions).GetMethod("CastTo", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(item_type));

                    dynamic need_to_add = castTo.Invoke(null, new[] { except.Invoke(null, new[] { source_value, dest_value }) });
                    dynamic need_to_remove = castTo.Invoke(null, new[] { except.Invoke(null, new[] { dest_value, source_value }) });

                    for (var i = 0; i < need_to_remove.Count; i++)
                    {
                        remove.Invoke(dest_value, new[] { need_to_remove[i] });
                    }

                    for (var i = 0; i < need_to_add.Count; i++)
                    {
                        // For each item that needs to be added, go and get it from the DB first.
                        // This must be done or EF will try and insert new records.
                        var item = need_to_add[i];
                        item = context.Set(item_type).Find(item.GetKeyValues());
                        add.Invoke(dest_value, new[] { item ?? need_to_add[i] });
                    }
                }
                else if (prop.PropertyType != typeof(string)
                    && prop.PropertyType.IsClass)
                {
                    // If the source value is an class then we want to check if it has a matching foreign key.
                    var fAttr = prop.GetCustomAttribute<ForeignKeyAttribute>();

                    if (fAttr == null)
                    {
                        prop.SetValue(destination, source_value);
                    }
                    else
                    {
                        // Check if we can lazy load the object based on the foreign key value.
                        if (source_value == null)
                        {
                            var fProp = props.FirstOrDefault(p => p.Name == fAttr.Name);

                            var fValue = fProp.GetValue(source);

                            if (fValue == null)
                            {
                                prop.SetValue(destination, source_value);
                            }
                            else
                            {
                                // Get the value from Db.
                                var value = context.Set(prop.PropertyType).Find(fValue);
                                prop.SetValue(destination, value);
                            }
                        }
                        else
                        {
                            prop.SetValue(destination, source_value);
                        }
                    }
                }
                else
                {
                    prop.SetValue(destination, source_value);
                }
            }
        }
    }
}
