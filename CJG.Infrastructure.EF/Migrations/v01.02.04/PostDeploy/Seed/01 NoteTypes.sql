PRINT 'Inserting [NoteTypes]'

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[NoteTypes] WHERE [Id] = 18))
BEGIN
	SET IDENTITY_INSERT [dbo].[NoteTypes] ON 
	INSERT [dbo].[NoteTypes]
	 ([Id], [IsSystem], [IsActive], [RowSequence], [Caption], [Description]) VALUES
	 (18, 1, 1, 10, N'TD', N'Training Date Change')
	SET IDENTITY_INSERT [dbo].[NoteTypes] OFF
END