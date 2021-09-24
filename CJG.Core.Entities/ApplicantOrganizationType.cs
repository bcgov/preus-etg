using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJG.Core.Entities
{
    public class ApplicantOrganizationType : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="ApplicationType"/> object.
        /// </summary>
        public ApplicantOrganizationType() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="ApplicationType"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public ApplicantOrganizationType(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}
