-- Agreement CoverLetter
PRINT 'INSERT DEFAULT CJG AGREEMENT COVER LETTER TEMPLATE'

INSERT INTO [dbo].[DocumentTemplates] ([DocumentType], [Title], [Body], [IsActive], [DateAdded], [DateUpdated]) VALUES (
	1
	, N'Default CJG Applicant Cover Letter Template'
	, N'@model CJG.Core.Entities.GrantApplication
	<div style="float: left;">@CJG.Core.Entities.AppDateTime.UtcNow.ToString("MMMM dd, yyyy"))</div>
	<div style="float: right;">Application# @Model.FileNumber</div>
	<br clear="all" />
	<br />@string.Format("{0} {1}", Model.ApplicantFirstName, Model.ApplicantLastName)
	<br />@Model.Organization.LegalName
	<br />@Model.ApplicantPhysicalAddress
	<br />
	<br />
	<p>Attention:</p>
	<p>Based upon your application for funding under the @Model.GrantOpening.GrantStream.GrantProgram.Name
		<strong>("@Model.GrantOpening.GrantStream.GrantProgram.ProgramCode")</strong>, we are pleased to inform you that we have approved the Training Provider, Training and Eligible Training
		Costs set out in Schedule A &ndash; Grant Services. Please refer to Schedule B for additional defined terms used in this
		<strong>"Approval Letter"</strong> (Schedule B governs if there is an inconsistency between provisions).</p>

	<p>When you have accepted all parts of this Agreement, the following provisions of this Approval Letter, along with the Schedules
		A &ndash; Grant Services (the Approved Training, Training Provider and Eligible Training Costs) and B &ndash; Definitions and General
		Terms, will form a Shared Cost Agreement between you and the Province. When the Agreement is in place, your primary obligations
		and the grant reimbursement claim process will be as follows:</p>
	<ol class="list--numbered">
		<li>Subject to the terms of this Agreement, we will reimburse you for up to @Model.ReimbursementRateInText of the Eligible Training Costs,
			to the amount shown in Schedule A ("Reimbursement").</li>
		<li>You must submit a claim for reimbursement of Eligible Training Costs using the
			<a href="@Model.BaseURL">Skills Training Grants</a> system at
			<a href="@Model.BaseURL">SkillsTrainingGrants.gov.bc.ca</a>, along with copies of all receipts and any Reports or other supporting documentation
			reasonably required by us. This Reimbursement Claim may be submitted as soon as Training starts and must be received by
			us no later than 30 days after start of the Training Program set out in Schedule A &ndash; Grant Services. If a Reimbursement
			Claim is not received within the 30 day period, you will no longer be eligible for a grant.</li>
		<li>You must arrange and pay for the Training in full, and the Participants must start their Training, in accordance with Schedule
			A &ndash; Grant Services, before a Reimbursement Claim can be submitted.</li>
		<li>Participants must be at least 15 years old and be Canadian Citizens, Permanent Residents or Protected Persons entitled to
			work in Canada. You are responsible to ensure that participants submit Participant Information Forms using the
			<a href="@Model.BaseURL">Skills Training Grants</a> system. If a Participant is found to be ineligible, payment will be prorated for eligible Participants
			only. Reimbursement payments will normally be made within 60 days following receipt of the Reimbursement Claim. Any costs
			or expenses that are not reimbursed by us will remain your responsibility as part of the Employer Contribution.</li>
		<li> All or any part of the Reimbursement paid to you may later be deemed by us to be an Overpayment if:
			<ol class="list--lettered">
				<li>the Participant does not complete their training, or the training is not completed by the end of the Term;</li>
				<li>you do not employ the Participant following the completion of their training;</li>
				<li>your Reimbursement Claim included any items that were not Eligible Training Costs;</li>
				<li>you fail to provide any Reports or other information that we require;</li>
				<li>you receive funding from any other person or entity, including another government or governmental body, that reimburses
					you for the portion of the Training that is the subject of this Agreement or you receive any form of refund from the
					Training provider.</li>
			</ol>
		</li>
		<li>Any outstanding Overpayments will become a debt owing to the Province and must be repaid by you within 14 days.</li>
		<li> You are responsible for:
			<ol class="list--lettered">
				<li>any Training costs (including Eligible Training Costs) beyond the maximum amount reimbursed by the Province under all
					Agreements (capped at @Model.MaxReimbursementAmt.ToString("C0") per Participant per Fiscal Year);</li>
				<li>any Training costs that are at any time deemed ineligible, due to Employer, Participant, Training or Training provider
					ineligibility; and </li>
				<li>any Eligible Training Costs not submitted to the Province as part of the Reimbursement Claim within the 30 day claim period
					following the start of each Training program set out in Schedule A.</li>
			</ol>
		</li>
		<li>No Reimbursement will be made until the Province has received a complete Reimbursement Claim.</li>
		<li>In addition to the Reimbursement Claim, you will be required to submit an "Employer Completion Report" at the end of training.
			</li>
		<li>Reimbursement of Eligible Training Costs is subject to audit and verification by the Province at any time and original receipts
			and/or proof of expenditure records must be kept by you and made available for review for a minimum period of seven years.</li>
		<li>You must also comply with all other parts of this Agreement (including Schedule B), the Program Requirements and all applicable
			laws.</li>
	</ol>
	<p>If after electronically signing the Agreement, you want to make a change to the Training Provider, you must use the
		<a href="@Model.BaseURL">Skills Training Grants</a> system to submit a change request to the Province. Your Training Provider change request must
		be approved by the Province before Training begins. See the Agreement Schedule A &ndash; Grant Services in the system for how
		to submit a change request. </p>
	<p>Training Start and end dates may be modified in the
		<a href="@Model.BaseURL">Skills Training Grants</a> system without prior approval of the Province as long as Training starts within the approved
		Training period and no other material changes are made to the Training program or Training provider. The Training start
		date cannot be moved outside of the Training period for which the Training was approved.</p>
	<p>If, after you have electronically signed the Agreement, you want to make any other changes to the Training, you must first
		contact the Province.</p>
	<p>If, before electronically signing the Agreement, you do not intend to proceed with the Training, you may reject this Agreement
		(by clicking Cancel below and then Reject Agreement).</p>
	<p>If the terms of this Agreement are acceptable to you and you wish to proceed with the Training, you must first review and
		confirm you have read and understand each section of the Agreement including Schedules A and B. Then, in order to obtain
		a grant, you must accept by electronically signing the Agreement using the Accept Agreement button on the Review and Accept
		Grant Agreement page. If we do not receive your acceptance of the Agreement by way of your electronic signature by the date
		set out in Schedule A &ndash; Grant Services, this Agreement will expire.</p>
	<p>Thank you for your participation in the B.C. Employer Training Grant program. The Province of British Columbia introduced
		the B.C. Employer Training Grant to help better align skills training with employer needs, create more jobs for Canadians
		and build a stronger, more skilled workforce. Employers are seen as partners in the skills training system. By sharing in
		the associated costs, you are demonstrating your commitment to the continued success of the Canadian economy. We are interested
		in your feedback and would appreciate learning from your experience with the program. Please contact us with any questions
		or concerns using the Contact US link below.</p>
	<p>Sincerely,</p> Director
	<br /> B.C. Employer Training Grant Program
	<br />
	<br />
	<p>I am authorized to act and to enter into this Agreement on behalf of the Employer. On the Employer''s behalf, I do hereby
		accept and agree to the terms and conditions of this Agreement, including this Approval Letter and the attached Schedules
		A - Grant Services (Approved Training and Eligible Training Costs) and Schedule B - Definitions and General Terms.</p>
	<p>Note: If the Employer''s signing authority is also a Participant under this Agreement but is not an owner or co-owner of the
		business, he/she cannot sign on the Employer''s behalf. In this case, please ensure an alternate signing authority signs
		this Agreement on behalf of the Employer.</p>'
	, 1
	, SYSDATETIME()
	, SYSDATETIME());

