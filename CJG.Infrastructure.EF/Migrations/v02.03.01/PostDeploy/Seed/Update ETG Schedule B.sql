PRINT 'Updating Document Template - ETG Schedule B'

update [dbo].[DocumentTemplates]
set [Body] = N'@model CJG.Application.Business.Models.DocumentTemplate.GrantApplicationTemplateModel
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
		<a href="mailto:@Model.GrantProgramEmail">@Model.GrantProgramEmail</a> or such other address as we may specify from time to time; </li>
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
@if (Model.RequireAllParticipantsBeforeSubmission)
{
<p>
	<b>15. </b> &nbsp; In order to be effective, any legal notice required by this Agreement (&ldquo;
	<b>Notice</b>&rdquo;) must be in writing and delivered as follows:</p>
}
else
{
<p>
	<b>15. </b> &nbsp; In order to be effective, this agreement and any legal notice required by this Agreement (&ldquo;
	<b>Notice</b>&rdquo;) must be in writing and delivered as follows:</p>
}
<ol class="list--lettered">
	<li> to you, to your mailing address shown in the Approval Letter
		<ol class="list--numerals">
			<li>by hand (including by courier), in which case it will be deemed to be received on the day of its delivery, or</li>
			<li>by prepaid mail, in which case it will be deemed to be received on the fifth business day after its mailing; or</li>
		</ol>
	</li>
	<li> to us:
		<ol class="list--numerals">
			@if (Model.RequireAllParticipantsBeforeSubmission)
			{
			<li> by hand (including by courier), in which case it will be deemed to be received on the day of its delivery, to:
				<br /> &nbsp; &nbsp;
				<span style="text-decoration: underline;">ETG Appeals. 2nd Floor, 1106 Cook Street, Victoria, BC, V8V 3Z9</span>
			</li>
			}
			else
			{
			<li> by hand (including by courier), in which case it will be deemed to be received on the day of its delivery, to:
				<br /> &nbsp; &nbsp;
				<span style="text-decoration: underline;">2nd Floor, 1106 Cook Street, Victoria, BC, V8V 3Z9</span>
			</li>
			}
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
<br />',
[DateUpdated] = GETUTCDATE()

where Id = 3