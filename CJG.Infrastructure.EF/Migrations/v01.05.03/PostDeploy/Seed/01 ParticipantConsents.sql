PRINT 'Updating [ParticipantConsents]'

UPDATE [dbo].[ParticipantConsents]
SET [IsActive] = 0

INSERT [dbo].[ParticipantConsents]
 ([IsActive], [Description]) VALUES
 (1, 
N'<h2 class="subheader">Personal Information</h2>

<p>
	The Government of Canada ("Canada") provides funding for the Canada-B.C. Job Grant ("CJG") under the Canada-British Columbia Job Fund Agreement ("CJF"), administered by the British Columbia Ministry of Advanced Education, Skills and Training ("AEST"), and under the Labour Market Development Agreement ("LMDA"), administered by the British Columbia Ministry of Social Development and Poverty Reduction ("SDPR"). 
</p>
<p>
	All personal information related to your participation in a CJG training opportunity ("Personal Information") is collected pursuant to sections 26(c) and 26(e) of the Freedom of Information and Protection of Privacy Act ("FOIPPA").  This information will be used for administrative, evaluation, research, accountability, and reporting purposes, including to determine your eligibility for participation in the CJG program and to meet federal reporting requirements under the CJF and LMDA.
</p>
<p>
	Under the authority of Section 33.1(d) and 33.2(d) of FOIPPA, the personal information collected may be disclosed to the Department of Employment and Skills Development Canada ("ESDC") or the Canada Employment Insurance Commission ("CEIC") for the purposes of administering the Employment Insurance Act.  The CEIC may also use your Personal Information for policy analysis, research or evaluation purposes.  
</p>
<p>
	Under the authority of Section 3.6 of the LMDA, if you are currently in receipt of Employment Insurance (EI) benefits, a referral under Section 25 of the EI Act may be placed on your EI claim, to allow you to continue to receive EI benefits, up to the end of your EI benefit period, while you participate in the Canada-BC Job Grant program. 
</p>
<p>
	All CJG training participants are required to complete two satisfaction surveys at approximately 3 months and 12 months following completion of their training ("Surveys").  The Surveys will ask basic questions about the outcomes of training and whether the training met your employment needs.  Your contact information will be shared with British Columbia''s statistical agency, BC Stats, in order for them to contact you to conduct these Surveys.
</p>
<p>
	You may also be asked if you wish to, or you may volunteer to, provide a testimonial regarding your CJG training experience ("Testimonial").  Testimonials, and any Personal Information that you choose to include in a Testimonial, may be used and disclosed to the public to promote the CJG program.
</p>

<h2>Consent and Certification</h2>

<p>
	Effective as of the date set out below, and in consideration of the opportunity for me to participate in CJG training, I:
</p>
<ul>
	<li>consent to the collection use, and disclosure of my Personal Information for purposes set out above; </li>
	<li>consent to my Personal Information being used to contact me to conduct the Surveys and to request a Testimonial;</li>
	<li>certify that all of the information that I have provided in this form is accurate and complete; and</li>
	<li>certify that I understand that I am expected to complete my training and I must complete the Surveys in order for my employer to meet all of its Canada-BC Job Grant Agreement obligations;</li>
</ul>
<p>
	By confirming my consent, I acknowledge and agree that this typed signature has the same legal effect as a written signature.
</p>
<hr>
<p>
	If you have any questions about the collection, use or disclosure of your Personal Information, please contact the Program Manager, Canada-BC Job Grant at 1-877-952-6914, or by mail at: Program Manager, Canada-BC Job Grant, 2nd Floor, 1106 Cook Street, Victoria, BC, V8V 3Z9 or by submitting an email to <a href="mailto:CJGInfo@gov.bc.ca">CJGInfo@gov.bc.ca</a> 
</p>')