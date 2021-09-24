PRINT 'Inserting [DeliveryPartners]'
SET IDENTITY_INSERT [dbo].[DeliveryPartners] ON 
INSERT [dbo].[DeliveryPartners]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 0, N'Small Business BC')
,(2, 1, 0, N'ASPECT')
,(3, 1, 0, N'Back in Motion')
,(4, 1, 0, N'BCAAFC')
,(5, 1, 0, N'Bowman')
,(6, 1, 0, N'Northern Interior Woodworkers Association')
,(7, 1, 0, N'BC Alliance for Manufacturing')
,(8, 1, 0, N'YMCA')
SET IDENTITY_INSERT [dbo].[DeliveryPartners] OFF