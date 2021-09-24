using CJG.Application.Services;
using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Models.Shared.Reports
{
	public class CompletionReportQuestionDetailsViewModel : BaseViewModel
	{
		#region Properties
		public int CompletionReportId { get; set; }
		public string Question { get; set; }
		public string Description { get; set; }
		public int Audience { get; set; }
		public int GroupId { get; set; }
		public bool IsRequired { get; set; }
		public CompletionReportQuestionTypes QuestionType { get; set; }
		public string DefaultText { get; set; }
		public int? DefaultAnswerId { get; set; }
		public int? ContinueIfAnswerId { get; set; }
		public int? StopIfAnswerId { get; set; }
		public string AnswerTableHeadings { get; set; }
		public string[] TableHeadings { get; set; }
		public bool HasParticipantOutcomeReporting { get; set; }

		public ICollection<CompletionReportAnswerDetailsViewModel> Level1Answers { get; set; } = new List<CompletionReportAnswerDetailsViewModel>();
		public ICollection<CompletionReportAnswerDetailsViewModel> Level2Answers { get; set; } = new List<CompletionReportAnswerDetailsViewModel>();
		#endregion

		#region Constructors
		public CompletionReportQuestionDetailsViewModel()
		{
		}

		public CompletionReportQuestionDetailsViewModel(GrantApplication grantApplication,
			CompletionReportQuestion completionReportQuestion,
			IEnumerable<ParticipantForm> participants)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (completionReportQuestion == null) throw new ArgumentNullException(nameof(completionReportQuestion));
			if (participants == null) throw new ArgumentNullException(nameof(participants));

			Utilities.MapProperties(completionReportQuestion, this);

			var options = completionReportQuestion.Options.Select(o => o);
			var level1Options = options.Where(o => o.Level == 1).ToList();
			var level2Options = options.Where(o => o.Level == 2).ToList();

			switch (completionReportQuestion.QuestionType)
			{
				// Question type 1: Always used on page 1 of both ETG and CWRG reports.
				// Question type 1 and "Page 1" is synonymous.
				// This question displays all participants, and gathers information on participants who left the course.
				case CompletionReportQuestionTypes.Default:
					{
						var tableHeadings = this.AnswerTableHeadings?.Split(',') ?? new string[] { };
						var answers = grantApplication.ParticipantCompletionReportAnswers.Where(o => o.QuestionId == completionReportQuestion.Id);
						var level1Answer = new CompletionReportAnswerDetailsViewModel(completionReportQuestion, grantApplication, 1)
						{
							BoolAnswer = answers.All(o => o.AnswerId == completionReportQuestion.DefaultAnswerId)
						};
						this.Level1Answers.Add(level1Answer);
						foreach (var participant in participants)
						{
							var answer = answers.FirstOrDefault(o => o.Answer.Level == 2 && o.ParticipantFormId == participant.Id);
							var level2Answer = new CompletionReportAnswerDetailsViewModel(completionReportQuestion, grantApplication, participant, answer, level2Options, 2)
							{
								BoolAnswer = answer != null
							};
							if (level2Answer.BoolAnswer)
								this.Level2Answers.Add(level2Answer);
						}
						if (tableHeadings.Length > 0 && tableHeadings[0].ToLower().Contains("select all"))
							this.AnswerTableHeadings = string.Join(",", tableHeadings.Skip(1));
						break;
					}
				case CompletionReportQuestionTypes.MultipleChoice:
				case CompletionReportQuestionTypes.Freeform:
				case CompletionReportQuestionTypes.CommunityList:
				case CompletionReportQuestionTypes.NAICSList:
				case CompletionReportQuestionTypes.NOCList:
				case CompletionReportQuestionTypes.MultipleCheckbox:
					{
						switch (this.GroupId)
						{
							// Page 3 ETG: gather info on participants who completed the course.
							// This page holds a single question.
							case Core.Entities.Constants.CompletionStepWithMultipleQuestions:
								{
									var answers = grantApplication.ParticipantCompletionReportAnswers.Where(o => o.QuestionId == completionReportQuestion.Id);
									foreach (var participant in participants)
									{
										var answer = answers.FirstOrDefault(o => o.ParticipantFormId == participant.Id);
										var level1Answer = new CompletionReportAnswerDetailsViewModel(completionReportQuestion, grantApplication, participant, answer, level1Options, 1);
										this.Level1Answers.Add(level1Answer);
									}
									break;
								}
							// Page 2 CWRG: gather info on participants who completed the course.
							// This page holds multiple questions that are asked based on the answer to the first (or previous) question.
							case Core.Entities.Constants.CompletionReportCWRGPage2:
								{
									var answers = grantApplication.ParticipantCompletionReportAnswers.Where(o => o.QuestionId == completionReportQuestion.Id);
									foreach (var participant in participants)
									{
										var answer = answers.FirstOrDefault(o => o.ParticipantFormId == participant.Id);
										var level1Answer = new CompletionReportAnswerDetailsViewModel(completionReportQuestion, grantApplication, participant, answer, level1Options, 1);
										this.Level1Answers.Add(level1Answer);
									}
									break;
								}
							// Page 3 CWRG: gather info on participants who completed the course.
							// This page holds one question.
							case Core.Entities.Constants.CompletionReportCWRGPage3:
								{
									var answers = grantApplication.ParticipantCompletionReportAnswers.Where(o => o.QuestionId == completionReportQuestion.Id);
									foreach (var participant in participants)
									{
										var answer = answers.FirstOrDefault(o => o.ParticipantFormId == participant.Id);
										var level1Answer = new CompletionReportAnswerDetailsViewModel(completionReportQuestion, grantApplication, participant, answer, level1Options, 1);
										this.Level1Answers.Add(level1Answer);
									}
									break;
								}
							// The default option supports all the questions in this set. It handles single questions that are not associated with
							// participants. The question types that fall through here, and are in use currently, are MultipleChoice and Freeform.
							default:
								{
									var answer = grantApplication.EmployerCompletionReportAnswers.FirstOrDefault(o => o.QuestionId == completionReportQuestion.Id);
									var level1Answer = new CompletionReportAnswerDetailsViewModel(completionReportQuestion, grantApplication, answer, level1Options, 1);
									this.Level1Answers.Add(level1Answer);
									break;
								}
						}
						break;
					}
				case CompletionReportQuestionTypes.DynamicCheckbox:
					{
						foreach (var participant in participants)
						{
							var level1Answer = new CompletionReportAnswerDetailsViewModel(completionReportQuestion, grantApplication, participant, 1)
							{
								EligibleCostBreakdownIds = participant.EligibleCostBreakdowns.Select(o => o.Id).ToArray()
							};
							this.Level1Answers.Add(level1Answer);
						}
						break;
					}
				case CompletionReportQuestionTypes.NOCAndNAICSList:
					{
						var tableHeadings = this.AnswerTableHeadings.Split('|');
						var descriptions = this.Description.Split('|');
						var answers = grantApplication.ParticipantCompletionReportAnswers.Where(o => o.QuestionId == completionReportQuestion.Id);
						var level1Answer = new CompletionReportAnswerDetailsViewModel(completionReportQuestion, grantApplication, 1)
						{
							BoolAnswer = answers.All(o => o.AnswerId == completionReportQuestion.DefaultAnswerId)
						};
						this.Level1Answers.Add(level1Answer);
						this.HasParticipantOutcomeReporting = grantApplication.GrantOpening.GrantStream.HasParticipantOutcomeReporting;
						foreach (var participant in participants)
						{
							var answer = answers.FirstOrDefault(o => o.Answer.Level == 2 && o.ParticipantFormId == participant.Id);
							var level2Answer = new CompletionReportAnswerDetailsViewModel(completionReportQuestion, grantApplication, participant, answer, level2Options, 2)
							{
								BoolAnswer = answer != null
							};
							if (level2Answer.BoolAnswer || this.HasParticipantOutcomeReporting)
								this.Level2Answers.Add(level2Answer);
						}
						if (this.HasParticipantOutcomeReporting)
						{
							this.AnswerTableHeadings = tableHeadings[1];
							this.Description = $"{descriptions[0]}<br />{descriptions[1]}";
						}
						else
						{
							this.AnswerTableHeadings = tableHeadings[0];
							this.Description = descriptions[0];
						}
						break;
					}
			}

			this.TableHeadings = this.AnswerTableHeadings?.Split(',') ?? new string[] { };
		}
		#endregion
	}
}
