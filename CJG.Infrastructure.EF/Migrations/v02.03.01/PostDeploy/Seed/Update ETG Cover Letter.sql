PRINT 'Updating Document Template - ETG Cover Letter'

update [dbo].[DocumentTemplates]
set [Body] = N'@model CJG.Application.Business.Models.DocumentTemplate.GrantApplicationTemplateModel
<div style="float: left;">@CJG.Core.Entities.AppDateTime.UtcNow.ToString("MMMM dd, yyyy")</div>
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
	@if (Model.RequireAllParticipantsBeforeSubmission)
	{
	<li>Participants must be at least 16 years old at the time of your application for Training and be Canadian Citizens, Permanent Residents or Protected Persons entitled to
		work in Canada. You are responsible to ensure that Participants submit Participant Information Forms using the
		STG System. If a Participant is found to be ineligible, or if any person that participates in Training was not approved by the Province and listed in Schedule A, 
		payment will be prorated for eligible Participants only. Reimbursement payments will normally be made within 60 days following receipt of the Claim. 
		Any costs or expenses that are not reimbursed by us will remain your responsibility as part of the Employer Contribution.</li>
	}
	else
	{
	<li>Participants must be at least 15 years old at the time of your application for Training and be Canadian Citizens, Permanent Residents or Protected Persons entitled to
		work in Canada. You are responsible to ensure that Participants submit Participant Information Forms using the
		STG System. If a Participant is found to be ineligible, payment will be prorated for eligible Participants only. Reimbursement payments will normally be made within 60 days following receipt of the Claim. 
		Any costs or expenses that are not reimbursed by us will remain your responsibility as part of the Employer Contribution.</li>
	}
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
		@if (Model.RequireAllParticipantsBeforeSubmission)
		{
		<li><u>change the start and/or end of Training</u>, you may make these changes using the STG System
			without prior approval of the Province as long as Training starts within the approved
			Training period and no other material changes are made to the Program or Training Provider. The Training start
			date cannot be moved outside of the Training period for which the Training was approved;</li>
		<li><u>change Participants or any Participant costs</u>, you must withdraw or cancel your Agreement and submit a new application prior to the Training start date; or</li>
		}
		else
		{
		<li><u>change the start and/or end of Training</u>, you may make these changes using the STG System
			without prior approval of the Province as long as Training starts within the approved
			Training period and no other material changes are made to the Program or Training Provider. The Training start
			date cannot be moved outside of the Training period for which the Training was approved; or</li>		
		}
		<li><u>make any other changes</u>, you must first contact the Province by email at:
		<a href="mailto:@Model.GrantProgramEmail">@Model.GrantProgramEmail</a>.</li>
	</ol>
</p>
<p>Change requests can be made in the STG System. For step-by-step instructions, email
	<a href="mailto:@Model.GrantProgramEmail">@Model.GrantProgramEmail</a>.</p>
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
</div>',
[DateUpdated] = GETUTCDATE()

where Id = 1