PRINT 'Inserting [Completion Report Groups]'
SET IDENTITY_INSERT [dbo].[CompletionReportGroups] ON 
INSERT [dbo].[CompletionReportGroups] ([Id], [Title], [DateAdded], [DateUpdated]) VALUES (1, N'Participant Completion', GETUTCDATE(), NULL)
INSERT [dbo].[CompletionReportGroups] ([Id], [Title], [DateAdded], [DateUpdated]) VALUES (2, N'Participant Employment', GETUTCDATE(), NULL)
INSERT [dbo].[CompletionReportGroups] ([Id], [Title], [DateAdded], [DateUpdated]) VALUES (3, N'Training Outcomes', GETUTCDATE(), NULL)
INSERT [dbo].[CompletionReportGroups] ([Id], [Title], [DateAdded], [DateUpdated]) VALUES (4, N'Employer Survey', GETUTCDATE(), NULL)
SET IDENTITY_INSERT [dbo].[CompletionReportGroups] OFF