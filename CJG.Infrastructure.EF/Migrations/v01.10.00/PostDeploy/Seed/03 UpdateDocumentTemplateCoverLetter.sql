PRINT 'UPDATE [DocumentTemplates] - CWRG - Cover Letter'

UPDATE [dbo].[DocumentTemplates]
SET [Body] =
N'@model CJG.Core.Entities.GrantApplication
<div style="float: left;">@CJG.Core.Entities.AppDateTime.UtcNow.ToString("MMMM dd, yyyy")</div>
<div style="float: right;">Application# @Model.FileNumber</div>
<br clear="all" />
<br />@string.Format("{0} {1}", Model.ApplicantFirstName, Model.ApplicantLastName)
<br />@Model.Organization.LegalName
<br />@Model.ApplicantPhysicalAddress
<br />@Model.ApplicantPhoneNumber
<br />
<br />
<p>Attention:</p>
<p>
	Based upon your application for funding under the @Model.GrantOpening.GrantStream.Name stream of the Community Workforce Response Grant Program (&ldquo;<strong>@Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program</strong>&rdquo;),
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
					meet any other eligibility requirements applicable to the particular @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program funding stream under which the Program is being funded, as identified in the CWRG Program Requirements.
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
			You must comply with all parts of this Agreement (including Schedule B), the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program Requirements, and all applicable laws.
		</p>
	</li>
	<li>
		<p>
			The Province reserves the right to contact Participants, Third Party Service Providers, or any other person in order to substantiate Claim requests, Program Services activities, records or other matters pertaining to your obligations under or your participation in the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program.
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
	Thank you for your participation in the @Model.GrantOpening.GrantStream.GrantProgram.Name Program. The Province of British Columbia introduced the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program to assist communities, sectors and industries to respond to emerging, urgent labour market needs by providing skills and supports to unemployed and precariously employed British Columbians and connecting them to good-paying jobs in their communities.
</p>
<p>
	We are interested in your feedback and would appreciate learning from your experience with the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program. Please contact us with any questions or concerns at <a href="mailto:@(Model.GrantOpening.GrantStream.GrantProgram.ProgramCode + "@gov.bc.ca")">@(Model.GrantOpening.GrantStream.GrantProgram.ProgramCode + "@gov.bc.ca")</a>.
</p>
<p>Sincerely,</p> Director
<br /> @Model.GrantOpening.GrantStream.GrantProgram.Name Program
<br />
<br />
<p>
	I am authorized to act and to enter into this Agreement on behalf of the Applicant. On the Applicant’s behalf, I do hereby accept and agree to all of the terms and conditions of this Agreement, including this Approval Letter and associated Schedule A and Schedule B.
</p>
'
WHERE ID = 12