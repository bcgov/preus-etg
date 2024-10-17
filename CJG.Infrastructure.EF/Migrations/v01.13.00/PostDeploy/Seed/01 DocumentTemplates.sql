 PRINT 'Updating [DocumentTemplates] - Fix CWRG Schedule A Eligible Components'

UPDATE dbo.[DocumentTemplates]
SET [Body] = '@model CJG.Core.Entities.GrantApplication 
@using CJG.Core.Entities 
@using System.Globalization 
@{  
	var showAgreedCosts = Model.TrainingPrograms.FirstOrDefault().GrantApplication.ApplicationStateInternal.ShowAgreedCosts();  
	var programType = Model.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;  
	var showEmployerContribution = !((Model.ReimbursementRate == 1) && (Model.CalculateAgreedEmployerContribution() == 0)); 
} 
@functions {  
	public string ToCurrency(decimal number, int precision = 2, int currencyNegativePattern = 0) {   
		NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;   
		nfi.CurrencyDecimalDigits = precision;   
		nfi.CurrencyNegativePattern = currencyNegativePattern;   
		return number.ToString("C", nfi);  
	} 
} 

<div class="form--readonly">  
	<div class="form__group two-col">   
		<label class="form__label schedule-a__label" for="FileNumber">Agreement Number:</label>   
		<div class="form__control schedule-a__control">@Model.FileNumber</div>
	</div>  
	<div class="form__group two-col">   
		<label class="form__label schedule-a__label" for="Organization_LegalName">Applicant Name:</label>   
		<div class="form__control schedule-a__control">@Model.Organization.LegalName</div>  
	</div>  
	<p><h3>Agreement Term</h3></p>  
	<div class="form__group two-col">   
		<label class="form__label schedule-a__label" for="GrantAgreement_StartDate">Term Start Date:</label>   
		<div class="form__control schedule-a__control">@(Model.GrantAgreement.StartDate.ToLocalTime().ToString("MMMM dd, yyyy"))</div>  
	</div>  
	<div class="form__group two-col">   
		<label class="form__label schedule-a__label" for="GrantAgreement_EndDate">Term End Date:</label>   
		<div class="form__control schedule-a__control">@(Model.GrantAgreement.EndDate.ToLocalTime().ToString("MMMM dd, yyyy"))</div>  
	</div>  
	<p><h3>Program Delivery Plan</h3></p>  
	<div class="form__group__wrapper ">   
		<div class="form__group two-col">    
			<label class="form__label schedule-a__label" for="DefaultTrainingProgram_StartDate">Delivery Start Date:</label>    
			<div class="form__control schedule-a__control">@(Model.StartDate.ToLocalTime().ToString("MMMM dd, yyyy"))</div>   
		</div>  
	</div>  
	<div class="form__group two-col">   
		<label class="form__label schedule-a__label" for="DefaultTrainingProgram_EndDate">Delivery End Date:</label>   
		<div class="form__control schedule-a__control">@(Model.EndDate.ToLocalTime().ToString("MMMM dd, yyyy"))</div>  
	</div>  
	::RequestChangeTrainingDates::  <br />  
	@{   
		var TrainingPrograms = Model.TrainingPrograms;  
	}  
	<table id="tblTrainingPrograms" class="table table-training-services js-table-training-programs">   
		<thead>    
			<tr>     
				<th>Skills training Course Title</th>
				<th>Third Party Service Provider</th>
				<th>Start Date</th>
				<th>End Date</th>
			</tr>   
		</thead>   
		<tbody>    
		@foreach (var trainingProgram in Model.TrainingPrograms.Where(tp => tp.EligibleCostBreakdown.IsEligible)) {
			<tr>      
				<td>@(trainingProgram.CourseTitle)</td>      
				<td>@(trainingProgram.TrainingProvider.Name)</td>      
				<td>@(trainingProgram.StartDate.ToStringLocalTime())</td>      
				<td>@(trainingProgram.EndDate.ToStringLocalTime())</td>     
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
		@foreach (var EligibleCost in Model.TrainingCost.EligibleCosts.Where(ec => ec.AgreedMaxCost > 0 && ec.EligibleExpenseType.ServiceCategory.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports)) {     
			<tr>      
				<td>@(EligibleCost.EligibleExpenseType.Caption)</td>      
				<td>       
				@foreach (var TrainingProvider in EligibleCost.TrainingProviders) {        
					@(TrainingProvider.Name)<br />       
				}      
				</td>      
				<td>       
				@foreach (var Breakdown in EligibleCost.Breakdowns) {        
					@(Breakdown.EligibleExpenseBreakdown.Caption)<br />       
				}      
				</td>     
			</tr>   
		}   
		</tbody>  
	</table>  <br />  
	<div class="form__group two-col">   
		<label class="form__label schedule-a__label" for="GrantAgreement_ParticipantReportingDueDate">Participant Reporting Due Date:</label>   
		<div class="form__control schedule-a__control">@(Model.GrantAgreement.ParticipantReportingDueDate.ToLocalTime().ToString("MMMM dd, yyyy"))</div>  
	</div>  
	<div class="form__group two-col">   
		<label class="form__label schedule-a__label" for="GrantAgreement_ReimbursementClaimDueDate">Number of Participants in Program:</label>   
		<div class="form__control schedule-a__control">    
			@if (Model.TrainingCost.AgreedParticipants > 0) {     
				@Model.TrainingCost.AgreedParticipants    
			} else {     
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
				<th class="num-col">Number of participants</th>     
				<th class="num-col">Average Cost per Participant</th>     
				<th class="num-col">Total Maximum of Eligible Costs</th>     
				@if (showEmployerContribution) {      
					<th class="num-col">@(programType == ProgramTypes.WDAService ? "Applicant contribution" : "Employer contribution")</th>     
				}     
				<th class="num-col">Requested Government Contribution</th>    
			</tr>   
		</thead>   
		<tbody>    
			@{     
				var OrderedList = Model.TrainingCost.EligibleCosts.Where(ec => ec.AgreedMaxCost > 0).OrderBy(x => x.Id);     
				var sumESSAverageCost = @OrderedList.Where(x => x.EligibleExpenseType.ServiceCategory.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports).Sum(x => showAgreedCosts ? x.AgreedMaxParticipantCost : x.EstimatedParticipantCost);    
			}    

			@foreach (var expenseItem in OrderedList) {     
				if (showAgreedCosts) {     
					<tr data-expense-id="@expenseItem.EligibleExpenseType.Id" data-expense-item-id="@expenseItem.Id" data-total="@expenseItem.AgreedMaxCost.ToString()" data-participants="@expenseItem.AgreedMaxParticipants.ToString()">       
						<td>@(expenseItem.EligibleExpenseType.Caption)</td>       
						<td class="num-col">@((expenseItem.EligibleExpenseType.ServiceCategory != null && expenseItem.EligibleExpenseType.ServiceCategory.ServiceTypeId == ServiceTypes.Administration) ? "" : expenseItem.AgreedMaxParticipants.ToString())</td>       
						<td class="num-col">@((expenseItem.EligibleExpenseType.ServiceCategory != null && expenseItem.EligibleExpenseType.ServiceCategory.ServiceTypeId == ServiceTypes.Administration) ? "" : ToCurrency(expenseItem.AgreedMaxParticipantCost))</td>       
						<td class="num-col">@(ToCurrency(expenseItem.AgreedMaxCost))</td>       
						@if (showEmployerContribution) {        
							<td class="num-col">@(ToCurrency(expenseItem.AgreedEmployerContribution))</td>
						}       
						<td class="num-col">@(ToCurrency(expenseItem.AgreedMaxReimbursement))</td>      
					</tr>   
					if (expenseItem.EligibleExpenseType.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining) {
						foreach (var breakdown in expenseItem.Breakdowns.Where(b => b.IsEligible)) {
							var program = Model.TrainingPrograms.First(tp => tp.EligibleCostBreakdownId == breakdown.Id);
							<tr class="srow">         
								<td class="scolumn"><text>> </text> @(program.CourseTitle)</td>         
								<td class="scolumn"></td>         
								<td class="scolumn"></td>         
								<td class="scolumn">@(ToCurrency(program.EligibleCostBreakdown.AssessedCost))</td>         
								@if (showEmployerContribution) {          
									<td class="scolumn"></td>
								}         
								<td class="scolumn"></td>        
							</tr>   
						} 
					}    
				} else {      
					<tr data-expense-id="@expenseItem.EligibleExpenseType.Id" data-expense-item-id="@expenseItem.Id" data-total="@expenseItem.EstimatedCost.ToString()" data-participants="@expenseItem.EstimatedParticipants.ToString()">       
						<td>@(expenseItem.EligibleExpenseType.Caption)</td>       
						<td class="num-col">@(expenseItem.EstimatedParticipants.ToString())</td>       
						<td class="num-col">@(ToCurrency(expenseItem.EstimatedParticipantCost))</td>       
						<td class="num-col">@(ToCurrency(expenseItem.EstimatedCost))</td>       
						@if (showEmployerContribution) {        
							<td class="num-col">@(ToCurrency(expenseItem.EstimatedEmployerContribution))</td>
						}       
						<td class="num-col">@(ToCurrency(expenseItem.EstimatedReimbursement))</td>      
					</tr>     
				}    
			}    
			<tr>     
				<td><b>Total Maximum of Eligible Costs</b></td>     
				<td></td>     
				<td></td>     
				<td class="num-col"><b>@(ToCurrency(Model.TrainingCost.TotalAgreedMaxCost))</b></td>     
				@if (showEmployerContribution) {      
					<td class="num-col"><b>@(ToCurrency(Model.CalculateAgreedEmployerContribution()))</b></td>     
				}     
				<td class="num-col"><b>@(ToCurrency(Model.CalculateAgreedMaxReimbursement()))</b></td>    
			</tr>  
		</tbody>  
	</table>  
	@if (programType == ProgramTypes.WDAService) {   
		var includedServices = String.Join(" and ", Model.TrainingCost.EligibleCosts.Where(ec => ec.EligibleExpenseType.ServiceCategory.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports && ec.AgreedMaxCost > 0).Select(ec => ec.EligibleExpenseType.ServiceCategory.Caption));   
		if (!String.IsNullOrWhiteSpace(includedServices)) {    
			<p class="schedule-a-ess-sum">@includedServices total maximum blended cost per Participant is @(ToCurrency(sumESSAverageCost))</p>   
		}  
	} 
</div>'
WHERE Id = 13
