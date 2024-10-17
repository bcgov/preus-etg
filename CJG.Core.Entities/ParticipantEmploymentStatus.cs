using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{

    /// <summary>
    /// ParticipantEmploymentStatus class, provides a list of valid participant employment statuses.
    /// </summary>
    public class ParticipantEmploymentStatus : LookupTable<int>
    {
        #region Properties
        /// <summary>
        /// get/set - A description of this participant employment status.
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a ParticipantEmploymentStatus object.
        /// </summary>
        public ParticipantEmploymentStatus()
        {

        }

        /// <summary>
        /// Creates a new instance of a ParticipantEmploymentStatus object and initializes it.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public ParticipantEmploymentStatus(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}
