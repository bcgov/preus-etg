-- Agreement CoverLetter
PRINT 'UPDATE DEFAULT AGREEMENT COVER LETTER TEMPLATE'

UPDATE [dbo].[DocumentTemplates] 
SET [Body] = 
N'
@model CJG.Application.Business.Models.DocumentTemplate.GrantApplicationTemplateModel
<div style="float: left;">@CJG.Core.Entities.AppDateTime.UtcNow.ToString("MMMM dd, yyyy"))</div>
<div style="float: right;">Application# @Model.FileNumber</div>
<br clear="all" />
<br />@string.Format("{0} {1}", Model.ApplicantFirstName, Model.ApplicantLastName)
<br />@Model.OrganizationLegalName
<br />@Model.ApplicantPhysicalAddress
<br />
<br />
<p>Attention:</p>
<p>Based upon your application for funding under the @Model.GrantProgramName program,
	we are pleased to inform you that we have approved the Training Provider, Training and Eligible Training
	Costs set out in Schedule A &ndash; Grant Services (<strong>&ldquo;Schedule A&rdquo;</strong>). Please refer to Schedule B &ndash;
	Definitions and General Terms (<strong>&ldquo;Schedule B&rdquo;</strong>) for additional defined terms used in this
	<strong>&ldquo;Approval Letter&rdquo;</strong> (Schedule B governs if there is an inconsistency between provisions).</p>

<p>The following provisions of this Approval Letter, along with Schedule A and Schedule B,
	will form a Shared Cost Arrangement Agreement (the <strong>&ldquo;Agreement&rdquo;</strong>) between you and the Province. When the Agreement is in place, your primary obligations
	and the grant reimbursement claim process will be as follows:</p>
<ol class="list--numbered">
	<li>Subject to the terms of this Agreement, we will reimburse you for up to @Model.ReimbursementRate of the Eligible Training Costs,
		to the amount shown in Schedule A (<strong>&ldquo;Reimbursement&rdquo;</strong>).</li>
	<li>You must submit a claim for Reimbursement of Eligible Training Costs (<strong>&ldquo;Claim&rdquo;</strong>) using the
		<a href="@Model.BaseURL"><u>Skills Training Grants</u></a> system (the <strong>&ldquo;STG System&rdquo;</strong>) at
		<a href="@Model.BaseURL"><u>SkillsTrainingGrants.gov.bc.ca</u></a>, along with copies of all receipts and any Reports or other supporting documentation
		reasonably required by us. A Claim may only be submitted after Training starts and must be received by
		us no later than 30 days after start of the Program. If a Claim is not received within the 30 day period, you will no longer be eligible to receive a Reimbursement.</li>
	<li>You must arrange and pay for the Training in full for all Participants.</li>
	<li>Participants must be at least 15 years old at the time of your application for Training and be Canadian Citizens, Permanent Residents or Protected Persons entitled to
		work in Canada. You are responsible to ensure that Participants submit Participant Information Forms using the
		STG System. If a Participant is found to be ineligible, payment will be prorated for eligible Participants
		only. Reimbursement payments will normally be made within 60 days following receipt of the Claim. Any costs
		or expenses that are not reimbursed by us will remain your responsibility as part of the Employer Contribution.</li>
	<li>Should a Participant be required to directly pay for any Eligible Costs associated with the Program,
		you will be required to reimburse that Participant for those costs in full, even if they are later deemed to not be an Eligible Cost.</li>
	<li>All or any part of the Reimbursement paid to you may later be deemed by us, acting reasonably, to be an Overpayment if:
		<ol class="list--lettered dotted">
			<li>the Participant does not complete their Training, or the Training is not completed by the end of the Term;</li>
			<li>you do not employ the Participant following the completion of their Training;</li>
			<li>your Claim included any items that were not Eligible Training Costs or any Participant or Training Provider is found to be ineligible;</li>
			<li>you fail to provide any Reports or other information that we require;</li>
			<li>you receive funding from any other person or entity, including another government or governmental body, that reimburses
				you for the portion of the Training that is the subject of this Agreement or you receive any form of refund from the
				Training Provider.</li>
		</ol>
	</li>
	<li>Any outstanding Overpayments will become a debt owing to the Province and must be repaid by you within 14 days.</li>
	<li>You are responsible for:
		<ol class="list--lettered dotted">
			<li>any Training costs (including Eligible Training Costs) beyond the maximum amount reimbursed by the Province under all
				@Model.GrantProgramName agreements (capped at @Model.MaxReimbursementAmt per individual Participant per Fiscal Year);</li>
			<li>any Training costs that are at any time deemed ineligible, due to Employer, Participant, Training or Training Provider
				ineligibility; and </li>
			<li>any Eligible Training Costs not submitted to the Province as part of a Claim within the 30 day claim period
				following the start of each Program set out in Schedule A.</li>
		</ol>
	</li>
	<li>No Reimbursement will be made until the Province has received a complete Claim.</li>
	<li>In addition to the Claim, you will be required to submit an &ldquo;Employer Completion Report&rdquo; once all Participants have completed (or ceased) Training.</li>
	<li>Reimbursement of Eligible Training Costs is subject to audit and verification by the Province at any time and original receipts
		and/or proof of expenditure records must be kept by you and made available for review for a minimum period of seven years.</li>
	<li>You must also comply with all other parts of this Agreement (including Schedule B), the Program Requirements and all applicable laws.</li>
	<li>The Province reserves the right to contact Participants, Training Providers or any other person in order to substantiate Claim requests,
		Training activities, records or other matters pertaining to your obligations under or your participation in the Program.</li>
</ol>
<p>If, <u>before</u> electronically signing the Agreement, you do not intend to proceed with the Training, you may reject this Agreement (by clicking &ldquo;Cancel&rdquo; below and then &ldquo;Reject Agreement&rdquo;).</p>
<p>If <u>after</u> electronically signing the Agreement, you want to:
	<ol class="list--lettered dotted">
		<li><u>change the Training Provider</u>, you must use the STG System to submit a change request to the Province.
			Your Training Provider change request must be approved by the Province before Training begins;</li>
		<li><u>change the start and/or end of Training</u>, you may make these changes using the STG System
			without prior approval of the Province as long as Training starts within the approved
			Training period and no other material changes are made to the Program or Training Provider. The Training start
			date cannot be moved outside of the Training period for which the Training was approved; or</li>
		<li><u>make any other changes</u>, you must first contact the Province by email at:
		<a href="mailto:@(Model.GrantProgramCode)@("@gov.bc.ca")">@(Model.GrantProgramCode)@("@gov.bc.ca")</a>.</li>
	</ol>
</p>
<p>Change requests can be made in the STG System. For step-by-step instructions, email
	<a href="mailto:@(Model.GrantProgramCode)@("@gov.bc.ca")">@(Model.GrantProgramCode)@("@gov.bc.ca")</a>.</p>
<p>If the terms of this Agreement are acceptable to you and you wish to proceed with the Training, you must first review and
	confirm you have read and understand each section of the Agreement including Schedules A and B. Then, in order to obtain
	a grant, you must agree to be bound by the Agreement by electronically signing the Agreement using the &ldquo;Accept Agreement&rdquo; button on the &ldquo;Review and Accept Grant Agreement&rdquo;
	page. If we do not receive your acceptance of the Agreement by way of your electronic signature by the date set out in Schedule A, this approval will expire.</p>
<p>Thank you for your participation in the @Model.GrantProgramName program. The Province of British Columbia introduced
	the @Model.GrantProgramName to help better align skills training with employer needs, create more jobs for Canadians
	and build a stronger, more skilled workforce. Employers are seen as partners in the skills training system. By sharing in
	the associated costs, you are demonstrating your commitment to the continued success of the Canadian economy. We are interested
	in your feedback and would appreciate learning from your experience with the Program. Please contact us with any questions
	or concerns using the &ldquo;Contact Us&rdquo; link below.</p>
<p>Sincerely,</p> Director
<br /> @Model.GrantProgramName Program
<br />
<hr />
<p><b><u>Declaration and acceptance of Agreement:</u></b></p>
<p>I am authorized to act and to enter into this Agreement on behalf of the Employer. On the Employer’s behalf, I do hereby
	accept and agree to the terms and conditions of this Agreement, including this Approval Letter, Schedule A and Schedule B.</p>
<div style="padding: 0 50px;">
<p class="well well--dark" style="background: none;">Note: If the Employer’s signing authority is also a Participant under this Agreement, but is not an owner or co-owner of the business,
	he/she cannot sign on the Employer’s behalf. In this case, please ensure an alternate signing authority accepts this Agreement on behalf of the Employer.</p>
</div>
'
WHERE ID = 1;

