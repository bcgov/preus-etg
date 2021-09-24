PRINT 'Update [NotificationTemplates] - Add/Update templates'

-- Update ETG and CJG notification templates.

SET IDENTITY_INSERT [dbo].[NotificationTemplates] ON

DECLARE @MaxNotificationTemplateId INT
SELECT @MaxNotificationTemplateId = MAX([Id]) FROM dbo.[NotificationTemplates]

INSERT INTO dbo.[NotificationTypes] (
	[Id]
	, [Caption]
	, [Description]
	, [MilestoneDateName]
	, [MilestoneDateOffset]
	, [IsActive]
	, [RowSequence]
	, [DateAdded]
	, [NotificationTemplateId] -- Temporarily added, as this column is being deleted in this release.
	, [RecipientType] -- Temporarily added, as this column is being deleted in this release.
) VALUES (
	29 -- ClaimReportingPastDueAllParticipants30
	, N'Claim reporting past due with all participants reported - 30'
	, N'Claim has not been received 30 days after training start date AND all participants have been reported.'
	, N'TrainingStartDate'
	, 31
	, 1
	, 0
	, GETUTCDATE()
	, 1
	, 0
), (
	16 -- ClaimReportingPastDueNoParticipants60
	, N'Claim reporting past due with no participants reported - 60'
	, N'Claim has not been received 60 days after training start date AND no participants have been reported.'
	, N'TrainingStartDate'
	, 61
	, 1
	, 0
	, GETUTCDATE()
	, 1
	, 0
), (
	18 -- ClaimReportingPastDueNoParticipants90
	, N'Claim reporting past due with no participants reported - 90'
	, N'Claim has not been received 90 days after training start date AND no participants have been reported.'
	, N'TrainingStartDate'
	, 91
	, 1
	, 0
	, GETUTCDATE()
	, 1
	, 0
), (
	17 -- ClaimReportingPastDueSomeParticipants60
	, N'Claim reporting past due with some participants reported - 60'
	, N'Claim has not been received 60 days after training start date AND 1 or more participants have been reported.'
	, N'TrainingStartDate'
	, 61
	, 1
	, 0
	, GETUTCDATE()
	, 1
	, 0
), (
	19 -- ClaimReportingPastDueSomeParticipants90
	, N'Claim reporting past due with some participants reported - 90'
	, N'Claim has not been received 90 days after training start date AND 1 or more participants have been reported.'
	, N'TrainingStartDate'
	, 91
	, 1
	, 0
	, GETUTCDATE()
	, 1
	, 0
)

