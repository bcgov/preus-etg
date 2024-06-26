PRINT 'Inserting [ApplicationUserRoles]'

INSERT [dbo].[ApplicationUserRoles]
 ([UserId], [RoleId], [Discriminator]) VALUES

-- Internal UAT Users
 (N'3cfc3f41-93c2-4dda-ae0d-bbf7177ff6e8', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'd42359d1-b297-46b8-946c-30e52df8c575', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'72235ded-3045-4a00-8e08-bfdb47957617', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'51c90c85-3a04-4f6c-84d6-a23701d089c5', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'b7cefa1a-e0fa-4155-a8f6-a6331252740c', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'ce7e601e-80b2-4436-a46f-52e3711c1987', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'74edd439-defc-463d-96b1-3e5b797217df', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'f5d167fc-119e-4734-81b5-e3e85358db0d', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'19086edf-737c-4966-b7f2-d4ed4970d37b', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'ead65bc3-86ca-4f35-a83b-e6fad9b42566', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'2c103774-39a3-49e5-a1de-a0540a5b7f70', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')

-- Internal Team Users
,(N'c52deb65-50a7-4cca-bbe9-902df4209a2c', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'a2ede42f-e4df-48a7-8e06-acdc174a60af', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'54159c2b-8be3-4d70-a4cb-3d7ab53a5ef9', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'5cc22b21-d0a2-498c-a78d-b9e1bcc91967', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'78fdd78b-e018-4a72-8906-b49286065119', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'43a4dcd7-f69b-407b-8667-1a681c24d570', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'08741393-98b7-4e95-b73e-d56d8c7a7b27', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'6756f909-b8a2-40ca-835f-6115df7d001f', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'Assessor'), N'IdentityUserRole')
,(N'7017e627-1e3a-4cc5-b730-ba7b7171cef3', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'Assessor'), N'IdentityUserRole')
,(N'c729532a-b9d9-4175-a386-ed19cc94bf92', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'Director'), N'IdentityUserRole')
,(N'482019c5-ddf9-46c7-aa7b-9ee760fbff82', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'Operations Manager'), N'IdentityUserRole')
,(N'2c3c7700-05d0-43de-93bc-440baf261d81', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'Measurement And Reporting'), N'IdentityUserRole')
,(N'8dbb0c49-cd49-48fa-9512-c80e4d581dfd', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
,(N'4ebb6bdd-228c-42fb-ace8-71cf72501b18', (SELECT TOP 1 [Id] FROM [dbo].[ApplicationRoles] WHERE [Name] = N'System Administrator'), N'IdentityUserRole')