-- Agreement Schedule A
PRINT 'UPDATE DEFAULT AGREEMENT SCHEDULE A TEMPLATE'

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
						<td class="num-col">@(ToCurrency(expenseItem.AgreedEmployerContribution))</td>
						<td class="num-col">@(ToCurrency(expenseItem.AgreedMaxReimbursement))</td>
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
					<b>@(ToCurrency(Model.TrainingCost.TotalAgreedMaxCost))</b>
				</td>
				<td class="num-col">
					<b>@(ToCurrency(Model.TrainingCost.AgreedEmployerContribution))</b>
				</td>
				<td class="num-col">
					<b>@(ToCurrency(Model.TrainingCost.AgreedMaxReimbursement))</b>
				</td>
			</tr>
		</tbody>
	</table>
</div>
'
WHERE ID = 2;

-- Agreement Schedule B
PRINT 'UPDATE DEFAULT AGREEMENT SCHEDULE B TEMPLATE'

UPDATE [dbo].[DocumentTemplates] 
SET [Body] = 
N'
 @model CJG.Application.Business.Models.DocumentTemplate.GrantApplicationTemplateModel
<h4>Schedule B - Definitions and General Terms</h4> In this Agreement, including the Approval Letter:
<br />
<ul class="list--nostyle">
	<li>&ldquo;<b>Canadian Citizen</b>&rdquo; means a person who has Canadian citizenship status by birth or as granted by Canada;</li>
	<li>&ldquo;<b>Eligible Training Costs</b>&rdquo; means tuition fees charged by a third-party Training Provider, mandatory student fees,
		examination fees and costs of textbooks, software and other eligible materials required for training, or in-house training
		costs that are actually incurred by the Employer for the Training, up to the maximum amount set out for each Participant
		in Schedule A and subject to any other applicable funding caps or restrictions referred to in this Agreement;</li>
	<li>&ldquo;<b>Employer</b>&rdquo;, &ldquo;
		<b>you</b>&rdquo; or &ldquo;
		<b>your</b>&rdquo; means the employer, or organization acting on behalf of employers, that applied for the Training and to
		whom the Province&#8217;s Approval Letter has been addressed;</li>
	<li>&ldquo;<b>Employer Contribution</b>&rdquo; means the total amount of all Training related expenses, less the amount of any Eligible
		Training Costs payable by the Province to the Employer, in accordance with this Agreement;</li>
	<li>&ldquo;<b>Fiscal Year</b>&rdquo; means April 1 to March 31 of the following year;</li>
	<li>&ldquo;<b>Overpayment</b>&rdquo; means any and all amounts provided by the Province to the Employer that are not expended during
		the Term on Eligible Training Costs;</li>
	<li>&ldquo;<b>Participant</b>&rdquo; means each individual set out in Schedule A for whom the Employer has applied, and the Province
		has approved, to receive Training;</li>
	<li>&ldquo;<b>Parties</b>&rdquo; means collectively the Employer and the Province;</li>
	<li>&ldquo;<b>Party</b>&rdquo; means either the Employer or the Province, as the context requires;</li>
	<li>&ldquo;<b>Permanent Resident</b>&rdquo; means a person who has legally immigrated to Canada but is not yet a Canadian citizen. This
		person has acquired permanent resident status and has not subsequently lost that status under section 46 of the
		<i>Immigration and Refugee Protection Act</i>;</li>
	<li>&ldquo;<b>Protected Persons</b>&rdquo; means a person who has been determined to be a Convention refugee or person in similar circumstances
		by a Canadian visa officer outside Canada, a person whom the Immigration and Refugee Board of Canada has determined to
		be a Convention refugee or in need of protection in Canada, or a person who has had a positive pre-removal risk assessment
		(in most cases);</li>
	<li>&ldquo;<b>Program</b>&rdquo; means the entire project proposed by the Employer and approved by the Province pursuant to the Approval Letter,
		for the provision of the Training to Participants in accordance with this Agreement;</li>
	<li>&ldquo;<b>Program Requirements</b>&rdquo; means the general principles, criteria, requirements and Participant and Employer expectations
		and obligations relating to the Program and the Training, as may be amended by the Province in its sole discretion from
		time to time, which can be viewed via <a href="https://www.workbc.ca/Employer-Resources.aspx" target="_blank">About Skills Training Grants</a> or such other location as we may specify from time to time;</li>
	<li>&ldquo;<b>Province</b>&rdquo;, &ldquo;
		<b>we</b>&rdquo;, &ldquo;
		<b>us</b>&rdquo; or &ldquo;
		<b>our</b>&rdquo; means Her Majesty the Queen in right of the Province of British Columbia, represented by the Minister of
		Advanced Education, Skills and Training;</li>
	<li>&ldquo;<b>Reports</b>&rdquo; means, collectively, any and all reports or additional information at any time required under the Program
		Requirements or this Agreement to be provided to the Province by the Employer or by any Participant; </li>
	<li>&ldquo;<b>Term</b>&rdquo; means the period that begins upon the electronic acceptance of the Agreement by or on behalf of the Employer,
		provided that such acceptance occurs prior to the expiration of the Province’s approval in accordance with the Approval Letter,
		and ends on the end date set out in Schedule A or March 31, 2020, whichever is earlier;</li>
	<li>&ldquo;<b>Training</b>&rdquo; means the approved training opportunity for each Participant set out in Schedule A;</li>
	<li>&ldquo;<b>Training Provider</b>&rdquo; means the third party provider(s) of Training as set out in Schedule A.</li>
</ul>
<h4>Employer Obligations</h4>
<p>
	<b>1.</b> &nbsp; In addition to your other obligations set out elsewhere in this Agreement, including the Approval Letter,
	you must:</p>
<ol class="list--lettered">
	<li>only request reimbursement for Eligible Training Costs that have been incurred by you;</li>
	<li>upon our request, promptly inform us regarding Participants’ Training and employment status and provide any additional information
		we may reasonably require;</li>
	<li>comply with any &ldquo;Marketing, Publicity and Communications Guidelines&rdquo; regarding any public announcements that
		you make with respect to your participation in the Program, the Training and your acknowledgment of funding received for
		the Training from the Province and the Government of Canada. Prior to making any such public announcements, you must obtain
		the current &ldquo;Marketing, Publicity and Communications Guidelines&rdquo; by email request to
		<a href="mailto:@(Model.GrantProgramCode)@("@gov.bc.ca")">@(Model.GrantProgramCode)@("@gov.bc.ca")</a> or such other address as we may specify from time to time; </li>
	<li>submit all Employer Reports and collect and submit all Participant Reports required by us;</li>
	<li>immediately provide us with full and complete details regarding any Training funding that you receive or anticipate receiving
		from any other person or entity, including another government or governmental body; and</li>
	<li> establish and maintain complete and accurate accounting and administrative records with respect to the Participants, the
		Training, the Eligible Training Costs, the Employer Contribution and any Overpayments, including any original receipts
		and other supporting documentation, in form and content satisfactory to us, and you must keep and make those records available
		to us upon request for a period of at least seven years following the completion of the Training.</li>
</ol>
<h4>Holdback and Set-Off</h4>
<p>
	<b>2.</b> &nbsp; We may temporarily withhold or set-off from any payment to you the amount of any Overpayment.</p>
<h4>Relationship and Conflict of Interest</h4>
<p>
	<b>3.</b> &nbsp; Even though both Parties have certain obligations relating to Training costs, you are an independent contractor
	and no partnership, joint venture, agency or other legal entity or structure will be created or deemed to be created by
	this Agreement or any actions of the Parties under this Agreement.</p>
<p>
	<b>4.</b> &nbsp; You must not in any way commit or purport to commit the Province to the payment of money to any person or
	entity.</p>
<p>
	<b>5.</b> &nbsp; During the Term, you must not perform a service for, or provide advice to, any person or entity where the
	performance of such service or the provision of the advice may, in our reasonable opinion, give rise to a conflict of interest.
	You must also not, without our consent, use any person or entity to deliver Training where that delivery of Training may
	give rise to an actual or a perceived conflict of interest.</p>
<h4>Assignment and Subcontracting</h4>
<p>
	<b>6.</b> &nbsp; You will not, without our prior written consent, either directly or indirectly assign this Agreement or any
	of your rights under this Agreement.</p>
<p>
	<b>7.</b> &nbsp; No sub-contract or Training contract entered into by you will relieve you from any of your obligations under
	this Agreement or impose upon the Province any obligation or liability arising from any such sub-contract or Training contract.</p>
<p>
	<b>8.</b> &nbsp; This Agreement will be binding upon and enure to the benefit of the Province and its assigns and you and your
	successors and permitted assigns.</p>
<h4>Representations and Warranties</h4>
<p>
	<b>9. </b> &nbsp; You represent and warrant that:</p>
<ol class="list--lettered">
	<li>you have the legal capacity to enter into and to fulfil your obligations under this Agreement;</li>
	<li>you have no knowledge of any fact that materially adversely affects, or so far as you can foresee, might materially adversely
		affect, your properties, assets, condition (financial or otherwise), business or operations or your ability to fulfil your
		obligations under this Agreement;</li>
	<li>all information, statements, documents and reports at any time provided by you in connection with this Agreement are, will
		be and will remain, true and correct; and</li>
	<li>you are not in breach of, or in default under, any law, statute or regulation of Canada or of the Province of British Columbia
		that is relevant to the Training, the Program or the subject matter of this Agreement.</li>
