PRINT 'Updating [NotificationTypes]'
IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[NotificationTypes] WHERE [Id] = 28))
BEGIN
	INSERT [dbo].[NotificationTypes]
	 ([Id], [NotificationTemplateId], [RecipientType], [MilestoneDateOffset], [MilestoneDateName], [NotificationTypeName]) VALUES
	(28, 24, 0,   0, N'ClaimApproved', N'InsClaimApproved')
END
ELSE
BEGIN
	UPDATE [dbo].[NotificationTypes]
	SET [NotificationTemplateId] = 24,
		[RecipientType] = 0,
		[MilestoneDateOffset] = 0,
		[MilestoneDateName] = N'ClaimApproved',
		[NotificationTypeName] = N'InsClaimApproved'
	WHERE [Id] =28
END
