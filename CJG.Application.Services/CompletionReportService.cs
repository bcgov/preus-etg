using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public class CompletionReportService : Service, ICompletionReportService
	{
		#region Variables
		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of a <typeparamref name="CompletionReportService"/> and initializes the specified properties.
		/// </summary>
		/// <param name="unitOfWork"></param>
		public CompletionReportService(
			IDataContext dbContext,
			HttpContextBase httpContext,
			ILogger logger) : base(dbContext, httpContext, logger)
		{
		}

		#endregion

		#region Methods
		/// <summary>
		/// Get the completion report by ID
		/// </summary>
		/// <returns></returns>
		public CompletionReport GetCurrentCompletionReport(int completionReportId)
		{
			// This returns all grant applications associated with the completion report record, which is probably not required anywhere.
			return _dbContext.CompletionReports.AsNoTracking().Where(cr => cr.Id == completionReportId).FirstOrDefault();
		}

		public IEnumerable<CompletionReportQuestion> GetCompletionReportQuestionsForStep(int completionReportId, int groupId)
		{
			return _dbContext.CompletionReportQuestions.AsNoTracking().Where(crq => crq.CompletionReportId == completionReportId && crq.GroupId == groupId && crq.IsActive).OrderBy(crq => crq.Sequence).ToArray();
		}

		public ICollection<CompletionReportOption> GetCompletionReportOptionsForQuestions(int[] questions)
		{
			return _dbContext.CompletionReportOptions.AsNoTracking().Where(cro => questions.Contains(cro.QuestionId) && cro.IsActive).Distinct().ToList();
		}

		public CompletionReportOption GetCompletionReportOption(int id)
		{
			return _dbContext.CompletionReportOptions.AsNoTracking().Where(cro => cro.Id == id).First();
		}

		public IEnumerable<CompletionReportOption> GetCompletionReportOptions()
		{
			return _dbContext.CompletionReportOptions.Where(cro => cro.IsActive);
		}


		/// <summary>
		/// Delete all the answers for the specified participants (and questions).
		/// </summary>
		/// <param name="participantFormIds"></param>
		/// <param name="questionIds"></param>
		/// <returns>Number of answers deleted.</returns>
		public int DeleteAnswersFor(int[] participantFormIds, int[] questionIds = null)
		{
			if (participantFormIds == null) throw new ArgumentNullException(nameof(participantFormIds));
			if (questionIds == null) questionIds = new int[0];

			var questions = _dbContext.ParticipantCompletionReportAnswers.Where(pcra => participantFormIds.Contains(pcra.ParticipantFormId) && (!questionIds.Any() || questionIds.Contains(pcra.QuestionId)));
			foreach (var question in questions)
			{
				question.MultAnswers.Clear();
				_dbContext.ParticipantCompletionReportAnswers.Remove(question);
			}
			return _dbContext.CommitTransaction();
		}

		public bool RecordCompletionReportAnswersForStep(int stepNo, IEnumerable<ParticipantCompletionReportAnswer> participantAnswers, IEnumerable<EmployerCompletionReportAnswer> employerAnswers, int completionReportId, int[] participantEnrollmentsForReport)
		{
			if (stepNo == Core.Entities.Constants.CompletionStepWithMultipleQuestions)
			{
				// Need to remove the previously-recorded answers, since they may no longer apply
				var questionIds = GetCompletionReportQuestionsForStep(completionReportId, stepNo).Select(cpr => cpr.Id).Distinct().ToArray();

				foreach (var participantEnrollmentId in participantEnrollmentsForReport)
				{
					RemoveParticipantAnswers(questionIds, participantEnrollmentId);
				}

				CommitTransaction();
			}

			foreach (var participantAnswer in participantAnswers)
			{
				// Check to see if this participant answer has already been recorded
				if (!_dbContext.ParticipantCompletionReportAnswers.Any(
					pcra => pcra.QuestionId == participantAnswer.QuestionId &&
					pcra.ParticipantFormId == participantAnswer.ParticipantFormId &&
					pcra.GrantApplicationId == participantAnswer.GrantApplicationId))
				{
					_dbContext.ParticipantCompletionReportAnswers.Add(participantAnswer);
				}
				else
				{
					// Check to see if the answer has changed from the previous insert/update
					if (_dbContext.ParticipantCompletionReportAnswers.Any(pcra => pcra.QuestionId == participantAnswer.QuestionId && pcra.ParticipantFormId == participantAnswer.ParticipantFormId && (participantAnswer.AnswerId != pcra.AnswerId || participantAnswer.OtherAnswer != pcra.OtherAnswer)))
					{
						var question = _dbContext.CompletionReportQuestions.Find(participantAnswer.QuestionId);

						// Need to consider that there may be participant answers for a later step which are potentially no longer applicable
						if (question.DefaultAnswerId.HasValue && participantAnswer.AnswerId != question.DefaultAnswerId.GetValueOrDefault())
						{
							// Get the questions for later steps and remove them for the participant
							var questionIds = _dbContext.CompletionReportQuestions.AsNoTracking().Where(crq => crq.GroupId > question.GroupId && crq.Audience == question.Audience).Select(crq => crq.Id).Distinct().ToArray();

							// Get any of the participant's answers for subsequent questions that are no longer applicable
							var participantCompletionReportAnswers = _dbContext.ParticipantCompletionReportAnswers.Where(pcra => pcra.ParticipantFormId == participantAnswer.ParticipantFormId && questionIds.Contains(pcra.QuestionId));

							// Remove each answer
							foreach (var participantCompletionReportAnswer in participantCompletionReportAnswers)
							{
								Attach(participantCompletionReportAnswer);
								_dbContext.ParticipantCompletionReportAnswers.Remove(participantCompletionReportAnswer);
							}
						}
					}

					var currentParticipantAnswer = _dbContext.ParticipantCompletionReportAnswers.FirstOrDefault(o =>
						o.ParticipantFormId == participantAnswer.ParticipantFormId
						&& o.QuestionId == participantAnswer.QuestionId
						&& o.GrantApplicationId == participantAnswer.GrantApplicationId);
					if (currentParticipantAnswer != null)
					{
						currentParticipantAnswer.AnswerId = participantAnswer.AnswerId;
						currentParticipantAnswer.OtherAnswer = participantAnswer.OtherAnswer;
					}
				}
			}

			foreach (var employerAnswer in employerAnswers)
			{
				// Check to see if this employer answer has already been recorded
				if (!_dbContext.EmployerCompletionReportAnswers.Any(ecra => ecra.QuestionId == employerAnswer.QuestionId && ecra.GrantApplicationId == employerAnswer.GrantApplicationId))
				{
					_dbContext.EmployerCompletionReportAnswers.Add(employerAnswer);
				}
				else
				{
					var currentEmployerAnswer = _dbContext.EmployerCompletionReportAnswers.Find(employerAnswer.GrantApplicationId, employerAnswer.QuestionId);
					if (currentEmployerAnswer != null)
					{
						currentEmployerAnswer.AnswerId = employerAnswer.AnswerId;
						currentEmployerAnswer.OtherAnswer = employerAnswer.OtherAnswer;
					}
				}
			}

			CommitTransaction();

			return true;
		}

		public IEnumerable<ParticipantForm> GetAffirmativeCompletionReportParticipants(int[] participantIds, int affirmativeAnswerId)
		{
			// Need to get all of the participant enrollment ids that answered in the affirmative for the previous step
			var participantEnrollmentIds = _dbContext.ParticipantCompletionReportAnswers.AsNoTracking().Where(pcra => participantIds.Contains(pcra.ParticipantFormId) && pcra.AnswerId == affirmativeAnswerId).Select(pcra => pcra.ParticipantFormId).ToArray();

			return _dbContext.ParticipantForms.AsNoTracking().Where(pe => participantEnrollmentIds.Contains(pe.Id)).ToList();
		}

		public ICollection<ParticipantForm> GetCompletionReportParticipants(int[] participantIds, int questionId)
		{
			// Need to get all of the participant enrollment ids from the specified question
			var participantFormIds = _dbContext.ParticipantCompletionReportAnswers.AsNoTracking().Where(pcra => participantIds.Contains(pcra.ParticipantFormId) && pcra.QuestionId == questionId).Distinct().Select(pcra => pcra.ParticipantFormId).ToArray();

			return _dbContext.ParticipantForms.AsNoTracking().Where(pe => participantFormIds.Contains(pe.Id)).ToList();
		}

		public int[] GetCompletionReportParticipantQuestionIds(int[] participantIds)
		{
			var questionIds = _dbContext.ParticipantCompletionReportAnswers.AsNoTracking().Where(pcra => participantIds.Contains(pcra.ParticipantFormId)).Distinct().Select(pcra => pcra.QuestionId).ToArray();

			return _dbContext.CompletionReportQuestions.AsNoTracking().Where(crq => crq.Audience == 0 && crq.IsActive && questionIds.Contains(crq.Id)).OrderBy(crq => crq.Sequence).Select(crq => crq.Id).ToArray();
		}

		public CompletionReport GetCompletionReportForParticipants(int[] participantIds)
		{
			// Get any questions that may have previously been answered for the participants
			var questionIds = _dbContext.ParticipantCompletionReportAnswers
				.AsNoTracking()
				.Where(pcra => participantIds.Contains(pcra.ParticipantFormId))
				.Distinct()
				.Select(pcra => pcra.QuestionId).ToArray();

			if (questionIds.Length > 0)
			{
				// Get the first question
				var question = _dbContext.CompletionReportQuestions.Find(questionIds[0]);
				if (question != null)
				{
					// Get the completion report for the question
					return _dbContext.CompletionReports.Find(question.CompletionReportId);
				}
			}

			return null;
		}

		public IEnumerable<ParticipantCompletionReportAnswer> GetParticipantCompletionReportNonDefaultAnswers(int[] participantIds, int questionId)
		{
			return _dbContext.ParticipantCompletionReportAnswers.AsNoTracking().Where(
					pcra =>
						pcra.QuestionId == questionId &&
							participantIds.Contains(pcra.ParticipantFormId) &&
							(
								pcra.Question.DefaultAnswerId.HasValue && pcra.AnswerId != pcra.Question.DefaultAnswerId
								|| !pcra.Question.DefaultAnswerId.HasValue
							)
				).ToList();
		}

		public IEnumerable<ParticipantCompletionReportAnswer> GetParticipantCompletionReportAnswers(int[] participantIds, int questionId)
		{
			return _dbContext.ParticipantCompletionReportAnswers.AsNoTracking().Where(
					pcra =>
						pcra.QuestionId == questionId && participantIds.Contains(pcra.ParticipantFormId)
				).ToList();
		}

		public IEnumerable<EmployerCompletionReportAnswer> GetEmployerCompletionReportAnswers(int grantApplicationId)
		{
			return _dbContext.EmployerCompletionReportAnswers.AsNoTracking().Where(ecra => ecra.GrantApplicationId == grantApplicationId).ToList();
		}

		public bool AllParticipantsHaveCompletedReport(int[] participantsOnClaim, int completionReportId, int completionGroupId)
		{
			// Need to get the id of the first question for the completion report
			var questionId = GetCompletionReportQuestionsForStep(completionReportId, completionGroupId).First().Id;

			// Now get the participants who answered that question
			var reportParticipants = _dbContext.ParticipantCompletionReportAnswers
				.AsNoTracking().Where(pcra => pcra.QuestionId == questionId)
				.Select(pcra => pcra.ParticipantFormId)
				.Where(pcra => participantsOnClaim.Contains(pcra)).OrderBy(pcra => pcra).ToArray();

			return Enumerable.SequenceEqual(participantsOnClaim, reportParticipants);
		}

		/// <summary>
		/// Get the status of the completion report.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		public string GetCompletionReportStatus(int grantApplicationId)
		{
			var grantApplication = Get<GrantApplication>(grantApplicationId);

			if (!grantApplication.ParticipantForms.Any())
			{
				return "Not Started";
			}
			if (grantApplication.ApplicationStateExternal == ApplicationStateExternal.Closed)
			{
				return "Submitted";
			}
			else
			{
				var completionReportGroups = GetCompletionReportGroups(grantApplicationId);

				foreach (var completionReportGroup in completionReportGroups)
				{
					foreach (var completionReportQuestion in completionReportGroup.Questions)
					{
						switch (completionReportQuestion.QuestionType)
						{
							case CompletionReportQuestionTypes.Default:
								{
									if (!grantApplication.ParticipantCompletionReportAnswers.Any(o => o.QuestionId == completionReportQuestion.Id))
										switch (completionReportGroup.Id)
										{
											case 1:
												return "Not Started";
											default:
												return "Incomplete";
										}
									break;
								}
							case CompletionReportQuestionTypes.MultipleChoice:
							case CompletionReportQuestionTypes.Freeform:
								{
									if (completionReportQuestion.GroupId != Core.Entities.Constants.CompletionStepWithMultipleQuestions
										&& !grantApplication.EmployerCompletionReportAnswers.Any(o => o.QuestionId == completionReportQuestion.Id))
										return "Incomplete";
									break;
								}
							case CompletionReportQuestionTypes.DynamicCheckbox:
								{
									// no answer required
									break;
								}
							case CompletionReportQuestionTypes.NOCAndNAICSList:
								{
									if (grantApplication.GrantOpening.GrantStream.HasParticipantOutcomeReporting)
									{
										var answers = grantApplication.ParticipantCompletionReportAnswers.Where(o => o.QuestionId == completionReportQuestion.Id);

										foreach (var answer in answers)
										{
											if (answer.AnswerId == null
												&& (answer.ParticipantForm.NocId == null
												|| answer.ParticipantForm.NaicsId == null
												|| string.IsNullOrEmpty(answer.ParticipantForm.EmployerName)))
												return "Incomplete";
										}
									}

									break;
								}
						}
					}
				}
			}

			return "Complete";
		}

		public IEnumerable<ParticipantForm> GetParticipantsForReport(int grantApplicationId)
		{
			var grantApplication = Get<GrantApplication>(grantApplicationId);
			var claim = grantApplication.GetCurrentClaim();

			if (claim != null)
			{
				return claim.EligibleCosts.SelectMany(ec => ec.ParticipantCosts.Select(pc => pc.ParticipantForm)).Distinct().ToArray();
			}
			else
			{
				return grantApplication.ParticipantForms.ToArray();
			}
		}

		private void RemoveParticipantAnswers(int[] questionIds, int participantFormId)
		{
			var participantCompletionReportAnswers = _dbContext.ParticipantCompletionReportAnswers.Where(pcra => pcra.ParticipantFormId == participantFormId && questionIds.Contains(pcra.QuestionId));

			// Remove each answer
			foreach (var participantCompletionReportAnswer in participantCompletionReportAnswers)
			{
				Attach(participantCompletionReportAnswer);
				_dbContext.ParticipantCompletionReportAnswers.Remove(participantCompletionReportAnswer);
			}
		}

		/// <summary>
		/// Get List of Completion Report Question Groups
		/// </summary>
		/// <param name="grantApplicationId">Grant Application Id used for filtering</param>
		/// <returns>Completion Report Question Groups</returns>
		public IEnumerable<CompletionReportGroup> GetCompletionReportGroups(int grantApplicationId)
		{
			return GetCompletionReportGroups(grantApplicationId, o => o);
		}

		/// <summary>
		/// Get List of Completion Report Question Groups
		/// </summary>
		/// <typeparam name="T">generic type</typeparam>
		/// <param name="grantApplicationId">Grant Application Id used for filtering</param>
		/// <param name="select">object needed</param>
		/// <returns>Completion Report Question Groups</returns>
		/// <returns></returns>
		public IEnumerable<T> GetCompletionReportGroups<T>(int grantApplicationId, Func<CompletionReportGroup, T> select)
		{
			var grantApplication = Get<GrantApplication>(grantApplicationId);
			var programTypeId = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;

			return _dbContext.CompletionReportGroups.AsNoTracking().Where(o =>
				o.CompletionReportId == grantApplication.CompletionReportId &&
				(o.ProgramTypeId == null || o.ProgramTypeId == programTypeId))
				.OrderBy(o => o.RowSequence).Select(select).ToArray();
		}

		/// <summary>
		/// Get Completion Report Question Group by Id
		/// </summary>
		/// <param name="completionReportId">Completion Report Id</param>
		/// <param name="completionReportGroupId">Completion Report Group Id</param>
		/// <returns>Completion Report Question Group</returns>
		public CompletionReportGroup GetCompletionReportGroup(int completionReportId, int completionReportGroupId)
		{
			var completionReportGroup = _dbContext.CompletionReportGroups.FirstOrDefault(o => o.Id == completionReportGroupId && o.CompletionReportId == completionReportId);

			// Remove inactive options from the select dropdown.
			if (completionReportGroup != null)
			{
				foreach (var question in completionReportGroup.Questions)
				{
					question.Options = question.Options.Where(x => x.IsActive == true).ToList();
				}
			}
			return completionReportGroup;
		}
		#endregion
	}
}