PRINT 'UPDATE CJG PROGRAM DEFAULT TEMPLATE'
UPDATE [dbo].[GrantPrograms]
SET [ApplicantCoverLetterTemplateId] = @@IDENTITY
Where Id = 1;

-- Agreement Schedule A
PRINT 'INSERT DEFAULT CJG AGREEMENT SCHEDULE A TEMPLATE'
INSERT INTO [dbo].[DocumentTemplates] ([DocumentType], [Title], [Body], [IsActive], [DateAdded], [DateUpdated]) VALUES (
	2
	, N'Default CJG Applicant Schedule A Template'
	, N'@model CJG.Core.Entities.GrantApplication
	@using CJG.Core.Entities
	@using System.Globalization
	@{
		var showAgreedCosts = Model.TrainingPrograms.FirstOrDefault().GrantApplication.ApplicationStateInternal.ShowAgreedCosts();
	}

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
				@Model.Organization.LegalName
			</div>
		</div>
		<p><h3>Agreement Term</h3></p>
		<div class="form__group two-col">
			<label class="form__label" for="GrantAgreement_StartDate">Start Date:</label>
			<div class="form__control">
				@(Model.GrantAgreement.StartDate.ToLocalTime().ToString("MMMM dd, yyyy"))
			</div>
		</div>
		<div class="form__group two-col">
			<label class="form__label" for="GrantAgreement_EndDate">End Date:</label>
			<div class="form__control">
				@(Model.GrantAgreement.EndDate.ToLocalTime().ToString("MMMM dd, yyyy"))
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
					@Model.TrainingPrograms.FirstOrDefault().TrainingProvider.Name
				</div>
			</div>
			::RequestChangeTrainingProvider::
		</div>
		<div class="form__group__wrapper ">
			<div class="form__group two-col">
				<label class="form__label" for="DefaultTrainingProgram_StartDate">Training Start Date:</label>
				<div class="form__control">
					@Model.TrainingPrograms.FirstOrDefault().StartDate.ToLocalTime().ToString("MMMM dd, yyyy")
				</div>
			</div>
      
			::RequestChangeTrainingDates::
		</div>
		<div class="form__group two-col">
			<label class="form__label" for="DefaultTrainingProgram_EndDate">Training End Date:</label>
			<div class="form__control">
				@Model.TrainingPrograms.FirstOrDefault().EndDate.ToLocalTime().ToString("MMMM dd, yyyy")
			</div>
		</div>
		<div class="form__group two-col">
			<label class="form__label" for="GrantAgreement_ParticipantReportingDueDate">Participant Reporting Due Date:</label>
			<div class="form__control">
				@(Model.GrantAgreement.ParticipantReportingDueDate.ToLocalTime().ToString("MMMM dd, yyyy"))
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
				@(Model.GrantAgreement.ReimbursementClaimDueDate.ToLocalTime().ToString("MMMM dd, yyyy"))
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
					@Model.TrainingPrograms.FirstOrDefault().EstimatedParticipants
				}
			</div>
		</div>
		@* hide the edit/delete buttons when rendering to the application administrator *@
		<h3>Eligible Training Costs</h3>
		<table id="tblExpenses" class="table table--expenses js-table-expense-list">
			<thead>
				<tr>
					<th>Expense type</th>
					<th class="num-col">Number of participants</th>
					<th class="num-col">Cost per participant</th>
					<th class="num-col">Total training cost</th>
					<th class="num-col">Employer contribution</th>
					<th class="num-col">Requested Government Contribution</th>
				</tr>
			</thead>

			<tbody>
				@{
					var OrderedList = Model.EligibleCosts.Where(ec => ec.AgreedMaxCost > 0).OrderBy(x => x.TrainingProgram.Id);
				}

				@foreach (var expenseItem in OrderedList)
				{
					if (showAgreedCosts)
					{
						<tr data-expense-id="@expenseItem.EligibleExpenseType.Id"
							data-expense-item-id="@expenseItem.Id"
							data-total="@expenseItem.AgreedMaxCost.ToString()"
							data-participants="@expenseItem.AgreedMaxParticipants.ToString()">
							<td>@(expenseItem.EligibleExpenseType.Caption)</td>
							<td class="num-col">@(expenseItem.AgreedMaxParticipants.ToString())</td>
							<td class="num-col">@(ToCurrency(expenseItem.AgreedMaxParticipantCost))</td>
							<td class="num-col">@(ToCurrency(expenseItem.AgreedMaxCost))</td>
							<td class="num-col">@(ToCurrency(expenseItem.AgreedEmployerContribution))</td>
							<td class="num-col">@(ToCurrency(expenseItem.AgreedMaxReimbursement))</td>
						</tr>
					}
					else
					{
						<tr data-expense-id="@expenseItem.EligibleExpenseType.Id"
							data-expense-item-id="@expenseItem.Id"
							data-total="@expenseItem.EstimatedCost.ToString()"
							data-participants="@expenseItem.EstimatedParticipants.ToString()">
							<td>@(expenseItem.EligibleExpenseType.Caption)</td>
							<td class="num-col">@(expenseItem.EstimatedParticipants.ToString())</td>
							<td class="num-col">@(ToCurrency(expenseItem.EstimatedParticipantCost))</td>
							<td class="num-col">@(ToCurrency(expenseItem.EstimatedCost))</td>
							<td class="num-col">@(ToCurrency(expenseItem.EstimatedEmployerContribution))</td>
							<td class="num-col">@(ToCurrency(expenseItem.EstimatedReimbursement))</td>
						</tr>
					}
				}
				<tr>
					<td><b>Totals</b></td>
					<td></td>
					<td></td>
					<td class="num-col">
						<b>@(ToCurrency(Model.TrainingPrograms.FirstOrDefault().TotalAgreedMaxCost))</b>
					</td>
					<td class="num-col">
						<b>@(ToCurrency(Model.TrainingPrograms.FirstOrDefault().CalculateAgreedEmployerContribution()))</b>
					</td>
					<td class="num-col">
						<b>@(ToCurrency(Model.TrainingPrograms.FirstOrDefault().CalculateAgreedMaxReimbursement()))</b>
					</td>
				</tr>
			</tbody>
		</table>
	</div>'
	, 1
	, SYSDATETIME()
	, SYSDATETIME());

