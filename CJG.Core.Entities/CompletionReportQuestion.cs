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
    /// A CompletionReportQuestion class, provides a question for completion reports.
    /// </summary>
    public class CompletionReportQuestion : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key uses IDENTITY.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// get/set - The foreign key to the completion report.
        /// </summary>
        public int CompletionReportId { get; set; }

        /// <summary>
        /// get/set - The completion report to which this question belongs.
        /// </summary>
        [ForeignKey(nameof(CompletionReportId))]
        public virtual CompletionReport CompletionReport { get; set; }

        /// <summary>
        /// get/set - The text for the question.
        /// </summary>
        [Required, MaxLength(500)]
        public string Question { get; set; }

        /// <summary>
        /// get/set - The description for the question.
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// get/set - The audience for this completion report.
        /// </summary>
        public int Audience { get; set; }

        /// <summary>
        /// get/set - The group for this question.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// get/set - The completion report group (step) to which this question belongs.
        /// </summary>
        [ForeignKey(nameof(GroupId))]
        public virtual CompletionReportGroup Group { get; set; }

        /// <summary>
        /// get/set - The sequence of this question. 
        /// </summary>
        [DefaultValue(0)]
        public int Sequence { get; set; }

        /// <summary>
        /// get/set - Indicates whether or not this question is required.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// get/set - Indicates whether or not this question is active.
        /// </summary>
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// get/set - Determines the type of question e.g. multiple-choice, open-ended, etc.
        /// </summary>
        public CompletionReportQuestionTypes QuestionType { get; set; }

        /// <summary>
        /// get/set - The default text for the question.
        /// </summary>
        [MaxLength(500)]
        public string DefaultText { get; set; }

        /// <summary>
        /// get/set - The default answer for the the question.
        /// </summary>
        public int? DefaultAnswerId { get; set; }

        /// <summary>
        /// get/set - The default answer for the the question.
        /// </summary>
        [ForeignKey(nameof(DefaultAnswerId))]
        public virtual CompletionReportOption DefaultAnswer { get; set; }

        /// <summary>
        /// get/set - The default answer for the the question.
        /// </summary>
        public int? ContinueIfAnswerId { get; set; }

        /// <summary>
        /// get/set - The default answer for the the question.
        /// </summary>
        [ForeignKey(nameof(ContinueIfAnswerId))]
        public virtual CompletionReportOption ContinueIfAnswer { get; set; }

        /// <summary>
        /// get/set - The default answer for the the question.
        /// </summary>
        public int? StopIfAnswerId { get; set; }

        /// <summary>
        /// get/set - The default answer for the the question.
        /// </summary>
        [ForeignKey(nameof(StopIfAnswerId))]
        public virtual CompletionReportOption StopIfAnswer { get; set; }

        /// <summary>
        /// get/set - A heading for the answers.
        /// </summary>
        [MaxLength(500)]
        public string AnswerTableHeadings { get; set; }

		/// <summary>
		/// get/set - Indicates whether or not this question is active.
		/// </summary>
		[DefaultValue(false)]
		public bool DisplayOnlyIfGoto { get; set; } = false;

		/// <summary>
		/// get/set - After this question, go to this question. Used in conjunction with DisplayOnlyIfGoto.
		/// EG. Assuming there is a question to "go to" after this question, it is likely that the "goto" question
		/// should be set to DisplayOnlyIfGoto=true so that the question does not display in it natural sequence as well.
		/// </summary>
		public int?	NextQuestion { get; set; }

		/// <summary>
		/// get - A collection of the valid options for this question.
		/// </summary>
		public virtual ICollection<CompletionReportOption> Options { get; set; } = new List<CompletionReportOption>();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a CompletionReportOption object.
        /// </summary>
        public CompletionReportQuestion()
        {

        }

        /// <summary>
        /// Creates a new instance of a CompletionReportOption object and initializes it.
        /// </summary>
        /// <param name="completionReport"></param>
        /// <param name="question"></param>
        /// <param name="audience"></param>
        /// <param name="isRequired"></param>
        public CompletionReportQuestion(CompletionReport completionReport, string question, int audience = 0, bool isRequired = true)
        {
            this.CompletionReport = completionReport ?? throw new ArgumentNullException(nameof(CompletionReport));
            this.CompletionReportId = completionReport.Id;
            this.Question = question ?? throw new ArgumentNullException(nameof(question));
            this.Audience = audience;
            this.IsRequired = isRequired;
        }
        #endregion
    }
}
