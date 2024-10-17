using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="NoteType"/> class, provides the ORM a way to manage note types.
    /// </summary>
    public class NoteType : LookupTable<NoteTypes>
    {
        #region Properties
        /// <summary>
        /// get/set - The description for this note type.
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }

        [DefaultValue(false)]
        public bool IsSystem { get; set; } = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="NoteType"/> object.
        /// </summary>
        public NoteType() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="NoteType"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        /// <param name="rowSequence"></param>
        public NoteType(NoteTypes type, string description, int rowSequence = 0) : base(type.GetDescription(), rowSequence)
        {
            this.Description = description;
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="NoteType"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        /// <param name="isSystem">Is a system generated note.</param>
        /// <param name="rowSequence"></param>
        public NoteType(NoteTypes type, string description, bool isSystem, int rowSequence = 0) : this(type, description, rowSequence)
        {
            this.IsSystem = isSystem;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns a string in the following format - $"{Caption}"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Caption}";
        }
        #endregion
    }

    /// <summary>
    /// <typeparamref name="NoteTypes"/> enum, provides a list of all valid Note Type Id's.
    /// </summary>
    public enum NoteTypes
    {
        /// <summary>
        /// AS - Note to Assessor
        /// </summary>
        AS = 12,
        /// <summary>
        /// PD = Note to Director
        /// </summary>
        PD = 7,
        /// <summary>
        /// PM - Note to Policy
        /// </summary>
        PM = 13,
        /// <summary>
        /// QA - Note to QA
        /// </summary>
        QA = 8,
        /// <summary>
        /// NR - Note to Reimbursement
        /// </summary>
        NR = 14,
        /// <summary>
        /// ED - File Change
        /// </summary>
        ED = 15,
        /// <summary>
        /// NT - Notification
        /// </summary>
        NT = 16,
        /// <summary>
        /// WF - Workflow
        /// </summary>
        WF = 11,
        /// <summary>
        /// AA - Assessor Assigned
        /// </summary>
        AA = 17,
        /// <summary>
        /// TD - Training Date Change
        /// </summary>
        TD = 18
    }
}
