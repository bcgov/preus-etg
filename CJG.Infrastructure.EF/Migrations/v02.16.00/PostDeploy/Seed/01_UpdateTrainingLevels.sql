PRINT 'Start Inserting extra [TrainingLevels]'
SET IDENTITY_INSERT [dbo].[TrainingLevels] ON 

UPDATE [dbo].[TrainingLevels]
SET RowSequence = RowSequence + 1,
    DateUpdated = GETUTCDATE()

INSERT [dbo].[TrainingLevels] ([Id], [IsActive], [RowSequence], [Caption], [DateAdded]) 
VALUES
(5, 1, 6, N'Certification Exam', GETUTCDATE()),
(6, 1, 7, N'Endorsement', GETUTCDATE()),
(7, 1, 1, N'Foundation', GETUTCDATE())

SET IDENTITY_INSERT [dbo].[TrainingLevels] OFF

PRINT 'Done Inserting extra [TrainingLevels]'