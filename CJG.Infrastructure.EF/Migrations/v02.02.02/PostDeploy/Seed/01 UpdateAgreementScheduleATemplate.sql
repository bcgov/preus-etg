-- Default Applicant Schedule A Template
PRINT 'UPDATE Default Applicant Schedule A Template - CWRG'

UPDATE [dbo].[ServiceCategories]
SET [Caption] = 'Administration Fees',
	[DateUpdated] = GETUTCDATE()
WHERE [Id] = 5
GO

UPDATE [dbo].[EligibleExpenseTypes]
SET [Caption] = 'Administration Fees',
	[DateUpdated] = GETUTCDATE()
WHERE [Caption] = 'Administration'
GO

UPDATE [dbo].[ProgramConfigurations]
SET [ESSMaxEstimatedParticipantCost] = 5000.00,
	[DateUpdated] = GETUTCDATE()
WHERE [ClaimTypeId] = 2
GO

UPDATE [dbo].[DocumentTemplates]
SET [Body] =
N'
@model CJG.Application.Business.Models.DocumentTemplate.GrantApplicationTemplateModel
@using CJG.Core.Entities
@using System.Globalization
@functions {
	public string ToCurrency(decimal number, int precision = 2, int currencyNegativePattern = 0)
	{
		NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
		nfi.CurrencyDecimalDigits = precision;
		nfi.CurrencyNegativePattern = currencyNegativePattern;
		return number.ToString("C", nfi);
	}
}
<div class="form--readonly">
	<div class="form__group two-col">
		<label class="form__label schedule-a__label" for="FileNumber">Agreement Number:</label>
		<div class="form__control schedule-a__control">
			@Model.FileNumber
		</div>
	</div>
	<div class="form__group two-col">
		<label class="form__label schedule-a__label" for="Organization_LegalName">Applicant Name:</label>
		<div class="form__control schedule-a__control">
			@Model.OrganizationLegalName
		</div>
	</div>
	<p><h3>Agreement Term</h3></p>
	<div class="form__group two-col">
		<label class="form__label schedule-a__label" for="GrantAgreement_StartDate">Term Start Date:</label>
		<div class="form__control schedule-a__control">
			@Model.GrantAgreementStartDate
		</div>
	</div>
	<div class="form__group two-col">
		<label class="form__label schedule-a__label" for="GrantAgreement_EndDate">Term End Date:</label>
		<div class="form__control schedule-a__control">
			@Model.GrantAgreementEndDate
		</div>
	</div>
	<div class="form__group two-col">
		<label class="form__label schedule-a__label">Agreement Fiscal Year:</label>
		<div class="form__control schedule-a__control">
			@Model.AgreementFiscalYear
		</div>
	</div>
	<div class="form__group two-col">
		<label class="form__label schedule-a__label">Claim submission deadline:</label>
		<div class="form__control schedule-a__control">
			<b>@Model.ClaimDeadline</b>
		</div>
	</div>
	<p><h3>Project Delivery Plan</h3>
	Any material changes to this Agreement, including any changes to the training venue(s), require the consultation and approval of the @Model.GrantProgramCode Program staff.</p>
	<div class="form__group__wrapper ">
		<div class="form__group two-col">
			<label class="form__label schedule-a__label" for="DefaultTrainingProgram_StartDate">Project Delivery Start Date:</label>
			<div class="form__control schedule-a__control">
				@Model.StartDate
			</div>
		</div>
	</div>
	<div class="form__group two-col">
		<label class="form__label schedule-a__label" for="DefaultTrainingProgram_EndDate">Project Delivery End Date:</label>
		<div class="form__control schedule-a__control">
			@Model.EndDate
		</div>
	</div>
	::RequestChangeTrainingDates::
	<br />
	<table id="tblTrainingPrograms" class="table table-training-services js-table-training-programs">
		<thead>
			<tr>
				<th>Skills Training Course Title</th>
				<th>Third Party Training Provider</th>
				<th>Start Date</th>
				<th>End Date</th>
			</tr>
		</thead>
		<tbody>
			@foreach (var trainingProgram in Model.TrainingPrograms)
			{
				<tr>
					<td>@(trainingProgram.CourseTitle)</td>
					<td>@(trainingProgram.TrainingProviderName)</td>
					<td>@(trainingProgram.StartDate)</td>
					<td>@(trainingProgram.EndDate)</td>
				</tr>
			}
		</tbody>
	</table>
	<table id="tblServices" class="table table-training-services js-table-services">
		<thead>
			<tr>
				<th>Service Component</th>
				<th>Third Party Service Provider</th>
				<th>In-Scope Services</th>
			</tr>
		</thead>
		<tbody>
			@foreach (var EligibleCost in Model.TrainingCost.EmploymentServicesAndSupports)
			{
				<tr>
					<td>@(EligibleCost.EligibleExpenseTypeCaption)</td>
					<td>
						@foreach (var TrainingProvider in EligibleCost.TrainingProviders)
						{
							@(TrainingProvider.Name)
							<br />
						}
					</td>
					<td>
						@foreach (var Breakdown in EligibleCost.Breakdowns)
						{
							@(Breakdown.EligibleExpenseBreakdownCaption)
							<br />
						}
					</td>
				</tr>
			}
		</tbody>
	</table>
	<br />
	<p><b>NOTE:</b> Participant Information Forms (PIFs) must be submitted five (5) days before the training start date. Where an Agreement Holder is unable to ensure submission, they should contact the Ministry. No PIFs will be accepted seven (7) days after the training start date.</p>
	<div class="form__group two-col">
		<label class="form__label schedule-a__label" for="GrantAgreement_ParticipantReportingDueDate">Participant Reporting Due Date:</label>
		<div class="form__control schedule-a__control">
			@Model.GrantAgreementParticipantReportingDueDate
		</div>
	</div>
	<div class="form__group two-col">
		<label class="form__label schedule-a__label" for="GrantAgreement_ReimbursementClaimDueDate">
			Number of Participants in Project:
		</label>
		<div class="form__control schedule-a__control">
			@if (Model.TrainingCost.AgreedParticipants > 0)
			{
				@Model.TrainingCost.AgreedParticipants
			}
			else
			{
				@Model.TrainingCost.EstimatedParticipants
			}
		</div>
	</div>
	@* hide the edit/delete buttons when rendering to the application administrator *@
	<h3>Eligible Costs</h3>
	<table id="tblExpenses" class="table table--expenses js-table-expense-list">
		<thead>
			<tr>
				<th>Expense type</th>
				<th class="num-col">Number of Participants</th>
				<th class="num-col">Average Cost per Participant</th>
				<th class="num-col">Total Cost</th>
				@if (Model.ShowEmployerContribution)
				{
					<th class="num-col">@((Model.ProgramType == ProgramTypes.WDAService) ? "Applicant Contribution" : "Employer Contribution")</th>
				}
				<th class="num-col">Requested Government Contribution</th>
			</tr>
		</thead>
		<tbody>
			@foreach (var expenseItem in Model.TrainingCost.EligibleCosts)
			{
				if (Model.ShowAgreedCosts)
				{
					<tr data-expense-id="@expenseItem.EligibleExpenseTypeId"
						data-expense-item-id="@expenseItem.Id"
						data-total="@expenseItem.AgreedMaxCost.ToString()"
						data-participants="@expenseItem.AgreedMaxParticipants.ToString()">
						<td>
							@(expenseItem.EligibleExpenseTypeCaption)
						</td>
						<td class="num-col">@((expenseItem.EligibleExpenseTypeServiceCategoryServiceTypeId == ServiceTypes.Administration) ? "" : expenseItem.AgreedMaxParticipants.ToString())</td>
						<td class="num-col">@((expenseItem.EligibleExpenseTypeServiceCategoryServiceTypeId == ServiceTypes.Administration) ? "" : ToCurrency(expenseItem.AgreedMaxParticipantCost))</td>
						<td class="num-col">@(ToCurrency(expenseItem.AgreedMaxCost))</td>
						@if (Model.ShowEmployerContribution)
						{
							<td class="num-col">@(ToCurrency(expenseItem.AgreedEmployerContribution))</td>}
						<td class="num-col">@(ToCurrency(expenseItem.AgreedMaxReimbursement))</td>
					</tr>
					if (expenseItem.EligibleExpenseTypeId == 11)
					{
						foreach (var trainingProgram in Model.TrainingPrograms)
						{
							<tr class="srow">
								<td class="scolumn">
									<text>> </text> @(trainingProgram.CourseTitle)
								</td>
								<td class="scolumn"></td>
								<td class="scolumn"></td>
								<td class="scolumn">@(ToCurrency(trainingProgram.AssessedCost))</td>
								@if (Model.ShowEmployerContribution)
								{
									<td class="scolumn"></td>}
								<td class="scolumn"></td>
							</tr>
						}
					}
				}
				else
				{
					<tr data-expense-id="@expenseItem.EligibleExpenseTypeId"
						data-expense-item-id="@expenseItem.Id"
						data-total="@expenseItem.EstimatedCost.ToString()"
						data-participants="@expenseItem.EstimatedParticipants.ToString()">
						<td>@(expenseItem.EligibleExpenseTypeCaption)</td>
						<td class="num-col">@(expenseItem.EstimatedParticipants.ToString())</td>
						<td class="num-col">@(ToCurrency(expenseItem.EstimatedParticipantCost))</td>
						<td class="num-col">@(ToCurrency(expenseItem.EstimatedCost))</td>
						@if (Model.ShowEmployerContribution)
						{
							<td class="num-col">@(ToCurrency(expenseItem.EstimatedEmployerContribution))</td>}
						<td class="num-col">@(ToCurrency(expenseItem.EstimatedReimbursement))</td>
					</tr>
				}
			}
			<tr>
				<td><b>Totals</b></td>
				<td></td>
				<td></td>
				<td class="num-col">
					<b>@(ToCurrency(Model.TrainingCost.TotalAgreedMaxCost))</b>
				</td>
				@if (Model.ShowEmployerContribution)
				{
					<td class="num-col">
						<b>@(ToCurrency(Model.TrainingCost.AgreedEmployerContribution))</b>
					</td>
				}
				<td class="num-col">
					<b>@(ToCurrency(Model.TrainingCost.AgreedMaxReimbursement))</b>
				</td>
			</tr>		</tbody>
	</table>
	@if (Model.ProgramType == ProgramTypes.WDAService)
	{
		var includedServices = String.Join(" and ", Model.TrainingCost.EmploymentServicesAndSupports.Select(ec => ec.EligibleExpenseTypeServiceCategoryCaption));
		if (!String.IsNullOrWhiteSpace(includedServices))
		{
			<p class="schedule-a-ess-sum">Maximum amount allocated per Participant for Employment Assistance Services and Participant Financial Supports, combined, is $5,000.00.</p>
		}
	}
</div>
'
WHERE ID = 13;
GO

PRINT 'Update Completed!'