INSERT INTO dbo.[NotificationTemplates] (
	[Id]
	, [AlertCaption]
	, [EmailSubject]
	, [EmailBody]
	, [DefaultExpiryDays]
	, [DateAdded]
) VALUES (
	@MaxNotificationTemplateId + 1 -- ClaimReportingPastDueAllParticipants30 = 29
	, N'#@Model.FileNumber Reimbursement Claim is Overdue'
	, N'#@Model.FileNumber Reimbursement Claim is Overdue'
	, N'<!DOCTYPE html><html><head/><body>
<p>Dear @Model.RecipientFirstName</p>
<p>Your reimbursement claim for Grant Agreement <b>#@Model.FileNumber</b>, "<b>@Model.TrainingProgramTitle</b>", has not been received by the deadline in the Grant Agreement.</p>
<p>If training has started and you intend to submit a claim, please log into <a href="@Model.BaseURL">@Model.BaseURL</a> to <b>submit your claim today</b>.  If you have decided not to proceed with training, please cancel your agreement by logging into <a href="@Model.BaseURL">@Model.BaseURL</a> or contact the program immediately at <a href="mailto:@(Model.ProgramCode)@("@gov.bc.ca")">@(Model.ProgramCode)@("@gov.bc.ca")</a>.</p>
<p><b>As you have not met the deadline for claim submission in your Grant agreement, the Ministry may cancel your Grant Agreement at any time to reinvest funding for other employer training opportunities.</b></p>
<p>@Model.ProgramName Grant</p></body></html>'
	, 5
	, GETUTCDATE()
), (
	@MaxNotificationTemplateId + 2 -- ClaimReportingPastDueNoParticipants60 = 16
	, N'#@Model.FileNumber Reimbursement Claim is Overdue'
	, N'#@Model.FileNumber Reimbursement Claim is Overdue'
	, N'<!DOCTYPE html><html><head/><body>
<p>Dear @Model.RecipientFirstName</p>
<p>Your reimbursement claim for Grant Agreement <b>#@Model.FileNumber</b>, "<b>@Model.TrainingProgramTitle</b>", has not been received by the deadline in the Grant Agreement and <b>no Participant Information Forms have been submitted</b>.</p>
<p>If training has started and you intend to submit a claim, please ensure all Participants who are taking training have completed their Participant Information Forms, and then log into <a href="@Model.BaseURL">@Model.BaseURL</a> to <b>submit your claim today</b>.  If you have decided not to proceed with training, please cancel your agreement by logging into <a href="@Model.BaseURL">@Model.BaseURL</a> or contact the program immediately at <a href="mailto:@(Model.ProgramCode)@("@gov.bc.ca")">@(Model.ProgramCode)@("@gov.bc.ca")</a>.</p>
<p>As you have not met the deadline for claim submission in your Grant agreement, the Ministry may cancel your Grant Agreement at any time to reinvest funding for other employer training opportunities.</p>
<p><b>Note: Should you fail to submit a claim within the next 30 days or to contact the Ministry to discuss the status of your Agreement, your Agreement will be cancelled.</b></p>
<p>@Model.ProgramName Grant</p></body></html>'
	, 5
	, GETUTCDATE()
), (
	@MaxNotificationTemplateId + 3 -- ClaimReportingPastDueNoParticipants90 = 18
	, N'#@Model.FileNumber Reimbursement Claim is Overdue'
	, N'#@Model.FileNumber Reimbursement Claim is Overdue'
	, N'<!DOCTYPE html><html><head/><body>
<p>Dear @Model.RecipientFirstName</p>
<p>Your reimbursement claim for Grant Agreement <b>#@Model.FileNumber</b>, "<b>@Model.TrainingProgramTitle</b>", has not been received by the deadline in the Grant Agreement and <b>no Participant Information Forms have been submitted</b>.</p>
<p>If training has started and you intend to submit a claim, please ensure all Participants who are taking training have completed their Participant Information Forms, and then log into <a href="@Model.BaseURL">@Model.BaseURL</a> to <b>submit your claim today</b>.  If you have decided not to proceed with training, please cancel your agreement by logging into <a href="@Model.BaseURL">@Model.BaseURL</a> or contact the program immediately at <a href="mailto:@(Model.ProgramCode)@("@gov.bc.ca")">@(Model.ProgramCode)@("@gov.bc.ca")</a>.</p>
<p>As you have not met the deadline for claim submission in your Grant agreement, the Ministry may cancel your Grant Agreement at any time to reinvest funding for other employer training opportunities.</p>
<p><b>As you have not met the deadline for claim submission in your Grant agreement, the Ministry will be cancelling your Grant Agreement within 10 days of this notification, in order to reinvest funding for other employer training opportunities.</b></p>
<p>@Model.ProgramName Grant</p></body></html>'
	, 5
	, GETUTCDATE()
), (
	@MaxNotificationTemplateId + 4 -- ClaimReportingPastDueSomeParticipants60 = 17
	, N'#@Model.FileNumber Reimbursement Claim is Overdue'
	, N'#@Model.FileNumber Reimbursement Claim is Overdue'
	, N'<!DOCTYPE html><html><head/><body>
<p>Dear @Model.RecipientFirstName</p>
<p>Your reimbursement claim for Grant Agreement <b>#@Model.FileNumber</b>, "<b>@Model.TrainingProgramTitle</b>", has not been received by the deadline in the Grant Agreement and <b>no Participant Information Forms have been submitted</b>.</p>
<p>If training has started and you intend to submit a claim, please ensure all Participants who are taking training have completed their Participant Information Forms, and then log into <a href="@Model.BaseURL">@Model.BaseURL</a> to <b>submit your claim today</b>.  If you have decided not to proceed with training, please cancel your agreement by logging into <a href="@Model.BaseURL">@Model.BaseURL</a> or contact the program immediately at <a href="mailto:@(Model.ProgramCode)@("@gov.bc.ca")">@(Model.ProgramCode)@("@gov.bc.ca")</a>.</p>
<p>As you have not met the deadline for claim submission in your Grant agreement, the Ministry may cancel your Grant Agreement at any time to reinvest funding for other employer training opportunities.</p>
<p><b>Note: Should you fail to submit a claim within the next 30 days or to contact the Ministry to discuss the status of your Agreement, your Agreement will be cancelled.</b></p>
<p>@Model.ProgramName Grant</p></body></html>'
	, 5
	, GETUTCDATE()
), (
	@MaxNotificationTemplateId + 5 -- ClaimReportingPastDueSomeParticipants90 = 19
	, N'#@Model.FileNumber Reimbursement Claim is Overdue'
	, N'#@Model.FileNumber Reimbursement Claim is Overdue'
	, N'<!DOCTYPE html><html><head/><body>
<p>Dear @Model.RecipientFirstName</p>
<p>Your reimbursement claim for Grant Agreement <b>#@Model.FileNumber</b>, "<b>@Model.TrainingProgramTitle</b>", has not been received by the deadline in the Grant Agreement and <b>no Participant Information Forms have been submitted</b>.</p>
<p>If training has started and you intend to submit a claim, please ensure all Participants who are taking training have completed their Participant Information Forms, and then log into <a href="@Model.BaseURL">@Model.BaseURL</a> to <b>submit your claim today</b>.  If you have decided not to proceed with training, please cancel your agreement by logging into <a href="@Model.BaseURL">@Model.BaseURL</a> or contact the program immediately at <a href="mailto:@(Model.ProgramCode)@("@gov.bc.ca")">@(Model.ProgramCode)@("@gov.bc.ca")</a>.</p>
<p><b>As you have not met the deadline for claim submission in your Grant agreement, the Ministry will be cancelling your Grant Agreement within 10 days of this notification</b>, in order to reinvest funding for other employer training opportunities.</p>
<p>@Model.ProgramName Grant</p></body></html>'
	, 5
	, GETUTCDATE()
)