</ol>
<h4>Default and Termination</h4>
<p>
	<b>10. </b> &nbsp; If you fail to comply with any provision of this Agreement, or if any representation or warranty made by
	you is or becomes untrue or incorrect, or if, in our opinion, you cease to operate or if a change occurs with respect to
	any one or more of your properties, assets, condition (financial or otherwise), business or operations which, in our opinion,
	materially adversely affects your ability to fulfil your obligations under this Agreement (each a &ldquo;
	<b>Default</b>&rdquo;), then we may do any one or more of the following:</p>
<ol class="list--lettered">
	<li>waive the Default;</li>
	<li>require you to remedy the Default within a time period specified by us;</li>
	<li>suspend any reimbursement of Eligible Training Costs or any other amount that is due to you while the Default continues;</li>
	<li>terminate this Agreement, in which case the payment of the amount required under paragraph 13 will discharge us of all liability
		to you under this Agreement; or</li>
	<li>pursue any other remedy available to us at law or in equity.</li>
</ol>
<p>
	<b>11. </b> &nbsp; Either Party may terminate this Agreement by giving the other Party at least 30 days&#8217; written notice.</p>
<p>
	<b>12. </b> &nbsp; We may also terminate this Agreement, with immediate effect, if we determine that any action or inaction
	by you places the health or safety of any person at immediate risk.</p>
<p>
	<b>13. </b> &nbsp; In the event that this Agreement is terminated by either Party, we will pay the Eligible Training Costs
	in respect of any Training completed on or before the effective date of termination, less any outstanding Overpayment, which
	will discharge us of all liability to you under this Agreement.</p>
<h4>Indemnity</h4>
<p>
	<b>14. </b> &nbsp; You will indemnify and save harmless the Province, its employees, agents and contractors, from and against
	any and all losses, claims, damages, actions, causes of action, costs and expenses that it or they may sustain, incur, suffer
	or be put to at any time either before or after the expiration or termination of this Agreement, where the same or any of
	them are based upon, arise out of or occur, directly or indirectly, by reason of any act or omission by you or by any of
	your agents, employees, officers, directors or sub-contractors pursuant to this Agreement, excepting always liability arising
	out of the independent negligent acts of the Province.</p>
<h4>Delivery and Notice</h4>
<p>
	<b>15. </b> &nbsp; In order to be effective, this Agreement and any legal notice required by this Agreement (&ldquo;
	<b>Notice</b>&rdquo;) must be in writing and delivered as follows:</p>
<ol class="list--lettered">
	<li> to you, to your mailing address shown in the Approval Letter
		<ol class="list--numerals">
			<li>by hand (including by courier), in which case it will be deemed to be received on the day of its delivery, or</li>
			<li>by prepaid mail, in which case it will be deemed to be received on the fifth business day after its mailing; or</li>
		</ol>
	</li>
	<li> to us:
		<ol class="list--numerals">
			<li> by hand (including by courier), in which case it will be deemed to be received on the day of its delivery, to:
				<br /> &nbsp; &nbsp;
				<span style="text-decoration: underline;">2nd Floor, 1106 Cook Street, Victoria, BC, V8V 3Z9</span>
			</li>
			<li> by prepaid mail, in which case it will be deemed to be received on the fifth business day after its mailing, to:
				<br /> &nbsp; &nbsp;
				<span style="text-decoration: underline;">PO Box 9189, Stn Prov Govt, Victoria, BC, V8W 9E6</span>
			</li>
		</ol>
	</li>
</ol>
<p>
	<b>16. </b> &nbsp; Either Party may, from time to time, notify the other Party in writing of a change of address for delivery
	and, following the receipt of such Notice in accordance with paragraph 15, the new address will, for the purposes of paragraph
	15, be deemed to be the delivery address of the Party giving Notice.</p>
<p>
	<b>17. </b> &nbsp; The Parties may execute and deliver separate copies (counterparts) of this Agreement and each executed and
	delivered counterpart will be considered to be an original. These counterparts may be delivered by email (with a scanned
	copy attached in PDF format) to an email address provided by the other Party for such purpose
	or by way of electronic submission in the manner provided for in this Agreement or the Program Requirements.</p>
<h4>Miscellaneous Terms</h4>
<p>
	<b>18. </b> &nbsp; The Schedules to this Agreement are an integral part of this Agreement as if included in the body of this
	Agreement.</p>
<p>
	<b>19. </b> &nbsp; This Agreement together with any documents or other information referred to in it, including the Approval
	Letter and the Program Requirements, constitute the entire Agreement between the Parties with respect to the subject matter
	of this Agreement. </p>
<p>
	<b>20. </b> &nbsp; In this Agreement, unless the context requires otherwise, words using the singular form include the plural
	form and
	<i>vice versa</i>.</p>
<p>
	<b>21. </b> &nbsp; The headings in this Agreement are inserted for convenience only and do not form part of this Agreement.</p>
<p>
	<b>22. </b> &nbsp; No amendment to or modification of this Agreement will be effective unless it is in writing and signed by
	both Parties, unless otherwise specifically contemplated by this Agreement.</p>
<p>
	<b>23. </b> &nbsp; Nothing in this Agreement operates as a consent, permit, approval or authorization by any Ministry or Branch
	of the Government of the Province of British Columbia for anything that, by statute, you are required to obtain, unless
	this Agreement expressly indicates otherwise. </p>
<p>
	<b>24. </b> &nbsp; No term or condition of this Agreement and no breach of any such term or condition by you will be deemed
	to have been waived unless such waiver is in writing and signed by both Parties.</p>
<p>
	<b>25. </b> &nbsp; Our written waiver of any breach by you of a term or condition of this Agreement will not be deemed to be
	a waiver of any other provision of this Agreement or of any prior or subsequent breach.</p>
<p>
	<b>26. </b> &nbsp; This Agreement will be governed by and construed in accordance with the laws of the Province of British
	Columbia and the laws of Canada applicable therein.</p>
<p>
	<b>27. </b> &nbsp; The courts of the Province of British Columbia, sitting in Victoria, will have the exclusive jurisdiction
	to hear any disputes arising from or in any way related to this Agreement or the relationship of the Parties.</p>
<p>
	<b>28. </b> &nbsp; If any provision of this Agreement or its application to any person or circumstance is found by a court
	of competent jurisdiction to be invalid or unenforceable to any extent, the remainder of this Agreement and the application
	of that provision to any other person or circumstance will not be affected or impaired and will be enforceable to the extent
	permitted by law.</p>
<p>
	<b>29. </b> &nbsp; The provisions of the Approval Letter and paragraphs 1 - 4, 7 - 9, 13, 14 and 18 - 29 of this schedule and
	any other provision(s) or other section(s) of this Agreement (including this schedule) or the Program Requirements which,
	by their terms or nature, are intended to survive the completion or termination of this Agreement or are necessary for the
	interpretation or enforcement of this Agreement, will continue in force indefinitely subject to any applicable limitation
	period prescribed by law, even after this Agreement ends.</p>
<br />
'
WHERE ID = 3;

-- Agreement Declaration
PRINT 'UPDATE Default Applicant Declaration Template'
UPDATE [dbo].[DocumentTemplates] 
SET [Body] = 
N'
@model CJG.Application.Business.Models.DocumentTemplate.GrantApplicationTemplateModel
<li>I certify that I am authorized to submit this Application and to make this declaration on behalf of the applicant referred to in this Application (the "Applicant");</li> 
<li>I acknowledge that I have read and understand the @Model.GrantProgramName criteria applicable to this Application, including the sample @Model.GrantProgramName Agreement, consisting of the Approval Letter, Schedule A and Schedule B (and the Program Requirements referred to therein), as made available by the Province of British Columbia at the link below;</li> 
<li>I acknowledge that, as the terms and conditions of the @Model.GrantProgramName Agreement are subject to change from time to time, should this Application be approved, the @Model.GrantProgramName Agreement that will be sent to the Applicant for signature may materially differ from the sample @Model.GrantProgramName Agreement that was posted at the time this Application was submitted and I acknowledge that I (or another individual authorized by the Applicant) will be responsible for reviewing, understanding and agreeing to the terms and conditions as they appear at the time the Applicant enters into a @Model.GrantProgramName Agreement with the Province;</li> 
<li>I certify that all of the information provided on this Application is true and correct to the best of my knowledge and belief;</li> 
<li>I acknowledge and agree that checking the box below has the same legal effect as making this declaration under a hand-written signature; and </li> 
<li>I do hereby make this declaration on my own behalf and on behalf of the Applicant as of the date that this Application is submitted.</li> 
'
WHERE ID = 4;

