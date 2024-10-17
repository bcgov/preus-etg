using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{

    /// <summary>
    /// <typeparamref name="GrantApplicationClaimState"/> class, provides the ORM a way to manage GrantApplication External State information.
    /// </summary>
    [Table("ClaimStates")]
    public class GrantApplicationClaimState : LookupTable<int>
    {
        #region Properties
        [MaxLength(1000)]
        public string Description { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantApplicationClaimState"/> object.
        /// </summary>
        public GrantApplicationClaimState() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantApplicationClaimState"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="description"></param>
        /// <param name="rowSequence"></param>
        public GrantApplicationClaimState(ClaimState state, string description, int rowSequence = 0) : base(state.GetDescription(), rowSequence)
        {
            this.Id = (int)state;
            this.Description = description;
        }
        #endregion
    }
}
