PRINT 'Inserting [CanadianStatus]'
SET IDENTITY_INSERT [dbo].[CanadianStatus] ON 
INSERT [dbo].[CanadianStatus]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 1, N'Canadian Citizen')
,(2, 1, 2, N'Permanent Resident')
,(3, 1, 3, N'A protected person entitled to work in Canada')
,(4, 1, 4, N'None of these')
SET IDENTITY_INSERT [dbo].[CanadianStatus] OFF