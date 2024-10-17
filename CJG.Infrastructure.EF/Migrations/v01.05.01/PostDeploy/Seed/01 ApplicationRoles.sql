PRINT 'Updating [ApplicationRoles]'
IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[ApplicationRoles] WHERE [Id] = '2d962d82-0810-4f02-86dd-1148151a0861'))
BEGIN
	INSERT [dbo].[ApplicationRoles]
	([Id], [Discriminator], [Name]) VALUES 
	(N'2d962d82-0810-4f02-86dd-1148151a0861', N'ApplicationRole', N'Financial Clerk')
END
ELSE
BEGIN
	UPDATE [dbo].[ApplicationRoles]
	SET [Discriminator] = N'ApplicationRole',
		[Name] = N'Financial Clerk'
	WHERE [Id] = '2d962d82-0810-4f02-86dd-1148151a0861'
END