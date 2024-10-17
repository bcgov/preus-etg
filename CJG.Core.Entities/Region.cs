using DataAnnotationsExtensions;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="Region"/> class, provides the ORM a way to manage country regions.
    /// </summary>
    public class Region : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key and unique code for this region.
        /// </summary>
        [Key, Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.None), MaxLength(10)]
        public string Id { get; set; }

        /// <summary>
        /// get/set - The primary key and foreign key to the country.
        /// </summary>
        [Key, Column(Order = 2), Index("IX_Region", 2, IsUnique = true), MaxLength(20)]
        public string CountryId { get; set; }

        /// <summary>
        /// get/set - The parent country.
        /// </summary>
        [ForeignKey(nameof(CountryId))]
        public virtual Country Country { get; set; }

        /// <summary>
        /// get/set - The unique name of the region.
        /// </summary>
        [MaxLength(250), Index("IX_Region", 1, IsUnique = true)]
        public string Name { get; set; }

        /// <summary>
        /// get/set - Whether this region is active.
        /// </summary>
        [DefaultValue(true)]
        public bool IsActive { get; set; }

        /// <summary>
        /// get/set - The order to display this region.
        /// </summary>
        [Min(0)]
        public int RowSequence { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="Region"/> object.
        /// </summary>
        public Region()
        {
            this.IsActive = true;
            this.RowSequence = 0;
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="Region"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="country"></param>
        /// <param name="rowSequence"></param>
        public Region(string id, string name, Country country, int rowSequence = 0)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            this.Id = id;
            this.Name = name;
            this.Country = country ?? throw new ArgumentNullException(nameof(country));
            this.CountryId = country.Id;
            this.IsActive = true;
            this.RowSequence = rowSequence;
        }
        #endregion

        #region Methods

        #endregion
    }
}
