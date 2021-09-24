PRINT 'Updating Document Template - ETG Schedule A'

UPDATE	[dbo].[DocumentTemplates]
SET		[Body] = N'@model CJG.Application.Business.Models.DocumentTemplate.GrantApplicationTemplateModel
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
		<label class="form__label" for="FileNumber">File Number:</label>
		<div class="form__control">
			@Model.FileNumber
		</div>
	</div>
	<div class="form__group two-col">
		<label class="form__label" for="Organization_LegalName">Applicant Name:</label>
		<div class="form__control">
			@Model.OrganizationLegalName
		</div>
	</div>
	<p><h3>Agreement Term</h3></p>
	<div class="form__group two-col">
		<label class="form__label" for="GrantAgreement_StartDate">Start Date:</label>
		<div class="form__control">
			@Model.GrantAgreementStartDate
		</div>
	</div>
	<div class="form__group two-col">
		<label class="form__label" for="GrantAgreement_EndDate">End Date:</label>
		<div class="form__control">
			@Model.GrantAgreementEndDate
		</div>
	</div>
	<p><h3>Delivery Plan</h3></p>
	<div class="form__group two-col">
		<label class="form__label" for="DefaultTrainingProgram_CourseTitle">Training Program:</label>
		<div class="form__control">
			@Model.TrainingPrograms.FirstOrDefault().CourseTitle
		</div>
	</div>
	<div class="form__group__wrapper">
		<div class="form__group two-col">
			<label class="form__label" for="DefaultTrainingProgram_TrainingProvider_Name">Training Provider:</label>
			<div class="form__control">
				@Model.TrainingPrograms.FirstOrDefault().TrainingProviderName
			</div>
		</div>
		::RequestChangeTrainingProvider::
	</div>
	<div class="form__group__wrapper ">
		<div class="form__group two-col">
			<label class="form__label" for="DefaultTrainingProgram_StartDate">Training Start Date:</label>
			<div class="form__control">
				@Model.StartDate
			</div>
		</div>
		::RequestChangeTrainingDates::
	</div>
	<div class="form__group two-col">
		<label class="form__label" for="DefaultTrainingProgram_EndDate">Training End Date:</label>
		<div class="form__control">
			@Model.EndDate
		</div>
	</div>
	<div class="form__group two-col">
		<label class="form__label" for="GrantAgreement_ParticipantReportingDueDate">Participant Reporting Due Date:</label>
		<div class="form__control">
			@Model.GrantAgreementParticipantReportingDueDate
		</div>
	</div>
	<div class="form__group two-col">
		<div class="label-desc-group">
			<label class="form__label" for="GrantAgreement_ReimbursementClaimDueDate">
				Reimbursement Claim Due Date:
			</label>
			<br />
			<span class="text--small text--normal">To remain eligible for a grant, a Reimbursement Claim must be submitted on or before this date.</span>
		</div>
		<div class="form__control">
			@Model.GrantAgreementReimbursementClaimDueDate
		</div>
	</div>
	<div class="form__group two-col">
		<label class="form__label" for="GrantAgreement_ReimbursementClaimDueDate">
			Number of Participants in Training Program:
		</label>
		<div class="form__control">
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
	@if (Model.RequireAllParticipantsBeforeSubmission)
	{
	<div class="form__group two-col">
		<label class="form__label" for="GrantAgreement_ReimbursementClaimDueDate">
			Participant Name(s):
		</label>
		<div class="form__control">
			@foreach (var participant in Model.Participants)
			{
				<div>@participant.Name</div>
			}
		</div>
	</div>
	}
	@* hide the edit/delete buttons when rendering to the application administrator *@
	<h3>Eligible Training Costs</h3>
	<table id="tblExpenses" class="table table--expenses js-table-expense-list">
		<thead>
			<tr>
				<th>Expense type</th>
				<th class="num-col">Number of participants</th>
				<th class="num-col">Cost per participant</th>
				<th class="num-col">Total training cost</th>
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
						<td>@(expenseItem.EligibleExpenseTypeCaption)</td>
						<td class="num-col">@(expenseItem.AgreedMaxParticipants.ToString())</td>
						<td class="num-col">@(ToCurrency(expenseItem.AgreedMaxParticipantCost))</td>
						<td class="num-col">@(ToCurrency(expenseItem.AgreedMaxCost))</td>
					</tr>
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
			</tr>
			<tr>
				<td><b>Employer contribution</b></td>
				<td></td>
				<td></td>
				<td class="num-col">
					<b>@(ToCurrency(Model.TrainingCost.AgreedEmployerContribution))</b>
				</td>
			</tr>
			<tr>
				<td><b>Requested government contribution</b></td>
				<td></td>
				<td></td>
				<td class="num-col">
					<b>@(ToCurrency(Model.TrainingCost.AgreedMaxReimbursement))</b>
				</td>
			</tr>
		</tbody>
	</table>
</div>
',
	[DateUpdated] = GETUTCDATE()
WHERE [Id] = 2