PRINT 'UPDATE CJG PROGRAM DEFAULT TEMPLATE'
UPDATE [dbo].[GrantPrograms]
SET [ApplicantScheduleATemplateId] = @@IDENTITY
Where Id = 1;

-- Agreement Schedule B
PRINT 'INSERT DEFAULT CJG APPLICANT SCHEDULE B TEMPLATE'
INSERT INTO [dbo].[DocumentTemplates] ([DocumentType], [Title], [Body], [IsActive], [DateAdded], [DateUpdated]) VALUES (
	3
	, N'Default CJG Applicant Schedule B Template'
	, N'@model CJG.Core.Entities.GrantApplication
<h4>Schedule B - Definitions and General Terms</h4>
In this Agreement, including the Approval Letter:<br />
<ul class="list--nostyle">
    <li>&ldquo;<b>Canadian Citizen</b>&rdquo; means a person who has Canadian citizenship status by birth or as granted by Canada;</li>
    <li>&ldquo;<b>Eligible Training Costs</b>&rdquo; means tuition fees charged by a third-party training provider, mandatory student fees, examination fees and costs of textbooks, software and other materials required for training, that are actually incurred by the Employer for the Training, up to the maximum amount set out for each Participant in Schedule A;</li>
    <li>&ldquo;<b>Employer</b>&rdquo;, &ldquo;<b>you</b>&rdquo; or &ldquo;<b>your</b>&rdquo; means the employer, or organization acting on behalf of employers, that applied for the Training and to whom the Province''s Approval Letter has been addressed;</li>
    <li>&ldquo;<b>Employer Contribution</b>&rdquo; means the total amount of all Training related expenses, less the amount of any Eligible Training Costs payable by the Province to the Employer, in accordance with this Agreement;</li>
    <li>&ldquo;<b>Fiscal Year</b>&rdquo; means April 1 to March 31 of the following year;</li>
    <li>&ldquo;<b>Overpayment</b>&rdquo; means any and all amounts provided by the Province to the Employer that are not expended during the Term on Eligible Training Costs;</li>
    <li>&ldquo;<b>Participant</b>&rdquo; means each individual set out in Schedule A for whom the Employer has applied, and the Province has approved, to receive Training;</li>
    <li>&ldquo;<b>Parties</b>&rdquo; means collectively the Employer and the Province;</li>
    <li>&ldquo;<b>Party</b>&rdquo; means either the Employer or the Province, as the context requires;</li>
    <li>&ldquo;<b>Permanent Resident</b>&rdquo; means a person who has legally immigrated to Canada but is not yet a Canadian citizen. This person has acquired permanent resident status and has not subsequently lost that status under section 46 of the <i>Immigration and Refugee Protection Act</i>.</li>
    <li>&ldquo;<b>Protected Persons</b>&rdquo; means a person who has been determined to be a Convention refugee or person in similar circumstances by a Canadian visa officer outside Canada, a person whom the Immigration and Refugee Board of Canada has determined to be a Convention refugee or in need of protection in Canada, or a person who has had a positive pre-removal risk assessment (in most cases). </li>
    <li>&ldquo;<b>Program Requirements</b>&rdquo; means the general principles, criteria, requirements and Participant and Employer expectations and obligations relating to the Program and the Training, as may be amended by the Province in its sole discretion from time to time, which can be viewed via <a href="https://www.workbc.ca/Employer-Resources/Canada-BC-Job-Grant/What-is-the-Canada-B-C-Job-Grant.aspx" target="_blank">About @Model.GrantOpening.GrantStream.GrantProgram.Name</a>  or such other location as we may specify from time to time;</li>
    <li>&ldquo;<b>Province</b>&rdquo;, &ldquo;<b>we</b>&rdquo;, &ldquo;<b>us</b>&rdquo; or &ldquo;<b>our</b>&rdquo; means Her Majesty the Queen in right of the Province of British Columbia, represented by the Minister of Advanced Education, Skills and Training;</li>
    <li>&ldquo;<b>Reports</b>&rdquo; means, collectively, any and all reports or additional information at any time required under the Program Requirements or this Agreement to be provided to the Province by the Employer or by any Participant; </li>
    <li>&ldquo;<b>Term</b>&rdquo; means the period that begins upon the return of a signed copy of the Approval Letter by the Employer to the Province and ends on the end date set out in Schedule A or March 31, 2020, whichever is earlier; and</li>
    <li>&ldquo;<b>Training</b>&rdquo; means the approved training opportunity for each Participant set out in Schedule A.</li>
</ul>
<h4>Employer Obligations</h4>
<p><b>1.</b> &nbsp; In addition to your other obligations set out elsewhere in this Agreement, including the Approval Letter, you must:</p>
<ol class="list--lettered">
    <li>only request reimbursement for Eligible Training Costs that have been incurred by you;</li>
    <li>upon our request, promptly inform us regarding Participants'' Training and employment status and provide any additional information we may reasonably require;</li>
    <li>comply with any &ldquo;Marketing, Publicity and Communications Guidelines&rdquo; regarding any public announcements that you make with respect to your participation in the Program, the Training and your acknowledgment of funding received for the Training from the Province and the Government of Canada. Prior to making any such public announcements, you must obtain the current &ldquo;Marketing, Publicity and Communications Guidelines&rdquo; by email request to <a href="mailto:@(Model.GrantOpening.GrantStream.GrantProgram.ProgramCode)info@gov.bc.ca">@(Model.GrantOpening.GrantStream.GrantProgram.ProgramCode)info@gov.bc.ca</a> or such other address as we may specify from time to time; </li>
    <li>submit all Employer Reports and collect and submit all Participant Reports required by us;</li>
    <li>immediately provide us with full and complete details regarding any Training funding that you receive or anticipate receiving from any other person or entity, including another government or governmental body; and</li>
    <li>
        establish and maintain complete and accurate accounting and administrative records with respect to the Participants, the Training, the Eligible Training Costs, the Employer Contribution and any Overpayments, including any original receipts and other supporting documentation, in form and content satisfactory to us, and you must keep and make those records available to us upon request for a period of at least seven years following the completion of the Training. <br><br>
        We reserve the right to contact Participants, trainers or any other person in order to substantiate reimbursement requests, Training activities, records or other matters pertaining to your obligations or your participation in the Program.
    </li>
</ol>
<h4>Holdback and Set-Off</h4>
<p><b>2.</b> &nbsp; We may temporarily withhold or set-off from any payment to you the amount of any Overpayment.</p>
<h4>Relationship and Conflict of Interest</h4>
<p><b>3.</b> &nbsp; Even though both Parties have certain obligations relating to Training costs, you are an independent contractor and no partnership, joint venture, agency or other legal entity or structure will be created or deemed to be created by this Agreement or any actions of the Parties under this Agreement.</p>
<p><b>4.</b> &nbsp; You must not in any way commit or purport to commit the Province to the payment of money to any person or entity.</p>
<p><b>5.</b> &nbsp; During the Term, you must not perform a service for, or provide advice to, any person or entity where the performance of such service or the provision of the advice may, in our reasonable opinion, give rise to a conflict of interest. You must also not, without our consent, use any person or entity to deliver Training where that delivery of Training may give rise to an actual or a perceived conflict of interest.</p>
<h4>Assignment and Subcontracting</h4>
<p><b>6.</b> &nbsp; You will not, without our prior written consent, either directly or indirectly assign this Agreement or any of your rights under this Agreement.</p>
<p><b>7.</b> &nbsp; No sub-contract or Training contract entered into by you will relieve you from any of your obligations under this Agreement or impose upon the Province any obligation or liability arising from any such sub-contract or Training contract.</p>
<p><b>8.</b> &nbsp; This Agreement will be binding upon and enure to the benefit of the Province and its assigns and you and your successors and permitted assigns.</p>
<h4>Representations and Warranties</h4>
<p><b>9. </b> &nbsp; You represent and warrant that:</p>
<ol class="list--lettered">
    <li>you have the legal capacity to enter into and to fulfil your obligations under this Agreement;</li>
    <li>you have no knowledge of any fact that materially adversely affects, or so far as you can foresee, might materially adversely affect, your properties, assets, condition (financial or otherwise), business or operations or your ability to fulfil your obligations under this Agreement;</li>
    <li>all information, statements, documents and reports at any time provided by you in connection with this Agreement are, will be and will remain, true and correct; and</li>
    <li>you are not in breach of, or in default under, any law, statute or regulation of Canada or of the Province of British Columbia that is relevant to the Training, the Program or the subject matter of this Agreement.</li>
</ol>
<h4>Default and Termination</h4>
<p><b>10. </b> &nbsp; If you fail to comply with any provision of this Agreement, or if any representation or warranty made by you is or becomes untrue or incorrect, or if, in our opinion, you cease to operate or if a change occurs with respect to any one or more of your properties, assets, condition (financial or otherwise), business or operations which, in our opinion, materially adversely affects your ability to fulfil your obligations under this Agreement (each a &ldquo;<b>Default</b>&rdquo;), then we may do any one or more of the following:</p>
<ol class="list--lettered">
    <li>waive the Default;</li>
    <li>require you to remedy the Default within a time period specified by us;</li>
    <li>suspend any reimbursement of Eligible Training Costs or any other amount that is due to you while the Default continues;</li>
    <li>terminate this Agreement, in which case the payment of the amount required under paragraph 13 will discharge us of all liability to you under this Agreement; or</li>
    <li>pursue any other remedy available to us at law or in equity.</li>
</ol>
<p><b>11. </b> &nbsp; Either Party may terminate this Agreement by giving the other Party at least 30 days'' written notice.</p>
<p><b>12. </b> &nbsp; We may also terminate this Agreement, with immediate effect, if we determine that any action or inaction by you places the health or safety of any person at immediate risk.</p>
<p><b>13. </b> &nbsp; In the event that this Agreement is terminated by either Party, we will pay the Eligible Training Costs in respect of any Training completed on or before the effective date of termination, less any outstanding Overpayment, which will discharge us of all liability to you under this Agreement.</p>
<h4>Indemnity</h4>
<p><b>14. </b> &nbsp; You will indemnify and save harmless the Province, its employees, agents and contractors, from and against any and all losses, claims, damages, actions, causes of action, costs and expenses that it or they may sustain, incur, suffer or be put to at any time either before or after the expiration or termination of this Agreement, where the same or any of them are based upon, arise out of or occur, directly or indirectly, by reason of any act or omission by you or by any of your agents, employees, officers, directors or sub-contractors pursuant to this Agreement, excepting always liability arising out of the independent negligent acts of the Province.</p>
<h4>Delivery and Notice</h4>
<p><b>15. </b> &nbsp; In order to be effective, this Agreement and any legal notice required by this Agreement (&ldquo;<b>Notice</b>&rdquo;) must be in writing and delivered as follows:</p>
<ol class="list--lettered">
    <li>
        to you, to your mailing address or email address as shown in the Approval Letter
        <ol class="list--numerals">
            <li>by hand (including by courier), in which case it will be deemed to be received on the day of its delivery, or</li>
            <li>by prepaid mail, in which case it will be deemed to be received on the fifth business day after its mailing; or</li>
        </ol>
    </li>
    <li>
        to us:
        <ol class="list--numerals">
            <li>
                by hand (including by courier), in which case it will be deemed to be received on the day of its delivery, to:<br />
                &nbsp;  &nbsp; <span style="text-decoration: underline;">2nd Floor, 1106 Cook Street, Victoria, BC, V8V 3Z9</span>
            </li>
            <li>
                by prepaid mail, in which case it will be deemed to be received on the fifth business day after its mailing, to:<br />
                &nbsp;  &nbsp; <span style="text-decoration: underline;">PO Box 9189, Stn Prov Govt, Victoria, BC, V8W 9E6</span>
            </li>
        </ol>
    </li>
</ol>
<p><b>16. </b> &nbsp; Either Party may, from time to time, notify the other Party in writing of a change of address for delivery and, following the receipt of such Notice in accordance with paragraph 15, the new address will, for the purposes of paragraph 15, be deemed to be the delivery address of the Party giving Notice.</p>
<p><b>17. </b> &nbsp; The Parties may execute and deliver separate copies (counterparts) of this Agreement and each executed and delivered counterpart will be considered to be an original. These counterparts may be delivered by email (with a scanned copy attached in PDF format) to an email address provided by the other Party for such purpose.</p>
<h4>Miscellaneous Terms</h4>
<p><b>18. </b> &nbsp; The Schedules to this Agreement are an integral part of this Agreement as if included in the body of this Agreement.</p>
<p><b>19. </b> &nbsp; This Agreement together with any documents or other information referred to in it, including the Approval Letter and the Program Requirements, constitute the entire Agreement between the Parties with respect to the subject matter of this Agreement. </p>
<p><b>20. </b> &nbsp; In this Agreement, unless the context requires otherwise, words using the singular form include the plural form and <i>vice versa</i>.</p>
<p><b>21. </b> &nbsp; The headings in this Agreement are inserted for convenience only and do not form part of this Agreement.</p>
<p><b>22. </b> &nbsp; No amendment to or modification of this Agreement will be effective unless it is in writing and signed by both Parties.</p>
<p><b>23. </b> &nbsp; Nothing in this Agreement operates as a consent, permit, approval or authorization by any Ministry or Branch of the Government of the Province of British Columbia for anything that, by statute, you are required to obtain, unless this Agreement expressly indicates otherwise. </p>
<p><b>24. </b> &nbsp; No term or condition of this Agreement and no breach of any such term or condition by you will be deemed to have been waived unless such waiver is in writing and signed by both Parties.</p>
<p><b>25. </b> &nbsp; Our written waiver of any breach by you of a term or condition of this Agreement will not be deemed to be a waiver of any other provision of this Agreement or of any prior or subsequent breach.</p>
<p><b>26. </b> &nbsp; This Agreement will be governed by and construed in accordance with the laws of the Province of British Columbia and the laws of Canada applicable therein.</p>
<p><b>27. </b> &nbsp; The courts of the Province of British Columbia, sitting in Victoria, will have the exclusive jurisdiction to hear any disputes arising from or in any way related to this Agreement or the relationship of the Parties.</p>
<p><b>28. </b> &nbsp; If any provision of this Agreement or its application to any person or circumstance is found by a court of competent jurisdiction to be invalid or unenforceable to any extent, the remainder of this Agreement and the application of that provision to any other person or circumstance will not be affected or impaired and will be enforceable to the extent permitted by law.</p>
<p><b>29. </b> &nbsp; The provisions of the Approval Letter and paragraphs 1 - 4, 7 - 9, 13, 14 and 18 - 29 of this schedule and any other provision(s) or other section(s) of this Agreement (including this schedule) or the Program Requirements which, by their terms or nature, are intended to survive the completion or termination of this Agreement or are necessary for the interpretation or enforcement of this Agreement, will continue in force indefinitely subject to any applicable limitation period prescribed by law, even after this Agreement ends.</p>
<br />'
	, 1
	, SYSDATETIME()
	, SYSDATETIME());

