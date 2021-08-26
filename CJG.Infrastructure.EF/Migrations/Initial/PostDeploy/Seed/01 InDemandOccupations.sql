PRINT 'Inserting [InDemandOccupations]'
SET IDENTITY_INSERT [dbo].[InDemandOccupations] ON 
INSERT [dbo].[InDemandOccupations]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (01, 1, 0, N'Carpenters')
,(02, 1, 0, N'Gasfitters')
,(03, 1, 0, N'Electricians')
,(04, 1, 0, N'Heavy Duty Equipment Mechanic')
,(05, 1, 0, N'Heavy Equipment Operators')
,(06, 1, 0, N'Machinist')
,(07, 1, 0, N'Millwrights')
,(08, 1, 0, N'Plumbers')
,(09, 1, 0, N'Sheet Metal Workers')
,(10, 1, 0, N'Steamfitters, Pipefitters, and Sprinkler System Installers')
,(11, 1, 0, N'Industrial Electricians')
,(12, 1, 0, N'Crane Operators')
,(13, 1, 0, N'Concrete Finishers')
,(14, 1, 0, N'Cooks')
,(15, 1, 0, N'Bakers')
,(16, 1, 0, N'Welders')
SET IDENTITY_INSERT [dbo].[InDemandOccupations] OFF