INSERT INTO dbo.[GrantProgramNotifications] (
	[GrantProgramId]
	, [NotificationTemplateId]
	, [NotificationTypeId]
	, [IsActive]
	, [DateAdded]
) VALUES (
	1
	, @MaxNotificationTemplateId + 1
	, 29
	, 1
	, GETUTCDATE()
), (
	1
	, @MaxNotificationTemplateId + 2
	, 16
	, 1
	, GETUTCDATE()
), (
	1
	, @MaxNotificationTemplateId + 3
	, 18
	, 1
	, GETUTCDATE()
), (
	1
	, @MaxNotificationTemplateId + 4
	, 17
	, 1
	, GETUTCDATE()
), (
	1
	, @MaxNotificationTemplateId + 5
	, 19
	, 1
	, GETUTCDATE()
), (
	2
	, @MaxNotificationTemplateId + 1
	, 29
	, 1
	, GETUTCDATE()
), (
	2
	, @MaxNotificationTemplateId + 2
	, 16
	, 1
	, GETUTCDATE()
), (
	2
	, @MaxNotificationTemplateId + 3
	, 18
	, 1
	, GETUTCDATE()
), (
	2
	, @MaxNotificationTemplateId + 4
	, 17
	, 1
	, GETUTCDATE()
), (
	2
	, @MaxNotificationTemplateId + 5
	, 19
	, 1
	, GETUTCDATE()
)

SET IDENTITY_INSERT [dbo].[NotificationTemplates] OFF

-- ClaimReportingPastDueNoParticipants30
UPDATE t
SET t.EmailSubject = N'#@Model.FileNumber Reimbursement Claim is Overdue'
	, t.EmailBody = N'<!DOCTYPE html><html><head/><body>
<p>Dear @Model.RecipientFirstName</p>
<p>Your reimbursement claim for Grant Agreement <b>#@Model.FileNumber</b>, "<b>@Model.TrainingProgramTitle</b>", has not been received by the deadline in the Grant Agreement and <b>no Participant Information Forms have been submitted</b>.</p>
<p>If training has started and you intend to submit a claim, please ensure all Participants who are taking training have completed their Participant Information Forms, and then log into <a href="@Model.BaseURL">@Model.BaseURL</a> to <b>submit your claim today</b>.  If you have decided not to proceed with training, please cancel your agreement by logging into <a href="@Model.BaseURL">@Model.BaseURL</a> or contact the program immediately at <a href="mailto:@(Model.ProgramCode)@("@gov.bc.ca")">@(Model.ProgramCode)@("@gov.bc.ca")</a>.</p>
<p><b>As you have not met the deadline for claim submission in your Grant agreement, the Ministry may cancel your Grant Agreement at any time to reinvest funding for other employer training opportunities.</b></p>
<p>@Model.ProgramName Grant</p></body></html>'
FROM dbo.[NotificationTemplates] t
JOIN dbo.[GrantProgramNotifications] gpn ON t.Id = gpn.NotificationTemplateId
WHERE gpn.NotificationTypeId = 10
	AND gpn.GrantProgramId IN (1, 2)

-- ClaimReportingPastDueSomeParticipants30
UPDATE t
SET t.EmailSubject = N'#@Model.FileNumber Reimbursement Claim is Overdue'
	, t.EmailBody = N'<!DOCTYPE html><html><head/><body>
<p>Dear @Model.RecipientFirstName</p>
<p>Your reimbursement claim for Grant Agreement <b>#@Model.FileNumber</b>, "<b>@Model.TrainingProgramTitle</b>", has not been received by the deadline in the Grant Agreement.</p>
<p>According to your Agreement, not all of your participants have completed their Participant Information Forms. Note: You will not be eligible to claim reimbursement for participants who do not submit Participant Information Forms.</p>
<p>If training has started and you intend to submit a claim, please ensure all Participants who are taking training have completed their Participant Information Forms, and then log into <a href="@Model.BaseURL">@Model.BaseURL</a> to <b>submit your claim today</b>.  If you have decided not to proceed with training, please cancel your agreement by logging into <a href="@Model.BaseURL">@Model.BaseURL</a> or contact the program immediately at <a href="mailto:@(Model.ProgramCode)@("@gov.bc.ca")">@(Model.ProgramCode)@("@gov.bc.ca")</a>.  If not all participants will be taking training, please advise the Ministry immediately.</p>
<p><b>As you have not met the deadline for claim submission in your Grant agreement, the Ministry may cancel your Grant Agreement at any time to reinvest funding for other employer training opportunities.</b></p>
<p>@Model.ProgramName Grant</p></body></html>'
FROM dbo.[NotificationTemplates] t
JOIN dbo.[GrantProgramNotifications] gpn ON t.Id = gpn.NotificationTemplateId
WHERE gpn.NotificationTypeId = 11
	AND gpn.GrantProgramId IN (1, 2)