-- Agreement Consent
PRINT 'UPDATE Default Participant Consent Template'
UPDATE [dbo].[DocumentTemplates] 
SET [Body] = 
N'
@model CJG.Application.Business.Models.DocumentTemplate.GrantApplicationTemplateModel
<h2 class="subheader">Personal Information</h2>
<p>
	The Government of Canada ("Canada") provides funding for the @Model.GrantProgramName ("@Model.GrantProgramCode") under the Canada-British Columbia Workforce Development Agreement (WDA), administered by the British Columbia Ministry of Advanced Education, Skills and Training ("AEST"), and under the Labour Market Development Agreement ("LMDA"), administered by the British Columbia Ministry of Social Development and Poverty Reduction ("SDPR").
</p>
<p>
	All personal information related to your participation in a @Model.GrantProgramCode training opportunity ("Personal Information") is collected pursuant to sections 26(c) and 26(e) of the Freedom of Information and Protection of Privacy Act ("FOIPPA").  This information will be used for administrative, evaluation, research, accountability, and reporting purposes, including to determine your eligibility for participation in the @Model.GrantProgramCode program and to meet federal reporting requirements under the WDA and LMDA.
</p>
<p>
	Under the authority of Section 33.1(d) and 33.2(d) of FOIPPA, the personal information collected may be disclosed to the Department of Employment and Skills Development Canada ("ESDC") or the Canada Employment Insurance Commission ("CEIC") for the purposes of administering the Employment Insurance Act.  The CEIC may also use your Personal Information for policy analysis, research or evaluation purposes.
</p>
<p>
	Under the authority of Section 3.6 of the LMDA, if you are currently in receipt of Employment Insurance (EI) benefits, a referral under Section 25 of the EI Act may be placed on your EI claim, to allow you to continue to receive EI benefits, up to the end of your EI benefit period, while you participate in the @Model.GrantProgramName program.
</p>
<p>
	All @Model.GrantProgramCode training participants are required to complete two satisfaction surveys at approximately 3 months and 12 months following completion of their training ("Surveys").  The Surveys will ask basic questions about the outcomes of training and whether the training met your employment needs.  Your contact information will be shared with British Columbia''s statistical agency, BC Stats, in order for them to contact you to conduct these Surveys.
</p>
<p>
	You may also be asked if you wish to, or you may volunteer to, provide a testimonial regarding your @Model.GrantProgramCode training experience ("Testimonial").  Testimonials, and any Personal Information that you choose to include in a Testimonial, may be used and disclosed to the public to promote the @Model.GrantProgramCode program.
</p>
<h2>Consent and Certification</h2>
<p>
	Effective as of the date set out below, and in consideration of the opportunity for me to participate in @Model.GrantProgramCode training, I:
</p>
<ul>
	<li>consent to the collection use, and disclosure of my Personal Information for purposes set out above; </li>
	<li>consent to my Personal Information being used to contact me to conduct the Surveys and to request a Testimonial;</li>
	<li>certify that all of the information that I have provided in this form is accurate and complete; and</li>
	<li>certify that I understand that I am expected to complete my training and I must complete the Surveys in order for my employer to meet all of its @Model.GrantProgramName Agreement obligations;</li>
</ul>
<p>
	By confirming my consent, I acknowledge and agree that this typed signature has the same legal effect as a written signature.
</p>
<hr>
<p>
	If you have any questions about the collection, use or disclosure of your Personal Information, please contact the Program Manager, B.C. Employer Training Grant at 1-877-952-6914, or by mail at: Program Manager, @Model.GrantProgramName, 2nd Floor, 1106 Cook Street, Victoria, BC, V8V 3Z9 or by submitting an email to <a href="mailto:@(Model.GrantProgramCode)@("@")gov.bc.ca">@(Model.GrantProgramCode)@("@")gov.bc.ca</a>
</p>
'
WHERE ID = 5;

-- Agreement Cover Letter
PRINT 'UPDATE Default CJG Applicant Cover Letter Template'
UPDATE [dbo].[DocumentTemplates] 
SET [Body] = 
N'
@model CJG.Application.Business.Models.DocumentTemplate.GrantApplicationTemplateModel
<div style="float: left;">@CJG.Core.Entities.AppDateTime.UtcNow.ToString("MMMM dd, yyyy"))</div>
<div style="float: right;">Application# @Model.FileNumber</div>
<br clear="all" />
<br />@string.Format("{0} {1}", Model.ApplicantFirstName, Model.ApplicantLastName)
<br />@Model.OrganizationLegalName
<br />@Model.ApplicantPhysicalAddress
<br />
<br />
<p>Attention:</p>
<p>Based upon your application for funding under the @Model.GrantProgramName
	<strong>("@Model.GrantProgramCode")</strong>, we are pleased to inform you that we have approved the Training Provider, Training and Eligible Training
	Costs set out in Schedule A &ndash; Grant Services. Please refer to Schedule B for additional defined terms used in this
	<strong>"Approval Letter"</strong> (Schedule B governs if there is an inconsistency between provisions).</p>

<p>When you have accepted all parts of this Agreement, the following provisions of this Approval Letter, along with the Schedules
	A &ndash; Grant Services (the Approved Training, Training Provider and Eligible Training Costs) and B &ndash; Definitions and General
	Terms, will form a Shared Cost Agreement between you and the Province. When the Agreement is in place, your primary obligations
	and the grant reimbursement claim process will be as follows:</p>
<ol class="list--numbered">
	<li>Subject to the terms of this Agreement, we will reimburse you for up to @Model.ReimbursementRate of the Eligible Training Costs,
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
				Agreements (capped at @Model.MaxReimbursementAmt per Participant per Fiscal Year);</li>
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
<p>Thank you for your participation in the @Model.GrantProgramName program. The Province of British Columbia introduced
	the @Model.GrantProgramName to help better align skills training with employer needs, create more jobs for Canadians
	and build a stronger, more skilled workforce. Employers are seen as partners in the skills training system. By sharing in
	the associated costs, you are demonstrating your commitment to the continued success of the Canadian economy. We are interested
	in your feedback and would appreciate learning from your experience with the program. Please contact us with any questions
	or concerns using the Contact US link below.</p>
<p>Sincerely,</p> Director
<br /> @Model.GrantProgramName Program
<br />
<br />
<p>I am authorized to act and to enter into this Agreement on behalf of the Employer. On the Employer''s behalf, I do hereby
	accept and agree to the terms and conditions of this Agreement, including this Approval Letter and the attached Schedules
	A - Grant Services (Approved Training and Eligible Training Costs) and Schedule B - Definitions and General Terms.</p>
<p>Note: If the Employer''s signing authority is also a Participant under this Agreement but is not an owner or co-owner of the
	business, he/she cannot sign on the Employer''s behalf. In this case, please ensure an alternate signing authority signs
	this Agreement on behalf of the Employer.</p>
'
WHERE ID = 6;

-- Agreement Schedule A
PRINT 'UPDATE Default CJG Applicant Schedule A Template'
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
						<td class="num-col">@(ToCurrency(expenseItem.AgreedEmployerContribution))</td>
						<td class="num-col">@(ToCurrency(expenseItem.AgreedMaxReimbursement))</td>
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
					<b>@(ToCurrency(Model.TrainingCost.TotalAgreedMaxCost))</b>
				</td>
				<td class="num-col">
					<b>@(ToCurrency(Model.TrainingCost.AgreedEmployerContribution))</b>
				</td>
				<td class="num-col">
					<b>@(ToCurrency(Model.TrainingCost.AgreedMaxReimbursement))</b>
				</td>
			</tr>
		</tbody>
	</table>
</div>
'
WHERE ID = 7;

-- Agreement Schedule B
PRINT 'UPDATE Default CJG Applicant Schedule B Template'
UPDATE [dbo].[DocumentTemplates] 
SET [Body] = 
N'
@model CJG.Application.Business.Models.DocumentTemplate.GrantApplicationTemplateModel
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
	<li>&ldquo;<b>Program Requirements</b>&rdquo; means the general principles, criteria, requirements and Participant and Employer expectations and obligations relating to the Program and the Training, as may be amended by the Province in its sole discretion from time to time, which can be viewed via <a href="https://www.workbc.ca/Employer-Resources/Canada-BC-Job-Grant/What-is-the-Canada-B-C-Job-Grant.aspx" target="_blank">About @Model.GrantProgramName</a>  or such other location as we may specify from time to time;</li>
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
	<li>comply with any &ldquo;Marketing, Publicity and Communications Guidelines&rdquo; regarding any public announcements that you make with respect to your participation in the Program, the Training and your acknowledgment of funding received for the Training from the Province and the Government of Canada. Prior to making any such public announcements, you must obtain the current &ldquo;Marketing, Publicity and Communications Guidelines&rdquo; by email request to <a href="mailto:@(Model.GrantProgramCode)@("@gov.bc.ca")">@(Model.GrantProgramCode)@("@gov.bc.ca")</a> or such other address as we may specify from time to time; </li>
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
<br />
'
WHERE ID = 8;

