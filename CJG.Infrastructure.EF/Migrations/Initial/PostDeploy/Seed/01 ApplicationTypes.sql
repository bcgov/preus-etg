PRINT 'Inserting [ApplicationTypes]'
SET IDENTITY_INSERT [dbo].[ApplicationTypes] ON 
INSERT [dbo].[ApplicationTypes]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 0, N'Employer')
SET IDENTITY_INSERT [dbo].[ApplicationTypes] OFF