PRINT 'Inserting [DeliveryMethods]'
SET IDENTITY_INSERT [dbo].[DeliveryMethods] ON 
INSERT [dbo].[DeliveryMethods]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 1, N'Class Room')
,(2, 1, 2, N'Workplace')
,(3, 1, 3, N'Online')
SET IDENTITY_INSERT [dbo].[DeliveryMethods] OFF