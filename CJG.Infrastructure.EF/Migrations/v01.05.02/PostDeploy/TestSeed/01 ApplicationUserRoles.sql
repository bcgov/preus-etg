PRINT 'Updating [ApplicationUserRoles]'

-- Change IDIR2 internal user account role from Assessor to Financial Clerk
IF (EXISTS (SELECT TOP 1 * FROM [dbo].[ApplicationUserRoles] WHERE [UserId] = N'7017e627-1e3a-4cc5-b730-ba7b7171cef3'))
BEGIN
	UPDATE [dbo].[ApplicationUserRoles]
	SET [RoleId] = (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'Financial Clerk')
	WHERE [UserId] = N'7017e627-1e3a-4cc5-b730-ba7b7171cef3'
END

