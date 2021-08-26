using System.Collections.Generic;

namespace CJG.Core.Entities
{
    /// <summary>
    /// A Community class, provides a way to manage a list of communities.
    /// </summary>
    public class Community : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a Community object.
        /// </summary>
        public Community()
        {

        }
      
        /// <summary>
        /// Createss a new instance of a Community object and initializes it.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public Community(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}
