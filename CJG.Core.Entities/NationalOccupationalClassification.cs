using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="NationalOccupationalClassification"/> class, provides the ORM with a way to manage national occupational classifications.
	/// </summary>
	public class NationalOccupationalClassification : EntityBase
	{
		/// <summary>
		/// get/set - The primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - A unique code to identify this NOC.
		/// </summary>
		[Required, MaxLength(10), Index("IX_NaIndustryClassificationSystem", 1, IsUnique = true)]
		public string Code { get; set; }

		/// <summary>
		/// get/set - A description of this NOC.
		/// </summary>
		[Required, MaxLength(250)]
		public string Description { get; set; }

		/// <summary>
		/// get/set - The level of this NOC.
		/// </summary>
		[Required]
		[Index("IX_NaIndustryClassificationSystem", 2, IsUnique = true)]
		public int Level { get; set; }

		/// <summary>
		/// get/set - The foreign key of the parent NOC.
		/// </summary>
		public int? ParentId { get; set; }

		/// <summary>
		/// get/set - The parent NOC.
		/// </summary>
		[ForeignKey(nameof(ParentId))]
		public NationalOccupationalClassification Parent { get; set; }

		/// <summary>
		/// get/set - The smallest Id of the child NOC.
		/// </summary>
		[Required]
		public int Left { get; set; }

		/// <summary>
		/// get/set - The largest ID of the child NOC.
		/// </summary>
		[Required]
		public int Right { get; set; }

		// The implementation of the NOC Version coding used for this NOC. 2016 or 2021 are the current valid values
		[Required, MaxLength(10), Index("IX_NaIndustryClassificationSystem", 3, IsUnique = true)]
		public string NOCVersion { get; set; }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="NationalOccupationalClassification"/> object.
		/// </summary>
		public NationalOccupationalClassification()
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="NationalOccupationalClassification"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="code"></param>
		/// <param name="description"></param>
		/// <param name="level"></param>
		/// <param name="parentId"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public NationalOccupationalClassification(string code, string description, int level, int? parentId, int left, int right)
		{
			if (string.IsNullOrEmpty(code))
				throw new ArgumentNullException(nameof(code));

			if (string.IsNullOrEmpty(description))
				throw new ArgumentNullException(nameof(description));

			if (level < 0)
				throw new ArgumentException("The level must be greater than or equal to 0.", nameof(level));

			if (left < 0)
				throw new ArgumentException("Left must be greater than or equal to 0.", nameof(left));

			if (right < 0)
				throw new ArgumentException("Right must be greater than or equal to 0.", nameof(right));

			Code = code;
			Description = description;
			Level = level;
			ParentId = parentId;
			Left = left;
			Right = right;
		}

		/// <summary>
		/// Provides a formatted string that represents this <typeparamref name="NationalOccupationalClassification"/>.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Code} | {Description}";
		}
	}
}