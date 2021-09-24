PRINT 'Inserting [ExpectedQualifications]'
SET IDENTITY_INSERT [dbo].[ExpectedQualifications] ON 
INSERT [dbo].[ExpectedQualifications]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 1, N'Educational')
,(2, 1, 2, N'Industry/Occupation (less than 10 hours)')
,(3, 1, 3, N'Industry/Occupation (more than 10 hours)')
,(4, 1, 4, N'Proprietary (firm issued)')
,(5, 1, 5, N'None')
SET IDENTITY_INSERT [dbo].[ExpectedQualifications] OFF