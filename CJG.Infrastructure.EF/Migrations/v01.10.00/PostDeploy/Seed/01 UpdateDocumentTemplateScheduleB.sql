PRINT 'UPDATE [DocumentTemplates] - CWRG - Schedule B'

UPDATE [dbo].[DocumentTemplates]
SET [Body] =
N'@model CJG.Core.Entities.GrantApplication
<h4>1 &nbsp; Definitions</h4>

<ul class="info-list">
	<li><p>In this Agreement (including the Approval Letter), in addition to terms defined in the Approval Letter:</p></li>
	<li>&ldquo;<b>Administration Fees</b>&rdquo; means the amount available to the Applicant to offset costs associated with project management, the outreach to and recruitment of Participants, and the preparation and submission of Claims and Reports, to the maximum amount set out in Schedule A;</li>
	<li>&ldquo;<b>Agreement</b>&rdquo; means the Shared Cost Arrangement Agreement between the Province and the Applicant which will be formed upon the Applicant’s acceptance of the terms and provisions of the Approval Letter, Schedule A and Schedule B; </li>
	<li>&ldquo;<b>Applicant</b>&rdquo;, &ldquo;<b>you</b>&rdquo; or &ldquo;<b>your</b>&rdquo; means the organization that applied for a grant under the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program and to whom the Approval Letter has been addressed;</li>
	<li>&ldquo;<b>Claim</b>&rdquo; means the submission of any Receipts or Invoices in order to claim a Payment;</li>
	<li>&ldquo;<b>@Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program Requirements</b>&rdquo; means the general principles, intent, policy, criteria, requirements and Participant and Applicant obligations relating to the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program, as may be amended by the Province in its sole discretion from time to time, which can be viewed on the ‘Grant Overview’ page at https://www.workbc.ca/Employment-Services/Community-Workforce-Response-Grant/Grant-Overview.aspx, or such other location as we may specify from time to time;
	<li>&ldquo;<b>Eligible Costs</b>&rdquo; means those costs associated with providing Program Services that are eligible for Payment in strict accordance with the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program Requirements and this Agreement, up to the maximum amount set out for each Participant, and in aggregate, in Schedule A and, for Payment purposes, including the Administration Fee that may be earned by the Applicant up to the maximum amount set out in Schedule A;</li>
	<li>&ldquo;<b>Employment Assistance Services</b>&rdquo; (if included in Schedule A) means the component of Program Services provided to Participants during their participation in the Program which are intended to enable labour market participation and increase employability, using services such as assessments, training plan development, job readiness skills development (including resume writing and interview skills), mentoring, coaching and indigenous cultural supports, in accordance with the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program Requirements and as set out in more detail in Schedule A;</li>
	<li>&ldquo;<b>Fiscal Year</b>&rdquo; means the period beginning on April 1 and ending on March 31 of the following calendar year;</li>
	<li>&ldquo;<b>Invoice</b>&rdquo; means the document(s) related to a request for Payment, without a Receipt,  of any Eligible Costs;</li>
	<li>&ldquo;<b>Overpayment</b>&rdquo; means any and all amounts provided by the Province to the Applicant that are not expended during the Term on Eligible Costs or that are otherwise deemed ineligible in accordance with this Agreement;</li>
	<li>&ldquo;<b>Participant</b>&rdquo; means each individual that the Applicant has enrolled in the Program and   that is receiving, or has received, Program Services;</li>
	<li>&ldquo;<b>Participant Completion Report</b>&rdquo; means the final report, as required by the Province upon the earlier of the completion date of the entire Program or the end of the Term, providing details regarding Program completion and the achievement of Outcomes for all Participants, in a form to be provided by the Province; </li>
	<li>&ldquo;<b>Participant Information Forms</b>&rdquo; means Participant personal information forms that are to be submitted directly by all Participants via the Skills Training Grant System;</li>
	<li>&ldquo;<b>Participant Financial Supports</b>&rdquo; (if included in Schedule A) means the financial supports component of the Program Services provided to Participants during their participation in the Program that are intended to remove barriers to their success in the Program, such as costs for childcare, transportation, personal protective gear, required uniforms, travel, accommodation and disability supports, in accordance with the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program Requirements and as set out in more detail in Schedule A;</li>
	<li>&ldquo;<b>Parties</b>&rdquo; means, collectively, the Applicant and the Province;</li>
	<li>&ldquo;<b>Party</b>&rdquo; means either the Applicant or the Province, as the context requires;</li>
	<li>&ldquo;<b>Program</b>&rdquo; means the entire project proposed by the Applicant and approved by the Province pursuant to the Approval Letter, for the provision of the Program Services to Participants in accordance with the Agreement;</li>
	<li>&ldquo;<b>Program Delivery Plan</b>&rdquo; means the plan for the Applicant’s delivery of the Program, as approved by the Province;</li>
	<li>&ldquo;<b>Program Services</b>&rdquo; means all of the Employment Assistance Services, Participant Financial Supports, and Skills Training to be provided to Participants under the Program, as set out in Schedule A;</li>
	<li>&ldquo;<b>Province</b>&rdquo; •	&ldquo;&rdquo;, &ldquo;we&rdquo;, &ldquo;us&rdquo; or &ldquo;our&rdquo; means Her Majesty the Queen in right of the Province of British Columbia, represented by the Minister of Advanced Education, Skills and Training;</li>
	<li>&ldquo;<b>Receipt</b>&rdquo; means the document(s) that are related to and that verify a request for a Payment for any Employment Assistance Services, Participant Financial Supports or Training expenses already incurred by the Applicant;</li>
	<li>&ldquo;<b>Reports</b>&rdquo; means, collectively, any and all reports or other information as may at any time be required under the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program Requirements or this Agreement, which are to be provided to the Province by the Applicant or by any Participant; </li>
	<li>&ldquo;<b>Skills Training Grant System</b>&rdquo; means the web-based @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Grant application system through which applications, change requests, Participant Information Forms, Claims, and Reports are to be submitted;</li>
	<li>&ldquo;<b>Skills Training Program</b>&rdquo; means the mandatory training component of the Program Services, which must be 52 weeks or less in duration and delivered by a Third Party Service Provider to Participants during their participation in the Program. It is intended to provide Participants with skills that are necessary to be successful in a job, such as occupational, essential or soft skills, whether provided in a classroom setting and/or online, in accordance with the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program Requirements and as set out in more detail in Schedule A; </li>
	<li>&ldquo;<b>Term</b>&rdquo; •	 &ldquo;&rdquo; means the period that begins and, unless otherwise agreed in writing or this Agreement is terminated earlier in accordance with its terms, ends on the dates set out in Schedule A; </li>
	<li>&ldquo;<b>Third Party Service Provider</b>&rdquo; means any third party service provider that is eligible, in accordance with the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program Requirements, and at arms’ length from the Applicant and is chosen by the Applicant and listed in Schedule A or, in the case of any Participant Financial Supports, is chosen by the Participant, to provide any Program Services; and</li>
	<li>&ldquo;<b>Training</b>&rdquo; means a Participant’s participation in, and receipt of services under, the Skills Training Program.</li>
