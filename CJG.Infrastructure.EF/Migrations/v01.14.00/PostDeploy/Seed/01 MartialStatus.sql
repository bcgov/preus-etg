PRINT 'Inserting [MartialStatus]'

INSERT [dbo].[MartialStatus]
 ([IsActive], [RowSequence], [Caption], [DateAdded]) VALUES
 (1, 1, N'Married or equivalent', GETUTCDATE())
,(1, 2, N'Single', GETUTCDATE())
,(1, 3, N'Prefer not to answer', GETUTCDATE())


CHECKPOINT