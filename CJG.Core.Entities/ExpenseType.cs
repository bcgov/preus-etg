using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CJG.Core.Entities
{
    /// <summary>
    /// ExpenseTypes enum, provides a way to control how each expense type is implemented.
    /// </summary>
    public enum ExpenseTypes
    {
        /// <summary>
        /// ParticipantAssigned - These expenses must be assigned to participants.
        /// </summary>
        [Description("Participant Assigned Cost")]
        ParticipantAssigned = 1,
        /// <summary>
        /// ParticipantLimited - These expenses are not assigned to participants during estimates or claims, but their limits are based on the max cost per participant.
        /// </summary>
        [Description("Participant Maximum Cost Limit")]
        ParticipantLimited = 2,
        /// <summary>
        /// NotParticipantLimited - These expenses are not assigned to participants during estimates or claims, their limits are not related to max cost per participant.
        /// </summary>
        [Description("Not Participant Limited")]
        NotParticipantLimited = 3,
        /// <summary>
        /// AutoLimitEstimatedCosts - These expenses are not assigned to participants during estimates or claims, their limits are based on the total estimated cost of all expense lines * the eligible expense type rate.
        /// </summary>
        [Description("Auto Limit Estimated Costs")]
        AutoLimitEstimatedCosts = 4
    }
   
    /// <summary>
    /// ExpenseType class, provides a way to manage expense types.  An expense type controls the way an eligible expense is implemented.
    /// </summary>
    public class ExpenseType : LookupTable<ExpenseTypes>
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key does not use IDENTITY.
        /// </summary>
        //[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        //public new ExpenseTypes Id { get; set; }

        /// <summary>
        /// get/set - A description for this expense type.
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// get - All the eligible expense type associated with this expense type.
        /// </summary>
        public virtual ICollection<EligibleExpenseType> EligibleExpenseTypes { get; set; } = new List<EligibleExpenseType>();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of an ExpenseType object.
        /// </summary>
        public ExpenseType()
        {

        }

        /// <summary>
        /// Creates a new instance of an ExpenseType object.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="rowSequence"></param>
        public ExpenseType(ExpenseTypes type, int rowSequence = 0) : base(type.GetDescription(), rowSequence)
        {
            this.Id = type;
        }
        #endregion
    }
}
