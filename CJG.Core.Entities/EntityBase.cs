using CJG.Core.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="EntityBase"/> abstract class, provides the ORM a base class to ensure all objects have a number of properties.
    /// </summary>
   public abstract class EntityBase : IValidatableObject
    {
        #region Properties
        /// <summary>
        /// get/set - The date the entity was added to the datastore.  This is automatically generated.
        /// </summary>
        [DateTimeKind(DateTimeKind.Utc)]
        [Column(TypeName = "DATETIME2")]
        public DateTime DateAdded { get; set; }

        /// <summary>
        /// get/set - The date the entity was updated last.  This is automatically generated.
        /// </summary>
        [DateTimeKind(DateTimeKind.Utc)]
        [Column(TypeName = "DATETIME2")]
        public DateTime? DateUpdated { get; set; }

        /// <summary>
        /// get/set - A concurrency timestamp.
        /// </summary>
        [Timestamp()]
        public byte[] RowVersion { get; set; }
        #endregion  

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="EntityBase"/> object.
        /// </summary>
        public EntityBase()
        {
            this.DateAdded = AppDateTime.UtcNow;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Validates all the data annotation attributes.
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var properties = (from p in this.GetType().GetProperties()
                              where p.GetCustomAttributes(typeof(ValidationAttribute), true).Count() > 0
                              select p);

            foreach (var prop in properties)
            {
                foreach (ValidationAttribute attr in prop.GetCustomAttributes(typeof(ValidationAttribute), true))
                {
                    if (!attr.IsValid(prop.GetValue(this)))
                    {
                        yield return new ValidationResult(attr.FormatErrorMessage(prop.Name), new[] { prop.Name });
                    }
                }
            }
            yield break;
        }

        /// <summary>
        /// Get the key values for this entity object.
        /// </summary>
        /// <returns>An new instance of an <typeparamref name="Array"/> of <typeparamref name="object"/>.</returns>
        public object[] GetKeyValues()
        {
            return EntityExtensions.GetKeyValues(this);
        }

        /// <summary>
        /// Compares the specified rowversion with the entity's to determine whether they are the same.
        /// </summary>
        /// <param name="rowVersionToCompare"></param>
        /// <returns>True if the rowversion match.</returns>
        public bool IsMatchRowVersion(byte[] rowVersionToCompare)
        {
            return RowVersion.SequenceEqual(rowVersionToCompare);
        }

        #endregion
    }
}
