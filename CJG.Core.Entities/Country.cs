using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="Country"/> class, provides the ORM a way to manage country information.
    /// </summary>
    public class Country : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key does not use IDENTITY.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), MaxLength(20)]
        public string Id { get; set; }

        [Required, MaxLength(150), Index("IX_Country", IsUnique = true)]
        public string Name { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        [Min(0)]
        public int RowSequence { get; set; }

        public virtual ICollection<Region> Regions { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="Country"/> object.
        /// </summary>
        public Country()
        {
            this.IsActive = true;
            this.RowSequence = 0;
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="Country"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="rowSequence"></param>
        public Country(string id, string name, int rowSequence = 0)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            this.Id = id;
            this.Name = name;
            this.IsActive = true;
            this.RowSequence = rowSequence;
        }
        #endregion

        #region Methods

        #endregion
    }
}
