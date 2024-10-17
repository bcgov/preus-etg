PRINT 'Inserting [OrganizationTypes]'
SET IDENTITY_INSERT [dbo].[OrganizationTypes] ON 
INSERT [dbo].[OrganizationTypes]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 0, N'Non-profit industry association or society')
,(2, 1, 0, N'Non-profit employer association or employer consortium')
,(3, 1, 0, N'Non-profit organization or association')
,(4, 1, 0, N'Union hall or joint training board')
,(5, 1, 0, N'Aboriginal organization')
SET IDENTITY_INSERT [dbo].[OrganizationTypes] OFF