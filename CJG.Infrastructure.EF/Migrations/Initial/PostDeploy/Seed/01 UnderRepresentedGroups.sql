PRINT 'Inserting [UnderRepresentedGroups]'
SET IDENTITY_INSERT [dbo].[UnderRepresentedGroups] ON 
INSERT [dbo].[UnderRepresentedGroups]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 0, N'Aboriginal or First Nations people')
,(2, 1, 0, N'Youth (Ages 15 to 24)')
,(3, 1, 0, N'Women')
,(4, 1, 0, N'Persons with disabilities')
,(5, 1, 0, N'Recent immigrants (In Canada for 5 years or less)')
SET IDENTITY_INSERT [dbo].[UnderRepresentedGroups] OFF