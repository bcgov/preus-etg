PRINT 'Starting Update of ETG Training Provider Types for CJG-1230'

PRINT '-- Deactivating Training Provider Types'
UPDATE [dbo].[TrainingProviderTypes]
SET [IsActive] = 0,
[DateUpdated] = GETDATE()
WHERE [Id] IN (3, 8, 2, 9, 10, 12)

PRINT '-- Changing Private Provider Type name'

UPDATE [dbo].[TrainingProviderTypes]
SET Caption = 'B.C. public post-secondary institution',
[DateUpdated] = GETDATE()
WHERE [Id] = 1

UPDATE [dbo].[TrainingProviderTypes]
SET Caption = 'Private training institution',
[DateUpdated] = GETDATE(),
RowSequence = 2
WHERE [Id] = 7


PRINT '-- Inserting New Training Provider Types'

SET IDENTITY_INSERT [dbo].[TrainingProviderTypes] ON 

INSERT [dbo].[TrainingProviderTypes] ([Id], [IsActive], [RowSequence], [Caption], [DateAdded]) 
VALUES (13, 1, 3, N'Indigenous Elders', GETDATE())
 
 INSERT [dbo].[TrainingProviderTypes] ([Id], [IsActive], [RowSequence], [Caption], [DateAdded]) 
 VALUES (14, 1, 4, N'Other', GETDATE())
 
SET IDENTITY_INSERT [dbo].[TrainingProviderTypes] OFF

PRINT 'Done Update of ETG Training Provider Types'
