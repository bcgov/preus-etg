PRINT 'Inserting [FederalOfficialLanguages]'

INSERT [dbo].[FederalOfficialLanguages]
 ([IsActive], [RowSequence], [Caption], [DateAdded]) VALUES
 (1, 1, N'English', GETUTCDATE())
,(1, 2, N'French', GETUTCDATE())
,(1, 3, N'English and French', GETUTCDATE())
,(1, 4, N'Not a federal official language', GETUTCDATE())


CHECKPOINT