PRINT 'Deleting [InternalUsers]'

IF (EXISTS (SELECT * FROM [dbo].[InternalUsers] WHERE [IDIR] = 'jefoster'))
BEGIN
	DELETE FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = N'5cc22b21-d0a2-498c-a78d-b9e1bcc91967'
	 
	DELETE FROM [dbo].[ApplicationUsers] WHERE [ApplicationUserId] = N'5cc22b21-d0a2-498c-a78d-b9e1bcc91967'
	 
	DELETE FROM [dbo].[InternalUsers] WHERE [IDIR] = 'jefoster'
END