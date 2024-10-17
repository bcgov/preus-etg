PRINT 'Updating [ExpectedQualifications]'

UPDATE [dbo].[ExpectedQualifications]
SET [IsActive] = 0,
[DateUpdated] = GETDATE()
WHERE [Id] = 1

UPDATE [dbo].[ExpectedQualifications]
SET [IsActive] = 0,
[DateUpdated] = GETDATE()
WHERE [Id] = 2

UPDATE [dbo].[ExpectedQualifications]
SET [IsActive] = 0,
[DateUpdated] = GETDATE()
WHERE [Id] = 3

UPDATE [dbo].[ExpectedQualifications]
SET [IsActive] = 0,
[DateUpdated] = GETDATE()
WHERE [Id] = 4


PRINT 'Inserting [ExpectedQualifications]'

SET IDENTITY_INSERT [dbo].[ExpectedQualifications] ON 

INSERT [dbo].[ExpectedQualifications]
 ([Id], [IsActive], [RowSequence], [Caption], [DateAdded]) VALUES
 (6, 1, 1, N'Trade Certification', GETDATE())
 
 INSERT [dbo].[ExpectedQualifications]
 ([Id], [IsActive], [RowSequence], [Caption], [DateAdded]) VALUES
 (7, 1, 2, N'Occupational Certification', GETDATE())
 
 INSERT [dbo].[ExpectedQualifications]
 ([Id], [IsActive], [RowSequence], [Caption], [DateAdded]) VALUES
 (8, 1, 3, N'Industry-recognized credential', GETDATE())
 
 INSERT [dbo].[ExpectedQualifications]
 ([Id], [IsActive], [RowSequence], [Caption], [DateAdded]) VALUES
 (9, 1, 4, N'Proprietary credential (firm-issued)', GETDATE())
 
SET IDENTITY_INSERT [dbo].[TrainingProviderTypes] OFF





