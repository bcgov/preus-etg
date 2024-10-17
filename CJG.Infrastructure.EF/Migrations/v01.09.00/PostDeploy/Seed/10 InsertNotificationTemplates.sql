PRINT 'INSERT [NotificationTemplates] - CWRG'

SET IDENTITY_INSERT [dbo].[NotificationTemplates] ON 
-- 1
INSERT [dbo].[NotificationTemplates] ([Id], [AlertCaption], [EmailSubject], [EmailBody], [DefaultExpiryDays], [DateAdded], [DateUpdated]) 
VALUES 
	(25, N'@Model.ProgramName File #@Model.FileNumber Your Application has been Received', N'@Model.ProgramName File #@Model.FileNumber Your Application has been Received', N'<!DOCTYPE html>
<html>
<head />
<body>
    <p>Hello @Model.RecipientFirstName,</p>
    <p>Your @Model.ProgramName application for "@Model.ProgramTitle" has been received and assigned the file number @Model.FileNumber. Future emails regarding the application will have the file number in the subject line and it is your reference to the file in the <a href="@Model.BaseURL">@Model.BaseURL</a>.</p>
    <p>PLEASE NOTE: You will be informed of decisions within 60 days of submitting an application. If training proceeds before receiving a decision from the Ministry, the Applicant will be liable for all incurred costs should the application be denied.</p><p>
    <b>IMPORTANT!</b> 
    <p>Participants who are currently Employment Insurance or Income Assistance Clients must have approval prior to the start of training if they wish to maintain their financial supports. Refer to Appendix A of the General Program Criteria for details. EI or IA clients who do not obtain pre-approval before participating in training may become ineligible for continued financial supports under EI or IA. All Participant Information Forms are due no less than 5 business days prior to the start of training. However, if you are applying for Participants who may currently be Employment Insurance or Income Assistance Clients, please send notification to the<a href="mailto:@(Model.ProgramCode)@gov.bc.ca">@(Model.ProgramCode)@gov.bc.ca</a>.</p>
    <p>@Model.ProgramName Team</p>
</body>
</html>', 1, GETUTCDATE(), NULL)
-- 2
, (26, N'@Model.ProgramName File #@Model.FileNumber A grant agreement is ready for your acceptance', N'@Model.ProgramName File #@Model.FileNumber A grant agreement is ready for your acceptance', N'<!DOCTYPE html>
<html>
<head />
<body>
    <p>Hello @Model.RecipientFirstName,</p><p>We are pleased to inform you that your @Model.ProgramName application @Model.FileNumber, "@Model.ProgramTitle", has been assessed and an offer in the form of a Grant Agreement has been added to your file for your acceptance.</p>
    <p>In order to obtain your grant, please Login at <a href="@Model.BaseURL">@Model.BaseURL</a> and review and confirm your acceptance of the Grant Agreement by @Model.AgreementAcceptanceDueDate in order to secure funding.</p>
    <p>PLEASE NOTE: Reimbursement of expenses is conditional upon all requirements being met at the time you submit your claim(s).</p>
    <p>@Model.ProgramName Team</p>
</body>
</html>', 5, GETUTCDATE(), NULL)
-- 3
, (27, N'@Model.ProgramName File #@Model.FileNumber Your agreement is accepted and your application is approved',N'@Model.ProgramName File #@Model.FileNumber Your agreement is accepted and your application is approved', N'<!DOCTYPE html>
<html>
<head />
<body>
    <p>Congratulations @Model.RecipientFirstName,</p><p>Your @Model.ProgramName Agreement #@Model.FileNumber, "@Model.ProgramTitle", is approved.</p>
    <p>You may login to the system at <a href="@Model.BaseURL">@Model.BaseURL</a> and begin inviting participants to submit their Participant Information Forms. Please ensure all Participant Information Forms are submitted by @Model.ParticipantReportDueDate, five days before the training start date.</p>
    <p>Refer to your Grant Agreement Schedule A for your reporting deadlines.</p>
    <p>If you need to change your training start date or request a change to your training provider, login at <a href="@Model.BaseURL">@Model.BaseURL</a> and view your Grant Agreement Schedule A where you can request these changes.</p>
    <p>@Model.ProgramName Team</p>
</body>
</html>
', 5, GETUTCDATE(), NULL)
-- 4
, (28, N'@Model.ProgramName File #@Model.FileNumber Your acceptance of the grant agreement is overdue', N'@Model.ProgramName File #@Model.FileNumber Your acceptance of the grant agreement is overdue', N'<!DOCTYPE html>
<html>
<head />
<body>
    <p>Dear @Model.RecipientFirstName,</p>
    <p>A @Model.ProgramName Agreement #@Model.FileNumber was issued to you for your acceptance on @Model.AgreementIssueDate.</p>
    <p>The due date for your acceptance of this offer (@Model.AgreementAcceptanceDueDate) has passed and the offer may be withdrawn at any time. If you wish to take advantage of the grant you must login at <a href="@Model.BaseURL">@Model.BaseURL</a> and accept the agreement today.</p>
    <p>Failure to accept this offer may result in it being rescinded so funding can be reinvested to support training opportunities for other applicants.</p><p>@Model.ProgramName Team</p>
</body>
</html>
', 5, GETUTCDATE(), NULL)
-- 5
, (29, N'@Model.ProgramName File #@Model.FileNumber Reporting Reminder - Participant Information Forms', N'@Model.ProgramName File #@Model.FileNumber Reporting Reminder - Participant Information Forms', N'<!DOCTYPE html>
<html>
<head />
<body>
    <p>Hello @Model.RecipientFirstName,</p>
    <p>Your delivery start date for Grant Agreement #@Model.FileNumber, "@Model.ProgramTitle", is fast approaching!</p>
    <p>All Participant Information Forms must be submitted by @Model.ParticipantReportDueDate (five days before the training start date). If you have not already done so, login to the system at <a href="@Model.BaseURL">@Model.BaseURL</a> and invite participants to submit their Participant Information Forms.</p>
    <p>Refer to your Schedule A for your reporting deadlines.</p>
    <p>If you need to change your delivery start date or request a change to your training provider, login at <a href="@Model.BaseURL">@Model.BaseURL</a> and view your Grant Agreement Schedule A where you can request these changes.</p>
    <p>@Model.ProgramName Team</p>
</body>
</html>
', 5, GETUTCDATE(), NULL)
-- 6
, (30, N'@Model.ProgramName File #@Model.FileNumber Participant Information Forms are Due Today', N'@Model.ProgramName File #@Model.FileNumber Participant Information Forms are Due Today', N'<!DOCTYPE html><html><head/><body><p>Dear @Model.RecipientFirstName,</p><p >For your Grant Agreement #@Model.FileNumber, "@Model.ProgramTitle", Participant Information Forms are due today and @Model.NumberofParticipants out of a possible @Model.MaximumNumberofParticipants have submitted their Participant Information Forms.</p><p >You will not be able to claim reimbursement for participants who do not submit a Participant Information Form. Please login at <a href="@Model.BaseURL">@Model.BaseURL</a> and check your participant list. Make sure it is complete and accurate.</p><p >If you need to, invite participants to submit their Participant Information Forms today and follow up with them to make sure they understand the purpose of the invitation and what is required.</p><p>@Model.ProgramName Team</p></body></html>', 5, GETUTCDATE(), NULL)
-- 7
, (31, N'@Model.ProgramName File #@Model.FileNumber No Participants have submitted their information', N'@Model.ProgramName File #@Model.FileNumber No Participants have submitted their information', 
N'
<!DOCTYPE html>
<html>
<head />
<body>
    <p>Dear @Model.RecipientFirstName,</p>
    <p>For Grant Agreement #@Model.FileNumber, "@Model.ProgramTitle", your delivery start date has passed and no participants have submitted their Participant Information Forms.</p>
    <p>If the training did not take place, please login at <a href="@Model.BaseURL">@Model.BaseURL</a> and cancel the agreement.</p>
    <p>If training did take place, please login at <a href="@Model.BaseURL">@Model.BaseURL</a> and invite participants to submit their Participant Information Forms today and follow up with them to make sure they understand the purpose of the invitation and what is required.</p>
    <p>You will not be able to claim reimbursement for participants who do not submit a Participant Information Form.</p><p>@Model.ProgramName Team</p>
</body>
</html>
', 5, GETUTCDATE(), NULL)
-- 12
, (32, N'@Model.ProgramName File #@Model.FileNumber Completion Report Reminder', N'@Model.ProgramName File #@Model.FileNumber Completion Report Reminder', 
N'<!DOCTYPE html>
<html>
<head />
<body>
    <p>Dear @Model.RecipientFirstName,</p><p>Your delivery end date has passed for Grant Agreement #@Model.FileNumber, "@Model.ProgramTitle".</p><p>Please login at <a href="@Model.BaseURL">@Model.BaseURL</a> and enter your Completion Report before @Model.CompletionReportDueDate.</p><p>@Model.ProgramName Team</p>
</body>
</html>', 5, GETUTCDATE(), NULL)
-- 13
, (33, N'@Model.ProgramName File #@Model.FileNumber Completion Report is Overdue', N'@Model.ProgramName File #@Model.FileNumber Completion Report is Overdue', 
N'<!DOCTYPE html>
<html>
<head />
<body>
    <p>Dear @Model.RecipientFirstName,</p>
    <p>Your Completion Report deadline has passed for Grant Agreement #@Model.FileNumber, "@Model.ProgramTitle", and only you have not done your Completion Reporting.</p>
    <p>Please login at <a href="@Model.BaseURL">@Model.BaseURL</a> and enter your Completion Report for all participants. Failing to report completion may impact your ability to use the @Model.ProgramName in the future.</p>
    <p>@Model.ProgramName Team</p>
</body>
</html>
', 5, GETUTCDATE(), NULL)
-- 14
, (34, N'@Model.ProgramName File #@Model.FileNumber Change Request Approved', N'@Model.ProgramName File #@Model.FileNumber Change Request Approved', 
N'<!DOCTYPE html>
<html>
<head />
<body>
    <p>Dear @Model.RecipientFirstName,</p><p>Your provider change request for Grant Agreement #@Model.FileNumber, "@Model.ProgramTitle", has been accepted.</p><p>Please Login to the system at: <a href="@Model.BaseURL">@Model.BaseURL</a> and review the change to your grant agreement.</p>
</body>
<p>@Model.ProgramName Team</p>
</html>', 5, GETUTCDATE(), NULL)
-- 15
, (35, N'@Model.ProgramName File #@Model.FileNumber Change Request Denied', N'@Model.ProgramName File #@Model.FileNumber Change Request Denied',
 N'<!DOCTYPE html><html><head/><body><p>Dear @Model.RecipientFirstName,</p><p >Your provider change request for Grant Agreement #@Model.FileNumber, "@Model.ProgramTitle", has been denied for the following reason:</p><p >@Model.CRDeniedReason</p><p >You may Login to the system at: <a href="@Model.BaseURL">@Model.BaseURL</a> to submit another request.</p><p>@Model.ProgramName Team</p></body></html>', 5, GETUTCDATE(), NULL)
-- 16
, (36, N'@Model.ProgramName File #@Model.FileNumber Grant Agreement Cancelled', N'@Model.ProgramName File #@Model.FileNumber Grant Agreement Cancelled', 
N'<!DOCTYPE html><html><head/><body><p>Dear @Model.RecipientFirstName,</p><p >Your @Model.ProgramName Agreement #@Model.FileNumber, "@Model.ProgramTitle", has been cancelled for the following reason:</p><p >@Model.CancellationReason</p><p >The file has been closed. Please login at: <a href="@Model.BaseURL">@Model.BaseURL</a> if you wish to view your file.</p><p>@Model.ProgramName Team</p></body></html>', 5, GETUTCDATE(), NULL)
-- 17
, (37, N'@Model.ProgramName File #@Model.FileNumber Reimbursement Claim Received', N'@Model.ProgramName File #@Model.FileNumber Reimbursement Claim Received',
 N'<!DOCTYPE html><html><head/><body><p>Dear @Model.RecipientFirstName,</p><p >We have received your reimbursement claim and supporting documentation for Grant Agreement #@Model.FileNumber, "@Model.ProgramTitle".</p><p >Your claim has been placed in the queue for processing. Requests are reviewed in the order they are received. Should additional information be required, a program representative may contact you. If you have any questions about your claim, please contact us at <a href="mailto:@(Model.ProgramCode)@gov.bc.ca">@(Model.ProgramCode)@gov.bc.ca</a></p><p>@Model.ProgramName Team</p></body></html>', 5, GETUTCDATE(), NULL)
-- 18
, (38, N'@Model.ProgramName File #@Model.FileNumber Reimbursement Claim is Approved and payment has been requested', N'@Model.ProgramName File #@Model.FileNumber Reimbursement Claim is Approved and payment has been requested', 
N'<!DOCTYPE html><html><head/><body><p>Dear @Model.RecipientFirstName,</p><p >Your reimbursement claim for Grant Agreement #@Model.FileNumber, "@Model.ProgramTitle," has been processed and a reimbursement in the amount of @Model.ReimbursementPayment will be sent to you by electronic funds transfer or by a cheque mailed to the business mailing address you submitted with your application.</p><p >If you have any questions, please contact us at @(Model.ProgramCode)@gov.bc.ca</p><p>@Model.ProgramName Team</p></body></html>', 5, GETUTCDATE(), NULL)
-- 19
, (39, N'@Model.ProgramName File #@Model.FileNumber Reimbursement Claim Denied', N'@Model.ProgramName File #@Model.FileNumber Reimbursement Claim Denied', 
N'<!DOCTYPE html>
<html>
<head />
<body>
    <p>Dear: @Model.RecipientFirstName</p>
    <p>Your @Model.ProgramName claim for Grant Agreement #@Model.FileNumber, "@Model.ProgramTitle,", has been denied for the following reason:</p>
    <p>@Model.ReimbursementClaimDeniedReason </p> 
    <p>Please login at <a href="@Model.BaseURL">@Model.BaseURL</a> to see the details of your assessment.</p>
    <br />@Model.ProgramName Program
</body>
</html>
', 5, GETUTCDATE(), NULL)
-- 20
, (40, N'@Model.ProgramName File #@Model.FileNumber Reimbursement Claim Returned', N'@Model.ProgramName File #@Model.FileNumber Reimbursement Claim Returned',
 N'<!DOCTYPE html><html><head/><body><p>Dear @Model.RecipientFirstName,</p><p >Your @Model.ProgramName reimbursement claim for Grant Agreement #@Model.FileNumber, "@Model.ProgramTitle", has been returned to you for the following reason:</p><p >@Model.ReimbursementClaimReturnedReason</p><p >Please login at <a href="@Model.BaseURL">@Model.BaseURL</a> and adjust your claim to be compliant with your Agreement and resubmit it to the Ministry.</p><p>@Model.ProgramName Team</p></body></html>', 5, GETUTCDATE(), NULL)
-- 21
, (41, N'RE: @Model.ProgramName File #@Model.FileNumber Application Denied', N'RE: @Model.ProgramName File #@Model.FileNumber Application Denied', 
N'<!DOCTYPE html>
<html>
<head />
<body>
    <p>Dear @Model.RecipientFirstName,</p><p>Your @Model.ProgramName application #@Model.FileNumber, for "@Model.ProgramTitle", has been denied for the following reason(s):</p><p>@Model.DeniedReason</p>
    <p>
        Your application file status shows Agreement Withdrawn and the file has been closed.
        Please login at: <a href="@Model.BaseURL">@Model.BaseURL</a> if you wish to view your application.
    </p><p>@Model.ProgramName Team</p>
</body>
</html>', 5, GETUTCDATE(), NULL)
-- 22
, (42, N'@Model.ProgramName File #@Model.FileNumber Grant Agreement Withdrawn', N'@Model.ProgramName File #@Model.FileNumber Grant Agreement Withdrawn', 
N'<!DOCTYPE html><html><head/><body><p>Dear @Model.RecipientFirstName,</p><p >The offer to you for your Canada-BC Job Application #@Model.FileNumber, "@Model.ProgramTitle", has been withdrawn.</p><p >Your application file status shows Agreement Withdrawn and the file has been closed. Please login at: <a href="@Model.BaseURL">@Model.BaseURL</a> if you wish to view your application.</p><p>@Model.ProgramName Team</p></body></html>', 5, GETUTCDATE(), NULL)
-- 23
, (43, N'@Model.ProgramName File #@Model.FileNumber Not Accepted – Please reapply.', N'@Model.ProgramName File #@Model.FileNumber Not Accepted – Please reapply.', 
N'<!DOCTYPE html><html><head/><body><p>Dear @Model.RecipientFirstName,</p><p >Thank you for your @Model.ProgramName Application #@Model.FileNumber, "@Model.ProgramTitle". Your application has not been assessed because all grant funds have been committed for your grant selection: "@Model.FullStreamName" for delivery starting in the period @Model.TrainingPeriodStartDate to @Model.TrainingPeriodEndDate.</p><p >Please reapply for the next available grant opening.</p><p>@Model.ProgramName Team</p></body></html>', 5, GETUTCDATE(), NULL)
-- 24
, (44, N'@Model.ProgramName File #@Model.FileNumber Reimbursement Claim Approved', N'@Model.ProgramName File #@Model.FileNumber Reimbursement Claim Approved', 
N'<!DOCTYPE html><html><head/><body><p>Dear @Model.RecipientFirstName,</p><p >Your @Model.ProgramName reimbursement claim for Grant Agreement #@Model.FileNumber, "@Model.ProgramTitle", has been approved. An explanation of the assessment follows:</p><p >@Model.ReimbursementClaimApprovedReason</p><p >Please login at <a href=''@Model.BaseURL''>@Model.BaseURL</a> to view your assessment.</p><p>@Model.ProgramName Team</p></body></html>', 5, CAST(N'2018-09-17T17:23:26.6166667' AS DateTime2), NULL)
SET IDENTITY_INSERT [dbo].[NotificationTemplates] OFF