PRINT 'UPDATE CJG PROGRAM DEFAULT TEMPLATE'
UPDATE [dbo].[GrantPrograms]
SET [ApplicantScheduleBTemplateId] = @@IDENTITY
Where Id = 1;

-- Applicant Declaration
PRINT 'INSERT DEFAULT CJG APPLICANT DECLARATION TEMPLATE'
INSERT INTO [dbo].[DocumentTemplates] ([DocumentType], [Title], [Body], [IsActive], [DateAdded], [DateUpdated]) VALUES (
	4
	, N'Default CJG Applicant Declaration Template'
	, N'@model CJG.Core.Entities.GrantApplication
		<li>I certify that I am authorized to submit this Application and to make this declaration on behalf of the applicant referred to in this Application (the "Applicant");</li>
		<li>I acknowledge that I have read and understand the @(Model.GrantOpening.GrantStream.GrantProgram.ProgramCode) criteria applicable to this Application, including the sample @(Model.GrantOpening.GrantStream.GrantProgram.ProgramCode) Agreement, consisting of the Approval Letter, Schedule A and Schedule B (and the Program Requirements referred to therein), as made available by the Province of British Columbia at the link below;</li>
		<li>I acknowledge that, as the terms and conditions of the @(Model.GrantOpening.GrantStream.GrantProgram.ProgramCode) Agreement are subject to change from time to time, should this Application be approved, the @(Model.GrantOpening.GrantStream.GrantProgram.ProgramCode) Agreement that will be sent to the Applicant for signature may materially differ from the sample @(Model.GrantOpening.GrantStream.GrantProgram.ProgramCode) Agreement that was posted at the time this Application was submitted and I acknowledge that I (or another individual authorized by the Applicant) will be responsible for reviewing, understanding and agreeing to the terms and conditions as they appear at the time the Applicant enters into a @(Model.GrantOpening.GrantStream.GrantProgram.ProgramCode) Agreement with the Province;</li>
		<li>I certify that all of the information provided on this Application is true and correct to the best of my knowledge and belief;</li>
		<li>I acknowledge and agree that checking the box below has the same legal effect as making this declaration under a hand-written signature; and </li>
		<li>I do hereby make this declaration on my own behalf and on behalf of the Applicant as of the date that this Application is submitted.</li>'
	, 1
	, SYSDATETIME()
	, SYSDATETIME());

