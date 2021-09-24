PRINT 'Inserting [TrainingResults]'
SET IDENTITY_INSERT [dbo].[TrainingResults] ON 
INSERT [dbo].[TrainingResults]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 1, N'Increased security (training will ensure participant can maintain employment)')
,(2, 1, 2, N'Increase in pay')
,(3, 1, 3, N'Increase job-related skills')
,(4, 1, 4, N'Promotion to another position')
,(5, 1, 5, N'Move from part-time to full-time employment')
,(6, 1, 6, N'Move from temporary, casual, or seasonal employment to permanent')
SET IDENTITY_INSERT [dbo].[TrainingResults] OFF