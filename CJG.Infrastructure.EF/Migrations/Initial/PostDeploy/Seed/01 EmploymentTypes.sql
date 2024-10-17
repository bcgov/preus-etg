PRINT 'Inserting [EmploymentTypes]'
SET IDENTITY_INSERT [dbo].[EmploymentTypes] ON 
INSERT [dbo].[EmploymentTypes]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 1, N'Seasonal')
,(2, 1, 2, N'Temporary')
,(3, 1, 3, N'Casual')
,(4, 1, 4, N'Permanent')
SET IDENTITY_INSERT [dbo].[EmploymentTypes] OFF