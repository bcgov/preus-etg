PRINT 'Inserting [StreamCriterias]'
SET IDENTITY_INSERT [dbo].[StreamCriterias] ON
INSERT [dbo].[StreamCriterias]
 ([Id], [Caption], [IsActive], [RowSequence], [Description]) VALUES
 (1, N'1', 1, 1, N'This is the description for criteria Type1')
,(2, N'2', 1, 2, N'This is the description for criteria Type2')
SET IDENTITY_INSERT [dbo].[StreamCriterias] OFF