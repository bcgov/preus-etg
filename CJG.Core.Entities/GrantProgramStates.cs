using System.ComponentModel;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="GrantProgramState"/> enum, provides all possible states for Grant Programs
    /// </summary>
    public enum GrantProgramStates
    {
        /// <summary>
        /// NotImplemented - A newly created Grant Program which cannot contain published Grant Openings.
        /// </summary>
        [Description("Not Implemented")]
        NotImplemented = 0,
        /// <summary>
        /// Implemented - A Grant Program that is available for the public.
        /// </summary>
        [Description("Implemented")]
        Implemented = 1
    }
}
