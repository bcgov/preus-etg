PRINT 'Updating [Users]'
-- External Users
UPDATE [dbo].[Users]
SET [LastName] = N'Moyer'
WHERE [BCeIDGuid] = N'394e765b-e171-4467-a41c-02d7c5e3e2b3'

PRINT 'Inserting [Users]'

IF (NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [BCeIDGuid] = N'2d8702eb-c272-4f8b-93c9-f2ad825ca08a'))
BEGIN
	INSERT [dbo].[Users]
	 ([AccountType], [BCeIDGuid], [Salutation], [FirstName], [LastName], [EmailAddress], [JobTitle], [OrganizationId], [IsOrganizationProfileAdministrator], [IsSubscriberToEmail], [PhoneNumber]) VALUES
	 (0, N'2d8702eb-c272-4f8b-93c9-f2ad825ca08a', N'Mr.', N'Chris', N'Clarke', N'Chris.Clarke@gov.bc.ca', 'Program Director', (SELECT TOP 1 Id fROM [dbo].[Organizations] WHERE [BCeIDGuid] = N'3ad433a8-614c-4484-912c-854e17ff6228'), 0, 0, N'(604) 662-8407') -- cclarke1
END

IF (NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [BCeIDGuid] = N'428c86a0-1b20-4425-a971-d5d829831450'))
BEGIN
	INSERT [dbo].[Users]
	 ([AccountType], [BCeIDGuid], [Salutation], [FirstName], [LastName], [EmailAddress], [JobTitle], [OrganizationId], [IsOrganizationProfileAdministrator], [IsSubscriberToEmail], [PhoneNumber]) VALUES
	 (0, N'428c86a0-1b20-4425-a971-d5d829831450', N'Mrs.', N'Genevieve', N'Casault', N'Genevieve.Casault@gov.bc.ca', 'Program Director', (SELECT TOP 1 Id fROM [dbo].[Organizations] WHERE [BCeIDGuid] = N'3ad433a8-614c-4484-912c-854e17ff6228'), 0, 0, N'(604) 662-8407') --gcasault
END

IF (NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [BCeIDGuid] = N'7e155bef-b9e3-4bb3-83c7-4fb57b329d1e'))
BEGIN
	INSERT [dbo].[Addresses]
	 ([AddressLine1], [AddressLine2], [City], [PostalCode], [RegionId], [CountryId]) VALUES
	 (N'949 Tayberry Terrace.',      N'',         N'Victoria',        N'V9C 0E4', N'BC', N'CA')

	INSERT [dbo].[Users]
	 ([AccountType], [BCeIDGuid], [Salutation], [FirstName], [LastName], [EmailAddress], [JobTitle], [PhysicalAddressId], [MailingAddressId], [OrganizationId], [IsOrganizationProfileAdministrator], [IsSubscriberToEmail], [PhoneNumber]) VALUES
	 (0, N'7e155bef-b9e3-4bb3-83c7-4fb57b329d1e', N'Mr.', N'Jeremy', N'Foster', N'jeremy.foster@avocette.com', 'Program Director', SCOPE_IDENTITY(), SCOPE_IDENTITY(), (SELECT TOP 1 Id fROM [dbo].[Organizations] WHERE [BCeIDGuid] = N'3ad433a8-614c-4484-912c-854e17ff6228'), 0, 0, N'(604) 395-6000') -- jfoster1
END

IF (NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [BCeIDGuid] = N'abac32db-32b3-4dd4-a573-3d4a9743c606'))
BEGIN
	INSERT [dbo].[Users]
	 ([AccountType], [BCeIDGuid], [Salutation], [FirstName], [LastName], [EmailAddress], [JobTitle], [OrganizationId], [IsOrganizationProfileAdministrator], [IsSubscriberToEmail], [PhoneNumber]) VALUES
	 (0, N'abac32db-32b3-4dd4-a573-3d4a9743c606', N'Mr.', N'Per', N'Wallenius', N'Per.Wallenius@gov.bc.ca', 'Program Director', (SELECT TOP 1 Id fROM [dbo].[Organizations] WHERE [BCeIDGuid] = N'3ad433a8-614c-4484-912c-854e17ff6228'), 0, 0, N'(604) 395-6000') --pwallenicjg
END

IF (NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [BCeIDGuid] = N'dcff0f71-1430-4554-8500-7397c38a1e4f'))
BEGIN
	INSERT [dbo].[Users]
	 ([AccountType], [BCeIDGuid], [Salutation], [FirstName], [LastName], [EmailAddress], [JobTitle], [OrganizationId], [IsOrganizationProfileAdministrator], [IsSubscriberToEmail], [PhoneNumber]) VALUES
	 (0, N'dcff0f71-1430-4554-8500-7397c38a1e4f', N'Mr.', N'Sushil', N'Bhojwani', N'Sushil.Bhojwani@gov.bc.ca', 'Program Director', (SELECT TOP 1 Id fROM [dbo].[Organizations] WHERE [BCeIDGuid] = N'3ad433a8-614c-4484-912c-854e17ff6228'), 0, 0, N'(604) 395-6000') -- sbhojwani1
END

IF (NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [BCeIDGuid] = N'fb4fbec2-8248-42b6-a117-09ba64cc133b'))
BEGIN
	INSERT [dbo].[Users]
	 ([AccountType], [BCeIDGuid], [Salutation], [FirstName], [LastName], [EmailAddress], [JobTitle], [OrganizationId], [IsOrganizationProfileAdministrator], [IsSubscriberToEmail], [PhoneNumber]) VALUES
	 (0, N'fb4fbec2-8248-42b6-a117-09ba64cc133b', N'Mr.', N'Shane', N'Mantle', N'Shane.Mantle@gov.bc.ca', 'Program Director', (SELECT TOP 1 Id fROM [dbo].[Organizations] WHERE [BCeIDGuid] = N'3ad433a8-614c-4484-912c-854e17ff6228'), 0, 0, N'(604) 395-6000') -- smantle2
END

IF (NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [BCeIDGuid] = N'2b4bd3a4-e75a-4b40-9327-f8347450c757'))
BEGIN
	INSERT [dbo].[Users]
	 ([AccountType], [BCeIDGuid], [Salutation], [FirstName], [LastName], [EmailAddress], [JobTitle], [OrganizationId], [IsOrganizationProfileAdministrator], [IsSubscriberToEmail], [PhoneNumber]) VALUES
	 (0, N'2b4bd3a4-e75a-4b40-9327-f8347450c757', N'Mr.', N'CJG06', N'Test06', N'ian@ccal.ca', 'Program Director', (SELECT TOP 1 Id fROM [dbo].[Organizations] WHERE [BCeIDGuid] = N'3ad433a8-614c-4484-912c-854e17ff6228'), 0, 0, N'(604) 395-6000') -- cjgtest06
END

UPDATE [dbo].[Users] 
SET [EmailAddress] = N'vance.mccoll@avocette.com'
WHERE [BCeIDGuid] = N'729E2A4D-FB99-4D4D-B34E-A08097E37486'