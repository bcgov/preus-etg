PRINT 'Inserting [Users]'

-- External Users
IF (NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [BCeIDGuid] = N'02E4D93E-9D19-4F8A-92B1-2546E0201A10'))
BEGIN
	INSERT [dbo].[Users]
		([AccountType], [BCeIDGuid], [Salutation], [FirstName], [LastName], [EmailAddress], [JobTitle], [OrganizationId], [IsOrganizationProfileAdministrator], [IsSubscriberToEmail], [DateAdded], [DateUpdated]) VALUES
		(0, N'02E4D93E-9D19-4F8A-92B1-2546E0201A10', N'Mr.', N'Nicoleta', N'Turtureanu', N'Nicoleta.Turtureanu@gov.bc.ca', 'Program Director', (SELECT TOP 1 Id fROM [dbo].[Organizations] WHERE [BCeIDGuid] = N'3ad433a8-614c-4484-912c-854e17ff6228'), 0, 0, GETUTCDATE(), GETUTCDATE())
END
