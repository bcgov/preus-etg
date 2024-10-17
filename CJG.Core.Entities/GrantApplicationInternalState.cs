using System.ComponentModel.DataAnnotations;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="GrantApplicationInternalState"/> class, provides the ORM a way to manage GrantApplication Internal State information.
    /// </summary>
    public class GrantApplicationInternalState : LookupTable<int>
    {
        #region Properties
        [MaxLength(1000)]
        public string Description { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantApplicationInternalState"/> object.
        /// </summary>
        public GrantApplicationInternalState() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantApplicationInternalState"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="description"></param>
        /// <param name="rowSequence"></param>
        public GrantApplicationInternalState(ApplicationStateInternal state, string description, int rowSequence = 0) : base(state.GetDescription(), rowSequence)
        {
            this.Id = (int)state;
            this.Description = description;
        }
        #endregion
    }
}
