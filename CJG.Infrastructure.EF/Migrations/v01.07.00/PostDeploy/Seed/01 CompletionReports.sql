PRINT 'Inserting [Completion Report]'
SET IDENTITY_INSERT [dbo].[CompletionReports] ON
INSERT INTO [dbo].[CompletionReports]
	([Id], [Caption] ,[Description], [IsActive], [EffectiveDate], [DateAdded], [DateUpdated])
     VALUES
    (1, 'Completion Report Caption', 'Completion Report Description', 1, GETUTCDATE(), GETUTCDATE(), NULL)
SET IDENTITY_INSERT [dbo].[CompletionReports] OFF