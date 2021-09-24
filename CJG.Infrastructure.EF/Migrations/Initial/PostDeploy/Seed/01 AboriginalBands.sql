PRINT 'Inserting [AboriginalBands]'
SET IDENTITY_INSERT [dbo].[AboriginalBands] ON 
INSERT [dbo].[AboriginalBands]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 1, N'First Nations')
,(2, 1, 2, N'Métis')
,(3, 1, 3, N'Inuit')
,(4, 1, 4, N'Other')
,(5, 1, 5, N'Prefer not to Answer')
SET IDENTITY_INSERT [dbo].[AboriginalBands] OFF