-- Agreement Declaration
PRINT 'UPDATE Default CJG Applicant Declaration Template'
UPDATE [dbo].[DocumentTemplates] 
SET [Body] = 
N'
@model CJG.Application.Business.Models.DocumentTemplate.GrantApplicationTemplateModel
<li>I certify that I am authorized to submit this Application and to make this declaration on behalf of the applicant referred to in this Application (the "Applicant");</li>
<li>I acknowledge that I have read and understand the @(Model.GrantProgramCode) criteria applicable to this Application, including the sample @(Model.GrantProgramCode) Agreement, consisting of the Approval Letter, Schedule A and Schedule B (and the Program Requirements referred to therein), as made available by the Province of British Columbia at the link below;</li>
<li>I acknowledge that, as the terms and conditions of the @(Model.GrantProgramCode) Agreement are subject to change from time to time, should this Application be approved, the @(Model.GrantProgramCode) Agreement that will be sent to the Applicant for signature may materially differ from the sample @(Model.GrantProgramCode) Agreement that was posted at the time this Application was submitted and I acknowledge that I (or another individual authorized by the Applicant) will be responsible for reviewing, understanding and agreeing to the terms and conditions as they appear at the time the Applicant enters into a @(Model.GrantProgramCode) Agreement with the Province;</li>
<li>I certify that all of the information provided on this Application is true and correct to the best of my knowledge and belief;</li>
<li>I acknowledge and agree that checking the box below has the same legal effect as making this declaration under a hand-written signature; and </li>
<li>I do hereby make this declaration on my own behalf and on behalf of the Applicant as of the date that this Application is submitted.</li>
'
WHERE ID = 9;

-- Agreement Consent
PRINT 'UPDATE Default CJG Participant Consent Template'
UPDATE [dbo].[DocumentTemplates] 
SET [Body] = 
N'
@model CJG.Application.Business.Models.DocumentTemplate.GrantApplicationTemplateModel
<h2 class="subheader">Consent for Collection and Use of Personal Information</h2>
<p>
	All personal information collected here and by future surveys and testimonials and other information
	related to your participation in a @Model.GrantProgramName training opportunity ("Personal Information")
	is collected pursuant to sections 26(c) and 26(e) of the Freedom of Information and Protection of Privacy Act.
</p>

<p>
	This information will be used for administrative and evaluation purposes, including to determine your eligibility for participation in the @Model.GrantProgramName program,
	and, in the case of any Testimonial, may be used and disclosed to publicly promote the @Model.GrantProgramName program ("Purposes").
	The Government of Canada ("Canada") provides funding for the @Model.GrantProgramName under the Canada-British Columbia Job Fund Agreement.
	Personal Information will be used to create reports about @Model.GrantProgramName training in British Columbia that will be provided to Canada in aggregate form only.
	No personally identifiable information will be disclosed to Canada for this purpose.
	Personal information will be shared with BC Stats, BC''s central statistics agency, for the purpose of conducting outcome surveys with @Model.GrantProgramName participants.
	All information provided to BC Stats is protected under the BC Statistics Act and is kept confidential.
	If you have any questions about the use of this information, please contact the Program Manager, @Model.GrantProgramName at 1-877-952-6914, or by mail to:
</p>
<p class="well">
	Program Manager, @Model.GrantProgramName<br />
	2nd Floor, 1106 Cook Street<br />
	Victoria, BC, V8V 3Z9<br />
</p>
<p>
	Effective as of the date set out below, and in consideration of the opportunity for me to participate in @Model.GrantProgramName training, I certify that:
</p>
<ul>
	<li>all of the information that I will provide is accurate and complete;</li>
	<li>
		I understand that I am expected to complete my training and I must complete surveys in order for the employer to meet all of their obligations under their @Model.GrantProgramName Agreement; and,
	</li>
</ul>
<p>I consent to:</p>
<ul>
	<li>
		the collection and use of my Personal Information by the Province of British Columbia for purposes of accountability, quality assurance, research, and evaluation of the @Model.GrantProgramName; and,
	</li>
	<li>my Personal Information being used to contact me to conduct the Surveys and to request a Testimonial.</li>
</ul>
'
WHERE ID = 10;

-- Agreement Schedule B
PRINT 'UPDATE Default CWRG Applicant Schedule B Template'
UPDATE [dbo].[DocumentTemplates] 
SET [Body] = 
N'
@model CJG.Application.Business.Models.DocumentTemplate.GrantApplicationTemplateModel
<h4>1 &nbsp; Definitions</h4>

<ul class="info-list">
	<p>In this Agreement (including the Approval Letter), in addition to terms defined in the Approval Letter:</p>
	<li>&ldquo;<b>Administration Fees</b>&rdquo; means the amount available to the Applicant to offset costs associated with project management, the outreach to and recruitment of Participants, and the preparation and submission of Claims and Reports, to the maximum amount set out in Schedule A;</li>
	<li>&ldquo;<b>Agreement</b>&rdquo; means the Shared Cost Arrangement Agreement between the Province and the Applicant which will be formed upon the Applicant’s acceptance of the terms and provisions of the Approval Letter, Schedule A and Schedule B;</li>
	<li>&ldquo;<b>Applicant</b>&rdquo;, &ldquo;<b>you</b>&rdquo; or &ldquo;<b>your</b>&rdquo; means the organization that applied for a grant under the @Model.GrantProgramCode Program and to whom the Approval Letter has been addressed;</li>
	<li>&ldquo;<b>Claim</b>&rdquo; means the submission of any Receipts or Invoices in order to claim a Payment;</li>
	<li>&ldquo;<b>@Model.GrantProgramCode Program Requirements</b>&rdquo; means the general principles, intent, policy, criteria, requirements and Participant and Applicant obligations relating to the @Model.GrantProgramCode Program, as may be amended by the Province in its sole discretion from time to time, which can be viewed on the ‘Grant Overview’ page at https://www.workbc.ca/Employment-Services/Community-Workforce-Response-Grant/Grant-Overview.aspx, or such other location as we may specify from time to time;
	<li>&ldquo;<b>Eligible Costs</b>&rdquo; means those costs associated with providing Program Services that are eligible for Payment in strict accordance with the @Model.GrantProgramCode Program Requirements and this Agreement, up to the maximum amount set out for each Participant, and in aggregate, in Schedule A and, for Payment purposes, including the Administration Fee that may be earned by the Applicant up to the maximum amount set out in Schedule A;</li>
	<li>&ldquo;<b>Employment Assistance Services</b>&rdquo; (if included in Schedule A) means the component of Program Services provided to Participants during their participation in the Program which are intended to enable labour market participation and increase employability, using services such as assessments, training plan development, job readiness skills development (including resume writing and interview skills), mentoring, coaching and indigenous cultural supports, in accordance with the @Model.GrantProgramCode Program Requirements and as set out in more detail in Schedule A;</li>
	<li>&ldquo;<b>Fiscal Year</b>&rdquo; means the period beginning on April 1 and ending on March 31 of the following calendar year;</li>
	<li>&ldquo;<b>Invoice</b>&rdquo; means the document(s) related to a request for Payment, without a Receipt, of any Eligible Costs;</li>
	<li>&ldquo;<b>Overpayment</b>&rdquo; means any and all amounts provided by the Province to the Applicant that are not expended during the Term on Eligible Costs or that are otherwise deemed ineligible in accordance with this Agreement;</li>
	<li>&ldquo;<b>Participant</b>&rdquo; means each individual that the Applicant has enrolled in the Program and   that is receiving, or has received, Program Services;</li>
	<li>&ldquo;<b>Participant Completion Report</b>&rdquo; means the final report, as required by the Province upon the earlier of the completion date of the entire Program or the end of the Term, providing details regarding Program completion and the achievement of Outcomes for all Participants, in a form to be provided by the Province;</li>
	<li>&ldquo;<b>Participant Information Forms</b>&rdquo; means Participant personal information forms that are to be submitted directly by all Participants via the Skills Training Grant System;</li>
	<li>&ldquo;<b>Participant Financial Supports</b>&rdquo; (if included in Schedule A) means the financial supports component of the Program Services provided to Participants during their participation in the Program that are intended to remove barriers to their success in the Program, such as costs for childcare, transportation, personal protective gear, required uniforms, travel, accommodation and disability supports, in accordance with the @Model.GrantProgramCode Program Requirements and as set out in more detail in Schedule A;</li>
	<li>&ldquo;<b>Parties</b>&rdquo; means, collectively, the Applicant and the Province;</li>
	<li>&ldquo;<b>Party</b>&rdquo; means either the Applicant or the Province, as the context requires;</li>
	<li>&ldquo;<b>Program</b>&rdquo; means the entire project proposed by the Applicant and approved by the Province pursuant to the Approval Letter, for the provision of the Program Services to Participants in accordance with the Agreement;</li>
	<li>&ldquo;<b>Program Delivery Plan</b>&rdquo; means the plan for the Applicant’s delivery of the Program, as approved by the Province;</li>
	<li>&ldquo;<b>Program Services</b>&rdquo; means all of the Employment Assistance Services, Participant Financial Supports, and Skills Training to be provided to Participants under the Program, as set out in Schedule A;</li>
	<li>&ldquo;<b>Province</b>&rdquo; &ldquo;<b>we</b>&rdquo;, &ldquo;<b>us</b>&rdquo; or &ldquo;<b>our</b>&rdquo; means Her Majesty the Queen in right of the Province of British Columbia, represented by the Minister of Advanced Education, Skills and Training;</li>
	<li>&ldquo;<b>Receipt</b>&rdquo; means the document(s) that are related to and that verify a request for a Payment for any Employment Assistance Services, Participant Financial Supports or Training expenses already incurred by the Applicant;</li>
	<li>&ldquo;<b>Reports</b>&rdquo; means, collectively, any and all reports or other information as may at any time be required under the @Model.GrantProgramCode Program Requirements or this Agreement, which are to be provided to the Province by the Applicant or by any Participant;</li>
	<li>&ldquo;<b>Skills Training Grant System</b>&rdquo; means the web-based @Model.GrantProgramCode Grant application system through which applications, change requests, Participant Information Forms, Claims, and Reports are to be submitted;</li>
	<li>&ldquo;<b>Skills Training Program</b>&rdquo; means the mandatory training component of the Program Services, which must be 52 weeks or less in duration and delivered by a Third Party Service Provider to Participants during their participation in the Program. It is intended to provide Participants with skills that are necessary to be successful in a job, such as occupational, essential or soft skills, whether provided in a classroom setting and/or online, in accordance with the @Model.GrantProgramCode Program Requirements and as set out in more detail in Schedule A;</li>
	<li>&ldquo;<b>Term</b>&rdquo; means the period that begins and, unless otherwise agreed in writing or this Agreement is terminated earlier in accordance with its terms, ends on the dates set out in Schedule A; </li>
	<li>&ldquo;<b>Third Party Service Provider</b>&rdquo; means any third party service provider that is eligible, in accordance with the @Model.GrantProgramCode Program Requirements, and at arms’ length from the Applicant and is chosen by the Applicant and listed in Schedule A or, in the case of any Participant Financial Supports, is chosen by the Participant, to provide any Program Services; and</li>
	<li>&ldquo;<b>Training</b>&rdquo; means a Participant’s participation in, and receipt of services under, the Skills Training Program.</li>
