PRINT 'Inserting [NoteTypes]'


SET IDENTITY_INSERT [dbo].[NoteTypes] ON 
INSERT [dbo].[NoteTypes]
 ([Id], [IsSystem], [IsActive], [RowSequence], [Caption], [Description]) VALUES
 (17, 1, 1, 08, N'AA', N'Assessor Assignment')
SET IDENTITY_INSERT [dbo].[NoteTypes] OFF