PRINT 'UPDATE CJG PROGRAM DEFAULT TEMPLATE'
UPDATE [dbo].[GrantPrograms]
SET [ApplicantDeclarationTemplateId] = @@IDENTITY
Where Id = 1;

-- Participant Consent
PRINT 'INSERT DEFAULT CJG PARTICIPANT CONSENT TEMPLATE'
INSERT INTO [dbo].[DocumentTemplates] ([DocumentType], [Title], [Body], [IsActive], [DateAdded], [DateUpdated]) VALUES (
	5
	, N'Default CJG Participant Consent Template'
	, N'@model CJG.Core.Entities.GrantApplication
@{
    var program = Model.GrantOpening.GrantStream.GrantProgram;
}
<h2 class="subheader">Consent for Collection and Use of Personal Information</h2>
<p>
    All personal information collected here and by future surveys and testimonials and other information
    related to your participation in a @program.Name training opportunity ("Personal Information")
    is collected pursuant to sections 26(c) and 26(e) of the Freedom of Information and Protection of Privacy Act.
</p>

<p>
    This information will be used for administrative and evaluation purposes, including to determine your eligibility for participation in the @program.Name program,
    and, in the case of any Testimonial, may be used and disclosed to publicly promote the @program.Name program ("Purposes").
    The Government of Canada ("Canada") provides funding for the @program.Name under the Canada-British Columbia Job Fund Agreement.
    Personal Information will be used to create reports about @program.Name training in British Columbia that will be provided to Canada in aggregate form only.
    No personally identifiable information will be disclosed to Canada for this purpose.
    Personal information will be shared with BC Stats, BC''s central statistics agency, for the purpose of conducting outcome surveys with @program.Name participants.
    All information provided to BC Stats is protected under the BC Statistics Act and is kept confidential.
    If you have any questions about the use of this information, please contact the Program Manager, @program.Name at 1-877-952-6914, or by mail to:
</p>
<p class="well">
    Program Manager, @program.Name<br />
    2nd Floor, 1106 Cook Street<br />
    Victoria, BC, V8V 3Z9<br />
</p>
<p>
    Effective as of the date set out below, and in consideration of the opportunity for me to participate in @program.Name training, I certify that:
</p>
<ul>
    <li>all of the information that I will provide is accurate and complete;</li>
    <li>
        I understand that I am expected to complete my training and I must complete surveys in order for the employer to meet all of their obligations under their @program.Name Agreement; and,
    </li>
</ul>
<p>I consent to:</p>
<ul>
    <li>
        the collection and use of my Personal Information by the Province of British Columbia for purposes of accountability, quality assurance, research, and evaluation of the @program.Name; and,
    </li>
    <li>my Personal Information being used to contact me to conduct the Surveys and to request a Testimonial.</li>
</ul>'
	, 1
	, SYSDATETIME()
	, SYSDATETIME());

PRINT 'UPDATE CJG PROGRAM DEFAULT TEMPLATE'
UPDATE [dbo].[GrantPrograms]
SET [ParticipantConsentTemplateId] = @@IDENTITY
Where Id = 1;


