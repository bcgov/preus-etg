PRINT 'Inserting [EmploymentStatus]'
SET IDENTITY_INSERT [dbo].[EmploymentStatus] ON 
INSERT [dbo].[EmploymentStatus]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 1, N'Unemployed')
,(2, 1, 2, N'Self-employed')
,(3, 1, 3, N'Employed')
,(4, 1, 4, N'Not in labour force')
,(5, 1, 5, N'In school or training')
SET IDENTITY_INSERT [dbo].[EmploymentStatus] OFF