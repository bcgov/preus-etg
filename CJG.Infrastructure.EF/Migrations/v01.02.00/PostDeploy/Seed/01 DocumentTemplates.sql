PRINT 'Updating [DocumentTemplates]'
UPDATE [dbo].[DocumentTemplates]
 SET [Body] =
  N'
<div style="float: left;">::AgreementIssueDate::</div>
<div style="float: right;">Application# ::FileNo::</div>
<br clear="all" />
<br />::ApplicationAdministratorName::
<br />::ApplicantBusinessName::
<br />::ApplicationAdministratorAddress::
<br />
<br />
<p>Attention:</p>
<p>Based upon your application for funding under the Canada-BC Job Grant program <strong>("CJG")</strong>, we are pleased to inform you that we have approved the Training Provider, Training and Eligible Training Costs set out in Schedule A – Grant Services.  Please refer to Schedule B for additional defined terms used in this <strong>"Approval Letter"</strong> (Schedule B governs if there is an inconsistency between provisions).</p>
<p>When you have accepted all parts of this agreement, the following provisions of this Approval Letter, along with the Schedules A – Grant Services (the Approved Training and Eligible Training Costs) and B – Definitions and General Terms, will form a Shared Cost Agreement between you and the Province.  When the Agreement is in place, your primary obligations and the grant reimbursement claim process will be as follows:</p>
<ol class="list--numbered">
<li>Subject to the terms of this Agreement, we will reimburse you for up to ::ReimbursementRate:: of the Eligible Training Costs, to the amount shown in Schedule A ("Reimbursement").</li>
<li>You must submit a claim for reimbursement of Eligible Training Costs using the Canada-BC Job Grant system, along with copies of all receipts and any Reports or other supporting documentation reasonably required by us.  This Reimbursement Claim may be submitted as soon as Training starts and must be received by us no later than 30 days after start of the Training program set out in Schedule A – Grant Services.  If a Reimbursement Claim is not received within the 30 day period, you will no longer be eligible for a grant.</li>
<li>You must arrange and pay for the Training in full, and the Participants must start their Training, in accordance with Schedule A – Grant Services, before a Reimbursement Claim can be submitted.</li>
<li>Participants must be at least 15 years old and be Canadian Citizens, Permanent Residents or Protected Persons entitled to work in Canada.  You are responsible to ensure that participants submit Participant Information Forms using the Canada-BC Job Grant system.  If a Participant is found to be ineligible, payment will be prorated for eligible Participants only.  Reimbursement payments will normally be made within 60 days following receipt of the Reimbursement Claim.  Any costs or expenses that are not reimbursed by us will remain your responsibility as part of the Employer Contribution.</li>
<li>All or any part of the Reimbursement paid to you may later be deemed by us to be an Overpayment if:
<ol class="list--lettered">
<li>the Participant does not complete their training, or the Training is not completed by the end of the Term;</li>
<li>you do not employ the Participant following the completion of their Training;</li>
<li>your Reimbursement Claim included any items that were not Eligible Training Costs;</li>
<li>you fail to provide any Reports or other information that we require;</li>
<li>you receive funding from any other person or entity, including another government or governmental body, that reimburses you for the portion of the Training that is the subject of this Agreement or you receive any form of refund from the Training provider.</li>
</ol>
</li>
<li>Any outstanding Overpayments will become a debt owing to the Province and must be repaid by you within 14 days.</li>
<li>You are responsible for:
<ol class="list--lettered">
<li>any Training costs (including Eligible Training Costs) beyond the maximum amount reimbursed by the Province under this Agreement (capped at ::MaxReimbursementAmt:: per Participant per Fiscal Year);</li>
<li>any Training costs that are at any time deemed ineligible, due to Employer, Participant, Training or Training provider ineligibility; and </li>
<li>any Eligible Training Costs not submitted to the Province as part of the Reimbursement Claim within the 30 day claim period following the start of each Training program set out in Schedule A.</li>
</ol>
</li>
<li>No Reimbursement will be made until the Province has received a complete Reimbursement Claim.</li>
<li>In addition to the Reimbursement Claim, you will be required to submit an "Employer Completion Report" at the end of training. </li>
<li>Reimbursement of Eligible Training Costs is subject to audit and verification by the Province at any time and original receipts and/or proof of expenditure records must be kept by you and made available for review for a minimum period of seven years.</li>
<li>You must also comply with all other parts of this Agreement (including Schedule B), the Program Requirements and all applicable laws.</li>
</ol>
<p>If after electronically signing the Agreement, you want to make a change to the Training provider, you must use the Canada-BC Job Grant system to submit a change request to the Province.  Your Training provider change request must be approved by the Province before Training begins.  See the Agreement Schedule A – Grant Services in the system for how to submit a change request. </p>
<p>Training Start and end dates may be modified in the Canada-Job Grant system without prior approval of the Province as long as Training starts within the approved Training period and no other material changes are made to the Training program or Training provider.  The Training start date cannot be moved outside of the Training period for which the Training was approved.</p>
<p>If, after you have electronically signed the Agreement, you want to make any other changes to the Training, you must first contact the Province.</p>
<p>If, before electronically signing the Agreement, you do not intend to proceed with the Training, you may reject this Agreement (by clicking Cancel below and then Reject Agreement).</p>
<p>If the terms of this Agreement are acceptable to you and you wish to proceed with the Training, you must first review and confirm you have read and understand each section of the Agreement including Schedules A and B.  Then, in order to obtain a grant, you must accept by electronically signing the agreement using the Accept Agreement button on the Review and Accept Grant Agreement page.  If we do not receive your acceptance of the Agreement by way of your electronic signature by the date set out in Schedule A – Grant Services, this agreement will expire.</p>
<p>Thank you for your participation in the Canada-BC Job Grant program.  The Government of Canada introduced the Canada Job Grant to help better align skills training with employer needs, create more jobs for Canadians and build a stronger, more skilled workforce.  Employers are seen as partners in the skills training system.  By sharing in the associated costs, you are demonstrating your commitment to the continued success of the Canadian economy.  We are interested in your feedback and would appreciate learning from your experience with the program.  Please contact us with any questions or concerns using the Contact US link below.</p>
<p>Sincerely,</p>
Director<br />
Canada-BC Job Grant Program<br />
<br />
<p>I am authorized to act and to enter into this Agreement on behalf of the Employer. On the Employer''s behalf, I do hereby accept and agree to the terms and conditions of this Agreement, including this Approval Letter and the attached Schedules A - Grant Services (Approved Training and Eligible Training Costs) and Schedule B - Definitions and General Terms.</p>
<p>Note:  If the Employer''s signing authority is also a participant under this Agreement but is not an owner or co-owner of the business, he/she cannot sign on the Employer''s behalf.  In this case, please ensure an alternate signing authority signs this Agreement on behalf of the Employer.</p>
',
	[DateUpdated] = GETUTCDATE()
WHERE [Id] = 1