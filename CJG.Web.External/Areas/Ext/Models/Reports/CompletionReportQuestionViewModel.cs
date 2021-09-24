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
		public bool DisplayOnlyIfGoto { get; set; }
		public int? NextQuestion { get; set; }
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

			// TODO: Add default answers for new types here
			// TODO: This code is a mess of conditionals in the wrong place. Fix.

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
				case CompletionReportQuestionTypes.CommunityList:
				case CompletionReportQuestionTypes.NOCList:
				case CompletionReportQuestionTypes.NAICSList:
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
							case Core.Entities.Constants.CompletionReportCWRGPage2:
								{
									if (completionReportQuestion.QuestionType == CompletionReportQuestionTypes.NOCList)
									{
										var answers = grantApplication.ParticipantCompletionReportAnswers.Where(o => o.QuestionId == completionReportQuestion.Id);
										foreach (var participant in grantApplication.ParticipantForms.OrderBy(o => o.LastName))
										{
											var answer = answers.FirstOrDefault(o => o.ParticipantFormId == participant.Id);
											if (answer != null)
											{
												var nocs = nationalOccupationalClassificationService.GetNationalOccupationalClassifications(participant.NocId);

												var level1Answer = new CompletionReportAnswerViewModel(completionReportQuestion, grantApplication, participant, answer, 1);
												nocs.ForEach(item =>
												{
													var property = level1Answer.GetType().GetProperty($"Noc{item.Level}Id");
													property?.SetValue(level1Answer, item.Id);
												});

												this.Level1Answers.Add(level1Answer);
											}
										}
									}
									else if (completionReportQuestion.QuestionType == CompletionReportQuestionTypes.NAICSList)
									{
										var answers = grantApplication.ParticipantCompletionReportAnswers.Where(o => o.QuestionId == completionReportQuestion.Id);
										foreach (var participant in grantApplication.ParticipantForms.OrderBy(o => o.LastName))
										{
											var answer = answers.FirstOrDefault(o => o.ParticipantFormId == participant.Id);
											if (answer != null)
											{
												var naics = naIndustryClassificationSystemService.GetNaIndustryClassificationSystems(participant.NaicsId);

												var level1Answer = new CompletionReportAnswerViewModel(completionReportQuestion, grantApplication, participant, answer, 1);
												naics.ForEach(item =>
												{
													var property = level1Answer.GetType().GetProperty($"Naics{item.Level}Id");
													property?.SetValue(level1Answer, item.Id);
												});

												this.Level1Answers.Add(level1Answer);
											}
										}
									}
									else
									{
										var answers = grantApplication.ParticipantCompletionReportAnswers.Where(o => o.QuestionId == completionReportQuestion.Id);
										foreach (var participant in grantApplication.ParticipantForms.OrderBy(o => o.LastName))
										{
											var answer = answers.FirstOrDefault(o => o.ParticipantFormId == participant.Id);
											var level1Answer = new CompletionReportAnswerViewModel(completionReportQuestion, grantApplication, participant, answer, 1);
											this.Level1Answers.Add(level1Answer);
										}
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
				case CompletionReportQuestionTypes.MultipleCheckbox:
					{
						var answers = grantApplication.ParticipantCompletionReportAnswers.Where(o => o.QuestionId == completionReportQuestion.Id);
						foreach (var participant in grantApplication.ParticipantForms.OrderBy(o => o.LastName))
						{
							var answer = answers.FirstOrDefault(o => o.ParticipantFormId == participant.Id);
							if (answer != null)
							{
								// CompletionReportQMultiCheckbox type data is passed in the client data model in CSV format in the StringAnswer.
								// Inside the client code, the selected items are stored in a single answer by participant in the IsSelected[] Array (as a dictionary/associative array)
								// IsSelected[] is used this way to enable connection with the Angular ng-model which must use it to address its data.
								//
								// Note that this is different to the Dynamic Checkbox above. It sends down 1 answer per selection per participant.
								// Which is slow when validating the data client side, b/c searching means looking at the entire answer array before
								// determining the selection does not exist.
								// This is important in the JS validateCompletionReportStep(), which is called every time Angular changes the data.

								answer.OtherAnswer = "";
								if (answer.MultAnswers != null)
								{
									foreach (var multAnswer in answer.MultAnswers)
									{
										if (answer.OtherAnswer.Length != 0)
											answer.OtherAnswer += ",";
										answer.OtherAnswer += multAnswer.Id.ToString(); // The Option ID
									}
								}
								var level1Answer = new CompletionReportAnswerViewModel(completionReportQuestion, grantApplication, participant, answer, 1);
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
