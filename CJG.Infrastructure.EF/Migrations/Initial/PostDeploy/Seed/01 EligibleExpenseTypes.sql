PRINT 'Inserting [EligibleExpenseTypes]'
SET IDENTITY_INSERT [dbo].[EligibleExpenseTypes] ON 
INSERT [dbo].[EligibleExpenseTypes]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 0, N'Tuition fees')
,(2, 1, 1, N'Mandatory student fees')
,(3, 1, 2, N'Textbooks')
,(4, 1, 3, N'Training software')
,(5, 1, 4, N'Other required materials')
,(6, 1, 5, N'Examination fees')
,(7, 1, 6, N'Travel - Accommodation')
,(8, 1, 7, N'Travel - Transportation')
,(9, 1, 8, N'Travel - Other')
SET IDENTITY_INSERT [dbo].[EligibleExpenseTypes] OFF