</ul>

<h4>2 &nbsp; Applicant Obligations</h4>
<p>In addition to your other obligations set out elsewhere in this Agreement, including the Approval Letter, you must:</p>
<ol class="list--lettered">
	<li>submit a Claim in order to request Payment for Eligible Costs, supported by Invoices, Receipts or other verification documentation acceptable to and as required by the Province, and in accordance with the &ldquo;Payment Guidelines&rdquo; attached as Appendix 1 to this Schedule B;</li>
	<li>upon our request, promptly inform us regarding Participants’ Training and employment status and provide any additional information we may reasonably require;</li>
	<li>prior to making any public announcements with respect to your participation in the @Model.GrantProgramCode Program, obtain (by email request to <a href="mailto:@(Model.GrantProgramCode + "@gov.bc.ca")">@(Model.GrantProgramCode + "@gov.bc.ca")</a> or such other address as we may specify from time to time) and comply with the most current &ldquo;Marketing, Publicity and Communications Guidelines&rdquo; and any other directions regarding such announcements and your acknowledgment of funding received from the Province and the Government of Canada for the Program;</li>
	<li>ensure that all Reports required by us are or have been submitted; </li>
	<li>immediately provide us with full and complete details regarding any Program funding that you receive or anticipate receiving from any other person or entity, including another government or governmental body; and</li>
	<li>without limiting the record-keeping requirements contained in the Approval Letter, establish, maintain, and make available to us upon request, complete and accurate accounting and administrative records with respect to the Program, the Program Services, Third Party Service Providers (including their contact information), all Eligible Costs, Claims and any Overpayments (including any original receipts and other supporting documentation), in form and content satisfactory to us, and you must keep and make those records available to us upon request, for a period of at least seven years following the end of the Term. </li>
</ol>

<h4>3 &nbsp; Holdback and Set-Off</h4>
<p>We may temporarily withhold or set-off from any payment to you the amount of any Overpayment.</p>

<h4>4 &nbsp; Relationship and Conflict of Interest</h4>
<ol class="list--lettered">
	<li>
		You are an independent contractor and no partnership, joint venture, agency or other legal entity, relationship or structure will be created or will be deemed to be created by this Agreement or any actions of the Parties under this Agreement.
	</li>
	<li>
		You must not in any way commit or purport to commit the Province to the payment of money to any person or entity.
	</li>
	<li>
		During the Term, you must not perform a service for, or provide advice to, any person or entity where the performance of such service or the provision of the advice may, in our reasonable opinion, give rise to a conflict of interest. You must also not, without our consent, permit a Third Party Service Provider to deliver any Program Services where that delivery of Program Services may give rise to an actual or a perceived conflict of interest.
	</li>
</ol>

<h4>5 &nbsp; Assignment and Subcontracting</h4>
<ol class="list--lettered">
	<li>
		You will not, without our prior written consent, either directly or indirectly assign this Agreement or any of your rights or obligations under this Agreement.
	</li>
	<li>
		Unless approved by the Province in advance and in writing, only those Third Party Service Providers set out in Schedule A will be entitled to provide the Training Program Services and Employment Assistance Services in accordance with Schedule A.
	</li>
	<li>
		Subject to your reasonable oversight, discretion, and direction.
	</li>
	<li>
		No Third Party Service Provider arrangement entered into by you or any Participant will relieve you from any of your obligations under this Agreement or impose upon the Province any obligation or liability to such Third Party Service Provider, or any other third party including any Participant, under or in any way associated with any such contract.
	</li>
	<li>
		This Agreement will be binding upon and enure to the benefit of the Province and its assigns and you and your successors and permitted assigns.
	</li>
</ol>

<h4>6 &nbsp; Representations and Warranties</h4>
<p>You represent and warrant that:</p>
<ol class="list--lettered">
	<li>you have the legal capacity to enter into and to fulfil your obligations under this Agreement;</li>
	<li>you have no knowledge of any fact that materially adversely affects, or so far as you can foresee, might materially adversely affect, your properties, assets, condition (financial or otherwise), business or operations or your ability to fulfil your obligations under this Agreement; </li>
	<li>all information, statements, documents and Reports at any time provided by you in connection with this Agreement are, will be and will remain, true and correct; and</li>
	<li>you are not in breach of, or in default under, any law, statute or regulation of Canada or of the Province of British Columbia that is relevant to the @Model.GrantProgramCode Program or the subject matter of this Agreement.</li>
</ol>

<h4>7 &nbsp; Default and Termination</h4>
<ol class="list--lettered">
	<li>
		<p>
			If you fail to comply with any provision of this Agreement, or if any representation or warranty made by you is or becomes untrue or incorrect, or if, in our opinion, you cease to operate or if a change occurs with respect to any one or more of your properties, assets, condition (financial or otherwise), business or operations which, in our opinion, materially adversely affects your ability to fulfil your obligations under this Agreement (each a &ldquo;<b>Default</b>&rdquo;), then we may do any one or more of the following:
		</p>
		<ol type="i">
			<li>
				waive the Default;
			</li>
			<li>
				require you to remedy the Default within a time period specified by us;
			</li>
			<li>
				suspend any Payment of Eligible Costs or any other amount that is due to you while the Default continues;
			</li>
			<li>
				terminate this Agreement, in which case the payment of the amount required under subsection 7 d) below will discharge us of all liability to you under this Agreement; or
			</li>
			<li>
				pursue any other remedy available to us at law or in equity.
			</li>
		</ol>

	</li>
	<li>
		Either Party may terminate this Agreement by giving the other Party at least 30 days’ written Notice.
	</li>
	<li>
		We may also terminate this Agreement, with immediate effect, if we determine that any action or inaction by you places the health or safety of any person at immediate risk.
	</li>
	<li>
		In the event that this Agreement is terminated by either Party, we will pay the Eligible Costs in respect of any component of the Program completed on or before the effective date of termination, plus any earned Administration Fees but less any outstanding Overpayment, which will discharge us of all liability to you under this Agreement.
	</li>
</ol>

<h4>8&nbsp; Indemnity and Insurance</h4>
<ol class="list--lettered">
	<li>
		You will indemnify and save harmless the Province, its employees, agents and contractors, from and against any and all losses, claims, damages, actions, causes of action, costs and expenses that it or they may sustain, incur, suffer or be put to at any time either before or after the expiration or termination of this Agreement, where the same or any of them are based upon, arise out of or occur, directly or indirectly, by reason of any act or omission by you or by any of your agents, employees, officers, directors or sub-contractors (including any Third Party Service Providers) pursuant to this Agreement, excepting always liability arising out of the independent negligent acts of the Province.
	</li>
	<li>
		During the Term of this Agreement, you will provide, maintain and pay for insurance as would normally be carried by a reasonably prudent service provider operating in British Columbia and providing services similar to the Program Services. Where applicable, the Province shall be added as an additional insured to all insurance relevant to the Program Services and evidence of such insurance shall be provided by you on a Province of British Columbia Certificate of Insurance within 10 business days of being requested to do so by us.
	</li>
