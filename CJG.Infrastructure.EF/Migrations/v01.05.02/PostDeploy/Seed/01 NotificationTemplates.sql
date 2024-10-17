PRINT 'Updating [NotificationTemplates]'
IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[NotificationTemplates] WHERE [Id] = 24))
BEGIN
	SET IDENTITY_INSERT [dbo].[NotificationTemplates] ON 
	INSERT [dbo].[NotificationTemplates]
	([Id], [DefaultExpiryDays], [AlertCaption], [EmailSubject], [EmailBody]) VALUES 
	(24, 5, N'Canada-BC Job Grant File #::FileNumber:: Reimbursement Claim Approved', N'Canada-BC Job Grant File #::FileNumber:: Reimbursement Claim Approved', N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >Your Canada-BC Job Grant reimbursement claim for Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", has been approved. An explanation of the assessment follows:</p><p >::ReimbursementClaimApprovedReason::</p><p >You may log in at <a href=''::BaseURL::''>skillstraininggrants.gov.bc.ca</a> to view your assessment.</p><p>Canada-BC Job Grant Team</p></body></html>')
	SET IDENTITY_INSERT [dbo].[NotificationTemplates] OFF
END
ELSE
BEGIN
	UPDATE [dbo].[NotificationTemplates]
	SET [DefaultExpiryDays] = 5,
		[AlertCaption] = N'Canada-BC Job Grant File #::FileNumber:: Reimbursement Claim Approved',
		[EmailSubject] = N'Canada-BC Job Grant File #::FileNumber:: Reimbursement Claim Approved',
		[EmailBody] = N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >Your Canada-BC Job Grant reimbursement claim for Grant Agreement #::FileNumber::, "::TrainingProgramTitle::", has been approved. An explanation of the assessment follows:</p><p >::ReimbursementClaimApprovedReason::</p><p >You may log in at <a href=''::BaseURL::''>skillstraininggrants.gov.bc.ca</a> to view your assessment.</p><p>Canada-BC Job Grant Team</p></body></html>'
	WHERE [Id] = 24
END