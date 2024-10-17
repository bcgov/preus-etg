using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// ParticipantCompletionReportAnswer class, provides a way to collect all the participant completion report answers.
    /// </summary>
    public class ParticipantCompletionReportAnswer : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key and foreign key to the participant form.
        /// </summary>
        [Key, Column(Order = 0)]
        public int ParticipantFormId { get; set; }

        /// <summary>
        /// get/set - The participant form to which this answer belongs.
        /// </summary>
        [ForeignKey(nameof(ParticipantFormId))]
        public virtual ParticipantForm ParticipantForm { get; set; }

        /// <summary>
        /// get/set - The primary key and foreign key to the grant application.
        /// </summary>
        [Key, Column(Order = 1)]
        public int? GrantApplicationId { get; set; }

        /// <summary>
        /// get/set - The grant application to which this answer belongs.
        /// </summary>
        [ForeignKey(nameof(GrantApplicationId))]
        public virtual GrantApplication GrantApplication { get; set; }

        /// <summary>
        /// get/set - The primary key and foreign key to the completion report question.
        /// </summary>
        [Key, Column(Order = 2)]
        public int QuestionId { get; set; }

        /// <summary>
        /// get/set - The completion report question to which this answer belongs.
        /// </summary>
        [ForeignKey(nameof(QuestionId))]
        public virtual CompletionReportQuestion Question { get; set; }

        /// <summary>
        /// get/set - The foreign key to the completion report option given as the answer.
        /// </summary>
        public int? AnswerId { get; set; }

        /// <summary>
        /// get/set - The completion report option given as the answer.
        /// </summary>
        [ForeignKey(nameof(AnswerId))]
        public virtual CompletionReportOption Answer { get; set; }

		/// <summary>
		/// get/set - A collection of multiple answers associated with this answer.
		/// </summary>
		public virtual ICollection<CompletionReportOption> MultAnswers { get; set; } = new List<CompletionReportOption>();

		/// <summary>
		/// get/set - The free-format text answer when "other" was the option chosen.
		/// </summary>
		[MaxLength(500)]
        public string OtherAnswer { get; set; }

		/// <summary>
		/// get/set - The foreign key to the community Id given as the answer.
		/// </summary>
		public int? CommunityId { get; set; }

		/// <summary>
		/// get/set - The community given as the answer.
		/// </summary>
		[ForeignKey(nameof(CommunityId))]
		public virtual Community Community { get; set; }

		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of an EmployerCompletionReportAnswer object.
		/// </summary>
		public ParticipantCompletionReportAnswer()
        {

        }

        /// <summary>
        /// Creates a new instance of an EmployerCompletionReqportAnswer object and initializes it.
        /// </summary>
        /// <param name="participantForm"></param>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        public ParticipantCompletionReportAnswer(ParticipantForm participantForm, CompletionReportQuestion question, CompletionReportOption answer)
        {
            this.ParticipantForm = participantForm ?? throw new ArgumentNullException(nameof(participantForm));
            this.ParticipantFormId = participantForm.Id;
            this.GrantApplication = participantForm.GrantApplication;
            this.GrantApplicationId = participantForm.GrantApplicationId;
            this.Question = question ?? throw new ArgumentNullException(nameof(question));
            this.QuestionId = question.Id;
            this.Answer = answer ?? throw new ArgumentNullException(nameof(answer));
            this.AnswerId = answer.Id;
			this.MultAnswers.Clear();
        }

        /// <summary>
        /// Creates a new instance of an EmployerCompletionReqportAnswer object and initializes it.
        /// </summary>
        /// <param name="participantForm"></param>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        public ParticipantCompletionReportAnswer(ParticipantForm participantForm, CompletionReportQuestion question, string answer)
        {
            if (String.IsNullOrWhiteSpace(answer))
                throw new ArgumentException("The answer is required.", nameof(answer));

            this.ParticipantForm = participantForm ?? throw new ArgumentNullException(nameof(participantForm));
            this.ParticipantFormId = participantForm.Id;
            this.GrantApplication = participantForm.GrantApplication;
            this.GrantApplicationId = participantForm.GrantApplicationId;
            this.Question = question ?? throw new ArgumentNullException(nameof(question));
            this.QuestionId = question.Id;
            this.OtherAnswer = answer;
			this.MultAnswers.Clear();
		}
        #endregion

        #region Methods
        /// <summary>
        /// Validates the EmployerCompletionReportAnswer property values.
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // An answer must be provided.
            //if (this.AnswerId == null || String.IsNullOrWhiteSpace(this.OtherAnswer))
            //    yield return new ValidationResult("An answer must be entered.", new[] { nameof(this.Answer) });

            foreach (var validation in base.Validate(validationContext))
            {
                yield return validation;
            }
        }
        #endregion
    }
}
