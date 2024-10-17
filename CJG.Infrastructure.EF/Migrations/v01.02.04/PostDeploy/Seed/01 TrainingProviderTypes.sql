PRINT 'Inserting [TrainingProviderTypes]'

SET IDENTITY_INSERT [dbo].[TrainingProviderTypes] ON 
INSERT [dbo].[TrainingProviderTypes]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (9, 1, 10, N'Industry Association')
,(10, 1, 11, N'Small Business BC')
,(11, 1, 4, N'Technical School - non-ITA Designated')
SET IDENTITY_INSERT [dbo].[TrainingProviderTypes] OFF

PRINT 'Updating [TrainingProviderTypes]'

UPDATE [dbo].[TrainingProviderTypes]
SET [RowSequence] = 1
	,[Caption] = N'BC Public Post-Secondary Institution'
WHERE [Id] = 1

UPDATE [dbo].[TrainingProviderTypes]
SET [RowSequence] = 2
	,[Caption] = N'Training Organization operated by a BC School District'
WHERE [Id] = 2

UPDATE [dbo].[TrainingProviderTypes]
SET [RowSequence] = 3
	,[Caption] = N'Industry Training Authority Designated Trade School'
WHERE [Id] = 3

UPDATE [dbo].[TrainingProviderTypes]
SET [RowSequence] = 5
	,[Caption] = N'Union Hall or Training Board'
WHERE [Id] = 4

UPDATE [dbo].[TrainingProviderTypes]
SET [RowSequence] = 6
	,[Caption] = N'Industry Recognized Safety Trainer'
WHERE [Id] = 5

UPDATE [dbo].[TrainingProviderTypes]
SET [RowSequence] = 7
	,[Caption] = N'BC Private Post-Secondary Institution - Certified by Private Training Institutions Branch'
WHERE [Id] = 6

UPDATE [dbo].[TrainingProviderTypes]
SET [RowSequence] = 8
	,[Caption] = N'Private Training Provider - Certified by Private Training Institutions Branch'
WHERE [Id] = 7

UPDATE [dbo].[TrainingProviderTypes]
SET [RowSequence] = 9
	,[Caption] = N'Private Training Provider - Not Certified by Private Training Institutions Branch'
WHERE [Id] = 8


