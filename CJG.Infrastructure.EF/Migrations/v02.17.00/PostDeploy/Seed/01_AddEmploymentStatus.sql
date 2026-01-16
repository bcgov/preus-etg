PRINT 'Start Inserting [EmploymentStatus]'
SET IDENTITY_INSERT [dbo].[EmploymentStatus] ON 

INSERT [dbo].[EmploymentStatus] ([Id], [IsActive], [RowSequence], [Caption], [DateAdded]) 
VALUES (6, 1, 6, N'Work-sharing', GETUTCDATE())

SET IDENTITY_INSERT [dbo].[EmploymentStatus] OFF
PRINT 'Done Inserting [EmploymentStatus]'
