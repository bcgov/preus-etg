PRINT 'Start Inserting [TrainingObjectives]'

SET IDENTITY_INSERT [dbo].[TrainingObjectives] ON 

INSERT [dbo].[TrainingObjectives] ([Id], [IsActive], [RowSequence], [Caption], [DateAdded])
VALUES
 (1, 1, 1, N'Occupational Skills Training', GETUTCDATE()),
 (2, 1, 2, N'Short-term Training', GETUTCDATE()),
 (3, 1, 3, N'Apprenticeship Training', GETUTCDATE()),
 (4, 1, 4, N'Literacy and Language Training', GETUTCDATE()),
 (10, 1, 5, N'Other', GETUTCDATE())

SET IDENTITY_INSERT [dbo].[TrainingObjectives] OFF

PRINT 'Done Inserting [TrainingObjectives]'