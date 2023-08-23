PRINT 'Notification Template Changes for Email Template'

-- Add new Notification Trigger (we will hide this from the UI)
INSERT INTO dbo.NotificationTriggers (Id, Caption, Description, DateAdded, DateUpdated, IsActive, RowSequence)
VALUES (3, 'EmailTemplate', 'Notifications that are only used as an email template.', GETUTCDATE(), GETUTCDATE(), 1, 0)

DECLARE @TemplateId INT
SET @TemplateId = IDENT_CURRENT('NotificationTemplates')

SET IDENTITY_INSERT dbo.NotificationTemplates ON

-- Add new Template body for Notification
INSERT INTO dbo.NotificationTemplates (Id, EmailSubject, EmailBody, DateAdded, Caption)
VALUES (@TemplateId, 'Invitation to Participate in @Model.TrainingProgramTitle - Training Funded by the B.C. Employer Training Grant',
'Template sourced from code', GETUTCDATE(), 'Participant Invitation Placeholder')

SET IDENTITY_INSERT dbo.NotificationTemplates OFF

-- Add new Invitation Notification
INSERT INTO NotificationTypes (MilestoneDateName, MilestoneDateOffset, Caption, DateAdded, [Description], IsActive, 
NotificationTemplateId, PreviousApplicationState, CurrentApplicationState, 
NotificationTriggerId, MilestoneDateExpires, ResendDelayDays, ResendRule, ApprovalRule, ParticipantReportRule, ClaimReportRule, CompletionReportRule, RecipientRule)
VALUES('NotApplicable', 0, 'PIF Invitation', GETUTCDATE(), 'PIF Invitation with personalised invitation link', 1,
@TemplateId, NULL, NULL,
3, 0, 0, 1, 0, 0, 0, 0, 0)

