PRINT 'Inserting [PrioritySectors]'
SET IDENTITY_INSERT [dbo].[PrioritySectors] ON 
INSERT [dbo].[PrioritySectors]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (01, 1, 0, N'Aboriginal Peoples and First Nations')
,(02, 1, 0, N'Agrifoods')
,(03, 1, 0, N'Construction')
,(04, 1, 0, N'Forestry')
,(05, 1, 0, N'In Demand Organizations')
,(06, 1, 0, N'Manufacturing')
,(07, 1, 0, N'Mining and Energy')
,(08, 1, 0, N'Natural Gas')
,(09, 1, 0, N'Small Business')
,(10, 1, 0, N'Technology and Green Economy')
,(11, 1, 0, N'Tourism')
,(12, 1, 0, N'Transportation')
SET IDENTITY_INSERT [dbo].[PrioritySectors] OFF