PRINT 'Updating [NotificationTemplates]'
IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[NotificationTemplates] WHERE [Id] = 21))
BEGIN
	SET IDENTITY_INSERT [dbo].[NotificationTemplates] ON 
	INSERT [dbo].[NotificationTemplates]
	([Id], [DefaultExpiryDays], [AlertCaption], [EmailSubject], [EmailBody]) VALUES 
	(21, 5, N'RE: Canada-BC Job Grant File #::FileNumber:: Application Denied', N'RE: Canada-BC Job Grant File #::FileNumber:: Application Denied', N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >Your Canada-BC Job Grant application #::FileNumber::, for "::TrainingProgramTitle::",  has been assessed and denied for the following reason(s):</p><p >::DeniedReason::</p><p >Your denied application can be viewed at <a href=''::BaseURL::''>skillstraininggrants.gov.bc.ca</a>.</p><p>Canada-BC Job Grant Team</p></body></html>')
	SET IDENTITY_INSERT [dbo].[NotificationTemplates] OFF
END
ELSE
BEGIN
	UPDATE [dbo].[NotificationTemplates]
	SET [DefaultExpiryDays] = 5,
		[AlertCaption] = N'RE: Canada-BC Job Grant File #::FileNumber:: Application Denied',
		[EmailSubject] = N'RE: Canada-BC Job Grant File #::FileNumber:: Application Denied',
		[EmailBody] = N'<!DOCTYPE html><html><head/><body><p>Dear ::AAFirstName::,</p><p >Your Canada-BC Job Grant application #::FileNumber::, for "::TrainingProgramTitle::",  has been assessed and denied for the following reason(s):</p><p >::DeniedReason::</p><p >Your denied application can be viewed at <a href=''::BaseURL::''>skillstraininggrants.gov.bc.ca</a>.</p><p>Canada-BC Job Grant Team</p></body></html>'
	WHERE [Id] = 21
END