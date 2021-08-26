using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Reports
{
	public class CompletionReportQuestionViewModel : BaseViewModel
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

		public IEnumerable<CompletionReportOptionViewModel> Level1Options { get; set; }
		public IEnumerable<CompletionReportOptionViewModel> Level2Options { get; set; }

		public ICollection<CompletionReportAnswerViewModel> Level1Answers { get; set; } = new List<CompletionReportAnswerViewModel>();
		public ICollection<CompletionReportAnswerViewModel> Level2Answers { get; set; } = new List<CompletionReportAnswerViewModel>();
		#endregion

		#region Constructors
		public CompletionReportQuestionViewModel()
		{
		}

		public CompletionReportQuestionViewModel(GrantApplication grantApplication,
			CompletionReportQuestion completionReportQuestion,
			INaIndustryClassificationSystemService naIndustryClassificationSystemService,
			INationalOccupationalClassificationService nationalOccupationalClassificationService)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (completionReportQuestion == null) throw new ArgumentNullException(nameof(completionReportQuestion));

			Utilities.MapProperties(completionReportQuestion, this);
			var options = completionReportQuestion.Options.Select(o => new CompletionReportOptionViewModel(o));
			var level1Options = options.Where(o => o.Level == 1).ToList();
			var level2Options = options.Where(o => o.Level == 2).ToList();

			level2Options.Insert(0, new CompletionReportOptionViewModel()
			{
				QuestionId = completionReportQuestion.Id,
				Answer = completionReportQuestion.DefaultText,
				Level = 2
			});

			switch (completionReportQuestion.QuestionType)
			{
				case CompletionReportQuestionTypes.Default:
					{
						var answers = grantApplication.ParticipantCompletionReportAnswers.Where(o => o.QuestionId == completionReportQuestion.Id);
						var level1Answer = new CompletionReportAnswerViewModel(completionReportQuestion, grantApplication, 1)
						{
							BoolAnswer = answers.All(o => o.AnswerId == completionReportQuestion.DefaultAnswerId)
						};
						this.Level1Answers.Add(level1Answer);
						foreach (var participant in grantApplication.ParticipantForms.OrderBy(o => o.LastName))
						{
							var answer = answers.FirstOrDefault(o => o.Answer.Level == 2 && o.ParticipantFormId == participant.Id);
							var level2Answer = new CompletionReportAnswerViewModel(completionReportQuestion, grantApplication, participant, answer, 2)
							{
								BoolAnswer = answer != null
							};
							this.Level2Answers.Add(level2Answer);
						}
						break;
					}
				case CompletionReportQuestionTypes.MultipleChoice:
				case CompletionReportQuestionTypes.Freeform:
					{
						level1Options.Insert(0, new CompletionReportOptionViewModel()
						{
							QuestionId = completionReportQuestion.Id,
							Answer = completionReportQuestion.DefaultText,
							Level = 1
						});

						switch (this.GroupId)
						{
							case Core.Entities.Constants.CompletionStepWithMultipleQuestions:
								{
									var answers = grantApplication.ParticipantCompletionReportAnswers.Where(o => o.QuestionId == completionReportQuestion.Id);
									foreach (var participant in grantApplication.ParticipantForms.OrderBy(o => o.LastName))
									{
										var answer = answers.FirstOrDefault(o => o.ParticipantFormId == participant.Id);
										var level1Answer = new CompletionReportAnswerViewModel(completionReportQuestion, grantApplication, participant, answer, 1);
										this.Level1Answers.Add(level1Answer);
									}
									break;
								}
							default:
								{
									var answer = grantApplication.EmployerCompletionReportAnswers.FirstOrDefault(o => o.QuestionId == completionReportQuestion.Id);
									var level1Answer = new CompletionReportAnswerViewModel(completionReportQuestion, grantApplication, answer, 1);
									this.Level1Answers.Add(level1Answer);
									break;
								}
						}
						break;
					}
				case CompletionReportQuestionTypes.DynamicCheckbox:
					{
						foreach (var participant in grantApplication.ParticipantForms.OrderBy(o => o.LastName))
						{
							foreach (var eligibleCostBreakdown in participant.EligibleCostBreakdowns)
							{
								var level1Answer = new CompletionReportAnswerViewModel(completionReportQuestion, grantApplication, participant, 1)
								{
									EligibleCostBreakdownId = eligibleCostBreakdown.Id
								};
								this.Level1Answers.Add(level1Answer);
							}
						}
						break;
					}
				case CompletionReportQuestionTypes.NOCAndNAICSList:
					{
						var tableHeadings = this.AnswerTableHeadings.Split('|');
						var descriptions = this.Description.Split('|');
						var answers = grantApplication.ParticipantCompletionReportAnswers.Where(o => o.QuestionId == completionReportQuestion.Id);
						var level1Answer = new CompletionReportAnswerViewModel(completionReportQuestion, 1)
						{
							GrantApplicationId = grantApplication.Id,
							BoolAnswer = answers.All(o => o.AnswerId == completionReportQuestion.DefaultAnswerId)
						};
						this.Level1Answers.Add(level1Answer);
						foreach (var participant in grantApplication.ParticipantForms.OrderBy(o => o.LastName))
						{
							var answer = answers.FirstOrDefault(o => o.Answer.Level == 2 && o.ParticipantFormId == participant.Id);
							var naics = naIndustryClassificationSystemService.GetNaIndustryClassificationSystems(participant.NaicsId);
							var nocs = nationalOccupationalClassificationService.GetNationalOccupationalClassifications(participant.NocId);

							var level2Answer = new CompletionReportAnswerViewModel(completionReportQuestion, 2)
							{
								GrantApplicationId = grantApplication.Id,
								ParticipantFormId = participant.Id,
								BoolAnswer = answer != null,
								IntAnswer = answer?.AnswerId ?? 0,
								StringAnswer = answer?.OtherAnswer,
								EmployerName = participant.EmployerName
							};

							naics.ForEach(item =>
							{
								var property = level2Answer.GetType().GetProperty($"Naics{item.Level}Id");
								property?.SetValue(level2Answer, item.Id);
							});

							nocs.ForEach(item =>
							{
								var property = level2Answer.GetType().GetProperty($"Noc{item.Level}Id");
								property?.SetValue(level2Answer, item.Id);
							});

							this.Level2Answers.Add(level2Answer);
						}
						if (this.HasParticipantOutcomeReporting = grantApplication.GrantOpening.GrantStream.HasParticipantOutcomeReporting)
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

			this.Level1Options = level1Options.ToArray();
			this.Level2Options = level2Options.ToArray();
			this.TableHeadings = this.AnswerTableHeadings?.Split(',') ?? new string[] { };
		}
		#endregion
	}
}