</ol>

<h4>9 &nbsp; Notice and Delivery</h4>
<ol class="list--lettered">
	<li>
		<p>In order to be effective, any legal notice required by this Agreement (&ldquo;<b>Notice</b>&rdquo;) must be in writing and delivered as follows:</p>

		<ol type="i">
			<li>
				<p>to you, at your address shown in the Approval Letter, by</p>
				<ol>
					<li>
						hand delivery (including by courier), in which case it will be deemed to be received on the day of its delivery, or
					</li>
					<li>
						prepaid mail, in which case it will be deemed to be received on the fifth business day after its mailing; or
					</li>
				</ol>
			</li>
			<li>
				<p>to us by:</p>
				<ol>
					<li>email to the following email address: <a href="mailto:@(Model.GrantProgramCode + "@gov.bc.ca")">@(Model.GrantProgramCode + "@gov.bc.ca")</a>,</li>
					<li>
						hand delivery (including courier), in which case it will be deemed to be received on the day of its delivery, to:<br />
						&nbsp;  &nbsp; <span style="text-decoration: underline;">2nd Floor, 1106 Cook Street, Victoria, BC, V8V 3Z9</span>, or
					</li>
					<li>
						prepaid mail, in which case it will be deemed to be received on the fifth business day after its mailing, to:<br />
						&nbsp;  &nbsp; <span style="text-decoration: underline;">PO Box 9189, Stn Prov Govt, Victoria, BC, V8W 9E6</span>
					</li>
				</ol>

			</li>
		</ol>
	</li>
	<li>
		Either Party may, from time to time, notify the other Party in writing of a change of address for delivery and, following the receipt of such Notice in accordance with subsection 9 a), the new address will, for the purposes of this section 9, be deemed the delivery address of the Party giving Notice.
	</li>
</ol>

<h4>10 &nbsp; Miscellaneous Terms</h4>
<ol class="list--lettered">
	<li>
		<p>
			The Schedules to this Agreement and the @Model.GrantProgramCode Program Requirements are an integral part of this Agreement as if included in the body of this Agreement.
		</p>
	</li>
	<li>
		<p>
			This Agreement together with any documents or other information referred to in it, including the Approval Letter, the @Model.GrantProgramCode Program Requirements and the Payment Guidelines, constitute the entire Agreement between the Parties with respect to the subject matter of this Agreement, with a descending order of precedence in the event of a conflict or inconsistency as follows: Schedule B, Schedule A, Approval Letter, @Model.GrantProgramCode Program Requirements.
		</p>
	</li>
	<li>
		<p>
			In this Agreement, unless the context requires otherwise, words using the singular form include the plural form and <em>vice versa</em>.
		</p>
	</li>
	<li>
		<p>
			The headings in this Agreement are inserted for convenience only and do not form part of this Agreement.
		</p>
	</li>
	<li>
		<p>
			No amendment to or modification of this Agreement will be effective unless it is in writing and signed by both Parties.
		</p>
	</li>
	<li>
		<p>
			Nothing in this Agreement operates as a consent, permit, approval or authorization by any Ministry or Branch of the Government of the Province of British Columbia for anything that, by statute, you are required to obtain, unless this Agreement expressly indicates otherwise.
		</p>
	</li>
	<li>
		<p>
			No term or condition of this Agreement and no breach of any such term or condition by you will be deemed to have been waived unless such waiver is in writing and signed by both Parties.
		</p>
	</li>
	<li>
		<p>
			Our written waiver of any breach by you of a term or condition of this Agreement will not be deemed a waiver of any other provision of this Agreement or of any prior or subsequent breach.
		</p>
	</li>
	<li>
		<p>
			This Agreement will be governed by and construed in accordance with the laws of the Province of British Columbia and the laws of Canada applicable therein.
		</p>
	</li>
	<li>
		<p>
			In the event of any dispute between the Parties arising out of or in connection with this Agreement, the following dispute resolution process will apply unless the Parties otherwise agree in writing:
		</p>
		<ol type="i">
			<li>the Parties must initially attempt to resolve the dispute through collaborative negotiation;</li>
			<li>if the dispute is not resolved through collaborative negotiation within 15 business days of the dispute arising, the Parties must then attempt to resolve the dispute through mediation under the rules of the Mediate BC Society; </li>
			<li>if the dispute is not resolved through mediation within 30 business days of the commencement of mediation, the dispute must be referred to and finally resolved by arbitration under the British Columbia Arbitration Act;</li>
			<li>unless the Parties otherwise agree in writing, an arbitration or mediation under this section will be held in Victoria, British Columbia; and</li>
			<li>unless the Parties otherwise agree in writing or, in the case of an arbitration, the arbitrator otherwise orders, the Parties must share equally the costs of a mediation or arbitration under this section other than those costs relating to the production of expert evidence or representation by counsel.</li>
		</ol>
	</li>
	<li>
		<p>
			If any provision of this Agreement or its application to any person or circumstance is found by a court of competent jurisdiction or, if applicable, an arbitrator, to be invalid or unenforceable to any extent, the remainder of this Agreement and the application of that provision to any other person or circumstance will not be affected or impaired and will be enforceable to the extent permitted by law.
		</p>
	</li>
	<li>
		<p>
			The provisions of the Approval Letter and sections 2, 5 d), 5 e), 7 d) and 8 of this Schedule B and any other provision(s) or section(s) of this Agreement (including this Schedule B) or the @Model.GrantProgramCode Program Requirements, which, by their terms or nature, are intended to survive the completion or termination of this Agreement or are necessary for the interpretation or enforcement of this Agreement, will continue in force indefinitely subject to any applicable limitation period prescribed by law, even after this Agreement ends.
		</p>
	</li>
</ol>
<br />
'
WHERE ID = 11;

-- Agreement Cover Letter
PRINT 'UPDATE Default CWRG Applicant Cover Letter Template'
UPDATE [dbo].[DocumentTemplates] 
SET [Body] = 
N'
@model CJG.Application.Business.Models.DocumentTemplate.GrantApplicationTemplateModel
<div style="float: left;">@CJG.Core.Entities.AppDateTime.UtcNow.ToString("MMMM dd, yyyy")</div>
<div style="float: right;">Application# @Model.FileNumber</div>
<br clear="all" />
<br />@string.Format("{0} {1}", Model.ApplicantFirstName, Model.ApplicantLastName)
<br />@Model.OrganizationLegalName
<br />@Model.ApplicantPhysicalAddress
<br />@Model.ApplicantPhoneNumber
<br />
<br />
<p>Attention:</p>
<p>
	Based upon your application for funding under the @Model.GrantStreamName stream of the Community Workforce Response Grant Program (&ldquo;<strong>@Model.GrantProgramCode Program</strong>&rdquo;),
	we are pleased to inform you that we have approved your application for the <strong>Program</strong>, including the <strong>Program Delivery Plan</strong> and <strong>Eligible Costs</strong> set out in Schedule A – Grant Services (&ldquo;<strong>Schedule A</strong>&rdquo;).
	Please refer to Schedule B – Definitions and General Terms (&ldquo;<strong>Schedule B</strong>&rdquo;) for additional defined terms used in this Approval Letter (Schedule B governs in the event of any inconsistency).
</p>

<p>
	The following provisions of this Approval Letter, along with Schedule A and Schedule B, will together form a &ldquo;Shared Cost Arrangement Agreement&rdquo; between you and the Province with respect to your delivery of the Program. 
	When the Agreement is in place, your primary obligations and the funding Claim process will be as follows:
