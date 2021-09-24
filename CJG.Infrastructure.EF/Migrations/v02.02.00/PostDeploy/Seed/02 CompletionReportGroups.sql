PRINT 'Inserting [Completion Report Groups]'

IF (EXISTS (SELECT * FROM [dbo].[CompletionReports] WHERE Id = 2))
AND (NOT EXISTS (SELECT * FROM [dbo].[CompletionReportGroups] WHERE CompletionReportId = 2))
	BEGIN
		SET IDENTITY_INSERT [dbo].[CompletionReportGroups] ON

		INSERT [dbo].[CompletionReportGroups] ([Id], [Title], [DateAdded], [DateUpdated], [RowSequence], [CompletionReportId]) VALUES (6, N'Participant Completion', GETUTCDATE(), NULL, 1, 2)
		INSERT [dbo].[CompletionReportGroups] ([Id], [Title], [DateAdded], [DateUpdated], [RowSequence], [CompletionReportId]) VALUES (7, N'Participant Employment', GETUTCDATE(), NULL, 2, 2)
		INSERT [dbo].[CompletionReportGroups] ([Id], [Title], [DateAdded], [DateUpdated], [RowSequence], [CompletionReportId]) VALUES (8, N'Training Outcomes', GETUTCDATE(), NULL, 3, 2)
		INSERT [dbo].[CompletionReportGroups] ([Id], [Title], [DateAdded], [DateUpdated], [RowSequence], [CompletionReportId]) VALUES (9, N'Community Survey', GETUTCDATE(), NULL, 5, 2)

		SET IDENTITY_INSERT [dbo].[CompletionReportGroups] OFF
	END
ELSE
	BEGIN
		PRINT 'CWRG Completion Report Groups already exist';
	END
PRINT 'Completed Inserting [Completion Report Groups]'
