PRINT 'Inserting [EIBenefits]'
SET IDENTITY_INSERT [dbo].[EIBenefits] ON 
INSERT [dbo].[EIBenefits]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 0, N'Currently receiving')
,(2, 1, 0, N'In last Month')
,(3, 1, 0, N'In last 3 months')
,(4, 1, 0, N'In last 36 months(3 years)')
,(5, 1, 0, N'In last 60 months(5 years)')
,(6, 1, 0, N'None of the above')
SET IDENTITY_INSERT [dbo].[EIBenefits] OFF