</ul>

<h4>2 &nbsp; Applicant Obligations</h4>
<p>In addition to your other obligations set out elsewhere in this Agreement, including the Approval Letter, you must:</p>
<ol class="list--lettered">
	<li>submit a Claim in order to request Payment for Eligible Costs, supported by Invoices, Receipts or other verification documentation acceptable to and as required by the Province, and in accordance with the &ldquo;Payment Guidelines&rdquo; attached as Appendix 1 to this Schedule B;</li>
	<li>upon our request, promptly inform us regarding Participants’ Training and employment status and provide any additional information we may reasonably require;</li>
	<li>prior to making any public announcements with respect to your participation in the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program, obtain (by email request to <a href="mailto:@(Model.GrantOpening.GrantStream.GrantProgram.ProgramCode + "@gov.bc.ca")">@(Model.GrantOpening.GrantStream.GrantProgram.ProgramCode + "@gov.bc.ca")</a> or such other address as we may specify from time to time) and comply with the most current &ldquo;Marketing, Publicity and Communications Guidelines&rdquo; and any other directions regarding such announcements and your acknowledgment of funding received from the Province and the Government of Canada for the Program;</li>
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
	<li>you are not in breach of, or in default under, any law, statute or regulation of Canada or of the Province of British Columbia that is relevant to the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program or the subject matter of this Agreement.</li>
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
					<li>email to the following email address: <a href="mailto:@(Model.GrantOpening.GrantStream.GrantProgram.ProgramCode + "@gov.bc.ca")">@(Model.GrantOpening.GrantStream.GrantProgram.ProgramCode + "@gov.bc.ca")</a>,</li>
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

<h4>10 &nbsp;	Miscellaneous Terms</h4>
<ol class="list--lettered">
	<li>
		<p>
			The Schedules to this Agreement and the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program Requirements are an integral part of this Agreement as if included in the body of this Agreement.
		</p>
	</li>
	<li>
		<p>
			This Agreement together with any documents or other information referred to in it, including the Approval Letter, the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program Requirements and the Payment Guidelines, constitute the entire Agreement between the Parties with respect to the subject matter of this Agreement, with a descending order of precedence in the event of a conflict or inconsistency as follows: Schedule B, Schedule A, Approval Letter, @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program Requirements.
		</p>
	</li>
	<li>
		<p>
			In this Agreement, unless the context requires otherwise, words using the singular form include the plural form and vice versa.
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
			The courts of the Province of British Columbia, sitting in Victoria, will have the exclusive jurisdiction to hear any disputes arising from or in any way related to this Agreement or the relationship of the Parties.
		</p>
	</li>
	<li>
		<p>
			If any provision of this Agreement or its application to any person or circumstance is found by a court of competent jurisdiction to be invalid or unenforceable to any extent, the remainder of this Agreement and the application of that provision to any other person or circumstance will not be affected or impaired and will be enforceable to the extent permitted by law.
		</p>
	</li>
	<li>
		<p>
			The provisions of the Approval Letter and sections 2, 5 d), 5 e), 7 d) and 8 of this Schedule B and any other provision(s) or section(s) of this Agreement (including this Schedule B) or the @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program Requirements, which, by their terms or nature, are intended to survive the completion or termination of this Agreement or are necessary for the interpretation or enforcement of this Agreement, will continue in force indefinitely subject to any applicable limitation period prescribed by law, even after this Agreement ends.
		</p>
	</li>
</ol>
<br />'
WHERE ID = 11