PRINT 'Inserting [TrainingLevels]'
SET IDENTITY_INSERT [dbo].[TrainingLevels] ON 
INSERT [dbo].[TrainingLevels]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 1, N'Level 1')
,(2, 1, 2, N'Level 2')
,(3, 1, 3, N'Level 3')
,(4, 1, 4, N'Level 4')
SET IDENTITY_INSERT [dbo].[TrainingLevels] OFF