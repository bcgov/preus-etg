using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="NaIndustryClassificationSystem"/> class, provides the ORM with a way to manage national industry classification systems.
    /// </summary>
    public class NaIndustryClassificationSystem : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key uses IDENTITY.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// get/set - The unique code to identify this NAICS.
        /// </summary>
        [Required, MaxLength(10), Index("IX_NaIndustryClassificationSystem", IsUnique = true)]
        public string Code { get; set; }

        /// <summary>
        /// get/set - A description of this NAICS.
        /// </summary>
        [Required, MaxLength(250)]
        public string Description { get; set; }

        /// <summary>
        /// get/set - The level of this NAICS.
        /// </summary>
        [Required]
        [Index("IX_NaIndustryClassificationSystem", 3, IsUnique = true)]
        public int Level { get; set; }

        /// <summary>
        /// get/set - The foreign key to the parent NAICS.
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// get/set - The parent NAICS.
        /// </summary>
        [ForeignKey(nameof(ParentId))]
        public NaIndustryClassificationSystem Parent { get; set; }

        /// <summary>
        /// get/set - The lowest Id that belongs to this NAICS.
        /// </summary>
        [Required]
        [Index("IX_NaIndustryClassificationSystem", 1, IsUnique = true)]
        public int Left { get; set; }

        /// <summary>
        /// get/set - The highest I dthat belongs to this NAICS.
        /// </summary>
        [Required]
        [Index("IX_NaIndustryClassificationSystem", 2, IsUnique = true)]
        public int Right { get; set; }
		#endregion

		/// <summary>
		/// get/set - field specifying the NAICS version
		/// </summary>
		[Required]
		public int NAICSVersion { get; set; }

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="NaIndustryClassificationSystem"/> object.
		/// </summary>
		public NaIndustryClassificationSystem()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="NaIndustryClassificationSystem"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="description"></param>
        /// <param name="level"></param>
        /// <param name="parentId"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public NaIndustryClassificationSystem(string code, string description, int level, int? parentId, int left, int right, int naicsVersion)
        {
            if (String.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));

            if (String.IsNullOrEmpty(description))
                throw new ArgumentNullException(nameof(description));

            if (level < 0)
                throw new ArgumentException("The level must be greater than or equal to 0.", nameof(level));

            if (left < 0)
                throw new ArgumentException("Left must be greater than or equal to 0.", nameof(left));

            if (right < 0)
                throw new ArgumentException("Right must be greater than or equal to 0.", nameof(right));

            this.Code = code;
            this.Description = description;
            this.Level = level;
            this.ParentId = parentId;
            this.Left = left;
            this.Right = right;
			this.NAICSVersion = naicsVersion;

		}
        #endregion

        #region Methods
        /// <summary>
        /// Provides a formatted string that represents this <typeparamref name="NaIndustryClassificationSystem"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Code} | {Description}";
        }
        #endregion
    }
}