using System.ComponentModel;

namespace CJG.Core.Entities
{
	/// <summary>
	/// ReconcilationStates enum, provides all possible states for individual payment requests.
	/// </summary>
	public enum ReconciliationStates
	{
		/// <summary>
		/// NotReconciled - This payment has not be reconciled yet.  This is the default state.
		/// </summary>
		[Description("Not Reconciled")]
		NotReconciled = 0,
		/// <summary>
		/// Reconciled - The payment has been reconciled.
		/// </summary>
		[Description("Reconciled")]
		Reconciled = 1,
		/// <summary>
		/// NoMatch - There is no matching payment in either CAS or STG.
		/// </summary>
		[Description("No Match")]
		NoMatch = 2,
		/// <summary>
		/// Duplicate - There is a duplicate payment in CAS.
		/// </summary>
		[Description("Duplicate")]
		Duplicate = 3,
		/// <summary>
		/// InvalidAmount - There is an invalid amount in CAS or STG.
		/// </summary>
		[Description("Invalid Amount")]
		InvalidAmount = 4,
		/// <summary>
		/// InvalidDocumentNumber - There is an invalid document number in CAS.
		/// </summary>
		[Description("Invalid Document Number")]
		InvalidDocumentNumber = 5,
		/// <summary>
		/// InvalidSupplierName - There is an invalid supplier name in CAS or STG.
		/// </summary>
		[Description("Invalid Supplier Name")]
		InvalidSupplierName = 6,
		/// <summary>
		/// InvalidSupplierNumber - There is an invalid supplier number in CAS or STG.
		/// </summary>
		[Description("Invalid Supplier Number")]
		InvalidSupplierNumber = 7,
	}
}
