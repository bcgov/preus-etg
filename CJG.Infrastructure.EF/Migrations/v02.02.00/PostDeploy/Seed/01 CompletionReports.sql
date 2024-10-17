PRINT 'Inserting [Completion Report]';

IF (NOT EXISTS (SELECT * FROM [dbo].[CompletionReports] WHERE Id = 2))
	BEGIN
		SET IDENTITY_INSERT [dbo].[CompletionReports] ON;

		INSERT INTO [dbo].[CompletionReports]
			([Id], [Caption] ,[Description], [IsActive], [EffectiveDate], [DateAdded], [DateUpdated])
		 VALUES
			(2, 'CWRG Completion Report', 'CWRG Completion Report', 1, GETUTCDATE(), GETUTCDATE(), NULL);

		SET IDENTITY_INSERT [dbo].[CompletionReports] OFF;
	END
ELSE
	BEGIN
		PRINT 'CWRG Completion Report already exists';
	END
PRINT 'Completed Inserting [Completion Report]';