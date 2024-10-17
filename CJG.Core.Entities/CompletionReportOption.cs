using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJG.Core.Entities
{
    /// <summary>
    /// A CompletionReportOption class, provides a way to manage a list of options to a question.
    /// </summary>
    public class CompletionReportOption : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key uses IDENTITY.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// get/set - The foreign key to the completion report question.
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// get/set - The completion report question to which this option belongs.
        /// </summary>
        [ForeignKey(nameof(QuestionId))]
        public virtual CompletionReportQuestion Question { get; set; }

        /// <summary>
        /// get/set - The answer for the question.
        /// </summary>
        [Required, MaxLength(500)]
        public string Answer { get; set; }

        /// <summary>
        /// get/set - Indicates the level of option this is.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// get/set - Indicates whether or not this question is active.
        /// </summary>
        [DefaultValue(false)]
        public bool TriggersNextLevel { get; set; }

		/// <summary>
		/// get/set - Indicates the Question to display if the user selects this option.
		/// </summary>
		public int NextQuestion { get; set; }

		/// <summary>
		/// get/set - Indicates the sequence of the option.
		/// </summary>
		[DefaultValue(0)]
        public int Sequence { get; set; }

        /// <summary>
        /// get/set - Indicates whether or not this option is active.
        /// </summary>
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// get/set - Indicates whether or not to include an "other" option that requires free-format text.
        /// </summary>
        [DefaultValue(false)]
        public bool DisplayOther { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a CompletionReportOption object.
        /// </summary>
        public CompletionReportOption()
        {

        }

        /// <summary>
        /// Creates a new instance of a CompletionReportOption object and initializes it.
        /// </summary>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        /// <param name="level"></param>
        /// <param name="triggersNextLevel"></param>
        /// <param name="displayOther"></param>
        /// <param name="sequence"></param>
        public CompletionReportOption(CompletionReportQuestion question, string answer, int level, bool triggersNextLevel, bool displayOther, int sequence = 0, int NextQuestion = 0)
        {
            this.Question = question ?? throw new ArgumentNullException(nameof(question));
            this.QuestionId = question.Id;
            this.Answer = answer ?? throw new ArgumentNullException(nameof(answer));
            this.Level = level;
            this.TriggersNextLevel = triggersNextLevel;
			this.NextQuestion = NextQuestion;
			this.DisplayOther = displayOther;
            this.Sequence = sequence;
        }
        #endregion
    }
}
