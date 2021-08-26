PRINT 'Inserting [EducationLevels]'
SET IDENTITY_INSERT [dbo].[EducationLevels] ON 
INSERT [dbo].[EducationLevels]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 1, N'Less than high school')
,(2, 1, 2, N'High school')
,(3, 1, 3, N'Some post-secondary')
,(4, 1, 4, N'Trades certificate or diploma')
,(5, 1, 5, N'Certificate or diploma')
,(6, 1, 6, N'University degree')
SET IDENTITY_INSERT [dbo].[EducationLevels] OFF