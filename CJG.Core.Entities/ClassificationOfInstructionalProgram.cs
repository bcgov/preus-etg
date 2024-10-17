using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="ClassificationOfInstructionalProgram"/> class, provides the ORM with a way to manage national industry classification systems.
	/// </summary>
	public class ClassificationOfInstructionalProgram : EntityBase
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
        [Required, MaxLength(10), Index("IX_ClassificationOfInstructionalPrograms", IsUnique = true)]
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
        [Index("IX_ClassificationOfInstructionalPrograms", 3, IsUnique = true)]
        public int Level { get; set; }

        /// <summary>
        /// get/set - The foreign key to the parent NAICS.
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// get/set - The parent NAICS.
        /// </summary>
        [ForeignKey(nameof(ParentId))]
        public ClassificationOfInstructionalProgram Parent { get; set; }


		/// <summary>
		/// get/set - field specifying the NAICS version
		/// </summary>
		[Required]
		public int CIPSVersion { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="NaIndustryClassificationSystem"/> object.
		/// </summary>
		public ClassificationOfInstructionalProgram()
        {

        }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ClassificationOfInstructionalProgram"/> object.
		/// </summary>
		/// <param name="code"></param>
		/// <param name="description"></param>
		/// <param name="level"></param>
		/// <param name="parentId"></param>
		/// <param name="cipsVersion"></param>
		public ClassificationOfInstructionalProgram(string code, string description, int level, int? parentId, int cipsVersion)
        {
            if (String.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));

            if (String.IsNullOrEmpty(description))
                throw new ArgumentNullException(nameof(description));

            if (level < 0)
                throw new ArgumentException("The level must be greater than or equal to 0.", nameof(level));


            this.Code = code;
            this.Description = description;
            this.Level = level;
            this.ParentId = parentId;
			this.CIPSVersion = cipsVersion;

		}
		#endregion

		#region Methods
		/// <summary>
		/// Provides a formatted string that represents this <typeparamref name="ClassificationOfInstructionalProgram"/>.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
        {
            return $"{Code} | {Description}";
        }
        #endregion
    }
}