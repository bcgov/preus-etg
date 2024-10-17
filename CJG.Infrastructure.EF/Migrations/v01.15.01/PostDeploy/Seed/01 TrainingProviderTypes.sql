PRINT 'Updating [TrainingProviderTypes]'

UPDATE [dbo].[TrainingProviderTypes]
SET [Caption] = N'B.C. Public Post-Secondary Institution',
[DateUpdated] = GETDATE()
WHERE [Id] = 1

UPDATE [dbo].[TrainingProviderTypes]
SET [Caption] = N'Training organization run by a B.C. school district',
[RowSequence] = 6,
[DateUpdated] = GETDATE()
WHERE [Id] = 2

UPDATE [dbo].[TrainingProviderTypes]
SET [Caption] = N'Trade or technical school designated by the Industry Training Authority',
[RowSequence] = 2,
[DateUpdated] = GETDATE()
WHERE [Id] = 3

UPDATE [dbo].[TrainingProviderTypes]
SET [IsActive] = 0,
[DateUpdated] = GETDATE()
WHERE [Id] = 4

UPDATE [dbo].[TrainingProviderTypes]
SET [IsActive] = 0,
[DateUpdated] = GETDATE()
WHERE [Id] = 5

UPDATE [dbo].[TrainingProviderTypes]
SET [IsActive] = 0,
[DateUpdated] = GETDATE()
WHERE [Id] = 6

UPDATE [dbo].[TrainingProviderTypes]
SET [Caption] = N'Private training institution certified by the Private Training Institutions Branch.',
[RowSequence] = 4,
[DateUpdated] = GETDATE()
WHERE [Id] = 7

UPDATE [dbo].[TrainingProviderTypes]
SET [Caption] = N'Private training institution not certified by the Private Training Institutions Branch.',
[RowSequence] = 5,
[DateUpdated] = GETDATE()
WHERE [Id] = 8

UPDATE [dbo].[TrainingProviderTypes]
SET [Caption] = N'Industry association',
[RowSequence] = 9,
[DateUpdated] = GETDATE()
WHERE [Id] = 9

UPDATE [dbo].[TrainingProviderTypes]
SET [Caption] = N'Small Business B.C.',
[RowSequence] = 10,
[DateUpdated] = GETDATE()
WHERE [Id] = 10

UPDATE [dbo].[TrainingProviderTypes]
SET [IsActive] = 0,
[DateUpdated] = GETDATE()
WHERE [Id] = 11

PRINT 'Inserting [TrainingProviderTypes]'

SET IDENTITY_INSERT [dbo].[TrainingProviderTypes] ON 
INSERT [dbo].[TrainingProviderTypes]
 ([Id], [IsActive], [RowSequence], [Caption], [DateAdded]) VALUES
 (12, 1, 12, N'In-house training', GETDATE())
SET IDENTITY_INSERT [dbo].[TrainingProviderTypes] OFF





