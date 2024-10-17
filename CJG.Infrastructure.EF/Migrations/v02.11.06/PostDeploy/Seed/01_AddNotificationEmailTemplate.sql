PRINT 'Notification Template Changes for Email Template'
BEGIN

	-- Add new Notification Trigger (we will hide this from the UI)
	INSERT INTO dbo.NotificationTriggers (Id, Caption, Description, DateAdded, DateUpdated, IsActive, RowSequence)
	VALUES (3, 'EmailTemplate', 'Notifications that are only used as an email template.', GETUTCDATE(), GETUTCDATE(), 1, 0)

	-- Add new Template body for Notification
	INSERT INTO dbo.NotificationTemplates (EmailSubject, EmailBody, DateAdded, Caption)
	VALUES ('Invitation to Participate in @Model.TrainingProgramTitle - Training Funded by the B.C. Employer Training Grant',
	'Template sourced from code', GETUTCDATE(), 'Participant Invitation Placeholder')

	-- Get Inserted Tmplate Id
	DECLARE @TemplateId INT
	SET @TemplateId = SCOPE_IDENTITY()

	-- Add new Invitation Notification
	INSERT INTO NotificationTypes (MilestoneDateName, MilestoneDateOffset, Caption, DateAdded, [Description], IsActive, 
	NotificationTemplateId, PreviousApplicationState, CurrentApplicationState, 
	NotificationTriggerId, MilestoneDateExpires, ResendDelayDays, ResendRule, ApprovalRule, ParticipantReportRule, ClaimReportRule, CompletionReportRule, RecipientRule)
	VALUES('NotApplicable', 0, 'PIF Invitation', GETUTCDATE(), 'PIF Invitation with personalised invitation link', 1,
	@TemplateId, NULL, NULL,
	3, 0, 0, 1, 0, 0, 0, 0, 0)

END