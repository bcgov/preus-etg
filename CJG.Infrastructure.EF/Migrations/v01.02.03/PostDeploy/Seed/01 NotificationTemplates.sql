UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = N'<!DOCTYPE html><html><head/><body><p>Hello ::AAFirstName::,</p><p>Your Canada-BC Job Grant application for "::TrainingProgramTitle::" has been received and assigned the file number ::FileNumber::. Future emails regarding the application will have the file number in the subject line and it is your reference to the file in the <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a>.</p><p> PLEASE NOTE: You will be informed of decisions within 60 days of submitting an application. If training proceeds before receiving a decision from the Ministry, the employer will be liable for all incurred costs should the application be denied.</p><p><b>IMPORTANT! Participants on Employment Insurance (EI) or British Columbia Employment Assistance clients in receipt of Income Assistance</b></p><p>Individuals who are active claimants in receipt of Employment Insurance (EI) Part I benefits or British Columbia Employment Assistance clients in receipt of Income Assistance (IA) may be eligible for training funded through the CJG if prior approval is obtained from the Ministry of Social Development and Social Innovation (SDSI).  EI or IA clients who do not obtain pre-approval before participating in training may become ineligible for continued financial supports under EI or IA. All Participant Information Forms are due no less than 5 business days prior to the start of training.  However, if you are applying for Participants who may currently be Employment Insurance or Income Assistance Clients, please send notification to the <a href="mailto:CJGInfo@gov.bc.ca">CJGInfo@gov.bc.ca</a>.</p><p>Canada-BC Job Grant Team</p></body></html>' 
WHERE [Id] = 01

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Hello ::AAFirstName::,</p><p>We are pleased to inform you that your Canada-BC Job Grant application ::FileNumber::, "::TrainingProgramTitle::", has been assessed and an offer in the form of a Grant Agreement has been added to your file for your acceptance.</p><p>In order to obtain your grant, please log in to the system at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and review and confirm your acceptance of the Grant Agreement by ::AgreementAcceptanceDueDate:: in order to secure funding.</p><p>PLEASE NOTE: Reimbursement of eligible travel expenses is conditional upon all requirements being met at the time you submit your reimbursement claim.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 02

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Congratulations ::AAFirstName::,</p><p>Your Canada-BC Job Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", is approved.</p><p>You may login to the system at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and begin inviting participants to submit their Participant Information Forms. Please ensure all Participant Information Forms are submitted by ::ParticipantReportDueDate::, five days before the training start date.</p><p>Refer to your Grant Agreement Schedule A for your reporting deadlines.</p><p>If you need to change your training start date or request a change to your training provider, login at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and view your Grant Agreement Schedule A where you can request these changes.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 03

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >A Canada-BC Job Grant Agreement #::FileNumber:: was issued to you for your acceptance on ::AgreementIssueDate::.</p><p >The due date for your acceptance of this offer (::AgreementAcceptanceDueDate::) has passed and the offer may be withdrawn at any time. If you wish to take advantage of the grant you must login at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and accept the agreement today.</p><p >Failure to accept this offer may result in it being rescinded so funding can be reinvested to support training opportunities for other employers.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 04

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Hello ::AAFirstName::,</p><p >Your training start date for Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", is fast approaching!</p><p >All Participant Information Forms must be submitted by ::ParticipantReportDueDate:: (five days before the training start date). If you have not already done so, login to the system at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and invite participants to submit their Participant Information Forms.</p><p >Refer to your Schedule A for your reporting deadlines.</p><p >If you need to change your training start date or request a change to your training provider, login at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and view your Grant Agreement Schedule A where you can request these changes.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 05

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >For your Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", Participant Information Forms are due today and ::NumberofParticipants:: out of a possible ::MaximumNumberofParticipants:: have submitted their Participant Information Forms.</p><p >You will not be able to claim reimbursement for participants who do not submit a Participant Information Form. Please login at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and check your participant list. Make sure it is complete and accurate.</p><p >If you need to, invite participants to submit their Participant Information Forms today and follow up with them to make sure they understand the purpose of the invitation and what is required.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 06

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >For Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", your training start date has passed and no participants have submitted their Participant Information Forms.</p><p >If the training did not take place, please login at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and cancel the agreement.</p><p >If training did take place, please login at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and invite participants to submit their Participant Information Forms today and follow up with them to make sure they understand the purpose of the invitation and what is required.</p><p >You will not be able to claim reimbursement for participants who do not submit a Participant Information Form.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 07


UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >For Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", your training start date has passed and ::NumberofParticipants:: out of a possible ::MaximumNumberofParticipants:: have submitted their Participant Information Forms.</p><p >If your participant list is complete and accurate, you may begin preparing your reimbursement claim now. Make sure it is submitted by ::ClaimReportDueDate::.</p><p >You will not be able to claim reimbursement for participants who do not submit a Participant Information Form.</p><p >Login to the system at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> for your next step. </p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 08

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >For Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", your reimbursement claim submission deadline is approaching on ::ClaimReportDueDate::; please make sure that you login at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and prepare and submit your reimbursement claim by this date in order to receive reimbursement.</p><p >Please ensure all participants have submitted their Participant Information Forms. You will not be able to claim reimbursement for participants who do not submit a Participant Information Form.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 09

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >For Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", your reimbursement claim has not been received by the deadline in your agreement and no Participant Information Forms have been submitted.</p><p >If you intend to submit a reimbursement claim please do so today by logging in at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a>. You will not be eligible to claim reimbursement for participants that do not submit Participant Information Forms.</p><p >The Ministry may cancel your Grant Agreement at any time to reinvested funding for other employer training opportunities.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 10

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >Your reimbursement claim for Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", has not been received by the deadline in the Grant Agreement.</p><p >You have reported ::NumberofParticipants:: out of a possible ::MaximumNumberofParticipants::. You will not be eligible to claim reimbursement for participants that do not submit Participant Information Forms.</p><p >If you intend to submit a claim you should login at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and enter and submit your claim today.</p><p >The Ministry may cancel your Grant Agreement at any time to reinvested funding for other employer training opportunities.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 11

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >Your training end date has passed for Grant Agreement #::FileNumber::, "::TrainingProgramTitle::".</p><p >Please login at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and enter your Completion Report before ::CompletionReportDueDate::.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 12


UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >Your Completion Report deadline has passed for Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", and only ::ParticipantsWithCompletionReport:: out of ::MaximumNumberofParticipants:: have been reported.</p><p >Please login at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and enter your Completion Report for all participants. Failing to report completion may impact your ability to use the Canada-BC Job Grant in the future.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 13

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >Your training provider change request for Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", has been accepted.</p><p >Please login to the system at: <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and review the change to your grant agreement.</p></body><p>Canada-BC Job Grant Team</p></html>'
WHERE [Id] = 14

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >Your training provider change request for Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", has been denied for the following reason:</p><p >::CRDeniedReason::</p><p >You may login to the system at: <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> to submit another request.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 15

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >Your Canada-BC Job Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", has been cancelled for the following reason:</p><p >::CancellationReason::</p><p >The file has been closed. Please login at: <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> if you wish to view your file.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 16

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >We have received your reimbursement claim and supporting documentation for Grant Agreement #::FileNumber::, "::TrainingProgramTitle::".</p><p >Your claim has been placed in the queue for processing. Requests are reviewed in the order they are received. Should additional information be required, a program representative may contact you. If you have any questions about your claim, please contact us at <a href="mailto:cjgreimbursement@gov.bc.ca">cjgreimbursement@gov.bc.ca</a></p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 17

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >Your reimbursement claim for Grant Agreement #::FileNumber::, "::TrainingProgramTitle::," has been processed and a reimbursement in the amount of ::ReimbursementPayment:: will be sent to you by electronic funds transfer or by a cheque mailed to the business mailing address you submitted with your application.</p><p >If you have any questions, please contact us at cjgreimbursement@gov.bc.ca</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 18

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body>
<p>Application #::FileNumber:: </p>
<br />::AALastName::, ::AAFirstName:: 
<br />::ApplicantBusinessName:: 
<div style="float: left; display: block; padding-top:5px; padding-bottom: 5px; padding-left: 10px">
::ApplicationAdministratorAddress:: 
</div>
<p >Attention: ::AALastName::, ::AAFirstName::</p>
<p >Thank you for your application to the Canada-BC Job Grant Program. After assessment of your Reimbursement Claim, it has been determined that eligibility measures for the Canada-BC Job Grant Program have not been met: </p>
<p >::ReimbursementClaimDeniedReason:: </p>
<p >If you have any questions regarding this decision, please do not hesitate to contact the Canada-BC Job Grant Program. </p>
<p >Sincerely,  </p>
<br />Director 
<br />Canada-BC Job Grant Program 
</body></html>'
WHERE [Id] = 19

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >Your Canada-BC Job Grant reimbursement claim for Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", has been returned to you for the following reason:</p><p >::ReimbursementClaimReturnedReason::</p><p >Please log in at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> and adjust your claim to be compliant with your Agreement and resubmit it to the Ministry.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 20

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >Your Canada-BC Job Grant application #::FileNumber::, "::TrainingProgramTitle::", has been denied for the following reason: </p><p >::DeniedReason::</p><p >Please login to at <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> to see the details of your assessment.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 21

UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >The offer to you for your Canada-BC Job Application #::FileNumber::, "::TrainingProgramTitle::", has been withdrawn.</p><p >Your application file status shows Agreement Withdrawn and the file has been closed. Please login at: <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a> if you wish to view your application.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 22


UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = 
N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >Thank you for your Canada-BC Job Grant Application #::FileNumber::, "::TrainingProgramTitle::". Your application has not been assessed because all grant funds have been committed for your grant selection: "::StreamName::" for training starting in the period ::TrainingPeriodStartDate:: to ::TrainingPeriodEndDate::.</p><p >Please reapply for the next available grant opening.</p><p>Canada-BC Job Grant Team</p></body></html>'
WHERE [Id] = 23