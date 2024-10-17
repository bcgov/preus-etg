using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="TrainingProviderInventory"/> class, provides the ORM a way to manage training provider inventory.
	/// </summary>
	[Table(nameof(TrainingProviderInventory))]
	public class TrainingProviderInventory : EntityBase
	{
		/// <summary>
		/// get/set - The primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The name to identify this training provider inventory.
		/// </summary>
		[Required, MaxLength(250)]
		[Index("IX_TrainingProviderInventory", 3)]
		public string Name { get; set; }

		/// <summary>
		/// get/set - A way to include and search the Training Provider Inventory by an acronym.
		/// </summary>
		[Index("IX_TrainingProviderInventory", 4), MaxLength(10)]
		public string Acronym { get; set; }

		/// <summary>
		/// get/set - The notest associated with this training provider inventory.
		/// </summary>
		public string Notes { get; set; }

		/// <summary>
		/// get/set - Whether this training provider inventory is eligible.
		/// </summary>
		[Index("IX_TrainingProviderInventory", 2)]
		public bool IsEligible { get; set; }

		/// <summary>
		/// get/set - Whether this training provider inventory is active.
		/// </summary>
		[Index("IX_TrainingProviderInventory", 1)]
		public bool IsActive { get; set; }

		/// <summary>
		/// get/set - whether a training provider is classified as risky or not
		/// </summary>
		public bool RiskFlag { get; set; }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingProviderInventory"/> object.
		/// </summary>
		public TrainingProviderInventory()
		{ }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingProviderInventory"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="isEligible"></param>
		/// <param name="isActive"></param>
		public TrainingProviderInventory(string name, bool isEligible = true, bool isActive = true)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			Name = name;
			IsEligible = isEligible;
			IsActive = isActive;
		}
	}
}
