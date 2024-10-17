PRINT 'Inserting [LegalStructures]'
SET IDENTITY_INSERT [dbo].[LegalStructures] ON 
INSERT [dbo].[LegalStructures]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (01, 1, 01, N'Sole Proprietorship')
,(02, 1, 02, N'Band Council')
,(03, 1, 03, N'Partnership')
,(04, 1, 04, N'British Columbia Corporation')
,(05, 1, 05, N'Federal Corporation')
,(06, 1, 06, N'Society')
,(07, 1, 07, N'Unincorporated Association')
,(08, 1, 08, N'British Columbia Cooperative')
,(09, 1, 09, N'Federal Cooperative')
,(10, 1, 10, N'Other')
SET IDENTITY_INSERT [dbo].[LegalStructures] OFF