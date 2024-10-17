using System.ComponentModel.DataAnnotations;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="GrantApplicationExternalState"/> class, provides the ORM a way to manage GrantApplication External State information.
    /// </summary>
    public class GrantApplicationExternalState : LookupTable<int>
    {
        #region Properties
        [MaxLength(1000)]
        public string Description { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantApplicationExternalState"/> object.
        /// </summary>
        public GrantApplicationExternalState() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantApplicationExternalState"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="description"></param>
        /// <param name="rowSequence"></param>
        public GrantApplicationExternalState(ApplicationStateExternal state, string description, int rowSequence = 0) : base(state.GetDescription(), rowSequence)
        {
            this.Id = (int)state;
            this.Description = description;
        }
        #endregion
    }
}
