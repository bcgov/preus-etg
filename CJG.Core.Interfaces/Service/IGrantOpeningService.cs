using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Core.Interfaces.Service
{
	public interface IGrantOpeningService : IService
	{
		GrantOpening Get(int id);
		GrantOpening Add(GrantOpening grantOpening);
		GrantOpening Update(GrantOpening grantOpening);
		void Delete(int grantOpeningId);
		void Delete(GrantOpening grantOpening);

		IEnumerable<GrantOpening> GetGrantOpenings(GrantStream grantStream);
		IEnumerable<GrantOpening> GetGrantOpenings(FiscalYear fiscalYear);
		IEnumerable<GrantOpening> GetGrantOpeningsForFiscalYear(int fiscalYearId);
		IEnumerable<GrantOpening> GetGrantOpenings(DateTime forThisDate);
		IEnumerable<GrantOpening> GetGrantOpenings(DateTime forThisDate, int grantProgramId);
		IEnumerable<GrantOpening> GetGrantOpenings(DateTime forThisDate, int grantStreamId, int trainingPeriodId);
		IEnumerable<GrantOpening> GetGrantOpenings();
		IEnumerable<GrantOpening> GetPublishedGrantOpenings(DateTime forThisDate);
		bool AssociatedWithAGrantStream(GrantStream grantStream);
		GrantOpening GetGrantOpening(int grantStreamId, int trainingPeriodId);
		IEnumerable<GrantOpening> GetGrantOpeningsInStates(IEnumerable<GrantOpeningStates> filterStates);
		bool CanDeleteGrantOpening(GrantOpening grantOpening);
		List<int> MakeReservation(GrantApplication grantApplication);
		bool CanMakeReservation(GrantApplication grantApplication);
		void AdjustFinancialStatements(GrantApplication grantApplication, ApplicationStateInternal priorState, ApplicationWorkflowTrigger trigger);
		IDictionary<int, decimal> GetBudgetAllocationAmountsInTrainingPeriods(int fiscalYearId, int programId, int grantStreamId);
		IDictionary<int, decimal[]> GetGrantOpeningFinancialsInTrainingPeriods(int fiscalYearId, int programId, int grantStreamId, decimal slippage);

		IQueryable<GrantOpening> GetGrantOpenings(GrantProgram grantProgram);

		#region Workflow
		void Schedule(GrantOpening grantOpening);
		void Unschedule(GrantOpening grantOpening);
		void Close(GrantOpening grantOpening);
		void OpenForSubmit(GrantOpening grantOpening);
		void Reopen(GrantOpening grantOpening);
		bool CheckGrantOpeningByFiscalAndStream(int trainingPeriodId, int grantStreamId);
		#endregion
	}
}
