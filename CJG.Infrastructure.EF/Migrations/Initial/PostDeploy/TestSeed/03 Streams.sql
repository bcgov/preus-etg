PRINT 'Inserting [Streams]'
SET IDENTITY_INSERT [dbo].[Streams] ON
INSERT [dbo].[Streams]
 ([Id], [MaxReimbursementAmt], [ReimbursementRate], [DefaultDeniedRate], [DefaultWithdrawnRate], [DefaultReductionRate], [DefaultSlippageRate], [DefaultCancellationRate], [Name]) VALUES
 (1, 10000, 0.666666667, 0.051, 0.045, 0.025, 0.165, 0.035, N'Priority Sectors')
,(2, 15000, 1.000000000, 0.041, 0.044, 0.035, 0.126, 0.034, N'Unemployed')
,(3, 10000, 0.666666667, 0.031, 0.043, 0.045, 0.136, 0.033, N'Underrepresented Groups in the Workforce')
,(4, 10000, 0.666666667, 0.021, 0.042, 0.055, 0.145, 0.032, N'Rural')
SET IDENTITY_INSERT [dbo].[Streams] OFF