</p>
<ol class="list--numbered">
	<li>
		<p>
			You must arrange and pay for all components of the Program delivered by Third Party Service Providers.   Should a Participant be required to directly pay for any Eligible Costs associated with the Program, you will be required to reimburse that Participant for those costs in full, even if they are later deemed to not be an Eligible Cost.
		</p>
	</li>
	<li>
		<p>
			Subject to the terms of this Agreement, the Province will compensate you for up to 100 percent of the Eligible Costs, to the amount(s) shown in Schedule A (&ldquo;<strong>Payment</strong>&rdquo;).
		</p>
	</li>
	<li>
		<p>
			Unless otherwise directed by the Province, you must use the Skills Training Grant System to electronically submit Claims and any Reports or other supporting documentation reasonably required by the Province. No Payment will be made until the Province has received a complete Claim.
		</p>
	</li>
	<li>
		<p>
			All Participants must have submitted complete Participant Information Forms using the Skills Training Grant System before any Payment can be made
		</p>
	</li>
	<li>
		<p>
			When any Payment is made to you based on an Invoice provided by a Third Party Service Provider, copies of Receipts from the Third Party Service Provider for the payment of each such Invoice must be submitted to the Province no later than 30 days following the making of the Payment. Unless otherwise approved in advance in writing, all Claims must be submitted in the Fiscal Year in which this Agreement is formed.
		</p>
	</li>
	<li>
		<p>
			Payments will normally be made within 30 days following receipt of a Claim.
		</p>
	</li>
	<li>
		<p>
			Claims for Eligible Costs are subject to audit and verification by the Province at any time, and original Receipts, Invoices and/or proof of expenditure records must be kept by you and made available for review for a minimum period of seven years following the end of the Term.
		</p>
	</li>
	<li>
		<p>
			You are responsible for any costs or expenses, including Eligible Program Delivery Costs, that are:
			<ol class="list--lettered">
				<li>
					not identified in Schedule A;
				</li>
				<li>
					over the maximum amount to be paid by the Province under this Agreement;
				</li>
				<li>
					at any time deemed by the Province to be ineligible, due to Participant or Third Party Service Provider ineligibility, or to be an Overpayment; or
				</li>
				<li>
					not supported by Receipts or other records in accordance with section 5 above; or
				</li>
				<li>
					not submitted as a Claim in the same Fiscal Year as this Agreement is formed in accordance with section 5 above.
				</li>
			</ol>
		</p>
	</li>
	<li>
		<p>
			Participants must:
			<ol class="list--lettered">
				<li>
					be at least 15 years old at the time of entry into the Program;
				</li>
				<li>
					be Canadian citizens, permanent residents or protected persons entitled to work in Canada; and
				</li>
				<li>
					meet any other eligibility requirements applicable to the particular @Model.GrantProgramCode Program funding stream under which the Program is being funded, as identified in the CWRG Program Requirements.
				</li>
			</ol>
		</p>
	</li>

	<li>
		<p>
			You are responsible for ensuring that Participants meet the above eligibility requirements before they enter the Program and that they submit Participant Information Forms using the Skills Training Grant System. If any Participants are found to be ineligible, the amount of the Payment will be prorated accordingly or, in the event of a Payment already made, the amount paid for any ineligible Participants will become an Overpayment.
		</p>
	</li>
	<li>
		<p>
			All or any part of any Payment made to you, including Administration Fees, may later be deemed by the Province, acting reasonably, to be an Overpayment (to the extent that value for money was not received, as determined by us in our sole discretion) for any of the following reasons:
			<ol class="list--lettered">
				<li>
					the Program was not delivered in whole or in part in accordance with the Schedule A;
				</li>
				<li>
					the Program is not completed by the end of the Term;
				</li>
				<li>
					your Claim included any items that were not Eligible Costs (including if those items are your responsibility in accordance with section 8 above;
				</li>
				<li>
					you fail to provide or ensure the provision of any Receipts or Report(s) that we require; or
				</li>
				<li>
					you receive funding or any refund from any other person or entity, including another government or governmental body, that covers any of the Eligible Costs.
				</li>
			</ol>
		</p>
	</li>
	<li>
		<p>
			At the end of the Term, any outstanding Overpayments will become a debt owing to the Province and must be repaid by you within 14 days.
		</p>
	</li>
	<li>
		<p>
			Within 30 days of the end of the entire Program, you must submit a Participant Completion Report.
		</p>
	</li>
	<li>
		<p>
			For greater certainty, you will have the autonomy, flexibility and discretion to deliver the Program Services set out in Schedule A in a manner that aligns with each Participant’s needs.  With respect to the funds identified in Schedule A for Employment Assistance Services and Participant Financial Supports, you will allocate and expend those funds across these two areas of Program Services in a manner that you believe will best support each Participant’s needs.
		</p>
	</li>
	<li>
		<p>
			You must comply with all parts of this Agreement (including Schedule B), the @Model.GrantProgramCode Program Requirements, and all applicable laws.
		</p>
	</li>
	<li>
		<p>
			The Province reserves the right to contact Participants, Third Party Service Providers, or any other person in order to substantiate Claim requests, Program Services activities, records or other matters pertaining to your obligations under or your participation in the @Model.GrantProgramCode Program.
		</p>
	</li>
</ol>
<p>
	If the Terms of this Agreement are acceptable to you and you wish to proceed with the Program, you must first review and confirm that you have read and understand each part of the Agreement, including Schedule A and Schedule B.  You must then agree to be bound by the Agreement by electronically accepting the Agreement using the &ldquo;Accept Agreement&rdquo; button on the Review and Accept Grant Agreement page.  If we do not receive your acceptance of the Agreement within 5 business days of the date this offer was sent to you, this approval will expire.
</p>
<p>
	If, after entering into the Agreement, you want to make a change to the Program (other than as set out in the following paragraph), including making a change to a Third Party Service Provider, you must use the Skills Training Grant System to submit a change request. The change request must be approved by the Province before the Program can begin.
</p>
<p>
	Start and end dates for certain components of the Program may be modified in the Skills Training Grant System at any time without prior approval as long as:
</p>
<ol class="list--numbered">
	<li>
		<p>
			the start date for each Program component still falls within the approved period set out in the Program Delivery Plan;
		</p>
	</li>
	<li>
		<p>
			the Eligible Costs for all of the components of the Program are still incurred in the Fiscal Year for which they were approved;
		</p>
	</li>
	<li>
		<p>
			the end date must not be more than one year after the start date; and
		</p>
	</li>
	<li>
		<p>
			no other material changes are made to the Program, or to any Third Party Service Provider.
		</p>
	</li>
</ol>
<p>
	If, before entering into the Agreement, you do not intend to proceed with the Program, you may reject this Agreement by clicking the &ldquo;Cancel&rdquo; button and then the &ldquo;Reject Agreement&rdquo; button in the Skills Training Grant System.
</p>
<p>
	Thank you for your participation in the @Model.GrantProgramName Program. The Province of British Columbia introduced the @Model.GrantProgramCode Program to assist communities, sectors and industries to respond to emerging, urgent labour market needs by providing skills and supports to unemployed and precariously employed British Columbians and connecting them to good-paying jobs in their communities.
</p>
<p>
	We are interested in your feedback and would appreciate learning from your experience with the @Model.GrantProgramCode Program. Please contact us with any questions or concerns at <a href="mailto:@(Model.GrantProgramCode + "@gov.bc.ca")">@(Model.GrantProgramCode + "@gov.bc.ca")</a>.
</p>
<p>Sincerely,</p> Director
<br /> @Model.GrantProgramName Program
<br />
<br />
<p>
	I am authorized to act and to enter into this Agreement on behalf of the Applicant. On the Applicant’s behalf, I do hereby accept and agree to all of the terms and conditions of this Agreement, including this Approval Letter and associated Schedule A and Schedule B.
</p>
'
WHERE ID = 12;

-- Agreement Schedule A
PRINT 'UPDATE Default CWRG Applicant Schedule A Template'
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
	<p><h3>Program Delivery Plan</h3></p>
	<div class="form__group__wrapper ">
		<div class="form__group two-col">
			<label class="form__label schedule-a__label" for="DefaultTrainingProgram_StartDate">Delivery Start Date:</label>
			<div class="form__control schedule-a__control">
				@Model.StartDate
			</div>
		</div>
	</div>
	<div class="form__group two-col">
		<label class="form__label schedule-a__label" for="DefaultTrainingProgram_EndDate">Delivery End Date:</label>
		<div class="form__control schedule-a__control">
			@Model.EndDate
		</div>
	</div>
	::RequestChangeTrainingDates::
	<br />
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
	<div class="form__group two-col">
		<label class="form__label schedule-a__label" for="GrantAgreement_ParticipantReportingDueDate">Participant Reporting Due Date:</label>
		<div class="form__control schedule-a__control">
			@Model.GrantAgreementParticipantReportingDueDate
		</div>
	</div>
	<div class="form__group two-col">
		<label class="form__label schedule-a__label" for="GrantAgreement_ReimbursementClaimDueDate">
			Number of Participants in Program:
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
				<th class="num-col">Number of participants</th>
				<th class="num-col">Average Cost per Participant</th>
				<th class="num-col">Total Maximum of Eligible Costs</th>
				@if (Model.ShowEmployerContribution)
				{
					<th class="num-col">@((Model.ProgramType == ProgramTypes.WDAService) ? "Applicant contribution" : "Employer contribution")</th>
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
				<td><b>Total Maximum of Eligible Costs</b></td>
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
			</tr>

		</tbody>
	</table>
	@if (Model.ProgramType == ProgramTypes.WDAService)
	{
		var includedServices = String.Join(" and ", Model.TrainingCost.EmploymentServicesAndSupports.Select(ec => ec.EligibleExpenseTypeServiceCategoryCaption));
		if (!String.IsNullOrWhiteSpace(includedServices))
		{
			<p class="schedule-a-ess-sum">@includedServices total maximum blended cost per Participant is @(ToCurrency(Model.TrainingCost.SumESSAverageCost))</p>
		}
	}
</div>
'
WHERE ID = 13;

CHECKPOINT