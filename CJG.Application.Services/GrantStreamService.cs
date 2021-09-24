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
	/// <summary>
	/// GrantStreamService class, provides a way to manage grant streams.
	/// </summary>
	public class GrantStreamService : Service, IGrantStreamService
	{
		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public GrantStreamService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		public IEnumerable<GrantStream> GetAll()
		{
			var defaultGrantProgramId = GetDefaultGrantProgramId();
			return _dbContext.GrantStreams
				.Where(gs => gs.GrantProgram.Id == defaultGrantProgramId)
				.ToArray();
		}

		public GrantStream Get(string streamName)
		{
			return _dbContext.GrantStreams
				       .AsNoTracking()
				       .FirstOrDefault(x => x.Name == streamName) ?? throw new NoContentException($"Unable to find grant stream '{streamName}'.");
		}

		public GrantStream Get(int id)
		{
			return Get<GrantStream>(id);
		}

		/// <summary>
		/// Get all the grant streams for the specified grant program and filter based on whether they are live (active or assigned to grant openings).
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <param name="onlyLive"></param>
		/// <returns></returns>
		public IEnumerable<GrantStream> GetGrantStreamsForProgram(int grantProgramId, bool onlyLive = true)
		{
			if (grantProgramId == 0)
				grantProgramId = GetDefaultGrantProgramId();

			return _dbContext.GrantStreams.AsNoTracking()
				.Where(gs => gs.GrantProgramId == grantProgramId && (!onlyLive || gs.IsActive || gs.GrantOpenings.Count > 0))
				.OrderBy(gs => gs.Name)
				.ThenByDescending(gs => gs.DateFirstUsed)
				.ThenByDescending(gs => gs.DateAdded);
		}

		/// <summary>
		/// Add a new grant stream to the datasource.
		/// </summary>
		/// <param name="grantStream"></param>
		/// <returns></returns>
		public GrantStream Add(GrantStream grantStream)
		{
			if (grantStream == null)
				throw new ArgumentNullException(nameof(grantStream));

			_dbContext.GrantStreams.Add(grantStream);
			_dbContext.CommitTransaction();

			return grantStream;
		}

		/// <summary>
		/// Update the specified grant stream in the datasource.
		/// </summary>
		/// <param name="grantStream"></param>
		/// <returns></returns>
		public GrantStream Update(GrantStream grantStream)
		{
			if (grantStream == null)
				throw new ArgumentNullException(nameof(grantStream));

			_dbContext.Update(grantStream);
			_dbContext.CommitTransaction();

			return grantStream;
		}

		/// <summary>
		/// Delete the specified grant stream from the datasource.
		/// </summary>
		/// <param name="grantStream"></param>
		public void Delete(GrantStream grantStream)
		{
			if (grantStream == null)
				throw new ArgumentNullException(nameof(grantStream));

			if (grantStream.GrantOpenings.Count > 0)
				throw new InvalidOperationException("Cannot delete a grant stream that has grant openings.");

			if (grantStream.ProgramConfigurationId != grantStream.GrantProgram.ProgramConfigurationId)
			{
				foreach (var eligibleExpenseType in grantStream.ProgramConfiguration.EligibleExpenseTypes.ToArray())
				{
					foreach (var breakdown in eligibleExpenseType.Breakdowns.ToArray())
					{
						_dbContext.EligibleExpenseBreakdowns.Remove(breakdown);
					}
					_dbContext.EligibleExpenseTypes.Remove(eligibleExpenseType);
				}
				_dbContext.ProgramConfigurations.Remove(grantStream.ProgramConfiguration);
			}
			if (grantStream.GrantStreamEligibilityQuestions != null)
			{
				foreach(var question in grantStream.GrantStreamEligibilityQuestions.ToArray())
				{
					_dbContext.GrantStreamEligibilityQuestions.Remove(question);
				}
			}

			if (grantStream.AccountCodeId != grantStream.GrantProgram.AccountCodeId)
				_dbContext.AccountCodes.Remove(grantStream.AccountCode);

			_dbContext.GrantStreams.Remove(grantStream);
			_dbContext.CommitTransaction();
		}

		public IEnumerable<ReportRate> GetReportRates(int grantStreamId)
		{
			return _dbContext.ReportRates.AsNoTracking().Where(rr => rr.GrantStreamId == grantStreamId);
		}

		public EligibleExpenseType AddEligibleExpenseType(int grantStreamId, EligibleExpenseType eligibleExpenseType)
		{
			var config = Get<GrantStream>(grantStreamId).ProgramConfiguration;
			eligibleExpenseType.ProgramConfigurations.Add(config);
			_dbContext.EligibleExpenseTypes.Add(eligibleExpenseType);

			_dbContext.CommitTransaction();
			return eligibleExpenseType;
		}

		public IEnumerable<EligibleExpenseType> GetAllActiveEligibleExpenseTypes(int grantStreamId)
		{
			return Get<GrantStream>(grantStreamId).ProgramConfiguration.EligibleExpenseTypes.
													Where(t => t.IsActive).
													OrderBy(t => t.RowSequence).
													ThenBy(t => t.Caption).ToArray();
		}

		public IEnumerable<EligibleExpenseType> GetAutoIncludeActiveEligibleExpenseTypes(int grantStreamId)
		{
			return Get<GrantStream>(grantStreamId).ProgramConfiguration.EligibleExpenseTypes.
													Where(t => t.IsActive && t.AutoInclude).
													OrderBy(t => t.RowSequence).
													ThenBy(t => t.Caption).ToArray();
		}

		public IEnumerable<EligibleExpenseType> GetAllEligibleExpenseTypes(int grantStreamId)
		{
			return Get<GrantStream>(grantStreamId).ProgramConfiguration.EligibleExpenseTypes.
													OrderBy(t => t.RowSequence).
													ThenBy(t => t.Caption).ToArray();
		}

		public IEnumerable<EligibleExpenseType> GetAllActiveForGrantStream(int id)
		{
			return Get<GrantStream>(id).ProgramConfiguration.EligibleExpenseTypes.
													Where(t => t.IsActive).
													OrderBy(t => t.RowSequence).
													ThenBy(t => t.Caption).ToArray();
		}

		public IEnumerable<GrantStream> GetGrantStreams(int? year, int? program)
		{
			var defaultGrantProgramId = GetDefaultGrantProgramId();
			if (program == null)
				program = defaultGrantProgramId;

			var streamIds = _dbContext.GrantOpenings.AsNoTracking()
				.Where(x => x.TrainingPeriod.FiscalYearId == year && x.GrantStream.GrantProgramId == program)
				.Select(x => x.GrantStreamId)
				.ToList();

			return _dbContext.GrantStreams.AsNoTracking()
				.Where(x => streamIds.Contains(x.Id))
				.OrderBy(gp => gp.Name);
		}

		public IEnumerable<GrantStreamEligibilityQuestion> GetGrantStreamQuestions(int grantStreamId)
		{
			return _dbContext.GrantStreamEligibilityQuestions.Where(x => x.GrantStreamId == grantStreamId).OrderBy(n => n.RowSequence);
		}

		public GrantStreamEligibilityQuestion GetGrantStreamQuestion(int id)
		{
			return _dbContext.GrantStreamEligibilityQuestions.AsNoTracking().First(x => x.Id == id);
		}

		/// <summary>
		/// Update a grant stream question to the datasource.
		/// </summary>
		/// <param name="question"></param>
		/// <returns></returns>
		public GrantStreamEligibilityQuestion UpdateGrantStreamQuestion(GrantStreamEligibilityQuestion question)
		{
			if (question == null)
				throw new ArgumentNullException(nameof(question));

			_dbContext.Update(question);
			_dbContext.CommitTransaction();
			return question;
		}

		/// <summary>
		/// Update a list of grant stream questions to the datasource.
		/// </summary>
		/// <param name="question"></param>
		/// <returns></returns>
		public IEnumerable<GrantStreamEligibilityQuestion> UpdateGrantStreamQuestions(IEnumerable<GrantStreamEligibilityQuestion> questions)
		{
			if (questions == null)
				throw new ArgumentNullException(nameof(questions));

			foreach(var question in questions)
			{
				if (question.Id == 0)
					_dbContext.GrantStreamEligibilityQuestions.Add(question);
			}
			_dbContext.CommitTransaction();
			return GetGrantStreamQuestions(questions.First().GrantStreamId);
		}

		/// <summary>
		/// Add a new grant stream question to the datasource.
		/// </summary>
		/// <param name="question"></param>
		/// <returns></returns>
		public GrantStreamEligibilityQuestion AddGrantStreamQuestion(GrantStreamEligibilityQuestion question)
		{
			if (question == null)
				throw new ArgumentNullException(nameof(question));

			_dbContext.GrantStreamEligibilityQuestions.Add(question);
			_dbContext.CommitTransaction();

			return question;
		}

		/// <summary>
		/// Add a list of grant stream answers to the datasource.
		/// </summary>
		/// <param name="answers"></param>
		/// <returns></returns>
		public void AddGrantStreamAnswers(IEnumerable<GrantStreamEligibilityAnswer> answers)
		{
			if (answers == null)
				throw new ArgumentNullException(nameof(answers));

			foreach(var answer in answers)
			{
				_dbContext.GrantStreamEligibilityAnswers.Add(answer);
			}
			_dbContext.CommitTransaction();
		}
		/// <summary>
		/// Get a list of grant stream answers.
		/// </summary>
		/// <param name="grantId"></param>
		/// <returns></returns>
		public IEnumerable<GrantStreamEligibilityAnswer> GetGrantStreamAnswers(int grantId)
		{
			return _dbContext.GrantStreamEligibilityAnswers.Where(x => x.GrantApplicationId == grantId);
		}

		/// <summary>
		/// Remove existing grant stream answers if the grant is altered.
		/// </summary>
		/// <param name="grantId"></param>
		/// <returns></returns>
		public void RemoveGrantStreamAnswers(int grantId)
		{
			if (grantId == 0)
				throw new ArgumentNullException(nameof(grantId));
			var answers = GetGrantStreamAnswers(grantId);
			if (answers.Count() != 0)
			{
				_dbContext.GrantStreamEligibilityAnswers.RemoveRange(answers);
			}
		}

		/// <summary>
		/// Determine if there are any applications associated with the specified grant stream.
		/// </summary>
		/// <param name="grantStreamId"></param>
		/// <returns></returns>
		public bool HasApplications(int grantStreamId)
		{
			return _dbContext.GrantStreams.Any(gs => gs.Id == grantStreamId && gs.GrantOpenings.Any(go => go.GrantApplications.Any()));
		}
		#endregion
	}
}